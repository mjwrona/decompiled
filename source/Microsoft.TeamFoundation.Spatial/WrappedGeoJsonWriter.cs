// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.WrappedGeoJsonWriter
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  internal sealed class WrappedGeoJsonWriter : GeoJsonWriterBase
  {
    private readonly IGeoJsonWriter writer;

    public WrappedGeoJsonWriter(IGeoJsonWriter writer) => this.writer = writer;

    protected override void StartObjectScope() => this.writer.StartObjectScope();

    protected override void StartArrayScope() => this.writer.StartArrayScope();

    protected override void AddPropertyName(string name) => this.writer.AddPropertyName(name);

    protected override void AddValue(string value) => this.writer.AddValue(value);

    protected override void AddValue(double value) => this.writer.AddValue(value);

    protected override void EndArrayScope() => this.writer.EndArrayScope();

    protected override void EndObjectScope() => this.writer.EndObjectScope();
  }
}
