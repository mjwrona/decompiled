// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryEntityComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class DirectoryEntityComparer
  {
    internal static readonly IComparer<IDirectoryEntity> DisplayName = (IComparer<IDirectoryEntity>) new DirectoryEntityComparer.DisplayNameComparer();
    internal static readonly IComparer<IDirectoryEntity> EntityId = (IComparer<IDirectoryEntity>) new DirectoryEntityComparer.EntityIdComparer();

    internal static IComparer<IDirectoryEntity> DefaultComparer => DirectoryEntityComparer.EntityId;

    private static bool TryCompareIfNulls(
      IDirectoryEntity entity1,
      IDirectoryEntity entity2,
      out int compare)
    {
      compare = -1;
      if (entity1 == null && entity2 == null)
      {
        compare = 0;
        return true;
      }
      if (entity1 == null)
      {
        compare = -1;
        return true;
      }
      if (entity2 != null)
        return false;
      compare = 1;
      return true;
    }

    private class DisplayNameComparer : IComparer<IDirectoryEntity>
    {
      public int Compare(IDirectoryEntity entity1, IDirectoryEntity entity2)
      {
        int compare;
        if (DirectoryEntityComparer.TryCompareIfNulls(entity1, entity2, out compare))
          return compare;
        int num = string.Compare(entity1["DisplayName"] as string, entity2["DisplayName"] as string, StringComparison.CurrentCultureIgnoreCase);
        return num != 0 ? num : DirectoryEntityComparer.EntityId.Compare(entity1, entity2);
      }
    }

    private class EntityIdComparer : IComparer<IDirectoryEntity>
    {
      public int Compare(IDirectoryEntity entity1, IDirectoryEntity entity2)
      {
        int compare;
        return DirectoryEntityComparer.TryCompareIfNulls(entity1, entity2, out compare) ? compare : VssStringComparer.DirectoryKeyStringComparer.Compare(entity1.EntityId, entity2.EntityId);
      }
    }
  }
}
