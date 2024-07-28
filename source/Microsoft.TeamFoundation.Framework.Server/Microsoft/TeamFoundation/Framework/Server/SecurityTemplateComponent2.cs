// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityTemplateComponent2
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
  internal class SecurityTemplateComponent2 : SecurityTemplateComponent
  {
    private static readonly SqlMetaData[] typ_SecurityTemplateEntryTable = new SqlMetaData[8]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("HostType", SqlDbType.Int),
      new SqlMetaData("NamespaceId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AclStoreId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TokenTemplate", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SubjectTemplate", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Allow", SqlDbType.Int),
      new SqlMetaData("Deny", SqlDbType.Int)
    };

    public override IReadOnlyList<SecurityTemplateComponent.SecurityTemplateEntry> QuerySecurityTemplateEntries(
      out long sequenceId)
    {
      this.PrepareStoredProcedure("prc_QuerySecurityTemplateEntries");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new SecurityTemplateComponent2.SequenceIdColumns());
        resultCollection.AddBinder<SecurityTemplateComponent.SecurityTemplateEntry>((ObjectBinder<SecurityTemplateComponent.SecurityTemplateEntry>) new SecurityTemplateComponent2.SecurityTemplateEntryColumns());
        sequenceId = resultCollection.GetCurrent<long>().First<long>();
        resultCollection.NextResult();
        return (IReadOnlyList<SecurityTemplateComponent.SecurityTemplateEntry>) resultCollection.GetCurrent<SecurityTemplateComponent.SecurityTemplateEntry>().Items;
      }
    }

    public override long UpdateSecurityTemplateEntries(
      IEnumerable<SecurityTemplateComponent.SecurityTemplateEntry> entries)
    {
      this.PrepareStoredProcedure("prc_UpdateSecurityTemplateEntries");
      this.BindSecurityTemplateEntriesTable("@entries", entries);
      return (long) this.ExecuteScalar();
    }

    public override long DeleteSecurityTemplateEntries(IEnumerable<Guid> ids)
    {
      this.PrepareStoredProcedure("prc_DeleteSecurityTemplateEntries");
      this.BindGuidTable("@ids", ids);
      return (long) this.ExecuteScalar();
    }

    protected SqlParameter BindSecurityTemplateEntriesTable(
      string parameterName,
      IEnumerable<SecurityTemplateComponent.SecurityTemplateEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<SecurityTemplateComponent.SecurityTemplateEntry>();
      System.Func<SecurityTemplateComponent.SecurityTemplateEntry, SqlDataRecord> selector = (System.Func<SecurityTemplateComponent.SecurityTemplateEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityTemplateComponent2.typ_SecurityTemplateEntryTable);
        sqlDataRecord.SetGuid(0, entry.Id);
        sqlDataRecord.SetInt32(1, entry.HostType);
        sqlDataRecord.SetGuid(2, entry.NamespaceId);
        sqlDataRecord.SetGuid(3, entry.AclStoreId);
        sqlDataRecord.SetString(4, entry.TokenTemplate);
        sqlDataRecord.SetString(5, entry.SubjectTemplate);
        sqlDataRecord.SetInt32(6, entry.Allow);
        sqlDataRecord.SetInt32(7, entry.Deny);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_SecurityTemplateEntryTable", rows.Select<SecurityTemplateComponent.SecurityTemplateEntry, SqlDataRecord>(selector));
    }

    protected class SequenceIdColumns : ObjectBinder<long>
    {
      private SqlColumnBinder m_sequenceIdColumn = new SqlColumnBinder("SequenceId");

      protected override long Bind() => this.m_sequenceIdColumn.GetInt64((IDataReader) this.Reader);
    }

    protected class SecurityTemplateEntryColumns : 
      ObjectBinder<SecurityTemplateComponent.SecurityTemplateEntry>
    {
      private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");
      private SqlColumnBinder m_hostTypeColumn = new SqlColumnBinder("HostType");
      private SqlColumnBinder m_namespaceIdColumn = new SqlColumnBinder("NamespaceId");
      private SqlColumnBinder m_aclStoreIdColumn = new SqlColumnBinder("AclStoreId");
      private SqlColumnBinder m_tokenTemplateColumn = new SqlColumnBinder("TokenTemplate");
      private SqlColumnBinder m_subjectTemplateColumn = new SqlColumnBinder("SubjectTemplate");
      private SqlColumnBinder m_allowColumn = new SqlColumnBinder("Allow");
      private SqlColumnBinder m_denyColumn = new SqlColumnBinder("Deny");

      protected override SecurityTemplateComponent.SecurityTemplateEntry Bind() => new SecurityTemplateComponent.SecurityTemplateEntry(this.m_idColumn.GetGuid((IDataReader) this.Reader), this.m_hostTypeColumn.GetInt32((IDataReader) this.Reader), this.m_namespaceIdColumn.GetGuid((IDataReader) this.Reader), this.m_aclStoreIdColumn.GetGuid((IDataReader) this.Reader))
      {
        TokenTemplate = this.m_tokenTemplateColumn.GetString((IDataReader) this.Reader, false),
        SubjectTemplate = this.m_subjectTemplateColumn.GetString((IDataReader) this.Reader, false),
        Allow = this.m_allowColumn.GetInt32((IDataReader) this.Reader),
        Deny = this.m_denyColumn.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
