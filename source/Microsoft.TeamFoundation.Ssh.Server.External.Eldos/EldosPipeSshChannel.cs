// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.EldosPipeSshChannel
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using SBSSHCommon;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public sealed class EldosPipeSshChannel : IPipeSshChannel, IDisposable
  {
    private readonly ITraceRequest m_tracer;
    private readonly TElSSHTunnelConnection m_connection;
    private readonly CancellationTokenSource m_inCancellationSrc;
    private readonly CancellationTokenSource m_outCancellationSrc;
    private TaskCompletionSource<bool> m_disposed;
    private static readonly VssPerformanceCounter s_bytesReadCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Ssh.BytesReadFromClientPerSec");
    private static readonly VssPerformanceCounter s_bytesWrittenCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Ssh.BytesWrittenToClientPerSec");
    private static readonly PipeOptions s_pipeOptions = new PipeOptions((MemoryPool<byte>) null, (PipeScheduler) null, (PipeScheduler) null, 2097152L, 1048576L, -1, true);
    private const string c_layer = "EldosPipeSshChannel";

    public EldosPipeSshChannel(
      ITraceRequest tracer,
      TElSSHTunnelConnection connection,
      CancellationToken cancellation)
    {
      this.m_tracer = tracer;
      this.m_connection = connection;
      this.m_inCancellationSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
      this.m_outCancellationSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
      this.DataIn = this.CreateBaseInPipe(nameof (DataIn), (Action<TSSHDataEvent>) (onData => this.m_connection.OnData += onData));
      this.ExtendedDataIn = this.CreateBaseInPipe(nameof (ExtendedDataIn), (Action<TSSHDataEvent>) (onData => this.m_connection.OnExtendedData += onData));
      (this.DataOut, this.DataOutCompletion) = this.CreateBaseOutPipe(nameof (DataOut), new Action<byte[], int, int>(this.m_connection.SendData));
      (this.ExtendedDataOut, this.ExtendedDataOutCompletion) = this.CreateBaseOutPipe(nameof (ExtendedDataOut), new Action<byte[], int, int>(this.m_connection.SendExtendedData));
      this.EofSent = WaitForOutCompletionThenSendEofAsync();
      this.m_connection.OnError += new TSSHErrorEvent(this.Connection_OnError);
      this.m_connection.OnEOF += new TSSHEOFEvent(this.Connection_OnEOF);
      this.m_connection.OnClose += new TSSHChannelCloseEvent(this.Connection_OnClose);

      async Task WaitForOutCompletionThenSendEofAsync()
      {
        await this.DataOutCompletion;
        await this.ExtendedDataOutCompletion;
        this.m_outCancellationSrc.Token.ThrowIfCancellationRequested();
        this.m_connection.CloseLocal(true);
      }
    }

    private void Connection_OnError(object sender, int errorCode) => this.m_tracer.Trace(13001440, TraceLevel.Error, "Ssh", nameof (EldosPipeSshChannel), string.Format("{0}: {1}", (object) "OnError", (object) errorCode));

    private void Connection_OnEOF(object sender, ref bool closeChannel) => this.m_inCancellationSrc.Cancel();

    private void Connection_OnClose(object sender, TSSHCloseType closeType) => this.CancelAllBlocks();

    private void CancelAllBlocks()
    {
      this.m_inCancellationSrc.Cancel();
      this.m_outCancellationSrc.Cancel();
    }

    public PipeReader DataIn { get; }

    public PipeReader ExtendedDataIn { get; }

    public PipeWriter DataOut { get; }

    public Task DataOutCompletion { get; }

    public PipeWriter ExtendedDataOut { get; }

    public Task ExtendedDataOutCompletion { get; }

    public Task EofSent { get; }

    public int ExitStatus { get; set; }

    public void Dispose()
    {
      TaskCompletionSource<bool> completionSource = Interlocked.CompareExchange<TaskCompletionSource<bool>>(ref this.m_disposed, new TaskCompletionSource<bool>(), (TaskCompletionSource<bool>) null);
      if (completionSource != null)
      {
        completionSource.Task.Wait();
      }
      else
      {
        this.m_connection.OnEOF -= new TSSHEOFEvent(this.Connection_OnEOF);
        this.m_connection.OnClose -= new TSSHChannelCloseEvent(this.Connection_OnClose);
        try
        {
          this.m_connection.Close(this.ExitStatus, true);
        }
        catch (Exception ex)
        {
          this.m_tracer.TraceException(13002011, TraceLevel.Error, "Ssh", nameof (EldosPipeSshChannel), ex);
        }
        this.CancelAllBlocks();
        try
        {
          Task.WaitAll(this.DataOutCompletion, this.ExtendedDataOutCompletion, this.EofSent);
        }
        catch (AggregateException ex1)
        {
          try
          {
            SshAsyncUtil.RethrowNonCanceled(this.m_tracer, 13002012, nameof (EldosPipeSshChannel), ex1);
          }
          catch (Exception ex2)
          {
            this.m_tracer.TraceException(13002012, TraceLevel.Error, "Ssh", nameof (EldosPipeSshChannel), ex2);
          }
        }
        this.m_inCancellationSrc.Dispose();
        this.m_outCancellationSrc.Dispose();
        this.m_disposed.SetResult(true);
        this.m_tracer.Trace(13001072, TraceLevel.Info, "Ssh", nameof (EldosPipeSshChannel), "Pipe ssh channel disposed");
      }
    }

    private (EldosPipeSshChannel channel, string pipeName) CaptureDebugInfo(string pipeName) => (this, pipeName);

    private PipeReader CreateBaseInPipe(string pipeName, Action<TSSHDataEvent> attach)
    {
      Pipe pipe = new Pipe(EldosPipeSshChannel.s_pipeOptions);
      PipeWriter writer = pipe.Writer;
      this.m_inCancellationSrc.Token.Register((Action) (() => writer.Complete()), false);
      attach((TSSHDataEvent) ((sender, buffer) =>
      {
        this.CaptureDebugInfo(pipeName);
        if (writer.WriteAsync((ReadOnlyMemory<byte>) buffer, new CancellationToken()).AsTask().GetAwaiter().GetResult().IsCanceled)
          throw new System.OperationCanceledException("FlushResult");
        EldosPipeSshChannel.s_bytesReadCounter.IncrementBy((long) buffer.Length);
      }));
      return pipe.Reader;
    }

    private (PipeWriter writer, Task readerCompletion) CreateBaseOutPipe(
      string propertyName,
      Action<byte[], int, int> sendData)
    {
      Pipe pipe = new Pipe(EldosPipeSshChannel.s_pipeOptions);
      PipeReader reader = pipe.Reader;
      return (pipe.Writer, ReadPipeAsync());

      async Task ReadPipeAsync()
      {
        this.CaptureDebugInfo(propertyName);
        try
        {
          byte[] buf = new byte[32768];
          long num;
          long length1;
          do
          {
            ReadResult readResult;
            ReadOnlySequence<byte> readOnlySequence;
            long length2;
            do
            {
              CancellationToken token = this.m_outCancellationSrc.Token;
              token.ThrowIfCancellationRequested();
              int delayMillis = 2;
              while (!this.m_connection.CanSend())
              {
                await Task.Delay(delayMillis);
                delayMillis = Math.Min(delayMillis + delayMillis >> 1, 10000);
                token = this.m_outCancellationSrc.Token;
                token.ThrowIfCancellationRequested();
              }
              readResult = await reader.ReadAsync(this.m_outCancellationSrc.Token);
              readOnlySequence = !readResult.IsCanceled ? readResult.Buffer : throw new System.OperationCanceledException("ReadResult");
              length2 = Math.Min(readOnlySequence.Length, (long) buf.Length);
              readOnlySequence = readResult.Buffer;
              ReadOnlySequence<byte> source = readOnlySequence.Slice(0L, length2);
              source.CopyTo<byte>((Span<byte>) buf);
              sendData(buf, 0, checked ((int) length2));
              reader.AdvanceTo(source.End);
              EldosPipeSshChannel.s_bytesWrittenCounter.IncrementBy(length2);
            }
            while (!readResult.IsCompleted);
            num = length2;
            readOnlySequence = readResult.Buffer;
            length1 = readOnlySequence.Length;
          }
          while (num != length1);
          buf = (byte[]) null;
        }
        catch (Exception ex)
        {
          pipe.Reader.Complete(ex);
          throw;
        }
      }
    }
  }
}
