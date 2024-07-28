// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAgentSpec2008
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [XmlType("BuildAgentSpec")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public sealed class BuildAgentSpec2008 : BuildGroupItemSpec2010, IValidatable
  {
    private int m_port;
    private string m_machineName;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string MachineName
    {
      get => this.m_machineName;
      set => this.m_machineName = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int Port
    {
      get => this.m_port;
      set => this.m_port = value;
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      this.Validate(requestContext, context);
      ArgumentValidation.CheckBound("Port", this.m_port, -1, 65536);
      ArgumentValidation.CheckBuildMachine("MachineName", this.m_machineName, true);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgentSpec2008 MachineName={0} Port={1}]", (object) this.MachineName, (object) this.Port);
  }
}
