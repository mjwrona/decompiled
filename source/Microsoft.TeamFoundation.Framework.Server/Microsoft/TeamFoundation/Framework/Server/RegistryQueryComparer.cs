// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryQueryComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RegistryQueryComparer : IEqualityComparer<RegistryQuery>
  {
    public static readonly IEqualityComparer<RegistryQuery> Instance = (IEqualityComparer<RegistryQuery>) new RegistryQueryComparer();

    public bool Equals(RegistryQuery x, RegistryQuery y) => x.Depth == y.Depth && StringComparer.OrdinalIgnoreCase.Equals(x.Path, y.Path) && StringComparer.OrdinalIgnoreCase.Equals(x.Pattern ?? string.Empty, y.Pattern ?? string.Empty);

    public int GetHashCode(RegistryQuery query) => (StringComparer.OrdinalIgnoreCase.GetHashCode(query.Path) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(query.Pattern ?? string.Empty)) + query.Depth;
  }
}
