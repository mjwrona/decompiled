// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.ArtifactsVisitHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class ArtifactsVisitHelper
  {
    public static void PublishArtifactVisitedEvent(
      IVssRequestContext requestContext,
      Guid wikiId,
      Guid projectId,
      int? pageId,
      string pagePath)
    {
      try
      {
        ArtifactsVisitHelper.PublishArtifactVisitedEventInternal(requestContext, wikiId, projectId, pageId, pagePath);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(ex, 15253000, "Service");
      }
    }

    private static void PublishArtifactVisitedEventInternal(
      IVssRequestContext requestContext,
      Guid wikiId,
      Guid projectId,
      int? pageId,
      string pagePath)
    {
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Wiki", "Service"))
      {
        IContributionClaimService service = requestContext.GetService<IContributionClaimService>();
        if (!pageId.HasValue || !requestContext.IsFeatureEnabled("Wiki.Artifact.Visit.Publish") || !service.HasClaim(requestContext, "member"))
          return;
        Guid id = requestContext.GetAuthenticatedIdentity().Id;
        Guid artifactKind = new Guid("e9a7d0f1-3460-4118-89cc-bf15847656ae");
        string artifactId = wikiId.ToString() + "/" + pageId.Value.ToString();
        ArtifactsVisitEvent ev = new ArtifactsVisitEvent(artifactKind, id, artifactId, DateTime.UtcNow, projectId)
        {
          ActivityDetails = (IDictionary<string, string>) new Dictionary<string, string>()
        };
        ev.ActivityDetails.Add("PagePath", pagePath);
        requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((innerRequestContext, _) => innerRequestContext.GetService<ITeamFoundationEventService>().PublishNotification(innerRequestContext, (object) ev)));
        timedCiEvent.Properties.Add("ArtifactsVisitEventPublished", (object) true);
      }
    }
  }
}
