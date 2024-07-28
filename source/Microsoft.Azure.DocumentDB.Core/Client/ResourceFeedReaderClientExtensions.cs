// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.ResourceFeedReaderClientExtensions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Client
{
  internal static class ResourceFeedReaderClientExtensions
  {
    public static ResourceFeedReader<Database> CreateDatabaseFeedReader(
      this DocumentClient client,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Database>(client, ResourceType.Database, options, (string) null);
    }

    public static ResourceFeedReader<Document> CreateDocumentFeedReader(
      this DocumentClient client,
      string documentsFeedOrDatabaseLink,
      FeedOptions options = null,
      object partitionKey = null)
    {
      return new ResourceFeedReader<Document>(client, ResourceType.Document, options, documentsFeedOrDatabaseLink, partitionKey);
    }

    public static ResourceFeedReader<PartitionKeyRange> CreatePartitionKeyRangeFeedReader(
      this DocumentClient client,
      string partitionKeyRangesLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<PartitionKeyRange>(client, ResourceType.PartitionKeyRange, options, partitionKeyRangesLink);
    }

    public static ResourceFeedReader<DocumentCollection> CreateDocumentCollectionFeedReader(
      this DocumentClient client,
      string collectionsLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<DocumentCollection>(client, ResourceType.Collection, options, collectionsLink);
    }

    internal static ResourceFeedReader<ClientEncryptionKey> CreateClientEncryptionKeyFeedReader(
      this DocumentClient client,
      string dbLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<ClientEncryptionKey>(client, ResourceType.ClientEncryptionKey, options, dbLink);
    }

    public static ResourceFeedReader<User> CreateUserFeedReader(
      this DocumentClient client,
      string usersLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<User>(client, ResourceType.User, options, usersLink);
    }

    public static ResourceFeedReader<UserDefinedType> CreateUserDefinedTypeFeedReader(
      this DocumentClient client,
      string userDefinedTypesLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<UserDefinedType>(client, ResourceType.UserDefinedType, options, userDefinedTypesLink);
    }

    public static ResourceFeedReader<Permission> CreatePermissionFeedReader(
      this DocumentClient client,
      string permissionsLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Permission>(client, ResourceType.Permission, options, permissionsLink);
    }

    public static ResourceFeedReader<StoredProcedure> CreateStoredProcedureFeedReader(
      this DocumentClient client,
      string storedProceduresLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<StoredProcedure>(client, ResourceType.StoredProcedure, options, storedProceduresLink);
    }

    public static ResourceFeedReader<Trigger> CreateTriggerFeedReader(
      this DocumentClient client,
      string triggersLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Trigger>(client, ResourceType.Trigger, options, triggersLink);
    }

    public static ResourceFeedReader<UserDefinedFunction> CreateUserDefinedFunctionFeedReader(
      this DocumentClient client,
      string userDefinedFunctionsLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<UserDefinedFunction>(client, ResourceType.UserDefinedFunction, options, userDefinedFunctionsLink);
    }

    public static ResourceFeedReader<Attachment> CreateAttachmentFeedReader(
      this DocumentClient client,
      string attachmentsLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Attachment>(client, ResourceType.Attachment, options, attachmentsLink);
    }

    public static ResourceFeedReader<Conflict> CreateConflictFeedReader(
      this DocumentClient client,
      string conflictsLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Conflict>(client, ResourceType.Conflict, options, conflictsLink);
    }

    internal static ResourceFeedReader<Schema> CreateSchemaFeedReader(
      this DocumentClient client,
      string schemasLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Schema>(client, ResourceType.Schema, options, schemasLink);
    }

    public static ResourceFeedReader<Offer> CreateOfferFeedReader(
      this DocumentClient client,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Offer>(client, ResourceType.Offer, options, (string) null);
    }

    public static ResourceFeedReader<Snapshot> CreateSnapshotFeedReader(
      this DocumentClient client,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Snapshot>(client, ResourceType.Snapshot, options, (string) null);
    }
  }
}
