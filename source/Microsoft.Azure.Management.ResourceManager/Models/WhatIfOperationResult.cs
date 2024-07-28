// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.WhatIfOperationResult
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonTransformation]
  public class WhatIfOperationResult
  {
    public WhatIfOperationResult()
    {
    }

    public WhatIfOperationResult(string status = null, IList<WhatIfChange> changes = null, ErrorResponse error = null)
    {
      this.Status = status;
      this.Changes = changes;
      this.Error = error;
    }

    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    [JsonProperty(PropertyName = "properties.changes")]
    public IList<WhatIfChange> Changes { get; set; }

    [JsonProperty(PropertyName = "error")]
    public ErrorResponse Error { get; set; }
  }
}
