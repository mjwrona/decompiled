// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.DocumentClientHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Linq;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Client
{
  internal static class DocumentClientHelper
  {
    internal static async Task<Database> ReadDatabaseByIdPrivateAsync(
      this DocumentClient client,
      string databaseId,
      RequestOptions options)
    {
      IDocumentQuery<Database> q = client.CreateDatabaseQuery((FeedOptions) null).Where<Database>((Expression<Func<Database, bool>>) (c => c.Id == databaseId)).AsDocumentQuery<Database>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Database>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Database), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__0.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query Databases with id return multiple databases", (object) databaseId), (Exception) null, new HttpStatusCode?());
      }
      return (Database) null;
    }

    internal static async Task<DocumentCollection> ReadCollectionByIdPrivateAsync(
      this DocumentClient client,
      string collectionLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<DocumentCollection> q = client.CreateDocumentCollectionQuery(collectionLink, (FeedOptions) null).Where<DocumentCollection>((Expression<Func<DocumentCollection, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<DocumentCollection>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, DocumentCollection>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (DocumentCollection), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__1.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query collection with id return multiple collection", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (DocumentCollection) null;
    }

    internal static async Task<User> ReadUserByIdPrivateAsync(
      this DocumentClient client,
      string usersLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<User> q = client.CreateUserQuery(usersLink, (FeedOptions) null).Where<User>((Expression<Func<User, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<User>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, User>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (User), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__2.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query user with id return multiple uers", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (User) null;
    }

    internal static async Task<Trigger> ReadTriggerByIdPrivateAsync(
      this DocumentClient client,
      string collectionLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<Trigger> q = client.CreateTriggerQuery(collectionLink, (FeedOptions) null).Where<Trigger>((Expression<Func<Trigger, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<Trigger>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Trigger>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Trigger), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__3.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query trigger with id return multiple trigger", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (Trigger) null;
    }

    internal static async Task<StoredProcedure> ReadStoredProcedureByIdPrivateAsync(
      this DocumentClient client,
      string collectionLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<StoredProcedure> q = client.CreateStoredProcedureQuery(collectionLink, (FeedOptions) null).Where<StoredProcedure>((Expression<Func<StoredProcedure, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<StoredProcedure>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, StoredProcedure>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (StoredProcedure), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__4.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query StoredProcedure with id return multiple StoredProcedures", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (StoredProcedure) null;
    }

    internal static async Task<UserDefinedFunction> ReadUserDefinedFunctionByIdPrivateAsync(
      this DocumentClient client,
      string collsLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<UserDefinedFunction> q = client.CreateUserDefinedFunctionQuery(collsLink, (FeedOptions) null).Where<UserDefinedFunction>((Expression<Func<UserDefinedFunction, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<UserDefinedFunction>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, UserDefinedFunction>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (UserDefinedFunction), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__5.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query UserDefinedFunction with id return multiple UserDefinedFunctions", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (UserDefinedFunction) null;
    }

    internal static async Task<Conflict> ReadConflictByIdPrivateAsync(
      this DocumentClient client,
      string collsLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<Conflict> q = client.CreateConflictQuery(collsLink, (FeedOptions) null).Where<Conflict>((Expression<Func<Conflict, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<Conflict>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Conflict>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Conflict), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__6.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query conflicts with id return multiple conflicts", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (Conflict) null;
    }

    internal static async Task<Attachment> ReadAttachmentByIdPrivateAsync(
      this DocumentClient client,
      string docsLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<Attachment> q = client.CreateAttachmentQuery(docsLink, (FeedOptions) null).Where<Attachment>((Expression<Func<Attachment, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<Attachment>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Attachment>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Attachment), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__7.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query attachments with id return multiple attachments", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (Attachment) null;
    }

    internal static async Task<Permission> ReadPermissionByIdPrivateAsync(
      this DocumentClient client,
      string usersLink,
      string resourceId,
      RequestOptions options)
    {
      IDocumentQuery<Permission> q = client.CreatePermissionQuery(usersLink, (FeedOptions) null).Where<Permission>((Expression<Func<Permission, bool>>) (c => c.Id == resourceId)).AsDocumentQuery<Permission>();
      while (q.HasMoreResults)
      {
        FeedResponse<object> source = await q.ExecuteNextAsync();
        if (source.Count == 1)
        {
          // ISSUE: reference to a compiler-generated field
          if (DocumentClientHelper.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            DocumentClientHelper.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Permission>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Permission), typeof (DocumentClientHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return DocumentClientHelper.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) DocumentClientHelper.\u003C\u003Eo__8.\u003C\u003Ep__0, source.First<object>());
        }
        if (source.Count > 1)
          throw new DocumentClientException(string.Format("Query Permission with id return multiple Permission", (object) resourceId), (Exception) null, new HttpStatusCode?());
      }
      return (Permission) null;
    }
  }
}
