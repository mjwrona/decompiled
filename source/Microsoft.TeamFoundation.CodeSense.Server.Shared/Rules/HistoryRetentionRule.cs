// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Rules.HistoryRetentionRule
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Jobs;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server.Rules
{
  public class HistoryRetentionRule : JobRule
  {
    private readonly WatermarkIterator _iterator;

    public HistoryRetentionRule(
      IVssRequestContext requestContext,
      JobRule.SatisfactionHandler callIfSatisfied,
      JobRule.SatisfactionHandler callIfUnsatisfied,
      WatermarkIterator retentionIterator)
      : base(requestContext, callIfSatisfied, callIfUnsatisfied)
    {
      this._iterator = retentionIterator;
    }

    protected override bool IsSatisfiedBy()
    {
      int retentionPeriod = this.RequestContext.GetService<IVssRegistryService>().GetRetentionPeriod(this.RequestContext);
      return retentionPeriod == -1 || this._iterator.CurrentChangesetTime >= DateTime.UtcNow.AddMonths(-1 * retentionPeriod);
    }
  }
}
