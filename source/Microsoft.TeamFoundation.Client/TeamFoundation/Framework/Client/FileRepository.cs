// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FileRepository
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FileRepository
  {
    private FileHandlerWebService m_fileServiceProxy;
    private TfsConnection m_connection;

    public FileRepository(TfsConnection teamFoundationServer)
    {
      this.m_connection = teamFoundationServer;
      this.m_fileServiceProxy = new FileHandlerWebService(teamFoundationServer);
    }

    public HttpWebRequest CreateDownloadRequest(
      Uri downloadUri,
      string downloadUrl,
      FileRepository.CredentialsType proxyCredentials,
      out bool isProxyUrl)
    {
      return this.m_fileServiceProxy.CreateDownloadRequest(downloadUri, downloadUrl, proxyCredentials, out isProxyUrl);
    }

    public HttpWebRequest CreateDownloadRequest(
      string downloadUrl,
      FileRepository.CredentialsType proxyCredentials,
      out bool isProxyUrl)
    {
      return this.m_fileServiceProxy.CreateDownloadRequest(downloadUrl, proxyCredentials, out isProxyUrl);
    }

    public FileRepositoryProperties QueryForRepositoryProperties() => this.m_fileServiceProxy.QueryForRepositoryProperties();

    public enum CredentialsType
    {
      Tfs,
      Default,
    }
  }
}
