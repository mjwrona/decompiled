// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.TableBatchOperationRESTCommandGenerator
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
  internal static class TableBatchOperationRESTCommandGenerator
  {
    internal static RESTCommand<TableBatchResult> GenerateCMDForTableBatchOperation(
      TableBatchOperation batch,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<TableBatchResult> cmd1 = new RESTCommand<TableBatchResult>(client.Credentials, client.StorageUri);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableBatchResult>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      TableBatchResult results = new TableBatchResult();
      cmd1.CommandLocationMode = batch.ContainsWrites ? CommandLocationMode.PrimaryOnly : CommandLocationMode.PrimaryOrSecondary;
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOnly;
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableBatchResult>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableBatchOperation(uri, batch, (ICanonicalizer) SharedKeyCanonicalizer.Instance, table.Name, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableBatchResult>, HttpResponseMessage, Exception, OperationContext, TableBatchResult>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<TableBatchResult>(HttpStatusCode.Accepted, resp != null ? resp.StatusCode : HttpStatusCode.Unused, results, cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<TableBatchResult>, HttpResponseMessage, OperationContext, CancellationToken, Task<TableBatchResult>>) ((cmd, resp, ctx, token) => TableOperationHttpResponseParsers.TableBatchOperationPostProcessAsync(results, batch, cmd, resp, ctx, requestOptions, client.Credentials.AccountName, token));
      cmd1.RecoveryAction = (Action<RESTCommand<TableBatchResult>, Exception, OperationContext>) ((cmd, ex, ctx) => results.Clear());
      return cmd1;
    }
  }
}
