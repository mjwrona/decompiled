// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.WorkspaceTemplate
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [CallOnDeserialization("AfterDeserialize")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class WorkspaceTemplate : IValidatable
  {
    private List<WorkspaceMapping> m_mappings = new List<WorkspaceMapping>();

    public WorkspaceTemplate()
    {
    }

    internal WorkspaceTemplate(List<WorkspaceMapping> mappings) => this.m_mappings = mappings;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string DefinitionUri { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalMappings")]
    public List<WorkspaceMapping> Mappings => this.m_mappings;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime LastModifiedDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LastModifiedBy { get; set; }

    internal int WorkspaceId { get; set; }

    internal Guid ProjectId { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.Check("Mappings", (object) this.m_mappings, false);
      ArgumentValidation.CheckUri("DefinitionUri", this.DefinitionUri, "Definition", true, (string) null);
      if (this.m_mappings.Count == 0)
        throw new ArgumentException(ResourceStrings.WorkspaceTemplateMustDefineMapping());
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.FilePath);
      Dictionary<string, object> dictionary2 = new Dictionary<string, object>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
      for (int index = 0; index < this.m_mappings.Count; ++index)
      {
        Validation.CheckValidatable(requestContext, string.Empty, (IValidatable) this.m_mappings[index], false, context);
        if (dictionary2.ContainsKey(this.m_mappings[index].ServerItem))
          throw new ArgumentException(ResourceStrings.DuplicateWorkspaceMappingDefinition((object) this.m_mappings[index].ServerItem));
        if (!string.IsNullOrEmpty(this.m_mappings[index].LocalItem))
        {
          if (dictionary1.ContainsKey(this.m_mappings[index].LocalItem))
            throw new ArgumentException(ResourceStrings.DuplicateWorkspaceMappingDefinition((object) this.m_mappings[index].LocalItem));
          dictionary1.Add(this.m_mappings[index].LocalItem, (object) null);
        }
        dictionary2.Add(this.m_mappings[index].ServerItem, (object) null);
      }
    }

    internal static WorkspaceTemplate LoadFromFile(string filePath)
    {
      List<WorkspaceMapping> mappingsFromFile = WorkspaceTemplate.GetMappingsFromFile(filePath);
      string serverParentPath = WorkspaceTemplate.GetCommonServerParentPath(mappingsFromFile);
      foreach (WorkspaceMapping workspaceMapping in mappingsFromFile)
      {
        if (workspaceMapping.MappingType == WorkspaceMappingType.Map)
        {
          string str = workspaceMapping.ServerItem.Replace(serverParentPath, BuildCommonUtil.SourceDirEnvironmentVariable);
          workspaceMapping.LocalItem = str.Replace('/', Path.DirectorySeparatorChar);
        }
      }
      return new WorkspaceTemplate(mappingsFromFile);
    }

    private static List<WorkspaceMapping> GetMappingsFromFile(string filePath)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      XmlReader xmlReader = XmlReader.Create(filePath, settings);
      List<WorkspaceMapping> mappingsFromFile = new List<WorkspaceMapping>();
      try
      {
        while (xmlReader.Read())
        {
          if (VssStringComparer.XmlNodeName.Equals(xmlReader.LocalName, "InternalMapping"))
          {
            WorkspaceMapping workspaceMapping = new WorkspaceMapping();
            while (xmlReader.MoveToNextAttribute())
            {
              switch (xmlReader.Name)
              {
                case "ServerItem":
                  workspaceMapping.ServerItem = VersionControlPath.GetFullPath(xmlReader.Value);
                  continue;
                case "LocalItem":
                  workspaceMapping.LocalItem = xmlReader.Value;
                  continue;
                case "Type":
                  workspaceMapping.MappingType = (WorkingFolderType) Enum.Parse(typeof (WorkingFolderType), xmlReader.Value) != WorkingFolderType.Map ? WorkspaceMappingType.Cloak : WorkspaceMappingType.Map;
                  continue;
                default:
                  continue;
              }
            }
            if (workspaceMapping.MappingType == WorkspaceMappingType.Cloak)
              workspaceMapping.LocalItem = (string) null;
            if (!string.IsNullOrEmpty(workspaceMapping.ServerItem))
              mappingsFromFile.Add(workspaceMapping);
          }
        }
      }
      finally
      {
        xmlReader.Close();
      }
      return mappingsFromFile;
    }

    internal static string GetCommonServerParentPath(List<WorkspaceMapping> mappings)
    {
      if (mappings == null || mappings.Count == 0)
        return "$/";
      string path1 = mappings[0].ServerItem;
      for (int index = 1; index < mappings.Count; ++index)
        path1 = WorkspaceTemplate.GetCommonServerParentPath(path1, mappings[index].ServerItem);
      return path1;
    }

    private static string GetCommonServerParentPath(string path1, string path2)
    {
      if (!VersionControlPath.IsServerItem(path1) || !VersionControlPath.IsServerItem(path2))
        return "$/";
      string parent;
      string str;
      if (VersionControlPath.GetFolderDepth(path1) >= VersionControlPath.GetFolderDepth(path2))
      {
        parent = path2;
        str = path1;
      }
      else
      {
        parent = path1;
        str = path2;
      }
      while (!VersionControlPath.IsSubItem(str, parent))
        parent = VersionControlPath.GetFolderName(parent);
      return parent;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[WorkspaceTemplate WorkspaceId={0} DefinitionUri={1} Mappings={2}]", (object) this.WorkspaceId, (object) this.DefinitionUri, (object) this.Mappings.ListItems<WorkspaceMapping>());
  }
}
