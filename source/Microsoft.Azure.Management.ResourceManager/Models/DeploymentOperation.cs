// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentOperation
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DeploymentOperation
  {
    public DeploymentOperation()
    {
    }

    public DeploymentOperation(
      string id = null,
      string operationId = null,
      DeploymentOperationProperties properties = null)
    {
      this.Id = id;
      this.OperationId = operationId;
      this.Properties = properties;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "operationId")]
    public string OperationId { get; private set; }

    [JsonProperty(PropertyName = "properties")]
    public DeploymentOperationProperties Properties { get; set; }
  }
}
