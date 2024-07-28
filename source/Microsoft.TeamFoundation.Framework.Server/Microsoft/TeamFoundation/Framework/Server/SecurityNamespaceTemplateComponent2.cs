// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceTemplateComponent2
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
  public class SecurityNamespaceTemplateComponent2 : SecurityNamespaceTemplateComponent
  {
    private static readonly SqlMetaData[] typ_SecurityNamespaceTemplate = new SqlMetaData[3]
    {
      new SqlMetaData("HostType", SqlDbType.Int),
      new SqlMetaData("NamespaceId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L)
    };

    public override IReadOnlyList<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> QuerySecurityNamespaceTemplates(
      out long sequenceId)
    {
      this.PrepareStoredProcedure("prc_QuerySecurityNamespaceTemplates");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new SecurityNamespaceTemplateComponent2.SequenceIdColumns());
        resultCollection.AddBinder<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>((ObjectBinder<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>) new SecurityNamespaceTemplateComponent2.SecurityNamespaceTemplateColumns());
        sequenceId = resultCollection.GetCurrent<long>().First<long>();
        resultCollection.NextResult();
        return (IReadOnlyList<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>) resultCollection.GetCurrent<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>().Items;
      }
    }

    public override long UpdateSecurityNamespaceTemplates(
      IEnumerable<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> templates)
    {
      this.PrepareStoredProcedure("prc_UpdateSecurityNamespaceTemplates");
      this.BindSecurityNamespaceTemplates("@templates", templates);
      return (long) this.ExecuteScalar();
    }

    public override long DeleteSecurityNamespaceTemplates(
      IEnumerable<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> templates)
    {
      this.PrepareStoredProcedure("prc_DeleteSecurityNamespaceTemplates");
      this.BindSecurityNamespaceTemplates("@templates", templates, true);
      return (long) this.ExecuteScalar();
    }

    private SqlParameter BindSecurityNamespaceTemplates(
      string parameterName,
      IEnumerable<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> rows,
      bool isDelete = false)
    {
      rows = rows ?? Enumerable.Empty<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>();
      System.Func<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate, SqlDataRecord> selector = (System.Func<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate, SqlDataRecord>) (template =>
      {
        SqlDataRecord record = new SqlDataRecord(SecurityNamespaceTemplateComponent2.typ_SecurityNamespaceTemplate);
        record.SetInt32(0, template.HostType);
        record.SetGuid(1, template.NamespaceId);
        if (isDelete)
          record.SetNullableString(2, template.Description);
        else
          record.SetString(2, template.Description);
        return record;
      });
      return this.BindTable(parameterName, "typ_SecurityNamespaceTemplate", rows.Select<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate, SqlDataRecord>(selector));
    }

    private class SequenceIdColumns : ObjectBinder<long>
    {
      private SqlColumnBinder m_sequenceIdColumn = new SqlColumnBinder("SequenceId");

      protected override long Bind() => this.m_sequenceIdColumn.GetInt64((IDataReader) this.Reader);
    }

    private class SecurityNamespaceTemplateColumns : 
      ObjectBinder<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>
    {
      private SqlColumnBinder m_hostTypeColumn = new SqlColumnBinder("HostType");
      private SqlColumnBinder m_namespaceIdColumn = new SqlColumnBinder("NamespaceId");
      private SqlColumnBinder m_descriptionColumn = new SqlColumnBinder("Description");

      protected override SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate Bind() => new SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate(this.m_hostTypeColumn.GetInt32((IDataReader) this.Reader), this.m_namespaceIdColumn.GetGuid((IDataReader) this.Reader), this.m_descriptionColumn.GetString((IDataReader) this.Reader, false));
    }
  }
}
