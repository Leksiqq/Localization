using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.Leksi.Localization;

public class LocalizationBase
{
    private class CultureComparer : IComparer<CultureInfo>
    {
        private static CultureComparer instance = new();
        internal static CultureComparer Instance => instance;
        private CultureComparer() { }
        public int Compare(CultureInfo? x, CultureInfo? y)
        {
            if (string.IsNullOrEmpty(y!.Name) && !string.IsNullOrEmpty(x!.Name))
            {
                return -1;
            }
            else if (!string.IsNullOrEmpty(y!.Name) && string.IsNullOrEmpty(x!.Name))
            {
                return 1;
            }
            else if(x!.Name == y!.Name)
            {
                return 0;
            }
            string[] xParts = GetParts(x!);
            string[] yParts = GetParts(y!);
            int result = xParts[0].CompareTo(yParts[0]);
            if (result != 0)
            {
                return result;
            }
            return xParts[1].CompareTo(yParts[1]);
        }
        private static string[] GetParts(CultureInfo culture)
        {
            int pos = culture.Name.IndexOf('-');
            if(pos > 0)
            {
                return culture.Name.Split('-', 2);
            }
            return [culture.Name, string.Empty];
        }
    }
    private readonly List<ResourceManager> _managers = [];
    private readonly Dictionary<string, ValueHolder> _cachedValues = [];
    private readonly List<CultureInfo> _probeCultureList = [];
    private string? _lastAsk;
    private CultureInfo? _cachedCulture = null;
    private CultureInfo? _culture = null;
    public CultureInfo? Culture
    {
        get => _culture ?? CultureInfo.CurrentUICulture;
        set
        {
            _culture = value;
        }
    }
    public string this[string ask] => GetString(ask);
    public LocalizationBase()
    {
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
    public IEnumerable<ResourceInfo> GetResourceInfo()
    {
        foreach (PropertyInfo pi in GetType().GetProperties().OrderBy(pi => pi.Name))
        {
            if (pi.DeclaringType != typeof(LocalizationBase))
            {
                _lastAsk = null;
                try
                {
                    _ = pi.GetValue(this);
                }
                catch { }
                if (_lastAsk == pi.Name)
                {
                    ValueHolder vh = _cachedValues[pi.Name];
                    yield return new ResourceInfo
                    {
                        Name = pi.Name,
                        Value = vh.Value,
                        BaseName = vh.BaseName,
                        ReturnType = vh.ReturnType,
                        DeclaringType = pi.DeclaringType!,
                        Culture = vh.Culture,
                    };
                }
            }
        }
    }
    public IEnumerable<CultureInfo> GetSupportedCultures()
    {
        CultureInfo? saved = _culture;
        foreach(
            CultureInfo culture 
            in 
            CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Order(CultureComparer.Instance).Append(CultureInfo.InvariantCulture)
        )
        {
            Culture = culture;
            foreach (PropertyInfo pi in GetType().GetProperties().OrderBy(pi => pi.Name))
            {
                if (pi.DeclaringType != typeof(LocalizationBase))
                {
                    _lastAsk = null;
                    try
                    {
                        _ = pi.GetValue(this);
                    }
                    catch { }
                    if (_lastAsk == pi.Name )
                    {
                        ValueHolder vh = _cachedValues[pi.Name];
                        if(vh.Culture == culture)
                        {
                            yield return culture;
                            break;
                        }
                    }
                }
            }
        }
        Culture = saved;
    }
    protected string GetString([CallerMemberName] string ask = null!)
    {
        return Get<string>(ask => $"[{ask}]", ask)!;
    }
    protected object? GetObject([CallerMemberName] string ask = null!)
    {
        return Get<object>(ask => null, ask);
    }
    private T? Get<T>(Func<string,T?> getDefault, string ask)
    {
        _lastAsk = ask;
        CheckCulture();
        if (!_cachedValues.TryGetValue(ask!, out ValueHolder? getter))
        {
            getter = new ValueHolder
            {
                ReturnType = typeof(T),
            };
            foreach (CultureInfo culture in _probeCultureList)
            {
                foreach (ResourceManager rm in _managers)
                {
                    if (rm.GetObject(ask, culture) is T o)
                    {
                        getter.Value = o;
                        getter.BaseName = rm.BaseName;
                        getter.Culture = culture;
                    }
                }
                if (getter.Value is { })
                {
                    break;
                }
            }
            getter.Value ??= getDefault(ask);
            _cachedValues[ask] = getter;
        }
        return (T)getter.Value!;
    }

    private void CheckCulture()
    {
        if (_cachedCulture != Culture)
        {
            _cachedValues.Clear();
            _cachedCulture = Culture!;
            _probeCultureList.Clear();
            _probeCultureList.AddRange(
                CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Where(c => Culture!.Name.StartsWith(c.Name))
                    .OrderBy(c => c.Name).Reverse()
            );
            _probeCultureList.Add(CultureInfo.InvariantCulture);
        }
    }
}
