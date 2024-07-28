// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelComponent6
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
  internal class LabelComponent6 : LabelComponent5
  {
    private static readonly SqlMetaData[] typ_CreateLabelInput2 = new SqlMetaData[4]
    {
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("VersionFrom", SqlDbType.Int),
      new SqlMetaData("FromChildLabel", SqlDbType.Bit)
    };

    protected override SqlParameter BindLabelItemTable(
      string parameterName,
      IEnumerable<LabelItem> rows)
    {
      rows = rows ?? Enumerable.Empty<LabelItem>();
      System.Func<LabelItem, SqlDataRecord> selector = (System.Func<LabelItem, SqlDataRecord>) (item =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(LabelComponent6.typ_CreateLabelInput2);
        sqlDataRecord.SetInt32(0, this.GetDataspaceIdFromPath(item.ServerItem));
        sqlDataRecord.SetInt32(1, item.ItemId);
        if (item.VersionFrom != 0)
          sqlDataRecord.SetInt32(2, item.VersionFrom);
        else
          sqlDataRecord.SetDBNull(2);
        sqlDataRecord.SetBoolean(3, item.FromChildLabel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CreateLabelInput2", rows.Select<LabelItem, SqlDataRecord>(selector));
    }

    protected override LabelColumns CreateLabelColumns() => (LabelColumns) new LabelColumns6((VersionControlSqlResourceComponent) this);

    protected override LabelWithFilterItemColumns CreateLabelWithFilterItemColumns() => (LabelWithFilterItemColumns) new LabelWithFilterItemColumns6((VersionControlSqlResourceComponent) this);

    public override ResultCollection CreateLabel(
      string labelName,
      ItemPathPair labelScopePair,
      Guid ownerId,
      string comment,
      List<LabelItem> items)
    {
      this.PrepareStoredProcedure("prc_CreateLabel", 3600);
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@labelScopeDataspaceId", "@labelScope", labelScopePair, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindLabelItemTable("@labelItems", (IEnumerable<LabelItem>) items);
      ResultCollection label = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      label.AddBinder<Failure>((ObjectBinder<Failure>) new LabelFailureBinder6((VersionControlSqlResourceComponent) this));
      return label;
    }

    public override void CreateRecursiveLabel(
      string labelName,
      ItemPathPair labelScopePair,
      Guid ownerId,
      string comment,
      string serverItem,
      int versionFrom)
    {
      this.PrepareStoredProcedure("prc_CreateRecursiveLabel");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@labelScopeDataspaceId", "@labelScope", labelScopePair, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindServerItem("@serverItem", this.ConvertToPathWithProjectGuid(serverItem), true);
      this.BindInt("@versionFrom", versionFrom);
      this.ExecuteNonQuery();
    }

    public override void CreateWorkspaceLabel(
      string labelName,
      ItemPathPair labelScopePair,
      Guid ownerId,
      string comment,
      Guid workspaceOwnerId,
      string workspaceName,
      string serverItem)
    {
      this.PrepareStoredProcedure("prc_CreateWorkspaceLabel", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@labelScopeDataspaceId", "@labelScope", labelScopePair, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@workspaceOwnerId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindServerItem("@serverItem", this.ConvertToPathWithProjectGuid(serverItem), true);
      this.ExecuteNonQuery();
    }

    public override void CreateWorkspaceLabelLocal(
      string labelName,
      ItemPathPair labelScopePair,
      Guid ownerId,
      string comment,
      Guid workspaceOwnerId,
      string workspaceName,
      string localItem,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_CreateWorkspaceLabelLocal", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@labelScopeDataspaceId", "@labelScope", labelScopePair, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@workspaceOwnerId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, true);
      this.ExecuteNonQuery();
    }

    public override ResultCollection UpdateLabel(
      string labelName,
      ItemPathPair labelScopePair,
      Guid ownerId,
      string comment,
      List<LabelItem> items)
    {
      this.PrepareStoredProcedure("prc_UpdateLabel", 3600);
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindServerItemPathPair("@labelScope", labelScopePair, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindLabelItemTable("@labelItems", (IEnumerable<LabelItem>) items);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new LabelFailureBinder6((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public override ResultCollection CompareLabels(
      string startLabelName,
      string startLabelScope,
      string endLabelName,
      string endLabelScope,
      bool includeFiles,
      int minChangeSet,
      int maxChangeSets)
    {
      this.PrepareStoredProcedure("prc_CompareLabels");
      this.BindString("@startLabelName", startLabelName, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@startLabelScope", DBPath.ServerToDatabasePath(this.ConvertToPathWithProjectGuid(startLabelScope)), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@endLabelName", endLabelName, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@endLabelScope", DBPath.ServerToDatabasePath(this.ConvertToPathWithProjectGuid(endLabelScope)), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@includeFiles", includeFiles);
      this.BindInt("@minChangeSet", minChangeSet);
      this.BindInt("@maxChangeSets", maxChangeSets);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      if (includeFiles)
        resultCollection.AddBinder<Change>((ObjectBinder<Change>) new ChangeColumns15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public override void DeleteLabel(string labelName, ItemPathPair itemPathPair)
    {
      this.PrepareStoredProcedure("prc_DeleteLabel");
      this.BindString("@labelName", labelName, 64, false, SqlDbType.NVarChar);
      this.BindServerItemPathPair("@serverItem", itemPathPair, false);
      this.ExecuteNonQuery();
    }

    public override ResultCollection QueryLabels(
      string labelName,
      ItemPathPair labelScopePair,
      Microsoft.VisualStudio.Services.Identity.Identity owner,
      ItemPathPair itemPathPair,
      int versionItem)
    {
      this.PrepareStoredProcedure("prc_QueryLabels", 3600);
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindServerItemPathPair("@labelScope", labelScopePair, true);
      this.BindNullableGuid("@ownerId", owner == null ? Guid.Empty : owner.Id);
      this.BindServerItemPathPair("@serverItem", itemPathPair, true);
      this.BindNullableInt("@versionItem", versionItem, 0);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!string.IsNullOrEmpty(itemPathPair.ProjectNamePath))
        resultCollection.AddBinder<VersionControlLabel>((ObjectBinder<VersionControlLabel>) this.CreateLabelWithFilterItemColumns());
      else
        resultCollection.AddBinder<VersionControlLabel>((ObjectBinder<VersionControlLabel>) this.CreateLabelColumns());
      return resultCollection;
    }
  }
}
