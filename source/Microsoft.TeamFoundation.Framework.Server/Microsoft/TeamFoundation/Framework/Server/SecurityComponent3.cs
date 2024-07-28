// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent3
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
  internal class SecurityComponent3 : SecurityComponent2
  {
    private static readonly SqlMetaData[] typ_TokenRenameTable = new SqlMetaData[4]
    {
      new SqlMetaData("OldToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NewToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Copy", SqlDbType.Bit),
      new SqlMetaData("Recurse", SqlDbType.Bit)
    };

    protected virtual void BindServiceHostId()
    {
      Guid parameterValue = Guid.Empty;
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        parameterValue = this.RequestContext.ServiceHost.InstanceId;
      this.BindGuid("@serviceHostId", parameterValue);
    }

    protected virtual SqlParameter BindTokenRenameTable(
      string parameterName,
      IEnumerable<TokenRename> rows)
    {
      rows = rows ?? Enumerable.Empty<TokenRename>();
      System.Func<TokenRename, SqlDataRecord> selector = (System.Func<TokenRename, SqlDataRecord>) (rename =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent3.typ_TokenRenameTable);
        sqlDataRecord.SetString(0, rename.OldToken);
        sqlDataRecord.SetString(1, rename.NewToken);
        sqlDataRecord.SetBoolean(2, rename.Copy);
        sqlDataRecord.SetBoolean(3, rename.Recurse);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TokenRenameTable", rows.Select<TokenRename, SqlDataRecord>(selector));
    }

    public override ResultCollection QuerySecurityData(
      Guid namespaceId,
      bool usesInheritInformation,
      int lastSyncId,
      char separator = '\0')
    {
      this.PrepareStoredProcedure("prc_QuerySecurityInfo");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindBoolean("@getInheritInformation", usesInheritInformation);
      this.BindInt("@lastSyncId", lastSyncId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QuerySecurityInfo", this.RequestContext);
      resultCollection.AddBinder<DatabaseAccessControlEntry>((ObjectBinder<DatabaseAccessControlEntry>) this.GetDatabaseAccessControlEntryColumns());
      if (usesInheritInformation)
        resultCollection.AddBinder<string>((ObjectBinder<string>) this.GetNoInheritColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      return resultCollection;
    }

    public override int SetPermissions(
      Guid namespaceId,
      string token,
      IEnumerable<DatabaseAccessControlEntry> permissions,
      bool merge,
      char separator)
    {
      this.PrepareStoredProcedure("prc_SetAccessControlLists2");
      this.BindServiceHostId();
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
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindBoolean("@mergePermissions", mergePermissions);
      this.BindBoolean("@overwriteACL", overwriteAcl);
      this.BindSeparator("@separator", separator);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    public override int SetInheritFlag(
      Guid namespaceId,
      string token,
      bool inheritFlag,
      char separator)
    {
      this.PrepareStoredProcedure("prc_SetInheritFlag");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
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

    public override void RemoveAllAccessControlLists(Guid namespaceId)
    {
      this.PrepareStoredProcedure("prc_RemoveAllAccessControlLists");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public override int RemoveAccessControlLists(
      Guid namespaceId,
      IEnumerable<string> tokens,
      bool recurse,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RemoveAccessControlList");
      this.BindServiceHostId();
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

    public override int RenameToken(
      Guid namespaceId,
      string originalToken,
      string newToken,
      bool copy,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RenameToken");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindString("@originalToken", originalToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@newToken", newToken, -1, false, SqlDbType.NVarChar);
      this.BindBoolean("@copy", copy);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }

    public override int RenameTokens(
      Guid namespaceId,
      IEnumerable<TokenRename> renameTokens,
      char separator)
    {
      this.PrepareStoredProcedure("prc_RenameTokens");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindTokenRenameTable("@renameTable", renameTokens);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
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
      this.BindServiceHostId();
      this.BindGuid("@namespaceId", namespaceId);
      this.BindBoolean("@mergePermissions", mergePermissions);
      this.BindBoolean("@overwriteACL", mergePermissions);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override ResultCollection BulkExportSecurityData(
      Guid namespaceId,
      string securityToken,
      char separator)
    {
      this.PrepareStoredProcedure("prc_QuerySnapshotInstanceSecurityData");
      this.BindServiceHostId();
      this.BindGuid("@namespaceId", namespaceId);
      this.BindString("@securityToken", securityToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QuerySnapshotInstanceSecurityData", this.RequestContext);
      resultCollection.AddBinder<IAccessControlEntry>((ObjectBinder<IAccessControlEntry>) new SecurityComponent.BulkExportACEColumns());
      resultCollection.AddBinder<bool>((ObjectBinder<bool>) new SecurityComponent.BulkExportInheritColumns());
      return resultCollection;
    }
  }
}
