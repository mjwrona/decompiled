// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.GeoJsonWriterAdapter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.Spatial;

namespace Microsoft.OData.Json
{
  internal sealed class GeoJsonWriterAdapter : IGeoJsonWriter
  {
    private readonly IJsonWriter writer;

    internal GeoJsonWriterAdapter(IJsonWriter writer) => this.writer = writer;

    void IGeoJsonWriter.StartObjectScope() => this.writer.StartObjectScope();

    void IGeoJsonWriter.EndObjectScope() => this.writer.EndObjectScope();

    void IGeoJsonWriter.StartArrayScope() => this.writer.StartArrayScope();

    void IGeoJsonWriter.EndArrayScope() => this.writer.EndArrayScope();

    void IGeoJsonWriter.AddPropertyName(string name) => this.writer.WriteName(name);

    void IGeoJsonWriter.AddValue(double value) => this.writer.WriteValue(value);

    void IGeoJsonWriter.AddValue(string value) => this.writer.WriteValue(value);
  }
}
