// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.RESTCommand`1
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal class RESTCommand<T>
  {
    public RESTCommand(StorageCredentials credentials, StorageUri storageUri)
      : this(credentials, storageUri, (UriQueryBuilder) null)
    {
    }

    public RESTCommand(
      StorageCredentials credentials,
      StorageUri storageUri,
      UriQueryBuilder builder)
    {
      this.Credentials = credentials;
      this.StorageUri = storageUri;
      this.Builder = builder;
      this.RequestResults = (IList<RequestResult>) new List<RequestResult>();
    }

    public Stream ResponseStream { get; set; }

    public RequestResult CurrentResult { get; set; }

    public IList<RequestResult> RequestResults { get; }

    public LocationMode LocationMode { get; set; }

    public CommandLocationMode CommandLocationMode { get; set; }

    public StorageCredentials Credentials { get; set; }

    public StorageUri StorageUri { get; set; }

    public UriQueryBuilder Builder { get; set; }

    public int? ServerTimeoutInSeconds { get; set; }

    public DateTime? OperationExpiryTime { get; set; }

    public object OperationState { get; set; }

    public HttpClient HttpClient { get; set; }

    public Func<RESTCommand<T>, OperationContext, HttpContent> BuildContent { get; set; }

    public Func<RESTCommand<T>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage> BuildRequest { get; set; }

    public Func<RESTCommand<T>, HttpResponseMessage, Exception, OperationContext, T> PreProcessResponse { get; set; }

    public Func<RESTCommand<T>, HttpResponseMessage, OperationContext, CancellationToken, Task<T>> PostProcessResponseAsync { get; set; }

    public Action<RESTCommand<T>, Exception, OperationContext> RecoveryAction { get; set; }

    public Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>> ParseErrorAsync { get; set; }
  }
}
