// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.NotificationHubPnsCredentials
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "NotificationHubPnsCredentials", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class NotificationHubPnsCredentials
  {
    [DataMember(Name = "ApnsCredential", IsRequired = false, EmitDefaultValue = false, Order = 1001)]
    public ApnsCredential ApnsCredential { get; set; }

    [DataMember(Name = "WnsCredential", IsRequired = false, EmitDefaultValue = false, Order = 1002)]
    public WnsCredential WnsCredential { get; set; }

    [DataMember(Name = "GcmCredential", IsRequired = false, EmitDefaultValue = false, Order = 1003)]
    public GcmCredential GcmCredential { get; set; }

    [DataMember(Name = "MpnsCredential", IsRequired = false, EmitDefaultValue = false, Order = 1004)]
    public MpnsCredential MpnsCredential { get; set; }

    [DataMember(Name = "UpdatedAt", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    public DateTime UpdatedAt { get; internal set; }

    [DataMember(Name = "Revision", IsRequired = false, Order = 1006, EmitDefaultValue = false)]
    public long Revision { get; set; }

    [DataMember(Name = "AdmCredential", IsRequired = false, EmitDefaultValue = false, Order = 1007)]
    public AdmCredential AdmCredential { get; set; }

    [DataMember(Name = "BaiduCredential", IsRequired = false, EmitDefaultValue = false, Order = 1009)]
    public BaiduCredential BaiduCredential { get; set; }

    [DataMember(Name = "NokiaXCredential", IsRequired = false, EmitDefaultValue = false, Order = 1008)]
    internal NokiaXCredential NokiaXCredential { get; set; }
  }
}
