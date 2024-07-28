// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.StructureComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class StructureComponent : IntegrationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<StructureComponent>(1, true),
      (IComponentCreator) new ComponentCreator<StructureComponent2>(2),
      (IComponentCreator) new ComponentCreator<StructureComponent3>(3),
      (IComponentCreator) new ComponentCreator<StructureComponent4>(4),
      (IComponentCreator) new ComponentCreator<StructureComponent5>(5)
    }, "Node");

    public static StructureComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<StructureComponent>();

    public StructureComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.None;

    internal CommonStructureNodeInfo GetNode(string nodeId)
    {
      this.PrepareStoredProcedure("prc_css_get_node");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new StructureComponent.NodeInfoColumns().bind(reader) : throw new NodeDoesNotExistException(nodeId);
    }

    internal CommonStructureNodeInfo GetNodeFromPath(string nodePath)
    {
      this.PrepareStoredProcedure("prc_css_get_node_from_path");
      this.BindNodePath("@path", nodePath);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new StructureComponent.NodeInfoColumns().bind(reader) : throw new NodePathDoesNotExistException(nodePath);
    }

    internal virtual List<CommonStructureNodeInfo> GetNodesFromIds(List<string> ids)
    {
      List<CommonStructureNodeInfo> nodesFromIds = new List<CommonStructureNodeInfo>();
      string sqlStatement = "SELECT [id], [name], [parent_id], [project_id], [structure_type], [path], [start_date], [finish_date] FROM [dbo].[tbl_nodes] nodes inner join @nodeIds ids on nodes.[id] = ids.Data";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindStringTable("@nodeIds", (IEnumerable<string>) ids);
      SqlDataReader reader = this.ExecuteReader();
      StructureComponent.NodeInfoColumns nodeInfoColumns = new StructureComponent.NodeInfoColumns();
      while (reader.Read())
        nodesFromIds.Add(nodeInfoColumns.bind(reader));
      return nodesFromIds;
    }

    internal void GetNodes(
      string nodeId,
      out CommonStructureNodeInfo nodeInfo,
      out Dictionary<string, List<CommonStructureNodeInfo>> parentDictionary)
    {
      this.PrepareStoredProcedure("prc_css_get_nodes");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new NodeDoesNotExistException(nodeId);
      StructureComponent.NodeInfoColumns nodeInfoColumns = new StructureComponent.NodeInfoColumns();
      CommonStructureNodeInfo structureNodeInfo1 = nodeInfoColumns.bind(reader);
      nodeInfo = structureNodeInfo1;
      parentDictionary = new Dictionary<string, List<CommonStructureNodeInfo>>();
      parentDictionary.Add(structureNodeInfo1.Uri, new List<CommonStructureNodeInfo>());
      while (reader.Read())
      {
        CommonStructureNodeInfo structureNodeInfo2 = nodeInfoColumns.bind(reader);
        parentDictionary.Add(structureNodeInfo2.Uri, new List<CommonStructureNodeInfo>());
        parentDictionary[structureNodeInfo2.ParentUri].Add(structureNodeInfo2);
      }
    }

    internal List<CommonStructureNodeInfo> GetRootNodes(string projectId)
    {
      List<CommonStructureNodeInfo> rootNodes = new List<CommonStructureNodeInfo>();
      this.PrepareStoredProcedure("prc_css_get_root_nodes");
      this.BindString("@project_id", projectId, 128, false, SqlDbType.VarChar);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new ProjectDoesNotExistException(projectId);
      StructureComponent.NodeInfoColumns nodeInfoColumns = new StructureComponent.NodeInfoColumns();
      rootNodes.Add(nodeInfoColumns.bind(reader));
      while (reader.Read())
        rootNodes.Add(nodeInfoColumns.bind(reader));
      return rootNodes;
    }

    internal List<DeletedNodeInfo> GetDeletedNodes(string projectId, DateTime since)
    {
      List<DeletedNodeInfo> deletedNodes = new List<DeletedNodeInfo>();
      this.PrepareStoredProcedure("prc_css_get_deleted_nodes");
      this.BindString("@project_id", projectId, 128, false, SqlDbType.VarChar);
      this.BindDateTime("@deleted_since", since);
      SqlDataReader reader = this.ExecuteReader();
      StructureComponent.DeletedNodeInfoColumns deletedNodeInfoColumns = new StructureComponent.DeletedNodeInfoColumns();
      while (reader.Read())
        deletedNodes.Add(deletedNodeInfoColumns.bind(reader));
      return deletedNodes;
    }

    internal List<ChangedNodeInfo> GetChangedNodes(int start_sequence_id, out int end_sequence_id)
    {
      List<ChangedNodeInfo> changedNodes = new List<ChangedNodeInfo>();
      this.PrepareStoredProcedure("prc_css_get_changed_nodes");
      this.BindInt("@start_sequence_id", start_sequence_id);
      SqlDataReader reader = this.ExecuteReader();
      reader.Read();
      end_sequence_id = reader.GetInt32(0);
      reader.NextResult();
      StructureComponent.ChangedNodeInfoColumns changedNodeInfoColumns = new StructureComponent.ChangedNodeInfoColumns();
      while (reader.Read())
        changedNodes.Add(changedNodeInfoColumns.bind(reader));
      return changedNodes;
    }

    internal void GetChangedNodesAndProjects(
      int start_sequence_id,
      out int end_sequence_id,
      out List<ChangedNodeInfo> nodes,
      out List<ChangedProjectInfo> projects)
    {
      nodes = new List<ChangedNodeInfo>();
      projects = new List<ChangedProjectInfo>();
      this.PrepareStoredProcedure("prc_css_get_changed_nodes");
      this.BindInt("@start_sequence_id", start_sequence_id);
      this.BindBoolean("@include_deleted_projects", true);
      SqlDataReader reader = this.ExecuteReader();
      reader.Read();
      end_sequence_id = reader.GetInt32(0);
      reader.NextResult();
      StructureComponent.ChangedNodeInfoColumns changedNodeInfoColumns = new StructureComponent.ChangedNodeInfoColumns();
      while (reader.Read())
        nodes.Add(changedNodeInfoColumns.bind(reader));
      reader.NextResult();
      StructureComponent.ChangedProjectInfoColumns projectInfoColumns = new StructureComponent.ChangedProjectInfoColumns();
      while (reader.Read())
        projects.Add(projectInfoColumns.bind(reader));
    }

    internal virtual void CreateInitialNodes(
      TeamFoundationIdentity identity,
      Guid projectId,
      string nodes,
      DateTime timeStamp,
      out int nodeSeqId)
    {
      throw new NotImplementedException();
    }

    internal void CreateNode(
      TeamFoundationIdentity identity,
      string nodeId,
      string nodeName,
      string parentId,
      DateTime? startDate,
      DateTime? finishDate,
      DateTime timeStamp,
      out CommonStructureNodeInfo nodeInfo,
      out int nodeSeqId,
      out string securityToken)
    {
      this.PrepareStoredProcedure("prc_css_create_node");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      this.BindNodeName("@name", nodeName);
      this.BindString("@parent_id", parentId, 128, false, SqlDbType.VarChar);
      this.BindGuid("@user_id", identity.TeamFoundationId);
      if (startDate.HasValue && finishDate.HasValue)
      {
        this.BindDateTime("@start", startDate.Value);
        this.BindDateTime("@end", finishDate.Value);
      }
      else
      {
        this.BindNullValue("@start", SqlDbType.DateTime);
        this.BindNullValue("@end", SqlDbType.DateTime);
      }
      this.BindDateTime("@time_stamp", timeStamp);
      SqlDataReader reader = this.ExecuteReader();
      reader.Read();
      nodeInfo = new StructureComponent.NodeInfoColumns().bind(reader);
      reader.NextResult();
      reader.Read();
      new StructureComponent.SequenceIdColumns().bind(reader, out nodeSeqId);
      reader.NextResult();
      reader.Read();
      new StructureComponent.SecurityTokenColumns().bind(reader, out securityToken);
    }

    internal virtual void DeleteNodes(
      Guid projectId,
      string userName,
      DateTime timeStamp,
      out int nodeSeqId)
    {
      throw new NotImplementedException();
    }

    internal void DeleteBranch(
      string nodeId,
      string reclassifyId,
      string userName,
      DateTime timeStamp,
      out List<CommonStructureNodeInfo> deletedNodes,
      out int nodeSeqId)
    {
      deletedNodes = new List<CommonStructureNodeInfo>();
      this.PrepareStoredProcedure("prc_css_delete_node");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      this.BindString("@reclassify_id", reclassifyId, 128, false, SqlDbType.VarChar);
      this.BindString("@user_name", userName, 256, false, SqlDbType.NVarChar);
      this.BindDateTime("@time_stamp", timeStamp);
      StructureComponent.NodeInfoColumns nodeInfoColumns = new StructureComponent.NodeInfoColumns();
      SqlDataReader reader = this.ExecuteReader();
      while (reader.Read())
        deletedNodes.Add(nodeInfoColumns.bind(reader));
      reader.NextResult();
      reader.Read();
      new StructureComponent.SequenceIdColumns().bind(reader, out nodeSeqId);
    }

    internal void MoveBranch(
      string nodeId,
      string newParentId,
      DateTime timeStamp,
      out CommonStructureNodeInfo nodeInfo,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_move_node");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      this.BindString("@parent_id", newParentId, 128, false, SqlDbType.VarChar);
      this.BindDateTime("@time_stamp", timeStamp);
      SqlDataReader reader = this.ExecuteReader();
      reader.Read();
      nodeInfo = new StructureComponent.NodeInfoColumns().bind(reader);
      reader.NextResult();
      reader.Read();
      nodeSeqId = reader.GetInt32(0);
    }

    internal void RenameNode(
      string nodeId,
      string newName,
      DateTime timeStamp,
      out CommonStructureNodeInfo nodeInfo,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_rename_node");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      this.BindNodeName("@name", newName);
      this.BindDateTime("@time_stamp", timeStamp);
      SqlDataReader reader = this.ExecuteReader();
      reader.Read();
      nodeInfo = new StructureComponent.NodeInfoColumns().bind(reader);
      reader.NextResult();
      reader.Read();
      nodeSeqId = reader.GetInt32(0);
    }

    internal void SetIterationDates(
      string nodeId,
      DateTime? startDate,
      DateTime? finishDate,
      DateTime timeStamp,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_set_iteration_dates");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      if (startDate.HasValue)
      {
        this.BindDateTime("@start", startDate.Value);
        this.BindDateTime("@end", finishDate.Value);
      }
      else
      {
        this.BindNullValue("@start", SqlDbType.DateTime);
        this.BindNullValue("@end", SqlDbType.DateTime);
      }
      this.BindDateTime("@time_stamp", timeStamp);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      nodeSeqId = sqlDataReader.GetInt32(0);
    }

    internal void ReorderNode(string nodeId, int moveBy, DateTime timeStamp, out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_reorder_node");
      this.BindString("@id", nodeId, 128, false, SqlDbType.VarChar);
      this.BindInt("@move_by", moveBy);
      this.BindDateTime("@time_stamp", timeStamp);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      nodeSeqId = sqlDataReader.GetInt32(0);
    }

    protected string ConvertPcwXmlToDbXml(string projectName, XmlNode[] pcwXml)
    {
      CommonStructureNodeInfo parent = new CommonStructureNodeInfo();
      parent.Path = "\\" + DBPath.UserToDatabasePath(projectName, true);
      StringBuilder output = new StringBuilder();
      using (XmlWriter writer = XmlWriter.Create(output))
      {
        writer.WriteStartDocument();
        writer.WriteStartElement("Nodes");
        List<string> siblings = new List<string>();
        foreach (XmlElement node in pcwXml)
        {
          int count = 0;
          this.ConvertSubTree((XmlNode) node, parent, 0, ref count, siblings, writer);
        }
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
        return output.ToString();
      }
    }

    private void ConvertSubTree(
      XmlNode node,
      CommonStructureNodeInfo parent,
      int level,
      ref int count,
      List<string> siblings,
      XmlWriter writer)
    {
      if (node.NodeType != XmlNodeType.Element)
        return;
      if (node.Name != "Node" || node.Attributes["Name"] == null || node.Attributes["StructureType"] == null)
        throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INVALID_SCHEMA(), nameof (node));
      if (CommonStructureUtils.IsMaxDepthExceeded(level))
        throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_MAX_DEPTH_EXCEEDED(), nameof (node));
      string str = node.Attributes["Name"].Value.Trim();
      if (!CssUtils.IsValidCssNodeName(str))
        throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INVALID_NAME((object) str), nameof (node));
      CommonStructureNodeInfo parent1 = new CommonStructureNodeInfo();
      parent1.Name = DBPath.UserToDatabasePath(str).Replace('\v', '$');
      parent1.StructureType = node.Attributes["StructureType"].Value.Trim();
      foreach (string sibling in siblings)
      {
        if (TFStringComparer.CssNodeName.Equals(sibling, parent1.Name))
          throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_SIBLING_NAME_CONFLICT(), nameof (node));
      }
      siblings.Add(parent1.Name);
      if (CommonStructureUtils.IsInvalidStructureType(parent1.StructureType))
        throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INVALID_TYPE((object) parent1.StructureType), nameof (node));
      parent1.ParentUri = parent.Uri;
      if (parent.StructureType != null && !TFStringComparer.CssStructureType.Equals(parent1.StructureType, parent.StructureType))
        throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_PARENT_CHILD_MISMATCH(), nameof (node));
      parent1.Path = parent.Path + DBPath.UserToDatabasePath(parent1.Name, true);
      parent1.Uri = Guid.NewGuid().ToString();
      writer.WriteStartElement("Node");
      writer.WriteAttributeString("order", count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      ++count;
      writer.WriteAttributeString("id", parent1.Uri);
      if (parent1.ParentUri != null)
        writer.WriteAttributeString("parent_id", parent1.ParentUri);
      writer.WriteAttributeString("name", parent1.Name);
      writer.WriteAttributeString("path", parent1.Path);
      writer.WriteAttributeString("type", parent1.StructureType);
      writer.WriteEndElement();
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.NodeType == XmlNodeType.Element)
        {
          if (childNode1.Name == "Children")
          {
            List<string> siblings1 = new List<string>();
            foreach (XmlNode childNode2 in childNode1.ChildNodes)
              this.ConvertSubTree(childNode2, parent1, level + 1, ref count, siblings1, writer);
          }
          else if (!(childNode1.Name == "Properties"))
            throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_INVALID_SCHEMA(), nameof (node));
        }
      }
    }

    private SqlParameter BindNodeName(string parameterName, string nodeName) => this.BindString(parameterName, DBPath.UserToDatabasePath(nodeName), (int) byte.MaxValue, false, SqlDbType.NVarChar);

    private SqlParameter BindNodePath(string parameterName, string path) => this.BindString(parameterName, DBPath.UserToDatabasePath(path, true), 4000, false, SqlDbType.NVarChar);

    internal bool SupportsGetAllNodes => this is StructureComponent4;

    internal virtual List<CommonStructureNodeInfo> GetAllNodes() => throw new NotImplementedException();

    private class SequenceIdColumns
    {
      private SqlColumnBinder nSeqId = new SqlColumnBinder("node_sequence_id");

      internal void bind(SqlDataReader reader, out int nodeSeqId) => nodeSeqId = this.nSeqId.GetInt32((IDataReader) reader);
    }

    private class SecurityTokenColumns
    {
      private SqlColumnBinder securityTokenColumn = new SqlColumnBinder("SecurityToken");

      internal void bind(SqlDataReader reader, out string securityToken) => securityToken = this.securityTokenColumn.GetString((IDataReader) reader, false).TrimEnd(AuthorizationSecurityConstants.SeparatorChar);
    }

    protected class NodeInfoColumns
    {
      private SqlColumnBinder id = new SqlColumnBinder(nameof (id));
      private SqlColumnBinder parent_id = new SqlColumnBinder(nameof (parent_id));
      private SqlColumnBinder project_id = new SqlColumnBinder(nameof (project_id));
      private SqlColumnBinder name = new SqlColumnBinder(nameof (name));
      private SqlColumnBinder path = new SqlColumnBinder(nameof (path));
      private SqlColumnBinder type = new SqlColumnBinder("structure_type");
      private SqlColumnBinder start_date = new SqlColumnBinder(nameof (start_date));
      private SqlColumnBinder finish_date = new SqlColumnBinder(nameof (finish_date));

      internal CommonStructureNodeInfo bind(SqlDataReader reader)
      {
        CommonStructureNodeInfo structureNodeInfo = new CommonStructureNodeInfo();
        structureNodeInfo.Name = DBPath.DatabaseToUserPath(this.name.GetString((IDataReader) reader, false));
        structureNodeInfo.Uri = CommonStructureUtils.GetNodeUri(this.id.GetGuid((IDataReader) reader, false));
        structureNodeInfo.ProjectUri = CommonStructureUtils.GetProjectUri(this.project_id.GetGuid((IDataReader) reader, false));
        structureNodeInfo.Path = DBPath.DatabaseToUserPath(this.path.GetString((IDataReader) reader, false), true);
        structureNodeInfo.StructureType = this.type.GetString((IDataReader) reader, false);
        Guid guid = this.parent_id.GetGuid((IDataReader) reader, true);
        if (guid != Guid.Empty)
          structureNodeInfo.ParentUri = CommonStructureUtils.GetNodeUri(guid);
        structureNodeInfo.Properties = Array.Empty<CommonStructureProperty>();
        if (!this.start_date.IsNull((IDataReader) reader))
        {
          structureNodeInfo.StartDate = new DateTime?(this.start_date.GetDateTime((IDataReader) reader));
          structureNodeInfo.FinishDate = new DateTime?(this.finish_date.GetDateTime((IDataReader) reader));
        }
        return structureNodeInfo;
      }
    }

    private class DeletedNodeInfoColumns
    {
      private SqlColumnBinder id = new SqlColumnBinder("node_id");
      private SqlColumnBinder reclassify_id = new SqlColumnBinder(nameof (reclassify_id));
      private SqlColumnBinder user = new SqlColumnBinder("deleted_user");
      private SqlColumnBinder time = new SqlColumnBinder("deleted_time");

      internal DeletedNodeInfo bind(SqlDataReader reader) => new DeletedNodeInfo()
      {
        Uri = CommonStructureUtils.GetNodeUri(this.id.GetGuid((IDataReader) reader, false)),
        ReclassifyUri = CommonStructureUtils.GetNodeUri(this.reclassify_id.GetGuid((IDataReader) reader, false)),
        User = this.user.GetString((IDataReader) reader, false),
        Time = this.time.GetDateTime((IDataReader) reader)
      };
    }

    private class ChangedNodeInfoColumns
    {
      private SqlColumnBinder deleted = new SqlColumnBinder(nameof (deleted));
      private SqlColumnBinder id = new SqlColumnBinder(nameof (id));
      private SqlColumnBinder project_id = new SqlColumnBinder(nameof (project_id));
      private SqlColumnBinder name = new SqlColumnBinder(nameof (name));
      private SqlColumnBinder type = new SqlColumnBinder("structure_type");
      private SqlColumnBinder parent_id = new SqlColumnBinder(nameof (parent_id));
      private SqlColumnBinder reclassify_id = new SqlColumnBinder(nameof (reclassify_id));
      private SqlColumnBinder start_date = new SqlColumnBinder(nameof (start_date));
      private SqlColumnBinder finish_date = new SqlColumnBinder(nameof (finish_date));

      internal ChangedNodeInfo bind(SqlDataReader reader)
      {
        ChangedNodeInfo changedNodeInfo = new ChangedNodeInfo();
        changedNodeInfo.Uri = CommonStructureUtils.GetNodeUri(this.id.GetGuid((IDataReader) reader, false));
        changedNodeInfo.Deleted = this.deleted.GetBoolean((IDataReader) reader, false);
        changedNodeInfo.ProjectUri = CommonStructureUtils.GetProjectUri(this.project_id.GetGuid((IDataReader) reader, false));
        if (!this.start_date.IsNull((IDataReader) reader))
        {
          changedNodeInfo.StartDate = new DateTime?(this.start_date.GetDateTime((IDataReader) reader));
          changedNodeInfo.FinishDate = new DateTime?(this.finish_date.GetDateTime((IDataReader) reader));
        }
        if (changedNodeInfo.Deleted)
        {
          changedNodeInfo.ReclassifyUri = CommonStructureUtils.GetNodeUri(this.reclassify_id.GetGuid((IDataReader) reader, false));
        }
        else
        {
          Guid guid = this.parent_id.GetGuid((IDataReader) reader, true);
          if (guid != Guid.Empty)
            changedNodeInfo.ParentUri = CommonStructureUtils.GetNodeUri(guid);
          changedNodeInfo.Name = DBPath.DatabaseToUserPath(this.name.GetString((IDataReader) reader, false));
          changedNodeInfo.Type = this.type.GetString((IDataReader) reader, false);
        }
        return changedNodeInfo;
      }
    }

    private class ChangedProjectInfoColumns
    {
      private SqlColumnBinder project_id = new SqlColumnBinder(nameof (project_id));
      private SqlColumnBinder deleted = new SqlColumnBinder(nameof (deleted));

      internal ChangedProjectInfo bind(SqlDataReader reader) => new ChangedProjectInfo()
      {
        ProjectUri = CommonStructureUtils.GetProjectUri(this.project_id.GetGuid((IDataReader) reader, false)),
        Deleted = this.deleted.GetBoolean((IDataReader) reader, false)
      };
    }
  }
}
