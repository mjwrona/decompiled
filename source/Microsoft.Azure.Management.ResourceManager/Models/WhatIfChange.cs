// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.WhatIfChange
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class WhatIfChange
  {
    public WhatIfChange()
    {
    }

    public WhatIfChange(
      string resourceId,
      ChangeType changeType,
      object before = null,
      object after = null,
      IList<WhatIfPropertyChange> delta = null)
    {
      this.ResourceId = resourceId;
      this.ChangeType = changeType;
      this.Before = before;
      this.After = after;
      this.Delta = delta;
    }

    [JsonProperty(PropertyName = "resourceId")]
    public string ResourceId { get; set; }

    [JsonProperty(PropertyName = "changeType")]
    public ChangeType ChangeType { get; set; }

    [JsonProperty(PropertyName = "before")]
    public object Before { get; set; }

    [JsonProperty(PropertyName = "after")]
    public object After { get; set; }

    [JsonProperty(PropertyName = "delta")]
    public IList<WhatIfPropertyChange> Delta { get; set; }

    public virtual void Validate()
    {
      if (this.ResourceId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "ResourceId");
      if (this.Delta == null)
        return;
      foreach (WhatIfPropertyChange ifPropertyChange in (IEnumerable<WhatIfPropertyChange>) this.Delta)
        ifPropertyChange?.Validate();
    }
  }
}
