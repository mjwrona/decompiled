// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheKeySerializer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Redis;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheKeySerializer : IKeySerializer
  {
    string IKeySerializer.Serialize<T>(T key) => TypeDescriptor.GetConverter(key.GetType()).ConvertToInvariantString((object) key);

    T IKeySerializer.Deserialize<T>(string key)
    {
      if (string.IsNullOrEmpty(key))
        return default (T);
      if (typeof (T).IsAssignableFrom(typeof (string)))
        return (T) key;
      TypeConverter converter1 = TypeDescriptor.GetConverter(typeof (ImsCacheIdKey));
      if (converter1.IsValid((object) key))
        return (T) converter1.ConvertFrom((object) key);
      TypeConverter converter2 = TypeDescriptor.GetConverter(typeof (ImsCacheDescriptorKey));
      if (converter2.IsValid((object) key))
        return (T) converter2.ConvertFrom((object) key);
      TypeConverter converter3 = TypeDescriptor.GetConverter(typeof (ImsCacheScopedIdKey));
      if (converter3.IsValid((object) key))
        return (T) converter3.ConvertFrom((object) key);
      TypeConverter converter4 = TypeDescriptor.GetConverter(typeof (ImsCacheScopedNameKey));
      return converter4.IsValid((object) key) ? (T) converter4.ConvertFrom((object) key) : (T) ImsCacheKeySerializer.TypeDescriptor<T>.Converter.ConvertFromInvariantString(key);
    }

    private static class TypeDescriptor<T>
    {
      public static TypeConverter Converter { get; } = TypeDescriptor.GetConverter(typeof (T));
    }
  }
}
