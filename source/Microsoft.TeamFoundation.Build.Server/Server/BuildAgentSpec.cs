// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildAgentSpec
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnSerialization("BeforeSerialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildAgentSpec : IValidatable, IPropertyProviderSpec
  {
    private List<string> m_tags = new List<string>();
    private List<string> m_propertyNameFilters = new List<string>();

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string ServiceHostName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string ControllerName { get; set; }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public List<string> PropertyNameFilters => this.m_propertyNameFilters;

    [ClientProperty(ClientVisibility.Private)]
    public List<string> Tags => this.m_tags;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidAgentName(this.Name, true);
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidControllerName(this.ControllerName, true);
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidServiceHostName(this.ServiceHostName, true);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>("Tags", (IList<string>) this.Tags, BuildAgentSpec.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag ?? (BuildAgentSpec.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag = new Validate<string>(Validation.CheckBuildAgentTag)), false, (string) null);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgentSpec Name={0} ServiceHostName={1} ControllerName={2}]", (object) this.Name, (object) this.ServiceHostName, (object) this.ControllerName);
  }
}
