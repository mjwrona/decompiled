// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component78
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component78 : Build2Component77
  {
    protected static readonly SqlMetaData[] typ_CleanupArtifactRecords = new SqlMetaData[3]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("ArtifactId", SqlDbType.Int)
    };

    public override async Task<IList<ArtifactCleanupRecord>> GetArtifactsToCleanUp()
    {
      Build2Component78 build2Component78 = this;
      IList<ArtifactCleanupRecord> list;
      using (build2Component78.TraceScope(method: nameof (GetArtifactsToCleanUp)))
      {
        build2Component78.PrepareStoredProcedure("Build.prc_GetArtifactsToCleanUp");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component78.ExecuteReaderAsync(), build2Component78.ProcedureName, build2Component78.RequestContext))
        {
          resultCollection.AddBinder<ArtifactCleanupRecord>((ObjectBinder<ArtifactCleanupRecord>) build2Component78.GetArtifactCleanupRecordBinder());
          list = (IList<ArtifactCleanupRecord>) resultCollection.GetCurrent<ArtifactCleanupRecord>().Items.ToList<ArtifactCleanupRecord>();
        }
      }
      return list;
    }

    public override async Task DeleteArtifactCleanupRecords(
      IList<ArtifactCleanupRecordKey> artifactRecords)
    {
      Build2Component78 build2Component78 = this;
      using (build2Component78.TraceScope(method: nameof (DeleteArtifactCleanupRecords)))
      {
        build2Component78.PrepareStoredProcedure("Build.prc_DeleteArtifactCleanupRecords");
        build2Component78.BindArtifactCleanupRecordKeyTable("@recordTable", (IEnumerable<ArtifactCleanupRecordKey>) artifactRecords);
        int num = await build2Component78.ExecuteNonQueryAsync();
      }
    }

    protected virtual BuildObjectBinder<ArtifactCleanupRecord> GetArtifactCleanupRecordBinder() => (BuildObjectBinder<ArtifactCleanupRecord>) new ArtifactCleanupRecordBinder(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual SqlParameter BindArtifactCleanupRecordKeyTable(
      string parameterName,
      IEnumerable<ArtifactCleanupRecordKey> keys)
    {
      keys = keys ?? Enumerable.Empty<ArtifactCleanupRecordKey>();
      return this.BindTable(parameterName, "Build.typ_CleanupArtifactRecords", (IEnumerable<SqlDataRecord>) keys.Select<ArtifactCleanupRecordKey, SqlDataRecord>(new System.Func<ArtifactCleanupRecordKey, SqlDataRecord>(rowBinder)).ToArray<SqlDataRecord>());

      static SqlDataRecord rowBinder(ArtifactCleanupRecordKey key)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(Build2Component78.typ_CleanupArtifactRecords);
        sqlDataRecord.SetInt32(0, key.DataspaceId);
        sqlDataRecord.SetInt32(1, key.BuildId);
        sqlDataRecord.SetInt32(2, key.ArtifactId);
        return sqlDataRecord;
      }
    }
  }
}
