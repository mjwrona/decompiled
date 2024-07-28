// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureCloudTableClientAdapter
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureCloudTableClientAdapter : ITableClient
  {
    internal readonly CloudTableClient cloudTableClient;

    public AzureCloudTableClientAdapter(
      string accountConnectionString,
      Microsoft.Azure.Cosmos.Table.LocationMode? locationMode,
      IRetryPolicy overrideRetryPolicy)
    {
      CloudStorageAccount account = CloudStorageAccount.Parse(accountConnectionString);
      ServicePointManager.FindServicePoint(account.TableStorageUri.PrimaryUri).UpdateServicePointSettings(ServicePointConstants.MaxConnectionsPerProc16);
      ServicePointManager.FindServicePoint(account.TableStorageUri.SecondaryUri).UpdateServicePointSettings(ServicePointConstants.MaxConnectionsPerProc16);
      this.cloudTableClient = account.CreateCloudTableClient();
      this.cloudTableClient.DefaultRequestOptions.RetryPolicy = (IRetryPolicy) new AzureCloudTableClientAdapter.IncludeTimeoutsRetryPolicyWrapper(overrideRetryPolicy != null ? overrideRetryPolicy : (IRetryPolicy) new ExponentialRetry(TimeSpan.FromSeconds(4.0), 5));
      this.cloudTableClient.DefaultRequestOptions.LocationMode = locationMode;
    }

    public AzureCloudTableClientAdapter(CloudTableClient cloudTableClient) => this.cloudTableClient = cloudTableClient;

    public Microsoft.Azure.Cosmos.Table.LocationMode? LocationMode
    {
      get => this.cloudTableClient.DefaultRequestOptions.LocationMode;
      set => this.cloudTableClient.DefaultRequestOptions.LocationMode = value;
    }

    public ITable GetTableReference(string tableName) => (ITable) new AzureCloudTableAdapter(this.cloudTableClient.GetTableReference(tableName));

    public void UpdateConnectionString(string newConnectionString) => this.cloudTableClient.Credentials.UpdateKey(CloudStorageAccount.Parse(newConnectionString).Credentials.Key);

    private class IncludeTimeoutsRetryPolicyWrapper : IRetryPolicy
    {
      private readonly IRetryPolicy basePolicy;

      public IncludeTimeoutsRetryPolicyWrapper(IRetryPolicy basePolicy) => this.basePolicy = basePolicy;

      public IRetryPolicy CreateInstance() => (IRetryPolicy) new AzureCloudTableClientAdapter.IncludeTimeoutsRetryPolicyWrapper(this.basePolicy.CreateInstance());

      public bool ShouldRetry(
        int currentRetryCount,
        int statusCode,
        Exception lastException,
        out TimeSpan retryInterval,
        OperationContext operationContext)
      {
        if (statusCode == 306)
          statusCode = 500;
        return this.basePolicy.ShouldRetry(currentRetryCount, statusCode, lastException, out retryInterval, operationContext);
      }
    }
  }
}
