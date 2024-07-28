// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.LocationMapping
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  [CallOnSerialization("PrepareForWebServiceSerialization")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class LocationMapping
  {
    public LocationMapping()
    {
    }

    public LocationMapping(string accessMappingMoniker, string location)
      : this((string) null, Guid.Empty, accessMappingMoniker, location)
    {
    }

    internal LocationMapping(
      string serviceType,
      Guid serviceIdentifier,
      string accessMappingMoniker,
      string location)
    {
      this.ServiceType = serviceType;
      this.ServiceIdentifier = serviceIdentifier;
      this.AccessMappingMoniker = accessMappingMoniker;
      this.Location = location;
    }

    [XmlAttribute("accessMappingMoniker")]
    [ClientProperty(ClientVisibility.Private)]
    public string AccessMappingMoniker { get; set; }

    [XmlAttribute("location")]
    [ClientProperty(ClientVisibility.Private)]
    public string Location { get; set; }

    internal string ServiceType { get; set; }

    internal Guid ServiceIdentifier { get; set; }
  }
}
