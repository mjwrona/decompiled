// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.MetricValue
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Azure.NotificationHubs.Management
{
  public class MetricValue
  {
    [Key]
    public DateTime Timestamp { get; set; }

    public long Min { get; set; }

    public long Max { get; set; }

    public long Total { get; set; }

    public float Average { get; set; }
  }
}
