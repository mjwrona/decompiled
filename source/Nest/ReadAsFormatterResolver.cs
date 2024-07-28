// Decompiled with JetBrains decompiler
// Type: Nest.ReadAsFormatterResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Reflection;

namespace Nest
{
  internal sealed class ReadAsFormatterResolver : IJsonFormatterResolver
  {
    public static readonly IJsonFormatterResolver Instance = (IJsonFormatterResolver) new ReadAsFormatterResolver();

    private ReadAsFormatterResolver()
    {
    }

    public IJsonFormatter<T> GetFormatter<T>() => ReadAsFormatterResolver.FormatterCache<T>.Formatter;

    private static class FormatterCache<T>
    {
      public static readonly IJsonFormatter<T> Formatter;

      static FormatterCache()
      {
        ReadAsAttribute customAttribute = typeof (T).GetCustomAttribute<ReadAsAttribute>();
        if (customAttribute == null)
          return;
        try
        {
          Type type;
          if (customAttribute.Type.IsGenericType && !customAttribute.Type.IsConstructedGenericType)
            type = typeof (ReadAsFormatter<,>).MakeGenericType(customAttribute.Type.MakeGenericType(typeof (T).GenericTypeArguments), typeof (T));
          else
            type = typeof (ReadAsFormatter<,>).MakeGenericType(customAttribute.Type, typeof (T));
          ReadAsFormatterResolver.FormatterCache<T>.Formatter = (IJsonFormatter<T>) Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException("Can not create formatter from ReadAsAttribute for " + customAttribute.Type.Name, ex);
        }
      }
    }
  }
}
