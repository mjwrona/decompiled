// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.RESTCommandGeneratorUtils
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class RESTCommandGeneratorUtils
  {
    internal static StorageUri GenerateRequestURI(
      TableOperation operation,
      StorageUri uriList,
      string tableName)
    {
      return new StorageUri(RESTCommandGeneratorUtils.GenerateRequestURI(operation, uriList.PrimaryUri, tableName), RESTCommandGeneratorUtils.GenerateRequestURI(operation, uriList.SecondaryUri, tableName));
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

    internal static HttpMethod ExtractHttpMethod(TableOperation operation)
    {
      switch (operation.OperationType)
      {
        case TableOperationType.Insert:
          return HttpMethod.Post;
        case TableOperationType.Delete:
          return HttpMethod.Delete;
        case TableOperationType.Replace:
        case TableOperationType.InsertOrReplace:
          return HttpMethod.Put;
        case TableOperationType.Merge:
        case TableOperationType.InsertOrMerge:
          return HttpMethod.Post;
        case TableOperationType.Retrieve:
          return HttpMethod.Get;
        default:
          throw new NotSupportedException();
      }
    }

    internal static UriQueryBuilder GenerateQueryBuilder(
      TableQuery query,
      bool? projectSystemProperties)
    {
      UriQueryBuilder queryBuilder = new UriQueryBuilder();
      if (!string.IsNullOrEmpty(query.FilterString))
        queryBuilder.Add("$filter", query.FilterString);
      if (query.TakeCount.HasValue)
        queryBuilder.Add("$top", Convert.ToString(Math.Min(1000, query.TakeCount.Value), (IFormatProvider) CultureInfo.InvariantCulture));
      if (query.SelectColumns != null && query.SelectColumns.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        for (int index = 0; index < query.SelectColumns.Count; ++index)
        {
          if (query.SelectColumns[index] == "PartitionKey")
            flag2 = true;
          else if (query.SelectColumns[index] == "RowKey")
            flag1 = true;
          else if (query.SelectColumns[index] == "Timestamp")
            flag3 = true;
          stringBuilder.Append(query.SelectColumns[index]);
          if (index < query.SelectColumns.Count - 1)
            stringBuilder.Append(",");
        }
        if (projectSystemProperties.Value)
        {
          if (!flag2)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("PartitionKey");
          }
          if (!flag1)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("RowKey");
          }
          if (!flag3)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("Timestamp");
          }
        }
        queryBuilder.Add("$select", stringBuilder.ToString());
      }
      return queryBuilder;
    }

    internal static UriQueryBuilder GenerateQueryBuilder(
      TableOperation operation,
      bool? projectSystemProperties)
    {
      UriQueryBuilder queryBuilder = new UriQueryBuilder();
      if (operation.SelectColumns != null && operation.SelectColumns.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        for (int index = 0; index < operation.SelectColumns.Count; ++index)
        {
          if (operation.SelectColumns[index] == "PartitionKey")
            flag2 = true;
          else if (operation.SelectColumns[index] == "RowKey")
            flag1 = true;
          else if (operation.SelectColumns[index] == "Timestamp")
            flag3 = true;
          stringBuilder.Append(operation.SelectColumns[index]);
          if (index < operation.SelectColumns.Count - 1)
            stringBuilder.Append(",");
        }
        if (projectSystemProperties.Value)
        {
          if (!flag2)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("PartitionKey");
          }
          if (!flag1)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("RowKey");
          }
          if (!flag3)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("Timestamp");
          }
        }
        queryBuilder.Add("$select", stringBuilder.ToString());
      }
      return queryBuilder;
    }

    internal static UriQueryBuilder GenerateQueryBuilder<TInput>(
      TableQuery<TInput> query,
      bool? projectSystemProperties)
    {
      UriQueryBuilder queryBuilder = new UriQueryBuilder();
      if (!string.IsNullOrEmpty(query.FilterString))
        queryBuilder.Add("$filter", query.FilterString);
      if (query.TakeCount.HasValue)
        queryBuilder.Add("$top", Convert.ToString(query.TakeCount.Value, (IFormatProvider) CultureInfo.InvariantCulture));
      if (query.SelectColumns != null && query.SelectColumns.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        for (int index = 0; index < query.SelectColumns.Count; ++index)
        {
          if (query.SelectColumns[index] == "PartitionKey")
            flag2 = true;
          else if (query.SelectColumns[index] == "RowKey")
            flag1 = true;
          else if (query.SelectColumns[index] == "Timestamp")
            flag3 = true;
          stringBuilder.Append(query.SelectColumns[index]);
          if (index < query.SelectColumns.Count - 1)
            stringBuilder.Append(",");
        }
        if (projectSystemProperties.Value)
        {
          if (!flag2)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("PartitionKey");
          }
          if (!flag1)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("RowKey");
          }
          if (!flag3)
          {
            stringBuilder.Append(",");
            stringBuilder.Append("Timestamp");
          }
        }
        queryBuilder.Add("$select", stringBuilder.ToString());
      }
      return queryBuilder;
    }

    internal static void ApplyToUriQueryBuilder(
      TableContinuationToken token,
      UriQueryBuilder builder)
    {
      if (!string.IsNullOrEmpty(token.NextPartitionKey))
        builder.Add("NextPartitionKey", token.NextPartitionKey);
      if (!string.IsNullOrEmpty(token.NextRowKey))
        builder.Add("NextRowKey", token.NextRowKey);
      if (string.IsNullOrEmpty(token.NextTableName))
        return;
      builder.Add("NextTableName", token.NextTableName);
    }

    internal static CommandLocationMode GetListingLocationMode(TableContinuationToken token)
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

    internal static void ApplyTableRequestOptionsToStorageCommand<T>(
      TableRequestOptions options,
      RESTCommand<T> cmd)
    {
      if (options.LocationMode.HasValue)
        cmd.LocationMode = options.LocationMode.Value;
      if (options.ServerTimeout.HasValue)
        cmd.ServerTimeoutInSeconds = new int?((int) options.ServerTimeout.Value.TotalSeconds);
      if (options.OperationExpiryTime.HasValue)
      {
        cmd.OperationExpiryTime = options.OperationExpiryTime;
      }
      else
      {
        if (!options.MaximumExecutionTime.HasValue)
          return;
        cmd.OperationExpiryTime = new DateTime?(DateTime.UtcNow + options.MaximumExecutionTime.Value);
      }
    }
  }
}
