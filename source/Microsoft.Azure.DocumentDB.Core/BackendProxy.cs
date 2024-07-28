// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BackendProxy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal abstract class BackendProxy : IBackendProxy, IDisposable
  {
    private readonly IMediaHandler mediaHandler;
    private readonly IServiceConfigurationReader serviceConfigurationReader;

    protected BackendProxy(
      IServiceConfigurationReader serviceConfigurationReader,
      IMediaHandler mediaHandler)
    {
      this.mediaHandler = mediaHandler;
      this.serviceConfigurationReader = serviceConfigurationReader;
    }

    public DatabaseAccount DocumentService => new DatabaseAccount()
    {
      ReplicationPolicy = {
        AsyncReplication = this.serviceConfigurationReader.UserReplicationPolicy.AsyncReplication,
        MinReplicaSetSize = this.serviceConfigurationReader.UserReplicationPolicy.MinReplicaSetSize,
        MaxReplicaSetSize = this.serviceConfigurationReader.UserReplicationPolicy.MaxReplicaSetSize
      },
      ConsistencyPolicy = {
        DefaultConsistencyLevel = this.serviceConfigurationReader.DefaultConsistencyLevel
      },
      SystemReplicationPolicy = {
        AsyncReplication = this.serviceConfigurationReader.SystemReplicationPolicy.AsyncReplication,
        MinReplicaSetSize = this.serviceConfigurationReader.SystemReplicationPolicy.MinReplicaSetSize,
        MaxReplicaSetSize = this.serviceConfigurationReader.SystemReplicationPolicy.MaxReplicaSetSize
      },
      ReadPolicy = {
        PrimaryReadCoefficient = this.serviceConfigurationReader.ReadPolicy.PrimaryReadCoefficient,
        SecondaryReadCoefficient = this.serviceConfigurationReader.ReadPolicy.SecondaryReadCoefficient
      }
    };

    public IServiceConfigurationReader ConfigurationReader => this.serviceConfigurationReader;

    public abstract void Dispose();

    public abstract Task StartAsync();

    public abstract Task<DocumentServiceResponse> CreateAsync(DocumentServiceRequest request);

    public abstract Task<DocumentServiceResponse> UpsertAsync(DocumentServiceRequest request);

    public abstract Task<DocumentServiceResponse> ReadAsync(DocumentServiceRequest request);

    public abstract Task<DocumentServiceResponse> ReplaceAsync(DocumentServiceRequest request);

    public abstract Task<DocumentServiceResponse> DeleteAsync(DocumentServiceRequest request);

    public abstract Task<DocumentServiceResponse> ExecuteAsync(DocumentServiceRequest request);

    public abstract Task<DocumentServiceResponse> ReadFeedAsync(
      DocumentServiceRequest request,
      ReadType readType);

    public virtual Task UploadMediaAsync(
      string mediaId,
      Stream mediaStream,
      INameValueCollection headers,
      int singleBlobUploadThresholdInBytes,
      TimeSpan blobUploadTiemoutSeconds)
    {
      if (this.mediaHandler == null)
        throw new MethodNotAllowedException();
      return this.mediaHandler.UploadMediaAsync(mediaId, mediaStream, headers, singleBlobUploadThresholdInBytes, blobUploadTiemoutSeconds);
    }

    public virtual Task DeleteMediaAsync(string mediaId, INameValueCollection headers)
    {
      if (this.mediaHandler == null)
        throw new MethodNotAllowedException();
      return this.mediaHandler.DeleteMediaAsync(mediaId, headers);
    }

    public virtual Task<Tuple<INameValueCollection, INameValueCollection>> HeadMediaAsync(
      string mediaId,
      INameValueCollection headers)
    {
      if (this.mediaHandler == null)
        throw new MethodNotAllowedException();
      return this.mediaHandler.HeadMediaAsync(mediaId, headers);
    }

    public virtual Task<Tuple<Stream, INameValueCollection, INameValueCollection>> DownloadMediaAsync(
      string mediaId,
      INameValueCollection headers,
      TimeSpan blobDownloadTimeoutSeconds)
    {
      if (this.mediaHandler == null)
        throw new MethodNotAllowedException();
      return this.mediaHandler.DownloadMediaAsync(mediaId, headers, blobDownloadTimeoutSeconds);
    }
  }
}
