// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DefaultErrorResponse
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DefaultErrorResponse
  {
    public DefaultErrorResponse()
    {
    }

    public DefaultErrorResponse(
      string code = null,
      string message = null,
      string target = null,
      IList<DefaultErrorResponse> details = null)
    {
      this.Code = code;
      this.Message = message;
      this.Target = target;
      this.Details = details;
    }

    [JsonProperty(PropertyName = "code")]
    public string Code { get; set; }

    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    [JsonProperty(PropertyName = "target")]
    public string Target { get; set; }

    [JsonProperty(PropertyName = "details")]
    public IList<DefaultErrorResponse> Details { get; set; }
  }
}
