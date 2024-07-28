// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.ConnectionDetail
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "ConnectionDetail", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class ConnectionDetail
  {
    [Key]
    [DataMember(Name = "KeyName", IsRequired = true, Order = 1001, EmitDefaultValue = false)]
    public string KeyName { get; set; }

    [DataMember(Name = "ConnectionString", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
    public string ConnectionString { get; set; }

    [DataMember(Name = "SecondaryConnectionString", IsRequired = false, Order = 1003, EmitDefaultValue = false)]
    public string SecondaryConnectionString { get; set; }

    [DataMember(Name = "AuthorizationType", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    public string AuthorizationType { get; set; }

    [DataMember(Name = "Rights", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    public IEnumerable<AccessRights> Rights { get; set; }
  }
}
