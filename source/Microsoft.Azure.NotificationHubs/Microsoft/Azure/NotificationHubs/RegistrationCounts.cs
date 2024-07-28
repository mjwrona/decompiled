// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.RegistrationCounts
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "RegistrationCounts", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  internal sealed class RegistrationCounts
  {
    [DataMember(Name = "AllRegistrationsCount", IsRequired = true, Order = 1001, EmitDefaultValue = true)]
    public long AllRegistrationsCount { get; internal set; }

    [DataMember(Name = "WindowsRegistrationsCount", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
    public long WindowsRegistrationsCount { get; internal set; }

    [DataMember(Name = "AppleRegistrationsCount", IsRequired = false, Order = 1003, EmitDefaultValue = false)]
    public long AppleRegistrationsCount { get; internal set; }

    [DataMember(Name = "GcmRegistrationsCount", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    public long GcmRegistrationsCount { get; internal set; }

    [DataMember(Name = "MpnsRegistrationsCount", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    public long MpnsRegistrationsCount { get; internal set; }

    [DataMember(Name = "AdmRegistrationsCount", IsRequired = false, Order = 1006, EmitDefaultValue = false)]
    public long AdmRegistrationsCount { get; internal set; }

    [DataMember(Name = "BaiduRegistrationsCount", IsRequired = false, Order = 1007, EmitDefaultValue = false)]
    public long BaiduRegistrationsCount { get; internal set; }
  }
}
