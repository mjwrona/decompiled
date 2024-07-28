// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildAgentUpdateOptions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildAgentUpdateOptions : IValidatable, IUpdatePropertyProvider
  {
    private List<PropertyValue> m_attachedProperties = new List<PropertyValue>();
    private List<string> m_tags = new List<string>();

    public BuildAgentUpdateOptions() => this.Fields = BuildAgentUpdate.None;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildAgentUpdate.None)]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public BuildAgentUpdate Fields { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    public string ControllerUri { get; set; }

    [XmlAttribute]
    public string BuildDirectory { get; set; }

    [XmlAttribute]
    public AgentStatus Status { get; set; }

    [XmlAttribute]
    public bool Enabled { get; set; }

    [XmlAttribute]
    public string StatusMessage { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public List<PropertyValue> AttachedProperties
    {
      get => this.m_attachedProperties;
      set => this.m_attachedProperties = value;
    }

    public List<string> Tags => this.m_tags;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("Uri", this.Uri, "Agent", false, (string) null);
      if ((this.Fields & BuildAgentUpdate.BuildDirectory) == BuildAgentUpdate.BuildDirectory)
      {
        string buildDirectory = this.BuildDirectory;
        ArgumentValidation.CheckBuildDirectory("BuildDirectory", ref buildDirectory, false);
        this.BuildDirectory = buildDirectory;
      }
      if ((this.Fields & BuildAgentUpdate.ControllerUri) == BuildAgentUpdate.ControllerUri)
        ArgumentValidation.CheckUri("ControllerUri", this.ControllerUri, "Controller", true, ResourceStrings.MissingController());
      if ((this.Fields & BuildAgentUpdate.Description) == BuildAgentUpdate.Description)
        Validation.CheckDescription("Description", this.Description, true);
      if ((this.Fields & BuildAgentUpdate.Name) == BuildAgentUpdate.Name)
        Microsoft.TeamFoundation.Build.Common.Validation.CheckValidAgentName(this.Name, false);
      if ((this.Fields & BuildAgentUpdate.Tags) != BuildAgentUpdate.Tags)
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>("Tags", (IList<string>) this.m_tags, BuildAgentUpdateOptions.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag ?? (BuildAgentUpdateOptions.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag = new Validate<string>(Validation.CheckBuildAgentTag)), false, (string) null);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgentUpdate Uri={0} Name={1} ControllerUri={2} Fields={3}]", (object) this.Uri, (object) this.Name, (object) this.ControllerUri, (object) this.Fields);
  }
}
