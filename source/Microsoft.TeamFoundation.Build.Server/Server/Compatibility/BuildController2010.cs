// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildController2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildController")]
  public sealed class BuildController2010 : IValidatable
  {
    private List<string> m_tags = new List<string>();

    public BuildController2010() => this.Status = ControllerStatus2010.Unavailable;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private)]
    public string ServiceHostUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string CustomAssemblyPath { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public int MaxConcurrentBuilds { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int QueueCount { get; set; }

    [XmlAttribute]
    [DefaultValue(ControllerStatus2010.Unavailable)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public ControllerStatus2010 Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string StatusMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool Enabled { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Url { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateCreated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateUpdated { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "InternalTags", Direction = ClientPropertySerialization.ServerToClientOnly)]
    public List<string> Tags => this.m_tags;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidControllerName(this.Name, false);
      Microsoft.TeamFoundation.Build.Server.Validation.CheckDescription("Description", this.Description, true);
      ArgumentValidation.CheckUri("ServiceHostUri", this.ServiceHostUri, "ServiceHost", false, (string) null);
      ArgumentValidation.CheckBound("MaxConcurrentBuilds", this.MaxConcurrentBuilds, 0, int.MaxValue);
      string customAssemblyPath = this.CustomAssemblyPath;
      Microsoft.TeamFoundation.Build.Server.Validation.CheckVersionControlPath("CustomAssemblyPath", ref customAssemblyPath, true);
      this.CustomAssemblyPath = customAssemblyPath;
      if (!string.IsNullOrEmpty(this.CustomAssemblyPath) && VersionControlPath.Equals(this.CustomAssemblyPath, "$/"))
        throw new ArgumentException(ResourceStrings.InvalidCustomAssemblyPath());
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildController2010 Uri={0} Name={1} ServiceHostUri={2} Status={3}]", (object) this.Uri, (object) this.Name, (object) this.ServiceHostUri, (object) this.Status);
  }
}
