// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.FeedAuditHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public static class FeedAuditHelper
  {
    private static readonly string feedChangeKey = "FeedChanges";
    private static readonly string upstreamsAddedKey = "UpstreamsAdded";
    private static readonly string upstreamsRemovedKey = "UpstreamsRemoved";
    private static readonly string retentionPolicyKey = "RetentionPolicy";
    private static readonly string feedViewChangeKey = "FeedViewChanges";
    private static readonly string viewVisibilityChangedKey = "Visibility";
    private static readonly string feedCreatedWithUpstreamServiceConnections = "UpstreamsWithServiceConnection";
    private static readonly FeedAuditHelper.AuditSet FeedCreate = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgCreate, ArtifactsAuditingConstants.FeedProjectCreate);
    private static readonly FeedAuditHelper.AuditSet FeedCreateWithUpstreams = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgCreateWithUpstreams, ArtifactsAuditingConstants.FeedProjectCreateWithUpstreams);
    private static readonly FeedAuditHelper.AuditSet FeedSoftDelete = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgSoftDelete, ArtifactsAuditingConstants.FeedProjectSoftDelete);
    private static readonly FeedAuditHelper.AuditSet FeedHardDelete = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgHardDelete, ArtifactsAuditingConstants.FeedProjectHardDelete);
    private static readonly FeedAuditHelper.AuditSet FeedModify = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgModify, ArtifactsAuditingConstants.FeedProjectModify);
    private static readonly FeedAuditHelper.AuditSet FeedPermissionsModify = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgPermissionModified, ArtifactsAuditingConstants.FeedProjectPermissionModified);
    private static readonly FeedAuditHelper.AuditSet FeedPermissionsDeleted = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgPermissionDeleted, ArtifactsAuditingConstants.FeedProjectPermissionDeleted);
    private static readonly FeedAuditHelper.AuditSet FeedViewCreate = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgViewCreate, ArtifactsAuditingConstants.FeedProjectViewCreate);
    private static readonly FeedAuditHelper.AuditSet FeedViewDelete = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgViewDelete, ArtifactsAuditingConstants.FeedProjectViewDelete);
    private static readonly FeedAuditHelper.AuditSet FeedViewModify = new FeedAuditHelper.AuditSet(ArtifactsAuditingConstants.FeedOrgViewModify, ArtifactsAuditingConstants.FeedProjectViewModify);

    public static void AuditCreateFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      try
      {
        if (feed.UpstreamSources != null && feed.UpstreamSources.Where<UpstreamSource>((Func<UpstreamSource, bool>) (x => x.ServiceEndpointId.HasValue)).Any<UpstreamSource>())
        {
          IEnumerable<UpstreamSource> upstreamSources = feed.UpstreamSources.Where<UpstreamSource>((Func<UpstreamSource, bool>) (x => x.ServiceEndpointId.HasValue));
          Dictionary<string, object> feedAuditDictionary = FeedAuditHelper.GetFeedAuditDictionary(feed);
          feedAuditDictionary.Add(FeedAuditHelper.feedCreatedWithUpstreamServiceConnections, (object) FeedAuditHelper.FormatUpstreamChange(upstreamSources));
          FeedAuditHelper.FeedCreateWithUpstreams.LogAuditEvent(requestContext, feed, feedAuditDictionary);
        }
        else
          FeedAuditHelper.FeedCreate.LogAuditEvent(requestContext, feed, FeedAuditHelper.GetFeedAuditDictionary(feed));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditSoftDeleteFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      try
      {
        FeedAuditHelper.FeedSoftDelete.LogAuditEvent(requestContext, feed, FeedAuditHelper.GetFeedAuditDictionary(feed));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditHardDeleteFeed(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      try
      {
        FeedAuditHelper.FeedHardDelete.LogAuditEvent(requestContext, feed, FeedAuditHelper.GetFeedAuditDictionary(feed));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditModifyFeed(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed formerFeed,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed updatedFeed)
    {
      try
      {
        Dictionary<string, object> feedAuditDictionary = FeedAuditHelper.GetFeedAuditDictionary(updatedFeed);
        List<string> source = new List<string>();
        if (formerFeed.Name != updatedFeed.Name)
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditFromFormat((object) "Name", (object) formerFeed.Name));
        if (formerFeed.Description != updatedFeed.Description)
          source.Add("Description");
        if (formerFeed.UpstreamEnabled != updatedFeed.UpstreamEnabled)
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditToFormat((object) "UpstreamEnabled", (object) updatedFeed.UpstreamEnabled));
        if (formerFeed.HideDeletedPackageVersions != updatedFeed.HideDeletedPackageVersions)
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditToFormat((object) "HideDeletedPackageVersions", (object) updatedFeed.HideDeletedPackageVersions));
        if (formerFeed.DefaultViewId != updatedFeed.DefaultViewId)
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditToFormat((object) "DefaultViewId", (object) updatedFeed.DefaultViewId));
        if (formerFeed.BadgesEnabled != updatedFeed.BadgesEnabled)
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditToFormat((object) "BadgesEnabled", (object) updatedFeed.BadgesEnabled));
        IList<UpstreamSource> upstreamSources1 = formerFeed.UpstreamSources;
        IEnumerable<UpstreamSource> upstreamSources2 = (IEnumerable<UpstreamSource>) ((upstreamSources1 != null ? (object) upstreamSources1.Where<UpstreamSource>((Func<UpstreamSource, bool>) (t => !t.DeletedDate.HasValue)) : (object) null) ?? (object) Array.Empty<UpstreamSource>());
        IList<UpstreamSource> upstreamSources3 = updatedFeed.UpstreamSources;
        IEnumerable<UpstreamSource> upstreamSources4 = (IEnumerable<UpstreamSource>) ((upstreamSources3 != null ? (object) upstreamSources3.Where<UpstreamSource>((Func<UpstreamSource, bool>) (t => !t.DeletedDate.HasValue)) : (object) null) ?? (object) Array.Empty<UpstreamSource>());
        FeedAuditHelper.UpstreamIdComparer comparer = new FeedAuditHelper.UpstreamIdComparer();
        if (!upstreamSources2.SequenceEqual<UpstreamSource>(upstreamSources4, (IEqualityComparer<UpstreamSource>) comparer))
        {
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditModifedFormat((object) "UpstreamSources"));
          IEnumerable<UpstreamSource> upstreamSources5 = upstreamSources4.Except<UpstreamSource>(upstreamSources2, (IEqualityComparer<UpstreamSource>) comparer);
          if (upstreamSources5.Any<UpstreamSource>())
          {
            feedAuditDictionary.Add(FeedAuditHelper.upstreamsAddedKey, (object) upstreamSources5);
            source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditAddedFormat((object) FeedAuditHelper.FormatUpstreamChange(upstreamSources5)));
          }
          IEnumerable<UpstreamSource> upstreamSources6 = upstreamSources2.Except<UpstreamSource>(upstreamSources4, (IEqualityComparer<UpstreamSource>) comparer);
          if (upstreamSources6.Any<UpstreamSource>())
          {
            feedAuditDictionary.Add(FeedAuditHelper.upstreamsRemovedKey, (object) upstreamSources6);
            source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditRemovedFormat((object) FeedAuditHelper.FormatUpstreamChange(upstreamSources6)));
          }
        }
        if (!source.Any<string>())
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditNoAdditionalDetails());
        feedAuditDictionary.Add(FeedAuditHelper.feedChangeKey, (object) source);
        FeedAuditHelper.FeedModify.LogAuditEvent(requestContext, updatedFeed, feedAuditDictionary);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditFeedPermissionsChanges(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<FeedPermission> permissions)
    {
      try
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, nameof (feed));
        ArgumentUtility.CheckForNull<IEnumerable<FeedPermission>>(permissions, nameof (permissions));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        List<Tuple<string, string>> list1 = permissions.Where<FeedPermission>((Func<FeedPermission, bool>) (p => p.Role == FeedRole.None)).Select<FeedPermission, Tuple<string, string>>(FeedAuditHelper.\u003C\u003EO.\u003C0\u003E__ExtractTuple ?? (FeedAuditHelper.\u003C\u003EO.\u003C0\u003E__ExtractTuple = new Func<FeedPermission, Tuple<string, string>>(FeedAuditHelper.ExtractTuple))).ToList<Tuple<string, string>>();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        List<Tuple<string, string>> list2 = permissions.Where<FeedPermission>((Func<FeedPermission, bool>) (p => p.Role != FeedRole.None)).Select<FeedPermission, Tuple<string, string>>(FeedAuditHelper.\u003C\u003EO.\u003C0\u003E__ExtractTuple ?? (FeedAuditHelper.\u003C\u003EO.\u003C0\u003E__ExtractTuple = new Func<FeedPermission, Tuple<string, string>>(FeedAuditHelper.ExtractTuple))).ToList<Tuple<string, string>>();
        FeedAuditHelper.SendPermissionsAuditEvent(requestContext, feed, FeedAuditHelper.GetFeedAuditDictionary(feed), list2, false);
        FeedAuditHelper.SendPermissionsAuditEvent(requestContext, feed, FeedAuditHelper.GetFeedAuditDictionary(feed), list1, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditRetentionChange(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedRetentionPolicy policy)
    {
      try
      {
        Dictionary<string, object> feedAuditDictionary = FeedAuditHelper.GetFeedAuditDictionary(feed);
        feedAuditDictionary.Add(FeedAuditHelper.feedChangeKey, (object) Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditModifedFormat((object) Microsoft.VisualStudio.Services.Feed.Server.Resources.RetentionPolicy()));
        feedAuditDictionary.Add(FeedAuditHelper.retentionPolicyKey, (object) policy);
        FeedAuditHelper.FeedModify.LogAuditEvent(requestContext, feed, feedAuditDictionary);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditCreateFeedView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView view)
    {
      try
      {
        FeedAuditHelper.FeedViewCreate.LogAuditEvent(requestContext, feed, FeedAuditHelper.GetFeedViewAuditDictionary(feed, view));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditDeleteFeedView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView view)
    {
      try
      {
        FeedAuditHelper.FeedViewDelete.LogAuditEvent(requestContext, feed, FeedAuditHelper.GetFeedViewAuditDictionary(feed, view));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    public static void AuditModifyFeedView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView formerView,
      FeedView updatedView)
    {
      try
      {
        Dictionary<string, object> viewAuditDictionary = FeedAuditHelper.GetFeedViewAuditDictionary(feed, updatedView);
        List<string> source = new List<string>();
        if (formerView.Name != updatedView.Name)
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditFromFormat((object) "Name", (object) formerView.Name));
        FeedVisibility? visibility1 = formerView.Visibility;
        FeedVisibility? visibility2 = updatedView.Visibility;
        if (!(visibility1.GetValueOrDefault() == visibility2.GetValueOrDefault() & visibility1.HasValue == visibility2.HasValue))
        {
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditModifiedFromToFormat((object) "Visibility", (object) formerView.Visibility, (object) updatedView.Visibility));
          viewAuditDictionary.Add(FeedAuditHelper.viewVisibilityChangedKey, (object) updatedView.Visibility);
        }
        if (!source.Any<string>())
          source.Add(Microsoft.VisualStudio.Services.Feed.Server.Resources.AuditNoAdditionalDetails());
        viewAuditDictionary.Add(FeedAuditHelper.feedViewChangeKey, (object) source);
        FeedAuditHelper.FeedViewModify.LogAuditEvent(requestContext, feed, viewAuditDictionary);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019127, "Feed", "Audit", ex);
      }
    }

    private static string FormatUpstreamChange(IEnumerable<UpstreamSource> upstreamSources) => string.Join(", ", upstreamSources.Select<UpstreamSource, string>((Func<UpstreamSource, string>) (t => string.Format("{0}{1}", (object) string.Format("{0} ({1})", (object) t.Name, (object) t.Protocol), t.ServiceEndpointId.HasValue ? (object) string.Format(" - {0} {1}", (object) "ServiceEndpoint", (object) t.ServiceEndpointId.Value) : (object) ""))));

    private static Tuple<string, string> ExtractTuple(FeedPermission p)
    {
      string str = p.DisplayName?.ToString();
      if (str == null)
      {
        Guid? identityId = p.IdentityId;
        ref Guid? local = ref identityId;
        str = (local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "unspecified";
      }
      return new Tuple<string, string>(str, p.Role.ToString());
    }

    private static void SendPermissionsAuditEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Dictionary<string, object> auditDictionary,
      List<Tuple<string, string>> tuples,
      bool isDeletion)
    {
      if (tuples == null || !tuples.Any<Tuple<string, string>>())
        return;
      auditDictionary.Add("DisplayName", (object) tuples.First<Tuple<string, string>>().Item1);
      auditDictionary.Add("Role", (object) tuples.First<Tuple<string, string>>().Item2);
      if (tuples.Count<Tuple<string, string>>() > 1)
        auditDictionary.Add("EventSummary", (object) tuples);
      if (isDeletion)
        FeedAuditHelper.FeedPermissionsDeleted.LogAuditEvent(requestContext, feed, auditDictionary);
      else
        FeedAuditHelper.FeedPermissionsModify.LogAuditEvent(requestContext, feed, auditDictionary);
    }

    private static Dictionary<string, object> GetFeedAuditDictionary(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => new Dictionary<string, object>()
    {
      {
        "FeedName",
        (object) feed.Name
      },
      {
        "FeedId",
        (object) feed.Id
      }
    };

    private static Dictionary<string, object> GetFeedViewAuditDictionary(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, FeedView view) => new Dictionary<string, object>()
    {
      {
        "FeedName",
        (object) feed.Name
      },
      {
        "FeedId",
        (object) feed.Id
      },
      {
        "FeedViewName",
        (object) view.Name
      },
      {
        "FeedViewId",
        (object) view.Id
      },
      {
        "FeedViewType",
        (object) view.Type
      }
    };

    private class AuditSet
    {
      private string OrgAction { get; set; }

      private string ProjectAction { get; set; }

      public AuditSet(string orgAction, string projectAction)
      {
        this.OrgAction = orgAction;
        this.ProjectAction = projectAction;
      }

      public void LogAuditEvent(
        IVssRequestContext requestContext,
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
        Dictionary<string, object> auditDictionary)
      {
        if (feed.Project != (ProjectReference) null)
          requestContext.LogAuditEvent(this.ProjectAction, auditDictionary, projectId: feed.Project.Id);
        else
          requestContext.LogAuditEvent(this.OrgAction, auditDictionary);
      }
    }

    private sealed class UpstreamIdComparer : IEqualityComparer<UpstreamSource>
    {
      public bool Equals(UpstreamSource x, UpstreamSource y)
      {
        if (x == y)
          return true;
        return x != null && y != null && x.Id.Equals(y.Id);
      }

      public int GetHashCode(UpstreamSource obj) => obj.Id.GetHashCode();
    }
  }
}
