// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.TypeConverterSerializer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class TypeConverterSerializer : IKeySerializer
  {
    string IKeySerializer.Serialize<T>(T key) => TypeConverterSerializer.TypeDescriptor<T>.Converter.ConvertToInvariantString((object) key);

    T IKeySerializer.Deserialize<T>(string key)
    {
      if (string.IsNullOrEmpty(key))
        return default (T);
      return typeof (T).IsAssignableFrom(typeof (string)) ? (T) key : (T) TypeConverterSerializer.TypeDescriptor<T>.Converter.ConvertFromInvariantString(key);
    }

    private static class TypeDescriptor<T>
    {
      private static readonly TypeConverter s_converter = TypeDescriptor.GetConverter(typeof (T));

      public static TypeConverter Converter => TypeConverterSerializer.TypeDescriptor<T>.s_converter;
    }
  }
}
