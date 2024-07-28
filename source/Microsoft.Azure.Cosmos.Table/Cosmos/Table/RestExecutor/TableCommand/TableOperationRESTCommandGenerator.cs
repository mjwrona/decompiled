// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.TableOperationRESTCommandGenerator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class TableOperationRESTCommandGenerator
  {
    internal static RESTCommand<TableResult> GenerateCMDForTableOperation(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions modifiedOptions)
    {
      if (operation.OperationType == TableOperationType.Insert || operation.OperationType == TableOperationType.InsertOrMerge || operation.OperationType == TableOperationType.InsertOrReplace)
      {
        if (!operation.IsTableEntity && operation.OperationType != TableOperationType.Insert)
        {
          CommonUtility.AssertNotNull("Upserts require a valid PartitionKey", (object) operation.PartitionKey);
          CommonUtility.AssertNotNull("Upserts require a valid RowKey", (object) operation.RowKey);
        }
        return TableOperationRESTCommandGenerator.InsertImpl(operation, client, table, modifiedOptions);
      }
      if (operation.OperationType == TableOperationType.Delete)
      {
        if (!operation.IsTableEntity)
        {
          CommonUtility.AssertNotNullOrEmpty("Delete requires a valid ETag", operation.ETag);
          CommonUtility.AssertNotNull("Delete requires a valid PartitionKey", (object) operation.PartitionKey);
          CommonUtility.AssertNotNull("Delete requires a valid RowKey", (object) operation.RowKey);
        }
        return TableOperationRESTCommandGenerator.DeleteImpl(operation, client, table, modifiedOptions);
      }
      if (operation.OperationType == TableOperationType.Merge)
      {
        CommonUtility.AssertNotNullOrEmpty("Merge requires a valid ETag", operation.ETag);
        CommonUtility.AssertNotNull("Merge requires a valid PartitionKey", (object) operation.PartitionKey);
        CommonUtility.AssertNotNull("Merge requires a valid RowKey", (object) operation.RowKey);
        return TableOperationRESTCommandGenerator.MergeImpl(operation, client, table, modifiedOptions);
      }
      if (operation.OperationType == TableOperationType.Replace)
      {
        CommonUtility.AssertNotNullOrEmpty("Replace requires a valid ETag", operation.ETag);
        CommonUtility.AssertNotNull("Replace requires a valid PartitionKey", (object) operation.PartitionKey);
        CommonUtility.AssertNotNull("Replace requires a valid RowKey", (object) operation.RowKey);
        return TableOperationRESTCommandGenerator.ReplaceImpl(operation, client, table, modifiedOptions);
      }
      if (operation.OperationType == TableOperationType.Retrieve)
        return TableOperationRESTCommandGenerator.RetrieveImpl(operation, client, table, modifiedOptions);
      throw new NotSupportedException();
    }

    private static RESTCommand<TableResult> InsertImpl(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<TableResult> cmd1 = new RESTCommand<TableResult>(client.Credentials, RESTCommandGeneratorUtils.GenerateRequestURI(operation, client.StorageUri, table.Name));
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableResult>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      TableResult result = new TableResult()
      {
        Result = (object) operation.Entity
      };
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableResult>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableOperation(uri, operation, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableResult>, HttpResponseMessage, Exception, OperationContext, TableResult>) ((cmd, resp, ex, ctx) => TableOperationHttpResponseParsers.TableOperationPreProcess(result, operation, resp, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<TableResult>, HttpResponseMessage, OperationContext, CancellationToken, Task<TableResult>>) ((cmd, resp, ctx, token) => TableOperationHttpResponseParsers.TableOperationPostProcessAsync(result, operation, cmd, resp, ctx, requestOptions, client.Credentials.AccountName, token));
      return cmd1;
    }

    private static RESTCommand<TableResult> DeleteImpl(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<TableResult> cmd1 = new RESTCommand<TableResult>(client.Credentials, RESTCommandGeneratorUtils.GenerateRequestURI(operation, client.StorageUri, table.Name));
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableResult>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      TableResult result = new TableResult()
      {
        Result = (object) operation.Entity
      };
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableResult>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableOperation(uri, operation, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableResult>, HttpResponseMessage, Exception, OperationContext, TableResult>) ((cmd, resp, ex, ctx) => TableOperationHttpResponseParsers.TableOperationPreProcess(result, operation, resp, ex));
      return cmd1;
    }

    private static RESTCommand<TableResult> MergeImpl(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<TableResult> cmd1 = new RESTCommand<TableResult>(client.Credentials, RESTCommandGeneratorUtils.GenerateRequestURI(operation, client.StorageUri, table.Name));
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableResult>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      TableResult result = new TableResult()
      {
        Result = (object) operation.Entity
      };
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableResult>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableOperation(uri, operation, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableResult>, HttpResponseMessage, Exception, OperationContext, TableResult>) ((cmd, resp, ex, ctx) => TableOperationHttpResponseParsers.TableOperationPreProcess(result, operation, resp, ex));
      return cmd1;
    }

    private static RESTCommand<TableResult> ReplaceImpl(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<TableResult> cmd1 = new RESTCommand<TableResult>(client.Credentials, RESTCommandGeneratorUtils.GenerateRequestURI(operation, client.StorageUri, table.Name));
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableResult>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      TableResult result = new TableResult()
      {
        Result = (object) operation.Entity
      };
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableResult>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableOperation(uri, operation, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableResult>, HttpResponseMessage, Exception, OperationContext, TableResult>) ((cmd, resp, ex, ctx) => TableOperationHttpResponseParsers.TableOperationPreProcess(result, operation, resp, ex));
      return cmd1;
    }

    private static RESTCommand<TableResult> RetrieveImpl(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<TableResult> cmd1 = new RESTCommand<TableResult>(client.Credentials, RESTCommandGeneratorUtils.GenerateRequestURI(operation, client.StorageUri, table.Name));
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TableResult>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      TableResult result = new TableResult();
      if (operation.SelectColumns != null && operation.SelectColumns.Count > 0)
        cmd1.Builder = RESTCommandGeneratorUtils.GenerateQueryBuilder(operation, requestOptions.ProjectSystemProperties);
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.BuildRequest = (Func<RESTCommand<TableResult>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTableOperation(builder != null ? builder.AddToUri(uri) : uri, operation, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.PreProcessResponse = (Func<RESTCommand<TableResult>, HttpResponseMessage, Exception, OperationContext, TableResult>) ((cmd, resp, ex, ctx) => TableOperationHttpResponseParsers.TableOperationPreProcess(result, operation, resp, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<TableResult>, HttpResponseMessage, OperationContext, CancellationToken, Task<TableResult>>) ((cmd, resp, ctx, token) => resp.StatusCode == HttpStatusCode.NotFound ? Task.FromResult<TableResult>(result) : TableOperationHttpResponseParsers.TableOperationPostProcessAsync(result, operation, cmd, resp, ctx, requestOptions, client.Credentials.AccountName, token));
      return cmd1;
    }

    internal static StorageUri GenerateRequestURI(
      TableOperation operation,
      StorageUri uriList,
      string tableName)
    {
      return new StorageUri(TableOperationRESTCommandGenerator.GenerateRequestURI(operation, uriList.PrimaryUri, tableName), TableOperationRESTCommandGenerator.GenerateRequestURI(operation, uriList.SecondaryUri, tableName));
    }

    internal static Uri GenerateRequestURI(TableOperation operation, Uri uri, string tableName)
    {
      if (uri == (Uri) null)
        return (Uri) null;
      if (operation.OperationType == TableOperationType.Insert)
        return NavigationHelper.AppendPathToSingleUri(uri, tableName + "()");
      string str;
      if (operation.IsTableEntity)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) operation.Entity.WriteEntity((OperationContext) null)["TableName"].StringValue);
      else
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}='{1}',{2}='{3}'", (object) "PartitionKey", (object) operation.PartitionKey.Replace("'", "''"), (object) "RowKey", (object) operation.RowKey.Replace("'", "''"));
      return NavigationHelper.AppendPathToSingleUri(uri, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})", (object) tableName, (object) str));
    }

    private static CommandLocationMode GetListingLocationMode(TableContinuationToken token)
    {
      if (token != null && token.TargetLocation.HasValue)
      {
        switch (token.TargetLocation.Value)
        {
          case StorageLocation.Primary:
            return CommandLocationMode.PrimaryOnly;
          case StorageLocation.Secondary:
            return CommandLocationMode.SecondaryOnly;
          default:
            CommonUtility.ArgumentOutOfRange("TargetLocation", (object) token.TargetLocation.Value);
            break;
        }
      }
      return CommandLocationMode.PrimaryOrSecondary;
    }
  }
}
