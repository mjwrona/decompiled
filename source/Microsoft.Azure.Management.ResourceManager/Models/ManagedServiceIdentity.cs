// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ManagedServiceIdentity
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ManagedServiceIdentity
  {
    public ManagedServiceIdentity()
    {
    }

    public ManagedServiceIdentity(
      string type = null,
      IDictionary<string, UserAssignedIdentity> userAssignedIdentities = null)
    {
      this.Type = type;
      this.UserAssignedIdentities = userAssignedIdentities;
    }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    [JsonProperty(PropertyName = "userAssignedIdentities")]
    public IDictionary<string, UserAssignedIdentity> UserAssignedIdentities { get; set; }
  }
}
