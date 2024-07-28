// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CatalogRoots
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CatalogRoots
  {
    public static readonly string OrganizationalPath = "3eYRYkJOok6GHrKam0AcAA==";
    public static readonly string InfrastructurePath = "Vc1S6XwnTEe/isOiPfhmxw==";

    public static CatalogTree DetermineTree(string path) => VssStringComparer.CatalogNodePath.Equals(CatalogRoots.InfrastructurePath, path.Substring(0, CatalogConstants.MandatoryNodePathLength)) ? CatalogTree.Infrastructure : CatalogTree.Organizational;

    public static string DeterminePath(CatalogTree tree)
    {
      if (tree == CatalogTree.Organizational)
        return CatalogRoots.OrganizationalPath;
      return tree == CatalogTree.Infrastructure ? CatalogRoots.InfrastructurePath : (string) null;
    }
  }
}
