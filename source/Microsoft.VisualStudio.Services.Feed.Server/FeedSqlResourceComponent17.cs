// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlResourceComponent17
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedSqlResourceComponent17 : FeedSqlResourceComponent16
  {
    protected override SqlMetaData[] typ_Upstream => new SqlMetaData[12]
    {
      new SqlMetaData("UpstreamId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UpstreamName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("ProtocolType", SqlDbType.NVarChar, 20L),
      new SqlMetaData("Location", SqlDbType.NVarChar, 2000L),
      new SqlMetaData("UpstreamSourceType", SqlDbType.Int),
      new SqlMetaData("UpstreamIndex", SqlDbType.Int),
      new SqlMetaData("InternalUpstreamCollectionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("InternalUpstreamProjectId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("InternalUpstreamFeedId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("InternalUpstreamViewId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ServiceEndpointId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ServiceEndpointProjectId", SqlDbType.UniqueIdentifier)
    };

    protected override SqlParameter BindUpstreamTableParameter(
      string parameterName,
      IEnumerable<UpstreamSource> rows)
    {
      rows = rows ?? Enumerable.Empty<UpstreamSource>();
      rows = rows.Where<UpstreamSource>((System.Func<UpstreamSource, bool>) (x => x != null));
      IEnumerable<UpstreamSourceSql> source = rows.Select<UpstreamSource, UpstreamSourceSql>((System.Func<UpstreamSource, UpstreamSourceSql>) (x => new UpstreamSourceSql(x)));
      Func<UpstreamSourceSql, int, IEnumerable<SqlDataRecord>> selector = (Func<UpstreamSourceSql, int, IEnumerable<SqlDataRecord>>) ((row, index) =>
      {
        SqlDataRecord record = new SqlDataRecord(this.typ_Upstream);
        record.SetGuid(0, row.Id);
        record.SetString(1, row.Name, BindStringBehavior.EmptyStringToNull);
        record.SetString(2, row.Protocol, BindStringBehavior.EmptyStringToNull);
        record.SetString(3, row.Location, BindStringBehavior.EmptyStringToNull);
        record.SetInt32(4, (int) row.UpstreamSourceType);
        record.SetInt32(5, index);
        SqlDataRecord sqlDataRecord1 = record;
        Guid? nullable;
        SqlGuid sqlGuid1;
        if (!row.InternalUpstreamCollectionId.HasValue)
        {
          sqlGuid1 = SqlGuid.Null;
        }
        else
        {
          nullable = row.InternalUpstreamCollectionId;
          sqlGuid1 = (SqlGuid) nullable.Value;
        }
        sqlDataRecord1.SetSqlGuid(6, sqlGuid1);
        SqlDataRecord sqlDataRecord2 = record;
        nullable = row.InternalUpstreamProjectId;
        SqlGuid sqlGuid2;
        if (!nullable.HasValue)
        {
          sqlGuid2 = SqlGuid.Null;
        }
        else
        {
          nullable = row.InternalUpstreamProjectId;
          sqlGuid2 = (SqlGuid) nullable.Value;
        }
        sqlDataRecord2.SetSqlGuid(7, sqlGuid2);
        SqlDataRecord sqlDataRecord3 = record;
        nullable = row.InternalUpstreamFeedId;
        SqlGuid sqlGuid3;
        if (!nullable.HasValue)
        {
          sqlGuid3 = SqlGuid.Null;
        }
        else
        {
          nullable = row.InternalUpstreamFeedId;
          sqlGuid3 = (SqlGuid) nullable.Value;
        }
        sqlDataRecord3.SetSqlGuid(8, sqlGuid3);
        SqlDataRecord sqlDataRecord4 = record;
        nullable = row.InternalUpstreamViewId;
        SqlGuid sqlGuid4;
        if (!nullable.HasValue)
        {
          sqlGuid4 = SqlGuid.Null;
        }
        else
        {
          nullable = row.InternalUpstreamViewId;
          sqlGuid4 = (SqlGuid) nullable.Value;
        }
        sqlDataRecord4.SetSqlGuid(9, sqlGuid4);
        SqlDataRecord sqlDataRecord5 = record;
        nullable = row.ServiceEndpointId;
        SqlGuid sqlGuid5;
        if (!nullable.HasValue)
        {
          sqlGuid5 = SqlGuid.Null;
        }
        else
        {
          nullable = row.ServiceEndpointId;
          sqlGuid5 = (SqlGuid) nullable.Value;
        }
        sqlDataRecord5.SetSqlGuid(10, sqlGuid5);
        SqlDataRecord sqlDataRecord6 = record;
        nullable = row.ServiceEndpointProjectId;
        SqlGuid sqlGuid6;
        if (!nullable.HasValue)
        {
          sqlGuid6 = SqlGuid.Null;
        }
        else
        {
          nullable = row.ServiceEndpointProjectId;
          sqlGuid6 = (SqlGuid) nullable.Value;
        }
        sqlDataRecord6.SetSqlGuid(11, sqlGuid6);
        return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
        {
          record
        };
      });
      return this.BindTable(parameterName, "Feed.typ_Upstream4", source.SelectMany<UpstreamSourceSql, SqlDataRecord>(selector));
    }
  }
}
