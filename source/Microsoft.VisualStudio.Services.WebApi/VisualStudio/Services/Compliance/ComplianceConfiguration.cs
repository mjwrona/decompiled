// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Compliance.ComplianceConfiguration
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Compliance
{
  [Obsolete("This type is no longer used.")]
  public class ComplianceConfiguration
  {
    public ComplianceConfiguration()
    {
    }

    public ComplianceConfiguration(
      DateTime complianceInvalidated,
      TimeSpan complianceGracePeriod,
      TimeSpan complianceStateRevalidationInterval,
      string complianceStateRepositoryStrategy)
    {
      this.ComplianceGracePeriod = complianceGracePeriod;
      this.ComplianceInvalidated = new DateTimeOffset(complianceInvalidated);
      this.ComplianceStateRepositoryStrategy = complianceStateRepositoryStrategy;
      this.ComplianceStateRevalidationInterval = complianceStateRevalidationInterval;
    }

    public TimeSpan ComplianceGracePeriod { get; set; }

    public DateTimeOffset ComplianceInvalidated { get; set; }

    public string ComplianceStateRepositoryStrategy { get; set; }

    public TimeSpan ComplianceStateRevalidationInterval { get; set; }
  }
}
