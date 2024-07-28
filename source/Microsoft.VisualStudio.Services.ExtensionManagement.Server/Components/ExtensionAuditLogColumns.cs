// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Components.ExtensionAuditLogColumns
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Components
{
  internal class ExtensionAuditLogColumns : ObjectBinder<ExtensionAuditLogEntry>
  {
    private SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
    private SqlColumnBinder extensionNameColumn = new SqlColumnBinder("ExtensionName");
    private SqlColumnBinder auditActionColumn = new SqlColumnBinder("AuditAction");
    private SqlColumnBinder updatedByColumn = new SqlColumnBinder("UpdatedBy");
    private SqlColumnBinder auditDateColumn = new SqlColumnBinder("AuditDate");
    private SqlColumnBinder commentMessageColumn = new SqlColumnBinder("Comment");
    private IVssRequestContext m_requestContext;
    private const string s_area = "ExtensionAuditLogComponent";
    private const string s_layer = "ObjectBinder";

    public ExtensionAuditLogColumns(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(70325, "ExtensionAuditLogComponent", "ObjectBinder", "ExtensionAuditLogColumns.ctor");
      this.m_requestContext = requestContext;
      this.m_requestContext.TraceLeave(70330, "ExtensionAuditLogComponent", "ObjectBinder", "ExtensionAuditLogColumns.ctor");
    }

    protected override ExtensionAuditLogEntry Bind()
    {
      this.m_requestContext.TraceEnter(70335, "ExtensionAuditLogComponent", "ObjectBinder", nameof (Bind));
      ExtensionAuditLogEntry extensionAuditLogEntry = new ExtensionAuditLogEntry();
      extensionAuditLogEntry.PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false);
      extensionAuditLogEntry.ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, false);
      extensionAuditLogEntry.UpdatedBy = new IdentityRef()
      {
        Id = this.updatedByColumn.GetGuid((IDataReader) this.Reader, true).ToString()
      };
      extensionAuditLogEntry.AuditAction = this.auditActionColumn.GetString((IDataReader) this.Reader, false).Trim();
      extensionAuditLogEntry.AuditDate = this.auditDateColumn.GetDateTime((IDataReader) this.Reader);
      extensionAuditLogEntry.Comment = this.commentMessageColumn.GetString((IDataReader) this.Reader, true);
      this.m_requestContext.TraceLeave(70340, "ExtensionAuditLogComponent", "ObjectBinder", nameof (Bind));
      return extensionAuditLogEntry;
    }
  }
}
