// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.SortedDirectoryEntityCollector
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class SortedDirectoryEntityCollector : IEnumerable<IDirectoryEntity>, IEnumerable
  {
    private readonly LinkedList<IDirectoryEntity> sortedList = new LinkedList<IDirectoryEntity>();
    private readonly IComparer<IDirectoryEntity> entityIdComparer = DirectoryEntityComparer.EntityId;
    private readonly SortedDirectoryEntityCollector.MergeEntities merge;

    internal SortedDirectoryEntityCollector(SortedDirectoryEntityCollector.MergeEntities merge)
    {
      ArgumentUtility.CheckForNull<SortedDirectoryEntityCollector.MergeEntities>(merge, nameof (merge));
      this.merge = merge;
    }

    internal void Merge(IEnumerable<IDirectoryEntity> entities)
    {
      IOrderedEnumerable<IDirectoryEntity> orderedEnumerable = entities.OrderBy<IDirectoryEntity, IDirectoryEntity>((Func<IDirectoryEntity, IDirectoryEntity>) (entity => entity), this.entityIdComparer);
      LinkedListNode<IDirectoryEntity> first = this.sortedList.First;
      foreach (IDirectoryEntity directoryEntity in (IEnumerable<IDirectoryEntity>) orderedEnumerable)
        this.MergeEntity(ref first, directoryEntity);
    }

    private void MergeEntity(ref LinkedListNode<IDirectoryEntity> node, IDirectoryEntity value)
    {
      while (node != null)
      {
        int num = this.entityIdComparer.Compare(node.Value, value);
        if (num == 0)
        {
          node.Value = this.merge(node.Value, value);
          return;
        }
        if (num > 0)
        {
          this.sortedList.AddBefore(node, value);
          return;
        }
        node = node.Next;
      }
      this.sortedList.AddLast(value);
    }

    public IEnumerator<IDirectoryEntity> GetEnumerator() => (IEnumerator<IDirectoryEntity>) this.sortedList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.sortedList.GetEnumerator();

    internal delegate IDirectoryEntity MergeEntities(IDirectoryEntity x, IDirectoryEntity y);
  }
}
