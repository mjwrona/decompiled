// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Api.PublicPostmortem
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types.Api
{
  public class PublicPostmortem
  {
    [Key]
    public long Id { get; set; }

    public long PirId { get; set; }

    public string OwnerContactName { get; set; }

    public string OwningTeamName { get; set; }

    public string OwningTenantName { get; set; }

    public long SiloId { get; set; }

    public string Status { get; set; }

    public string Title { get; set; }

    public string ServicesImpacted { get; set; }

    public DateTime? IncidentStartDate { get; set; }

    public DateTime? ServiceRestoredDate { get; set; }

    public string Summary { get; set; }

    public string CustomerImpact { get; set; }

    public string Workaround { get; set; }

    public string AffectedSubRegions { get; set; }

    public string RootCauseDescription { get; set; }

    public string NextSteps { get; set; }

    public IList<TimeField> AdditionalTimeFields { get; set; }

    public IList<PirResource> Resources { get; set; }

    public IList<PirIncident> Incidents { get; set; }
  }
}
