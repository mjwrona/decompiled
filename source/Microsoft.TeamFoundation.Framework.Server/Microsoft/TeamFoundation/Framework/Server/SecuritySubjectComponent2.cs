// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuritySubjectComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuritySubjectComponent2 : SecuritySubjectComponent
  {
    private static readonly SqlMetaData[] typ_SecuritySubjectEntryTable = new SqlMetaData[3]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SubjectType", SqlDbType.Int),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L)
    };

    public override IReadOnlyList<SecuritySubjectComponent.SecuritySubjectEntry> QuerySecuritySubjectEntries(
      out long sequenceId)
    {
      this.PrepareStoredProcedure("prc_QuerySecuritySubjectEntries");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new SecuritySubjectComponent2.SequenceIdColumns());
        resultCollection.AddBinder<SecuritySubjectComponent.SecuritySubjectEntry>((ObjectBinder<SecuritySubjectComponent.SecuritySubjectEntry>) new SecuritySubjectComponent2.SecuritySubjectEntryColumns());
        sequenceId = resultCollection.GetCurrent<long>().First<long>();
        resultCollection.NextResult();
        return (IReadOnlyList<SecuritySubjectComponent.SecuritySubjectEntry>) resultCollection.GetCurrent<SecuritySubjectComponent.SecuritySubjectEntry>().Items;
      }
    }

    public override long UpdateSecuritySubjectEntries(
      IEnumerable<SecuritySubjectComponent.SecuritySubjectEntry> entries)
    {
      this.PrepareStoredProcedure("prc_UpdateSecuritySubjectEntries");
      this.BindSecuritySubjectEntriesTable("@entries", entries);
      return (long) this.ExecuteScalar();
    }

    public override long DeleteSecuritySubjectEntries(IEnumerable<Guid> ids)
    {
      this.PrepareStoredProcedure("prc_DeleteSecuritySubjectEntries");
      this.BindGuidTable("@ids", ids);
      return (long) this.ExecuteScalar();
    }

    protected SqlParameter BindSecuritySubjectEntriesTable(
      string parameterName,
      IEnumerable<SecuritySubjectComponent.SecuritySubjectEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<SecuritySubjectComponent.SecuritySubjectEntry>();
      System.Func<SecuritySubjectComponent.SecuritySubjectEntry, SqlDataRecord> selector = (System.Func<SecuritySubjectComponent.SecuritySubjectEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecuritySubjectComponent2.typ_SecuritySubjectEntryTable);
        sqlDataRecord.SetGuid(0, entry.Id);
        sqlDataRecord.SetInt32(1, entry.SubjectType);
        if (!string.IsNullOrEmpty(entry.Description))
          sqlDataRecord.SetString(2, entry.Description);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_SecuritySubjectEntryTable", rows.Select<SecuritySubjectComponent.SecuritySubjectEntry, SqlDataRecord>(selector));
    }

    protected class SequenceIdColumns : ObjectBinder<long>
    {
      private SqlColumnBinder m_sequenceIdColumn = new SqlColumnBinder("SequenceId");

      protected override long Bind() => this.m_sequenceIdColumn.GetInt64((IDataReader) this.Reader);
    }

    protected class SecuritySubjectEntryColumns : 
      ObjectBinder<SecuritySubjectComponent.SecuritySubjectEntry>
    {
      private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");
      private SqlColumnBinder m_subjectTypeColumn = new SqlColumnBinder("SubjectType");
      private SqlColumnBinder m_descriptionColumn = new SqlColumnBinder("Description");

      protected override SecuritySubjectComponent.SecuritySubjectEntry Bind() => new SecuritySubjectComponent.SecuritySubjectEntry(this.m_idColumn.GetGuid((IDataReader) this.Reader), this.m_subjectTypeColumn.GetInt32((IDataReader) this.Reader), (string) null, this.m_descriptionColumn.GetString((IDataReader) this.Reader, true));
    }
  }
}
