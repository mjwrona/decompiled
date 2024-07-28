// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.EldosSshCommandClient2
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using SBSSHClient;
using SBSSHCommon;
using SBSSHKeyStorage;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public class EldosSshCommandClient2 : IDisposable
  {
    private readonly RawTraceRequest m_tracer;
    private readonly SshOptions m_options;
    private readonly string m_keySha1Fingerprint;
    private readonly TElSSHClient m_sshClient;
    private readonly CancellationTokenSource m_disposeCancellationSrc;
    private readonly TaskCompletionSource<IPipeSshChannel> m_channelSrc;
    private SocketFacade m_socketFacade;
    private Task m_lifetimeTask;
    private const string c_layer = "EldosSshCommandClient2";

    public EldosSshCommandClient2(
      IVssRequestContext rc,
      TElSSHClient sshClient,
      SshOptions options)
    {
      this.m_tracer = new RawTraceRequest(rc.E2EId);
      this.m_options = options;
      this.m_keySha1Fingerprint = rc.GetService<TeamFoundationSshKeyService>().GetSha1KeyFingerprint(rc);
      this.m_sshClient = sshClient;
      this.m_sshClient.OnKeyValidate += new TSSHKeyValidateEvent(this.SSHClient_OnKeyValidate);
      this.m_sshClient.OnSend += new TSSHSendEvent(this.SSHClient_OnSend);
      this.m_sshClient.OnReceive += new TSSHReceiveEvent(this.SSHClient_OnReceive);
      this.m_sshClient.OnCloseConnection += new TSSHCloseConnectionEvent(this.SSHClient_OnCloseConnection);
      this.m_sshClient.OnError += new TSSHErrorEvent(this.SSHClient_OnError);
      this.m_disposeCancellationSrc = new CancellationTokenSource();
      this.m_channelSrc = new TaskCompletionSource<IPipeSshChannel>();
    }

    public void RunCommand(
      string host,
      int port,
      string username,
      string password,
      string command,
      Action<IPipeSshChannel> action)
    {
      this.m_tracer.TraceEnter(13001430, "Ssh", nameof (EldosSshCommandClient2), nameof (RunCommand));
      try
      {
        this.m_sshClient.UserName = username;
        this.m_sshClient.Password = password;
        TElSSHTunnelList telSshTunnelList = new TElSSHTunnelList();
        TElCommandSSHTunnel commandSshTunnel = new TElCommandSSHTunnel();
        commandSshTunnel.Command = command;
        commandSshTunnel.RequestTerminal = false;
        commandSshTunnel.TunnelList = telSshTunnelList;
        commandSshTunnel.OnOpen += (TTunnelEvent) ((sender, tunnelConnection) =>
        {
          this.m_tracer.Trace(13001408, TraceLevel.Info, "Ssh", nameof (EldosSshCommandClient2), "tunnel.OnOpen");
          this.m_channelSrc.SetResult((IPipeSshChannel) new EldosPipeSshChannel((ITraceRequest) this.m_tracer, tunnelConnection, this.m_disposeCancellationSrc.Token));
        });
        commandSshTunnel.OnClose += (TTunnelEvent) ((sender, tunnelConnection) => this.m_tracer.Trace(13001412, TraceLevel.Info, "Ssh", nameof (EldosSshCommandClient2), "tunnel.OnClose"));
        commandSshTunnel.OnError += (TTunnelErrorEvent) ((sender, error, data) => this.m_tracer.Trace(13001425, TraceLevel.Error, "Ssh", nameof (EldosSshCommandClient2), "tunnel.OnError: " + ErrorCodes.GetMessage(error)));
        this.m_sshClient.TunnelList = telSshTunnelList;
        this.m_socketFacade = this.ConnectSocket(host, port);
        this.m_lifetimeTask = this.StartLifetimeTask();
        using (IPipeSshChannel result = this.m_channelSrc.Task.Result)
          action(result);
      }
      finally
      {
        this.m_tracer.TraceLeave(13001431, "Ssh", nameof (EldosSshCommandClient2), nameof (RunCommand));
      }
    }

    private async Task StartLifetimeTask()
    {
      this.m_tracer.TraceEnter(13001432, "Ssh", nameof (EldosSshCommandClient2), nameof (StartLifetimeTask));
      try
      {
        this.m_sshClient.Open();
        await this.m_socketFacade.StartReadLoop(new Action(this.m_sshClient.DataAvailable));
      }
      catch (Exception ex)
      {
        if (ex is SshNoMoreAuthMethodsAvailableException)
          throw;
        else
          this.m_tracer.TraceException(13001433, TraceLevel.Error, "Ssh", nameof (EldosSshCommandClient2), ex);
      }
      finally
      {
        this.m_disposeCancellationSrc.Cancel();
        this.m_channelSrc.TrySetCanceled();
        if (this.m_channelSrc.Task.Status == TaskStatus.RanToCompletion)
          this.m_channelSrc.Task.Result.Dispose();
        this.m_socketFacade?.Dispose();
        this.m_tracer.TraceLeave(13001434, "Ssh", nameof (EldosSshCommandClient2), nameof (StartLifetimeTask));
      }
    }

    private SocketFacade ConnectSocket(string host, int port)
    {
      this.m_tracer.TraceEnter(13001428, "Ssh", nameof (EldosSshCommandClient2), nameof (ConnectSocket));
      Socket socket = (Socket) null;
      try
      {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(host, port);
        SocketFacade socketFacade = new SocketFacade(socket, this.m_options.SessionTimeout, this.m_disposeCancellationSrc.Token);
        socket = (Socket) null;
        return socketFacade;
      }
      finally
      {
        socket?.Dispose();
        this.m_tracer.TraceLeave(13001429, "Ssh", nameof (EldosSshCommandClient2), nameof (ConnectSocket));
      }
    }

    private void SSHClient_OnCloseConnection(object sender)
    {
      this.m_tracer.Trace(13001406, TraceLevel.Info, "Ssh", nameof (EldosSshCommandClient2), nameof (SSHClient_OnCloseConnection));
      this.m_disposeCancellationSrc.Cancel();
    }

    private void SSHClient_OnError(object sender, int errorCode)
    {
      this.m_tracer.Trace(13001426, TraceLevel.Error, "Ssh", nameof (EldosSshCommandClient2), "SSHClient_OnError: " + ErrorCodes.GetMessage(errorCode));
      if (errorCode == 114)
        throw new SshNoMoreAuthMethodsAvailableException(ErrorCodes.GetMessage(errorCode));
    }

    private void SSHClient_OnSend(object sender, byte[] buffer) => this.m_socketFacade.Send(buffer);

    private void SSHClient_OnReceive(
      object sender,
      ref byte[] buffer,
      int maxSize,
      out int written)
    {
      written = this.m_socketFacade.Receive(buffer, maxSize);
    }

    private void SSHClient_OnKeyValidate(object sender, TElSSHKey serverKey, ref bool validate)
    {
      this.m_tracer.TraceEnter(13001416, "Ssh", nameof (EldosSshCommandClient2), nameof (SSHClient_OnKeyValidate));
      validate = this.m_keySha1Fingerprint.Equals(serverKey.FingerprintSHA1String, StringComparison.OrdinalIgnoreCase);
      if (validate)
        return;
      this.m_tracer.Trace(13001424, TraceLevel.Error, "Ssh", nameof (EldosSshCommandClient2), "SSH Server keys didn't match so we aren't able to validate the remote server's key. This should never happen!");
    }

    public void Dispose()
    {
      try
      {
        this.m_disposeCancellationSrc.Cancel();
      }
      catch (ObjectDisposedException ex)
      {
      }
      this.m_lifetimeTask?.Wait();
      this.m_sshClient.Dispose();
      this.m_disposeCancellationSrc.Dispose();
      this.m_tracer.Trace(13001074, TraceLevel.Info, "Ssh", nameof (EldosSshCommandClient2), "Command client disposed");
    }
  }
}
