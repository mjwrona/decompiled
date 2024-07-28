// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceComponent
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
  internal class WorkspaceComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<WorkspaceComponent>(1),
      (IComponentCreator) new ComponentCreator<WorkspaceComponent2>(2),
      (IComponentCreator) new ComponentCreator<WorkspaceComponent3>(3),
      (IComponentCreator) new ComponentCreator<WorkspaceComponent4>(4),
      (IComponentCreator) new ComponentCreator<WorkspaceComponent5>(5),
      (IComponentCreator) new ComponentCreator<WorkspaceComponent6>(6),
      (IComponentCreator) new ComponentCreator<WorkspaceComponent7>(7)
    }, "VCWorkspaces");
    private readonly SqlMetaData[] typ_Mapping = new SqlMetaData[4]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MappingType", SqlDbType.Bit),
      new SqlMetaData("Depth", SqlDbType.TinyInt)
    };

    protected virtual SqlParameter BindMappingTable(string parameterName, IEnumerable<Mapping> rows)
    {
      rows = rows ?? Enumerable.Empty<Mapping>();
      System.Func<Mapping, SqlDataRecord> selector = (System.Func<Mapping, SqlDataRecord>) (mapping =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_Mapping);
        sqlDataRecord.SetString(0, DBPath.ServerToDatabasePath(mapping.ServerItem));
        if (mapping is WorkingFolder workingFolder2 && workingFolder2.Type == WorkingFolderType.Map && workingFolder2.LocalItem != null)
          sqlDataRecord.SetString(1, DBPath.LocalToDatabasePath(workingFolder2.LocalItem));
        else
          sqlDataRecord.SetDBNull(1);
        sqlDataRecord.SetBoolean(2, mapping.Type != WorkingFolderType.Cloak);
        sqlDataRecord.SetByte(3, (byte) mapping.Depth);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Mapping", rows.Select<Mapping, SqlDataRecord>(selector));
    }

    public virtual int CreateWorkspace(WorkspaceInternal workspace)
    {
      this.PrepareStoredProcedure("prc_CreateWorkspace");
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

    public virtual void DeleteWorkspace(Guid ownerId, string workspaceName, string securityToken)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkspace", 3600);
      this.BindGuid("@ownerId", ownerId);
      this.BindString("@workspaceName", workspaceName, 64, false, SqlDbType.NVarChar);
      this.BindGuid("@author", this.Author);
      this.BindGuid("@workspaceNamespaceGuid", SecurityConstants.WorkspaceSecurityNamespaceGuid);
      this.BindString("@securityToken", securityToken, 578, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public ResultCollection QueryWorkspaces(
      Guid ownerId,
      string workspaceName,
      string computerName)
    {
      this.PrepareStoredProcedure("prc_QueryWorkspaces");
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindString("@computerName", computerName, 64, true, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<WorkspaceInternal>((ObjectBinder<WorkspaceInternal>) new WorkspaceColumns());
      resultCollection.AddBinder<WorkingFolder>((ObjectBinder<WorkingFolder>) this.CreateWorkingFolderColumns());
      return resultCollection;
    }

    protected virtual WorkingFolderColumns CreateWorkingFolderColumns() => new WorkingFolderColumns();

    public ResultCollection QueryWorkspaceMappingsIfUpdated(
      int workspaceId,
      ref DateTime lastKnownUpdate)
    {
      this.PrepareStoredProcedure("prc_QueryWorkspaceMappingsIfUpdated");
      this.BindInt("@workspaceId", workspaceId);
      this.BindDateTime("@lastKnownUpdate", lastKnownUpdate);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<bool, DateTime>>((ObjectBinder<KeyValuePair<bool, DateTime>>) new MappingsUpdatedColumns());
      resultCollection.AddBinder<WorkingFolder>((ObjectBinder<WorkingFolder>) this.CreateWorkingFolderColumns());
      KeyValuePair<bool, DateTime> keyValuePair = resultCollection.GetCurrent<KeyValuePair<bool, DateTime>>().Items[0];
      if (!keyValuePair.Key)
        return (ResultCollection) null;
      lastKnownUpdate = keyValuePair.Value;
      resultCollection.NextResult();
      return resultCollection;
    }

    public virtual void UpdateWorkspace(
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
      this.ExecuteNonQuery();
    }

    public virtual IList<WorkspaceInternal> QueryWorkspaceMappingsByProjectName(string projectName) => throw new NotImplementedException();
  }
}
