// Decompiled with JetBrains decompiler
// Type: Nest.IdResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Nest
{
  public class IdResolver
  {
    private static readonly ConcurrentDictionary<Type, Func<object, string>> IdDelegates = new ConcurrentDictionary<Type, Func<object, string>>();
    private static readonly MethodInfo MakeDelegateMethodInfo = typeof (IdResolver).GetMethod("MakeDelegate", BindingFlags.Static | BindingFlags.NonPublic);
    private readonly IConnectionSettingsValues _connectionSettings;
    private readonly ConcurrentDictionary<Type, Func<object, string>> _localIdDelegates = new ConcurrentDictionary<Type, Func<object, string>>();

    public IdResolver(IConnectionSettingsValues connectionSettings) => this._connectionSettings = connectionSettings;

    private PropertyInfo GetPropertyCaseInsensitive(Type type, string fieldName) => type.GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

    internal Func<T, string> CreateIdSelector<T>() where T : class => new Func<T, string>(this.Resolve<T>);

    internal static Func<object, object> MakeDelegate<T, TReturn>(MethodInfo get)
    {
      Func<T, TReturn> f = (Func<T, TReturn>) get.CreateDelegate(typeof (Func<T, TReturn>));
      return (Func<object, object>) (t => (object) f((T) t));
    }

    public string Resolve<T>(T @object) where T : class => !this._connectionSettings.DefaultDisableIdInference && (object) @object != null ? this.Resolve(@object.GetType(), (object) @object) : (string) null;

    public string Resolve(Type type, object @object)
    {
      if (type == (Type) null || @object == null)
        return (string) null;
      if (this._connectionSettings.DefaultDisableIdInference || this._connectionSettings.DisableIdInference.Contains(type))
        return (string) null;
      bool flag = this._connectionSettings.IdProperties.TryGetValue(type, out string _);
      Func<object, string> func1;
      if (this._localIdDelegates.TryGetValue(type, out func1) || !flag && IdResolver.IdDelegates.TryGetValue(type, out func1))
        return func1(@object);
      PropertyInfo inferredId = this.GetInferredId(type);
      if (inferredId == (PropertyInfo) null)
        return (string) null;
      MethodInfo getMethod = inferredId.GetMethod;
      Func<object, object> func = (Func<object, object>) IdResolver.MakeDelegateMethodInfo.MakeGenericMethod(type, getMethod.ReturnType).Invoke((object) null, new object[1]
      {
        (object) getMethod
      });
      func1 = (Func<object, string>) (o => func(o)?.ToString());
      if (flag)
        this._localIdDelegates.TryAdd(type, func1);
      else
        IdResolver.IdDelegates.TryAdd(type, func1);
      return func1(@object);
    }

    private PropertyInfo GetInferredId(Type type)
    {
      string fieldName1;
      this._connectionSettings.IdProperties.TryGetValue(type, out fieldName1);
      if (!fieldName1.IsNullOrEmpty())
        return this.GetPropertyCaseInsensitive(type, fieldName1);
      ElasticsearchTypeAttribute elasticsearchTypeAttribute = ElasticsearchTypeAttribute.From(type);
      string fieldName2 = (elasticsearchTypeAttribute != null ? (elasticsearchTypeAttribute.IdProperty.IsNullOrEmpty() ? 1 : 0) : 1) != 0 ? "Id" : elasticsearchTypeAttribute.IdProperty;
      return this.GetPropertyCaseInsensitive(type, fieldName2);
    }
  }
}
