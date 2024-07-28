// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.TeamFoundationSshService
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  internal sealed class TeamFoundationSshService : ITeamFoundationSshService, IVssFrameworkService
  {
    private SshSocketListener _socketListener;
    private SshSessionManager _sessionManager;
    private const string s_Area = "Ssh";
    private const string s_Layer = "TeamFoundationSshService";

    internal TeamFoundationSshService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.TraceEnter(13000021, "Ssh", nameof (TeamFoundationSshService), "ServiceStart");
      this._sessionManager = SshSessionManager.Create(systemRequestContext);
      this._socketListener = new SshSocketListener(this._sessionManager, systemRequestContext.ServiceHost.DeploymentServiceHost);
      this._socketListener.Start(systemRequestContext);
      systemRequestContext.TraceLeave(13000025, "Ssh", nameof (TeamFoundationSshService), "ServiceStart");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(13000026, "Ssh", nameof (TeamFoundationSshService), "ServiceEnd");
      if (this._socketListener != null)
      {
        lock (this._socketListener)
        {
          this._socketListener.Dispose();
          this._socketListener = (SshSocketListener) null;
        }
      }
      systemRequestContext.TraceLeave(13000030, "Ssh", nameof (TeamFoundationSshService), "ServiceEnd");
    }

    public void CloseSessions()
    {
      if (this._sessionManager == null)
        return;
      this._sessionManager.Dispose();
      this._sessionManager = (SshSessionManager) null;
    }
  }
}
