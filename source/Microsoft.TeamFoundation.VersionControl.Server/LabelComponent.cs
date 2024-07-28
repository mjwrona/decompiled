// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelComponent
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
  internal class LabelComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<LabelComponent>(1, true),
      (IComponentCreator) new ComponentCreator<LabelComponent2>(2),
      (IComponentCreator) new ComponentCreator<LabelComponent3>(3),
      (IComponentCreator) new ComponentCreator<LabelComponent4>(4),
      (IComponentCreator) new ComponentCreator<LabelComponent5>(5),
      (IComponentCreator) new ComponentCreator<LabelComponent6>(6),
      (IComponentCreator) new ComponentCreator<LabelComponent7>(7)
    }, "VCLabel");
    private static readonly SqlMetaData[] typ_CreateLabelInput = new SqlMetaData[3]
    {
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("VersionFrom", SqlDbType.Int),
      new SqlMetaData("FromChildLabel", SqlDbType.Bit)
    };

    protected virtual SqlParameter BindLabelItemTable(
      string parameterName,
      IEnumerable<LabelItem> rows)
    {
      rows = rows ?? Enumerable.Empty<LabelItem>();
      System.Func<LabelItem, SqlDataRecord> selector = (System.Func<LabelItem, SqlDataRecord>) (item =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(LabelComponent.typ_CreateLabelInput);
        sqlDataRecord.SetInt32(0, item.ItemId);
        if (item.VersionFrom != 0)
          sqlDataRecord.SetInt32(1, item.VersionFrom);
        else
          sqlDataRecord.SetDBNull(1);
        sqlDataRecord.SetBoolean(2, item.FromChildLabel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CreateLabelInput", rows.Select<LabelItem, SqlDataRecord>(selector));
    }

    protected virtual LabelColumns CreateLabelColumns() => new LabelColumns();

    protected virtual LabelWithFilterItemColumns CreateLabelWithFilterItemColumns() => new LabelWithFilterItemColumns();

    public virtual void DeleteLabel(string labelName, ItemPathPair serverItem)
    {
      this.PrepareStoredProcedure("prc_DeleteLabel");
      this.BindString("@labelName", labelName, 64, false, SqlDbType.NVarChar);
      this.BindPreDataspaceServerItemPathPair("@serverItem", serverItem, false);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection CreateLabel(
      string labelName,
      ItemPathPair labelScope,
      Guid ownerId,
      string comment,
      List<LabelItem> items)
    {
      return this.createOrUpdateLabel("prc_CreateLabel", labelName, labelScope.ProjectNamePath, ownerId, comment, items);
    }

    public virtual void CreateRecursiveLabel(
      string labelName,
      ItemPathPair labelScope,
      Guid ownerId,
      string comment,
      string serverItem,
      int versionFrom)
    {
      this.PrepareStoredProcedure("prc_CreateRecursiveLabel");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindPreDataspaceServerItemPathPair("@labelScope", labelScope, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindServerItem("@serverItem", serverItem, true);
      this.BindInt("@versionFrom", versionFrom);
      this.ExecuteNonQuery();
    }

    public virtual void CreateRecursiveLabel(
      string labelName,
      ItemPathPair labelScopePair,
      Guid ownerId,
      string comment,
      List<RecursiveLabelItem> items)
    {
      this.CreateRecursiveLabel(labelName, labelScopePair, ownerId, comment, items[0].ServerItem, items[0].VersionFrom);
    }

    public virtual void CreateWorkspaceLabel(
      string labelName,
      ItemPathPair labelScope,
      Guid ownerId,
      string comment,
      Guid workspaceOwnerId,
      string workspaceName,
      string serverItem)
    {
      this.PrepareStoredProcedure("prc_CreateWorkspaceLabel");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindPreDataspaceServerItemPathPair("@labelScope", labelScope, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@workspaceOwnerId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindServerItem("@serverItem", serverItem, true);
      this.ExecuteNonQuery();
    }

    public virtual void CreateWorkspaceLabelLocal(
      string labelName,
      ItemPathPair labelScope,
      Guid ownerId,
      string comment,
      Guid workspaceOwnerId,
      string workspaceName,
      string localItem,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_CreateWorkspaceLabelLocal");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindPreDataspaceServerItemPathPair("@labelScope", labelScope, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@workspaceOwnerId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, true);
      this.ExecuteNonQuery();
    }

    protected ResultCollection createOrUpdateLabel(
      string procedureName,
      string labelName,
      string labelScope,
      Guid ownerId,
      string comment,
      List<LabelItem> items)
    {
      this.PrepareStoredProcedure(procedureName);
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindServerItem("@labelScope", labelScope, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindLabelItemTable("@labelItems", (IEnumerable<LabelItem>) items);
      ResultCollection orUpdateLabel = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      orUpdateLabel.AddBinder<Failure>((ObjectBinder<Failure>) new LabelFailureBinder());
      return orUpdateLabel;
    }

    public ResultCollection FindLabelByLabelId(int labelId)
    {
      this.PrepareStoredProcedure("prc_FindLabelByLabelId");
      this.BindInt("@labelId", labelId);
      ResultCollection labelByLabelId = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      labelByLabelId.AddBinder<VersionControlLabel>((ObjectBinder<VersionControlLabel>) this.CreateLabelColumns());
      return labelByLabelId;
    }

    public virtual ResultCollection QueryLabels(
      string labelName,
      ItemPathPair labelScope,
      Microsoft.VisualStudio.Services.Identity.Identity owner,
      ItemPathPair serverItem,
      int versionItem)
    {
      this.PrepareStoredProcedure("prc_QueryLabels", 3600);
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindPreDataspaceServerItemPathPair("@labelScope", labelScope, true);
      this.BindNullableGuid("@ownerId", owner == null ? Guid.Empty : owner.Id);
      this.BindPreDataspaceServerItemPathPair("@serverItem", serverItem, true);
      this.BindNullableInt("@versionItem", versionItem, 0);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!string.IsNullOrEmpty(serverItem.ProjectNamePath))
        resultCollection.AddBinder<VersionControlLabel>((ObjectBinder<VersionControlLabel>) this.CreateLabelWithFilterItemColumns());
      else
        resultCollection.AddBinder<VersionControlLabel>((ObjectBinder<VersionControlLabel>) this.CreateLabelColumns());
      return resultCollection;
    }

    public virtual ResultCollection UpdateLabel(
      string labelName,
      ItemPathPair labelScope,
      Guid ownerId,
      string comment,
      List<LabelItem> items)
    {
      return this.createOrUpdateLabel("prc_UpdateLabel", labelName, labelScope.ProjectNamePath, ownerId, comment, items);
    }

    public virtual ResultCollection CompareLabels(
      string startLabelName,
      string startLabelScope,
      string endLabelName,
      string endLabelScope,
      bool includeFiles,
      int minChangeSet,
      int maxChangeSets)
    {
      throw new FeatureNotSupportedException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("CannotCompareLabels"));
    }
  }
}
