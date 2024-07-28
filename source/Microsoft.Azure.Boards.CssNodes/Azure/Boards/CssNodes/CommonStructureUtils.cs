// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.CommonStructureUtils
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.Boards.CssNodes
{
  public sealed class CommonStructureUtils
  {
    private const string CSSURICONST = "Classification";
    private const int CSSMAXDEPTH = 14;
    public const string PathSeparator = "\\";
    private static readonly char[] s_illegalCharactersForNodeCreation = new char[1]
    {
      '+'
    };

    private static string GetNodeOrProjectUri(Guid id, bool projectId) => CommonStructureUtils.GetNodeOrProjectUri(id.ToString(), projectId);

    private static string GetNodeOrProjectUri(string id, bool projectId)
    {
      string empty = string.Empty;
      string str = !projectId ? "Node" : "TeamProject";
      return LinkingUtilities.EncodeUri(new ArtifactId()
      {
        Tool = "Classification",
        ArtifactType = str,
        ToolSpecificId = id
      });
    }

    public static bool TryParseUri(string uri, out Guid id, bool projectId)
    {
      string empty = string.Empty;
      id = Guid.Empty;
      ArtifactId artifactId = LinkingUtilities.DecodeUri(uri?.Trim());
      string y = !projectId ? "Node" : "TeamProject";
      return StringComparer.OrdinalIgnoreCase.Equals(artifactId.Tool, "Classification") && StringComparer.OrdinalIgnoreCase.Equals(artifactId.ArtifactType, y) && Guid.TryParse(artifactId.ToolSpecificId, out id);
    }

    public static string GetNodeUri(Guid id) => CommonStructureUtils.GetNodeOrProjectUri(id, false);

    public static string GetNodeUri(string id) => CommonStructureUtils.GetNodeOrProjectUri(id, false);

    public static bool TryParseNodeUri(string uri, out Guid id) => CommonStructureUtils.TryParseUri(uri, out id, false);

    public static string GetProjectUri(Guid id) => CommonStructureUtils.GetNodeOrProjectUri(id, true);

    public static string GetProjectUri(string id) => CommonStructureUtils.GetNodeOrProjectUri(id, true);

    public static bool TryParseProjectUri(string uri, out Guid id) => CommonStructureUtils.TryParseUri(uri, out id, true);

    public static bool IsInvalidStructureType(string type) => type != "ProjectLifecycle" && type != "ProjectModelHierarchy";

    public static bool IsMaxDepthExceeded(int level) => level >= 14;

    private static string NormalizeUri(string uri, string parameterName)
    {
      uri = uri != null ? uri.Trim() : throw new ArgumentNullException(parameterName);
      return !string.IsNullOrEmpty(uri) ? uri : throw new ArgumentException(ServerResources.CSS_EMPTY_ARGUMENT((object) parameterName), parameterName);
    }

    public static string ExtractProjectId(ref string projectUri, string parameterName) => CommonStructureUtils.GetProjectIdFromUri(projectUri, parameterName).ToString();

    public static Guid GetProjectIdFromUri(string projectUri, string parameterName = "ProjectUri")
    {
      CommonStructureUtils.NormalizeUri(projectUri, parameterName);
      Guid id;
      if (!CommonStructureUtils.TryParseProjectUri(projectUri, out id))
        throw new ArgumentException(ServerResources.CSS_INVALID_URI((object) parameterName, (object) projectUri));
      return id;
    }

    public static string ExtractNodeId(ref string nodeUri, string parameterName)
    {
      nodeUri = CommonStructureUtils.NormalizeUri(nodeUri, parameterName);
      Guid id;
      if (!CommonStructureUtils.TryParseNodeUri(nodeUri, out id))
        throw new ArgumentException(ServerResources.CSS_INVALID_URI((object) parameterName, (object) nodeUri));
      return id.ToString();
    }

    public static string[] ExtractNodeIds(ref string[] nodeUris, string parameterName)
    {
      CommonStructureUtils.CheckNullOrEmptyArrayArgument((object[]) nodeUris, parameterName);
      string[] nodeIds = new string[nodeUris.Length];
      for (int index = 0; index < nodeUris.Length; ++index)
      {
        try
        {
          nodeIds[index] = CommonStructureUtils.ExtractNodeId(ref nodeUris[index], parameterName);
        }
        catch
        {
          throw new ArgumentException(ServerResources.CSS_INVALID_ARRAY((object) parameterName), parameterName);
        }
      }
      return nodeIds;
    }

    public static string NormalizeNodeName(
      string nodeName,
      string parameterName,
      bool checkForIllegalCharactersForNodeCreation = true)
    {
      nodeName = nodeName != null ? nodeName.Trim() : throw new ArgumentNullException(parameterName);
      if (string.IsNullOrEmpty(nodeName))
        throw new ArgumentException(ServerResources.CSS_EMPTY_ARGUMENT((object) parameterName), parameterName);
      if (!CssUtils.IsValidCssNodeName(nodeName))
        throw new ArgumentException(ServerResources.CSS_INVALID_NAME((object) nodeName), parameterName);
      if (checkForIllegalCharactersForNodeCreation && -1 != nodeName.IndexOfAny(CommonStructureUtils.s_illegalCharactersForNodeCreation))
        throw new ArgumentException(ServerResources.CSS_INVALID_NAME((object) nodeName), parameterName);
      return nodeName;
    }

    public static string NormalizeNodePath(string nodePath, string parameterName)
    {
      nodePath = nodePath != null ? nodePath.Trim().TrimEnd("/\\".ToCharArray()) : throw new ArgumentNullException(nameof (nodePath));
      if (string.IsNullOrEmpty(nodePath))
        throw new ArgumentException(ServerResources.CSS_EMPTY_ARGUMENT((object) nameof (nodePath)), nameof (nodePath));
      if (!nodePath.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
        nodePath = "\\" + nodePath;
      string[] strArray = nodePath.Substring(1).Split("\\".ToCharArray());
      if (strArray.Length < 2)
        throw new ArgumentException(ServerResources.CSS_INVALID_PATH((object) nodePath), nameof (nodePath));
      foreach (string nodeName in strArray)
      {
        try
        {
          CommonStructureUtils.NormalizeNodeName(nodeName, nameof (nodePath), false);
        }
        catch (ArgumentException ex)
        {
          throw new ArgumentException(ServerResources.CSS_INVALID_PATH((object) nodePath), nameof (nodePath));
        }
      }
      return nodePath;
    }

    public static XmlNode[] ParseNodesStructure(XmlElement structure, string parameterName)
    {
      if (structure == null)
        throw new ArgumentNullException(parameterName);
      if (structure.Name != "Nodes")
        throw new ArgumentException(ServerResources.CSS_INVALID_SCHEMA(), parameterName);
      List<XmlNode> xmlNodeList = new List<XmlNode>();
      foreach (XmlNode xmlNode in (XmlNode) structure)
      {
        if (xmlNode.NodeType == XmlNodeType.Element)
          xmlNodeList.Add(xmlNode);
      }
      XmlNode xmlNode1 = xmlNodeList.Count == 2 ? xmlNodeList[0] : throw new ArgumentException(ServerResources.CSS_INVALID_ROOT_NODE_COUNT(), parameterName);
      XmlNode xmlNode2 = xmlNodeList[1];
      if (xmlNode1.Name != "Node" || xmlNode2.Name != "Node" || xmlNode1.Attributes["StructureType"] == null || xmlNode2.Attributes["StructureType"] == null || xmlNode1.Attributes["Name"] == null || xmlNode2.Attributes["Name"] == null)
        throw new ArgumentException(ServerResources.CSS_INVALID_SCHEMA(), parameterName);
      string str1 = xmlNode1.Attributes["StructureType"].Value.Trim();
      string str2 = xmlNode2.Attributes["StructureType"].Value.Trim();
      string str3 = xmlNode1.Attributes["Name"].Value.Trim();
      string str4 = xmlNode2.Attributes["Name"].Value.Trim();
      if (TFStringComparer.CssNodeName.Equals(str3, str4))
        throw new ArgumentException(ServerResources.CSS_INVALID_ROOT_NODES_SAME_NAME(), parameterName);
      if (!CssUtils.IsValidCssNodeName(str3))
        throw new ArgumentException(ServerResources.CSS_INVALID_NAME((object) str3), parameterName);
      if (!CssUtils.IsValidCssNodeName(str4))
        throw new ArgumentException(ServerResources.CSS_INVALID_NAME((object) str4), parameterName);
      if (TFStringComparer.CssStructureType.Equals(str1, str2))
        throw new ArgumentException(ServerResources.CSS_INVALID_ROOT_NODES_SAME_TYPE(), parameterName);
      if (CommonStructureUtils.IsInvalidStructureType(str1))
        throw new ArgumentException(ServerResources.CSS_INVALID_TYPE((object) str1), parameterName);
      if (CommonStructureUtils.IsInvalidStructureType(str2))
        throw new ArgumentException(ServerResources.CSS_INVALID_TYPE((object) str2), parameterName);
      return xmlNodeList.ToArray();
    }

    public static List<CommonStructureNode> ConvertPcwXmlToNodes(
      string projectName,
      XmlNode[] pcwXml)
    {
      CommonStructureNode commonStructureNode = new CommonStructureNode();
      commonStructureNode.Path = "\\" + DBPath.UserToDatabasePath(projectName, true);
      List<CommonStructureNode> nodes = new List<CommonStructureNode>();
      List<string> stringList = new List<string>();
      foreach (XmlElement node in pcwXml)
      {
        int num = 0;
        CommonStructureNode parent = commonStructureNode;
        ref int local = ref num;
        List<string> siblings = stringList;
        List<CommonStructureNode> allNodes = nodes;
        CommonStructureUtils.ConvertSubTree((XmlNode) node, parent, 0, ref local, siblings, (XmlWriter) null, allNodes);
      }
      return nodes;
    }

    public static string ConvertPcwXmlToDbXml(string projectName, XmlNode[] pcwXml)
    {
      CommonStructureNode commonStructureNode = new CommonStructureNode();
      commonStructureNode.Path = "\\" + DBPath.UserToDatabasePath(projectName, true);
      StringBuilder output = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(output))
      {
        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("Nodes");
        List<string> stringList = new List<string>();
        foreach (XmlElement node in pcwXml)
        {
          int num = 0;
          CommonStructureNode parent = commonStructureNode;
          ref int local = ref num;
          List<string> siblings = stringList;
          XmlWriter writer = xmlWriter;
          CommonStructureUtils.ConvertSubTree((XmlNode) node, parent, 0, ref local, siblings, writer, (List<CommonStructureNode>) null);
        }
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
        return output.ToString();
      }
    }

    private static void ConvertSubTree(
      XmlNode node,
      CommonStructureNode parent,
      int level,
      ref int count,
      List<string> siblings,
      XmlWriter writer,
      List<CommonStructureNode> allNodes)
    {
      if (node.NodeType != XmlNodeType.Element)
        return;
      if (node.Name != "Node" || node.Attributes["Name"] == null || node.Attributes["StructureType"] == null)
        throw new ArgumentException(ServerResources.CSS_INVALID_SCHEMA(), nameof (node));
      if (CommonStructureUtils.IsMaxDepthExceeded(level))
        throw new ArgumentException(ServerResources.CSS_MAX_DEPTH_EXCEEDED(), nameof (node));
      string name = node.Attributes["Name"].Value.Trim();
      if (!CssUtils.IsValidCssNodeName(name))
        throw new ArgumentException(ServerResources.CSS_INVALID_NAME((object) name), nameof (node));
      CommonStructureNode parent1 = new CommonStructureNode();
      parent1.Name = name;
      string userPath = DBPath.UserToDatabasePath(parent1.Name).Replace('\v', '$');
      foreach (string sibling in siblings)
      {
        if (TFStringComparer.CssNodeName.Equals(sibling, parent1.Name))
          throw new ArgumentException(ServerResources.CSS_SIBLING_NAME_CONFLICT(), nameof (node));
      }
      siblings.Add(parent1.Name);
      string type = node.Attributes["StructureType"].Value.Trim();
      if (CommonStructureUtils.IsInvalidStructureType(type))
        throw new ArgumentException(ServerResources.CSS_INVALID_TYPE((object) type), nameof (node));
      parent1.Type = type == "ProjectModelHierarchy" ? CommonStructureNodeType.Area : CommonStructureNodeType.Iteration;
      parent1.ParentId = parent.Id;
      if (parent.Type != CommonStructureNodeType.None && parent1.Type != parent.Type)
        throw new ArgumentException(ServerResources.CSS_PARENT_CHILD_MISMATCH(), nameof (node));
      parent1.Path = parent.Path + DBPath.UserToDatabasePath(userPath, true);
      parent1.Id = Guid.NewGuid();
      allNodes?.Add(parent1);
      if (writer != null)
      {
        writer.WriteStartElement("Node");
        writer.WriteAttributeString("order", count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        ++count;
        writer.WriteAttributeString("id", parent1.Id.ToString("D"));
        if (parent1.ParentId != Guid.Empty)
          writer.WriteAttributeString("parent_id", parent1.ParentId.ToString("D"));
        writer.WriteAttributeString("name", userPath);
        writer.WriteAttributeString("path", parent1.Path);
        writer.WriteAttributeString("type", type);
        writer.WriteEndElement();
      }
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.NodeType == XmlNodeType.Element)
        {
          if (childNode1.Name == "Children")
          {
            List<string> siblings1 = new List<string>();
            foreach (XmlNode childNode2 in childNode1.ChildNodes)
              CommonStructureUtils.ConvertSubTree(childNode2, parent1, level + 1, ref count, siblings1, writer, allNodes);
          }
          else if (!(childNode1.Name == "Properties"))
            throw new ArgumentException(ServerResources.CSS_INVALID_SCHEMA(), nameof (node));
        }
      }
    }

    public static CommonStructureProjectState ParseProjectState(string state, string parameterName)
    {
      state = state != null ? state.Trim() : throw new ArgumentNullException(parameterName);
      if (string.IsNullOrEmpty(state))
        throw new ArgumentException(ServerResources.CSS_EMPTY_ARGUMENT((object) parameterName), parameterName);
      try
      {
        return (CommonStructureProjectState) Enum.Parse(typeof (CommonStructureProjectState), state);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException(ServerResources.CSS_INVALID_STATE((object) state), (Exception) ex);
      }
    }

    public static string CheckAndNormalizeUri(
      string uri,
      string parameterName,
      bool isProjectUri,
      out Guid id)
    {
      uri = CommonStructureUtils.NormalizeUri(uri, parameterName);
      return CommonStructureUtils.TryParseUri(uri, out id, isProjectUri) ? uri : throw new ArgumentException(ServerResources.CSS_INVALID_URI((object) parameterName, (object) uri));
    }

    public static void CheckNullOrEmptyArrayArgument(object[] argument, string parameterName)
    {
      if (argument == null)
        throw new ArgumentNullException(parameterName);
      if (argument.Length == 0)
        throw new ArgumentException(ServerResources.CSS_INVALID_ARRAY((object) parameterName), parameterName);
    }

    public static void ValidateIterationDates(DateTime? startDate, DateTime? finishDate)
    {
      if (!startDate.HasValue && !finishDate.HasValue)
        return;
      if (startDate.HasValue && !finishDate.HasValue || !startDate.HasValue && finishDate.HasValue)
        throw new ArgumentException(ServerResources.CSS_MUST_SPECIFY_BOTH_DATES());
      DateTime? nullable1 = startDate;
      DateTime? nullable2 = finishDate;
      if ((nullable1.HasValue & nullable2.HasValue ? (nullable1.GetValueOrDefault() > nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        throw new ArgumentException(ServerResources.CSS_START_AFTER_FINISH());
      nullable2 = startDate;
      DateTime dateTime1 = SqlDateTime.MinValue.Value;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() < dateTime1 ? 1 : 0) : 0) != 0)
        throw new ArgumentException(ServerResources.CSS_DATE_RANGE_INVALID());
      nullable2 = finishDate;
      DateTime dateTime2 = SqlDateTime.MinValue.Value;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() < dateTime2 ? 1 : 0) : 0) != 0)
        throw new ArgumentException(ServerResources.CSS_DATE_RANGE_INVALID());
    }

    public static IDictionary<string, ProjectProperty> NormalizeAndToDictionary(
      IEnumerable<ProjectProperty> projectProperties)
    {
      IList<ProjectProperty> list = (IList<ProjectProperty>) projectProperties.ToList<ProjectProperty>();
      ProjectProperty.NormalizeProperties(ref list, "projectPropertyList");
      return (IDictionary<string, ProjectProperty>) list.ToDictionary<ProjectProperty, string>((Func<ProjectProperty, string>) (p => p.Name), (IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName);
    }

    public static IEnumerable<ProjectProperty> GetPropertyChanges(
      IDictionary<string, ProjectProperty> newProperties,
      IDictionary<string, ProjectProperty> currentProperties)
    {
      List<ProjectProperty> propertyChanges = new List<ProjectProperty>();
      foreach (ProjectProperty projectProperty1 in (IEnumerable<ProjectProperty>) newProperties.Values)
      {
        ProjectProperty projectProperty2;
        if (currentProperties.TryGetValue(projectProperty1.Name, out projectProperty2))
        {
          if (!string.Equals((string) projectProperty1.Value, (string) projectProperty2.Value, StringComparison.Ordinal))
            propertyChanges.Add(projectProperty1);
        }
        else
          propertyChanges.Add(projectProperty1);
      }
      foreach (ProjectProperty projectProperty in (IEnumerable<ProjectProperty>) currentProperties.Values)
      {
        if (!newProperties.TryGetValue(projectProperty.Name, out ProjectProperty _))
        {
          projectProperty.Value = (object) null;
          propertyChanges.Add(projectProperty);
        }
      }
      return (IEnumerable<ProjectProperty>) propertyChanges;
    }
  }
}
