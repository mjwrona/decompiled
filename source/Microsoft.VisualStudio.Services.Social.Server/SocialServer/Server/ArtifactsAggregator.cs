// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.ArtifactsAggregator
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public abstract class ArtifactsAggregator : IArtifactsAggregator
  {
    public void Aggregate(
      IVssRequestContext requestContext,
      IList<SocialActivityAggregatedMetric> aggregatedMetrics,
      out string resultMessage)
    {
      IList<AggregatedArtifactsRecord> aggregatedArtifacts = this.CalculateAggregatedArtifacts(requestContext, aggregatedMetrics, out resultMessage);
      if (aggregatedArtifacts.Count<AggregatedArtifactsRecord>() > 0)
        requestContext.GetService<SocialActivityAggregationService>().AddOrUpdateAggregatedArtifactsRecords(requestContext, Guid.Empty, aggregatedArtifacts);
      resultMessage += string.Format("{0} aggregatedArtifactsRecord were added/updated.", (object) aggregatedArtifacts.Count<AggregatedArtifactsRecord>());
    }

    protected abstract IList<AggregatedArtifactsRecord> CalculateAggregatedArtifacts(
      IVssRequestContext requestContext,
      IList<SocialActivityAggregatedMetric> aggregatedMetrics,
      out string resultMessage);
  }
}
