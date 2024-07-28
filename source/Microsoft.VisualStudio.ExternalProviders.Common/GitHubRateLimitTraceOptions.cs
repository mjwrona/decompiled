// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.GitHubRateLimitTraceOptions
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public class GitHubRateLimitTraceOptions
  {
    public int WarningRemainingPercentage { get; private set; }

    public bool LogPerUserUsage { get; private set; }

    public int GraphQLcostWarningThreshold { get; private set; }

    public GitHubRateLimitTraceOptions(
      int warningRemainingPercentage,
      int graphQLcostWarningThreshold,
      bool logPerUserUsage)
    {
      ArgumentUtility.CheckBoundsInclusive(warningRemainingPercentage, 0, 100, nameof (warningRemainingPercentage));
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) graphQLcostWarningThreshold, nameof (warningRemainingPercentage));
      this.WarningRemainingPercentage = warningRemainingPercentage;
      this.GraphQLcostWarningThreshold = graphQLcostWarningThreshold;
      this.LogPerUserUsage = logPerUserUsage;
    }
  }
}
