// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAgent2010
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
  [XmlType("BuildAgent")]
  public sealed class BuildAgent2010 : IValidatable
  {
    private List<string> m_tags = new List<string>();

    public BuildAgent2010() => this.Status = AgentStatus2010.Offline;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private)]
    public string ServiceHostUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ControllerUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string BuildDirectory { get; set; }

    [XmlAttribute]
    [DefaultValue(AgentStatus2010.Unavailable)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public AgentStatus2010 Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string StatusMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool Enabled { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    public string Url { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateCreated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateUpdated { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ReservedForBuild { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public List<string> Tags => this.m_tags;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidAgentName(this.Name, false);
      ArgumentValidation.CheckUri("ControllerUri", this.ControllerUri, "Controller", false, ResourceStrings.MissingController());
      ArgumentValidation.CheckUri("ServiceHostUri", this.ServiceHostUri, "ServiceHost", false, (string) null);
      string buildDirectory = this.BuildDirectory;
      ArgumentValidation.CheckBuildDirectory("BuildDirectory", ref buildDirectory, false);
      this.BuildDirectory = buildDirectory;
      Microsoft.TeamFoundation.Build.Server.Validation.CheckDescription("Description", this.Description, true);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>("Tags", (IList<string>) this.Tags, BuildAgent2010.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag ?? (BuildAgent2010.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag = new Validate<string>(Microsoft.TeamFoundation.Build.Server.Validation.CheckBuildAgentTag)), false, (string) null);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgent2010 Uri={0} Name={1} ServiceHostUri={2} ControllerUri={3} Status={4}]", (object) this.Uri, (object) this.Name, (object) this.ServiceHostUri, (object) this.ControllerUri, (object) this.Status);
  }
}
