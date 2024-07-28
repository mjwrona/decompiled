// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.EldosSshSession2
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.Serialization;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders;
using Microsoft.VisualStudio.Services.Common;
using SBSSHCommon;
using SBSSHKeyStorage;
using SBSSHServer;
using SBStringList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public class EldosSshSession2 : ISshSession, IDisposable
  {
    internal const string s_Layer = "EldosSshSession2";
    private TElSSHServer EldosSshServer;
    private SshOptions m_options;
    private CryptographicKeyPair m_cryptoKeyPair;
    private GitSshCommandInfo m_commandInfo;
    internal bool m_authenticatedUsingPassword;
    private Task m_lifetimeTask;
    private Task m_commandTask;
    private readonly CancellationTokenSource m_disposeCancellationSrc = new CancellationTokenSource();
    private SocketFacade m_socketFacade;
    private Guid? m_e2eId;
    private RawTraceRequest m_tracer;
    private string m_remoteIp;
    private readonly SshAccessAudit m_sshAccessAudit = new SshAccessAudit((IEventSerializer) new EventSerializer());

    static EldosSshSession2() => EldosInitializer.Init();

    public ITunneledCommandExtensionProvider TunneledCommandExtensionProvider { get; internal set; }

    public IVssRequestContextProvider RequestContextProvider { get; internal set; }

    public IAuthenticationProvider AuthenticationProvider { get; internal set; }

    public event SessionClosedHandler SessionClosed;

    public string ClientHostAddress { get; private set; }

    public string RemoteClientIp => this.m_remoteIp ?? this.ClientHostAddress;

    public string ClientPort { get; private set; }

    public DateTime SessionStartTime { get; private set; }

    public string UserName { get; internal set; }

    public Microsoft.VisualStudio.Services.Identity.Identity SessionUser { get; set; }

    public static byte[] PrepareMessageForStdErr(string msg) => SshEncodings.BestEffortUtf8NoBom.GetBytes("remote: " + msg + "\n");

    public void StartSession(
      IVssRequestContextProvider requestContextProvider,
      Socket socket,
      SshOptions options,
      IAuthenticationProvider authenticationProvider,
      ITunneledCommandExtensionProvider commandExtensionProvider)
    {
      this.RequestContextProvider = requestContextProvider;
      using (IVssRequestContext deploymentContext = this.RequestContextProvider.CreateSystemDeploymentContext())
      {
        deploymentContext.TraceEnter(13001001, "Ssh", nameof (EldosSshSession2), nameof (StartSession));
        try
        {
          this.m_e2eId = new Guid?(deploymentContext.E2EId);
          this.m_tracer = new RawTraceRequest(this.m_e2eId.Value);
          this.m_options = options;
          this.EldosSshServer = TElSSHServerFactory.Create(this.m_options, (ITraceRequest) this.m_tracer);
          this.EldosSshServer.KeyStorage = (TElSSHCustomKeyStorage) deploymentContext.GetService<TeamFoundationSshKeyService>().GetSshServerHostKey(deploymentContext);
          this.SetupEventHandlers();
          this.AuthenticationProvider = authenticationProvider;
          this.TunneledCommandExtensionProvider = commandExtensionProvider;
          this.SessionStartTime = DateTime.UtcNow;
          IPEndPoint remoteEndPoint = (IPEndPoint) socket.RemoteEndPoint;
          this.ClientHostAddress = remoteEndPoint.Address.ToString();
          this.ClientPort = remoteEndPoint.Port.ToString();
          deploymentContext.Trace(13001041, TraceLevel.Verbose, "Ssh", nameof (EldosSshSession2), "Connected to client {0}:{1}", (object) this.ClientHostAddress, (object) this.ClientPort);
          this.m_socketFacade = new SocketFacade(socket, this.m_options.SessionTimeout, this.m_disposeCancellationSrc.Token);
          this.m_lifetimeTask = this.StartLifetimeTask();
        }
        finally
        {
          if (this.m_lifetimeTask == null)
          {
            this.m_socketFacade?.Dispose();
            this.EldosSshServer?.Dispose();
          }
          deploymentContext.TraceLeave(13001009, "Ssh", nameof (EldosSshSession2), nameof (StartSession));
        }
      }
    }

    private async Task StartLifetimeTask()
    {
      EldosSshSession2 sender = this;
      sender.m_tracer.TraceEnter(13001060, "Ssh", nameof (EldosSshSession2), nameof (StartLifetimeTask));
      try
      {
        sender.EldosSshServer.Open();
        await sender.m_socketFacade.StartReadLoop(new Action(sender.EldosSshServer.DataAvailable));
      }
      catch (NullReferenceException ex)
      {
        if (ex.StackTrace.Contains("SBSSHKeyStorage.TElSSHMemoryKeyStorage"))
        {
          if (sender.EldosSshServer.KeyStorage == null && sender.m_disposeCancellationSrc != null && sender.m_disposeCancellationSrc.IsCancellationRequested)
            sender.m_tracer.Trace(13001070, TraceLevel.Warning, "Ssh", nameof (EldosSshSession2), "Ssh key storage (EldosSshServer.KeyStorage) is disposed");
          else
            sender.m_tracer.TraceException(13001071, TraceLevel.Error, "Ssh", nameof (EldosSshSession2), (Exception) ex, "Ssh key storage (EldosSshServer.KeyStorage) is not properly initialized. SSHServerState: {0}, Exception: {1}", (object[]) new string[2]
            {
              sender.EldosSshServer?.IsDisposed.ToString() ?? "null",
              ex.Message
            });
        }
        else
          sender.m_tracer.TraceException(13001073, TraceLevel.Error, "Ssh", nameof (EldosSshSession2), (Exception) ex);
      }
      catch (Exception ex)
      {
        sender.m_tracer.TraceException(13001068, TraceLevel.Error, "Ssh", nameof (EldosSshSession2), ex);
      }
      finally
      {
        sender.m_disposeCancellationSrc.Cancel();
        sender.m_tracer.Trace(13001304, TraceLevel.Verbose, "Ssh", nameof (EldosSshSession2), "Socket is being shutdown for {0} - {1}:{2}", (object) sender.UserName, (object) sender.ClientHostAddress, (object) sender.ClientPort);
        sender.m_socketFacade.Dispose();
        if (sender.m_commandTask != null)
          await (sender.m_commandTask ?? Task.CompletedTask);
        sender.EldosSshServer.Dispose();
        sender.m_disposeCancellationSrc.Dispose();
        SessionClosedHandler sessionClosed = sender.SessionClosed;
        if (sessionClosed != null)
          sessionClosed((ISshSession) sender);
        sender.m_tracer.TraceLeave(13001069, "Ssh", nameof (EldosSshSession2), nameof (StartLifetimeTask));
      }
    }

    internal void ParseCommand(IVssRequestContext requestContext, string command)
    {
      CommandParser.ParseCommand(requestContext, command, this.GetVirtualPathRoot(requestContext), out this.m_commandInfo, this.UserName, this.m_options.Port);
      this.UserName = this.m_commandInfo.Collection;
    }

    internal virtual string GetVirtualPathRoot(IVssRequestContext requestContext) => WebApiConfiguration.GetVirtualPathRoot(requestContext);

    private Task StartCommandTask(IPipeSshChannel channel, string command) => Task.Run((Action) (() =>
    {
      using (IVssRequestContext sshRequestContext = this.CreateSshRequestContext())
      {
        sshRequestContext.TraceEnter(13001240, "Ssh", nameof (EldosSshSession2), nameof (StartCommandTask));
        channel.ExitStatus = 1;
        SshSessionKpiHelper.Increment(sshRequestContext, SshSessionKpiHelper.SshSessionKpi.CommandExecution);
        try
        {
          this.ParseCommand(sshRequestContext, command);
          using (IVssRequestContext deploymentRequestContext = SshRequestContext.CreateSshNonDeploymentRequestContext(sshRequestContext, (this.m_commandInfo.Collection, this.m_commandInfo.Project, this.m_commandInfo.Repo), this.RemoteClientIp, this.m_e2eId))
          {
            this.DoCommandTaskCollectionish(deploymentRequestContext, channel, this.m_commandInfo);
            channel.ExitStatus = deploymentRequestContext.Status != null ? 2 : 0;
          }
        }
        catch (Exception ex1)
        {
          sshRequestContext.TraceException(13001248, "Ssh", nameof (EldosSshSession2), ex1);
          try
          {
            EldosSshSession2.SendBanner(ex1.Message, channel);
          }
          catch (Exception ex2)
          {
            sshRequestContext.TraceException(13001251, "Ssh", nameof (EldosSshSession2), ex2);
          }
        }
        finally
        {
          try
          {
            if (channel.ExitStatus != 0)
              SshSessionKpiHelper.Increment(sshRequestContext, SshSessionKpiHelper.SshSessionKpi.CommandFailure);
            channel.DataOut.Complete();
            channel.ExtendedDataOut.Complete();
            channel.EofSent.GetAwaiter().GetResult();
          }
          catch (Exception ex)
          {
            sshRequestContext.TraceException(13001241, "Ssh", nameof (EldosSshSession2), ex);
          }
          channel.Dispose();
          sshRequestContext.TraceLeave(13001249, "Ssh", nameof (EldosSshSession2), nameof (StartCommandTask));
        }
      }
    }));

    private void DoCommandTaskCollectionish(
      IVssRequestContext collectionishRC,
      IPipeSshChannel channel,
      GitSshCommandInfo commandInfo)
    {
      string webMethodName1 = (string) null;
      try
      {
        bool flag1 = collectionishRC.Items.ContainsKey(RequestContextItemsKeys.HostProxyData);
        webMethodName1 = flag1 ? "proxy." + this.m_commandInfo.CommandName : this.m_commandInfo.CommandName;
        if (!flag1)
          collectionishRC.RootContext.Items[RequestContextItemsKeys.IsActivity] = (object) true;
        bool flag2 = this.m_authenticatedUsingPassword;
        if (this.m_cryptoKeyPair != null && !string.IsNullOrEmpty(this.UserName))
          flag2 = this.IsPublicKeyValid(collectionishRC);
        if (!flag2)
          throw new TeamFoundationServerUnauthorizedException(Resource.SshFailedToAuthenticatePublicKey);
        collectionishRC.To(TeamFoundationHostType.Deployment).GetService<IInternalTeamFoundationHostManagementService>().SetupUserContext(collectionishRC, this.SessionUser.Descriptor);
        AuthenticationHelpers.SetAuthenticationMechanism(collectionishRC, AuthenticationMechanism.SSH);
        if (!collectionishRC.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().IsRequestAuthenticationValid((IVssWebRequestContext) collectionishRC, true))
          throw new TeamFoundationServerUnauthorizedException(Resource.SshUnauthorizedException);
        IEnumerable<ITeamFoundationRequestFilter> requestFilters = collectionishRC.ServiceHost.ServiceHostInternal().RequestFilters.AsEmptyIfNull<ITeamFoundationRequestFilter>();
        collectionishRC.RunSynchronously((Func<Task>) (async () =>
        {
          foreach (ITeamFoundationRequestFilter foundationRequestFilter in requestFilters)
            await foundationRequestFilter.PostAuthenticateRequest(collectionishRC);
        }));
        collectionishRC.EnterMethod(new MethodInformation(webMethodName1, MethodType.Normal, EstimatedMethodCost.Moderate));
        PublicKeyAlgorithm publicKeyAlgorithm = (PublicKeyAlgorithm) this.EldosSshServer.PublicKeyAlgorithm;
        collectionishRC.TraceAlways(13001163, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), string.Format("Proccessing ssh command. PublicKeyAlgorithm:{0}, User:{1}, Proxy: {2}", (object) publicKeyAlgorithm, (object) this.SessionUser.Id, (object) flag1));
        if (!this.m_authenticatedUsingPassword)
        {
          collectionishRC.TraceAlways(13001250, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), string.Format("Ssh URL version:{0}. User:{1}", (object) this.m_commandInfo.CommandVersion, (object) this.SessionUser.Id));
          if (collectionishRC.IsFeatureEnabled("Ssh.SshRsaDeprecation.FailSshRsaTraffic") && publicKeyAlgorithm == PublicKeyAlgorithm.SSH_RSA && EldosSshSession2.IsSshRsaFailingWindowActive(collectionishRC, DateTime.UtcNow))
            this.FailSSHRsaSession(collectionishRC, channel, commandInfo.CommandName);
          if (collectionishRC.IsFeatureEnabled("Ssh.SshRsaDeprecation.ThrottleSshRsaTraffic") && publicKeyAlgorithm == PublicKeyAlgorithm.SSH_RSA)
            this.ThrottleSession(collectionishRC, channel, commandInfo.CommandName);
        }
        if (!flag1)
        {
          ITunneledCommandExtension commandExtension = this.TunneledCommandExtensionProvider.GetCommandExtension(collectionishRC, commandInfo.CommandName);
          PipeSshCommandStreams sshCommandStreams = new PipeSshCommandStreams(channel);
          IVssRequestContext rc = collectionishRC;
          GitSshCommandInfo commandInfo1 = commandInfo;
          PipeSshCommandStreams streams = sshCommandStreams;
          commandExtension.Execute(rc, commandInfo1, (ISshCommandStreams) streams, (ISshSession) this);
        }
        else
          new ForwardedSshCommandExtension().Execute(collectionishRC, commandInfo, channel, (ISshSession) this, this.m_options);
        if (!flag1)
          this.m_sshAccessAudit.AuditAuthorizedSshRequest(collectionishRC, commandInfo);
        collectionishRC.RunSynchronously((Func<Task>) (async () =>
        {
          foreach (ITeamFoundationRequestFilter foundationRequestFilter in requestFilters)
            await foundationRequestFilter.PostLogRequestAsync(collectionishRC);
        }));
      }
      catch (Exception ex1)
      {
        if (ex1?.InnerException is SshNoMoreAuthMethodsAvailableException)
        {
          collectionishRC.TraceAlways(13001245, TraceLevel.Warning, "Ssh", nameof (EldosSshSession2), ex1.ToReadableStackTrace());
          collectionishRC.Status = ex1.InnerException;
        }
        else
        {
          collectionishRC.TraceException(13001245, "Ssh", nameof (EldosSshSession2), ex1);
          collectionishRC.Status = ex1;
        }
        try
        {
          EldosSshSession2.SendBanner(ex1.Message, channel);
        }
        catch (Exception ex2)
        {
          collectionishRC.TraceException(13001244, "Ssh", nameof (EldosSshSession2), ex2);
        }
      }
      finally
      {
        if (collectionishRC.Method == null)
        {
          string webMethodName2 = webMethodName1 != null ? webMethodName1 + "_NeverEntered" : "EldosSshSession2_NoAssociatedMethod";
          collectionishRC.EnterMethod(new MethodInformation(webMethodName2, MethodType.Normal, EstimatedMethodCost.Moderate));
        }
        collectionishRC.LeaveMethod();
      }
    }

    internal static bool IsSshRsaFailingWindowActive(
      IVssRequestContext requestContext,
      DateTime utcNow)
    {
      string query = string.Format("/Configuration/SshServer/SshRsaDeprecation/FailRequestSchedule/{0}", (object) utcNow.ToString("yyyyMMdd"));
      string str1 = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) query, true, (string) null);
      if (string.IsNullOrWhiteSpace(str1))
      {
        requestContext.TraceAlways(13001165, TraceLevel.Error, "Ssh", nameof (EldosSshSession2), "No configured schedule found in " + query);
        return false;
      }
      string str2 = str1;
      char[] chArray1 = new char[1]{ ';' };
      foreach (string str3 in str2.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ '-' };
        string[] strArray = str3.Split(chArray2);
        TimeSpan result1;
        TimeSpan result2;
        if (strArray.Length != 2 || !TimeSpan.TryParse(strArray[0], out result1) || !TimeSpan.TryParse(strArray[1], out result2) || result1 > result2)
          requestContext.TraceAlways(13001165, TraceLevel.Error, "Ssh", nameof (EldosSshSession2), "Incorrect ssh-rsa failing schedule format in " + query + ". Value: " + str1);
        else if (utcNow.TimeOfDay >= result1 && utcNow.TimeOfDay <= result2)
          return true;
      }
      return false;
    }

    private IVssRequestContext CreateSshRequestContext()
    {
      using (IVssRequestContext deploymentContext = this.RequestContextProvider.CreateSystemDeploymentContext())
        return this.RequestContextProvider.CreateSshDeploymentContext(deploymentContext, this.EldosSshServer.ClientSoftwareName, this.RemoteClientIp, this.m_e2eId);
    }

    internal void SetupEventHandlers()
    {
      this.EldosSshServer.OnAuthAttempt += new TSSHAuthAttemptEvent(this.SSHServer_OnAuthAttempt);
      this.EldosSshServer.OnAuthFailed += new TSSHAuthenticationFailedEvent(this.SSHServer_OnAuthFailed);
      this.EldosSshServer.OnAuthPassword += new TSSHAuthPasswordEvent(this.SSHServer_OnAuthPassword);
      this.EldosSshServer.OnAuthPublicKey += new TSSHAuthPublicKeyEvent(this.SSHServer_OnAuthPublicKey);
      this.EldosSshServer.OnSend += new TSSHSendEvent(this.SSHServer_OnSend);
      this.EldosSshServer.OnReceive += new TSSHReceiveEvent(this.SSHServer_OnReceive);
      this.EldosSshServer.OnKexInitReceived += new TSSHKexInitReceivedEvent(this.SSHServer_OnKexInitReceived);
      this.EldosSshServer.OnCloseConnection += new TSSHCloseConnectionEvent(this.SSHServer_OnCloseConnection);
      this.EldosSshServer.OnError += new TSSHErrorEvent(this.SSHServer_OnError);
      this.EldosSshServer.OnBeforeOpenCommand += new TSSHBeforeOpenCommandEvent(this.SSHServer_OnBeforeOpenCommand);
      this.EldosSshServer.OnOpenCommand += new TSSHOpenCommandEvent(this.SSHServer_OnOpenCommand);
      this.EldosSshServer.OnBeforeOpenShell += new TSSHBeforeOpenShellEvent(this.SSHServer_OnBeforeOpenShell);
    }

    private void SSHServer_OnAuthAttempt(
      object sender,
      string username,
      int authenticationType,
      ref bool accept)
    {
      using (IVssRequestContext sshRequestContext = this.CreateSshRequestContext())
      {
        AuthenticationType authenticationType1 = (AuthenticationType) authenticationType;
        sshRequestContext.Trace(13001090, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), "Authenticate attempt for username {0} for AuthType {1}", (object) username, (object) authenticationType1.ToString());
        SshSessionKpiHelper.Increment(sshRequestContext, SshSessionKpiHelper.SshSessionKpi.AuthAttempt);
        bool flag = false;
        switch (authenticationType1)
        {
          case AuthenticationType.SSH_AUTH_TYPE_PUBLICKEY:
          case AuthenticationType.SSH_AUTH_TYPE_PUBLICKEYAGENT:
            flag = this.AuthenticationProvider.IsPublicKeySupported(sshRequestContext);
            break;
          case AuthenticationType.SSH_AUTH_TYPE_PASSWORD:
            flag = this.AuthenticationProvider.IsForwardedAuthSupported(sshRequestContext);
            break;
        }
        if (flag)
        {
          accept = true;
          sshRequestContext.Trace(13001091, TraceLevel.Verbose, "Ssh", nameof (EldosSshSession2), "Authenticate attempt for username {0} for AuthType {1} accepted.", (object) username, (object) authenticationType1.ToString());
        }
        else
        {
          accept = false;
          sshRequestContext.Trace(13001092, TraceLevel.Verbose, "Ssh", nameof (EldosSshSession2), "Authenticate attempt for username {0} for AuthType {1} rejected.", (object) username, (object) authenticationType1.ToString());
        }
      }
    }

    private void SSHServer_OnAuthFailed(object sender, int authenticationType)
    {
      using (IVssRequestContext sshRequestContext = this.CreateSshRequestContext())
      {
        AuthenticationType authenticationType1 = (AuthenticationType) authenticationType;
        sshRequestContext.Trace(13001100, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), "Authenticate attempt failed for AuthType {0}", (object) authenticationType1.ToString());
        SshSessionKpiHelper.Increment(sshRequestContext, SshSessionKpiHelper.SshSessionKpi.AuthFailure);
      }
    }

    private void SSHServer_OnAuthPassword(
      object sender,
      string username,
      string password,
      ref bool accept,
      ref bool forceChangePassword)
    {
      using (IVssRequestContext sshRequestContext = this.CreateSshRequestContext())
      {
        string webMethodName = nameof (SSHServer_OnAuthPassword);
        sshRequestContext.EnterMethod(new MethodInformation(webMethodName, MethodType.Normal, EstimatedMethodCost.Moderate));
        sshRequestContext.TraceEnter(13001170, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnAuthPassword));
        forceChangePassword = false;
        this.UserName = (string) null;
        accept = false;
        try
        {
          (Microsoft.VisualStudio.Services.Identity.Identity identity, Guid? e2eId, string clientIp) = this.AuthenticationProvider.AuthenticateForwardedCredentials(sshRequestContext, username, password);
          if (identity == null)
            return;
          this.m_authenticatedUsingPassword = true;
          accept = true;
          this.UserName = username;
          this.SessionUser = identity;
          if (e2eId.HasValue)
          {
            sshRequestContext.TraceAlways(13001171, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), string.Format("Old E2EId: {0}, new E2EId: {1}", (object) this.m_e2eId, (object) e2eId));
            this.m_e2eId = e2eId;
          }
          this.m_remoteIp = clientIp;
        }
        catch (Exception ex)
        {
          sshRequestContext.TraceException(13001178, "Ssh", nameof (EldosSshSession2), ex);
          sshRequestContext.Status = ex;
          accept = false;
          this.m_authenticatedUsingPassword = false;
        }
        finally
        {
          sshRequestContext.TraceLeave(13001179, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnAuthPassword));
          sshRequestContext.LeaveMethod();
        }
      }
    }

    private void SSHServer_OnAuthPublicKey(
      object sender,
      string username,
      TElSSHKey key,
      ref bool accept)
    {
      using (IVssRequestContext sshRequestContext = this.CreateSshRequestContext())
      {
        try
        {
          string webMethodName = nameof (SSHServer_OnAuthPublicKey);
          sshRequestContext.EnterMethod(new MethodInformation(webMethodName, MethodType.Normal, EstimatedMethodCost.Moderate));
          sshRequestContext.TraceEnter(13001220, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnAuthPublicKey));
          accept = false;
          this.UserName = username;
          this.m_cryptoKeyPair = new CryptographicKeyPair();
          string AlgName = "RSA";
          int Count = 4096;
          byte[] numArray = new byte[Count];
          int blob = key.SavePublicKeyToBlob(ref AlgName, numArray, 0, ref Count);
          if (blob != 0)
          {
            string str = "Couldn't save public key: " + ErrorCodes.GetMessage(blob);
            this.m_tracer.Trace(13001211, TraceLevel.Error, "Ssh", nameof (EldosSshSession2), str);
            throw new VssServiceException(str);
          }
          sshRequestContext.Trace(13001223, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), string.Format("SSH Auth with public key {0}(TElSSHKey.Algorithm: {1}) algortithm", (object) AlgName, (object) key.Algorithm));
          this.m_cryptoKeyPair.PublicKeyData = new byte[Count];
          Buffer.BlockCopy((Array) numArray, 0, (Array) this.m_cryptoKeyPair.PublicKeyData, 0, Count);
          this.m_cryptoKeyPair.Algorithm = KeyAlgorithm.RSA;
          accept = true;
        }
        catch (Exception ex)
        {
          sshRequestContext.TraceException(13001221, "Ssh", nameof (EldosSshSession2), ex);
          sshRequestContext.Status = ex;
          accept = false;
        }
        finally
        {
          sshRequestContext.TraceLeave(13001222, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnAuthPublicKey));
          sshRequestContext.LeaveMethod();
        }
      }
    }

    internal bool IsPublicKeyValid(IVssRequestContext collectionRequestContext)
    {
      bool flag = false;
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.AuthenticationProvider.AuthenticatePublicKey(collectionRequestContext, this.UserName, (ICryptographicKeyPair) this.m_cryptoKeyPair);
        if (identity != null)
        {
          this.SessionUser = identity;
          flag = true;
        }
      }
      finally
      {
        if (!flag)
          this.SSHServer_OnAuthFailed((object) this.EldosSshServer, 2);
      }
      return flag;
    }

    private void SSHServer_OnSend(object sender, byte[] buffer) => this.m_socketFacade.Send(buffer);

    internal void SSHServer_OnReceive(
      object sender,
      ref byte[] buffer,
      int maxSize,
      out int written)
    {
      written = this.m_socketFacade.Receive(buffer, maxSize);
    }

    private void SSHServer_OnKexInitReceived(object sender, TElStringList kexLines) => this.m_tracer.TraceConditionally(13001161, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), (Func<string>) (() => "SSHServer_OnKexInitReceived: " + kexLines.Text));

    private void SSHServer_OnCloseConnection(object sender)
    {
      this.m_tracer.Trace(13001160, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnCloseConnection));
      this.m_disposeCancellationSrc.Cancel();
    }

    private void SSHServer_OnError(object sender, int errorCode) => this.m_tracer.Trace(13001130, TraceLevel.Error, "Ssh", nameof (EldosSshSession2), "SSHServer_OnError: " + ErrorCodes.GetMessage(errorCode));

    private void SSHServer_OnBeforeOpenShell(
      object sender,
      TElSSHTunnelConnection connection,
      ref bool accept)
    {
      this.m_tracer.Trace(13001180, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnBeforeOpenShell));
      accept = false;
      byte[] Buffer = EldosSshSession2.PrepareMessageForStdErr(Resource.SshShellAccessBlocked);
      connection.SendExtendedData(Buffer, 0, Buffer.Length);
    }

    public void SSHServer_OnBeforeOpenCommand(
      object sender,
      TElSSHTunnelConnection connection,
      string command,
      ref bool accept)
    {
      this.m_tracer.Trace(13001181, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnBeforeOpenCommand));
      if (this.m_commandTask == null)
      {
        accept = true;
      }
      else
      {
        accept = false;
        byte[] Buffer = EldosSshSession2.PrepareMessageForStdErr(Resource.MultiplexingNotSupported);
        connection.SendExtendedData(Buffer, 0, Buffer.Length);
      }
    }

    public void SSHServer_OnOpenCommand(
      object sender,
      TElSSHTunnelConnection connection,
      string command)
    {
      using (IVssRequestContext sshRequestContext = this.CreateSshRequestContext())
      {
        sshRequestContext.TraceEnter(13001140, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnOpenCommand));
        try
        {
          sshRequestContext.Trace(13001141, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), "Command received {0}", (object) command);
          this.m_commandTask = this.StartCommandTask((IPipeSshChannel) new EldosPipeSshChannel((ITraceRequest) new RawTraceRequest(this.m_e2eId.GetValueOrDefault()), connection, this.m_disposeCancellationSrc.Token), command);
        }
        catch (Exception ex)
        {
          sshRequestContext.TraceException(13001148, "Ssh", nameof (EldosSshSession2), ex);
          byte[] Buffer = EldosSshSession2.PrepareMessageForStdErr(ex.Message);
          connection.SendExtendedData(Buffer, 0, Buffer.Length);
        }
        finally
        {
          if (this.m_commandTask == null)
            connection.Close(1, true);
          sshRequestContext.TraceLeave(13001140, "Ssh", nameof (EldosSshSession2), nameof (SSHServer_OnOpenCommand));
        }
      }
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
    }

    private static void SendBanner(string banner, IPipeSshChannel channel)
    {
      byte[] numArray = EldosSshSession2.PrepareMessageForStdErr(banner);
      channel.ExtendedDataOut.WriteAsync((ReadOnlyMemory<byte>) numArray, new CancellationToken()).AsTask().GetAwaiter().GetResult();
    }

    private void FailSSHRsaSession(
      IVssRequestContext requestContext,
      IPipeSshChannel channel,
      string commandName)
    {
      EldosSshSession2.SendBanner("Command " + commandName + ": " + Resource.SSHRSAIsFailed, channel);
      requestContext.TraceAlways(13001164, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), string.Format("Failing SSH session for User {0}", (object) this.SessionUser.Id));
      throw new SshNoMoreAuthMethodsAvailableException(ErrorCodes.GetMessage(ErrorCodes.ErrorStatusesToCodeMapping["ERROR_SSH_UNSUPPORTED_CIPHER"]));
    }

    private void ThrottleSession(
      IVssRequestContext requestContext,
      IPipeSshChannel channel,
      string command)
    {
      EldosSshSession2.SendBanner("Command " + command + ": " + Resource.SSHRSAIsThrottled, channel);
      int num1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/SshServer/SshRsaDeprecation/SshRsaDelayMilliseconds", 5000);
      int workerThreads1;
      int completionPortThreads1;
      ThreadPool.GetAvailableThreads(out workerThreads1, out completionPortThreads1);
      int workerThreads2;
      int completionPortThreads2;
      ThreadPool.GetMaxThreads(out workerThreads2, out completionPortThreads2);
      double num2 = (double) workerThreads1 / (double) workerThreads2;
      int millisecondsTimeout = num1;
      if (num2 < 0.5)
        millisecondsTimeout = num1 / 2;
      else if (num2 < 0.1)
        millisecondsTimeout = 0;
      requestContext.TraceAlways(13001164, TraceLevel.Info, "Ssh", nameof (EldosSshSession2), string.Format("Throttling SSH for User {0} for {1}ms (original {2}, workerRatio {3}). ThreadPool (available/max): Workers{4}/{5}, IO: {6}/{7}", (object) this.SessionUser.Id, (object) millisecondsTimeout, (object) num1, (object) num2, (object) workerThreads1, (object) workerThreads2, (object) completionPortThreads1, (object) completionPortThreads2));
      Thread.Sleep(millisecondsTimeout);
    }
  }
}
