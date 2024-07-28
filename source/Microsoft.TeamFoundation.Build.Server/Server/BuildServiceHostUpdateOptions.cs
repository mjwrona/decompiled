// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildServiceHostUpdateOptions
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
  public sealed class BuildServiceHostUpdateOptions : IValidatable
  {
    public BuildServiceHostUpdateOptions() => this.Fields = BuildServiceHostUpdate.None;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildServiceHostUpdate.None)]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public BuildServiceHostUpdate Fields { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string BaseUrl { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public bool RequireClientCertificates { get; set; }

    internal Guid? ServiceIdentityId { get; set; }

    internal Dictionary<string, object> Properties { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("Uri", this.Uri, "ServiceHost", false, (string) null);
      if ((this.Fields & BuildServiceHostUpdate.BaseUrl) == BuildServiceHostUpdate.BaseUrl)
        ArgumentValidation.CheckUri("BaseUrl", this.BaseUrl, false, (string) null);
      if ((this.Fields & BuildServiceHostUpdate.Name) != BuildServiceHostUpdate.Name)
        return;
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidServiceHostName(this.Name, false);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildServiceHostUpdate Uri={0} Name={1} Fields={2}]", (object) this.Uri, (object) this.Name, (object) this.Fields);
  }
}
