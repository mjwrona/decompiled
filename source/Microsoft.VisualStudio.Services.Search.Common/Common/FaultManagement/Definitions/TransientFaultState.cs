// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Definitions.TransientFaultState
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Definitions
{
  public class TransientFaultState
  {
    public IndexerFaultSeverity StateSeverity { get; set; }

    public int FailureCount { get; set; }

    public int SuccessCount { get; set; }

    public DateTime StateTimeStamp { get; set; }

    public ServiceFaultState ServiceFaultState { get; set; }

    public TransientFaultState() => this.Reset();

    public bool IsExpired(FaultManagementSettings settings)
    {
      if (settings == null)
        throw new ArgumentNullException(nameof (settings));
      if (this.StateSeverity.Equals((object) IndexerFaultSeverity.Medium))
        return DateTime.UtcNow.Subtract(this.StateTimeStamp).TotalSeconds > (double) settings.MidFailureExpirationInSec;
      return this.StateSeverity.Equals((object) IndexerFaultSeverity.Critical) && DateTime.UtcNow.Subtract(this.StateTimeStamp).TotalSeconds > (double) settings.CritFailureExpirationInSec;
    }

    public void Reset()
    {
      this.StateSeverity = IndexerFaultSeverity.Healthy;
      this.FailureCount = 0;
      this.SuccessCount = 0;
      this.StateTimeStamp = DateTime.UtcNow;
      this.ServiceFaultState = ServiceFaultState.Closed;
    }
  }
}
