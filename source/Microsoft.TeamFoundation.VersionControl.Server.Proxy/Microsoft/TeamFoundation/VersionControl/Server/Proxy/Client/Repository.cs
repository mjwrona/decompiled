// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Proxy.Client.Repository
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Server.Proxy;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Net;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server.Proxy.Client
{
  internal class Repository : TfsHttpClient, IRepository
  {
    private Uri m_downloadBaseUri;
    private FileRepository m_fileRepository;

    public Repository(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("b2b178f5-bef9-460d-a5cf-35bcc0281cc4");

    protected override string ServiceType => "ISCCProvider";

    public RepositoryProperties GetRepositoryProperties() => (RepositoryProperties) this.Invoke((TfsClientOperation) new Repository.GetRepositoryPropertiesClientOperation(), Array.Empty<object>());

    public HttpWebRequest CreateDownloadRequest(
      string downloadUrl,
      FileRepository.CredentialsType proxyCredentials,
      out bool isProxyUrl)
    {
      return this.FileRepository.CreateDownloadRequest(this.DownloadBaseUri, downloadUrl, proxyCredentials, out isProxyUrl);
    }

    protected override string ComponentName => "VersionControl";

    public TFProxyServer GetProxyServer() => this.Connection.ProxyServer;

    public byte[] GetDownloadKey() => this.GetRepositoryProperties().DownloadKey;

    public void EnsureAuthenticated() => this.Connection.Authenticate();

    private Uri DownloadBaseUri
    {
      get
      {
        if ((Uri) null == this.m_downloadBaseUri && !this.TryGetServiceLocation("FileDownloadService", ProxyConstants.GenericDownloadServiceIdentifier, out this.m_downloadBaseUri))
          this.m_downloadBaseUri = this.GetServiceLocation("Download", RepositoryConstants.DownloadServiceIdentifier);
        return this.m_downloadBaseUri;
      }
    }

    private FileRepository FileRepository
    {
      get
      {
        if (this.m_fileRepository == null)
          this.m_fileRepository = new FileRepository(this.Connection);
        return this.m_fileRepository;
      }
    }

    internal sealed class GetRepositoryPropertiesClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetRepositoryProperties";

      public override bool HasOutputs => true;

      public override string ResultName => "GetRepositoryPropertiesResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/ClientServices/03/GetRepositoryProperties";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/ClientServices/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) RepositoryProperties.FromXml(serviceProvider, reader);
    }
  }
}
