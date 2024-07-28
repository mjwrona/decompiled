// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.TableServiceStatsRESTCommandGenerator
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
  internal static class TableServiceStatsRESTCommandGenerator
  {
    internal static RESTCommand<ServiceStats> GenerateCMDForGetServiceStats(
      CloudTableClient client,
      TableRequestOptions requestOptions)
    {
      RESTCommand<ServiceStats> cmd1 = new RESTCommand<ServiceStats>(client.Credentials, client.StorageUri);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<ServiceStats>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      LocationMode? locationMode = requestOptions.LocationMode;
      if ((locationMode.GetValueOrDefault() != LocationMode.PrimaryOnly ? 0 : (locationMode.HasValue ? 1 : 0)) != 0)
        throw new InvalidOperationException("GetServiceStats cannot be run with a 'PrimaryOnly' location mode.");
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ServiceStats>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, uriBuilder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForGetServiceStats(uri, uriBuilder, timeout, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.PreProcessResponse = (Func<RESTCommand<ServiceStats>, HttpResponseMessage, Exception, OperationContext, ServiceStats>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ServiceStats>(HttpStatusCode.OK, resp, (ServiceStats) null, cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ServiceStats>, HttpResponseMessage, OperationContext, CancellationToken, Task<ServiceStats>>) ((cmd, resp, ctx, token) => TableOperationHttpResponseParsers.ReadServiceStatsAsync(cmd.ResponseStream));
      return cmd1;
    }
  }
}
