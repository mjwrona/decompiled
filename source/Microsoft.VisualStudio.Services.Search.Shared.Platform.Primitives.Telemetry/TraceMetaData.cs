// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.TraceMetaData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public class TraceMetaData
  {
    public int TracePoint { get; private set; }

    public string TraceArea { get; private set; }

    public string TraceLayer { get; private set; }

    public TraceMetaData(int tracePoint, string traceArea, string traceLayer)
    {
      this.TracePoint = tracePoint;
      this.TraceArea = traceArea;
      this.TraceLayer = traceLayer;
    }
  }
}
