using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.Leksi.Localization;

public class LocalizationBase
{
    private readonly List<ResourceManager> _managers = [];
    private readonly Dictionary<string, GetterHolder> _getters = [];
    private string _lastAsk = null!;
    public string this[string ask] => GetString(ask);
    public LocalizationBase()
    {
        CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        foreach (CultureInfo culture in allCultures)
        {
            Console.WriteLine(culture);
        }
        Stack<Type> stack = [];
        for (Type curr = GetType(); curr != typeof(LocalizationBase); curr = curr.BaseType!)
        {
            stack.Push(curr);
        }
        StringBuilder sb = new();
        int pos = 0;
        while (stack.TryPop(out Type? type))
        {
            foreach (ResourcePlaceAttribute attr in type.GetCustomAttributes<ResourcePlaceAttribute>(false))
            {
                Assembly ass = attr.OtherAssemblyFullName is { }
                    ? Assembly.Load(attr.OtherAssemblyFullName)
                    : type.Assembly;
                IEnumerable<string> resources = ass.GetManifestResourceNames()
                    .Select(v => v[..v.LastIndexOf(".resources")]);
                if (!resources.Contains(attr.BaseName))
                {
                    sb.Append($"\n{++pos}. There is no resource '{attr.BaseName}' in assembly {ass}");
                    sb.Append($"\n    Use one of [{string.Join(", ", resources)}], or create new one.");
                }
                else
                {
                    _managers.Add(new ResourceManager(attr.BaseName, ass));
                }
            }
        }
        if (sb.Length > 0)
        {
            sb.AppendLine();
            throw new Exception(sb.ToString());
        }
    }
    public IEnumerable<ResourceInfo> GetResourceInfo(CultureInfo? cultureInfo = null)
    {
        CultureInfo? savedCultureInfo = null;
        try
        {
            if (cultureInfo is { } && cultureInfo != CultureInfo.CurrentUICulture)
            {
                savedCultureInfo = CultureInfo.CurrentUICulture;
                CultureInfo.CurrentUICulture = cultureInfo;
            }
            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                if (pi.DeclaringType != typeof(LocalizationBase))
                {
                    object? ans = pi.GetValue(this);
                    if (_lastAsk == pi.Name)
                    {
                        GetterHolder gh = _getters[pi.Name];
                        yield return new ResourceInfo
                        {
                            Name = pi.Name,
                            Value = ans,
                            BaseName = gh.BaseName,
                            ReturnType = gh.ReturnType,
                            DeclaringType = pi.DeclaringType!,
                        };
                    }
                }
            }
        }
        finally 
        {
            if (savedCultureInfo is { })
            {
                CultureInfo.CurrentUICulture = savedCultureInfo;
            }
        }
    }
    protected string GetString([CallerMemberName] string ask = null!)
    {
        _lastAsk = ask;
        string ans = null!;
        if (!_getters.TryGetValue(ask!, out GetterHolder? getter))
        {
            getter = new GetterHolder
            {
                ReturnType = typeof(string),
            };
            foreach (ResourceManager rm in _managers)
            {
                if (rm.GetString(ask) is string s)
                {
                    ans = s;
                    getter.BaseName = rm.BaseName;
                    getter.Getter = arg => rm.GetString(arg);
                }
            }
            getter.Getter ??= arg => $"[{arg}]";
            _getters[ask] = getter;
            ans ??= (string)getter.Getter.Invoke(ask)!;
        }
        else
        {
            ans = (string)getter.Getter.Invoke(ask)!;
        }
        return ans;
    }
    protected object? GetObject([CallerMemberName] string ask = null!)
    {
        _lastAsk = ask;
        object? ans = null;
        if (!_getters.TryGetValue(ask!, out GetterHolder? getter))
        {
            getter = new GetterHolder
            {
                ReturnType = typeof(object),
            };
            foreach (ResourceManager rm in _managers)
            {
                if (rm.GetObject(ask) is object o)
                {
                    ans = o;
                    getter.BaseName = rm.BaseName;
                    getter.Getter = arg => rm.GetObject(arg);
                }
            }
            getter.Getter ??= arg => null;
            _getters[ask] = getter;
        }
        else
        {
            ans = getter.Getter.Invoke(ask);
        }
        return ans;
    }
}
