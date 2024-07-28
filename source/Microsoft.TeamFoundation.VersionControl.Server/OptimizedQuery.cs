// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.OptimizedQuery
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class OptimizedQuery : DeferredQuery
  {
    private List<DeferredQuery> m_children;

    internal OptimizedQuery(
      VersionControlRequestContext versionControlRequestContext,
      DeferredQuery[] children)
      : base(versionControlRequestContext, new ItemSpec(children[0].OptimizationRoot, RecursionType.OneLevel, children[0].ItemSpec.DeletionId), children[0].DeletedState, children[0].ItemType, children[0].VersionSpec, children[0].UseMappings, children[0].Options)
    {
      this.m_children = new List<DeferredQuery>((IEnumerable<DeferredQuery>) children);
      for (int index = 1; index < children.Length; ++index)
      {
        if (children[index].DeletedState != this.m_deletedState || children[index].DeletedState == DeletedState.Deleted)
          this.m_deletedState = DeletedState.Any;
        if (children[index].ItemType != this.m_itemType)
          this.m_itemType = ItemType.Any;
      }
      if (this.m_deletedState == DeletedState.NonDeleted)
        return;
      this.m_itemSpec.DeletionId = -1;
    }

    internal override void Execute(Workspace localWorkspace, VersionedItemComponent db)
    {
      base.Execute(localWorkspace, db);
      if (this.m_exception != null)
      {
        for (int index = 0; index < this.m_children.Count; ++index)
          this.m_children[index].ProcessException(this.m_exception);
      }
      else
      {
        List<DeferredQuery> deferredQueryList = new List<DeferredQuery>((IEnumerable<DeferredQuery>) this.m_children);
        Item result;
        while (base.TryGetNextItem(out result))
        {
          for (int index = 0; index < deferredQueryList.Count; ++index)
          {
            if (deferredQueryList[index].Match(result))
            {
              deferredQueryList[index].ProcessResult(result);
              deferredQueryList.RemoveAt(index--);
            }
          }
        }
      }
    }

    internal override bool TryGetNextItem(out Item item) => throw new NotSupportedException();
  }
}
