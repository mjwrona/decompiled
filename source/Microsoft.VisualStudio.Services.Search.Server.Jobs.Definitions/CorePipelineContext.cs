// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CorePipelineContext
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public abstract class CorePipelineContext : IDisposable
  {
    private readonly Stopwatch m_timer;
    private readonly TimeSpan m_maxPipelineExecutionTime;
    private bool m_disposedValue;

    protected CorePipelineContext(
      IndexingUnit indexingUnit,
      CoreIndexingExecutionContext indexingExecutionContext,
      TimeSpan maxPipelineExecutionTime)
    {
      this.IndexingUnit = indexingUnit;
      this.IndexingExecutionContext = indexingExecutionContext;
      this.m_maxPipelineExecutionTime = maxPipelineExecutionTime;
      this.PropertyBag = (IDictionary<string, object>) new FriendlyDictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.m_timer = new Stopwatch();
    }

    public void MarkStartOfPipelineExecution()
    {
      if (this.m_timer.IsRunning)
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Timer was already started {0} seconds ago. Restarting the timer is not supported.", (object) this.m_timer.Elapsed.TotalSeconds)));
      this.m_timer.Start();
    }

    public TimeSpan RemainingExecutionTime => this.m_maxPipelineExecutionTime - this.m_timer.Elapsed;

    public virtual IndexingUnit IndexingUnit { get; }

    public virtual CoreIndexingExecutionContext IndexingExecutionContext { get; }

    public IDictionary<string, object> PropertyBag { get; private set; }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      this.PropertyBag = (IDictionary<string, object>) null;
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
