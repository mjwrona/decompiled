// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceInstanceType
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public class ServiceInstanceType : PropertyContainerObject
  {
    public const string InstanceTypeName = "Microsoft.TeamFoundation.Location.InstanceTypeName";

    [DataMember]
    public Guid InstanceType { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public List<LocationMapping> LocationMappings { get; set; }

    public ServiceInstanceType() => this.LocationMappings = new List<LocationMapping>();

    internal ServiceInstanceType(ServiceDefinition serviceDefinition)
    {
      string property = serviceDefinition.GetProperty<string>("Microsoft.TeamFoundation.Location.InstanceTypeName", (string) null);
      LocationMapping locationMapping1 = serviceDefinition.GetLocationMapping(AccessMappingConstants.RootDomainMappingMoniker);
      LocationMapping locationMapping2 = serviceDefinition.GetLocationMapping(AccessMappingConstants.ServiceDomainMappingMoniker);
      LocationMapping locationMapping3 = serviceDefinition.GetLocationMapping(AccessMappingConstants.ServicePathMappingMoniker);
      this.InstanceType = serviceDefinition.Identifier;
      this.Name = property;
      this.Properties = serviceDefinition.Properties;
      this.LocationMappings = new List<LocationMapping>();
      if (locationMapping1 != null)
        this.LocationMappings.Add(locationMapping1);
      if (locationMapping2 != null)
        this.LocationMappings.Add(locationMapping2);
      if (locationMapping3 == null)
        return;
      this.LocationMappings.Add(locationMapping3);
    }

    public static void AddLocationMappings(
      ServiceInstanceType instanceType,
      AccessMapping rootDomainMapping,
      AccessMapping serviceDomainMapping,
      AccessMapping servicePathMapping)
    {
      if (instanceType.LocationMappings.Any<LocationMapping>())
        throw new ArgumentException("LocationMappings must be empty.");
      if (rootDomainMapping != null)
        instanceType.LocationMappings.Add(rootDomainMapping.ToLocationMapping());
      if (serviceDomainMapping != null)
        instanceType.LocationMappings.Add(serviceDomainMapping.ToLocationMapping());
      if (servicePathMapping == null)
        return;
      instanceType.LocationMappings.Add(servicePathMapping.ToLocationMapping());
    }

    public override string ToString() => string.Format("[Name={0}, InstanceType={1}]", (object) this.Name, (object) this.InstanceType);
  }
}
