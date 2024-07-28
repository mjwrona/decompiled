// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationOutcome
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "NotificationOutcome", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public sealed class NotificationOutcome
  {
    internal Dictionary<string, NotificationOutcomeCollection> perPlatformOutcomeCollection;

    internal bool DelayedRetry { get; set; }

    internal bool ReleaseSession { get; set; }

    public NotificationOutcomeState State { get; internal set; }

    [DataMember(Name = "Success", IsRequired = true, Order = 1001, EmitDefaultValue = true)]
    public long Success { get; set; }

    [DataMember(Name = "Failure", IsRequired = true, Order = 1002, EmitDefaultValue = true)]
    public long Failure { get; set; }

    [DataMember(Name = "Results", IsRequired = true, Order = 1003, EmitDefaultValue = true)]
    public List<RegistrationResult> Results { get; set; }

    public string NotificationId { get; set; }

    public string TrackingId { get; internal set; }

    internal static NotificationOutcome GetUnknownOutCome()
    {
      RegistrationResult registrationResult = new RegistrationResult()
      {
        ApplicationPlatform = "Unknown",
        PnsHandle = "Unknown",
        RegistrationId = "Unknown",
        Outcome = "UnknownError"
      };
      return new NotificationOutcome()
      {
        Failure = 1,
        Success = 0,
        Results = new List<RegistrationResult>()
        {
          registrationResult
        }
      };
    }
  }
}
