// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.SocketFacade
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  internal class SocketFacade : IDisposable
  {
    private readonly Socket m_socket;
    private readonly TimeSpan m_timeout;
    private readonly CancellationTokenSource m_timeoutCancellationSrc;
    private readonly CancellationTokenSource m_linkedCancellationSrc;
    private TaskCompletionSource<bool> m_disposed;
    private const string c_layer = "SocketFacade";

    public SocketFacade(Socket socket, TimeSpan timeout, CancellationToken cancellation)
    {
      this.m_socket = socket;
      this.m_timeout = timeout;
      this.m_timeoutCancellationSrc = new CancellationTokenSource(this.m_timeout);
      this.m_linkedCancellationSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellation, this.m_timeoutCancellationSrc.Token);
      this.m_linkedCancellationSrc.Token.Register(new Action(this.Dispose), false);
    }

    public void Dispose()
    {
      TaskCompletionSource<bool> completionSource = Interlocked.CompareExchange<TaskCompletionSource<bool>>(ref this.m_disposed, new TaskCompletionSource<bool>(), (TaskCompletionSource<bool>) null);
      if (completionSource != null)
      {
        completionSource.Task.Wait();
      }
      else
      {
        this.m_linkedCancellationSrc.Dispose();
        this.m_timeoutCancellationSrc.Dispose();
        this.m_socket.Dispose();
        this.m_disposed.SetResult(true);
      }
    }

    public Task StartReadLoop(Action onDataAvailable) => Task.Run((Action) (() =>
    {
      while (true)
      {
        int available;
        try
        {
          this.m_socket.Poll(-1, SelectMode.SelectRead);
          available = this.m_socket.Available;
        }
        catch (Exception ex) when (this.IsDueToSocketDisposal(ex))
        {
          this.ThrowIfTimedOut();
          break;
        }
        if (available != 0)
          onDataAvailable();
        else
          break;
      }
    }));

    public int Receive(byte[] buffer, int size)
    {
      int num;
      try
      {
        num = this.m_socket.Receive(buffer, size, SocketFlags.None);
      }
      catch (Exception ex) when (this.IsDueToSocketDisposal(ex))
      {
        this.ThrowIfTimedOut();
        return 0;
      }
      this.ExtendTimeout();
      return num;
    }

    public void Send(byte[] buffer)
    {
      try
      {
        this.m_socket.Send(buffer);
      }
      catch (Exception ex) when (this.IsDueToSocketDisposal(ex))
      {
        this.ThrowIfTimedOut();
        return;
      }
      this.ExtendTimeout();
    }

    private void ExtendTimeout()
    {
      try
      {
        this.m_timeoutCancellationSrc.CancelAfter(this.m_timeout);
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    private bool IsDueToSocketDisposal(Exception ex)
    {
      if (this.m_disposed == null)
        return false;
      return ex is SocketException || ex is ObjectDisposedException;
    }

    private void ThrowIfTimedOut()
    {
      if (this.m_timeoutCancellationSrc.IsCancellationRequested)
        throw new EldosSshSocketException(Resource.SocketClosedAfterReadWriteTimeout);
    }
  }
}
