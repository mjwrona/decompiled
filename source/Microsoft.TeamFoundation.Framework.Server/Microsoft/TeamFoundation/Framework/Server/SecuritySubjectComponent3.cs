// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuritySubjectComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuritySubjectComponent3 : SecuritySubjectComponent2
  {
    private static readonly SqlMetaData[] typ_SecuritySubjectEntryTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SubjectType", SqlDbType.Int),
      new SqlMetaData("Identifier", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 1048L)
    };

    public override IReadOnlyList<SecuritySubjectComponent.SecuritySubjectEntry> QuerySecuritySubjectEntries(
      out long sequenceId)
    {
      this.PrepareStoredProcedure("prc_QuerySecuritySubjectEntries");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new SecuritySubjectComponent2.SequenceIdColumns());
        resultCollection.AddBinder<SecuritySubjectComponent.SecuritySubjectEntry>((ObjectBinder<SecuritySubjectComponent.SecuritySubjectEntry>) new SecuritySubjectComponent3.SecuritySubjectEntryColumns2());
        sequenceId = resultCollection.GetCurrent<long>().First<long>();
        resultCollection.NextResult();
        return (IReadOnlyList<SecuritySubjectComponent.SecuritySubjectEntry>) resultCollection.GetCurrent<SecuritySubjectComponent.SecuritySubjectEntry>().Items;
      }
    }

    public override long UpdateSecuritySubjectEntries(
      IEnumerable<SecuritySubjectComponent.SecuritySubjectEntry> entries)
    {
      this.PrepareStoredProcedure("prc_UpdateSecuritySubjectEntries");
      this.BindSecuritySubjectEntriesTable2("@entries", entries);
      return (long) this.ExecuteScalar();
    }

    protected SqlParameter BindSecuritySubjectEntriesTable2(
      string parameterName,
      IEnumerable<SecuritySubjectComponent.SecuritySubjectEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<SecuritySubjectComponent.SecuritySubjectEntry>();
      System.Func<SecuritySubjectComponent.SecuritySubjectEntry, SqlDataRecord> selector = (System.Func<SecuritySubjectComponent.SecuritySubjectEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecuritySubjectComponent3.typ_SecuritySubjectEntryTable2);
        sqlDataRecord.SetGuid(0, entry.Id);
        sqlDataRecord.SetInt32(1, entry.SubjectType);
        if (!string.IsNullOrEmpty(entry.Identifier))
          sqlDataRecord.SetString(2, entry.Identifier);
        if (!string.IsNullOrEmpty(entry.Description))
          sqlDataRecord.SetString(3, entry.Description);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_SecuritySubjectEntryTable2", rows.Select<SecuritySubjectComponent.SecuritySubjectEntry, SqlDataRecord>(selector));
    }

    protected class SecuritySubjectEntryColumns2 : 
      SecuritySubjectComponent2.SecuritySubjectEntryColumns
    {
      private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");
      private SqlColumnBinder m_subjectTypeColumn = new SqlColumnBinder("SubjectType");
      private SqlColumnBinder m_identifierColumn = new SqlColumnBinder("Identifier");
      private SqlColumnBinder m_descriptionColumn = new SqlColumnBinder("Description");

      protected override SecuritySubjectComponent.SecuritySubjectEntry Bind() => new SecuritySubjectComponent.SecuritySubjectEntry(this.m_idColumn.GetGuid((IDataReader) this.Reader), this.m_subjectTypeColumn.GetInt32((IDataReader) this.Reader), this.m_identifierColumn.GetString((IDataReader) this.Reader, true), this.m_descriptionColumn.GetString((IDataReader) this.Reader, true));
    }
  }
}
