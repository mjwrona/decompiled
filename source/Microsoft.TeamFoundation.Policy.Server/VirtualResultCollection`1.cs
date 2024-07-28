// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.VirtualResultCollection`1
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class VirtualResultCollection<T> : IDisposable
  {
    private ResultCollection m_rc;

    public VirtualResultCollection()
    {
    }

    public VirtualResultCollection(ResultCollection rc) => this.m_rc = rc;

    public virtual IVssRequestContext RequestContext => this.m_rc.RequestContext;

    public virtual void AddBinder<T2>(ObjectBinder<T2> binder) => this.m_rc.AddBinder<T2>(binder);

    public virtual object GetCurrent() => this.m_rc.GetCurrent();

    public virtual ObjectBinder<T2> GetCurrent<T2>() => this.m_rc.GetCurrent<T2>();

    public virtual IEnumerable<T> GetCurrentAsEnumerable() => (IEnumerable<T>) this.m_rc.GetCurrent<T>();

    public virtual void IncrementRowCounter() => this.m_rc.IncrementRowCounter();

    public virtual void NextResult() => this.m_rc.NextResult();

    public virtual bool TryNextResult() => this.m_rc.TryNextResult();

    public virtual void Dispose()
    {
      if (this.m_rc == null)
        return;
      this.m_rc.Dispose();
      this.m_rc = (ResultCollection) null;
    }
  }
}
