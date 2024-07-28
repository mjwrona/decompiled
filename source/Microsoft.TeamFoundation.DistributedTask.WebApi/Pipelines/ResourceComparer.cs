// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceComparer
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal sealed class ResourceComparer : IEqualityComparer<Resource>
  {
    public bool Equals(Resource x, Resource y)
    {
      if (x != null && y != null)
        return string.Equals(x.Alias, y.Alias, StringComparison.OrdinalIgnoreCase);
      return x == null && y == null;
    }

    public int GetHashCode(Resource obj) => obj?.Alias != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Alias) : 0;
  }
}
