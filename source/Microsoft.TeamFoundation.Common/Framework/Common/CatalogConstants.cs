// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CatalogConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CatalogConstants
  {
    public static readonly int MandatoryNodePathLength = 24;
    public static readonly int MaximumPathLength = 888;
    public static readonly int ParentPathLength = 36 * CatalogConstants.MandatoryNodePathLength;
    public static readonly string FullRecurseStars = "**";
    public static readonly string FullRecurseDots = "...";
    public static readonly string SingleRecurseStar = "*";
    public static readonly char[] PatternMatchingCharacters = new char[2]
    {
      '*',
      '.'
    };
    public static readonly string IconRepositoryServiceType = "CatalogIconRepository";
  }
}
