// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRetentionPolicyStore
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Telemetry;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageRetentionPolicyStore : IPackageRetentionPolicyStore
  {
    public const string FeedRetentionPolicyPropertyNamePrefix = "Feed.Retention";
    private const string PropertyNameWildCard = "Feed.Retention.*";
    private const string VersionCountPropertyName = "Feed.Retention.VersionCount.MaxVersionsCount";
    private const string DaysToKeepRecentlyDownloadedVersionsPropertyName = "Feed.Retention.VersionCount.DaysToKeepRecentlyDownloadedPackages";
    private const int DefaultMaxVersionsPerPackageName = 5000;
    public const int RetentionPolicyMinimumCountLimit = 1;
    public const int RetentionPolicyMaximumCountLimit = 5000;
    public const int RetentionPolicyMinimumDaysToKeepRecentlyDownloadedPackages = 1;
    public const int RetentionPolicyMaximumDaysToKeepRecentlyDownloadedPackages = 365;
    public const string RetentionPolicyDefaultVersionCountDictionaryName = "versionCount";
    public const string RetentionPolicyDefaultDaysToKeepRecentlyDownloadedPackagesDictionaryName = "daysToKeepRecentlyDownloadedPackages";

    public FeedRetentionPolicy GetPolicy(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, nameof (feed));
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, (FeedCore) feed);
      ArtifactSpec feedArtifactSpec = PackageRetentionPolicyStore.GetFeedArtifactSpec(feed.Id);
      IList<ArtifactPropertyValue> list;
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, feedArtifactSpec, (IEnumerable<string>) new string[1]
      {
        "Feed.Retention.*"
      }))
      {
        list = (IList<ArtifactPropertyValue>) properties.CurrentEnumerable<ArtifactPropertyValue>().ToList<ArtifactPropertyValue>();
        if (!list.Any<ArtifactPropertyValue>())
          return (FeedRetentionPolicy) null;
      }
      FeedRetentionPolicy policy = new FeedRetentionPolicy();
      Dictionary<string, int> dictionary = this.LoadRetentionPolicyDefaultValues(requestContext);
      policy.DaysToKeepRecentlyDownloadedPackages = new int?(dictionary["daysToKeepRecentlyDownloadedPackages"]);
      foreach (PropertyValue propertyValue in list.Single<ArtifactPropertyValue>().PropertyValues)
      {
        if (propertyValue.PropertyName.Equals("Feed.Retention.VersionCount.MaxVersionsCount"))
        {
          int result;
          if (int.TryParse(propertyValue.Value.ToString(), out result))
            policy.CountLimit = new int?(result);
        }
        else
        {
          int result;
          if (propertyValue.PropertyName.Equals("Feed.Retention.VersionCount.DaysToKeepRecentlyDownloadedPackages") && int.TryParse(propertyValue.Value.ToString(), out result))
            policy.DaysToKeepRecentlyDownloadedPackages = new int?(result);
        }
      }
      return policy;
    }

    public void SetPolicy(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, FeedRetentionPolicy policy)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<FeedRetentionPolicy>(policy, nameof (policy));
      if (!policy.CountLimit.HasValue)
        throw new InvalidUserInputException(Resources.Error_PackageRetentionInvalidPolicy());
      if (!policy.DaysToKeepRecentlyDownloadedPackages.HasValue)
        throw new InvalidUserInputException(Resources.Error_PackageRetentionInvalidPolicy());
      ArgumentUtility.CheckBoundsInclusive(policy.CountLimit.Value, 1, 5000, "CountLimit");
      FeedSecurityHelper.CheckEditFeedPermissions(requestContext, (FeedCore) feed);
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec feedArtifactSpec = PackageRetentionPolicyStore.GetFeedArtifactSpec(feed.Id);
      ArgumentUtility.CheckBoundsInclusive(policy.DaysToKeepRecentlyDownloadedPackages.Value, 1, 365, "DaysToKeepRecentlyDownloadedPackages");
      PropertyValue[] propertyValueArray = new PropertyValue[2]
      {
        new PropertyValue("Feed.Retention.VersionCount.MaxVersionsCount", (object) policy.CountLimit.Value),
        new PropertyValue("Feed.Retention.VersionCount.DaysToKeepRecentlyDownloadedPackages", (object) policy.DaysToKeepRecentlyDownloadedPackages.Value)
      };
      if (!service.SetProperties(requestContext, feedArtifactSpec, (IEnumerable<PropertyValue>) propertyValueArray))
        return;
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      int? nullable = policy.CountLimit;
      int versionsToKeep = nullable.Value;
      nullable = policy.DaysToKeepRecentlyDownloadedPackages;
      int? daysToKeep = new int?(nullable.Value);
      FeedCiPublisher.PublishSetPackageRetentionPolicy(requestContext1, feed1, versionsToKeep, daysToKeep);
      FeedAuditHelper.AuditRetentionChange(requestContext, feed, policy);
    }

    public void DeletePolicy(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, nameof (feed));
      FeedSecurityHelper.CheckEditFeedPermissions(requestContext, (FeedCore) feed);
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec feedArtifactSpec = PackageRetentionPolicyStore.GetFeedArtifactSpec(feed.Id);
      IVssRequestContext requestContext1 = requestContext;
      ArtifactSpec[] artifactSpecArray = new ArtifactSpec[1]
      {
        feedArtifactSpec
      };
      service.DeleteArtifacts(requestContext1, (IEnumerable<ArtifactSpec>) artifactSpecArray);
      FeedCiPublisher.PublishDeletePackageRetentionPolicy(requestContext, feed);
      FeedAuditHelper.AuditRetentionChange(requestContext, feed, (FeedRetentionPolicy) null);
    }

    private static ArtifactSpec GetFeedArtifactSpec(Guid feedId) => new ArtifactSpec(FeedConstants.FeedArtifactKindId, feedId.ToByteArray(), 0);

    public ISet<Guid> GetFeeds(IVssRequestContext requestContext)
    {
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, FeedConstants.FeedArtifactKindId, (IEnumerable<string>) new string[1]
      {
        "Feed.Retention.*"
      }))
        return (ISet<Guid>) properties.CurrentEnumerable<ArtifactPropertyValue>().ToHashSet<ArtifactPropertyValue, Guid>((Func<ArtifactPropertyValue, Guid>) (apv => new Guid(apv.Spec.Id)));
    }

    public Dictionary<string, int> LoadRetentionPolicyDefaultValues(
      IVssRequestContext requestContext)
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Feed/PackageRetention/DefaultDaysToKeepRecentlyDownloadedPackagesLimit", true, 30);
      dictionary.Add("daysToKeepRecentlyDownloadedPackages", num1);
      int num2 = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Feed/PackageRetention/DefaultVersionCountLimit", true, 20);
      dictionary.Add("versionCount", num2);
      return dictionary;
    }
  }
}
