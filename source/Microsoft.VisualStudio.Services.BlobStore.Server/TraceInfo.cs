// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.TraceInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class TraceInfo
  {
    public int TracePointBase { get; set; }

    public bool ShouldTraceMethodDetail { get; set; }

    public string ControllerName { get; set; }

    public string TraceArea { get; set; }

    public TraceInfo(
      int tracePointBase,
      bool shouldTraceMethodDetail,
      string controllerName,
      string traceArea)
    {
      this.TracePointBase = tracePointBase;
      this.ShouldTraceMethodDetail = shouldTraceMethodDetail;
      this.ControllerName = controllerName;
      this.TraceArea = traceArea;
    }
  }
}
