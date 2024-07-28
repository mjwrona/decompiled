// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Telemetry.FeedCiPublisher
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server.Telemetry
{
  public static class FeedCiPublisher
  {
    public static void PublishCreateFeedEvent(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
      feedTelemitryObject.Add("HasDescription", !string.IsNullOrWhiteSpace(feed.Description));
      feedTelemitryObject.Add("UpstreamEnabled", feed.UpstreamEnabled);
      feedTelemitryObject.Add("AllowUpstreamNameConflict", feed.AllowUpstreamNameConflict);
      feedTelemitryObject.Add("FeedCapabilities", feed.Capabilities.ToString());
      FeedCiPublisher.Publish(requestContext, "CreateFeed", feedTelemitryObject);
    }

    public static void PublishCreateFeedViewEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView feedView)
    {
      CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
      feedTelemitryObject.Add("FeedViewId", feedView.Id.ToString());
      feedTelemitryObject.Add("FeedViewName", feedView.Name);
      feedTelemitryObject.Add("FeedViewType", feedView.Type.ToString());
      FeedCiPublisher.Publish(requestContext, "CreateFeedView", feedTelemitryObject);
    }

    public static void PublishDeleteFeedViewEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView feedView)
    {
      CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
      feedTelemitryObject.Add("FeedViewId", feedView.Id.ToString());
      feedTelemitryObject.Add("FeedViewName", feedView.Name);
      feedTelemitryObject.Add("FeedViewType", feedView.Type.ToString());
      FeedCiPublisher.Publish(requestContext, "DeleteFeedView", feedTelemitryObject);
    }

    public static void PublishDeletedFeedViewPermissionsEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedView feedView,
      bool removedAtleastOneACL)
    {
      CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
      feedTelemitryObject.Add("FeedViewId", feedView.Id.ToString());
      feedTelemitryObject.Add("FeedViewName", feedView.Name);
      feedTelemitryObject.Add("FeedViewType", feedView.Type.ToString());
      feedTelemitryObject.Add("RemovedAtleastOneACL", removedAtleastOneACL.ToString());
      FeedCiPublisher.Publish(requestContext, "DeleteFeedViewPermissions", feedTelemitryObject);
    }

    public static void PublishUpdateFeedEvent(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
      feedTelemitryObject.Add("HasName", !string.IsNullOrWhiteSpace(feed.Name));
      feedTelemitryObject.Add("HasDescription", !string.IsNullOrWhiteSpace(feed.Description));
      feedTelemitryObject.Add("UpstreamEnabled", feed.UpstreamEnabled);
      feedTelemitryObject.Add("AllowUpstreamNameConflict", feed.AllowUpstreamNameConflict);
      feedTelemitryObject.Add("FeedCapabilities", feed.Capabilities.ToString());
      FeedCiPublisher.Publish(requestContext, "UpdateFeed", feedTelemitryObject);
    }

    public static void PublishDeleteFeedEvent(IVssRequestContext requestContext, Guid feedId)
    {
      CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
      telemetryData.Add("FeedId", feedId.ToString());
      FeedCiPublisher.Publish(requestContext, "DeleteFeed", telemetryData);
    }

    public static void PublishPermanentDeleteFeedEvent(
      IVssRequestContext requestContext,
      Guid feedId)
    {
      CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
      telemetryData.Add("FeedId", feedId.ToString());
      FeedCiPublisher.Publish(requestContext, "PermanentDeleteFeed", telemetryData);
    }

    public static void PublishRestoreDeletedFeedEvent(
      IVssRequestContext requestContext,
      Guid feedId)
    {
      CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
      telemetryData.Add("FeedId", feedId.ToString());
      FeedCiPublisher.Publish(requestContext, "RestoreDeletedFeed", telemetryData);
    }

    public static void PublishCreateFeedPermissionEvent(
      IVssRequestContext requestContext,
      Guid feedId,
      IEnumerable<FeedPermission> feedPermissions)
    {
      FeedCiPublisher.PublishFeedPermissionEvent(requestContext, feedId, feedPermissions, "CreateFeedPermissions");
    }

    public static void PublishSetFeedPermissionEvent(
      IVssRequestContext requestContext,
      Guid feedId,
      IEnumerable<FeedPermission> feedPermissions)
    {
      FeedCiPublisher.PublishFeedPermissionEvent(requestContext, feedId, feedPermissions, "SetFeedPermissions");
    }

    public static void PublishSetGlobalCreateFeedPermissionEvent(
      IVssRequestContext requestContext,
      IEnumerable<GlobalPermission> globalPermissions)
    {
      FeedCiPublisher.PublishGlobalCreateFeedPermissionEvent(requestContext, globalPermissions, "SetGlobalCreateFeedPermissions");
    }

    public static void PublishSetPackageRetentionPolicy(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int versionsToKeep,
      int? daysToKeep)
    {
      CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
      feedTelemitryObject.Add("Enabled", true);
      feedTelemitryObject.Add("VersionsToKeep", (double) versionsToKeep);
      feedTelemitryObject.Add("DaysToKeepRecentlyDownloadedPackages", (object) daysToKeep);
      FeedCiPublisher.Publish(requestContext, "SetPackageRetention", feedTelemitryObject);
    }

    public static void PublishDeletePackageRetentionPolicy(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
      feedTelemitryObject.Add("Enabled", false);
      FeedCiPublisher.Publish(requestContext, "SetPackageRetention", feedTelemitryObject);
    }

    public static void PublishDeletePackageFromRetentionEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolName,
      IEnumerable<PackageVersionIdentity> VersionsDeleted)
    {
      foreach (PackageVersionIdentity packageVersionIdentity in VersionsDeleted)
      {
        CustomerIntelligenceData feedTelemitryObject = FeedCiPublisher.GetFeedTelemitryObject(feed);
        feedTelemitryObject.Add("ProtocolName", protocolName);
        feedTelemitryObject.Add("PackageName", packageVersionIdentity.PackageName);
        feedTelemitryObject.Add("PackageVersion", packageVersionIdentity.Version);
        FeedCiPublisher.Publish(requestContext, "RetentionPolicyDeletePackage", feedTelemitryObject);
      }
    }

    private static void PublishFeedPermissionEvent(
      IVssRequestContext requestContext,
      Guid feedId,
      IEnumerable<FeedPermission> feedPermissions,
      string feature)
    {
      CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
      List<\u003C\u003Ef__AnonymousType0<FeedRole, int>> list = feedPermissions.GroupBy<FeedPermission, FeedRole>((Func<FeedPermission, FeedRole>) (x => x.Role)).Select(x => new
      {
        Role = x.Key,
        Count = x.Count<FeedPermission>()
      }).ToList();
      telemetryData.Add("FeedId", feedId.ToString());
      telemetryData.Add("SetPermissions", (object) list);
      FeedCiPublisher.Publish(requestContext, feature, telemetryData);
    }

    private static void PublishGlobalCreateFeedPermissionEvent(
      IVssRequestContext requestContext,
      IEnumerable<GlobalPermission> globalPermissions,
      string feature)
    {
      CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
      List<\u003C\u003Ef__AnonymousType0<GlobalRole, int>> list = globalPermissions.GroupBy<GlobalPermission, GlobalRole>((Func<GlobalPermission, GlobalRole>) (x => x.Role)).Select(x => new
      {
        Role = x.Key,
        Count = x.Count<GlobalPermission>()
      }).ToList();
      telemetryData.Add("GlobalCreateFeedPermissions", (object) list);
      FeedCiPublisher.Publish(requestContext, feature, telemetryData);
    }

    private static void Publish(
      IVssRequestContext requestContext,
      string featureName,
      CustomerIntelligenceData telemetryData)
    {
      try
      {
        FeedCiPublisher.AccountCollectionInformation collectionInformation = FeedCiPublisher.GetAccountCollectionInformation(requestContext);
        telemetryData.Add("ActivityId", requestContext.ActivityId.ToString());
        telemetryData.Add("AccountId", collectionInformation.AccountId);
        telemetryData.Add("AccountName", collectionInformation.AccountName);
        telemetryData.Add("CollectionId", collectionInformation.CollectionId);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Packaging", featureName, telemetryData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10019057, "Feed", "CiPublishing", ex);
      }
    }

    private static FeedCiPublisher.AccountCollectionInformation GetAccountCollectionInformation(
      IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new InvalidRequestContextHostException("Function is expecting a host of type " + Enum.GetName(typeof (TeamFoundationHostType), (object) TeamFoundationHostType.ProjectCollection));
      return new FeedCiPublisher.AccountCollectionInformation()
      {
        AccountId = requestContext.ServiceHost.ParentServiceHost.InstanceId.ToString(),
        AccountName = requestContext.ServiceHost.ParentServiceHost.Name,
        CollectionId = requestContext.ServiceHost.InstanceId.ToString()
      };
    }

    private static CustomerIntelligenceData GetFeedTelemitryObject(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      CustomerIntelligenceData feedTelemitryObject = new CustomerIntelligenceData();
      feedTelemitryObject.Add("FeedId", feed.Id.ToString());
      feedTelemitryObject.Add("FeedName", feed.Name);
      return feedTelemitryObject;
    }

    private class AccountCollectionInformation
    {
      public string AccountId { get; set; }

      public string AccountName { get; set; }

      public string CollectionId { get; set; }
    }
  }
}
