// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.SshSocketListener
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  public class SshSocketListener : IDisposable
  {
    private readonly object _sync = new object();
    private Task _listenerTask;
    private SshSessionManager _sessionManager;
    private IVssDeploymentServiceHost _serviceHost;
    private CancellationTokenSource _cancellationTokenSource;
    private const string s_Area = "Ssh";
    private const string s_Layer = "SshSocketListener";
    private const string s_FeatureFlagPath = "/FeatureAvailability/Entries/VisualStudio.Services.Ssh.SocketListener/AvailabilityState";
    private bool _listenForSockets = true;
    private readonly TimeSpan _listenerTaskJoinTimeout = new TimeSpan(0, 0, 0, 0, 500);

    public SshSocketListener(
      SshSessionManager sessionManager,
      IVssDeploymentServiceHost serviceHost)
    {
      this._sessionManager = sessionManager;
      this._serviceHost = serviceHost;
      this.RegisterFeatureFlagNotification();
    }

    public void Start(IVssRequestContext deploymentContext)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(13000705, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Enter Start");
        this._cancellationTokenSource = new CancellationTokenSource();
        int port = deploymentContext.GetService<ISshOptionsService>().Options.Port;
        if (!this.IsTcpPortAvailable(port))
          throw new PortNotAvailableException(string.Format("Port {0} is not available on this machine.", (object) port));
        this._listenerTask = Task.Run((Action) (() => this.Listen(port, this._cancellationTokenSource.Token)), this._cancellationTokenSource.Token);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13000708, "Ssh", nameof (SshSocketListener), ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(13000710, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Leave Start");
      }
    }

    public void Dispose()
    {
      TeamFoundationTracingService.TraceRaw(13000746, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Enter Dipose");
      try
      {
        if (this._listenerTask != null)
        {
          this._cancellationTokenSource.Cancel();
          try
          {
            if (!this._listenerTask.Wait(this._listenerTaskJoinTimeout))
              TeamFoundationTracingService.TraceRaw(13000702, TraceLevel.Warning, "Ssh", nameof (SshSocketListener), "Listen thread took more time than anticipated for cancellation.");
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(13000701, "Ssh", nameof (SshSocketListener), ex);
          }
          this._listenerTask.Dispose();
          this._listenerTask = (Task) null;
          this._cancellationTokenSource.Dispose();
        }
        this.UnRegisterFeatureFlagNotification();
        lock (this._sync)
        {
          if (this._sessionManager == null)
            return;
          this._sessionManager.Dispose();
          this._sessionManager = (SshSessionManager) null;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13000748, "Ssh", nameof (SshSocketListener), ex);
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(13000750, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Leave Dipose");
      }
    }

    private void Listen(int port, CancellationToken cancellationToken)
    {
      TeamFoundationTracingService.TraceRaw(13000711, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Enter Listen");
      TcpListener socketListener = TcpListener.Create(port);
      socketListener.ExclusiveAddressUse = true;
      socketListener.Start();
      while (this._sessionManager != null && this._listenForSockets)
      {
        Socket socket = (Socket) null;
        try
        {
          cancellationToken.ThrowIfCancellationRequested();
          using (IVssRequestContext systemContext = this._serviceHost.CreateSystemContext())
            this._listenForSockets = systemContext.IsFeatureEnabled("VisualStudio.Services.Ssh.SocketListener");
          if (!this._listenForSockets)
          {
            TeamFoundationTracingService.TraceRaw(13000714, TraceLevel.Info, "Ssh", nameof (SshSocketListener), "Feature flag has been turned off. Breaking out of listen loop, before accepting socket.");
            break;
          }
          socket = this.AcceptSocket(socketListener, cancellationToken);
          using (IVssRequestContext systemContext = this._serviceHost.CreateSystemContext())
          {
            this._listenForSockets = systemContext.IsFeatureEnabled("VisualStudio.Services.Ssh.SocketListener");
            if (this._listenForSockets)
            {
              lock (this._sync)
              {
                if (this._sessionManager != null)
                {
                  this._sessionManager.AcceptSocket(systemContext, socket);
                  socket = (Socket) null;
                }
              }
            }
            else
              TeamFoundationTracingService.TraceRaw(13000715, TraceLevel.Info, "Ssh", nameof (SshSocketListener), "Feature flag has been turned off. Breaking out of listen loop, and closing socket.");
          }
        }
        catch (System.OperationCanceledException ex)
        {
          TeamFoundationTracingService.TraceRaw(13000712, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Exiting SSH listener loop due to task being cancelled");
          break;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(13000713, "Ssh", nameof (SshSocketListener), ex);
        }
        finally
        {
          socket?.Dispose();
        }
      }
      TeamFoundationTracingService.TraceRaw(13000716, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Stopping SocketListener");
      socketListener.Stop();
      TeamFoundationTracingService.TraceRaw(13000719, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Leave Listen");
    }

    private Socket AcceptSocket(TcpListener socketListener, CancellationToken cancellationToken)
    {
      TeamFoundationTracingService.TraceRaw(13000720, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Enter AcceptSocket");
      try
      {
        Task<Socket> task = socketListener.AcceptSocketAsync();
        task.Wait(cancellationToken);
        Socket result = task.Result;
        try
        {
          result.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, (object) new LingerOption(true, 300));
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(13001042, TraceLevel.Error, "Ssh", nameof (SshSocketListener), "Setting socket Linger options failed. Exception: {0}", (object) ex.ToString());
        }
        return result;
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(13000725, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Leave AcceptSocket");
      }
    }

    private bool IsTcpPortAvailable(int portNumber)
    {
      try
      {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
          socket.Bind((EndPoint) new IPEndPoint(IPAddress.Any, portNumber));
          return true;
        }
      }
      catch (SocketException ex)
      {
        TeamFoundationTracingService.TraceRaw(13000770, TraceLevel.Warning, "Ssh", nameof (SshSocketListener), ex.Message);
        return false;
      }
    }

    private void RegisterFeatureFlagNotification()
    {
      TeamFoundationTracingService.TraceRaw(13000760, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "Registering feature flag notification with Registry Service.");
      using (IVssRequestContext systemContext = this._serviceHost.CreateSystemContext())
        systemContext.GetService<IVssRegistryService>().RegisterNotification(systemContext, new RegistrySettingsChangedCallback(this.OnFeatureFlagStateChanged), "/FeatureAvailability/Entries/VisualStudio.Services.Ssh.SocketListener/AvailabilityState");
    }

    private void OnFeatureFlagStateChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection entries)
    {
      RegistryEntry entry;
      if (!entries.TryGetValue("/FeatureAvailability/Entries/VisualStudio.Services.Ssh.SocketListener/AvailabilityState", out entry) || !string.Equals(entry.Value, "1"))
        return;
      TeamFoundationTracingService.TraceRaw(13000762, TraceLevel.Info, "Ssh", nameof (SshSocketListener), "Received notification, SocketListener is turned on. Starting listener task.");
      if (this._listenForSockets)
        return;
      this._listenForSockets = true;
      this.Start(requestContext);
    }

    private void UnRegisterFeatureFlagNotification()
    {
      TeamFoundationTracingService.TraceRaw(13000761, TraceLevel.Verbose, "Ssh", nameof (SshSocketListener), "UnRegistering feature flag notification with Registry Service.");
      using (IVssRequestContext systemContext = this._serviceHost.CreateSystemContext())
        systemContext.GetService<IVssRegistryService>().UnregisterNotification(systemContext, new RegistrySettingsChangedCallback(this.OnFeatureFlagStateChanged));
    }
  }
}
