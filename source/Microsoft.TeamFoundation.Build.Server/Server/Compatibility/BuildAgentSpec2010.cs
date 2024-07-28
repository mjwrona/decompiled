// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAgentSpec2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildAgentSpec")]
  public sealed class BuildAgentSpec2010
  {
    private List<string> m_tags = new List<string>();

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string ServiceHostName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string ControllerName { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public List<string> Tags => this.m_tags;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgentSpec2010 Name={0} ServiceHostName={1} ControllerName={2}]", (object) this.Name, (object) this.ServiceHostName, (object) this.ControllerName);
  }
}
