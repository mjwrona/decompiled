// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent11 : SecurityComponent10
  {
    private static readonly SqlMetaData[] typ_AccessControlListTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Token", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexableToken", SqlDbType.NVarChar, 350L),
      new SqlMetaData("Inherit", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_PermissionTable2 = new SqlMetaData[6]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SecurityToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexableToken", SqlDbType.NVarChar, 350L),
      new SqlMetaData("AllowPermission", SqlDbType.Int),
      new SqlMetaData("DenyPermission", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_TokenTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Token", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Recurse", SqlDbType.Bit),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_TokenRenameTable2 = new SqlMetaData[5]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("OldToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NewToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Copy", SqlDbType.Bit),
      new SqlMetaData("Recurse", SqlDbType.Bit)
    };

    protected override SqlParameter BindAccessControlListTable(
      string parameterName,
      IEnumerable<IAccessControlList> rows,
      char separator)
    {
      rows = rows ?? Enumerable.Empty<IAccessControlList>();
      return this.BindTable(parameterName, "typ_AccessControlListTable2", this.BindAccessControlListRow(rows, separator));
    }

    private IEnumerable<SqlDataRecord> BindAccessControlListRow(
      IEnumerable<IAccessControlList> rows,
      char separator)
    {
      SecurityComponent11 securityComponent11 = this;
      foreach (IAccessControlList row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent11.typ_AccessControlListTable2);
        int dataspaceIdForToken = securityComponent11.GetDataspaceIdForToken(row.Token);
        string token = PermissionTable.AddSeparator(separator, row.Token);
        sqlDataRecord.SetInt32(0, dataspaceIdForToken);
        sqlDataRecord.SetString(1, token);
        sqlDataRecord.SetString(2, PermissionTable.GetIndexableTokenFromToken(token, separator));
        sqlDataRecord.SetBoolean(3, row.InheritPermissions);
        yield return sqlDataRecord;
      }
    }

    protected override SqlParameter BindPermissionTable(
      string parameterName,
      IEnumerable<DatabaseAccessControlEntry> rows,
      char separator,
      string token)
    {
      rows = rows ?? Enumerable.Empty<DatabaseAccessControlEntry>();
      string overrideSecurityToken = (string) null;
      string overrideIndexableToken = (string) null;
      int overrideDataspaceId = 0;
      if (!string.IsNullOrEmpty(token))
      {
        overrideDataspaceId = this.GetDataspaceIdForToken(token);
        overrideSecurityToken = PermissionTable.AddSeparator(separator, token);
        overrideIndexableToken = PermissionTable.GetIndexableTokenFromToken(overrideSecurityToken, separator);
      }
      System.Func<DatabaseAccessControlEntry, SqlDataRecord> selector = (System.Func<DatabaseAccessControlEntry, SqlDataRecord>) (ace =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent11.typ_PermissionTable2);
        string token1 = ace.Token;
        if (overrideSecurityToken != null)
          sqlDataRecord.SetInt32(0, overrideDataspaceId);
        else
          sqlDataRecord.SetInt32(0, this.GetDataspaceIdForToken(token1));
        sqlDataRecord.SetGuid(1, ace.SubjectId);
        if (overrideSecurityToken != null)
        {
          sqlDataRecord.SetString(2, overrideSecurityToken);
          sqlDataRecord.SetString(3, overrideIndexableToken);
        }
        else
        {
          string token2 = PermissionTable.AddSeparator(separator, token1);
          sqlDataRecord.SetString(2, token2);
          sqlDataRecord.SetString(3, PermissionTable.GetIndexableTokenFromToken(token2, separator));
        }
        sqlDataRecord.SetInt32(4, ace.Allow);
        sqlDataRecord.SetInt32(5, ace.Deny);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_PermissionTable2", rows.Select<DatabaseAccessControlEntry, SqlDataRecord>(selector));
    }

    protected override SqlParameter BindTokenTable(
      string parameterName,
      IEnumerable<string> rows,
      char separator,
      bool recurse)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (token =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent11.typ_TokenTable2);
        sqlDataRecord.SetInt32(0, this.GetDataspaceIdForToken(token));
        sqlDataRecord.SetString(1, PermissionTable.AddSeparator(separator, token));
        sqlDataRecord.SetBoolean(2, recurse);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TokenTable2", rows.Select<string, SqlDataRecord>(selector));
    }

    protected override SqlParameter BindTokenRenameTable(
      string parameterName,
      IEnumerable<TokenRename> rows)
    {
      rows = rows ?? Enumerable.Empty<TokenRename>();
      System.Func<TokenRename, SqlDataRecord> selector = (System.Func<TokenRename, SqlDataRecord>) (rename =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent11.typ_TokenRenameTable2);
        int dataspaceIdForToken1 = this.GetDataspaceIdForToken(rename.OldToken);
        int dataspaceIdForToken2 = this.GetDataspaceIdForToken(rename.NewToken);
        if (dataspaceIdForToken1 != dataspaceIdForToken2)
          throw new InvalidOperationException();
        sqlDataRecord.SetInt32(0, dataspaceIdForToken1);
        sqlDataRecord.SetString(1, rename.OldToken);
        sqlDataRecord.SetString(2, rename.NewToken);
        sqlDataRecord.SetBoolean(3, rename.Copy);
        sqlDataRecord.SetBoolean(4, rename.Recurse);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TokenRenameTable2", rows.Select<TokenRename, SqlDataRecord>(selector));
    }

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
      this.BindBoolean("@useTokenTranslator", description.UseTokenTranslator);
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
      this.BindBoolean("@useTokenTranslator", description.UseTokenTranslator);
      this.BindActionDefinitionTable("@action", description.Actions);
      this.BindGuid("@writerIdentifier", this.Author);
      if (description.SystemBitMask != 0)
        throw new NotSupportedException();
      this.ExecuteNonQuery();
    }

    protected override SecurityComponent.NamespaceDescriptionColumns GetNamespaceDescriptionColumns() => (SecurityComponent.NamespaceDescriptionColumns) new SecurityComponent11.NamespaceDescriptionColumns5();

    public override int SetInheritFlag(
      Guid namespaceId,
      string token,
      bool inheritFlag,
      char separator)
    {
      this.PrepareStoredProcedure("prc_SetInheritFlag");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindInt("@dataspaceId", this.GetDataspaceIdForToken(token));
      this.BindString("@token", token, 4000, false, SqlDbType.NVarChar);
      this.BindBoolean("@inheritFlag", inheritFlag);
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
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindInt("@dataspaceId", this.GetDataspaceIdForToken(token));
      this.BindGuidTable("@identityList", teamFoundationIds);
      this.BindString("@token", token, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("@separator", separator.ToString(), 1, true, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveAccessControlEntry", this.RequestContext);
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SecurityComponent.SecurityIdentityColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      resultCollection.NextResult();
      return resultCollection.GetCurrent<int>().First<int>();
    }

    public override int RenameToken(
      Guid namespaceId,
      string originalToken,
      string newToken,
      bool copy,
      char separator)
    {
      int dataspaceIdForToken1 = this.GetDataspaceIdForToken(originalToken);
      int dataspaceIdForToken2 = this.GetDataspaceIdForToken(newToken);
      if (dataspaceIdForToken1 != dataspaceIdForToken2)
        throw new InvalidOperationException();
      this.PrepareStoredProcedure("prc_RenameToken");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindInt("@dataspaceId", dataspaceIdForToken1);
      this.BindString("@originalToken", originalToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@newToken", newToken, -1, false, SqlDbType.NVarChar);
      this.BindBoolean("@copy", copy);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    protected override SecurityComponent.DatabaseAccessControlEntryColumns GetDatabaseAccessControlEntryColumns() => (SecurityComponent.DatabaseAccessControlEntryColumns) new SecurityComponent11.DatabaseAccessControlEntryColumns2();

    protected override SecurityComponent.NoInheritColumns GetNoInheritColumns() => (SecurityComponent.NoInheritColumns) new SecurityComponent11.NoInheritColumns2();

    protected class NamespaceDescriptionColumns5 : SecurityComponent10.NamespaceDescriptionColumns4
    {
      private SqlColumnBinder useTokenTranslatorColumn = new SqlColumnBinder("UseTokenTranslator");

      protected override bool GetUseTokenTranslator() => this.useTokenTranslatorColumn.GetBoolean((IDataReader) this.Reader);
    }

    protected class DatabaseAccessControlEntryColumns2 : 
      SecurityComponent.DatabaseAccessControlEntryColumns
    {
      private SqlColumnBinder dataspaceIdColumn = new SqlColumnBinder("DataspaceId");

      protected override DatabaseAccessControlEntry Bind() => base.Bind();
    }

    protected class NoInheritColumns2 : SecurityComponent.NoInheritColumns
    {
      private SqlColumnBinder dataspaceIdColumn = new SqlColumnBinder("DataspaceId");

      protected override string Bind() => base.Bind();
    }
  }
}
