// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationDetails
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "NotificationDetails", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public sealed class NotificationDetails : IExtensibleDataObject
  {
    [DataMember(Name = "NotificationId", IsRequired = false, Order = 1000, EmitDefaultValue = false)]
    public string NotificationId { get; set; }

    [DataMember(Name = "Location", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
    public Uri Location { get; set; }

    [DataMember(Name = "State", IsRequired = false, Order = 1003, EmitDefaultValue = true)]
    private string NotificationState
    {
      get => this.State.ToString();
      set
      {
        NotificationOutcomeState result;
        this.State = Enum.TryParse<NotificationOutcomeState>(value, true, out result) ? result : NotificationOutcomeState.Unknown;
      }
    }

    public NotificationOutcomeState State { get; set; }

    [DataMember(Name = "EnqueueTime", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    public DateTime? EnqueueTime { get; set; }

    [DataMember(Name = "StartTime", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(Name = "EndTime", IsRequired = false, Order = 1006, EmitDefaultValue = false)]
    public DateTime? EndTime { get; set; }

    [DataMember(Name = "NotificationBody", IsRequired = false, Order = 1007, EmitDefaultValue = false)]
    public string NotificationBody { get; set; }

    [DataMember(Name = "Tags", IsRequired = false, Order = 1008, EmitDefaultValue = false)]
    public string Tags { get; set; }

    [DataMember(Name = "TargetPlatforms", IsRequired = false, Order = 1009, EmitDefaultValue = false)]
    public string TargetPlatforms { get; set; }

    [DataMember(Name = "ApnsOutcomeCounts", IsRequired = false, Order = 1010, EmitDefaultValue = false)]
    public NotificationOutcomeCollection ApnsOutcomeCounts { get; set; }

    [DataMember(Name = "MpnsOutcomeCounts", IsRequired = false, Order = 1011, EmitDefaultValue = false)]
    public NotificationOutcomeCollection MpnsOutcomeCounts { get; set; }

    [DataMember(Name = "WnsOutcomeCounts", IsRequired = false, Order = 1012, EmitDefaultValue = false)]
    public NotificationOutcomeCollection WnsOutcomeCounts { get; set; }

    [DataMember(Name = "GcmOutcomeCounts", IsRequired = false, Order = 1013, EmitDefaultValue = false)]
    public NotificationOutcomeCollection GcmOutcomeCounts { get; set; }

    [DataMember(Name = "AdmOutcomeCounts", IsRequired = false, Order = 1014, EmitDefaultValue = false)]
    public NotificationOutcomeCollection AdmOutcomeCounts { get; set; }

    [DataMember(Name = "PnsErrorDetailsUri", IsRequired = false, Order = 1015, EmitDefaultValue = false)]
    public string PnsErrorDetailsUri { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
