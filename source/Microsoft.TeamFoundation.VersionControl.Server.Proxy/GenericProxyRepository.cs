// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.GenericProxyRepository
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  internal class GenericProxyRepository : FileRepository, IRepository
  {
    private TfsConnection m_teamFoundationServer;

    internal GenericProxyRepository(TfsConnection teamFoundationServer)
      : base(teamFoundationServer)
    {
      this.m_teamFoundationServer = teamFoundationServer;
    }

    public new HttpWebRequest CreateDownloadRequest(
      string downloadUrl,
      FileRepository.CredentialsType credentials,
      out bool isProxyUrl)
    {
      return base.CreateDownloadRequest(downloadUrl, credentials, out isProxyUrl);
    }

    public byte[] GetDownloadKey() => this.QueryForRepositoryProperties().SigningInfo.DownloadKey;

    public TFProxyServer GetProxyServer() => this.m_teamFoundationServer.ProxyServer;

    public void EnsureAuthenticated() => this.m_teamFoundationServer.Authenticate();
  }
}
