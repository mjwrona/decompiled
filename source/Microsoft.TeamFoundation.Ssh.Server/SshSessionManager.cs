// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.SshSessionManager
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders;
using Microsoft.TeamFoundation.Ssh.Server.ExtensibilityProviders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  public class SshSessionManager : IDisposable
  {
    private readonly ITunneledCommandExtensionProvider _extensionProvider;
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly ISshSessionFactory _sessionFactory;
    private readonly IVssRequestContextProvider _requestContextProvider;
    private ConcurrentDictionary<ISshSession, bool> _sessions;
    private static readonly VssPerformanceCounter s_activeConnections = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Ssh.ActiveConnections");
    private const string s_Area = "Ssh";
    private const string s_Layer = "SshSessionManager";

    public static SshSessionManager Create(IVssRequestContext systemRequestContext)
    {
      ITunneledCommandExtensionProvider extensionProvider = (ITunneledCommandExtensionProvider) new TunneledCommandExtensionProvider();
      extensionProvider.Initialize(systemRequestContext);
      return new SshSessionManager((IVssRequestContextProvider) new RequestContextProvider(systemRequestContext.ServiceHost.DeploymentServiceHost), extensionProvider, (IAuthenticationProvider) new AuthenticationProvider(), (ISshSessionFactory) new SshSessionFactory());
    }

    internal SshSessionManager(
      IVssRequestContextProvider requestContextProvider,
      ITunneledCommandExtensionProvider extensionProvider,
      IAuthenticationProvider authenticationProvider,
      ISshSessionFactory sessionFactory)
    {
      this._requestContextProvider = requestContextProvider;
      this._extensionProvider = extensionProvider;
      this._authenticationProvider = authenticationProvider;
      this._sessionFactory = sessionFactory;
      this._sessions = new ConcurrentDictionary<ISshSession, bool>();
    }

    public void AcceptSocket(IVssRequestContext deploymentRequestContext, Socket socket)
    {
      using (IVssRequestContext deploymentContext = this._requestContextProvider.CreateSshDeploymentContext(deploymentRequestContext, string.Empty))
      {
        MethodInformation methodInformation = new MethodInformation("SshSessionManager.AcceptSocket", MethodType.Normal, EstimatedMethodCost.Moderate);
        deploymentContext.EnterMethod(methodInformation);
        try
        {
          this.AcceptSocketInternal(socket, deploymentContext);
        }
        catch (Exception ex)
        {
          deploymentContext.TraceException(13000501, "Ssh", nameof (SshSessionManager), ex);
          deploymentContext.Status = ex;
        }
        finally
        {
          deploymentContext.LeaveMethod();
        }
      }
    }

    private void AcceptSocketInternal(Socket socket, IVssRequestContext systemContext)
    {
      SshOptions options = systemContext.GetService<ISshOptionsService>().Options;
      int count = this._sessions.Count;
      if (count >= options.MaxConcurrentConnections)
      {
        string message = string.Format("Reached max concurrent active sessions, closing socket. Current active sessions: {0} Max allowed concurrent sessions: {1}", (object) count, (object) options.MaxConcurrentConnections);
        systemContext.Trace(13000502, TraceLevel.Warning, "Ssh", nameof (SshSessionManager), message);
        socket.Dispose();
      }
      else
      {
        ISshSession key = this._sessionFactory.Create();
        key.SessionClosed += new SessionClosedHandler(this.OnSessionClosed);
        SshSessionManager.s_activeConnections.Increment();
        ((IDictionary<ISshSession, bool>) this._sessions).Add(key, false);
        key.StartSession(this._requestContextProvider, socket, options, this._authenticationProvider, this._extensionProvider);
      }
    }

    public void Dispose()
    {
      foreach (KeyValuePair<ISshSession, bool> session in this._sessions)
        session.Key.Dispose();
    }

    private void OnSessionClosed(ISshSession sender)
    {
      TeamFoundationTracingService.TraceRaw(13000510, TraceLevel.Verbose, "Ssh", nameof (SshSessionManager), "Enter OnSessionClosed");
      this._sessions.TryRemove(sender, out bool _);
      SshSessionManager.s_activeConnections.Decrement();
      TeamFoundationTracingService.TraceRaw(13000512, TraceLevel.Verbose, "Ssh", nameof (SshSessionManager), "Leave OnSessionClosed");
    }

    internal ICollection<ISshSession> TEST_sessions => this._sessions.Keys;
  }
}
