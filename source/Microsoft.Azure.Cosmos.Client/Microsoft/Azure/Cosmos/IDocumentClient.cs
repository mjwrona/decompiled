// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IDocumentClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal interface IDocumentClient
  {
    object Session { get; set; }

    Uri ServiceEndpoint { get; }

    Uri WriteEndpoint { get; }

    Uri ReadEndpoint { get; }

    ConnectionPolicy ConnectionPolicy { get; }

    SecureString AuthKey { get; }

    Microsoft.Azure.Documents.ConsistencyLevel ConsistencyLevel { get; }

    Task<AccountProperties> GetDatabaseAccountAsync();

    Task<ResourceResponse<Microsoft.Azure.Documents.Database>> CreateDatabaseAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Microsoft.Azure.Documents.Database>> CreateDatabaseIfNotExistsAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Document>> CreateDocumentAsync(
      string collectionLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> CreateDocumentAsync(
      Uri documentCollectionUri,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(
      Uri documentCollectionUri,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      string collectionLink,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Microsoft.Azure.Documents.Database>> DeleteDatabaseAsync(
      string databaseLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Microsoft.Azure.Documents.Database>> DeleteDatabaseAsync(
      Uri databaseUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      string documentCollectionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      Uri documentCollectionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Document>> DeleteDocumentAsync(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> DeleteDocumentAsync(
      Uri documentUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> DeleteTriggerAsync(string triggerLink, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> DeleteTriggerAsync(Uri triggerUri, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      string functionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      Uri functionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Conflict>> DeleteConflictAsync(
      string conflictLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Conflict>> DeleteConflictAsync(Uri conflictUri, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      Uri documentCollectionUri,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      string documentLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Uri documentUri,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Document document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      Uri storedProcedureUri,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(Trigger trigger, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(
      Uri triggerUri,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      Uri userDefinedFunctionUri,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Offer>> ReplaceOfferAsync(Offer offer);

    Task<ResourceResponse<Microsoft.Azure.Documents.Database>> ReadDatabaseAsync(
      string databaseLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Microsoft.Azure.Documents.Database>> ReadDatabaseAsync(
      Uri databaseUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      string documentCollectionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      Uri documentCollectionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Document>> ReadDocumentAsync(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> ReadDocumentAsync(
      Uri documentUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      Uri documentUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReadTriggerAsync(string triggerLink, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReadTriggerAsync(Uri triggerUri, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      string functionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      Uri functionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Conflict>> ReadConflictAsync(string conflictLink, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Conflict>> ReadConflictAsync(Uri conflictUri, Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Offer>> ReadOfferAsync(string offerLink);

    Task<DocumentFeedResponse<Microsoft.Azure.Documents.Database>> ReadDatabaseFeedAsync(
      FeedOptions options = null);

    Task<DocumentFeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      string partitionKeyRangesOrCollectionLink,
      FeedOptions options = null);

    Task<DocumentFeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      Uri partitionKeyRangesOrCollectionUri,
      FeedOptions options = null);

    Task<DocumentFeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      string collectionsLink,
      FeedOptions options = null);

    Task<DocumentFeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      Uri databaseUri,
      FeedOptions options = null);

    Task<DocumentFeedResponse<object>> ReadDocumentFeedAsync(
      string documentsLink,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<DocumentFeedResponse<object>> ReadDocumentFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<DocumentFeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      string storedProceduresLink,
      FeedOptions options = null);

    Task<DocumentFeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null);

    Task<DocumentFeedResponse<Trigger>> ReadTriggerFeedAsync(
      string triggersLink,
      FeedOptions options = null);

    Task<DocumentFeedResponse<Trigger>> ReadTriggerFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null);

    Task<DocumentFeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      string userDefinedFunctionsLink,
      FeedOptions options = null);

    Task<DocumentFeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null);

    Task<DocumentFeedResponse<Conflict>> ReadConflictFeedAsync(
      string conflictsLink,
      FeedOptions options = null);

    Task<DocumentFeedResponse<Conflict>> ReadConflictFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null);

    Task<DocumentFeedResponse<Offer>> ReadOffersFeedAsync(FeedOptions options = null);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      CancellationToken cancellationToken,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      CancellationToken cancellationToken,
      params object[] procedureParams);

    Task<ResourceResponse<Document>> UpsertDocumentAsync(
      string collectionLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> UpsertDocumentAsync(
      Uri documentCollectionUri,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(
      Uri documentCollectionUri,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      string collectionLink,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null);

    IOrderedQueryable<DocumentCollection> CreateDocumentCollectionQuery(
      Uri databaseUri,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentCollectionQuery(
      Uri databaseUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentCollectionQuery(
      Uri databaseUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<StoredProcedure> CreateStoredProcedureQuery(
      Uri documentCollectionUri,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateStoredProcedureQuery(
      Uri documentCollectionUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateStoredProcedureQuery(
      Uri documentCollectionUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Trigger> CreateTriggerQuery(
      Uri documentCollectionUri,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateTriggerQuery(
      Uri documentCollectionUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateTriggerQuery(
      Uri documentCollectionUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<UserDefinedFunction> CreateUserDefinedFunctionQuery(
      Uri documentCollectionUri,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateUserDefinedFunctionQuery(
      Uri documentCollectionUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateUserDefinedFunctionQuery(
      Uri documentCollectionUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Conflict> CreateConflictQuery(
      Uri documentCollectionUri,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateConflictQuery(
      Uri documentCollectionUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateConflictQuery(
      Uri documentCollectionUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<T> CreateDocumentQuery<T>(Uri documentCollectionUri, FeedOptions feedOptions = null);

    IQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Document> CreateDocumentQuery(
      Uri documentCollectionUri,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentQuery(
      Uri documentCollectionUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentQuery(
      Uri documentCollectionUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IDocumentQuery<Document> CreateDocumentChangeFeedQuery(
      Uri collectionLink,
      ChangeFeedOptions feedOptions);

    IOrderedQueryable<Microsoft.Azure.Documents.Database> CreateDatabaseQuery(
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDatabaseQuery(string sqlExpression, FeedOptions feedOptions = null);

    IQueryable<object> CreateDatabaseQuery(SqlQuerySpec querySpec, FeedOptions feedOptions = null);

    IOrderedQueryable<DocumentCollection> CreateDocumentCollectionQuery(
      string databaseLink,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentCollectionQuery(
      string databaseLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentCollectionQuery(
      string databaseLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<StoredProcedure> CreateStoredProcedureQuery(
      string collectionLink,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateStoredProcedureQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateStoredProcedureQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Trigger> CreateTriggerQuery(string collectionLink, FeedOptions feedOptions = null);

    IQueryable<object> CreateTriggerQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateTriggerQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<UserDefinedFunction> CreateUserDefinedFunctionQuery(
      string collectionLink,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateUserDefinedFunctionQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateUserDefinedFunctionQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Conflict> CreateConflictQuery(string collectionLink, FeedOptions feedOptions = null);

    IQueryable<object> CreateConflictQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateConflictQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<T> CreateDocumentQuery<T>(string collectionLink, FeedOptions feedOptions = null);

    IQueryable<T> CreateDocumentQuery<T>(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<T> CreateDocumentQuery<T>(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Document> CreateDocumentQuery(string collectionLink, FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateDocumentQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IDocumentQuery<Document> CreateDocumentChangeFeedQuery(
      string collectionLink,
      ChangeFeedOptions feedOptions);

    IOrderedQueryable<Offer> CreateOfferQuery(FeedOptions feedOptions = null);

    IQueryable<object> CreateOfferQuery(string sqlExpression, FeedOptions feedOptions = null);

    IQueryable<object> CreateOfferQuery(SqlQuerySpec querySpec, FeedOptions feedOptions = null);
  }
}
