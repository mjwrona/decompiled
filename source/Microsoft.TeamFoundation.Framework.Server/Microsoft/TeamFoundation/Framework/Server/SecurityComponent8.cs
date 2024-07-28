// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent8 : SecurityComponent6
  {
    public override void CreateRemoteSecurityNamespace(
      SecurityComponent.RemoteNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_CreateRemoteSecurityNamespace");
      this.BindGuid("@namespaceGuid", description.NamespaceId);
      this.BindString("@databaseCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindGuid("@serviceOwner", description.ServiceOwner);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    protected override SecurityComponent.NamespaceDescriptionColumns GetNamespaceDescriptionColumns() => (SecurityComponent.NamespaceDescriptionColumns) new SecurityComponent8.NamespaceDescriptionColumns2();

    protected class NamespaceDescriptionColumns2 : SecurityComponent.NamespaceDescriptionColumns
    {
      private SqlColumnBinder namespaceGuidColumn = new SqlColumnBinder("NamespaceGuid");
      private SqlColumnBinder databaseCategoryColumn = new SqlColumnBinder("DatabaseCategory");
      private SqlColumnBinder serviceOwnerColumn = new SqlColumnBinder("ServiceOwner");

      protected override SecurityComponent.NamespaceDescription Bind()
      {
        Guid guid = this.serviceOwnerColumn.GetGuid((IDataReader) this.Reader, true);
        return Guid.Empty == guid ? base.Bind() : (SecurityComponent.NamespaceDescription) new SecurityComponent.RemoteNamespaceDescription(this.namespaceGuidColumn.GetGuid((IDataReader) this.Reader, false), this.GetDataspaceCategory(this.databaseCategoryColumn.GetString((IDataReader) this.Reader, false)), guid);
      }
    }
  }
}
