// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.ISocialEngService
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  [DefaultServiceImplementation(typeof (SocialEngagementService))]
  public interface ISocialEngService : IVssFrameworkService
  {
    IEnumerable<IdentityRef> GetEngagedUsers(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      int top,
      int skip);

    SocialEngagementRecord CreateSocialEngagementRecord(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter);

    SocialEngagementRecord DeleteSocialEngagementRecord(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter);

    SocialEngagementRecord GetSocialEngagementRecord(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter);

    IEnumerable<SocialEngagementRecord> GetSocialEngagementRecords(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      Guid ownerId,
      string artifactType,
      ISet<string> artifactIds,
      IEnumerable<SocialEngagementType> socialEngagementTypes);

    List<KeyValuePair<SocialEngagementType, string>> GetSocialEngagementProviders(
      IVssRequestContext requestContext);

    SocialEngagementAggregateMetric GetSocialEngagementAggregateMetric(
      IVssRequestContext requestContext,
      SocialEngagementCreateParameter socialEngagementCreateParameter);

    int DeleteOldAggregatedSocialEngagementMetrics(IVssRequestContext requestContext);
  }
}
