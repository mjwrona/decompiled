// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Components.ScopeTemplateComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Components
{
  internal class ScopeTemplateComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ScopeTemplateComponent>(1)
    }, "ScopeTemplate");
    private static readonly SqlMetaData[] typ_ScopeTemplateEntryTable = new SqlMetaData[2]
    {
      new SqlMetaData("Identifier", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Template", SqlDbType.NVarChar, -1L)
    };

    public virtual IReadOnlyList<ScopeTemplateEntry> QueryScopeTemplateEntries()
    {
      this.PrepareStoredProcedure("prc_QueryScopeTemplateEntries");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ScopeTemplateEntry>((ObjectBinder<ScopeTemplateEntry>) new ScopeTemplateComponent.ScopeTemplateEntryColumns());
        return (IReadOnlyList<ScopeTemplateEntry>) resultCollection.GetCurrent<ScopeTemplateEntry>().Items;
      }
    }

    public virtual void UpdateScopeTemplateEntires(IEnumerable<ScopeTemplateEntry> entries)
    {
      this.PrepareStoredProcedure("prc_UpdateScopeTemplateEntries");
      this.BindScopeTemplateEntriesTable("@entries", entries);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteSecurityTemplateEntries(IEnumerable<string> identifiers)
    {
      this.PrepareStoredProcedure("prc_DeleteScopeTemplateEntries");
      this.BindStringTable("@identifiers", identifiers);
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindScopeTemplateEntriesTable(
      string parameterName,
      IEnumerable<ScopeTemplateEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<ScopeTemplateEntry>();
      System.Func<ScopeTemplateEntry, SqlDataRecord> selector = (System.Func<ScopeTemplateEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ScopeTemplateComponent.typ_ScopeTemplateEntryTable);
        sqlDataRecord.SetString(0, entry.Identifier);
        sqlDataRecord.SetString(1, entry.Templates.Serialize<ACETemplate[]>());
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ScopeTemplateEntryTable", rows.Select<ScopeTemplateEntry, SqlDataRecord>(selector));
    }

    protected class ScopeTemplateEntryColumns : ObjectBinder<ScopeTemplateEntry>
    {
      private SqlColumnBinder m_identifierColumn = new SqlColumnBinder("Identifier");
      private SqlColumnBinder m_templateColumn = new SqlColumnBinder("Templates");

      protected override ScopeTemplateEntry Bind() => new ScopeTemplateEntry(this.m_identifierColumn.GetString((IDataReader) this.Reader, false), this.m_templateColumn.GetString((IDataReader) this.Reader, false));
    }
  }
}
