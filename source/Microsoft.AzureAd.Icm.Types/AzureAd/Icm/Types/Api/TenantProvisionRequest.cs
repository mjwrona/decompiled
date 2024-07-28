// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Api.TenantProvisionRequest
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types.Api
{
  public class TenantProvisionRequest
  {
    public TenantProvisionRequest() => this.Datacenter = (ICollection<string>) new Collection<string>();

    [Key]
    public long Id { get; set; }

    public long? TenantId { get; set; }

    [Required]
    public string TenantName { get; set; }

    [Required]
    public string SiloName { get; set; }

    [Required]
    public string FrontEndCategoryName { get; set; }

    [Required]
    public string ServiceAdminUPN { get; set; }

    [Required]
    public string AccessRequestDisplayName { get; set; }

    [Required]
    public string AccessRequestAddress { get; set; }

    public ICollection<string> Datacenter { get; set; }

    [Required]
    public string TriageTeamAlias { get; set; }

    [Required]
    public string TriageTeamDomain { get; set; }

    [Required]
    public string ImTeamAlias { get; set; }

    [Required]
    public string ImTeamDomain { get; set; }

    [Required]
    public string ExecImTeamAlias { get; set; }

    [Required]
    public string ExecImTeamDomain { get; set; }

    public string CommsTeamAlias { get; set; }

    public string CommsTeamDomain { get; set; }

    public string Keywords { get; set; }

    public string Description { get; set; }
  }
}
