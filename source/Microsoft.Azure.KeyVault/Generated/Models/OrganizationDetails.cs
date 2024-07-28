// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.OrganizationDetails
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class OrganizationDetails
  {
    public OrganizationDetails()
    {
    }

    public OrganizationDetails(string id = null, IList<AdministratorDetails> adminDetails = null)
    {
      this.Id = id;
      this.AdminDetails = adminDetails;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "admin_details")]
    public IList<AdministratorDetails> AdminDetails { get; set; }
  }
}
