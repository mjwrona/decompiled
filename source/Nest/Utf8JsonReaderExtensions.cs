// Decompiled with JetBrains decompiler
// Type: Nest.Utf8JsonReaderExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.CompilerServices;

namespace Nest
{
  internal static class Utf8JsonReaderExtensions
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? ReadNullableDouble(ref this JsonReader reader)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.Null)
        return new double?(reader.ReadDouble());
      reader.ReadNext();
      return new double?();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long? ReadNullableLong(ref this JsonReader reader)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.Null)
        return new long?(reader.ReadInt64());
      reader.ReadNext();
      return new long?();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool? ReadNullableBoolean(ref this JsonReader reader)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.Null)
        return new bool?(reader.ReadBoolean());
      reader.ReadNext();
      return new bool?();
    }
  }
}
