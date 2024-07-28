// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IAzureTableStorageProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public interface IAzureTableStorageProvider
  {
    string FindNextTableName(IVssRequestContext requestContext, string tableName);

    string FindPreviousTableName(IVssRequestContext requestContext, string tableName);

    bool CreateIfNotExists(IVssRequestContext requestContext, string tableName);

    void DeleteIfExists(IVssRequestContext requestContext, string tableName);

    void CreateSharedAccessPolicy(
      IVssRequestContext requestContext,
      string tableName,
      string policyName,
      DateTime expiryDate,
      SharedAccessTablePermissions permissions);

    SharedAccessTablePolicies GetSharedAccessPolicies(
      IVssRequestContext requestContext,
      string tableName);

    void DeleteSharedAccessPolicies(
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<string> policyName);

    string GetSasTokenForPolicy(
      IVssRequestContext requestContext,
      string tableName,
      string policyName);

    IList<T> QueryTable<T>(
      IVssRequestContext requestContext,
      string tableName,
      TableQuery<T> query,
      ref TableContinuationToken continuationToken)
      where T : ITableEntity, new();

    TableResult ExecuteTableOperation(
      IVssRequestContext requestContext,
      string tableName,
      TableOperation tableOperation);

    Task<TableResult> ExecuteTableOperationAsync(
      IVssRequestContext requestContext,
      string tableName,
      TableOperation tableOperation);

    IList<TableResult> ExecuteBatchOperation(
      IVssRequestContext requestContext,
      string tableName,
      TableBatchOperation batchOperation);

    void UpdateConnectionString(
      IVssRequestContext requestContext,
      string storageAccountConnectionString);
  }
}
