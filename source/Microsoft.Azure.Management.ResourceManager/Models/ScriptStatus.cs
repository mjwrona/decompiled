// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ScriptStatus
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ScriptStatus
  {
    public ScriptStatus()
    {
    }

    public ScriptStatus(
      string containerInstanceId = null,
      string storageAccountId = null,
      DateTime? startTime = null,
      DateTime? endTime = null,
      DateTime? expirationTime = null,
      DefaultErrorResponse error = null)
    {
      this.ContainerInstanceId = containerInstanceId;
      this.StorageAccountId = storageAccountId;
      this.StartTime = startTime;
      this.EndTime = endTime;
      this.ExpirationTime = expirationTime;
      this.Error = error;
    }

    [JsonProperty(PropertyName = "containerInstanceId")]
    public string ContainerInstanceId { get; private set; }

    [JsonProperty(PropertyName = "storageAccountId")]
    public string StorageAccountId { get; private set; }

    [JsonProperty(PropertyName = "startTime")]
    public DateTime? StartTime { get; private set; }

    [JsonProperty(PropertyName = "endTime")]
    public DateTime? EndTime { get; private set; }

    [JsonProperty(PropertyName = "expirationTime")]
    public DateTime? ExpirationTime { get; private set; }

    [JsonProperty(PropertyName = "error")]
    public DefaultErrorResponse Error { get; set; }
  }
}
