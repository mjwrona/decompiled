// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IBackendProxy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal interface IBackendProxy : IDisposable
  {
    IServiceConfigurationReader ConfigurationReader { get; }

    Task StartAsync();

    Task<DocumentServiceResponse> CreateAsync(DocumentServiceRequest request);

    Task<DocumentServiceResponse> UpsertAsync(DocumentServiceRequest request);

    Task<DocumentServiceResponse> ReadAsync(DocumentServiceRequest request);

    Task<DocumentServiceResponse> ReplaceAsync(DocumentServiceRequest request);

    Task<DocumentServiceResponse> DeleteAsync(DocumentServiceRequest request);

    Task<DocumentServiceResponse> ExecuteAsync(DocumentServiceRequest request);

    Task<DocumentServiceResponse> ReadFeedAsync(DocumentServiceRequest request, ReadType readType);

    Task UploadMediaAsync(
      string mediaId,
      Stream mediaStream,
      INameValueCollection headers,
      int singleBlobUploadThresholdInBytes,
      TimeSpan blobUploadTimeoutSeconds);

    Task<Tuple<INameValueCollection, INameValueCollection>> HeadMediaAsync(
      string mediaId,
      INameValueCollection headers = null);

    Task<Tuple<Stream, INameValueCollection, INameValueCollection>> DownloadMediaAsync(
      string mediaId,
      INameValueCollection headers,
      TimeSpan blobDownloadTimeoutSeconds);

    Task DeleteMediaAsync(string mediaId, INameValueCollection headers = null);
  }
}
