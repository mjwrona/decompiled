// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.SingleLocationTracePoint
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class SingleLocationTracePoint
  {
    public readonly int TracePoint;
    public readonly string Area;
    public readonly string Layer;

    public SingleLocationTracePoint(int tracePoint, string area, string layer)
    {
      this.TracePoint = tracePoint;
      this.Area = area;
      this.Layer = layer;
    }
  }
}
