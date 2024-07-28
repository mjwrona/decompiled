// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent10
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent10 : SecurityComponent9
  {
    public override void CreateSecurityNamespace(
      SecurityComponent.LocalNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_CreateSecurityNamespace");
      this.BindGuid("@namespaceGuid", description.NamespaceId);
      this.BindString("@name", description.Name, 260, false, SqlDbType.NVarChar);
      this.BindString("@displayName", description.DisplayName, 260, false, SqlDbType.NVarChar);
      this.BindString("@dataspaceCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindInt("@structure", (int) description.Structure);
      this.BindString("@separatorChar", description.Separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindInt("@elementLength", description.ElementLength);
      this.BindInt("@writePermission", description.WritePermission);
      this.BindInt("@readPermission", description.ReadPermission);
      this.BindString("@extensionType", description.ExtensionType, -1, true, SqlDbType.VarChar);
      this.BindBoolean("@isRemotable", description.IsRemotable);
      this.BindActionDefinitionTable("@action", description.Actions);
      this.BindGuid("@writerIdentifier", this.Author);
      if (description.SystemBitMask != 0)
        throw new NotSupportedException();
      this.ExecuteNonQuery();
    }

    public override void UpdateSecurityNamespace(
      SecurityComponent.LocalNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_UpdateSecurityNamespace");
      this.BindGuid("@namespaceGuid", description.NamespaceId);
      this.BindString("@name", description.Name, 260, false, SqlDbType.NVarChar);
      this.BindString("@displayName", description.DisplayName, 260, false, SqlDbType.NVarChar);
      this.BindString("@dataspaceCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindInt("@structure", (int) description.Structure);
      this.BindString("@separatorChar", description.Separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindInt("@elementLength", description.ElementLength);
      this.BindInt("@writePermission", description.WritePermission);
      this.BindInt("@readPermission", description.ReadPermission);
      this.BindString("@extensionType", description.ExtensionType, -1, true, SqlDbType.VarChar);
      this.BindBoolean("@isRemotable", description.IsRemotable);
      this.BindActionDefinitionTable("@action", description.Actions);
      this.BindGuid("@writerIdentifier", this.Author);
      if (description.SystemBitMask != 0)
        throw new NotSupportedException();
      this.ExecuteNonQuery();
    }

    public override void CreateRemoteSecurityNamespace(
      SecurityComponent.RemoteNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_CreateRemoteSecurityNamespace");
      this.BindGuid("@namespaceGuid", description.NamespaceId);
      this.BindString("@dataspaceCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindGuid("@serviceOwner", description.ServiceOwner);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    protected override SecurityComponent.NamespaceDescriptionColumns GetNamespaceDescriptionColumns() => (SecurityComponent.NamespaceDescriptionColumns) new SecurityComponent10.NamespaceDescriptionColumns4();

    protected class NamespaceDescriptionColumns4 : SecurityComponent9.NamespaceDescriptionColumns3
    {
      private SqlColumnBinder namespaceGuidColumn = new SqlColumnBinder("NamespaceGuid");
      private SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
      private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder dataspaceCategoryColumn = new SqlColumnBinder("DataspaceCategory");
      private SqlColumnBinder structureColumn = new SqlColumnBinder("Structure");
      private SqlColumnBinder separatorColumn = new SqlColumnBinder("Separator");
      private SqlColumnBinder elementLengthColumn = new SqlColumnBinder("ElementLength");
      private SqlColumnBinder writePermissionColumn = new SqlColumnBinder("WritePermission");
      private SqlColumnBinder readPermissionColumn = new SqlColumnBinder("ReadPermission");
      private SqlColumnBinder extensionTypeColumn = new SqlColumnBinder("ExtensionType");
      private SqlColumnBinder serviceOwnerColumn = new SqlColumnBinder("ServiceOwner");
      private SqlColumnBinder isRemotableColumn = new SqlColumnBinder("IsRemotable");

      protected override SecurityComponent.NamespaceDescription Bind()
      {
        Guid guid = this.serviceOwnerColumn.GetGuid((IDataReader) this.Reader, true);
        return Guid.Empty == guid ? (SecurityComponent.NamespaceDescription) new SecurityComponent.LocalNamespaceDescription(this.namespaceGuidColumn.GetGuid((IDataReader) this.Reader, false), this.nameColumn.GetString((IDataReader) this.Reader, false), this.displayNameColumn.GetString((IDataReader) this.Reader, true), this.GetDataspaceCategory(this.dataspaceCategoryColumn.GetString((IDataReader) this.Reader, false)), this.separatorColumn.GetString((IDataReader) this.Reader, false)[0], this.elementLengthColumn.GetInt32((IDataReader) this.Reader), (SecurityNamespaceStructure) this.structureColumn.GetInt32((IDataReader) this.Reader), this.writePermissionColumn.GetInt32((IDataReader) this.Reader), this.readPermissionColumn.GetInt32((IDataReader) this.Reader), Enumerable.Empty<SecurityComponent.NamespaceAction>(), this.extensionTypeColumn.GetString((IDataReader) this.Reader, true), this.isRemotableColumn.GetBoolean((IDataReader) this.Reader, false), this.GetUseTokenTranslator(), 0) : (SecurityComponent.NamespaceDescription) new SecurityComponent.RemoteNamespaceDescription(this.namespaceGuidColumn.GetGuid((IDataReader) this.Reader, false), this.GetDataspaceCategory(this.dataspaceCategoryColumn.GetString((IDataReader) this.Reader, false)), guid);
      }
    }
  }
}
