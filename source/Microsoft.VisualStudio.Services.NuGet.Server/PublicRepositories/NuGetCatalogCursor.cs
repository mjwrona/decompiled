// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetCatalogCursor
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  [JsonConverter(typeof (NuGetCatalogCursor.CursorJsonConverter))]
  public record NuGetCatalogCursor(DateTimeOffset Value) : 
    IComparable<NuGetCatalogCursor>,
    IComparable
  {
    public static NuGetCatalogCursor Parse(string str) => new NuGetCatalogCursor(DateTimeOffset.Parse(str));

    public override string ToString() => string.Format("{0}({1:u})", (object) nameof (NuGetCatalogCursor), (object) this.Value);

    public int CompareTo(NuGetCatalogCursor other)
    {
      if ((object) this == (object) other)
        return 0;
      return (object) other == null ? 1 : this.Value.CompareTo(other.Value);
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      if ((object) this == obj)
        return 0;
      return this.CompareTo(obj as NuGetCatalogCursor ?? throw new ArgumentException("Object must be of type NuGetCatalogCursor"));
    }

    public static bool operator <(NuGetCatalogCursor left, NuGetCatalogCursor right) => Comparer<NuGetCatalogCursor>.Default.Compare(left, right) < 0;

    public static bool operator >(NuGetCatalogCursor left, NuGetCatalogCursor right) => Comparer<NuGetCatalogCursor>.Default.Compare(left, right) > 0;

    public static bool operator <=(NuGetCatalogCursor left, NuGetCatalogCursor right) => Comparer<NuGetCatalogCursor>.Default.Compare(left, right) <= 0;

    public static bool operator >=(NuGetCatalogCursor left, NuGetCatalogCursor right) => Comparer<NuGetCatalogCursor>.Default.Compare(left, right) >= 0;

    private class CursorJsonConverter : JsonConverter<NuGetCatalogCursor>
    {
      public override void WriteJson(
        JsonWriter writer,
        NuGetCatalogCursor? value,
        JsonSerializer serializer)
      {
        serializer.Serialize(writer, (object) value?.Value);
      }

      public override NuGetCatalogCursor? ReadJson(
        JsonReader reader,
        Type objectType,
        NuGetCatalogCursor? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
      {
        return new NuGetCatalogCursor(serializer.Deserialize<DateTimeOffset>(reader));
      }
    }
  }
}
