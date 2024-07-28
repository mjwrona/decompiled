// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.TEST_SortedObjectOrderer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal class TEST_SortedObjectOrderer : IObjectOrderer
  {
    private List<Sha1Id> m_order = new List<Sha1Id>();

    public void EnqueueObject(Sha1Id objectId, IEnumerable<Sha1Id> outNeighbors) => this.m_order.Add(objectId);

    public List<Sha1Id> DequeueAll()
    {
      List<Sha1Id> order = this.m_order;
      this.m_order = new List<Sha1Id>();
      order.Sort();
      return order;
    }
  }
}
