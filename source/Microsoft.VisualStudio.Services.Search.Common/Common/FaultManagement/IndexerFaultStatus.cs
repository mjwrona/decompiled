// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.IndexerFaultStatus
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public class IndexerFaultStatus
  {
    public IndexerFaultStatus() => this.Reset();

    public IndexerFaultStatus(IndexerFaultSeverity severity, bool isActive)
    {
      this.IsActive = isActive;
      this.Severity = severity;
    }

    public void Reset()
    {
      this.IsActive = false;
      this.Severity = IndexerFaultSeverity.Healthy;
    }

    public void ResetSeverity() => this.Severity = IndexerFaultSeverity.Healthy;

    public bool IsActive { get; set; }

    public IndexerFaultSeverity Severity { get; set; }
  }
}
