// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.KeyManagementService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class KeyManagementService : IKeyManagementService, IVssFrameworkService
  {
    private Task<string> m_pendingRequestTask;
    private object m_requestLock = new object();
    private byte[] m_gallerySigningKey;
    private const string s_area = "KeyManagementService";
    private const string s_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new NotSupportedException(ExtensionResources.OnPremisesNotSupported());
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", KeyManagementConstants.SigningKeyModified, new SqlNotificationHandler(this.OnSigningKeyChanged), false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", KeyManagementConstants.SigningKeyModified, new SqlNotificationHandler(this.OnSigningKeyChanged), false);

    public byte[] GetSigningKey(IVssRequestContext requestContext)
    {
      byte[] gallerySigningKey = this.m_gallerySigningKey;
      if (gallerySigningKey == null)
      {
        if (this.m_pendingRequestTask == null)
          this.RequestSigningKey(requestContext);
        lock (this.m_requestLock)
        {
          if (this.m_pendingRequestTask != null)
          {
            try
            {
              this.m_gallerySigningKey = Convert.FromBase64String(this.m_pendingRequestTask.SyncResult<string>());
            }
            finally
            {
              this.m_pendingRequestTask = (Task<string>) null;
            }
          }
          gallerySigningKey = this.m_gallerySigningKey;
        }
      }
      return gallerySigningKey;
    }

    private void OnSigningKeyChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      requestContext.Trace(10013287, TraceLevel.Info, nameof (KeyManagementService), "Service", "OnSigningKeyChanged: Received message that signing key has changed.  Clearing cache.");
      lock (this.m_requestLock)
        this.m_gallerySigningKey = (byte[]) null;
    }

    private void RequestSigningKey(IVssRequestContext requestContext)
    {
      GalleryHttpClient client = requestContext.GetClient<GalleryHttpClient>();
      lock (this.m_requestLock)
      {
        if (this.m_gallerySigningKey != null || this.m_pendingRequestTask != null)
          return;
        this.m_pendingRequestTask = client.GetSigningKeyAsync("AccountSigningKey");
      }
    }
  }
}
