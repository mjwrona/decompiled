// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemSetToVersionedItemSpecEnumerator
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class ItemSetToVersionedItemSpecEnumerator : 
    IEnumerable<ArtifactSpec>,
    IEnumerable,
    IEnumerator<ArtifactSpec>,
    IDisposable,
    IEnumerator
  {
    private IEnumerator<ItemSet> m_itemSets;
    private IEnumerator<Item> m_items;
    private bool m_unversioned;

    internal ItemSetToVersionedItemSpecEnumerator(bool unversioned, IEnumerable<ItemSet> itemSets)
    {
      this.m_unversioned = unversioned;
      this.m_itemSets = itemSets.GetEnumerator();
    }

    public IEnumerator<ArtifactSpec> GetEnumerator() => (IEnumerator<ArtifactSpec>) this;

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    public ArtifactSpec Current
    {
      get
      {
        int changesetId = this.m_unversioned ? 0 : this.m_items.Current.ChangesetId;
        return new ArtifactSpec(VersionControlPropertyKinds.VersionedItem, this.m_items.Current.ItemId, changesetId, this.m_items.Current.ItemDataspaceId);
      }
    }

    public void Dispose()
    {
    }

    object IEnumerator.Current => (object) this.Current;

    public bool MoveNext()
    {
      bool flag = this.m_items != null && this.m_items.MoveNext();
      if (!flag && (flag = this.m_itemSets.MoveNext()))
      {
        this.m_items = this.m_itemSets.Current.Items.GetEnumerator();
        flag = this.m_items.MoveNext();
      }
      return flag;
    }

    public void Reset()
    {
      this.m_itemSets.Reset();
      this.m_items.Reset();
    }
  }
}
