// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildControllerSpec
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

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
  public class BuildControllerSpec : IValidatable, IPropertyProviderSpec
  {
    private List<string> m_propertyNameFilters = new List<string>();

    public BuildControllerSpec()
    {
    }

    public BuildControllerSpec(string name, string serviceHostName, bool includeAgents)
    {
      this.Name = name;
      this.IncludeAgents = includeAgents;
      this.ServiceHostName = serviceHostName;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string ServiceHostName { get; set; }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public List<string> PropertyNameFilters => this.m_propertyNameFilters;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool IncludeAgents { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidControllerName(this.Name, true);
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidServiceHostName(this.ServiceHostName, true);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildControllerSpec Name={0} ServiceHostName={1} IncludeAgents={2}]", (object) this.Name, (object) this.ServiceHostName, (object) this.IncludeAgents);
  }
}
