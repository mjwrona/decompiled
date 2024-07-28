// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Api.Tenant
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types.Api
{
  public class Tenant
  {
    [Key]
    public long Id { get; set; }

    public string PublicId { get; set; }

    public Guid TenantGuid { get; set; }

    public string Name { get; set; }

    public long SiloId { get; set; }

    public string Description { get; set; }

    public int HighSeverityThreshold { get; set; }

    public int EmailSeverityThreshold { get; set; }

    public string SuppressionExpiryReminderEmail { get; set; }

    public int? SuppressionExpiryReminderFrequency { get; set; }

    public string AccessRequestAddress { get; set; }

    public string AccessRequestFriendlyName { get; set; }

    public int? AutoUpdateFrequency { get; set; }

    public string AutoUpdateExcludedEnvironments { get; set; }

    public string Keywords { get; set; }

    public long? FrontEndCategoryId { get; set; }

    public string FrontEndCategoryName { get; set; }

    public long? ExternalSyncAlertSourceLocalId { get; set; }

    public long? IncidentManagerTeamId { get; set; }

    public long? CommunicationsManagerTeamId { get; set; }

    public long? SecurityTeamId { get; set; }

    public string IncidentManagerTeamName { get; set; }

    public string CommunicationsManagerTeamName { get; set; }

    public string SecurityTeamName { get; set; }

    public virtual ICollection<Team> Teams { get; set; }

    public virtual ICollection<Contact> Members { get; set; }

    public virtual TenantSilo TenantSilo { get; set; }

    public ICollection<string> IncidentTypes { get; set; }

    public ICollection<string> DataCenters { get; set; }

    public ICollection<string> Components { get; set; }

    public string Status { get; set; }

    public override string ToString() => this.Name + " (" + (object) this.Id + ")";
  }
}
