// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.UserAssignedIdentity
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class UserAssignedIdentity
  {
    public UserAssignedIdentity()
    {
    }

    public UserAssignedIdentity(string principalId = null, string clientId = null)
    {
      this.PrincipalId = principalId;
      this.ClientId = clientId;
    }

    [JsonProperty(PropertyName = "principalId")]
    public string PrincipalId { get; set; }

    [JsonProperty(PropertyName = "clientId")]
    public string ClientId { get; set; }
  }
}
