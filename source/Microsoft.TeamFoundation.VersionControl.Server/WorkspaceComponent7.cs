// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceComponent7
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkspaceComponent7 : WorkspaceComponent6
  {
    private readonly SqlMetaData[] typ_Mapping3 = new SqlMetaData[6]
    {
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ProjectName", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MappingType", SqlDbType.Bit),
      new SqlMetaData("Depth", SqlDbType.TinyInt)
    };

    protected override SqlParameter BindMappingTable(
      string parameterName,
      IEnumerable<Mapping> rows)
    {
      rows = rows ?? Enumerable.Empty<Mapping>();
      Dictionary<int, string> dataspaceProjectNameMap = new Dictionary<int, string>();
      System.Func<Mapping, SqlDataRecord> selector = (System.Func<Mapping, SqlDataRecord>) (mapping =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_Mapping3);
        int dataspaceId;
        string projectName;
        string pathWithProjectGuid = this.BestEffortConvertToPathWithProjectGuid(mapping.ItemPathPair, out dataspaceId, out projectName);
        if (dataspaceId == 0)
          return (SqlDataRecord) null;
        if (!string.IsNullOrEmpty(projectName))
        {
          if (!dataspaceProjectNameMap.ContainsKey(dataspaceId))
            dataspaceProjectNameMap.Add(dataspaceId, projectName);
          else if (!string.Equals(projectName, dataspaceProjectNameMap[dataspaceId], StringComparison.OrdinalIgnoreCase))
            throw new ProjectRenamedMappingConflict(projectName, dataspaceProjectNameMap[dataspaceId]);
        }
        sqlDataRecord.SetInt32(0, dataspaceId);
        if (string.IsNullOrWhiteSpace(projectName))
          sqlDataRecord.SetDBNull(1);
        else
          sqlDataRecord.SetString(1, Microsoft.TeamFoundation.Framework.Server.DBPath.UserToDatabasePath(projectName));
        sqlDataRecord.SetString(2, DBPath.ServerToDatabasePath(pathWithProjectGuid));
        if (mapping is WorkingFolder workingFolder2 && workingFolder2.Type == WorkingFolderType.Map && workingFolder2.LocalItem != null)
          sqlDataRecord.SetString(3, DBPath.LocalToDatabasePath(workingFolder2.LocalItem));
        else
          sqlDataRecord.SetDBNull(3);
        sqlDataRecord.SetBoolean(4, mapping.Type != WorkingFolderType.Cloak);
        sqlDataRecord.SetByte(5, (byte) mapping.Depth);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Mapping3", rows.Select<Mapping, SqlDataRecord>(selector).Where<SqlDataRecord>((System.Func<SqlDataRecord, bool>) (r => r != null)));
    }

    public override int CreateWorkspace(WorkspaceInternal workspace)
    {
      this.PrepareStoredProcedure("prc_CreateWorkspace");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, true, SqlDbType.NVarChar);
      this.BindString("@computerName", workspace.Computer, 64, true, SqlDbType.NVarChar);
      this.BindString("@comment", workspace.Comment, -1, false, SqlDbType.NVarChar);
      this.BindBoolean("@isLocal", workspace.IsLocal);
      this.BindMappingTable("@folderList", (IEnumerable<Mapping>) workspace.InternalFolders);
      this.BindGuid("@workspaceNamespaceGuid", SecurityConstants.WorkspaceSecurityNamespaceGuid);
      this.BindString("@securityToken", workspace.SecurityToken, 578, false, SqlDbType.NVarChar);
      this.BindBoolean("@fileTime", (workspace.WorkspaceOptions & WorkspaceOptions.SetFileTimeToCheckin) != 0);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      return sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("WorkspaceId"));
    }

    public override void DeleteWorkspace(Guid ownerId, string workspaceName, string securityToken)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkspace", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", ownerId);
      this.BindString("@workspaceName", workspaceName, 64, false, SqlDbType.NVarChar);
      this.BindGuid("@author", this.Author);
      this.BindGuid("@workspaceNamespaceGuid", SecurityConstants.WorkspaceSecurityNamespaceGuid);
      this.BindString("@securityToken", securityToken, 578, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override Guid QueryPendingChangeSignature(string workspaceName, Guid ownerId)
    {
      this.PrepareStoredProcedure("prc_QueryPendingChangeSignature");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindGuid("@ownerId", ownerId);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      return sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("PendingChangeSignature"));
    }

    public override IList<WorkspaceInternal> QueryWorkspaceMappingsByProjectName(string projectName)
    {
      this.PrepareStoredProcedure("prc_QueryWorkspaceMappingsByProjectName");
      this.BindString("@projectName", Microsoft.TeamFoundation.Framework.Server.DBPath.UserToDatabasePath(projectName), 256, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<WorkspaceInternal>((ObjectBinder<WorkspaceInternal>) new WorkspaceColumns());
      return (IList<WorkspaceInternal>) resultCollection.GetCurrent<WorkspaceInternal>().Items;
    }

    public override void UpdateWorkspace(
      Guid ownerId,
      string workspaceName,
      Guid updatedOwnerId,
      string updatedName,
      string updatedComment,
      string updatedComputerName,
      List<WorkingFolder> workingFolders,
      string originalSecurityToken,
      string newSecurityToken,
      WorkspaceOptions options,
      bool isLocalWorkspace)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkspace");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerIdentity", ownerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindString("@updatedName", updatedName, 64, true, SqlDbType.NVarChar);
      this.BindGuid("@updatedIdentity", updatedOwnerId);
      this.BindString("@updatedComment", updatedComment, -1, false, SqlDbType.NVarChar);
      this.BindString("@updatedComputerName", updatedComputerName, 64, true, SqlDbType.NVarChar);
      this.BindMappingTable("@folderList", (IEnumerable<Mapping>) workingFolders);
      this.BindGuid("@author", this.Author);
      this.BindGuid("@workspaceNamespaceGuid", SecurityConstants.WorkspaceSecurityNamespaceGuid);
      this.BindString("@originalSecurityToken", originalSecurityToken, 578, false, SqlDbType.NVarChar);
      this.BindString("@newSecurityToken", newSecurityToken, 578, false, SqlDbType.NVarChar);
      this.BindBoolean("@fileTime", (options & WorkspaceOptions.SetFileTimeToCheckin) != 0);
      this.BindBoolean("@isLocal", isLocalWorkspace);
      this.ExecuteNonQuery();
    }

    protected override WorkingFolderColumns CreateWorkingFolderColumns() => (WorkingFolderColumns) new WorkingFolderColumns15((VersionControlSqlResourceComponent) this);
  }
}
