// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAgentUpdateOptions2010
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

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildAgentUpdateOptions")]
  public sealed class BuildAgentUpdateOptions2010 : IValidatable
  {
    private List<string> m_tags = new List<string>();

    public BuildAgentUpdateOptions2010() => this.Fields = BuildAgentUpdate2010.None;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildAgentUpdate2010.None)]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public BuildAgentUpdate2010 Fields { get; set; }

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
    public AgentStatus2010 Status { get; set; }

    [XmlAttribute]
    public bool Enabled { get; set; }

    [XmlAttribute]
    public string StatusMessage { get; set; }

    public List<string> Tags => this.m_tags;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("Uri", this.Uri, "Agent", false, (string) null);
      if ((this.Fields & BuildAgentUpdate2010.BuildDirectory) == BuildAgentUpdate2010.BuildDirectory)
      {
        string buildDirectory = this.BuildDirectory;
        ArgumentValidation.CheckBuildDirectory("BuildDirectory", ref buildDirectory, false);
        this.BuildDirectory = buildDirectory;
      }
      if ((this.Fields & BuildAgentUpdate2010.ControllerUri) == BuildAgentUpdate2010.ControllerUri)
        ArgumentValidation.CheckUri("ControllerUri", this.ControllerUri, "Controller", true, ResourceStrings.MissingController());
      if ((this.Fields & BuildAgentUpdate2010.Description) == BuildAgentUpdate2010.Description)
        Microsoft.TeamFoundation.Build.Server.Validation.CheckDescription("Description", this.Description, true);
      if ((this.Fields & BuildAgentUpdate2010.Name) == BuildAgentUpdate2010.Name)
        Microsoft.TeamFoundation.Build.Common.Validation.CheckValidAgentName(this.Name, false);
      if ((this.Fields & BuildAgentUpdate2010.Tags) != BuildAgentUpdate2010.Tags)
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>("Tags", (IList<string>) this.m_tags, BuildAgentUpdateOptions2010.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag ?? (BuildAgentUpdateOptions2010.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag = new Validate<string>(Microsoft.TeamFoundation.Build.Server.Validation.CheckBuildAgentTag)), false, (string) null);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgentUpdate2010 Uri={0} Name={1} ControllerUri={2} Fields={3}]", (object) this.Uri, (object) this.Name, (object) this.ControllerUri, (object) this.Fields);
  }
}
