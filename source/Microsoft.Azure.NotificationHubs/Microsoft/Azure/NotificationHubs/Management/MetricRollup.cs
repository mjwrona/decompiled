// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.MetricRollup
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Azure.NotificationHubs.Management
{
  public class MetricRollup
  {
    [Key]
    public TimeSpan TimeGrain { get; set; }

    public TimeSpan Retention { get; set; }

    public ICollection<MetricValue> Values { get; set; }
  }
}
