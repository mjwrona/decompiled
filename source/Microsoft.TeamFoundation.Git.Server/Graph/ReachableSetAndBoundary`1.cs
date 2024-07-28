// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.ReachableSetAndBoundary`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  public struct ReachableSetAndBoundary<T>
  {
    private ISet<T> m_reachable;
    private ISet<T> m_boundary;

    public ReachableSetAndBoundary(ISet<T> reachable, ISet<T> boundary)
    {
      this.m_reachable = reachable;
      this.m_boundary = boundary;
    }

    public IEnumerable<T> ReachableSet => (IEnumerable<T>) this.m_reachable;

    public IEnumerable<T> Boundary => (IEnumerable<T>) this.m_boundary;
  }
}
