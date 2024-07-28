// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.WorkspaceMapping2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("WorkspaceMapping")]
  public class WorkspaceMapping2010 : IValidatable
  {
    private int m_workspaceId;
    private string m_serverItem;
    private string m_localItem;
    private WorkspaceMappingType2010 m_mappingType;
    private int m_depth = 120;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string ServerItem
    {
      get => this.m_serverItem;
      set => this.m_serverItem = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string LocalItem
    {
      get => this.m_localItem;
      set => this.m_localItem = value;
    }

    [XmlAttribute]
    [DefaultValue(WorkspaceMappingType2010.Map)]
    [ClientProperty(ClientVisibility.Public)]
    public WorkspaceMappingType2010 MappingType
    {
      get => this.m_mappingType;
      set => this.m_mappingType = value;
    }

    [XmlAttribute]
    [DefaultValue(120)]
    [ClientProperty(ClientVisibility.Public)]
    public int Depth
    {
      get => this.m_depth;
      set => this.m_depth = value;
    }

    internal int WorkspaceId
    {
      get => this.m_workspaceId;
      set => this.m_workspaceId = value;
    }

    internal Guid ProjectId { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      this.m_serverItem = this.m_serverItem != null ? VersionControlPath.GetFullPath(this.m_serverItem) : throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "ServerItem"));
      if (VersionControlPath.IsWildcard(this.m_serverItem))
        throw new ArgumentException(ResourceStrings.InvalidWorkspaceMappingCannotContainWildcards((object) this.m_serverItem));
      if (this.m_depth != 1 && this.m_depth != 120)
        throw new ArgumentException(ResourceStrings.InvalidWorkspaceMappingDepth((object) this.m_depth));
      if (this.m_mappingType == WorkspaceMappingType2010.Cloak)
      {
        this.m_localItem = (string) null;
      }
      else
      {
        try
        {
          ArgumentValidation.CheckBuildDirectory("LocalItem", ref this.m_localItem, false);
        }
        catch (ArgumentException ex)
        {
          throw new ArgumentException(ResourceStrings.InvalidLocalItem((object) this.m_localItem, (object) ex.Message), (Exception) ex);
        }
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[WorkspaceMapping2010 ServerItem={0} LocalItem={1} MappingType={2} Depth={3}]", (object) this.ServerItem, (object) this.LocalItem, (object) this.MappingType, (object) this.Depth);
  }
}
