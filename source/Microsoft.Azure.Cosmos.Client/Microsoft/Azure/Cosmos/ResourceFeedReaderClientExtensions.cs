// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ResourceFeedReaderClientExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Cosmos
{
  internal static class ResourceFeedReaderClientExtensions
  {
    public static ResourceFeedReader<Microsoft.Azure.Documents.Database> CreateDatabaseFeedReader(
      this DocumentClient client,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Microsoft.Azure.Documents.Database>(client, ResourceType.Database, options, (string) null);
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

    public static ResourceFeedReader<Microsoft.Azure.Documents.User> CreateUserFeedReader(
      this DocumentClient client,
      string usersLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Microsoft.Azure.Documents.User>(client, ResourceType.User, options, usersLink);
    }

    public static ResourceFeedReader<UserDefinedType> CreateUserDefinedTypeFeedReader(
      this DocumentClient client,
      string userDefinedTypesLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<UserDefinedType>(client, ResourceType.UserDefinedType, options, userDefinedTypesLink);
    }

    public static ResourceFeedReader<Microsoft.Azure.Documents.Permission> CreatePermissionFeedReader(
      this DocumentClient client,
      string permissionsLink,
      FeedOptions options = null)
    {
      return new ResourceFeedReader<Microsoft.Azure.Documents.Permission>(client, ResourceType.Permission, options, permissionsLink);
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
