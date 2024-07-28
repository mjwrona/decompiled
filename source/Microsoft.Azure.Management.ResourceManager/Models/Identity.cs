// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Identity
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class Identity
  {
    public Identity()
    {
    }

    public Identity(
      string principalId = null,
      string tenantId = null,
      ResourceIdentityType? type = null,
      IDictionary<string, IdentityUserAssignedIdentitiesValue> userAssignedIdentities = null)
    {
      this.PrincipalId = principalId;
      this.TenantId = tenantId;
      this.Type = type;
      this.UserAssignedIdentities = userAssignedIdentities;
    }

    [JsonProperty(PropertyName = "principalId")]
    public string PrincipalId { get; private set; }

    [JsonProperty(PropertyName = "tenantId")]
    public string TenantId { get; private set; }

    [JsonProperty(PropertyName = "type")]
    public ResourceIdentityType? Type { get; set; }

    [JsonProperty(PropertyName = "userAssignedIdentities")]
    public IDictionary<string, IdentityUserAssignedIdentitiesValue> UserAssignedIdentities { get; set; }
  }
}
