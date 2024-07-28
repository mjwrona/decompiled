// Decompiled with JetBrains decompiler
// Type: Nest.InterfaceGenericReadOnlyDictionaryResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nest
{
  internal class InterfaceGenericReadOnlyDictionaryResolver : IJsonFormatterResolver
  {
    public static readonly InterfaceGenericReadOnlyDictionaryResolver Instance = new InterfaceGenericReadOnlyDictionaryResolver();

    public IJsonFormatter<T> GetFormatter<T>() => InterfaceGenericReadOnlyDictionaryResolver.FormatterCache<T>.Formatter;

    internal static bool IsGenericIReadOnlyDictionary(Type type) => ((IEnumerable<Type>) type.GetInterfaces()).Any<Type>((Func<Type, bool>) (t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IReadOnlyDictionary<,>)));

    private static class FormatterCache<T>
    {
      public static readonly IJsonFormatter<T> Formatter = (IJsonFormatter<T>) InterfaceGenericReadOnlyDictionaryResolver.DictionaryFormatterHelper.GetFormatter(typeof (T));
    }

    internal static class DictionaryFormatterHelper
    {
      internal static object GetFormatter(Type t)
      {
        if (!typeof (IEnumerable).IsAssignableFrom(t) || !InterfaceGenericReadOnlyDictionaryResolver.IsGenericIReadOnlyDictionary(t))
          return (object) null;
        Type[] genericArguments;
        t.TryGetGenericDictionaryArguments(out genericArguments);
        Type[] genericTypeArguments = new Type[3]
        {
          genericArguments[0],
          genericArguments[1],
          t
        };
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
        Type genericDictionaryInterface = typeof (IDictionary<,>).MakeGenericType(genericArguments);
        ConstructorInfo constructorInfo = ((IEnumerable<ConstructorInfo>) type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)).Select(c => new
        {
          c = c,
          p = c.GetParameters()
        }).Where(_param1 => _param1.p.Length == 1 && genericDictionaryInterface.IsAssignableFrom(_param1.p[0].ParameterType)).OrderByDescending(_param1 => _param1.p.Length).Select(_param1 => _param1.c).FirstOrDefault<ConstructorInfo>();
        if (constructorInfo == (ConstructorInfo) null)
          throw new Exception("Cannot create an instance of " + t.FullName + " because it does not have a public constructor accepting IDictionary<" + genericArguments[0].FullName + "," + genericArguments[1].FullName + "> argument");
        object obj = TypeExtensions.GetActivatorMethodInfo.MakeGenericMethod(t).Invoke((object) null, new object[1]
        {
          (object) constructorInfo
        });
        return InterfaceGenericReadOnlyDictionaryResolver.DictionaryFormatterHelper.CreateInstance(genericTypeArguments, obj, (object) (constructorInfo.GetParameters().Length == 0));
      }

      private static object CreateInstance(Type[] genericTypeArguments, params object[] args) => typeof (InterfaceReadOnlyDictionaryFormatter<,,>).MakeGenericType(genericTypeArguments).CreateInstance(args);
    }
  }
}
