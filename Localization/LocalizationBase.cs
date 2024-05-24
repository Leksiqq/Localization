using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.Leksi.Localization;

public class LocalizationBase
{
    private readonly Dictionary<string, List<ResourceManager>> _managers = [];
    private readonly Dictionary<string, string> _cache = [];
    private CultureInfo _culture;
    public string this[string ask] => GetString(ask);
    public LocalizationBase()
    {
        _culture = CultureInfo.CurrentUICulture;
        Dictionary<Type, List<ResourceHolder>> resourceHolders = [];
        Stack<Type> stack = [];
        for(Type curr = GetType(); curr != typeof(LocalizationBase); curr = curr.BaseType!)
        {
            resourceHolders[curr] = [];
            stack.Push(curr);
        }
        while(stack.TryPop(out Type? type))
        {
            foreach (ResourcePlaceAttribute attr in type.GetCustomAttributes<ResourcePlaceAttribute>(false))
            {
                ResourceHolder rh = new()
                {
                    BaseName = attr.BaseName
                };
                Type targetType = attr.TargetType ?? type;
                rh.Assembly = attr.ResourceAssembly switch {
                    ResourceAssembly.Current or null => type.Assembly,
                    ResourceAssembly.Defining => targetType.Assembly,
                    _ => Assembly.Load(attr.OtherAssemblyFullName ?? throw new Exception())
                };
                resourceHolders[targetType].Add(rh);
            }
        }
        StringBuilder sb = new();
        int pos = 0;
        foreach(KeyValuePair<Type, List<ResourceHolder>> entry in resourceHolders)
        {
            foreach (ResourceHolder rh in entry.Value)
            {
                IEnumerable<string> resources = rh.Assembly.GetManifestResourceNames().Select(v => v[..v.LastIndexOf(".resources")]);
                if (!resources.Contains(rh.BaseName))
                {
                    sb.Append($"\n{++pos}. {entry.Key}:\n    there is no resource '{rh.BaseName}' in assembly {rh.Assembly}");
                    sb.Append($"\n    Use one of [{string.Join(", ", resources)}], or create new one.");
                }
                else
                {
                    rh.ResourceManager = new ResourceManager(rh.BaseName, rh.Assembly);
                }
            }
        }
        foreach(PropertyInfo pi in GetType().GetProperties())
        {
            if(pi.DeclaringType != typeof(LocalizationBase))
            {
                _managers[pi.Name] = resourceHolders[pi.DeclaringType!]!.Select(rh => rh.ResourceManager).ToList();
                if (_managers[pi.Name].Count == 0)
                {
                    sb.Append($"\n{++pos}. {pi.DeclaringType!}:\n    there is no {typeof(ResourcePlaceAttribute)} for type.");
                }
            }
        }
        if (sb.Length > 0)
        {
            sb.AppendLine();
            throw new Exception(sb.ToString());
        }
    }
    protected string GetString([CallerMemberName] string ask = null!)
    {
        if (_culture != CultureInfo.CurrentUICulture)
        {
            _culture = CultureInfo.CurrentUICulture;
            _cache.Clear();
        }
        if (!_cache.TryGetValue(ask!, out string ? ans))
        {
            if (_managers.TryGetValue(ask, out List<ResourceManager>? list))
            {
                foreach (ResourceManager rm in list)
                {
                    if (rm.GetString(ask) is string s)
                    {
                        ans = s;
                    }
                }
            }
            ans ??= $"[{ask}]";
            _cache[ask] = ans;
        }
        return ans;
    }
}
