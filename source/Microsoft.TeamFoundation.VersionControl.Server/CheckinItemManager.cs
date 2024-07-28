// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinItemManager
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CheckinItemManager : 
    List<ItemPathPair>,
    ICheckinItemManager,
    IEnumerable<ItemPathPair>,
    IEnumerable,
    IDisposable
  {
    public void Enqueue(ItemPathPair item) => this.Add(item);

    public int ParameterId => throw new NotSupportedException();

    public int TotalCount => this.Count;

    public void Flush(bool isLastPage) => throw new NotSupportedException();

    public List<ItemPathPair> FirstPage => (List<ItemPathPair>) this;

    public void Dispose()
    {
    }
  }
}
