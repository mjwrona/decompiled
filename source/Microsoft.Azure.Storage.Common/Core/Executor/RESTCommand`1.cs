// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Executor.RESTCommand`1
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Executor
{
  internal class RESTCommand<T> : StorageCommandBase<T>
  {
    public LocationMode LocationMode;
    public CommandLocationMode CommandLocationMode;
    public StorageCredentials Credentials;
    public StorageUri StorageUri;
    public HttpClient HttpClient;
    public UriQueryBuilder Builder;
    private Stream responseStream;
    public Stream DestinationStream;
    public Stream ErrorStream;
    public bool RetrieveResponseStream;
    public ChecksumRequested ChecksumRequestedForResponseStream = ChecksumRequested.None;
    public Func<RESTCommand<T>, OperationContext, HttpContent> BuildContent;
    public Func<RESTCommand<T>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage> BuildRequest;
    public Func<RESTCommand<T>, HttpResponseMessage, Exception, OperationContext, T> PreProcessResponse;
    public Func<RESTCommand<T>, HttpResponseMessage, OperationContext, CancellationToken, Task<T>> PostProcessResponseAsync;
    public Action<RESTCommand<T>> DisposeAction;
    private Stream sendStream;
    public long? SendStreamLength;
    public Action<HttpRequestMessage, OperationContext> SetHeaders;
    public Action<HttpRequestMessage, OperationContext> SignRequest;

    public RESTCommand(
      StorageCredentials credentials,
      StorageUri storageUri,
      HttpClient httpClient)
      : this(credentials, storageUri, (UriQueryBuilder) null, httpClient)
    {
    }

    public RESTCommand(
      StorageCredentials credentials,
      StorageUri storageUri,
      UriQueryBuilder builder,
      HttpClient httpClient)
    {
      this.Credentials = credentials;
      this.StorageUri = storageUri;
      this.Builder = builder;
      this.HttpClient = httpClient;
    }

    public Stream ResponseStream
    {
      get => this.responseStream;
      set => this.responseStream = value == null ? (Stream) null : value.WrapWithByteCountingStream(this.CurrentResult);
    }

    public Stream StreamToDispose { get; set; }

    public Stream SendStream
    {
      get => this.sendStream;
      set => this.sendStream = value;
    }
  }
}
