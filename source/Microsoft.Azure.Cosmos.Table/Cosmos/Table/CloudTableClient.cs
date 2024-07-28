// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.CloudTableClient
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.Extensions;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Azure.Cosmos.Table.RestExecutor;
using Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table
{
  public class CloudTableClient
  {
    private string _accountName;
    internal static Func<Uri, string, ConnectionPolicy, Microsoft.Azure.Documents.ConsistencyLevel?, IDocumentClient> DocClientCreator = (Func<Uri, string, ConnectionPolicy, Microsoft.Azure.Documents.ConsistencyLevel?, IDocumentClient>) ((accountUri, key, connectionPolicy, consistencyLevel) => (IDocumentClient) new Microsoft.Azure.Documents.Client.DocumentClient(accountUri, key, EntityTranslator.JsonSerializerSettings, connectionPolicy, consistencyLevel));
    private Lazy<IDocumentClient> lazyDocumentClient;
    private Lazy<HttpClient> lazyHttpClient;
    internal const string LegacyCosmosTableDomain = ".table.cosmosdb.";
    internal const string CosmosTableDomain = ".table.cosmos.";
    internal const string CosmosDocumentsDomain = ".documents.";
    internal static readonly Dictionary<string, string> ReplaceMapping = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      {
        ".table.cosmosdb.windows-int.net",
        ".documents-test.windows-int.net"
      },
      {
        ".table.cosmos.windows-int.net",
        ".documents-test.windows-int.net"
      },
      {
        ".table.cosmosdb.windows-ppe.net",
        ".documents-staging.windows-ppe.net"
      },
      {
        ".table.cosmos.windows-ppe.net",
        ".documents-staging.windows-ppe.net"
      },
      {
        ".table.cosmosdb.",
        ".documents."
      },
      {
        ".table.cosmos.",
        ".documents."
      }
    };

    public CloudTableClient(
      Uri baseUri,
      StorageCredentials credentials,
      TableClientConfiguration configuration = null)
      : this(new StorageUri(baseUri), credentials, configuration)
    {
    }

    public CloudTableClient(StorageUri storageUri, StorageCredentials credentials)
      : this(storageUri, credentials, (TableClientConfiguration) null)
    {
    }

    public CloudTableClient(
      StorageUri storageUri,
      StorageCredentials credentials,
      TableClientConfiguration configuration = null)
    {
      this.StorageUri = storageUri;
      this.TableClientConfiguration = configuration ?? new TableClientConfiguration();
      this.Credentials = credentials ?? new StorageCredentials();
      this.DefaultRequestOptions = new TableRequestOptions(TableRequestOptions.BaseDefaultRequestOptions)
      {
        RetryPolicy = (IRetryPolicy) new ExponentialRetry(),
        ConsistencyLevel = (Microsoft.Azure.Cosmos.ConsistencyLevel?) configuration?.CosmosExecutorConfiguration?.ConsistencyLevel
      };
      this.InitializeExecutor();
      this.UsePathStyleUris = CommonUtility.UsePathStyleAddressing(this.BaseUri);
      if (!this.Credentials.IsSharedKey)
        this.AccountName = NavigationHelper.GetAccountNameFromUri(this.BaseUri, new bool?(this.UsePathStyleUris));
      this.lazyDocumentClient = new Lazy<IDocumentClient>(new Func<IDocumentClient>(this.CreateDocumentClient));
      this.lazyHttpClient = new Lazy<HttpClient>(new Func<HttpClient>(this.CreateHttpClient));
    }

    public StorageCredentials Credentials { get; private set; }

    public TableClientConfiguration TableClientConfiguration { get; private set; }

    public Uri BaseUri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public TableRequestOptions DefaultRequestOptions { get; set; }

    internal bool UsePathStyleUris { get; private set; }

    internal string AccountName
    {
      get => this._accountName ?? this.Credentials.AccountName;
      private set => this._accountName = value;
    }

    internal IExecutor Executor { get; set; }

    public virtual CloudTable GetTableReference(string tableName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (tableName), tableName);
      return new CloudTable(tableName, this);
    }

    public virtual IEnumerable<CloudTable> ListTables(
      string prefix = null,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      CloudTable tableReference = this.GetTableReference("Tables");
      return CloudTableClient.GenerateListTablesQuery(prefix, new int?()).ExecuteInternal(this, tableReference, requestOptions, operationContext).Select<DynamicTableEntity, CloudTable>((Func<DynamicTableEntity, CloudTable>) (tbl => new CloudTable(tbl["TableName"].StringValue, this)));
    }

    public virtual TableResultSegment ListTablesSegmented(TableContinuationToken currentToken) => this.ListTablesSegmented((string) null, currentToken);

    public virtual TableResultSegment ListTablesSegmented(
      string prefix,
      TableContinuationToken currentToken)
    {
      return this.ListTablesSegmented(prefix, new int?(), currentToken);
    }

    public virtual TableResultSegment ListTablesSegmented(
      string prefix,
      int? maxResults,
      TableContinuationToken currentToken,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      CloudTable tableReference = this.GetTableReference("Tables");
      TableQuerySegment<DynamicTableEntity> tableQuerySegment = CloudTableClient.GenerateListTablesQuery(prefix, maxResults).ExecuteQuerySegmentedInternal(currentToken, this, tableReference, requestOptions, operationContext);
      return new TableResultSegment(tableQuerySegment.Results.Select<DynamicTableEntity, CloudTable>((Func<DynamicTableEntity, CloudTable>) (tbl => new CloudTable(tbl.Properties["TableName"].StringValue, this))).ToList<CloudTable>())
      {
        ContinuationToken = tableQuerySegment.ContinuationToken
      };
    }

    public virtual Task<TableResultSegment> ListTablesSegmentedAsync(
      TableContinuationToken currentToken)
    {
      return this.ListTablesSegmentedAsync(currentToken, CancellationToken.None);
    }

    public virtual Task<TableResultSegment> ListTablesSegmentedAsync(
      TableContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListTablesSegmentedAsync((string) null, currentToken, cancellationToken);
    }

    public virtual Task<TableResultSegment> ListTablesSegmentedAsync(
      string prefix,
      TableContinuationToken currentToken)
    {
      return this.ListTablesSegmentedAsync(prefix, currentToken, CancellationToken.None);
    }

    public virtual Task<TableResultSegment> ListTablesSegmentedAsync(
      string prefix,
      TableContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListTablesSegmentedAsync(prefix, new int?(), currentToken, cancellationToken);
    }

    public virtual Task<TableResultSegment> ListTablesSegmentedAsync(
      string prefix,
      int? maxResults,
      TableContinuationToken currentToken,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.ListTablesSegmentedAsync(prefix, maxResults, currentToken, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<TableResultSegment> ListTablesSegmentedAsync(
      string prefix,
      int? maxResults,
      TableContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListTablesSegmentedAsync(prefix, maxResults, currentToken, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual async Task<TableResultSegment> ListTablesSegmentedAsync(
      string prefix,
      int? maxResults,
      TableContinuationToken currentToken,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudTableClient cloudTableClient = this;
      try
      {
        requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, cloudTableClient);
        operationContext = operationContext ?? new OperationContext();
        CloudTable tableReference = cloudTableClient.GetTableReference("Tables");
        TableQuerySegment<DynamicTableEntity> tableQuerySegment = await CloudTableClient.GenerateListTablesQuery(prefix, maxResults).ExecuteQuerySegmentedInternalAsync(currentToken, cloudTableClient, tableReference, requestOptions, operationContext, cancellationToken);
        // ISSUE: reference to a compiler-generated method
        return new TableResultSegment(tableQuerySegment.Results.Select<DynamicTableEntity, CloudTable>(new Func<DynamicTableEntity, CloudTable>(cloudTableClient.\u003CListTablesSegmentedAsync\u003Eb__44_0)).ToList<CloudTable>())
        {
          ContinuationToken = tableQuerySegment.ContinuationToken
        };
      }
      catch (StorageException ex)
      {
        int num1;
        if (ex == null)
        {
          num1 = 0;
        }
        else
        {
          int? httpStatusCode = ex.RequestInformation?.HttpStatusCode;
          int num2 = 404;
          num1 = httpStatusCode.GetValueOrDefault() == num2 & httpStatusCode.HasValue ? 1 : 0;
        }
        if (num1 != 0)
          return new TableResultSegment(new List<CloudTable>());
        throw;
      }
    }

    internal ExpressionParser GetExpressionParser() => this.IsPremiumEndpoint() ? (ExpressionParser) new TableExtensionExpressionParser() : new ExpressionParser();

    internal bool IsPremiumEndpoint()
    {
      CommonUtility.AssertNotNull("StorageUri", (object) this.StorageUri);
      string lowerInvariant = this.StorageUri.PrimaryUri.OriginalString.ToLowerInvariant();
      return lowerInvariant.Contains("https://localhost") && this.StorageUri.PrimaryUri.Port != 10002 || lowerInvariant.Contains(".table.cosmosdb.") || lowerInvariant.Contains(".table.cosmos.");
    }

    internal bool IsDocumentsEndPoint()
    {
      CommonUtility.AssertNotNull("StorageUri", (object) this.StorageUri);
      return this.StorageUri.PrimaryUri.OriginalString.ToLowerInvariant().Contains(".documents.");
    }

    private static TableQuery<DynamicTableEntity> GenerateListTablesQuery(
      string prefix,
      int? maxResults)
    {
      TableQuery<DynamicTableEntity> listTablesQuery = new TableQuery<DynamicTableEntity>();
      if (!string.IsNullOrEmpty(prefix))
      {
        string givenValue = prefix + "{";
        listTablesQuery = listTablesQuery.Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("TableName", "ge", prefix), "and", TableQuery.GenerateFilterCondition("TableName", "lt", givenValue)));
      }
      if (maxResults.HasValue)
        listTablesQuery = listTablesQuery.Take(new int?(maxResults.Value));
      return listTablesQuery;
    }

    public virtual ServiceProperties GetServiceProperties(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return this.Executor.GetServicePropertiesOperation(this, requestOptions, operationContext);
    }

    public virtual Task<ServiceProperties> GetServicePropertiesAsync() => this.GetServicePropertiesAsync(CancellationToken.None);

    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetServicePropertiesAsync((TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetServicePropertiesAsync(requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<ServiceProperties> GetServicePropertiesAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return this.Executor.GetServicePropertiesOperationAsync(this, requestOptions, operationContext, cancellationToken);
    }

    public virtual void SetServiceProperties(
      ServiceProperties properties,
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      this.Executor.SetServicePropertiesOperation(properties, this, requestOptions, operationContext);
    }

    public virtual Task SetServicePropertiesAsync(ServiceProperties properties) => this.SetServicePropertiesAsync(properties, CancellationToken.None);

    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      CancellationToken cancellationToken)
    {
      return this.SetServicePropertiesAsync(properties, (TableRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.SetServicePropertiesAsync(properties, requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task SetServicePropertiesAsync(
      ServiceProperties properties,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return this.Executor.SetServicePropertiesOperationAsync(properties, this, requestOptions, operationContext, cancellationToken);
    }

    public virtual ServiceStats GetServiceStats(
      TableRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return this.Executor.GetServiceStats(this, requestOptions, operationContext);
    }

    public virtual Task<ServiceStats> GetServiceStatsAsync() => this.GetServiceStatsAsync(CancellationToken.None);

    public virtual Task<ServiceStats> GetServiceStatsAsync(CancellationToken cancellationToken) => this.GetServiceStatsAsync((TableRequestOptions) null, (OperationContext) null, cancellationToken);

    public virtual Task<ServiceStats> GetServiceStatsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetServiceStatsAsync(requestOptions, operationContext, CancellationToken.None);
    }

    public virtual Task<ServiceStats> GetServiceStatsAsync(
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this);
      operationContext = operationContext ?? new OperationContext();
      return this.Executor.GetServiceStatsAsync(this, requestOptions, operationContext, cancellationToken);
    }

    internal HttpClient HttpClient => this.lazyHttpClient.Value;

    internal IDocumentClient DocumentClient => this.lazyDocumentClient.Value;

    private IDocumentClient CreateDocumentClient()
    {
      if (!this.IsPremiumEndpoint())
        throw new NotSupportedException(string.Format("{0} api is not supported in the current version.", (object) "Direct mode"));
      CosmosExecutorConfiguration executorConfiguration = this.TableClientConfiguration.CosmosExecutorConfiguration;
      ConnectionPolicy connectionPolicy = executorConfiguration.GetConnectionPolicy();
      Uri docDbDirectUrl = CloudTableClient.GetDocDbDirectUrl(this.StorageUri.PrimaryUri);
      return CloudTableClient.DocClientCreator(docDbDirectUrl, this.Credentials.Key, connectionPolicy, CloudTableClient.ToDocDbConsistencyLevel(executorConfiguration.ConsistencyLevel));
    }

    private HttpClient CreateHttpClient() => HttpClientFactory.HttpClientFromConfiguration(this.TableClientConfiguration.RestExecutorConfiguration);

    internal static Microsoft.Azure.Documents.ConsistencyLevel? ToDocDbConsistencyLevel(
      Microsoft.Azure.Cosmos.ConsistencyLevel? consistencyLevel)
    {
      return consistencyLevel.HasValue ? new Microsoft.Azure.Documents.ConsistencyLevel?((Microsoft.Azure.Documents.ConsistencyLevel) consistencyLevel.Value) : new Microsoft.Azure.Documents.ConsistencyLevel?();
    }

    internal static string ConvertToDocdbEndpoint(string tableEndpoint)
    {
      foreach (KeyValuePair<string, string> keyValuePair in CloudTableClient.ReplaceMapping)
      {
        if (tableEndpoint.Contains(keyValuePair.Key))
          return tableEndpoint.Replace(keyValuePair.Key, keyValuePair.Value);
      }
      return tableEndpoint;
    }

    internal static Uri GetDocDbDirectUrl(Uri tableUri)
    {
      string docdbEndpoint = CloudTableClient.ConvertToDocdbEndpoint(tableUri.Host);
      return new Uri(string.Format("{0}://{1}:{2}{3}", (object) tableUri.Scheme, (object) docdbEndpoint, (object) tableUri.Port, (object) tableUri.PathAndQuery));
    }

    internal void InitializeExecutor()
    {
      if (this.IsDocumentsEndPoint())
        throw new NotSupportedException("Only Cosmos table endpoint or azure storage table endpoint are supported.");
      if (this.IsPremiumEndpoint() && !this.TableClientConfiguration.UseRestExecutorForCosmosEndpoint)
        this.Executor = (IExecutor) new TableExtensionExecutor();
      else
        this.Executor = (IExecutor) new TableRestExecutor();
    }

    internal void AssertPremiumFeaturesOnlyToCosmosTables(
      int? throughput,
      string serializedIndexingPolicy)
    {
      if ((throughput.HasValue || serializedIndexingPolicy != null) && !this.IsPremiumEndpoint())
        throw new NotSupportedException("Only direct mode supports throughput and indexing policy.");
    }
  }
}
