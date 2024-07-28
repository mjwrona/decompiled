// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.RegistrationResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "RegistrationResult", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public sealed class RegistrationResult
  {
    [DataMember(Name = "ApplicationPlatform", IsRequired = true, Order = 1001, EmitDefaultValue = true)]
    public string ApplicationPlatform { get; set; }

    [DataMember(Name = "PnsHandle", IsRequired = true, Order = 1002, EmitDefaultValue = true)]
    public string PnsHandle { get; set; }

    [DataMember(Name = "RegistrationId", IsRequired = true, Order = 1003, EmitDefaultValue = true)]
    public string RegistrationId { get; set; }

    [DataMember(Name = "Outcome", Order = 1004, EmitDefaultValue = true)]
    public string Outcome { get; set; }
  }
}
