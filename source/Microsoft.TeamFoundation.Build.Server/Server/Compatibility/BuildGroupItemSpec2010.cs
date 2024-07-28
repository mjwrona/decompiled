// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildGroupItemSpec2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlInclude(typeof (BuildAgentSpec2008))]
  [XmlInclude(typeof (BuildDefinitionSpec2010))]
  [XmlType("BuildGroupItemSpec")]
  public abstract class BuildGroupItemSpec2010 : IValidatable
  {
    private string m_fullPath;
    private string m_teamProject;
    private string m_groupPath;
    private string m_itemPattern;

    protected BuildGroupItemSpec2010()
    {
    }

    internal BuildGroupItemSpec2010(string fullPath) => this.m_fullPath = fullPath;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string FullPath
    {
      get => this.m_fullPath;
      set => this.m_fullPath = value;
    }

    internal string TeamProject => this.m_teamProject;

    internal string GroupPath => this.m_groupPath;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context) => this.Validate(requestContext, context);

    internal void Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckItemPath("FullPath", ref this.m_fullPath, false, true);
      if (BuildPath.GetItemDepth(this.m_fullPath) < 2)
        throw new ArgumentException(ResourceStrings.BuildGroupItemNameRequired((object) this.m_fullPath));
      BuildPath.SplitTeamProject(this.m_fullPath, out this.m_teamProject, out this.m_groupPath);
      this.m_itemPattern = BuildPath.GetItemName(this.m_fullPath);
    }

    internal bool Match(BuildGroupItem2010 item) => item != null && FileSpec.Match(item.Name, this.m_itemPattern);
  }
}
