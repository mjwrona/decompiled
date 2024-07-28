// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.RegionCodeDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "RegionCodeDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class RegionCodeDescription
  {
    public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof (RegionCodeDescription));

    [DataMember(Name = "Code", IsRequired = true, Order = 100, EmitDefaultValue = false)]
    public string Code { get; set; }

    [DataMember(Name = "FullName", IsRequired = true, Order = 101, EmitDefaultValue = false)]
    public string FullName { get; set; }
  }
}
