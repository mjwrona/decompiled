// Decompiled with JetBrains decompiler
// Type: Nest.RoutingResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nest
{
  public class RoutingResolver
  {
    private static readonly ConcurrentDictionary<Type, Func<object, JoinField>> PropertyGetDelegates = new ConcurrentDictionary<Type, Func<object, JoinField>>();
    private static readonly MethodInfo MakeDelegateMethodInfo = typeof (RoutingResolver).GetMethod("MakeDelegate", BindingFlags.Static | BindingFlags.NonPublic);
    private readonly IConnectionSettingsValues _connectionSettings;
    private readonly IdResolver _idResolver;
    private readonly ConcurrentDictionary<Type, Func<object, string>> _localRouteDelegates = new ConcurrentDictionary<Type, Func<object, string>>();

    public RoutingResolver(IConnectionSettingsValues connectionSettings, IdResolver idResolver)
    {
      this._connectionSettings = connectionSettings;
      this._idResolver = idResolver;
    }

    private PropertyInfo GetPropertyCaseInsensitive(Type type, string fieldName) => type.GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

    internal static Func<object, object> MakeDelegate<T, TReturn>(MethodInfo get)
    {
      Func<T, TReturn> f = (Func<T, TReturn>) get.CreateDelegate(typeof (Func<T, TReturn>));
      return (Func<object, object>) (t => (object) f((T) t));
    }

    public string Resolve<T>(T @object) => (object) @object != null ? this.Resolve(@object.GetType(), (object) @object) : (string) null;

    public string Resolve(Type type, object @object)
    {
      string route;
      if (this.TryConnectionSettingsRoute(type, @object, out route))
        return route;
      return RoutingResolver.GetJoinFieldFromObject(type, @object)?.Match<string>((Func<JoinField.Parent, string>) (p => this._idResolver.Resolve<object>(@object)), (Func<JoinField.Child, string>) (c => this.ResolveId(c.ParentId, this._connectionSettings)));
    }

    private bool TryConnectionSettingsRoute(Type type, object @object, out string route)
    {
      route = (string) null;
      string fieldName;
      if (!this._connectionSettings.RouteProperties.TryGetValue(type, out fieldName))
        return false;
      Func<object, string> func1;
      if (this._localRouteDelegates.TryGetValue(type, out func1))
      {
        route = func1(@object);
        return true;
      }
      PropertyInfo propertyCaseInsensitive = this.GetPropertyCaseInsensitive(type, fieldName);
      Func<object, object> func = RoutingResolver.CreateGetterFunc(type, propertyCaseInsensitive);
      Func<object, string> func2 = (Func<object, string>) (o => func(o)?.ToString());
      this._localRouteDelegates.TryAdd(type, func2);
      route = func2(@object);
      return true;
    }

    private string ResolveId(Id id, IConnectionSettingsValues nestSettings) => id.Document == null ? id.StringOrLongValue : nestSettings.Inferrer.Id<object>(id.Document);

    private static JoinField GetJoinFieldFromObject(Type type, object @object)
    {
      if (type == (Type) null || @object == null)
        return (JoinField) null;
      Func<object, JoinField> func1;
      if (RoutingResolver.PropertyGetDelegates.TryGetValue(type, out func1))
        return func1(@object);
      PropertyInfo joinFieldProperty = RoutingResolver.GetJoinFieldProperty(type);
      if (joinFieldProperty == (PropertyInfo) null)
      {
        RoutingResolver.PropertyGetDelegates.TryAdd(type, (Func<object, JoinField>) (o => (JoinField) null));
        return (JoinField) null;
      }
      Func<object, object> func = RoutingResolver.CreateGetterFunc(type, joinFieldProperty);
      func1 = (Func<object, JoinField>) (o => func(o) as JoinField);
      RoutingResolver.PropertyGetDelegates.TryAdd(type, func1);
      return func1(@object);
    }

    private static Func<object, object> CreateGetterFunc(Type type, PropertyInfo joinProperty)
    {
      MethodInfo getMethod = joinProperty.GetMethod;
      return (Func<object, object>) RoutingResolver.MakeDelegateMethodInfo.MakeGenericMethod(type, getMethod.ReturnType).Invoke((object) null, new object[1]
      {
        (object) getMethod
      });
    }

    private static PropertyInfo GetJoinFieldProperty(Type type)
    {
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
      try
      {
        return ((IEnumerable<PropertyInfo>) properties).SingleOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType == typeof (JoinField)));
      }
      catch (InvalidOperationException ex)
      {
        throw new ArgumentException(type.Name + " has more than one JoinField property", (Exception) ex);
      }
    }
  }
}
