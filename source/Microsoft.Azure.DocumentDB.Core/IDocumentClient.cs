// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IDocumentClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  public interface IDocumentClient
  {
    object Session { get; set; }

    Uri ServiceEndpoint { get; }

    Uri WriteEndpoint { get; }

    Uri ReadEndpoint { get; }

    ConnectionPolicy ConnectionPolicy { get; }

    SecureString AuthKey { get; }

    ConsistencyLevel ConsistencyLevel { get; }

    Task<DatabaseAccount> GetDatabaseAccountAsync();

    Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      string documentLink,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      string attachmentsLink,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      Uri documentUri,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      Uri documentUri,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Database>> CreateDatabaseAsync(Database database, RequestOptions options = null);

    Task<ResourceResponse<Database>> CreateDatabaseIfNotExistsAsync(
      Database database,
      RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      RequestOptions options = null);

    Task<ResourceResponse<Document>> CreateDocumentAsync(
      string collectionLink,
      object document,
      RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> CreateDocumentAsync(
      Uri documentCollectionUri,
      object document,
      RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(
      Uri documentCollectionUri,
      StoredProcedure storedProcedure,
      RequestOptions options = null);

    Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      string collectionLink,
      Trigger trigger,
      RequestOptions options = null);

    Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      RequestOptions options = null);

    Task<ResourceResponse<User>> CreateUserAsync(
      string databaseLink,
      User user,
      RequestOptions options = null);

    Task<ResourceResponse<User>> CreateUserAsync(
      Uri databaseUri,
      User user,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> CreatePermissionAsync(
      string userLink,
      Permission permission,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> CreatePermissionAsync(
      Uri userUri,
      Permission permission,
      RequestOptions options = null);

    Task<ResourceResponse<Attachment>> DeleteAttachmentAsync(
      string attachmentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> DeleteAttachmentAsync(
      Uri attachmentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Database>> DeleteDatabaseAsync(
      string databaseLink,
      RequestOptions options = null);

    Task<ResourceResponse<Database>> DeleteDatabaseAsync(Uri databaseUri, RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      string documentCollectionLink,
      RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      Uri documentCollectionUri,
      RequestOptions options = null);

    Task<ResourceResponse<Document>> DeleteDocumentAsync(
      string documentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> DeleteDocumentAsync(
      Uri documentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      string storedProcedureLink,
      RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      Uri storedProcedureUri,
      RequestOptions options = null);

    Task<ResourceResponse<Trigger>> DeleteTriggerAsync(string triggerLink, RequestOptions options = null);

    Task<ResourceResponse<Trigger>> DeleteTriggerAsync(Uri triggerUri, RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      string functionLink,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      Uri functionUri,
      RequestOptions options = null);

    Task<ResourceResponse<User>> DeleteUserAsync(string userLink, RequestOptions options = null);

    Task<ResourceResponse<User>> DeleteUserAsync(Uri userUri, RequestOptions options = null);

    Task<ResourceResponse<Permission>> DeletePermissionAsync(
      string permissionLink,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> DeletePermissionAsync(
      Uri permissionUri,
      RequestOptions options = null);

    Task<ResourceResponse<Conflict>> DeleteConflictAsync(
      string conflictLink,
      RequestOptions options = null);

    Task<ResourceResponse<Conflict>> DeleteConflictAsync(Uri conflictUri, RequestOptions options = null);

    Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(
      Attachment attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(
      Uri attachmentUri,
      Attachment attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      DocumentCollection documentCollection,
      RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      Uri documentCollectionUri,
      DocumentCollection documentCollection,
      RequestOptions options = null);

    Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      string documentLink,
      object document,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Uri documentUri,
      object document,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Document document,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      StoredProcedure storedProcedure,
      RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      Uri storedProcedureUri,
      StoredProcedure storedProcedure,
      RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(Trigger trigger, RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(
      Uri triggerUri,
      Trigger trigger,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      UserDefinedFunction function,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      Uri userDefinedFunctionUri,
      UserDefinedFunction function,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> ReplacePermissionAsync(
      Permission permission,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> ReplacePermissionAsync(
      Uri permissionUri,
      Permission permission,
      RequestOptions options = null);

    Task<ResourceResponse<User>> ReplaceUserAsync(User user, RequestOptions options = null);

    Task<ResourceResponse<User>> ReplaceUserAsync(Uri userUri, User user, RequestOptions options = null);

    Task<ResourceResponse<Offer>> ReplaceOfferAsync(Offer offer);

    Task<MediaResponse> UpdateMediaAsync(
      string mediaLink,
      Stream mediaStream,
      MediaOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> ReadAttachmentAsync(
      string attachmentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> ReadAttachmentAsync(
      Uri attachmentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Database>> ReadDatabaseAsync(string databaseLink, RequestOptions options = null);

    Task<ResourceResponse<Database>> ReadDatabaseAsync(Uri databaseUri, RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      string documentCollectionLink,
      RequestOptions options = null);

    Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      Uri documentCollectionUri,
      RequestOptions options = null);

    Task<ResourceResponse<Document>> ReadDocumentAsync(
      string documentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> ReadDocumentAsync(
      Uri documentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      string documentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      Uri documentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      string storedProcedureLink,
      RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      Uri storedProcedureUri,
      RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReadTriggerAsync(string triggerLink, RequestOptions options = null);

    Task<ResourceResponse<Trigger>> ReadTriggerAsync(Uri triggerUri, RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      string functionLink,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      Uri functionUri,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> ReadPermissionAsync(
      string permissionLink,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> ReadPermissionAsync(
      Uri permissionUri,
      RequestOptions options = null);

    Task<ResourceResponse<User>> ReadUserAsync(string userLink, RequestOptions options = null);

    Task<ResourceResponse<User>> ReadUserAsync(Uri userUri, RequestOptions options = null);

    Task<ResourceResponse<Conflict>> ReadConflictAsync(string conflictLink, RequestOptions options = null);

    Task<ResourceResponse<Conflict>> ReadConflictAsync(Uri conflictUri, RequestOptions options = null);

    Task<ResourceResponse<Offer>> ReadOfferAsync(string offerLink);

    Task<MediaResponse> ReadMediaMetadataAsync(string mediaLink);

    Task<MediaResponse> ReadMediaAsync(string mediaLink, CancellationToken cancellationToken = default (CancellationToken));

    Task<FeedResponse<Attachment>> ReadAttachmentFeedAsync(
      string attachmentsLink,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<FeedResponse<Attachment>> ReadAttachmentFeedAsync(
      Uri documentUri,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<FeedResponse<Database>> ReadDatabaseFeedAsync(FeedOptions options = null);

    Task<FeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      string partitionKeyRangesOrCollectionLink,
      FeedOptions options = null);

    Task<FeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      Uri partitionKeyRangesOrCollectionUri,
      FeedOptions options = null);

    Task<FeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      string collectionsLink,
      FeedOptions options = null);

    Task<FeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      Uri databaseUri,
      FeedOptions options = null);

    Task<FeedResponse<object>> ReadDocumentFeedAsync(
      string documentsLink,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<FeedResponse<object>> ReadDocumentFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<FeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      string storedProceduresLink,
      FeedOptions options = null);

    Task<FeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null);

    Task<FeedResponse<Trigger>> ReadTriggerFeedAsync(string triggersLink, FeedOptions options = null);

    Task<FeedResponse<Trigger>> ReadTriggerFeedAsync(Uri documentCollectionUri, FeedOptions options = null);

    Task<FeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      string userDefinedFunctionsLink,
      FeedOptions options = null);

    Task<FeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null);

    Task<FeedResponse<Permission>> ReadPermissionFeedAsync(
      string permissionsLink,
      FeedOptions options = null);

    Task<FeedResponse<Permission>> ReadPermissionFeedAsync(Uri userUri, FeedOptions options = null);

    Task<FeedResponse<User>> ReadUserFeedAsync(string usersLink, FeedOptions options = null);

    Task<FeedResponse<User>> ReadUserFeedAsync(Uri databaseUri, FeedOptions options = null);

    Task<FeedResponse<Conflict>> ReadConflictFeedAsync(string conflictsLink, FeedOptions options = null);

    Task<FeedResponse<Conflict>> ReadConflictFeedAsync(
      Uri documentCollectionUri,
      FeedOptions options = null);

    Task<FeedResponse<Offer>> ReadOffersFeedAsync(FeedOptions options = null);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      RequestOptions options,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      RequestOptions options,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      RequestOptions options,
      CancellationToken cancellationToken,
      params object[] procedureParams);

    Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      RequestOptions options,
      CancellationToken cancellationToken,
      params object[] procedureParams);

    Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      string documentLink,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      string attachmentsLink,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      Uri documentUri,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      Uri documentUri,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> UpsertDocumentAsync(
      string collectionLink,
      object document,
      RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<Document>> UpsertDocumentAsync(
      Uri documentCollectionUri,
      object document,
      RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      RequestOptions options = null);

    Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(
      Uri documentCollectionUri,
      StoredProcedure storedProcedure,
      RequestOptions options = null);

    Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      string collectionLink,
      Trigger trigger,
      RequestOptions options = null);

    Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      RequestOptions options = null);

    Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> UpsertPermissionAsync(
      string userLink,
      Permission permission,
      RequestOptions options = null);

    Task<ResourceResponse<Permission>> UpsertPermissionAsync(
      Uri userUri,
      Permission permission,
      RequestOptions options = null);

    Task<ResourceResponse<User>> UpsertUserAsync(
      string databaseLink,
      User user,
      RequestOptions options = null);

    Task<ResourceResponse<User>> UpsertUserAsync(
      Uri databaseUri,
      User user,
      RequestOptions options = null);

    IOrderedQueryable<T> CreateAttachmentQuery<T>(Uri documentUri, FeedOptions feedOptions = null);

    IQueryable<T> CreateAttachmentQuery<T>(
      Uri documentUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<T> CreateAttachmentQuery<T>(
      Uri documentUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Attachment> CreateAttachmentQuery(Uri documentUri, FeedOptions feedOptions = null);

    IQueryable<object> CreateAttachmentQuery(
      Uri documentUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateAttachmentQuery(
      Uri documentUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

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

    IOrderedQueryable<User> CreateUserQuery(Uri documentCollectionUri, FeedOptions feedOptions = null);

    IQueryable<object> CreateUserQuery(
      Uri documentCollectionUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateUserQuery(
      Uri documentCollectionUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Permission> CreatePermissionQuery(Uri userUri, FeedOptions feedOptions = null);

    IQueryable<object> CreatePermissionQuery(
      Uri userUri,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreatePermissionQuery(
      Uri userUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<T> CreateAttachmentQuery<T>(string documentLink, FeedOptions feedOptions = null);

    IQueryable<T> CreateAttachmentQuery<T>(
      string documentLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<T> CreateAttachmentQuery<T>(
      string documentLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Attachment> CreateAttachmentQuery(
      string documentLink,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateAttachmentQuery(
      string documentLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateAttachmentQuery(
      string documentLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Database> CreateDatabaseQuery(FeedOptions feedOptions = null);

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

    IOrderedQueryable<User> CreateUserQuery(string usersLink, FeedOptions feedOptions = null);

    IQueryable<object> CreateUserQuery(
      string usersLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreateUserQuery(
      string usersLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Permission> CreatePermissionQuery(
      string permissionsLink,
      FeedOptions feedOptions = null);

    IQueryable<object> CreatePermissionQuery(
      string permissionsLink,
      string sqlExpression,
      FeedOptions feedOptions = null);

    IQueryable<object> CreatePermissionQuery(
      string permissionsLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null);

    IOrderedQueryable<Offer> CreateOfferQuery(FeedOptions feedOptions = null);

    IQueryable<object> CreateOfferQuery(string sqlExpression, FeedOptions feedOptions = null);

    IQueryable<object> CreateOfferQuery(SqlQuerySpec querySpec, FeedOptions feedOptions = null);
  }
}
