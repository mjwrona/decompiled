// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.UserVariablesInternal
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal class UserVariablesInternal
  {
    public readonly System.Collections.Generic.Dictionary<string, string> Dictionary = new System.Collections.Generic.Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public readonly UserVariablesInternal Parent;
    public int Count;

    internal UserVariablesInternal(UserVariablesInternal parent)
    {
      if (parent == null)
        return;
      this.Parent = parent;
      this.Count = parent.Count;
    }
  }
}
