// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Data.CodeSenseSqlResourceComponent3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Data
{
  internal class CodeSenseSqlResourceComponent3 : CodeSenseSqlResourceComponent2
  {
    protected readonly CodeSenseSqlResourceComponent3.Columns3 columns3 = new CodeSenseSqlResourceComponent3.Columns3();
    private SqlMetaData[] typ_FileIdTable = new SqlMetaData[1]
    {
      new SqlMetaData("FileId", SqlDbType.Int)
    };

    public override void AddFilesToDelete(IEnumerable<int> fileIds)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024780, TraceLayer.ExternalSql, "AddFilesToDelete()", Array.Empty<object>()))
        this.AddFilesToDeleteCore(fileIds);
    }

    public override int GetFilesPendingForDeletion(DateTime createdBefore)
    {
      this.PrepareStoredProcedure("CodeSense.prc_LegacyGetOrphanedFiles");
      this.BindDateTime("@createdBefore", createdBefore);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ProjectionBinder<int>((System.Func<SqlDataReader, int>) (reader => this.columns3.FileId.GetInt32((IDataReader) reader))));
        return resultCollection.GetCurrent<int>().Items.ToReadOnlyList<int>().Count;
      }
    }

    private void AddFilesToDeleteCore(IEnumerable<int> fileIds)
    {
      this.PrepareStoredProcedure("CodeSense.prc_AddFilesToDelete");
      this.BindFileIdTable("@filesToDelete", fileIds);
      this.ExecuteNonQuery(false);
    }

    private SqlParameter BindFileIdTable(string parameterName, IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (fileId =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_FileIdTable);
        sqlDataRecord.SetInt32(0, fileId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "CodeSense.typ_FileIdTable", rows.Select<int, SqlDataRecord>(selector));
    }

    protected class Columns3 : CodeSenseSqlResourceComponent2.Columns2
    {
      public SqlColumnBinder FileId = new SqlColumnBinder(nameof (FileId));
    }
  }
}
