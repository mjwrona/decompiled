// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.CatalogObjectEqualityComparer
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  internal class CatalogObjectEqualityComparer : IEqualityComparer<CatalogObject>
  {
    public bool Equals(CatalogObject x, CatalogObject y) => VssStringComparer.CatalogNodePath.Equals(x == null || x.CatalogNode == null ? (string) null : x.CatalogNode.FullPath, y == null || y.CatalogNode == null ? (string) null : y.CatalogNode.FullPath);

    public int GetHashCode(CatalogObject obj)
    {
      ArgumentUtility.CheckForNull<CatalogObject>(obj, nameof (obj));
      return VssStringComparer.CatalogNodePath.GetHashCode(obj.CatalogNode.FullPath);
    }
  }
}
