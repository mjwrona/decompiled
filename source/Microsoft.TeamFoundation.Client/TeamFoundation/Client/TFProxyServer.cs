// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TFProxyServer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TFProxyServer
  {
    private string m_url;
    private string m_lastException;
    private bool m_isEnabled;
    private bool m_wasAutoConfigured;
    private DateTime m_disableTime;
    private DateTime m_lastConfigureTime;
    private DateTime m_lastCheckTime;
    private int m_retryIntervalMinutes;
    private static FileRepository.CredentialsType s_credentialType;

    internal TFProxyServer(
      string url,
      bool isEnabled,
      int retryIntervalMinutes,
      bool wasAutoConfigured,
      DateTime lastConfigureTime,
      DateTime lastCheckTime)
    {
      this.m_url = url;
      this.m_isEnabled = isEnabled;
      this.m_retryIntervalMinutes = retryIntervalMinutes;
      this.m_wasAutoConfigured = wasAutoConfigured;
      this.m_lastConfigureTime = lastConfigureTime;
      this.m_lastCheckTime = lastCheckTime;
    }

    public async Task<Stream> GetFileAsync(TfsConnection connection, string ticket)
    {
      TFProxyServer tfProxyServer = this;
      FileRepository fileRepository = new FileRepository(connection);
      bool retryOnException = false;
      FileRepository.CredentialsType localCredType = TFProxyServer.s_credentialType;
      string downloadUrl = ticket;
      int proxyCredentials = (int) localCredType;
      bool isProxy;
      ref bool local = ref isProxy;
      HttpWebRequest downloadRequest = fileRepository.CreateDownloadRequest(downloadUrl, (FileRepository.CredentialsType) proxyCredentials, out local);
      HttpWebResponse response = (HttpWebResponse) null;
      try
      {
        try
        {
          response = (HttpWebResponse) await downloadRequest.GetResponseAsync();
        }
        catch (WebException ex)
        {
          if (WebExceptionStatus.RequestCanceled == ex.Status)
          {
            throw;
          }
          else
          {
            response = (HttpWebResponse) ex.Response;
            if (response == null)
              throw;
          }
        }
        if (HttpStatusCode.Unauthorized == response.StatusCode)
        {
          tfProxyServer.m_lastException = string.Format("Response status is {0}: ({1})", (object) response.StatusCode, (object) response.StatusDescription);
          throw new TeamFoundationServerUnauthorizedException();
        }
        if (HttpStatusCode.OK != response.StatusCode)
        {
          string header = response.Headers["X-VersionControl-Exception"];
          string message1 = string.Empty;
          if (!string.IsNullOrEmpty(header))
          {
            if (isProxy && header.Equals("UnknownRepositoryException", StringComparison.OrdinalIgnoreCase))
            {
              tfProxyServer.NotifyUnavailable();
              return await tfProxyServer.GetFileAsync(connection, ticket);
            }
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), TfsRequestSettings.RequestEncoding))
              message1 = streamReader.ReadToEnd();
            tfProxyServer.m_lastException = string.Format("Response status is {0}: ({1}) - Response error: {2} - {3}", (object) response.StatusCode, (object) response.StatusDescription, (object) header, (object) message1);
            throw new Exception(message1);
          }
          string message2 = string.Format("Response status is {0}: ({1})", (object) response.StatusCode, (object) response.StatusDescription);
          tfProxyServer.m_lastException = message2;
          throw new Exception(message2);
        }
      }
      catch (Exception ex)
      {
        tfProxyServer.m_lastException = string.Format("Response status is {0}: ({1}) - Response error is {2}- {3}", response != null ? (object) response.StatusCode.ToString() : (object) "Unknown", response != null ? (object) response.StatusDescription : (object) "Unknown", (object) ex.GetType().Name, (object) ex.Message);
        response?.Dispose();
        if (ex is TeamFoundationServerUnauthorizedException & isProxy && localCredType == FileRepository.CredentialsType.Tfs)
        {
          TFProxyServer.s_credentialType = FileRepository.CredentialsType.Default;
          retryOnException = true;
        }
        else
        {
          if (isProxy)
          {
            switch (ex)
            {
              case DestroyedContentUnavailableException _:
              case System.OperationCanceledException _:
                break;
              default:
                tfProxyServer.NotifyUnavailable();
                retryOnException = true;
                goto label_30;
            }
          }
          throw;
        }
      }
label_30:
      if (retryOnException)
        return await tfProxyServer.GetFileAsync(connection, ticket);
      Stream stream = response.GetResponseStream();
      if (VssStringComparer.ContentType.Equals(response.ContentType, "application/gzip"))
        stream = (Stream) new GZipStream(stream, CompressionMode.Decompress);
      return stream;
    }

    public void NotifyUnavailable() => this.m_disableTime = DateTime.UtcNow.AddMinutes((double) this.m_retryIntervalMinutes);

    internal void ResetDisableTime()
    {
      this.m_disableTime = new DateTime();
      this.m_lastException = (string) null;
    }

    public string Url
    {
      get => this.m_url;
      set
      {
        this.m_url = value;
        TFProxyServer.s_credentialType = FileRepository.CredentialsType.Tfs;
      }
    }

    public bool IsEnabled
    {
      get => this.m_isEnabled;
      set => this.m_isEnabled = value;
    }

    public bool IsAvailable => this.IsEnabled && DateTime.UtcNow > this.m_disableTime;

    public string LastException => this.m_lastException;

    internal int RetryIntervalMinutes
    {
      set => this.m_retryIntervalMinutes = value;
    }

    public bool WasAutoConfigured
    {
      get => this.m_wasAutoConfigured;
      set => this.m_wasAutoConfigured = value;
    }

    public DateTime LastConfigureTime
    {
      get => this.m_lastConfigureTime;
      set => this.m_lastConfigureTime = value;
    }

    internal DateTime LastCheckTime
    {
      get => this.m_lastCheckTime;
      set => this.m_lastCheckTime = value;
    }

    internal bool NeedsAutoConfigure
    {
      get
      {
        if (!(DateTime.UtcNow >= this.m_lastCheckTime.AddDays(1.0)))
          return false;
        return string.IsNullOrEmpty(this.Url) || this.WasAutoConfigured;
      }
    }
  }
}
