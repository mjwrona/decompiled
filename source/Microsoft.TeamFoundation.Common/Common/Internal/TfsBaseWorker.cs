// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TfsBaseWorker
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;
using System.Threading;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TfsBaseWorker
  {
    private object m_identifier;
    private ManualResetEvent m_backgroundWorkCompleted;

    public TfsBaseWorker(object identifier)
    {
      this.m_identifier = identifier;
      this.m_backgroundWorkCompleted = new ManualResetEvent(false);
    }

    public abstract object DoWork(object argument, CancelEventArgs e);

    public abstract void WorkCompleted(object argument, object result, AsyncCompletedEventArgs e);

    public object Identifier => this.m_identifier;

    public ManualResetEvent WaitHandle => this.m_backgroundWorkCompleted;

    public virtual bool PendingCancellation { get; set; }
  }
}
