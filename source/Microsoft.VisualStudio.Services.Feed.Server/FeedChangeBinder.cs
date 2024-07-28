// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedChangeBinder
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
  internal class FeedChangeBinder : ObjectBinder<FeedChange>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder feedId = new SqlColumnBinder("FeedId");
    private SqlColumnBinder feedName = new SqlColumnBinder("FeedName");
    private SqlColumnBinder feedDescription = new SqlColumnBinder("FeedDescription");
    private SqlColumnBinder deletedDate = new SqlColumnBinder("DeletedDate");
    private SqlColumnBinder permanentDeletedDate = new SqlColumnBinder("PermanentDeletedDate");
    private SqlColumnBinder upstreamEnabled = new SqlColumnBinder("UpstreamEnabled");
    private SqlColumnBinder upstreamEnabledChangedDate = new SqlColumnBinder("UpstreamEnabledChangedDate");
    private SqlColumnBinder allowUpstreamNameConflict = new SqlColumnBinder("AllowUpstreamNameConflict");
    private SqlColumnBinder feedContinuationToken = new SqlColumnBinder("FeedToken");
    private SqlColumnBinder latestPackageContinuationToken = new SqlColumnBinder("LatestPackageToken");
    private SqlColumnBinder defaultReaderView = new SqlColumnBinder("DefaultReaderView");
    private SqlColumnBinder capabilities = new SqlColumnBinder("Capabilities");
    private readonly TeamFoundationSqlResourceComponent sqlComponent;

    public FeedChangeBinder(TeamFoundationSqlResourceComponent sqlComponent) => this.sqlComponent = sqlComponent;

    protected override FeedChange Bind()
    {
      int? dataspaceId = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? new int?(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : new int?();
      DateTime dateTime1 = this.deletedDate.GetDateTime((IDataReader) this.Reader);
      DateTime dateTime2 = this.permanentDeletedDate.ColumnExists((IDataReader) this.Reader) ? this.permanentDeletedDate.GetDateTime((IDataReader) this.Reader) : DateTime.MinValue;
      bool flag1 = dateTime1 != DateTime.MinValue;
      bool flag2 = dateTime2 != DateTime.MinValue;
      DateTime dateTime3 = this.upstreamEnabledChangedDate.ColumnExists((IDataReader) this.Reader) ? this.upstreamEnabledChangedDate.GetDateTime((IDataReader) this.Reader) : DateTime.MinValue;
      bool flag3 = dateTime3 != DateTime.MinValue;
      Guid valueOrDefault = this.defaultReaderView.GetNullableGuid((IDataReader) this.Reader).GetValueOrDefault();
      FeedCapabilities int32 = (FeedCapabilities) this.capabilities.GetInt32((IDataReader) this.Reader, 0, 0);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = new Microsoft.VisualStudio.Services.Feed.WebApi.Feed();
      feed1.Id = this.feedId.GetGuid((IDataReader) this.Reader);
      feed1.Name = this.feedName.GetString((IDataReader) this.Reader, true);
      feed1.Description = this.feedDescription.GetString((IDataReader) this.Reader, true);
      feed1.UpstreamEnabled = this.upstreamEnabled.GetBoolean((IDataReader) this.Reader, false);
      feed1.UpstreamEnabledChangedDate = flag3 ? new DateTime?(dateTime3) : new DateTime?();
      feed1.AllowUpstreamNameConflict = this.allowUpstreamNameConflict.ColumnExists((IDataReader) this.Reader) && this.allowUpstreamNameConflict.GetBoolean((IDataReader) this.Reader, false);
      feed1.DeletedDate = flag1 ? new DateTime?(dateTime1) : new DateTime?();
      feed1.PermanentDeletedDate = flag2 ? new DateTime?(dateTime2) : new DateTime?();
      feed1.DefaultViewId = valueOrDefault;
      feed1.Capabilities = int32;
      feed1.UpstreamSources = (IList<UpstreamSource>) new List<UpstreamSource>();
      feed1.Project = ProjectHelper.ConvertDataspaceToProjectReference(dataspaceId, this.sqlComponent);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed2 = feed1;
      ChangeType changeType = !flag2 ? (!flag1 ? ChangeType.AddOrUpdate : ChangeType.Delete) : ChangeType.PermanentDelete;
      return new FeedChange()
      {
        Feed = feed2,
        ChangeType = changeType,
        FeedContinuationToken = this.feedContinuationToken.GetInt64((IDataReader) this.Reader, 0L),
        LatestPackageContinuationToken = this.latestPackageContinuationToken.GetInt64((IDataReader) this.Reader, 0L)
      };
    }
  }
}
