// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssMailSender
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class VssMailSender : IDisposable
  {
    public SendCompletedEventHandler SendCompleted;
    private SimpleTraceQueue m_tracer;
    protected int m_failures;

    protected VssMailSender(SimpleTraceQueue tracer = null)
    {
      this.SetTracer(tracer);
      this.Configure(20000, 3);
    }

    protected VssMailSender(int sendTimeout, int maxFailures, SimpleTraceQueue tracer = null)
    {
      this.SetTracer(tracer);
      this.Configure(sendTimeout, maxFailures);
    }

    public void Configure(int sendTimeout, int maxFailures)
    {
      this.SendTimeout = sendTimeout;
      this.MaxFailures = maxFailures;
    }

    protected void SetTracer(SimpleTraceQueue tracer = null) => this.m_tracer = tracer ?? new SimpleTraceQueue();

    public bool ForceTimeout { get; set; }

    public async Task SendMailAsync(
      MailSenderMessageContext context,
      CancellationToken cancellationToken)
    {
      VssMailSender vssMailSender1 = this;
      Task task = (Task) null;
      try
      {
        vssMailSender1.m_tracer.Trace(1001031, TraceLevel.Info, "Starting send {0} via {1} Sender Obj {2}. ForceTimeout flag is {3}", (object) context.GetMessageId(), (object) context.MailServer, (object) vssMailSender1.GetHashCode(), (object) vssMailSender1.ForceTimeout);
        task = !context.IsL2TestMessage() ? vssMailSender1.DoSendMailAsync(context, cancellationToken) : Task.Delay(5);
        await task.ConfigureAwait(false);
        vssMailSender1.OnSendCompleted(new AsyncCompletedEventArgs((Exception) null, false, (object) context));
        task = (Task) null;
      }
      catch (Exception ex)
      {
        vssMailSender1.m_tracer.Trace(1001032, TraceLevel.Error, "Exception for {0} via {1}: {2}", (object) vssMailSender1.GetHashCode(), (object) context.MailServer, (object) ex.ToReadableStackTrace());
        VssMailSender vssMailSender2 = vssMailSender1;
        Exception error = ex;
        Task task1 = task;
        int num = task1 != null ? (task1.IsCanceled ? 1 : 0) : 0;
        MailSenderMessageContext userState = context;
        AsyncCompletedEventArgs e = new AsyncCompletedEventArgs(error, num != 0, (object) userState);
        vssMailSender2.OnSendCompleted(e);
        task = (Task) null;
      }
    }

    public virtual void Dispose()
    {
    }

    public bool ShouldReuse => this.MaxFailures > this.m_failures;

    public int MaxFailures { get; private set; }

    public int SendTimeout { get; private set; }

    public SimpleTraceQueue Tracer => this.m_tracer;

    protected abstract Task DoSendMailAsync(
      MailSenderMessageContext context,
      CancellationToken cancellationToken);

    private void OnSendCompleted(AsyncCompletedEventArgs e)
    {
      MailSenderMessageContext userState = e.UserState as MailSenderMessageContext;
      this.m_tracer.Trace(1001031, TraceLevel.Info, "Send completed for {0} via {1}", (object) this.GetHashCode(), (object) userState?.MailServer);
      if (e.Cancelled)
        this.m_tracer.Trace(1001031, TraceLevel.Error, "Send cancelled for {0} via {1}", (object) this.GetHashCode(), (object) userState?.MailServer);
      if (e.Error != null)
      {
        this.m_tracer.Trace(1001032, TraceLevel.Error, "Exception for {0} via {1}: {2}", (object) this.GetHashCode(), (object) userState?.MailServer, (object) e.Error.ToReadableStackTrace());
        ++this.m_failures;
      }
      SendCompletedEventHandler sendCompleted = this.SendCompleted;
      if (sendCompleted == null)
        return;
      sendCompleted((object) this, e);
    }
  }
}
