// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.AdjustableSemaphoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class AdjustableSemaphoreService : 
    IAdjustableSemaphoreService,
    IVssFrameworkService,
    IDisposable
  {
    private ConcurrentDictionary<string, AdjustableSemaphore> m_namedAdjustableSemaphores;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_namedAdjustableSemaphores = new ConcurrentDictionary<string, AdjustableSemaphore>((IEqualityComparer<string>) StringComparer.Ordinal);

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.Dispose(true);

    public AdjustableSemaphore CreateOrUpdate(string name, int maxCount)
    {
      AdjustableSemaphore addValue = new AdjustableSemaphore(maxCount);
      AdjustableSemaphore orUpdate = this.m_namedAdjustableSemaphores.AddOrUpdate(name, addValue, (Func<string, AdjustableSemaphore, AdjustableSemaphore>) ((_name, existingSemaphore) =>
      {
        existingSemaphore.MaxCount = maxCount;
        return existingSemaphore;
      }));
      if (addValue != orUpdate)
        addValue.Dispose();
      return orUpdate;
    }

    public bool TryGetSemaphore(string name, out AdjustableSemaphore semaphore) => this.m_namedAdjustableSemaphores.TryGetValue(name, out semaphore);

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_namedAdjustableSemaphores == null)
        return;
      if (disposing)
      {
        foreach (KeyValuePair<string, AdjustableSemaphore> adjustableSemaphore in this.m_namedAdjustableSemaphores)
          adjustableSemaphore.Value?.Dispose();
      }
      this.m_namedAdjustableSemaphores.Clear();
      this.m_namedAdjustableSemaphores = (ConcurrentDictionary<string, AdjustableSemaphore>) null;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
