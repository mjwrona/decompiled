// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiChangelogCursor
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Newtonsoft.Json;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  [JsonConverter(typeof (PyPiChangelogCursor.ChangelogCursorJsonConverter))]
  public record PyPiChangelogCursor(ulong SinceSerial) : IComparable<PyPiChangelogCursor>
  {
    public static PyPiChangelogCursor Parse(string? input)
    {
      ulong result;
      return !ulong.TryParse(input, out result) ? new PyPiChangelogCursor(0UL) : new PyPiChangelogCursor(result);
    }

    public override string ToString() => string.Format("{0}({1})", (object) nameof (PyPiChangelogCursor), (object) this.SinceSerial);

    public int CompareTo(PyPiChangelogCursor? other)
    {
      if ((object) this == (object) other)
        return 0;
      return (object) other == null ? 1 : this.SinceSerial.CompareTo(other.SinceSerial);
    }

    private class ChangelogCursorJsonConverter : JsonConverter<PyPiChangelogCursor>
    {
      public override PyPiChangelogCursor? ReadJson(
        JsonReader reader,
        Type objectType,
        PyPiChangelogCursor? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
      {
        return new PyPiChangelogCursor(serializer.Deserialize<ulong>(reader));
      }

      public override void WriteJson(
        JsonWriter writer,
        PyPiChangelogCursor? value,
        JsonSerializer serializer)
      {
        serializer.Serialize(writer, (object) value?.SinceSerial);
      }
    }
  }
}
