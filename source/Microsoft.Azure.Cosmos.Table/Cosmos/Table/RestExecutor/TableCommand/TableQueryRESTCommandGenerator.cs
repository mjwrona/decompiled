// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.TableQueryRESTCommandGenerator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class TableQueryRESTCommandGenerator
  {
    internal static RESTCommand<TableQuerySegment<DynamicTableEntity>> GenerateCMDForTableQuery(
      TableQuery query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      UriQueryBuilder queryBuilder = RESTCommandGeneratorUtils.GenerateQueryBuilder(query, requestOptions.ProjectSystemProperties);
      if (token != null)
        RESTCommandGeneratorUtils.ApplyToUriQueryBuilder(token, queryBuilder);
      StorageUri uri1 = NavigationHelper.AppendPathToUri(client.StorageUri, table.Name);
      RESTCommand<TableQuerySegment<DynamicTableEntity>> cmd1 = new RESTCommand<TableQuerySegment<DynamicTableEntity>>(client.Credentials, uri1);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableQuerySegment<DynamicTableEntity>>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      cmd1.CommandLocationMode = RESTCommandGeneratorUtils.GetListingLocationMode(token);
      cmd1.Builder = queryBuilder;
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableQuerySegment<DynamicTableEntity>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableQuery(uri, builder, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableQuerySegment<DynamicTableEntity>>, HttpResponseMessage, Exception, OperationContext, TableQuerySegment<DynamicTableEntity>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<TableQuerySegment<DynamicTableEntity>>(HttpStatusCode.OK, resp != null ? resp.StatusCode : HttpStatusCode.Unused, (TableQuerySegment<DynamicTableEntity>) null, cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<TableQuerySegment<DynamicTableEntity>>, HttpResponseMessage, OperationContext, CancellationToken, Task<TableQuerySegment<DynamicTableEntity>>>) (async (cmd, resp, ctx, cancellationToken) =>
      {
        ResultSegment<DynamicTableEntity> resSeg = await TableOperationHttpResponseParsers.TableQueryPostProcessGenericAsync<DynamicTableEntity, DynamicTableEntity>(cmd.ResponseStream, new Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, DynamicTableEntity>(EntityUtilities.ResolveDynamicEntity), resp, requestOptions, ctx, cancellationToken);
        if (resSeg.ContinuationToken != null)
          resSeg.ContinuationToken.TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation);
        return new TableQuerySegment<DynamicTableEntity>(resSeg);
      });
      return cmd1;
    }

    internal static RESTCommand<TableQuerySegment<TResult>> GenerateCMDForTableQuery<TInput, TResult>(
      TableQuery<TInput> query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions)
    {
      UriQueryBuilder queryBuilder = RESTCommandGeneratorUtils.GenerateQueryBuilder<TInput>(query, requestOptions.ProjectSystemProperties);
      if (token != null)
        RESTCommandGeneratorUtils.ApplyToUriQueryBuilder(token, queryBuilder);
      StorageUri uri1 = NavigationHelper.AppendPathToUri(client.StorageUri, table.Name);
      RESTCommand<TableQuerySegment<TResult>> cmd1 = new RESTCommand<TableQuerySegment<TResult>>(client.Credentials, uri1);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableQuerySegment<TResult>>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      cmd1.CommandLocationMode = RESTCommandGeneratorUtils.GetListingLocationMode(token);
      cmd1.Builder = queryBuilder;
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableQuerySegment<TResult>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableQuery(uri, builder, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableQuerySegment<TResult>>, HttpResponseMessage, Exception, OperationContext, TableQuerySegment<TResult>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<TableQuerySegment<TResult>>(HttpStatusCode.OK, resp != null ? resp.StatusCode : HttpStatusCode.Unused, (TableQuerySegment<TResult>) null, cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<TableQuerySegment<TResult>>, HttpResponseMessage, OperationContext, CancellationToken, Task<TableQuerySegment<TResult>>>) (async (cmd, resp, ctx, cancellationToken) =>
      {
        ResultSegment<TResult> resSeg = await TableOperationHttpResponseParsers.TableQueryPostProcessGenericAsync<TResult, TInput>(cmd.ResponseStream, new Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, TResult>(resolver.Invoke), resp, requestOptions, ctx, cancellationToken);
        if (resSeg.ContinuationToken != null)
          resSeg.ContinuationToken.TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation);
        return new TableQuerySegment<TResult>(resSeg);
      });
      return cmd1;
    }

    internal static RESTCommand<TableQuerySegment<TResult>> GenerateCMDForTableQuery<TResult>(
      TableQuery query,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions)
    {
      UriQueryBuilder queryBuilder = RESTCommandGeneratorUtils.GenerateQueryBuilder(query, requestOptions.ProjectSystemProperties);
      if (token != null)
        RESTCommandGeneratorUtils.ApplyToUriQueryBuilder(token, queryBuilder);
      StorageUri uri1 = NavigationHelper.AppendPathToUri(client.StorageUri, table.Name);
      RESTCommand<TableQuerySegment<TResult>> cmd1 = new RESTCommand<TableQuerySegment<TResult>>(client.Credentials, uri1);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableQuerySegment<TResult>>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      cmd1.CommandLocationMode = RESTCommandGeneratorUtils.GetListingLocationMode(token);
      cmd1.Builder = queryBuilder;
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableQuerySegment<TResult>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableQuery(uri, builder, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableQuerySegment<TResult>>, HttpResponseMessage, Exception, OperationContext, TableQuerySegment<TResult>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<TableQuerySegment<TResult>>(HttpStatusCode.OK, resp != null ? resp.StatusCode : HttpStatusCode.Unused, (TableQuerySegment<TResult>) null, cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<TableQuerySegment<TResult>>, HttpResponseMessage, OperationContext, CancellationToken, Task<TableQuerySegment<TResult>>>) (async (cmd, resp, ctx, cancellationToken) =>
      {
        ResultSegment<TResult> resSeg = await TableOperationHttpResponseParsers.TableQueryPostProcessGenericAsync<TResult, DynamicTableEntity>(cmd.ResponseStream, new Func<string, string, DateTimeOffset, IDictionary<string, EntityProperty>, string, TResult>(resolver.Invoke), resp, requestOptions, ctx, cancellationToken);
        if (resSeg.ContinuationToken != null)
          resSeg.ContinuationToken.TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation);
        return new TableQuerySegment<TResult>(resSeg);
      });
      return cmd1;
    }
  }
}
