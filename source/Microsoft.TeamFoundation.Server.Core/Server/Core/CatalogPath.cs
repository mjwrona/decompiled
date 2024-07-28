// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogPath
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class CatalogPath
  {
    public static readonly string OrganizationalPath = CatalogRoots.OrganizationalPath;
    public static readonly string InfrastructurePath = CatalogRoots.InfrastructurePath;

    public static string MakeRecursive(string catalogPath, bool fullRecursion) => fullRecursion ? catalogPath + "**" : catalogPath + "*";
  }
}
