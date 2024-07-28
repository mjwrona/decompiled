// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.ISocialActivityAggregationService
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  [DefaultServiceImplementation(typeof (SocialActivityAggregationService))]
  public interface ISocialActivityAggregationService : IVssFrameworkService
  {
    void AddActivity(
      IVssRequestContext requestContext,
      string activityType,
      string data,
      string extendedData);

    void AddActivityImmediately(
      IVssRequestContext requestContext,
      string activityType,
      Guid activityId,
      DateTime activityTimeStamp,
      Guid userId,
      string data,
      string extendedData);

    IList<SocialActivityRecord> GetActivityRecords(
      IVssRequestContext requestContext,
      string activityType,
      DateTime fromDate,
      DateTime tillDate,
      bool fetchExtendedData,
      int skip,
      int take);

    IList<SocialActivityAggregatedMetric> GetActivityAggregatedMetrics(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid providerId,
      DateTime fromDate,
      DateTime tillDate,
      int skip,
      int take);

    IList<AggregatedArtifactsRecord> GetAggregatedArtifacts(
      IVssRequestContext requestContext,
      Guid dataSpaceIdentifier,
      Guid providerId);

    IList<SocialActivityAggregatedArtifact> GetSocialActivityAggregatedArtifactBatch(
      IVssRequestContext requestContext,
      Guid dataSpaceIdentifier,
      Guid providerId,
      byte artifactType,
      IList<string> artifactIds);

    void DeleteSocialActivityAggregatedArtifactBatch(
      IVssRequestContext requestContext,
      Guid dataSpaceIdentifier,
      Guid providerId,
      byte artifactType,
      IList<string> artifactIds);
  }
}
