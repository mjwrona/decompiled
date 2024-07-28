// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.Internal.NameResolutionKeyComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NameResolution.Server.Internal
{
  internal class NameResolutionKeyComparer : IEqualityComparer<NameResolutionKey>
  {
    private static readonly NameResolutionKeyComparer s_comparer = new NameResolutionKeyComparer();

    public bool Equals(NameResolutionKey x, NameResolutionKey y) => StringComparer.OrdinalIgnoreCase.Equals(x.Namespace, y.Namespace) && StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name);

    public int GetHashCode(NameResolutionKey obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Namespace) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);

    public static NameResolutionKeyComparer Instance => NameResolutionKeyComparer.s_comparer;
  }
}
