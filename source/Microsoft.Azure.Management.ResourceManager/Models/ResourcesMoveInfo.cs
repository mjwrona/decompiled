// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ResourcesMoveInfo
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ResourcesMoveInfo
  {
    public ResourcesMoveInfo()
    {
    }

    public ResourcesMoveInfo(IList<string> resources = null, string targetResourceGroup = null)
    {
      this.Resources = resources;
      this.TargetResourceGroup = targetResourceGroup;
    }

    [JsonProperty(PropertyName = "resources")]
    public IList<string> Resources { get; set; }

    [JsonProperty(PropertyName = "targetResourceGroup")]
    public string TargetResourceGroup { get; set; }
  }
}
