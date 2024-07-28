// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ResourceReferenceComparer`2
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public abstract class ResourceReferenceComparer<TId, TResource> : IEqualityComparer<TResource> where TResource : ResourceReference
  {
    private readonly IEqualityComparer<TId> m_idComparer;

    protected ResourceReferenceComparer(IEqualityComparer<TId> idComparer) => this.m_idComparer = idComparer;

    public abstract TId GetId(TResource resource);

    public bool Equals(TResource left, TResource right)
    {
      if ((object) left == null && (object) right == null)
        return true;
      return (object) left != null && (object) right != null && this.m_idComparer.Equals(this.GetId(left), this.GetId(right));
    }

    public int GetHashCode(TResource obj) => this.GetId(obj).GetHashCode();
  }
}
