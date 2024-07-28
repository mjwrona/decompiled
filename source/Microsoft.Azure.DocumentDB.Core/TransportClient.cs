// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TransportClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal abstract class TransportClient : IDisposable
  {
    public virtual void Dispose()
    {
    }

    public virtual Task<StoreResponse> InvokeResourceOperationAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, new ResourceOperation(request.OperationType, request.ResourceType), request);
    }

    public Task<StoreResponse> CreateOfferAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateOffer, request);

    public Task<StoreResponse> GetOfferAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadOffer, request);

    public Task<StoreResponse> ListOffersAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadOfferFeed, request);

    public Task<StoreResponse> DeleteOfferAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteOffer, request);

    public Task<StoreResponse> ReplaceOfferAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceOffer, request);
    }

    public Task<StoreResponse> QueryOfferAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Offer, request);

    public Task<StoreResponse> GetPartitionSetAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadPartitionSetInformation, request);
    }

    public Task<StoreResponse> GetRestoreMetadataFeedAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadRestoreMetadataFeed, request);
    }

    public Task<StoreResponse> GetReplicaAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadReplica, request);

    public Task<StoreResponse> ListDatabasesAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDatabaseFeed, request);
    }

    public Task<StoreResponse> HeadDatabasesAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.HeadDatabaseFeed, request);
    }

    public Task<StoreResponse> GetDatabaseAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDatabase, request);

    public Task<StoreResponse> CreateDatabaseAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateDatabase, request);
    }

    public Task<StoreResponse> UpsertDatabaseAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertDatabase, request);
    }

    public Task<StoreResponse> PatchDatabaseAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.PatchDatabase, request);
    }

    public Task<StoreResponse> ReplaceDatabaseAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceDatabase, request);
    }

    public Task<StoreResponse> DeleteDatabaseAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteDatabase, request);
    }

    public Task<StoreResponse> QueryDatabasesAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Database, request);
    }

    public Task<StoreResponse> ListDocumentCollectionsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadCollectionFeed, request);
    }

    public Task<StoreResponse> GetDocumentCollectionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadCollection, request);
    }

    public Task<StoreResponse> HeadDocumentCollectionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.HeadCollection, request);
    }

    public Task<StoreResponse> CreateDocumentCollectionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateCollection, request);
    }

    public Task<StoreResponse> PatchDocumentCollectionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.PatchCollection, request);
    }

    public Task<StoreResponse> ReplaceDocumentCollectionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceCollection, request);
    }

    public Task<StoreResponse> DeleteDocumentCollectionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteCollection, request);
    }

    public Task<StoreResponse> QueryDocumentCollectionsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Collection, request);
    }

    public Task<StoreResponse> CreateClientEncryptionKeyAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateClientEncryptionKey, request);
    }

    public Task<StoreResponse> ReadClientEncryptionKeyAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadClientEncryptionKey, request);
    }

    public Task<StoreResponse> DeleteClientEncryptionKeyAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteClientEncryptionKey, request);
    }

    public Task<StoreResponse> ReadClientEncryptionKeyFeedAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadClientEncryptionKeyFeed, request);
    }

    public Task<StoreResponse> ReplaceClientEncryptionKeyFeedAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceClientEncryptionKey, request);
    }

    public Task<StoreResponse> ListStoredProceduresAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadStoredProcedureFeed, request);
    }

    public Task<StoreResponse> GetStoredProcedureAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadStoredProcedure, request);
    }

    public Task<StoreResponse> CreateStoredProcedureAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateStoredProcedure, request);
    }

    public Task<StoreResponse> UpsertStoredProcedureAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertStoredProcedure, request);
    }

    public Task<StoreResponse> ReplaceStoredProcedureAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceStoredProcedure, request);
    }

    public Task<StoreResponse> DeleteStoredProcedureAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteStoredProcedure, request);
    }

    public Task<StoreResponse> QueryStoredProceduresAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.StoredProcedure, request);
    }

    public Task<StoreResponse> ListTriggersAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadTriggerFeed, request);
    }

    public Task<StoreResponse> GetTriggerAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadTrigger, request);

    public Task<StoreResponse> CreateTriggerAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXCreateTrigger, request);
    }

    public Task<StoreResponse> UpsertTriggerAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXUpsertTrigger, request);
    }

    public Task<StoreResponse> ReplaceTriggerAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXReplaceTrigger, request);
    }

    public Task<StoreResponse> DeleteTriggerAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXDeleteTrigger, request);
    }

    public Task<StoreResponse> QueryTriggersAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Trigger, request);
    }

    public Task<StoreResponse> ListUserDefinedFunctionsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadUserDefinedFunctionFeed, request);
    }

    public Task<StoreResponse> GetUserDefinedFunctionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXReadUserDefinedFunction, request);
    }

    public Task<StoreResponse> CreateUserDefinedFunctionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXCreateUserDefinedFunction, request);
    }

    public Task<StoreResponse> UpsertUserDefinedFunctionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXUpsertUserDefinedFunction, request);
    }

    public Task<StoreResponse> ReplaceUserDefinedFunctionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXReplaceUserDefinedFunction, request);
    }

    public Task<StoreResponse> DeleteUserDefinedFunctionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XXDeleteUserDefinedFunction, request);
    }

    public Task<StoreResponse> QueryUserDefinedFunctionsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.UserDefinedFunction, request);
    }

    public Task<StoreResponse> ListConflictsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XReadConflictFeed, request);
    }

    public Task<StoreResponse> GetConflictAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.XReadConflict, request);

    public Task<StoreResponse> DeleteConflictAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XDeleteConflict, request);
    }

    public Task<StoreResponse> QueryConflictsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Conflict, request);
    }

    public Task<StoreResponse> ListDocumentsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDocumentFeed, request);
    }

    public Task<StoreResponse> GetDocumentAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadDocument, request);

    public Task<StoreResponse> CreateDocumentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateDocument, request);
    }

    public Task<StoreResponse> UpsertDocumentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertDocument, request);
    }

    public Task<StoreResponse> PatchDocumentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.PatchDocument, request);
    }

    public Task<StoreResponse> ReplaceDocumentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceDocument, request);
    }

    public Task<StoreResponse> DeleteDocumentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteDocument, request);
    }

    public Task<StoreResponse> QueryDocumentsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Document, request);
    }

    public Task<StoreResponse> ListAttachmentsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadAttachmentFeed, request);
    }

    public Task<StoreResponse> GetAttachmentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadAttachment, request);
    }

    public Task<StoreResponse> CreateAttachmentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateAttachment, request);
    }

    public Task<StoreResponse> UpsertAttachmentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertAttachment, request);
    }

    public Task<StoreResponse> ReplaceAttachmentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceAttachment, request);
    }

    public Task<StoreResponse> DeleteAttachmentAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteAttachment, request);
    }

    public Task<StoreResponse> QueryAttachmentsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Attachment, request);
    }

    public Task<StoreResponse> ListUsersAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadUserFeed, request);

    public Task<StoreResponse> GetUserAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadUser, request);

    public Task<StoreResponse> CreateUserAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreateUser, request);

    public Task<StoreResponse> UpsertUserAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertUser, request);

    public Task<StoreResponse> PatchUserAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.PatchUser, request);

    public Task<StoreResponse> ReplaceUserAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplaceUser, request);

    public Task<StoreResponse> DeleteUserAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeleteUser, request);

    public Task<StoreResponse> QueryUsersAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeQueryStoreAsync(physicalAddress, ResourceType.User, request);

    public Task<StoreResponse> ListPermissionsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadPermissionFeed, request);
    }

    public Task<StoreResponse> GetPermissionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReadPermission, request);
    }

    public Task<StoreResponse> CreatePermissionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.CreatePermission, request);
    }

    public Task<StoreResponse> UpsertPermissionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.UpsertPermission, request);
    }

    public Task<StoreResponse> PatchPermissionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.PatchPermission, request);
    }

    public Task<StoreResponse> ReplacePermissionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.ReplacePermission, request);
    }

    public Task<StoreResponse> DeletePermissionAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.DeletePermission, request);
    }

    public Task<StoreResponse> QueryPermissionsAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeQueryStoreAsync(physicalAddress, ResourceType.Permission, request);
    }

    public Task<StoreResponse> ListRecordsAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.XReadRecordFeed, request);

    public Task<StoreResponse> CreateRecordAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XCreateRecord, request);
    }

    public Task<StoreResponse> ReadRecordAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.XReadRecord, request);

    public Task<StoreResponse> PatchRecordAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.XUpdateRecord, request);

    public Task<StoreResponse> DeleteRecordAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.XDeleteRecord, request);
    }

    public Task<StoreResponse> ExecuteAsync(Uri physicalAddress, DocumentServiceRequest request) => this.InvokeStoreAsync(physicalAddress, ResourceOperation.ExecuteDocumentFeed, request);

    public static void ThrowServerException(
      string resourceAddress,
      StoreResponse storeResponse,
      Uri physicalAddress,
      Guid activityId,
      DocumentServiceRequest request = null)
    {
      if (storeResponse.Status >= 300 && storeResponse.Status != 304 && (request == null || !request.IsValidStatusCodeForExceptionlessRetry(storeResponse.Status, storeResponse.SubStatusCode)))
      {
        DocumentClientException documentClientException;
        switch ((StatusCodes) storeResponse.Status)
        {
          case StatusCodes.StartingErrorCode:
            INameValueCollection responseHeaders1;
            documentClientException = (DocumentClientException) new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.BadRequest, out responseHeaders1)), responseHeaders1, physicalAddress);
            break;
          case StatusCodes.Unauthorized:
            INameValueCollection responseHeaders2;
            documentClientException = (DocumentClientException) new UnauthorizedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.Unauthorized, out responseHeaders2)), responseHeaders2, physicalAddress);
            break;
          case StatusCodes.Forbidden:
            INameValueCollection responseHeaders3;
            documentClientException = (DocumentClientException) new ForbiddenException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.Forbidden, out responseHeaders3)), responseHeaders3, physicalAddress);
            break;
          case StatusCodes.NotFound:
            INameValueCollection responseHeaders4;
            documentClientException = (DocumentClientException) new NotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.NotFound, out responseHeaders4)), responseHeaders4, physicalAddress);
            break;
          case StatusCodes.MethodNotAllowed:
            INameValueCollection responseHeaders5;
            documentClientException = (DocumentClientException) new MethodNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.MethodNotAllowed, out responseHeaders5)), responseHeaders5, physicalAddress);
            break;
          case StatusCodes.RequestTimeout:
            INameValueCollection responseHeaders6;
            documentClientException = (DocumentClientException) new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.RequestTimeout, out responseHeaders6)), responseHeaders6, physicalAddress);
            break;
          case StatusCodes.Conflict:
            INameValueCollection responseHeaders7;
            documentClientException = (DocumentClientException) new ConflictException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.EntityAlreadyExists, out responseHeaders7)), responseHeaders7, physicalAddress);
            break;
          case StatusCodes.Gone:
            TransportClient.LogGoneException(physicalAddress, activityId.ToString());
            INameValueCollection responseHeaders8;
            string errorResponse = TransportClient.GetErrorResponse(storeResponse, RMResources.Gone, out responseHeaders8);
            uint result = 0;
            string s = responseHeaders8.Get("x-ms-substatus");
            if (!string.IsNullOrEmpty(s) && !uint.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            {
              documentClientException = (DocumentClientException) new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponse) ? (object) RMResources.BadRequest : (object) errorResponse), responseHeaders8, physicalAddress);
              break;
            }
            switch (result)
            {
              case 1000:
                documentClientException = (DocumentClientException) new InvalidPartitionException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) errorResponse), responseHeaders8, physicalAddress);
                break;
              case 1002:
                documentClientException = (DocumentClientException) new PartitionKeyRangeGoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) errorResponse), responseHeaders8, physicalAddress);
                break;
              case 1007:
                documentClientException = (DocumentClientException) new PartitionKeyRangeIsSplittingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) errorResponse), responseHeaders8, physicalAddress);
                break;
              case 1008:
                documentClientException = (DocumentClientException) new PartitionIsMigratingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) errorResponse), responseHeaders8, physicalAddress);
                break;
              default:
                documentClientException = (DocumentClientException) new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), responseHeaders8, physicalAddress);
                break;
            }
            break;
          case StatusCodes.PreconditionFailed:
            INameValueCollection responseHeaders9;
            documentClientException = (DocumentClientException) new PreconditionFailedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.PreconditionFailed, out responseHeaders9)), responseHeaders9, physicalAddress);
            break;
          case StatusCodes.RequestEntityTooLarge:
            INameValueCollection responseHeaders10;
            documentClientException = (DocumentClientException) new RequestEntityTooLargeException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.RequestEntityTooLarge, (object) "x-ms-max-item-count"), out responseHeaders10)), responseHeaders10, physicalAddress);
            break;
          case StatusCodes.Locked:
            INameValueCollection responseHeaders11;
            documentClientException = (DocumentClientException) new LockedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.Locked, out responseHeaders11)), responseHeaders11, physicalAddress);
            break;
          case StatusCodes.TooManyRequests:
            INameValueCollection responseHeaders12;
            documentClientException = (DocumentClientException) new RequestRateTooLargeException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.TooManyRequests, out responseHeaders12)), responseHeaders12, physicalAddress);
            break;
          case StatusCodes.RetryWith:
            INameValueCollection responseHeaders13;
            documentClientException = (DocumentClientException) new RetryWithException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.RetryWith, out responseHeaders13)), responseHeaders13, physicalAddress);
            break;
          case StatusCodes.InternalServerError:
            INameValueCollection responseHeaders14;
            documentClientException = (DocumentClientException) new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.InternalServerError, out responseHeaders14)), responseHeaders14, physicalAddress);
            break;
          case StatusCodes.ServiceUnavailable:
            INameValueCollection responseHeaders15;
            documentClientException = (DocumentClientException) new ServiceUnavailableException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.ServiceUnavailable, out responseHeaders15)), responseHeaders15, physicalAddress);
            break;
          default:
            DefaultTrace.TraceCritical("Unrecognized status code {0} returned by backend. ActivityId {1}", (object) storeResponse.Status, (object) activityId);
            TransportClient.LogException((Exception) null, physicalAddress, resourceAddress, activityId);
            INameValueCollection responseHeaders16;
            documentClientException = (DocumentClientException) new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) TransportClient.GetErrorResponse(storeResponse, RMResources.InvalidBackendResponse, out responseHeaders16)), responseHeaders16, physicalAddress);
            break;
        }
        documentClientException.LSN = storeResponse.LSN;
        documentClientException.PartitionKeyRangeId = storeResponse.PartitionKeyRangeId;
        documentClientException.ResourceAddress = resourceAddress;
        throw documentClientException;
      }
    }

    protected Task<StoreResponse> InvokeQueryStoreAsync(
      Uri physicalAddress,
      ResourceType resourceType,
      DocumentServiceRequest request)
    {
      OperationType operationType = !string.Equals(request.Headers["Content-Type"], "application/sql", StringComparison.Ordinal) ? OperationType.Query : OperationType.SqlQuery;
      return this.InvokeStoreAsync(physicalAddress, ResourceOperation.Query(operationType, resourceType), request);
    }

    internal abstract Task<StoreResponse> InvokeStoreAsync(
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request);

    protected static async Task<string> GetErrorResponseAsync(HttpResponseMessage responseMessage) => responseMessage.Content != null ? TransportClient.GetErrorFromStream(await responseMessage.Content.ReadAsStreamAsync()) : "";

    protected static string GetErrorResponse(
      StoreResponse storeResponse,
      string defaultMessage,
      out INameValueCollection responseHeaders)
    {
      string str = (string) null;
      responseHeaders = (INameValueCollection) new DictionaryNameValueCollection();
      if (storeResponse.ResponseBody != null)
        str = TransportClient.GetErrorFromStream(storeResponse.ResponseBody);
      if (storeResponse.ResponseHeaderNames != null)
      {
        for (int index = 0; index < ((IEnumerable<string>) storeResponse.ResponseHeaderNames).Count<string>(); ++index)
          responseHeaders.Add(storeResponse.ResponseHeaderNames[index], storeResponse.ResponseHeaderValues[index]);
      }
      return !string.IsNullOrEmpty(str) ? str : defaultMessage;
    }

    protected static string GetErrorFromStream(Stream responseStream)
    {
      using (responseStream)
        return new StreamReader(responseStream).ReadToEnd();
    }

    protected static void LogException(Uri physicalAddress, string activityId) => DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Store Request Failed. Store Physical Address {0} ActivityId {1}", (object) physicalAddress, (object) activityId));

    protected static void LogException(
      Exception exception,
      Uri physicalAddress,
      string rid,
      Guid activityId)
    {
      if (exception != null)
        DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Store Request Failed. Exception {0} Store Physical Address {1} RID {2} ActivityId {3}", (object) exception.Message, (object) physicalAddress, (object) rid, (object) activityId.ToString()));
      else
        DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Store Request Failed. Store Physical Address {0} RID {1} ActivityId {2}", (object) physicalAddress, (object) rid, (object) activityId.ToString()));
    }

    protected static void LogGoneException(Uri physicalAddress, string activityId) => DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Listener not found. Store Physical Address {0} ActivityId {1}", (object) physicalAddress, (object) activityId));
  }
}
