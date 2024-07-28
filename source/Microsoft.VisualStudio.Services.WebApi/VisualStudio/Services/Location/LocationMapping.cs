// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.LocationMapping
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Location
{
  [DataContract]
  public class LocationMapping : ISecuredObject
  {
    public LocationMapping()
    {
    }

    public LocationMapping(string accessMappingMoniker, string location)
    {
      this.AccessMappingMoniker = accessMappingMoniker;
      this.Location = location;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("accessMappingMoniker")]
    public string AccessMappingMoniker { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("location")]
    public string Location { get; set; }

    public override string ToString() => this.Location;

    internal static LocationMapping FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      LocationMapping locationMapping = new LocationMapping();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "accessMappingMoniker":
              locationMapping.AccessMappingMoniker = reader.Value;
              continue;
            case "location":
              locationMapping.Location = reader.Value;
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return locationMapping;
    }

    Guid ISecuredObject.NamespaceId => LocationSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => LocationSecurityConstants.ServiceDefinitionsToken;

    public LocationMapping Clone() => new LocationMapping(this.AccessMappingMoniker, this.Location);
  }
}
