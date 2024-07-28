// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedBinder : ObjectBinder<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder feedId = new SqlColumnBinder("FeedId");
    private SqlColumnBinder feedName = new SqlColumnBinder("FeedName");
    private SqlColumnBinder feedDescription = new SqlColumnBinder("FeedDescription");
    private SqlColumnBinder upstreamEnabled = new SqlColumnBinder("UpstreamEnabled");
    private SqlColumnBinder upstreamEnabledChangedDate = new SqlColumnBinder("UpstreamEnabledChangedDate");
    private SqlColumnBinder allowUpstreamNameConflict = new SqlColumnBinder("AllowUpstreamNameConflict");
    private SqlColumnBinder hideDeletedPackageVersions = new SqlColumnBinder("HideDeletedPackageVersions");
    private SqlColumnBinder defaultReaderView = new SqlColumnBinder("DefaultReaderView");
    private SqlColumnBinder upstreamSources = new SqlColumnBinder("UpstreamSources");
    private SqlColumnBinder capabilities = new SqlColumnBinder("Capabilities");
    private SqlColumnBinder badgesEnabled = new SqlColumnBinder("BadgesEnabled");
    private SqlColumnBinder deletedDate = new SqlColumnBinder("DeletedDate");
    private SqlColumnBinder scheduledPermanentDeleteDate = new SqlColumnBinder("ScheduledPermanentDeleteDate");
    private readonly TeamFoundationSqlResourceComponent sqlComponent;

    public FeedBinder(TeamFoundationSqlResourceComponent sqlComponent) => this.sqlComponent = sqlComponent;

    protected override Microsoft.VisualStudio.Services.Feed.WebApi.Feed Bind()
    {
      DateTime dateTime = this.upstreamEnabledChangedDate.ColumnExists((IDataReader) this.Reader) ? this.upstreamEnabledChangedDate.GetDateTime((IDataReader) this.Reader) : DateTime.MinValue;
      bool flag = dateTime != DateTime.MinValue;
      bool hideDeletedPackageVersions = this.hideDeletedPackageVersions.ColumnExists((IDataReader) this.Reader) && this.hideDeletedPackageVersions.GetBoolean((IDataReader) this.Reader);
      Guid valueOrDefault = this.defaultReaderView.GetNullableGuid((IDataReader) this.Reader).GetValueOrDefault();
      FeedCapabilities int32 = (FeedCapabilities) this.capabilities.GetInt32((IDataReader) this.Reader, 0, 0);
      int? dataspaceId = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? new int?(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : new int?();
      DateTime? deletedDate = this.deletedDate.ColumnExists((IDataReader) this.Reader) ? new DateTime?(this.deletedDate.GetDateTime((IDataReader) this.Reader)) : new DateTime?();
      DateTime? scheduledPermanentDeleteDate = this.scheduledPermanentDeleteDate.ColumnExists((IDataReader) this.Reader) ? new DateTime?(this.scheduledPermanentDeleteDate.GetDateTime((IDataReader) this.Reader)) : new DateTime?();
      string json = this.upstreamSources.GetString((IDataReader) this.Reader, (string) null);
      List<UpstreamSource> upstreams = json != null ? JsonUtilities.Deserialize<List<UpstreamSource>>(json) : new List<UpstreamSource>();
      return FeedBinder.CreateFeed(this.feedId.GetGuid((IDataReader) this.Reader), this.feedName.GetString((IDataReader) this.Reader, false), this.feedDescription.GetString((IDataReader) this.Reader, true), this.upstreamEnabled.GetBoolean((IDataReader) this.Reader), flag ? new DateTime?(dateTime) : new DateTime?(), this.allowUpstreamNameConflict.ColumnExists((IDataReader) this.Reader) && this.allowUpstreamNameConflict.GetBoolean((IDataReader) this.Reader), hideDeletedPackageVersions, (IList<UpstreamSource>) upstreams, valueOrDefault, int32, this.badgesEnabled.ColumnExists((IDataReader) this.Reader) && this.badgesEnabled.GetBoolean((IDataReader) this.Reader), ProjectHelper.ConvertDataspaceToProjectReference(dataspaceId, this.sqlComponent), deletedDate, scheduledPermanentDeleteDate);
    }

    internal static Microsoft.VisualStudio.Services.Feed.WebApi.Feed DeepCloneFeed(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => feed == null ? (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null : FeedBinder.CreateFeed(feed.Id, feed.Name, feed.Description, feed.UpstreamEnabled, feed.UpstreamEnabledChangedDate, feed.AllowUpstreamNameConflict, feed.HideDeletedPackageVersions, feed.UpstreamSources, feed.DefaultViewId, feed.Capabilities, feed.BadgesEnabled, feed.Project, feed.DeletedDate, feed.ScheduledPermanentDeleteDate);

    private static Microsoft.VisualStudio.Services.Feed.WebApi.Feed CreateFeed(
      Guid id,
      string name,
      string description,
      bool upstreamEnabled,
      DateTime? upstreamEnabledChangedDate,
      bool allowUpstreamNameConflict,
      bool hideDeletedPackageVersions,
      IList<UpstreamSource> upstreams,
      Guid defaultReaderView,
      FeedCapabilities capabilities,
      bool badgesEnabled,
      ProjectReference project,
      DateTime? deletedDate,
      DateTime? scheduledPermanentDeleteDate)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = new Microsoft.VisualStudio.Services.Feed.WebApi.Feed();
      feed.Id = id;
      feed.Name = name;
      feed.Description = description;
      feed.UpstreamEnabled = upstreamEnabled;
      feed.UpstreamEnabledChangedDate = upstreamEnabledChangedDate;
      feed.AllowUpstreamNameConflict = allowUpstreamNameConflict;
      feed.HideDeletedPackageVersions = hideDeletedPackageVersions;
      feed.UpstreamSources = upstreams != null ? (IList<UpstreamSource>) new List<UpstreamSource>((IEnumerable<UpstreamSource>) upstreams) : (IList<UpstreamSource>) null;
      feed.DefaultViewId = defaultReaderView;
      feed.Capabilities = capabilities;
      feed.BadgesEnabled = badgesEnabled;
      feed.Project = project;
      feed.DeletedDate = deletedDate;
      feed.ScheduledPermanentDeleteDate = scheduledPermanentDeleteDate;
      return feed;
    }
  }
}
