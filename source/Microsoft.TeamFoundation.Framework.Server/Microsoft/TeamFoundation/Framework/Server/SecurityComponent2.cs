// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent2 : SecurityComponent
  {
    private static readonly SqlMetaData[] typ_ActionDefinitionTable = new SqlMetaData[3]
    {
      new SqlMetaData("Bit", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 260L),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 260L)
    };
    private static readonly SqlMetaData[] typ_PermissionTable = new SqlMetaData[5]
    {
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SecurityToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexableToken", SqlDbType.NVarChar, 350L),
      new SqlMetaData("AllowPermission", SqlDbType.Int),
      new SqlMetaData("DenyPermission", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TokenTable = new SqlMetaData[3]
    {
      new SqlMetaData("Token", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Recurse", SqlDbType.Bit),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_AccessControlListTable = new SqlMetaData[3]
    {
      new SqlMetaData("Token", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexableToken", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Inherit", SqlDbType.Bit)
    };

    public override void CreateSecurityNamespace(
      SecurityComponent.LocalNamespaceDescription description)
    {
      this.PrepareStoredProcedure("prc_CreateSecurityNamespace2");
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
      this.PrepareStoredProcedure("prc_UpdateSecurityNamespace2");
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

    protected SqlParameter BindActionDefinitionTable(
      string parameterName,
      IEnumerable<SecurityComponent.NamespaceAction> rows)
    {
      rows = rows ?? Enumerable.Empty<SecurityComponent.NamespaceAction>();
      return this.BindTable(parameterName, "typ_ActionDefinitionTable", this.BindActionDefinitionRows(rows));
    }

    private IEnumerable<SqlDataRecord> BindActionDefinitionRows(
      IEnumerable<SecurityComponent.NamespaceAction> rows)
    {
      foreach (SecurityComponent.NamespaceAction row in rows)
      {
        SqlDataRecord record = new SqlDataRecord(SecurityComponent2.typ_ActionDefinitionTable);
        record.SetInt32(0, row.Bit);
        record.SetString(1, row.Name);
        record.SetNullableString(2, row.DisplayName);
        yield return record;
      }
    }

    protected virtual SqlParameter BindPermissionTable(
      string parameterName,
      IEnumerable<DatabaseAccessControlEntry> rows,
      char separator,
      string token)
    {
      rows = rows ?? Enumerable.Empty<DatabaseAccessControlEntry>();
      string overrideSecurityToken = (string) null;
      string overrideIndexableToken = (string) null;
      if (!string.IsNullOrEmpty(token))
      {
        overrideSecurityToken = PermissionTable.AddSeparator(separator, token);
        overrideIndexableToken = PermissionTable.GetIndexableTokenFromToken(overrideSecurityToken, separator);
      }
      System.Func<DatabaseAccessControlEntry, SqlDataRecord> selector = (System.Func<DatabaseAccessControlEntry, SqlDataRecord>) (ace =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent2.typ_PermissionTable);
        sqlDataRecord.SetGuid(0, ace.SubjectId);
        if (overrideSecurityToken != null)
        {
          sqlDataRecord.SetString(1, overrideSecurityToken);
          sqlDataRecord.SetString(2, overrideIndexableToken);
        }
        else
        {
          string token1 = PermissionTable.AddSeparator(separator, ace.Token);
          sqlDataRecord.SetString(1, token1);
          sqlDataRecord.SetString(2, PermissionTable.GetIndexableTokenFromToken(token1, separator));
        }
        sqlDataRecord.SetInt32(3, ace.Allow);
        sqlDataRecord.SetInt32(4, ace.Deny);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_PermissionTable", rows.Select<DatabaseAccessControlEntry, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindTokenTable(
      string parameterName,
      IEnumerable<string> rows,
      char separator,
      bool recurse)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (token =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent2.typ_TokenTable);
        sqlDataRecord.SetString(0, PermissionTable.AddSeparator(separator, token));
        sqlDataRecord.SetBoolean(1, recurse);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TokenTable", rows.Select<string, SqlDataRecord>(selector));
    }

    public override int SetPermissions(
      Guid namespaceId,
      string token,
      IEnumerable<DatabaseAccessControlEntry> permissions,
      bool merge,
      char separator)
    {
      this.PrepareStoredProcedure("prc_SetAccessControlLists2");
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
      this.PrepareStoredProcedure("prc_SetAccessControlLists2");
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

    public override int RemovePermissions(
      Guid namespaceId,
      string token,
      IEnumerable<Guid> teamFoundationIds,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RemoveAccessControlEntry");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindGuidTable("@identityList", teamFoundationIds);
      this.BindString("@token", token, 4000, true, SqlDbType.NVarChar);
      this.BindString("@separator", separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveAccessControlEntry", this.RequestContext);
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SecurityComponent.SecurityIdentityColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      resultCollection.NextResult();
      return resultCollection.GetCurrent<int>().First<int>();
    }

    public override int RemoveAccessControlLists(
      Guid namespaceId,
      IEnumerable<string> tokens,
      bool recurse,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RemoveAccessControlList");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindTokenTable("@deleteToken", tokens, separator, recurse);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, true, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveAccessControlList", this.RequestContext);
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SecurityComponent.SecurityIdentityColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      resultCollection.NextResult();
      return resultCollection.GetCurrent<int>().First<int>();
    }

    protected virtual SqlParameter BindAccessControlListTable(
      string parameterName,
      IEnumerable<IAccessControlList> rows,
      char separator)
    {
      rows = rows ?? Enumerable.Empty<IAccessControlList>();
      return this.BindTable(parameterName, "typ_AccessControlListTable", this.BindAccessControlListRow(rows, separator));
    }

    private IEnumerable<SqlDataRecord> BindAccessControlListRow(
      IEnumerable<IAccessControlList> rows,
      char separator)
    {
      foreach (IAccessControlList row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent2.typ_AccessControlListTable);
        string token = PermissionTable.AddSeparator(separator, row.Token);
        sqlDataRecord.SetString(0, token);
        sqlDataRecord.SetString(1, PermissionTable.GetIndexableTokenFromToken(token, separator));
        sqlDataRecord.SetBoolean(2, row.InheritPermissions);
        yield return sqlDataRecord;
      }
    }

    public override void BulkImportSecurityData(
      Guid namespaceId,
      IEnumerable<IAccessControlList> accessControlLists,
      bool overwriteAcls,
      bool mergePermissions,
      char separator)
    {
      this.PrepareStoredProcedure("prc_SetSnapshotInstanceSecurityData");
      this.BindAccessControlListTable("@inheritPermissions", accessControlLists, separator);
      this.BindAccessControlEntryTable("@permissions", accessControlLists, separator);
      this.BindGuid("@namespaceId", namespaceId);
      this.BindBoolean("@mergePermissions", mergePermissions);
      this.BindBoolean("@overwriteACL", mergePermissions);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
