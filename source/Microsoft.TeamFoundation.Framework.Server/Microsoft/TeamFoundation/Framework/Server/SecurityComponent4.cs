// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent4 : SecurityComponent3
  {
    public SecurityComponent4() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override void BindServiceHostId()
    {
    }

    public override void CreateSecurityNamespace(
      SecurityComponent.LocalNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_CreateSecurityNamespace");
      this.BindGuid("@namespaceGuid", description.NamespaceId);
      this.BindString("@name", description.Name, 260, false, SqlDbType.NVarChar);
      this.BindString("@displayName", description.DisplayName, 260, false, SqlDbType.NVarChar);
      this.BindString("@databaseCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindInt("@structure", (int) description.Structure);
      this.BindString("@separatorChar", description.Separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindInt("@elementLength", description.ElementLength);
      this.BindInt("@writePermission", description.WritePermission);
      this.BindInt("@readPermission", description.ReadPermission);
      this.BindString("@extensionType", description.ExtensionType, -1, true, SqlDbType.VarChar);
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
      this.BindString("@databaseCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
      this.BindInt("@structure", (int) description.Structure);
      this.BindString("@separatorChar", description.Separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindInt("@elementLength", description.ElementLength);
      this.BindInt("@writePermission", description.WritePermission);
      this.BindInt("@readPermission", description.ReadPermission);
      this.BindString("@extensionType", description.ExtensionType, -1, true, SqlDbType.VarChar);
      this.BindActionDefinitionTable("@action", description.Actions);
      this.BindGuid("@writerIdentifier", this.Author);
      if (description.SystemBitMask != 0)
        throw new NotSupportedException();
      this.ExecuteNonQuery();
    }

    public override int SetPermissions(
      Guid namespaceId,
      string token,
      IEnumerable<DatabaseAccessControlEntry> permissions,
      bool merge,
      char separator)
    {
      this.PrepareStoredProcedure("prc_SetAccessControlLists");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindPermissionTable("@permissionList", permissions, separator, token);
      this.BindBoolean("@mergePermissions", merge);
      this.BindBoolean("@overwriteACL", false);
      this.BindSeparator("@separator", separator);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    public override int SetAccessControlLists(
      Guid namespaceId,
      IEnumerable<IAccessControlList> accessControlLists,
      IEnumerable<DatabaseAccessControlEntry> accessControlEntries,
      char separator,
      SecurityNamespaceStructure structure,
      bool mergePermissions,
      bool overwriteAcl)
    {
      this.PrepareStoredProcedure("prc_SetAccessControlLists");
      if (structure == SecurityNamespaceStructure.Hierarchical)
        this.BindAccessControlListTable("@inheritList", accessControlLists, separator);
      this.BindPermissionTable("@permissionList", accessControlEntries, separator, string.Empty);
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindBoolean("@mergePermissions", mergePermissions);
      this.BindBoolean("@overwriteACL", overwriteAcl);
      this.BindSeparator("@separator", separator);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }
  }
}
