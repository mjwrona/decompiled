// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationHubJob
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "NotificationHubJob", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public sealed class NotificationHubJob : EntityDescription, IResourceDescription
  {
    [DataMember(Name = "JobId", IsRequired = false, Order = 1001, EmitDefaultValue = false)]
    public string JobId { get; internal set; }

    public string CollectionName => "jobs";

    public string OutputFileName
    {
      get
      {
        string empty = string.Empty;
        this.OutputProperties?.TryGetValue("OutputFilePath", out empty);
        return empty;
      }
    }

    public string FailuresFileName
    {
      get
      {
        string empty = string.Empty;
        this.OutputProperties?.TryGetValue("FailedFilePath", out empty);
        return empty;
      }
    }

    [DataMember(Name = "Progress", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
    public Decimal Progress { get; internal set; }

    [DataMember(Name = "Type", IsRequired = true, Order = 1003, EmitDefaultValue = true)]
    public NotificationHubJobType JobType { get; set; }

    [DataMember(Name = "Status", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    public NotificationHubJobStatus Status { get; internal set; }

    [DataMember(Name = "OutputContainerUri", IsRequired = true, Order = 1005, EmitDefaultValue = false)]
    public Uri OutputContainerUri { get; set; }

    [DataMember(Name = "ImportFileUri", IsRequired = false, Order = 1006, EmitDefaultValue = false)]
    public Uri ImportFileUri { get; set; }

    [DataMember(Name = "InputProperties", IsRequired = false, Order = 1007, EmitDefaultValue = false)]
    public Dictionary<string, string> InputProperties { get; set; }

    [DataMember(Name = "Failure", IsRequired = false, Order = 1008, EmitDefaultValue = false)]
    public string Failure { get; internal set; }

    [DataMember(Name = "OutputProperties", IsRequired = false, Order = 1009, EmitDefaultValue = false)]
    public Dictionary<string, string> OutputProperties { get; internal set; }

    [DataMember(Name = "CreatedAt", IsRequired = false, Order = 1010, EmitDefaultValue = false)]
    public DateTime CreatedAt { get; internal set; }

    [DataMember(Name = "UpdatedAt", IsRequired = false, Order = 1011, EmitDefaultValue = false)]
    public DateTime UpdatedAt { get; internal set; }
  }
}
