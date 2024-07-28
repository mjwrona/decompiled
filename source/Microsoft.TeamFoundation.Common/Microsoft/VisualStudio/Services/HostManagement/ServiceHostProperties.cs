// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.ServiceHostProperties
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServiceHostProperties
  {
    [XmlAttribute("hostId")]
    [DataMember]
    public Guid HostId { get; set; }

    [XmlAttribute("parentHostId")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid ParentHostId { get; set; }

    [XmlAttribute("name")]
    [DataMember]
    public string Name { get; set; }

    [XmlAttribute("description")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [XmlAttribute("hostType")]
    [DataMember]
    public ServiceHostType HostType { get; set; }

    [XmlAttribute("region")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Region { get; set; }

    [XmlAttribute("state")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TeamFoundationServiceHostStatus State { get; set; }

    [XmlAttribute("statusReason")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StatusReason { get; set; }

    [XmlAttribute("subStatus")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ServiceHostSubStatus SubStatus { get; set; }

    public ServiceHostProperties()
    {
    }

    public ServiceHostProperties(ServiceHostProperties serviceHost)
    {
      this.HostId = serviceHost.HostId;
      this.ParentHostId = serviceHost.ParentHostId;
      this.Name = serviceHost.Name;
      this.Description = serviceHost.Description;
      this.HostType = serviceHost.HostType;
      this.Region = serviceHost.Region;
      this.State = serviceHost.State;
      this.StatusReason = serviceHost.StatusReason;
      this.SubStatus = serviceHost.SubStatus;
    }

    public ServiceHostProperties Clone() => new ServiceHostProperties(this);
  }
}
