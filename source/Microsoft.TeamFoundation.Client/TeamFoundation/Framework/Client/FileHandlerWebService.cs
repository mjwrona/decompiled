// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FileHandlerWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using System;
using System.Globalization;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class FileHandlerWebService : TfsHttpClient
  {
    private Uri m_downloadBaseUri;

    public FileHandlerWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("48850d87-0c57-4265-bc2b-812e445f73c6");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("48850d87-0c57-4265-bc2b-812e445f73c6");

    protected override string ServiceType => "FileHandlerService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public FileRepositoryProperties QueryForRepositoryProperties() => (FileRepositoryProperties) this.Invoke((TfsClientOperation) new FileHandlerWebService.QueryForRepositoryPropertiesClientOperation(), Array.Empty<object>());

    internal HttpWebRequest CreateDownloadRequest(
      string downloadUrl,
      FileRepository.CredentialsType credentials,
      out bool isProxyUrl)
    {
      return this.CreateDownloadRequest(this.DownloadBaseUri, downloadUrl, credentials, out isProxyUrl);
    }

    internal HttpWebRequest CreateDownloadRequest(
      Uri downloadUri,
      string downloadUrl,
      FileRepository.CredentialsType credentials,
      out bool isProxyUrl)
    {
      TFProxyServer proxyServer = this.Connection.ProxyServer;
      UriBuilder uri;
      if (proxyServer != null && proxyServer.IsAvailable)
      {
        downloadUrl = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}&{1}={2}", (object) downloadUrl, (object) "rid", (object) this.Connection.InstanceId.ToString());
        uri = TfsHttpRequestHelpers.CreateUri(proxyServer.Url + "/V1.0/item.asmx", downloadUrl);
        isProxyUrl = true;
      }
      else
      {
        uri = TfsHttpRequestHelpers.CreateUri(downloadUri.AbsoluteUri, downloadUrl);
        isProxyUrl = false;
      }
      if (this.State == TfsHttpClientState.Opened)
      {
        int num = this.Settings.Tracing.TraceInfo ? 1 : 0;
      }
      HttpWebRequest downloadRequest = TfsHttpRequestHelpers.PrepareWebRequest((HttpWebRequest) WebRequest.Create(uri.Uri), this.Connection);
      downloadRequest.Method = "GET";
      if (isProxyUrl && credentials == FileRepository.CredentialsType.Default)
      {
        downloadRequest.Credentials = CredentialCache.DefaultCredentials;
        downloadRequest.ConnectionGroupName = TfsHttpRequestHelpers.GetConnectionGroupName(downloadRequest.RequestUri, downloadRequest.Credentials);
      }
      return downloadRequest;
    }

    private Uri DownloadBaseUri
    {
      get
      {
        if (this.m_downloadBaseUri == (Uri) null)
          this.m_downloadBaseUri = this.GetServiceLocation("FileDownloadService", ProxyConstants.GenericDownloadServiceIdentifier);
        return this.m_downloadBaseUri;
      }
    }

    internal sealed class QueryForRepositoryPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryForRepositoryProperties";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryForRepositoryPropertiesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryForRepositoryProperties";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) FileRepositoryProperties.FromXml(serviceProvider, reader);
    }
  }
}
