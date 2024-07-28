// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionSpec
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnSerialization("BeforeSerialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildDefinitionSpec : IPropertyProviderSpec, IValidatable
  {
    private string m_path;
    private string m_fullPath;
    private string m_itemPattern;
    private string m_teamProject;
    private List<string> m_propertyNameFilters = new List<string>();

    public BuildDefinitionSpec()
    {
      this.Options = QueryOptions.All;
      this.TriggerType = DefinitionTriggerType.All;
    }

    internal BuildDefinitionSpec(string fullPath)
    {
      this.m_fullPath = fullPath;
      this.Options = QueryOptions.All;
      this.TriggerType = DefinitionTriggerType.All;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string FullPath
    {
      get => this.m_fullPath;
      set => this.m_fullPath = value;
    }

    [DefaultValue(DefinitionTriggerType.All)]
    [ClientProperty(ClientVisibility.Public)]
    public DefinitionTriggerType TriggerType { get; set; }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public List<string> PropertyNameFilters => this.m_propertyNameFilters;

    [DefaultValue(QueryOptions.All)]
    [ClientProperty(ClientVisibility.Public)]
    public QueryOptions Options { get; set; }

    internal string Path => this.m_path;

    internal string TeamProject => this.m_teamProject;

    internal bool Match(BuildDefinition definition) => definition != null && FileSpec.Match(definition.Name, this.m_itemPattern);

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckItemPath("FullPath", ref this.m_fullPath, false, true);
      if (BuildPath.GetItemDepth(this.m_fullPath) < 2)
        throw new ArgumentException(ResourceStrings.BuildGroupItemNameRequired((object) this.m_fullPath));
      BuildPath.SplitTeamProject(this.m_fullPath, out this.m_teamProject, out this.m_path);
      this.m_itemPattern = BuildPath.GetItemName(this.m_fullPath);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDefinitionSpec FullPath={0} Options={1}]", (object) this.FullPath, (object) this.Options);
  }
}
