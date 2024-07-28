// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsPermissionACLColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsPermissionACLColumns : ObjectBinder<AnalyticsPermissionAcl>
  {
    private SqlColumnBinder NamespaceGuidColumn = new SqlColumnBinder("NamespaceGuid");
    private SqlColumnBinder ServiceHostIdColumn = new SqlColumnBinder("ServiceHostId");
    private SqlColumnBinder SecurityTokenColumn = new SqlColumnBinder("SecurityToken");
    private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");
    private SqlColumnBinder AllowPermissionColumn = new SqlColumnBinder("AllowPermission");
    private SqlColumnBinder DenyPermissionColumn = new SqlColumnBinder("DenyPermission");

    protected override AnalyticsPermissionAcl Bind() => new AnalyticsPermissionAcl()
    {
      NamespaceGuid = this.NamespaceGuidColumn.GetGuid((IDataReader) this.Reader),
      ServiceHostId = this.ServiceHostIdColumn.GetGuid((IDataReader) this.Reader),
      SecurityToken = this.SecurityTokenColumn.GetString((IDataReader) this.Reader, false),
      TeamFoundationId = this.TeamFoundationIdColumn.GetGuid((IDataReader) this.Reader),
      AllowPermission = this.AllowPermissionColumn.GetInt32((IDataReader) this.Reader),
      DenyPermission = this.DenyPermissionColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
