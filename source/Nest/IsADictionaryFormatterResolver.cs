// Decompiled with JetBrains decompiler
// Type: Nest.IsADictionaryFormatterResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nest
{
  internal class IsADictionaryFormatterResolver : IJsonFormatterResolver
  {
    public static readonly IsADictionaryFormatterResolver Instance = new IsADictionaryFormatterResolver();

    public IJsonFormatter<T> GetFormatter<T>() => IsADictionaryFormatterResolver.FormatterCache<T>.Formatter;

    internal static bool IsIsADictionary(Type type) => ((IEnumerable<Type>) type.GetInterfaces()).Any<Type>((Func<Type, bool>) (t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IIsADictionary<,>)));

    private static class FormatterCache<T>
    {
      public static readonly IJsonFormatter<T> Formatter = (IJsonFormatter<T>) IsADictionaryFormatterResolver.IsADictionaryFormatter.GetFormatter(typeof (T));
    }

    internal static class IsADictionaryFormatter
    {
      internal static object GetFormatter(Type t)
      {
        if (!typeof (IIsADictionary).IsAssignableFrom(t) || !IsADictionaryFormatterResolver.IsIsADictionary(t))
          return (object) null;
        Type[] genericArguments;
        t.TryGetGenericDictionaryArguments(out genericArguments);
        List<Type> typeList = new List<Type>();
        typeList.AddRange((IEnumerable<Type>) genericArguments);
        typeList.Add(t);
        Type genericDictionaryInterface = typeof (IDictionary<,>).MakeGenericType(genericArguments);
        Type type;
        if (t.IsInterface)
        {
          ReadAsAttribute customAttribute = t.GetCustomAttribute<ReadAsAttribute>();
          if (customAttribute == null)
            throw new Exception("Unable to deserialize interface " + t.FullName);
          type = customAttribute.Type.IsGenericType ? customAttribute.Type.MakeGenericType(genericArguments) : customAttribute.Type;
        }
        else
          type = t;
        ConstructorInfo constructorInfo = ((IEnumerable<ConstructorInfo>) type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Select(c => new
        {
          c = c,
          p = c.GetParameters()
        }).Where(_param1 =>
        {
          if (_param1.p.Length == 0)
            return true;
          return _param1.p.Length == 1 && genericDictionaryInterface.IsAssignableFrom(_param1.p[0].ParameterType);
        }).OrderByDescending(_param1 => _param1.p.Length).Select(_param1 => _param1.c).FirstOrDefault<ConstructorInfo>();
        if (constructorInfo == (ConstructorInfo) null)
          throw new Exception("Cannot create an instance of " + t.FullName + " because it does not have a constructor accepting " + genericDictionaryInterface.FullName + " argument or a parameterless constructor");
        object obj = TypeExtensions.GetActivatorMethodInfo.MakeGenericMethod(t).Invoke((object) null, new object[1]
        {
          (object) constructorInfo
        });
        return IsADictionaryFormatterResolver.IsADictionaryFormatter.CreateInstance(typeList.ToArray(), obj, (object) (constructorInfo.GetParameters().Length == 0));
      }

      private static object CreateInstance(Type[] genericTypeArguments, params object[] args) => typeof (IsADictionaryBaseFormatter<,,>).MakeGenericType(genericTypeArguments).CreateInstance(args);
    }
  }
}
