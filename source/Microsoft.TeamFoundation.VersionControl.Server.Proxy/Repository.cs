// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.Repository
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using System;
using System.IO;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  internal class Repository : IDisposable
  {
    private object m_ticketSignerLock = new object();
    private ISigner m_signer;
    private IRepository m_repository;
    private DirectoryInfo m_topLevelDirectory;
    private DateTime m_lastInvalidation = DateTime.MinValue;

    public Repository(IRepository repository, string topLevelFolder)
    {
      this.m_repository = repository;
      this.m_topLevelDirectory = new DirectoryInfo(topLevelFolder);
    }

    internal DirectoryInfo TopLevelDirectory => this.m_topLevelDirectory;

    public void Dispose()
    {
      ISigner signer = this.m_signer;
      this.m_signer = (ISigner) null;
      signer?.Dispose();
    }

    public HttpWebResponse GetDownloadResponse(string downloadUrl)
    {
      TFProxyServer proxyServer = this.m_repository.GetProxyServer();
      if (proxyServer != null)
        proxyServer.IsEnabled = false;
      bool isProxyUrl;
      HttpWebRequest downloadRequest1 = this.m_repository.CreateDownloadRequest(downloadUrl, FileRepository.CredentialsType.Tfs, out isProxyUrl);
      Exception exception = (Exception) null;
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse) downloadRequest1.GetResponse();
      }
      catch (WebException ex)
      {
        exception = (Exception) ex;
        response = ex.Response as HttpWebResponse;
      }
      if (response != null && (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Unauthorized))
      {
        this.m_repository.EnsureAuthenticated();
        HttpWebRequest downloadRequest2 = this.m_repository.CreateDownloadRequest(downloadUrl, FileRepository.CredentialsType.Tfs, out isProxyUrl);
        try
        {
          response = (HttpWebResponse) downloadRequest2.GetResponse();
        }
        catch (WebException ex)
        {
          exception = (Exception) ex;
          response = ex.Response as HttpWebResponse;
        }
      }
      if (response != null)
      {
        if (response.StatusCode != HttpStatusCode.OK)
        {
          TeamFoundationTrace.Verbose("Response failed.  Status: {0}", (object) System.Enum.GetName(typeof (HttpStatusCode), (object) response.StatusCode));
          throw new Microsoft.TeamFoundation.Framework.Server.ProxyException(new StreamReader(response.GetResponseStream()).ReadToEnd());
        }
        return response;
      }
      if (exception != null)
        throw exception;
      throw new Microsoft.TeamFoundation.Framework.Server.ProxyException(ProxyResources.Get("UnknownErrorInGet"));
    }

    internal void InvalidateRepositoryProperties()
    {
      if (DateTime.UtcNow.Subtract(this.m_lastInvalidation) < TimeSpan.FromMinutes(1.0))
        return;
      this.TicketSigner = (ISigner) null;
      this.m_lastInvalidation = DateTime.UtcNow;
    }

    internal ISigner TicketSigner
    {
      get
      {
        ISigner signer;
        if ((signer = this.m_signer) == null)
        {
          lock (this.m_ticketSignerLock)
          {
            if ((signer = this.m_signer) == null)
            {
              signer = SigningManager.GetSigner(this.m_repository.GetDownloadKey(), SigningAlgorithm.SHA1);
              this.m_signer = signer;
            }
          }
        }
        return signer;
      }
      private set => this.m_signer = value;
    }
  }
}
