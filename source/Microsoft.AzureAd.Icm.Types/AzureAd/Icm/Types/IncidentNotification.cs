// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentNotification
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types
{
  public class IncidentNotification
  {
    [Key]
    public long Id { get; set; }

    public long IncidentId { get; set; }

    public DateTimeOffset CreatedTime { get; set; }

    public string TriggerType { get; set; }

    public NotificationAcknowledgeInfo AcknowledgeInfo { get; set; }

    public string Notes { get; set; }

    public string Status { get; set; }

    public string NotificationTargetKey { get; set; }

    public IList<NotificationTargetEntry> Targets { get; set; }

    public IList<NotificationAction> Actions { get; set; }
  }
}
