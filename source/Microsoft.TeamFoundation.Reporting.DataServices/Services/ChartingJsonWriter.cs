// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.ChartingJsonWriter
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Newtonsoft.Json;
using System.IO;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  internal class ChartingJsonWriter : JsonTextWriter
  {
    public ChartingJsonWriter(Stream stream)
      : base((TextWriter) new StreamWriter(stream))
    {
    }

    internal void WritePropertyNameValue(string name, string value)
    {
      this.WritePropertyName(name);
      this.WriteValue(value);
    }

    internal void WritePropertyNameValue(string name, float value)
    {
      this.WritePropertyName(name);
      this.WriteValue(value);
    }

    internal void WriteStartObjectArrayPair(string name)
    {
      this.WriteStartObject();
      this.WritePropertyName(name);
      this.WriteStartArray();
    }

    internal void WriteStartObjectArrayPair(string keyName, string keyValue, string valueName)
    {
      this.WriteStartObject();
      this.WritePropertyName(keyName);
      this.WriteValue(keyValue);
      this.WritePropertyName(valueName);
      this.WriteStartArray();
    }

    internal void WriteEndObjectArrayPair()
    {
      this.WriteEndArray();
      this.WriteEndObject();
    }
  }
}
