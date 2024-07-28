// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ClassificationNodeComponent3 : ClassificationNodeComponent2
  {
    public override IEnumerable<ClassificationNodeUpdateError> CreateClassificationNodes(
      Guid projectId,
      IEnumerable<ClassificationNodeUpdate> nodes,
      Guid requestIdentityId,
      out int nodeSequenceId)
    {
      nodeSequenceId = 0;
      this.PrepareStoredProcedure("prc_CreateClassificationNodes");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindClassificationNodeTable("@nodes", nodes);
      this.BindGuid("@changerTfId", requestIdentityId);
      this.BindGuid("@eventAuthor", this.Author);
      return (IEnumerable<ClassificationNodeUpdateError>) this.ExecuteUnknown<List<ClassificationNodeUpdateError>>((System.Func<IDataReader, List<ClassificationNodeUpdateError>>) (reader => new ClassificationNodeComponent.ClassificationNodeUpdateErrorBinder().BindAll(reader).ToList<ClassificationNodeUpdateError>()));
    }

    public override ClassificationNodeUpdateError UpdateClassificationNode(
      Guid projectId,
      Guid changerTfId,
      int nodeId,
      string nodeName,
      int? parentId,
      DateTime? startDate,
      DateTime? finishDate,
      bool setDates,
      int timeout,
      bool forceUpdate,
      out int nodeSequenceId)
    {
      nodeSequenceId = 0;
      this.PrepareStoredProcedure("prc_UpdateClassificationNode");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@nodeId", nodeId);
      this.BindString("@name", nodeName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      if (parentId.HasValue)
        this.BindInt("@parentId", parentId.Value);
      else
        this.BindNullValue("@parentId", SqlDbType.Int);
      this.BindIterationDate("@startDate", startDate);
      this.BindIterationDate("@finishDate", finishDate);
      this.BindBoolean("@areDatesChanged", setDates);
      this.BindGuid("@changerTfId", changerTfId);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindInt("@timeout", timeout);
      this.BindBoolean("@forceUpdate", forceUpdate);
      return this.ExecuteUnknown<ClassificationNodeUpdateError>((System.Func<IDataReader, ClassificationNodeUpdateError>) (reader => new ClassificationNodeComponent.ClassificationNodeUpdateErrorBinder().BindAll(reader).FirstOrDefault<ClassificationNodeUpdateError>()));
    }

    public override IEnumerable<ClassificationNodeUpdateError> DeleteClassificationNodes(
      Guid projectId,
      Guid changerTfId,
      IEnumerable<ClassificationNodeUpdate> nodes,
      int timeout,
      out int nodeSequenceId)
    {
      nodeSequenceId = 0;
      this.PrepareStoredProcedure("prc_DeleteClassificationNodes");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindClassificationNodeTable("@nodes", nodes);
      this.BindGuid("@changerTfId", changerTfId);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindInt("@timeout", timeout);
      return (IEnumerable<ClassificationNodeUpdateError>) this.ExecuteUnknown<List<ClassificationNodeUpdateError>>((System.Func<IDataReader, List<ClassificationNodeUpdateError>>) (reader => new ClassificationNodeComponent.ClassificationNodeUpdateErrorBinder().BindAll(reader).ToList<ClassificationNodeUpdateError>()));
    }

    public override List<TreeNodeRecord> GetClassificationNodes(
      IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId> ids,
      bool includeDeleted)
    {
      this.PrepareStoredProcedure("prc_GetClassificationNodesWithAncestors");
      this.BindKeyValuePairInt32Int32Table("@ids", ids.Select<WorkItemTrackingTreeService.ClassificationNodeId, KeyValuePair<int, int>>((System.Func<WorkItemTrackingTreeService.ClassificationNodeId, KeyValuePair<int, int>>) (x =>
      {
        int key = 0;
        if (x.ProjectId != Guid.Empty)
          key = this.GetDataspaceId(x.ProjectId);
        return new KeyValuePair<int, int>(key, x.NodeId);
      })));
      this.BindBoolean("@includeDeleted", includeDeleted);
      return this.ExecuteUnknown<List<TreeNodeRecord>>((System.Func<IDataReader, List<TreeNodeRecord>>) (reader => this.GetClassificationNodeBinder().BindAll(reader).ToList<TreeNodeRecord>()));
    }
  }
}
