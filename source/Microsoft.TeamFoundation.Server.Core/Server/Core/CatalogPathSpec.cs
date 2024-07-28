// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogPathSpec
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogPathSpec
  {
    private const int FullRecursion = 2;
    private const int OneLevelRecursion = 1;
    private const int NoRecursion = 0;
    private const string ValidBase64CharacterPattern = "^[a-zA-Z0-9\\+/=]+$";

    public static void ParsePathSpec(string pathSpec, out string path, out int recursion)
    {
      if (VssStringComparer.CatalogNodePath.EndsWith(pathSpec, CatalogConstants.FullRecurseDots))
      {
        recursion = 2;
        path = pathSpec.Trim('.');
      }
      else if (VssStringComparer.CatalogNodePath.EndsWith(pathSpec, CatalogConstants.FullRecurseStars))
      {
        recursion = 2;
        path = pathSpec.Trim('*');
      }
      else if (VssStringComparer.CatalogNodePath.EndsWith(pathSpec, CatalogConstants.SingleRecurseStar))
      {
        recursion = 1;
        path = pathSpec.Trim('*');
      }
      else
      {
        recursion = 0;
        path = pathSpec;
      }
    }

    public static void ValidatePathSpec(
      string pathSpec,
      bool allowWildcards,
      bool allowNullOrEmpty)
    {
      if (!allowNullOrEmpty)
        ArgumentUtility.CheckStringForNullOrEmpty(pathSpec, "path");
      else if (string.IsNullOrEmpty(pathSpec))
        return;
      string input = pathSpec;
      if (allowWildcards)
      {
        if (VssStringComparer.CatalogNodePath.EndsWith(pathSpec, CatalogConstants.FullRecurseDots))
          input = pathSpec.Substring(0, pathSpec.Length - 3);
        else if (VssStringComparer.CatalogNodePath.EndsWith(pathSpec, CatalogConstants.FullRecurseStars))
          input = pathSpec.Substring(0, pathSpec.Length - 2);
        else if (VssStringComparer.CatalogNodePath.EndsWith(pathSpec, CatalogConstants.SingleRecurseStar))
          input = pathSpec.Substring(0, pathSpec.Length - 1);
      }
      else if (pathSpec.IndexOfAny(CatalogConstants.PatternMatchingCharacters) != -1)
        throw new InvalidCatalogNodePathException(CatalogPathSpec.EnsureExceptionPathLengthLimit(pathSpec), allowWildcards);
      if (input.Length <= 0)
        return;
      if (input.Length > CatalogConstants.MaximumPathLength || input.Length % CatalogConstants.MandatoryNodePathLength != 0)
        throw new InvalidCatalogNodePathException(CatalogPathSpec.EnsureExceptionPathLengthLimit(pathSpec), allowWildcards);
      if (!Regex.IsMatch(input, "^[a-zA-Z0-9\\+/=]+$"))
        throw new InvalidCatalogNodePathException(pathSpec, allowWildcards);
    }

    internal static string EnsureExceptionPathLengthLimit(string path) => path.Length > 1000 ? TFCommonResources.Ellipsis((object) path.Substring(0, 1000)) : path;
  }
}
