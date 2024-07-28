// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.Notification.PackageChangedEvent
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.Common.Notification
{
  [DataContract]
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.azure-artifacts.packagemanagement-packagechanged-event")]
  public class PackageChangedEvent : PackageEventData
  {
    [DataMember]
    public PackageChangeType PublishType { get; set; }

    [DataMember(IsRequired = false)]
    public PackageVersionNotificationData VersionData { get; set; }

    public PackageChangedEvent()
    {
    }

    public PackageChangedEvent(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      PackageIndexEntry package,
      PackageIndexEntryResponse response,
      PackageChangeType publishType = PackageChangeType.Publish)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, nameof (feed));
      ArgumentUtility.CheckForEmptyGuid(feed.Id, "feed.Id");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(feed.Name, "feed.Name");
      ArgumentUtility.CheckForNull<PackageIndexEntry>(package, nameof (package));
      ArgumentUtility.CheckForNull<PackageIndexEntryResponse>(response, nameof (response));
      this.PublishType = publishType;
      this.Feed = this.CloneFeed(feed);
      this.Package = new Package()
      {
        Id = response.PackageId,
        Name = package.Name,
        ProtocolType = package.ProtocolType,
        NormalizedName = package.NormalizedName
      };
      this.PackageVersion = new MinimalPackageVersion()
      {
        Id = response.PackageVersionId,
        PackageDescription = package.PackageVersion.Description,
        Version = package.PackageVersion.Version,
        NormalizedVersion = package.PackageVersion.NormalizedVersion,
        PublishDate = package.PackageVersion.PublishDate
      };
      this.VersionData = new PackageVersionNotificationData()
      {
        Author = package.PackageVersion.Author
      };
    }

    public PackageChangedEvent(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IPackageInfo package,
      Microsoft.VisualStudio.Services.Feed.WebApi.PackageVersion version,
      PackageChangeType publishType = PackageChangeType.Promote)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, nameof (feed));
      ArgumentUtility.CheckForEmptyGuid(feed.Id, "feed.Id");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(feed.Name, "feed.Name");
      ArgumentUtility.CheckForNull<IPackageInfo>(package, nameof (package));
      this.PublishType = publishType;
      this.Feed = this.CloneFeed(feed);
      this.Package = new Package()
      {
        Id = package.Id,
        Name = package.Name,
        ProtocolType = package.ProtocolType,
        NormalizedName = package.NormalizedName
      };
      if (version == null)
        return;
      this.PackageVersion = new MinimalPackageVersion()
      {
        Id = version.Id,
        PackageDescription = version.Description,
        Version = version.Version,
        NormalizedVersion = version.NormalizedVersion,
        PublishDate = version.PublishDate
      };
      this.VersionData = new PackageVersionNotificationData()
      {
        Author = version.Author
      };
    }

    public override string ToString() => string.Format("{0}[{1}] {{FeedId:{2} FeedName:{3} PackageId:{4} PackageVersionId:{5}}}", (object) nameof (PackageChangedEvent), (object) this.PublishType, (object) this.Feed.Id, (object) this.Feed.Name, (object) this.Package.Id, (object) this.PackageVersion?.Id);

    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed CloneFeed(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = new Microsoft.VisualStudio.Services.Feed.WebApi.Feed();
      feed1.Id = feed.Id;
      feed1.Name = feed.Name;
      feed1.Project = feed.Project;
      feed1.UpstreamEnabled = feed.UpstreamEnabled;
      feed1.AllowUpstreamNameConflict = feed.AllowUpstreamNameConflict;
      feed1.View = feed.View;
      feed1.Capabilities = feed.Capabilities;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed2 = feed1;
      if (feed.UpstreamSources != null)
        feed2.UpstreamSources = (IList<UpstreamSource>) new List<UpstreamSource>((IEnumerable<UpstreamSource>) feed.UpstreamSources);
      return feed2;
    }
  }
}
