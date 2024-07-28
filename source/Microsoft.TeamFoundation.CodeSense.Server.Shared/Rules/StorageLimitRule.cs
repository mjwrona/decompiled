// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Rules.StorageLimitRule
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Extensions;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.CodeSense.Server.Rules
{
  public class StorageLimitRule : JobRule
  {
    private const long GigaByteToByte = 1073741824;

    public StorageLimitRule(
      IVssRequestContext requestContext,
      JobRule.SatisfactionHandler callIfSatisfied,
      JobRule.SatisfactionHandler callIfUnsatisfied)
      : base(requestContext, callIfSatisfied, callIfUnsatisfied)
    {
    }

    protected override bool IsSatisfiedBy()
    {
      long storageGrowthLimit = this.RequestContext.GetService<IVssRegistryService>().GetStorageGrowthLimit(this.RequestContext);
      return storageGrowthLimit == -1L || this.RequestContext.GetService<TeamFoundationCounterService>().GetStorageGrowth(this.RequestContext) < storageGrowthLimit * 1073741824L;
    }
  }
}
