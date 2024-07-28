// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemToAnnotationSpecEnumerator
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
  internal sealed class ItemToAnnotationSpecEnumerator : 
    IEnumerable<ArtifactSpec>,
    IEnumerable,
    IEnumerator<ArtifactSpec>,
    IDisposable,
    IEnumerator
  {
    private IEnumerator<Item> m_items;

    internal ItemToAnnotationSpecEnumerator(IEnumerable<Item> items) => this.m_items = items.GetEnumerator();

    public IEnumerator<ArtifactSpec> GetEnumerator() => (IEnumerator<ArtifactSpec>) this;

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    public ArtifactSpec Current => new ArtifactSpec(VersionControlPropertyKinds.Annotation, this.m_items.Current.ItemId, this.m_items.Current.ChangesetId, this.m_items.Current.ItemDataspaceId);

    public void Dispose()
    {
    }

    object IEnumerator.Current => (object) this.Current;

    public bool MoveNext() => this.m_items.MoveNext();

    public void Reset() => throw new NotImplementedException();
  }
}
