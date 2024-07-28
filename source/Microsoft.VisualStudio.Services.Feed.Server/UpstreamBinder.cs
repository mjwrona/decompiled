// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.UpstreamBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class UpstreamBinder : ObjectBinder<UpstreamSourceSql>
  {
    private SqlColumnBinder feedId = new SqlColumnBinder("FeedId");
    private SqlColumnBinder upstreamId = new SqlColumnBinder("UpstreamId");
    private SqlColumnBinder upstreamName = new SqlColumnBinder("UpstreamName");
    private SqlColumnBinder protocolType = new SqlColumnBinder("ProtocolType");
    private SqlColumnBinder location = new SqlColumnBinder("Location");
    private SqlColumnBinder upstreamSourceType = new SqlColumnBinder("UpstreamSourceType");
    private SqlColumnBinder internalUpstreamCollectionId = new SqlColumnBinder("InternalUpstreamCollectionId");
    private SqlColumnBinder internalUpstreamProjectId = new SqlColumnBinder("InternalUpstreamProjectId");
    private SqlColumnBinder internalUpstreamFeedId = new SqlColumnBinder("InternalUpstreamFeedId");
    private SqlColumnBinder internalUpstreamViewId = new SqlColumnBinder("InternalUpstreamViewId");
    private SqlColumnBinder deletedDate = new SqlColumnBinder("DeletedDate");
    private SqlColumnBinder serviceEndpointId = new SqlColumnBinder("ServiceEndpointId");
    private SqlColumnBinder serviceEndpointProjectId = new SqlColumnBinder("ServiceEndpointProjectId");
    private Guid hostId;

    public UpstreamBinder(Guid hostId) => this.hostId = hostId;

    protected override UpstreamSourceSql Bind()
    {
      Guid guid1 = this.feedId.GetGuid((IDataReader) this.Reader);
      Guid guid2 = this.upstreamId.GetGuid((IDataReader) this.Reader);
      string str1 = this.upstreamName.GetString((IDataReader) this.Reader, false);
      string str2 = this.protocolType.GetString((IDataReader) this.Reader, false);
      string str3 = this.location.GetString((IDataReader) this.Reader, false);
      int int32 = this.upstreamSourceType.GetInt32((IDataReader) this.Reader);
      DateTime dateTime = this.deletedDate.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      bool flag = dateTime != DateTime.MinValue;
      Guid? nullable = this.internalUpstreamCollectionId.GetNullableGuid((IDataReader) this.Reader, new Guid?());
      Guid? nullableGuid1 = this.internalUpstreamProjectId.GetNullableGuid((IDataReader) this.Reader, new Guid?());
      Guid? nullableGuid2 = this.internalUpstreamFeedId.GetNullableGuid((IDataReader) this.Reader, new Guid?());
      Guid? nullableGuid3 = this.internalUpstreamViewId.GetNullableGuid((IDataReader) this.Reader, new Guid?());
      Guid? nullableGuid4 = this.serviceEndpointId.GetNullableGuid((IDataReader) this.Reader, new Guid?());
      Guid? nullableGuid5 = this.serviceEndpointProjectId.GetNullableGuid((IDataReader) this.Reader, new Guid?());
      if (!nullable.HasValue && nullableGuid2.HasValue && nullableGuid3.HasValue)
        nullable = new Guid?(this.hostId);
      return new UpstreamSourceSql()
      {
        FeedId = guid1,
        Id = guid2,
        Name = str1,
        Protocol = str2,
        Location = str3,
        UpstreamSourceType = (UpstreamSourceType) int32,
        DeletedDate = flag ? new DateTime?(dateTime) : new DateTime?(),
        InternalUpstreamCollectionId = nullable,
        InternalUpstreamProjectId = nullableGuid1,
        InternalUpstreamFeedId = nullableGuid2,
        InternalUpstreamViewId = nullableGuid3,
        ServiceEndpointId = nullableGuid4,
        ServiceEndpointProjectId = nullableGuid5
      };
    }
  }
}
