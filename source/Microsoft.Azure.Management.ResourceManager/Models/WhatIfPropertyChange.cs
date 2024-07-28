// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.WhatIfPropertyChange
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class WhatIfPropertyChange
  {
    public WhatIfPropertyChange()
    {
    }

    public WhatIfPropertyChange(
      string path,
      PropertyChangeType propertyChangeType,
      object before = null,
      object after = null,
      IList<WhatIfPropertyChange> children = null)
    {
      this.Path = path;
      this.PropertyChangeType = propertyChangeType;
      this.Before = before;
      this.After = after;
      this.Children = children;
    }

    [JsonProperty(PropertyName = "path")]
    public string Path { get; set; }

    [JsonProperty(PropertyName = "propertyChangeType")]
    public PropertyChangeType PropertyChangeType { get; set; }

    [JsonProperty(PropertyName = "before")]
    public object Before { get; set; }

    [JsonProperty(PropertyName = "after")]
    public object After { get; set; }

    [JsonProperty(PropertyName = "children")]
    public IList<WhatIfPropertyChange> Children { get; set; }

    public virtual void Validate()
    {
      if (this.Path == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Path");
      if (this.Children == null)
        return;
      foreach (WhatIfPropertyChange child in (IEnumerable<WhatIfPropertyChange>) this.Children)
        child?.Validate();
    }
  }
}
