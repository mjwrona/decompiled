// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiStateDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class KpiStateDefinition
  {
    public KpiStateDefinition() => this.EventId = 0;

    public KpiStateDefinition(int kpiId)
      : this()
    {
      this.KpiId = kpiId;
    }

    public int Id { get; private set; }

    public int KpiId { get; private set; }

    public KpiState KpiState { get; set; }

    public double Limit { get; set; }

    public int EventId { get; set; }
  }
}
