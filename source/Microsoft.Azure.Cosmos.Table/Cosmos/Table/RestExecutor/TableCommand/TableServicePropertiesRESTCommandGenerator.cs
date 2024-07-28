// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.TableServicePropertiesRESTCommandGenerator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class TableServicePropertiesRESTCommandGenerator
  {
    internal static RESTCommand<ServiceProperties> GetServicePropertiesImpl(
      CloudTableClient client,
      TableRequestOptions requestOptions)
    {
      RESTCommand<ServiceProperties> cmd1 = new RESTCommand<ServiceProperties>(client.Credentials, client.StorageUri);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<ServiceProperties>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ServiceProperties>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, uriBuilder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableServiceProperties(uri, uriBuilder, timeout, HttpMethod.Get, (ServiceProperties) null, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.PreProcessResponse = (Func<RESTCommand<ServiceProperties>, HttpResponseMessage, Exception, OperationContext, ServiceProperties>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ServiceProperties>(HttpStatusCode.OK, resp, (ServiceProperties) null, cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ServiceProperties>, HttpResponseMessage, OperationContext, CancellationToken, Task<ServiceProperties>>) ((cmd, resp, ctx, token) => TableOperationHttpResponseParsers.ReadServicePropertiesAsync(cmd.ResponseStream));
      return cmd1;
    }

    internal static RESTCommand<NullType> SetServicePropertiesImpl(
      ServiceProperties properties,
      CloudTableClient client,
      TableRequestOptions requestOptions)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(client.Credentials, client.StorageUri);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<NullType>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, uriBuilder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableServiceProperties(uri, uriBuilder, timeout, HttpMethod.Put, properties, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, NullType.Value, cmd, ex));
      return cmd1;
    }
  }
}
