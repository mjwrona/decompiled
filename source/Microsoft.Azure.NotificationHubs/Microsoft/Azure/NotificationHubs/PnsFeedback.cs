// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.PnsFeedback
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "PnsFeedback", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public sealed class PnsFeedback
  {
    [DataMember(Name = "FeedbackTime", IsRequired = true, Order = 1000)]
    public DateTime FeedbackTime { get; set; }

    [DataMember(Name = "NotificationSystemError", IsRequired = true, Order = 1001)]
    public string NotificationSystemError { get; set; }

    [DataMember(Name = "Platform", IsRequired = true, Order = 1002)]
    public string Platform { get; set; }

    [DataMember(Name = "PnsHandle", IsRequired = true, Order = 1003)]
    public string PnsHandle { get; set; }

    [DataMember(Name = "RegistrationId", IsRequired = false, EmitDefaultValue = false, Order = 1004)]
    public string RegistrationId { get; set; }

    [DataMember(Name = "InstallationId", IsRequired = false, EmitDefaultValue = false, Order = 1005)]
    public string InstallationId { get; set; }

    [DataMember(Name = "NotificationId", IsRequired = false, EmitDefaultValue = false, Order = 1006)]
    public string NotificationId { get; set; }

    internal int PushOutcome { get; set; }
  }
}
