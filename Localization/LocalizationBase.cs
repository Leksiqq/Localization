using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.Leksi.Localization;

public class LocalizationBase
{
    private readonly List<ResourceManager> _managers = [];
    private readonly Dictionary<string, ValueHolder> _cachedValues = [];
    private readonly List<CultureInfo> _probeCultureList = [];
    private string _lastAsk = null!;
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
    public IEnumerable<ResourceInfo> GetResourceInfo(CultureInfo? cultureInfo = null)
    {
        CultureInfo? savedCultureInfo = null;
        bool cultureChanged = false;
        try
        {
            if (cultureInfo is { } && cultureInfo != Culture)
            {
                savedCultureInfo = _culture;
                Culture = cultureInfo;
                cultureChanged = true;
            }
            foreach (PropertyInfo pi in GetType().GetProperties().OrderBy(pi => pi.Name))
            {
                if (pi.DeclaringType != typeof(LocalizationBase))
                {
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
        finally
        {
            if (cultureChanged)
            {
                Culture = savedCultureInfo;
            }
        }
    }
    protected string GetString([CallerMemberName] string ask = null!)
    {
        _lastAsk = ask;
        CheckCulture();
        if (!_cachedValues.TryGetValue(ask!, out ValueHolder? getter))
        {
            getter = new ValueHolder
            {
                ReturnType = typeof(string),
            };
            foreach(CultureInfo culture in _probeCultureList)
            {
                foreach (ResourceManager rm in _managers)
                {
                    if(
                        rm.GetString(ask!, culture) is string s
                    )
                    {
                        getter.Value = s;
                        getter.BaseName = rm.BaseName;
                        getter.Culture = culture;
                    }
                }
                if (getter.Value is { }) 
                {
                    break;
                }
            }
            getter.Value ??= $"[{ask}]";
            _cachedValues[ask] = getter;
        }
        return (string)getter.Value!;
    }
    protected object? GetObject([CallerMemberName] string ask = null!)
    {
        _lastAsk = ask;
        CheckCulture();
        if (!_cachedValues.TryGetValue(ask!, out ValueHolder? getter))
        {
            getter = new ValueHolder
            {
                ReturnType = typeof(object),
            };
            foreach (CultureInfo culture in _probeCultureList)
            {
                foreach (ResourceManager rm in _managers)
                {
                    if (rm.GetObject(ask, culture) is object o)
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
            _cachedValues[ask] = getter;
        }
        return (string)getter.Value!;
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
