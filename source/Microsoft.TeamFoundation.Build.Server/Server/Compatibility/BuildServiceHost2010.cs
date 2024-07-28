// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildServiceHost2010
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
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildServiceHost")]
  public sealed class BuildServiceHost2010 : IValidatable
  {
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string BaseUrl { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool RequireClientCertificates { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public bool IsVirtual { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidServiceHostName(this.Name, false);
      ArgumentValidation.CheckUri("BaseUrl", this.BaseUrl, false, (string) null);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildServiceHost2010 Uri={0} Name={1} BaseUrl={2}]", (object) this.Uri, (object) this.Name, (object) this.BaseUrl);
  }
}
