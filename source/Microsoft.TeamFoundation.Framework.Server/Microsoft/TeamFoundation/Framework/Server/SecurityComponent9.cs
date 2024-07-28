// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent9 : SecurityComponent8
  {
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
      this.BindString("@databaseCategory", this.GetDataspaceCategory(description.DataspaceCategory), 260, false, SqlDbType.NVarChar);
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

    protected override SecurityComponent.NamespaceDescriptionColumns GetNamespaceDescriptionColumns() => (SecurityComponent.NamespaceDescriptionColumns) new SecurityComponent9.NamespaceDescriptionColumns3();

    protected class NamespaceDescriptionColumns3 : SecurityComponent8.NamespaceDescriptionColumns2
    {
      private SqlColumnBinder isRemotableColumn = new SqlColumnBinder("IsRemotable");

      protected override bool GetIsRemotable() => this.isRemotableColumn.GetBoolean((IDataReader) this.Reader, false);
    }
  }
}
