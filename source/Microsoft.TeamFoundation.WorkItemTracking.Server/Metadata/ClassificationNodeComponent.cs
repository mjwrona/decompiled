// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ClassificationNodeComponent : WorkItemTrackingResourceComponent
  {
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponent2>(2),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponent3>(3),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponent4>(4),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponent5>(5),
      (IComponentCreator) new ComponentCreator<ClassificationNodeComponent6>(6)
    }, "ClassificationNode", "WorkItem");
    private static readonly SqlMetaData[] typ_ClassificationNodeTable = new SqlMetaData[10]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("StructureType", SqlDbType.Int),
      new SqlMetaData("ParentIdentifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ReclassifyId", SqlDbType.Int),
      new SqlMetaData("IsDeleted", SqlDbType.Bit),
      new SqlMetaData("StartDate", SqlDbType.DateTime),
      new SqlMetaData("FinishDate", SqlDbType.DateTime),
      new SqlMetaData("AreDatesChanged", SqlDbType.Bit)
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => ClassificationNodeComponent.s_sqlExceptionFactories;

    static ClassificationNodeComponent()
    {
      ClassificationNodeComponent.s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400013, new SqlExceptionFactory(typeof (CreateACENoObjectException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400014, new SqlExceptionFactory(typeof (CreateACENoActionException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400015, new SqlExceptionFactory(typeof (UnregisterProjectException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400016, new SqlExceptionFactory(typeof (RegisterProjectException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400017, new SqlExceptionFactory(typeof (InternalStoredProcedureException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400018, new SqlExceptionFactory(typeof (RegisterObjectExistsException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400019, new SqlExceptionFactory(typeof (RegisterObjectNoClassException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400020, new SqlExceptionFactory(typeof (RegisterObjectNoProjectException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400035, new SqlExceptionFactory(typeof (RegisterObjectProjectMismatchException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400021, new SqlExceptionFactory(typeof (RegisterObjectBadParentException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400027, new SqlExceptionFactory(typeof (ClassIdDoesNotExistException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400028, new SqlExceptionFactory(typeof (SecurityObjectDoesNotExistException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400029, new SqlExceptionFactory(typeof (SecurityActionDoesNotExistException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400033, new SqlExceptionFactory(typeof (BadParentObjectClassIdException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400034, new SqlExceptionFactory(typeof (CircularObjectInheritanceException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400032, new SqlExceptionFactory(typeof (AddProjectGroupProjectMismatchException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AddProjectGroupProjectMismatchException(sqEr.ExtractString("group_name"), sqEr.ExtractString("member_name")))));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(400030, new SqlExceptionFactory(typeof (DeleteACEException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450000, new SqlExceptionFactory(typeof (NodeDoesNotExistException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450001, new SqlExceptionFactory(typeof (ProjectDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectDoesNotExistException(sqEr.ExtractString("project_id")))));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450002, new SqlExceptionFactory(typeof (ParentNodeDoesNotExistException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450003, new SqlExceptionFactory(typeof (ReclassificationNodeDoesNotExistException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450004, new SqlExceptionFactory(typeof (ProjectAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectAlreadyExistsException(sqEr.ExtractString("project_name")))));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450005, new SqlExceptionFactory(typeof (NodeAlreadyExistsException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450006, new SqlExceptionFactory(typeof (CannotModifyRootNodeException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450007, new SqlExceptionFactory(typeof (MoveArgumentOutOfRangeException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450008, new SqlExceptionFactory(typeof (CircularNodeReferenceException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450009, new SqlExceptionFactory(typeof (CannotChangeTreesException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450010, new SqlExceptionFactory(typeof (MaximumDepthExceededException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450011, new SqlExceptionFactory(typeof (ReclassifiedToDifferentTreeException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450012, new SqlExceptionFactory(typeof (ReclassifiedToSubTreeException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450013, new SqlExceptionFactory(typeof (ProjectNameNotRecognizedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProjectNameNotRecognizedException(sqEr.ExtractString("project_name")))));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(450014, new SqlExceptionFactory(typeof (CannotAddDateToNonIterationException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(470006, new SqlExceptionFactory(typeof (SyncBadBaselineRevServiceException)));
      ClassificationNodeComponent.s_sqlExceptionFactories.Add(470007, new SqlExceptionFactory(typeof (SyncSupersededBaselineRevServiceException)));
    }

    public virtual List<TreeNodeRecord> GetAllClassificationNodes(bool disableDataspaceRls = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      this.PrepareStoredProcedure("GetAllNodes");
      return this.ExecuteUnknown<List<TreeNodeRecord>>((System.Func<IDataReader, List<TreeNodeRecord>>) (reader => this.GetClassificationNodeBinder().BindAll(reader).Where<TreeNodeRecord>((System.Func<TreeNodeRecord, bool>) (x => x.Id != 0)).ToList<TreeNodeRecord>()));
    }

    public virtual List<TreeNodeRecord> GetClassificationNodes(
      IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId> ids,
      bool includeDeleted)
    {
      this.PrepareStoredProcedure("prc_GetTreeNodesWithAncestors");
      this.BindInt32Table("@ids", ids.Select<WorkItemTrackingTreeService.ClassificationNodeId, int>((System.Func<WorkItemTrackingTreeService.ClassificationNodeId, int>) (x => x.NodeId)));
      this.BindBoolean("@includeDeleted", includeDeleted);
      return this.ExecuteUnknown<List<TreeNodeRecord>>((System.Func<IDataReader, List<TreeNodeRecord>>) (reader => this.GetClassificationNodeBinder().BindAll(reader).ToList<TreeNodeRecord>()));
    }

    public virtual List<TreeNodeRecord> GetChangedClassificationNodesByCachestamp(
      long cachestamp,
      out long lastCachestmp,
      bool disableDataspaceRls = false)
    {
      throw new NotImplementedException();
    }

    public virtual List<TreeNodeRecord> GetChangedClassificationNodesBySequenceId(
      int sequenceId,
      out int lastSequenceId)
    {
      throw new NotImplementedException();
    }

    public virtual List<TreeNodeRecord> GetDeletedClassificationNodes(
      Guid projectId,
      DateTime since)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<ClassificationNodeUpdateError> CreateClassificationNodes(
      Guid projectId,
      IEnumerable<ClassificationNodeUpdate> nodes,
      Guid requestIdentityId,
      out int nodeSequenceId)
    {
      nodeSequenceId = 0;
      if (nodes.Count<ClassificationNodeUpdate>() == 1)
      {
        ClassificationNodeUpdate classificationNodeUpdate = nodes.First<ClassificationNodeUpdate>();
        this.PrepareStoredProcedure("prc_css_create_node");
        this.BindString("@id", classificationNodeUpdate.Identifier.ToString("D").ToLowerInvariant(), 128, false, SqlDbType.VarChar);
        this.BindString("@name", DBPath.UserToDatabasePath(classificationNodeUpdate.Name), (int) byte.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@parent_id", classificationNodeUpdate.ParentIdentifier.ToString("D").ToLowerInvariant(), 128, false, SqlDbType.VarChar);
        this.BindGuid("@user_id", requestIdentityId);
        this.BindIterationDate("@start", classificationNodeUpdate.StartDate);
        this.BindIterationDate("@end", classificationNodeUpdate.FinishDate);
        this.BindDateTime("@time_stamp", DateTime.UtcNow);
      }
      else
      {
        this.PrepareStoredProcedure("prc_css_create_initial_nodes");
        this.BindGuid("@project_id", projectId);
        this.BindString("@nodes", ClassificationNodeComponent.ConvertToCssDbXml(this.RequestContext, projectId, nodes), 0, false, SqlDbType.NVarChar);
        this.BindGuid("@user_id", requestIdentityId);
        this.BindDateTime("@time_stamp", DateTime.UtcNow);
      }
      this.ExecuteNonQuery();
      return (IEnumerable<ClassificationNodeUpdateError>) new ClassificationNodeUpdateError[1]
      {
        new ClassificationNodeUpdateError()
        {
          Id = 0,
          Identifier = Guid.Empty,
          ErrorCode = 3
        }
      };
    }

    protected void BindIterationDate(string parameterName, DateTime? dateTime) => this.BindNullableDateTime(parameterName, dateTime.HasValue ? new DateTime?(dateTime.Value.Date) : dateTime);

    internal static string ConvertToCssDbXml(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ClassificationNodeUpdate> nodes)
    {
      Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), projectId);
      dictionary[projectId] = "\\" + DBPath.UserToDatabasePath(projectName, true);
      StringBuilder output = new StringBuilder();
      int num = 0;
      using (XmlWriter xmlWriter = XmlWriter.Create(output))
      {
        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("Nodes");
        foreach (ClassificationNodeUpdate node in nodes)
        {
          if (!(node.ParentIdentifier == Guid.Empty))
          {
            xmlWriter.WriteStartElement("Node");
            xmlWriter.WriteAttributeString("order", (++num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("id", node.Identifier.ToString("D"));
            if (node.ParentIdentifier != projectId)
              xmlWriter.WriteAttributeString("parent_id", node.ParentIdentifier.ToString("D"));
            string databasePath = DBPath.UserToDatabasePath(node.Name);
            string str1 = dictionary[node.ParentIdentifier] + databasePath + "\\";
            dictionary[node.Identifier] = str1;
            xmlWriter.WriteAttributeString("name", databasePath);
            xmlWriter.WriteAttributeString("path", dictionary[node.ParentIdentifier] + databasePath);
            string str2 = node.StructureType == TreeStructureType.Area ? "ProjectModelHierarchy" : "ProjectLifecycle";
            xmlWriter.WriteAttributeString("type", str2);
            xmlWriter.WriteEndElement();
          }
        }
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
      }
      return output.ToString();
    }

    public virtual ClassificationNodeUpdateError UpdateClassificationNode(
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
      string lowerInvariant1 = this.RequestContext.WitContext().TreeService.GetTreeNode(projectId, nodeId).CssNodeId.ToString("D").ToLowerInvariant();
      if (nodeName != null)
      {
        if (parentId.HasValue | setDates)
          throw new NotSupportedException();
        this.PrepareStoredProcedure("prc_css_rename_node");
        this.BindString("@id", lowerInvariant1, 128, false, SqlDbType.VarChar);
        this.BindString("@name", DBPath.UserToDatabasePath(nodeName), (int) byte.MaxValue, false, SqlDbType.NVarChar);
        this.BindDateTime("@time_stamp", DateTime.UtcNow);
      }
      else if (parentId.HasValue)
      {
        if (nodeName != null | setDates)
          throw new NotSupportedException();
        string lowerInvariant2 = this.RequestContext.WitContext().TreeService.GetTreeNode(projectId, parentId.Value).CssNodeId.ToString("D").ToLowerInvariant();
        this.PrepareStoredProcedure("prc_css_move_node");
        this.BindString("@id", lowerInvariant1, 128, false, SqlDbType.VarChar);
        this.BindString("@parent_id", lowerInvariant2, 128, false, SqlDbType.VarChar);
        this.BindDateTime("@time_stamp", DateTime.UtcNow);
      }
      else if (setDates)
      {
        if (nodeName != null || parentId.HasValue)
          throw new NotSupportedException();
        this.PrepareStoredProcedure("prc_css_set_iteration_dates");
        this.BindString("@id", lowerInvariant1, 128, false, SqlDbType.VarChar);
        this.BindIterationDate("@start", startDate);
        this.BindIterationDate("@end", finishDate);
        this.BindDateTime("@time_stamp", DateTime.UtcNow);
      }
      this.ExecuteNonQuery();
      return new ClassificationNodeUpdateError()
      {
        Id = 0,
        Identifier = Guid.Empty,
        ErrorCode = 3
      };
    }

    public virtual IEnumerable<ClassificationNodeUpdateError> DeleteClassificationNodes(
      Guid projectId,
      Guid changerTfId,
      IEnumerable<ClassificationNodeUpdate> nodes,
      int timeout,
      out int nodeSequenceId)
    {
      nodeSequenceId = 0;
      foreach (ClassificationNodeUpdate node in nodes)
      {
        TreeNode treeNode = this.RequestContext.WitContext().TreeService.GetTreeNode(projectId, node.Id);
        TreeNode treeNode1 = this.RequestContext.WitContext().TreeService.GetTreeNode(projectId, node.ReclassifyId);
        if (treeNode1.IsProject)
          treeNode1 = treeNode1.Children.Values.First<TreeNode>((System.Func<TreeNode, bool>) (x => x.Type == treeNode.Type));
        this.PrepareStoredProcedure("prc_css_delete_node");
        this.BindString("@id", treeNode.CssNodeId.ToString("D").ToLowerInvariant(), 128, false, SqlDbType.VarChar);
        this.BindString("@reclassify_id", treeNode1.CssNodeId.ToString("D").ToLowerInvariant(), 128, false, SqlDbType.VarChar);
        this.BindString("@user_name", this.RequestContext.DomainUserName, 256, false, SqlDbType.NVarChar);
        this.BindDateTime("@time_stamp", DateTime.UtcNow);
        this.ExecuteNonQuery();
      }
      return (IEnumerable<ClassificationNodeUpdateError>) new ClassificationNodeUpdateError[1]
      {
        new ClassificationNodeUpdateError()
        {
          Id = 0,
          Identifier = Guid.Empty,
          ErrorCode = 3
        }
      };
    }

    public virtual WorkItemClassificationReconciliationState ReconcileWorkItems(
      Guid requestIdentityId)
    {
      return WorkItemClassificationReconciliationState.Succeeded;
    }

    public virtual IEnumerable<ClassificationNodeChange> GetClassificationNodeChanges(
      bool pendingReclassification,
      out long cachestamp)
    {
      cachestamp = 0L;
      return Enumerable.Empty<ClassificationNodeChange>();
    }

    public virtual void DeleteClassificationNodeChanges(long cachestamp)
    {
    }

    protected virtual ClassificationNodeComponent.ClassificationNodeBinder GetClassificationNodeBinder() => new ClassificationNodeComponent.ClassificationNodeBinder();

    public virtual void MarkRootNodeDeletionStatus(
      Guid projectId,
      Guid changerTfId,
      bool deleted,
      out int nodeSequenceId)
    {
      nodeSequenceId = 0;
    }

    protected SqlParameter BindClassificationNodeTable(
      string parameterName,
      IEnumerable<ClassificationNodeUpdate> nodes)
    {
      nodes = nodes ?? Enumerable.Empty<ClassificationNodeUpdate>();
      return this.BindTable(parameterName, "typ_ClassificationNodeTable", ClassificationNodeComponent.BindClassificationNodeRows(nodes));
    }

    private static IEnumerable<SqlDataRecord> BindClassificationNodeRows(
      IEnumerable<ClassificationNodeUpdate> nodes)
    {
      foreach (ClassificationNodeUpdate node in nodes)
      {
        SqlDataRecord record = new SqlDataRecord(ClassificationNodeComponent.typ_ClassificationNodeTable);
        ClassificationNodeComponent.BindNullableInt(record, 0, node.Id);
        ClassificationNodeComponent.BindNullableGuid(record, 1, node.Identifier);
        ClassificationNodeComponent.BindString(record, 2, node.Name);
        record.SetInt32(3, (int) node.StructureType);
        ClassificationNodeComponent.BindNullableGuid(record, 4, node.ParentIdentifier);
        ClassificationNodeComponent.BindNullableInt(record, 5, node.ReclassifyId);
        record.SetBoolean(6, node.IsDeleted);
        ClassificationNodeComponent.BindIterationDate(record, 7, node.StartDate);
        ClassificationNodeComponent.BindIterationDate(record, 8, node.FinishDate);
        record.SetBoolean(9, node.AreDatesChanged);
        yield return record;
      }
    }

    private static void BindNullableInt(SqlDataRecord record, int ordinal, int value)
    {
      if (value == 0)
        record.SetDBNull(ordinal);
      else
        record.SetInt32(ordinal, value);
    }

    private static void BindNullableGuid(SqlDataRecord record, int ordinal, Guid value)
    {
      if (value == Guid.Empty)
        record.SetDBNull(ordinal);
      else
        record.SetGuid(ordinal, value);
    }

    private static void BindIterationDate(SqlDataRecord record, int ordinal, DateTime? value)
    {
      if (!value.HasValue)
        record.SetDBNull(ordinal);
      else
        record.SetDateTime(ordinal, value.Value.Date);
    }

    private static void BindString(SqlDataRecord record, int ordinal, string value)
    {
      if (value == null)
        record.SetDBNull(ordinal);
      else
        record.SetString(ordinal, value);
    }

    protected class ClassificationNodeChangeBinder : 
      WorkItemTrackingObjectBinder<ClassificationNodeChange>
    {
      protected SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      protected SqlColumnBinder m_id = new SqlColumnBinder("Id");
      protected SqlColumnBinder m_path = new SqlColumnBinder("Path");
      protected System.Func<int, Guid> m_projectIdResolver;

      public ClassificationNodeChangeBinder(System.Func<int, Guid> projectIdResolver) => this.m_projectIdResolver = projectIdResolver;

      public override ClassificationNodeChange Bind(IDataReader reader) => new ClassificationNodeChange()
      {
        ProjectId = this.m_projectIdResolver(this.m_dataspaceId.GetInt32(reader)),
        Id = this.m_id.GetInt32(reader, 0),
        Path = this.m_path.GetString(reader, true)
      };
    }

    protected class ClassificationNodeUpdateErrorBinder : 
      WorkItemTrackingObjectBinder<ClassificationNodeUpdateError>
    {
      protected SqlColumnBinder m_id = new SqlColumnBinder("Id");
      protected SqlColumnBinder m_identifier = new SqlColumnBinder("Identifier");
      protected SqlColumnBinder m_errorCode = new SqlColumnBinder("ErrorCode");

      public override ClassificationNodeUpdateError Bind(IDataReader reader) => new ClassificationNodeUpdateError()
      {
        Id = this.m_id.GetInt32(reader, 0),
        Identifier = this.m_identifier.GetGuid(reader, true),
        ErrorCode = this.m_errorCode.GetInt32(reader)
      };
    }

    protected class ClassificationNodeBinder : WorkItemTrackingObjectBinder<TreeNodeRecord>
    {
      protected SqlColumnBinder m_id;
      protected SqlColumnBinder m_parentId;
      protected SqlColumnBinder m_name;
      protected SqlColumnBinder m_structureType;
      protected SqlColumnBinder m_typeId;
      protected SqlColumnBinder m_identifier;
      protected SqlColumnBinder m_isDeleted;

      public ClassificationNodeBinder()
      {
        this.m_id = new SqlColumnBinder("ID");
        this.m_parentId = new SqlColumnBinder("ParentID");
        this.m_name = new SqlColumnBinder("Name");
        this.m_structureType = new SqlColumnBinder("StructureType");
        this.m_typeId = new SqlColumnBinder("TypeID");
        this.m_identifier = new SqlColumnBinder("CssNodeId");
        this.m_isDeleted = new SqlColumnBinder("fDeleted");
      }

      public override TreeNodeRecord Bind(IDataReader reader)
      {
        int int32 = this.m_id.GetInt32(reader);
        object obj = this.m_identifier.GetObject(reader);
        result = Guid.Empty;
        if (!(obj is Guid result))
          Guid.TryParse(obj.ToString(), out result);
        bool flag = false;
        if (this.m_isDeleted.ColumnExists(reader))
          flag = this.m_isDeleted.GetBoolean(reader);
        return new TreeNodeRecord()
        {
          Id = int32,
          ParentId = this.m_parentId.GetInt32(reader, 0),
          Name = this.m_name.GetString(reader, false),
          StructureType = (TreeStructureType) this.m_structureType.GetInt32(reader),
          TreeTypeId = this.m_typeId.GetInt32(reader),
          CssNodeId = result,
          IsDeleted = flag
        };
      }
    }
  }
}
