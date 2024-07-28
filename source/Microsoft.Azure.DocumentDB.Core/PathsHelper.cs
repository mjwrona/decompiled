// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PathsHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class PathsHelper
  {
    private static bool isClientSideValidationEnabled = true;

    public static bool TryParsePathSegments(
      string resourceUrl,
      out bool isFeed,
      out string resourcePath,
      out string resourceIdOrFullName,
      out bool isNameBased,
      string clientVersion = "")
    {
      string databaseName = string.Empty;
      string collectionName = string.Empty;
      return PathsHelper.TryParsePathSegmentsWithDatabaseAndCollectionNames(resourceUrl, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased, out databaseName, out collectionName, clientVersion);
    }

    public static bool TryParsePathSegmentsWithDatabaseAndCollectionNames(
      string resourceUrl,
      out bool isFeed,
      out string resourcePath,
      out string resourceIdOrFullName,
      out bool isNameBased,
      out string databaseName,
      out string collectionName,
      string clientVersion = "",
      bool parseDatabaseAndCollectionNames = false)
    {
      resourcePath = string.Empty;
      resourceIdOrFullName = string.Empty;
      isFeed = false;
      isNameBased = false;
      databaseName = string.Empty;
      collectionName = string.Empty;
      if (string.IsNullOrEmpty(resourceUrl))
        return false;
      string[] segments = resourceUrl.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (segments == null || segments.Length < 1)
        return false;
      int length = segments.Length;
      string str1 = segments[length - 1].Trim('/');
      string str2;
      if (length < 2)
        str2 = string.Empty;
      else
        str2 = segments[length - 2].Trim('/');
      string str3 = str2;
      if (PathsHelper.IsRootOperation(str3, str1) || PathsHelper.IsTopLevelOperationOperation(str3, str1))
      {
        isFeed = false;
        resourceIdOrFullName = string.Empty;
        resourcePath = "/";
        return true;
      }
      if (length >= 2)
      {
        ResourceId rid;
        if (segments[0].Equals("dbs", StringComparison.OrdinalIgnoreCase) && (!ResourceId.TryParse(segments[1], out rid) || !rid.IsDatabaseId))
          isNameBased = true;
        else if (segments[0].Equals("snapshots", StringComparison.OrdinalIgnoreCase) && (!ResourceId.TryParse(segments[1], out rid) || !rid.IsSnapshotId))
          isNameBased = true;
        if (isNameBased)
          return PathsHelper.TryParseNameSegments(resourceUrl, segments, out isFeed, out resourcePath, out resourceIdOrFullName, out databaseName, out collectionName, parseDatabaseAndCollectionNames);
      }
      if (length % 2 != 0 && PathsHelper.IsResourceType(str1))
      {
        isFeed = true;
        resourcePath = str1;
        if (!str1.Equals("dbs", StringComparison.OrdinalIgnoreCase))
          resourceIdOrFullName = str3;
      }
      else
      {
        if (!PathsHelper.IsResourceType(str3))
          return false;
        isFeed = false;
        resourcePath = str3;
        resourceIdOrFullName = str1;
        if (!string.IsNullOrEmpty(clientVersion) && resourcePath.Equals("media", StringComparison.OrdinalIgnoreCase))
        {
          string attachmentId = (string) null;
          byte storageIndex = 0;
          if (!MediaIdHelper.TryParseMediaId(resourceIdOrFullName, out attachmentId, out storageIndex))
            return false;
          resourceIdOrFullName = attachmentId;
        }
      }
      return true;
    }

    public static void ParseDatabaseNameAndCollectionNameFromUrlSegments(
      string[] segments,
      out string databaseName,
      out string collectionName)
    {
      databaseName = string.Empty;
      collectionName = string.Empty;
      if (segments == null || segments.Length < 2 || !string.Equals(segments[0], "dbs", StringComparison.OrdinalIgnoreCase))
        return;
      databaseName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(segments[1])));
      if (segments.Length < 4 || !string.Equals(segments[2], "colls", StringComparison.OrdinalIgnoreCase))
        return;
      collectionName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(segments[3])));
    }

    private static bool TryParseNameSegments(
      string resourceUrl,
      string[] segments,
      out bool isFeed,
      out string resourcePath,
      out string resourceFullName,
      out string databaseName,
      out string collectionName,
      bool parseDatabaseAndCollectionNames)
    {
      isFeed = false;
      resourcePath = string.Empty;
      resourceFullName = string.Empty;
      databaseName = string.Empty;
      collectionName = string.Empty;
      if (segments == null || segments.Length < 1)
        return false;
      if (segments.Length % 2 == 0)
      {
        if (PathsHelper.IsResourceType(segments[segments.Length - 2]))
        {
          resourcePath = segments[segments.Length - 2];
          resourceFullName = resourceUrl;
          resourceFullName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(resourceFullName)));
          if (parseDatabaseAndCollectionNames)
            PathsHelper.ParseDatabaseNameAndCollectionNameFromUrlSegments(segments, out databaseName, out collectionName);
          return true;
        }
      }
      else if (PathsHelper.IsResourceType(segments[segments.Length - 1]))
      {
        isFeed = true;
        resourcePath = segments[segments.Length - 1];
        resourceFullName = resourceUrl.Substring(0, UrlUtility.RemoveTrailingSlashes(resourceUrl).LastIndexOf("/", StringComparison.CurrentCultureIgnoreCase));
        resourceFullName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(resourceFullName)));
        if (parseDatabaseAndCollectionNames)
          PathsHelper.ParseDatabaseNameAndCollectionNameFromUrlSegments(segments, out databaseName, out collectionName);
        return true;
      }
      return false;
    }

    public static ResourceType GetResourcePathSegment(string resourcePathSegment)
    {
      if (string.IsNullOrEmpty(resourcePathSegment))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.StringArgumentNullOrEmpty, (object) nameof (resourcePathSegment)));
      switch (resourcePathSegment.ToLowerInvariant())
      {
        case "addresses":
          return ResourceType.Address;
        case "attachments":
          return ResourceType.Attachment;
        case "clientencryptionkeys":
          return ResourceType.ClientEncryptionKey;
        case "colls":
          return ResourceType.Collection;
        case "conflicts":
          return ResourceType.Conflict;
        case "dbs":
          return ResourceType.Database;
        case "docs":
          return ResourceType.Document;
        case "media":
          return ResourceType.Media;
        case "offers":
          return ResourceType.Offer;
        case "permissions":
          return ResourceType.Permission;
        case "pkranges":
          return ResourceType.PartitionKeyRange;
        case "schemas":
          return ResourceType.Schema;
        case "snapshots":
          return ResourceType.Snapshot;
        case "sprocs":
          return ResourceType.StoredProcedure;
        case "triggers":
          return ResourceType.Trigger;
        case "udfs":
          return ResourceType.UserDefinedFunction;
        case "udts":
          return ResourceType.UserDefinedType;
        case "users":
          return ResourceType.User;
        default:
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, (object) resourcePathSegment));
      }
    }

    public static string GetResourcePath(ResourceType resourceType)
    {
      switch (resourceType)
      {
        case ResourceType.ControllerService:
        case ResourceType.Address:
        case ResourceType.ServiceFabricService:
        case ResourceType.Replica:
        case ResourceType.Module:
        case ResourceType.ModuleCommand:
        case ResourceType.Record:
        case ResourceType.BatchApply:
        case ResourceType.PartitionSetInformation:
        case ResourceType.XPReplicatorAddress:
        case ResourceType.DatabaseAccount:
        case ResourceType.Topology:
        case ResourceType.RestoreMetadata:
          return "/";
        case ResourceType.Media:
          return "//media/";
        case ResourceType.Database:
          return "dbs";
        case ResourceType.Collection:
        case ResourceType.PartitionKey:
          return "colls";
        case ResourceType.Document:
          return "docs";
        case ResourceType.Attachment:
          return "attachments";
        case ResourceType.User:
          return "users";
        case ResourceType.Permission:
          return "permissions";
        case ResourceType.Conflict:
          return "conflicts";
        case ResourceType.StoredProcedure:
          return "sprocs";
        case ResourceType.Trigger:
          return "triggers";
        case ResourceType.UserDefinedFunction:
          return "udfs";
        case ResourceType.Offer:
          return "offers";
        case ResourceType.MasterPartition:
        case ResourceType.ServerPartition:
          return "partitions";
        case ResourceType.Schema:
          return "schemas";
        case ResourceType.PartitionKeyRange:
          return "pkranges";
        case ResourceType.VectorClock:
          return "vectorclock";
        case ResourceType.RidRange:
          return "ridranges";
        case ResourceType.UserDefinedType:
          return "udts";
        case ResourceType.Snapshot:
          return "snapshots";
        case ResourceType.ClientEncryptionKey:
          return "clientencryptionkeys";
        default:
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, (object) resourceType.ToString()));
      }
    }

    public static string GeneratePath(
      ResourceType resourceType,
      DocumentServiceRequest request,
      bool isFeed,
      bool notRequireValidation = false)
    {
      return request.IsNameBased ? PathsHelper.GeneratePathForNameBased(resourceType, request.ResourceAddress, isFeed, notRequireValidation) : PathsHelper.GeneratePath(resourceType, request.ResourceId, isFeed);
    }

    public static string GenerateUserDefinedTypePath(string databaseName, string typeName) => "dbs/" + databaseName + "/udts/" + typeName;

    public static string GetCollectionPath(string resourceFullName)
    {
      if (resourceFullName != null)
      {
        int length = resourceFullName.Length <= 0 || resourceFullName[0] != '/' ? resourceFullName.IndexOfNth('/', 4) : resourceFullName.IndexOfNth('/', 5);
        if (length > 0)
          return resourceFullName.Substring(0, length);
      }
      return resourceFullName;
    }

    public static string GetDatabasePath(string resourceFullName)
    {
      if (resourceFullName != null)
      {
        int length = resourceFullName.Length <= 0 || resourceFullName[0] != '/' ? resourceFullName.IndexOfNth('/', 2) : resourceFullName.IndexOfNth('/', 3);
        if (length > 0)
          return resourceFullName.Substring(0, length);
      }
      return resourceFullName;
    }

    public static string GetParentByIndex(string resourceFullName, int segmentIndex)
    {
      int length = resourceFullName.IndexOfNth('/', segmentIndex);
      if (length > 0)
        return resourceFullName.Substring(0, length);
      return resourceFullName.IndexOfNth('/', segmentIndex - 1) > 0 ? resourceFullName : (string) null;
    }

    public static string GeneratePathForNameBased(
      Type resourceType,
      string resourceOwnerFullName,
      string resourceName)
    {
      if (resourceName == null)
        return (string) null;
      if ((object) resourceType == (object) typeof (Database))
        return "dbs/" + resourceName;
      if ((object) resourceType == (object) typeof (Snapshot))
        return "snapshots/" + resourceName;
      if (resourceOwnerFullName == null)
        return (string) null;
      if ((object) resourceType == (object) typeof (DocumentCollection))
        return resourceOwnerFullName + "/colls/" + resourceName;
      if ((object) resourceType == (object) typeof (ClientEncryptionKey))
        return resourceOwnerFullName + "/clientencryptionkeys/" + resourceName;
      if ((object) resourceType == (object) typeof (StoredProcedure))
        return resourceOwnerFullName + "/sprocs/" + resourceName;
      if ((object) resourceType == (object) typeof (UserDefinedFunction))
        return resourceOwnerFullName + "/udfs/" + resourceName;
      if ((object) resourceType == (object) typeof (Trigger))
        return resourceOwnerFullName + "/triggers/" + resourceName;
      if ((object) resourceType == (object) typeof (Conflict))
        return resourceOwnerFullName + "/conflicts/" + resourceName;
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Attachment), resourceType))
        return resourceOwnerFullName + "/attachments/" + resourceName;
      if ((object) resourceType == (object) typeof (User))
        return resourceOwnerFullName + "/users/" + resourceName;
      if ((object) resourceType == (object) typeof (UserDefinedType))
        return resourceOwnerFullName + "/udts/" + resourceName;
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Permission), resourceType))
        return resourceOwnerFullName + "/permissions/" + resourceName;
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Document), resourceType))
        return resourceOwnerFullName + "/docs/" + resourceName;
      if ((object) resourceType == (object) typeof (Offer))
        return "offers/" + resourceName;
      if ((object) resourceType == (object) typeof (Schema))
        return resourceOwnerFullName + "/schemas/" + resourceName;
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Resource), resourceType))
        return (string) null;
      throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, (object) resourceType.ToString()));
    }

    internal static void SetClientSidevalidation(bool validation) => PathsHelper.isClientSideValidationEnabled = validation;

    private static string GeneratePathForNameBased(
      ResourceType resourceType,
      string resourceFullName,
      bool isFeed,
      bool notRequireValidation = false)
    {
      if (isFeed && string.IsNullOrEmpty(resourceFullName) && resourceType != ResourceType.Database && resourceType != ResourceType.Snapshot)
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, (object) resourceType));
      ResourceType resourceType1;
      string pathForNameBased;
      if (!isFeed)
      {
        resourceType1 = resourceType;
        pathForNameBased = resourceFullName;
      }
      else
      {
        switch (resourceType)
        {
          case ResourceType.Database:
            return "dbs";
          case ResourceType.Collection:
            resourceType1 = ResourceType.Database;
            pathForNameBased = resourceFullName + "/colls";
            break;
          case ResourceType.Document:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/docs";
            break;
          case ResourceType.Attachment:
            resourceType1 = ResourceType.Document;
            pathForNameBased = resourceFullName + "/attachments";
            break;
          case ResourceType.User:
            resourceType1 = ResourceType.Database;
            pathForNameBased = resourceFullName + "/users";
            break;
          case ResourceType.Permission:
            resourceType1 = ResourceType.User;
            pathForNameBased = resourceFullName + "/permissions";
            break;
          case ResourceType.Conflict:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/conflicts";
            break;
          case ResourceType.StoredProcedure:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/sprocs";
            break;
          case ResourceType.Trigger:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/triggers";
            break;
          case ResourceType.UserDefinedFunction:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/udfs";
            break;
          case ResourceType.Offer:
            return resourceFullName + "/offers";
          case ResourceType.Schema:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/schemas";
            break;
          case ResourceType.PartitionKeyRange:
            return resourceFullName + "/pkranges";
          case ResourceType.UserDefinedType:
            resourceType1 = ResourceType.Database;
            pathForNameBased = resourceFullName + "/udts";
            break;
          case ResourceType.Snapshot:
            return "snapshots";
          case ResourceType.ClientEncryptionKey:
            resourceType1 = ResourceType.Database;
            pathForNameBased = resourceFullName + "/clientencryptionkeys";
            break;
          default:
            throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, (object) resourceType.ToString()));
        }
      }
      if (!notRequireValidation && PathsHelper.isClientSideValidationEnabled && !PathsHelper.ValidateResourceFullName(resourceType1, resourceFullName))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, (object) resourceType));
      return pathForNameBased;
    }

    public static string GeneratePath(
      ResourceType resourceType,
      string ownerOrResourceId,
      bool isFeed)
    {
      if (isFeed && string.IsNullOrEmpty(ownerOrResourceId) && resourceType != ResourceType.Database && resourceType != ResourceType.Offer && resourceType != ResourceType.DatabaseAccount && resourceType != ResourceType.Snapshot && resourceType != ResourceType.MasterPartition && resourceType != ResourceType.ServerPartition && resourceType != ResourceType.Topology && resourceType != ResourceType.RidRange && resourceType != ResourceType.VectorClock)
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, (object) resourceType));
      if (isFeed && resourceType == ResourceType.Database)
        return "dbs";
      if (resourceType == ResourceType.Database)
        return "dbs/" + ownerOrResourceId.ToString();
      if (isFeed && resourceType == ResourceType.Collection)
        return "dbs/" + ResourceId.Parse(ownerOrResourceId).DatabaseId.ToString() + "/colls";
      if (resourceType == ResourceType.Collection)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString();
      }
      if (isFeed && resourceType == ResourceType.Offer)
        return "offers";
      if (resourceType == ResourceType.Offer)
        return "offers/" + ownerOrResourceId.ToString();
      if (isFeed && resourceType == ResourceType.StoredProcedure)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/sprocs";
      }
      if (resourceType == ResourceType.StoredProcedure)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/sprocs/" + resourceId.StoredProcedureId.ToString();
      }
      if (isFeed && resourceType == ResourceType.UserDefinedFunction)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/udfs";
      }
      if (resourceType == ResourceType.UserDefinedFunction)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/udfs/" + resourceId.UserDefinedFunctionId.ToString();
      }
      if (isFeed && resourceType == ResourceType.Trigger)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/triggers";
      }
      if (resourceType == ResourceType.Trigger)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/triggers/" + resourceId.TriggerId.ToString();
      }
      if (isFeed && resourceType == ResourceType.Conflict)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/conflicts";
      }
      if (resourceType == ResourceType.Conflict)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/conflicts/" + resourceId.ConflictId.ToString();
      }
      if (isFeed && resourceType == ResourceType.PartitionKeyRange)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/pkranges";
      }
      if (resourceType == ResourceType.PartitionKeyRange)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/pkranges/" + resourceId.PartitionKeyRangeId.ToString();
      }
      if (isFeed && resourceType == ResourceType.Attachment)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/docs/" + resourceId.DocumentId.ToString() + "/attachments";
      }
      if (resourceType == ResourceType.Attachment)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/docs/" + resourceId.DocumentId.ToString() + "/attachments/" + resourceId.AttachmentId.ToString();
      }
      if (isFeed && resourceType == ResourceType.User)
        return "dbs/" + ownerOrResourceId + "/users";
      if (resourceType == ResourceType.User)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/users/" + resourceId.UserId.ToString();
      }
      if (isFeed && resourceType == ResourceType.ClientEncryptionKey)
        return "dbs/" + ownerOrResourceId + "/clientencryptionkeys";
      if (resourceType == ResourceType.ClientEncryptionKey)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/clientencryptionkeys/" + resourceId.ClientEncryptionKeyId.ToString();
      }
      if (isFeed && resourceType == ResourceType.UserDefinedType)
        return "dbs/" + ownerOrResourceId + "/udts";
      if (resourceType == ResourceType.UserDefinedType)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/udts/" + resourceId.UserDefinedTypeId.ToString();
      }
      if (isFeed && resourceType == ResourceType.Permission)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/users/" + resourceId.UserId.ToString() + "/permissions";
      }
      if (resourceType == ResourceType.Permission)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/users/" + resourceId.UserId.ToString() + "/permissions/" + resourceId.PermissionId.ToString();
      }
      if (isFeed && resourceType == ResourceType.Document)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/docs";
      }
      if (resourceType == ResourceType.Document)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/docs/" + resourceId.DocumentId.ToString();
      }
      if (isFeed && resourceType == ResourceType.Schema)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/schemas";
      }
      if (resourceType == ResourceType.Schema)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/schemas/" + resourceId.SchemaId.ToString();
      }
      if (isFeed && resourceType == ResourceType.DatabaseAccount)
        return "databaseaccount";
      if (resourceType == ResourceType.DatabaseAccount)
        return "databaseaccount/" + ownerOrResourceId;
      if (isFeed && resourceType == ResourceType.Snapshot)
        return "snapshots";
      if (resourceType == ResourceType.Snapshot)
        return "snapshots/" + ownerOrResourceId.ToString();
      if (isFeed && resourceType == ResourceType.MasterPartition)
        return "partitions";
      if (resourceType == ResourceType.MasterPartition)
        return "partitions/" + ownerOrResourceId;
      if (isFeed && resourceType == ResourceType.ServerPartition)
        return "partitions";
      if (resourceType == ResourceType.ServerPartition)
        return "partitions/" + ownerOrResourceId;
      if (isFeed && resourceType == ResourceType.Topology)
        return "topology";
      if (resourceType == ResourceType.Topology)
        return "topology/" + ownerOrResourceId;
      if (resourceType == ResourceType.RidRange)
        return "ridranges/" + ownerOrResourceId;
      if (resourceType == ResourceType.VectorClock)
        return "vectorclock/" + ownerOrResourceId;
      throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, (object) resourceType.ToString()));
    }

    public static string GenerateRootOperationPath(OperationType operationType)
    {
      switch (operationType)
      {
        case OperationType.ReadReplicaFromMasterPartition:
          return "operations/readreplicafrommasterpartition";
        case OperationType.GetUnwrappedDek:
          return "operations/getunwrappeddek";
        case OperationType.GetDatabaseAccountConfigurations:
          return "operations/getdatabaseaccountconfigurations";
        case OperationType.GetFederationConfigurations:
          return "operations/getfederationconfigurations";
        case OperationType.GetStorageAccountKey:
          return "operations/getstorageaccountkey";
        case OperationType.GetConfiguration:
          return "operations/getconfiguration";
        case OperationType.ControllerBatchGetOutput:
          return "operations/controllerbatchgetoutput";
        case OperationType.ControllerBatchReportCharges:
          return "operations/controllerbatchreportcharges";
        case OperationType.ReportThroughputUtilization:
          return "operations/reportthroughpututilization";
        case OperationType.ForceConfigRefresh:
          return "operations/forceConfigRefresh";
        case OperationType.Pause:
          return "operations/pause";
        case OperationType.Resume:
          return "operations/resume";
        case OperationType.Stop:
          return "operations/stop";
        case OperationType.Recycle:
          return "operations/recycle";
        case OperationType.Crash:
          return "operations/crash";
        case OperationType.BatchReportThroughputUtilization:
          return "operations/batchreportthroughpututilization";
        default:
          throw new NotFoundException();
      }
    }

    private static bool IsResourceType(string resourcePathSegment)
    {
      if (string.IsNullOrEmpty(resourcePathSegment))
        return false;
      switch (resourcePathSegment.ToLowerInvariant())
      {
        case "addresses":
        case "attachments":
        case "clientencryptionkeys":
        case "colls":
        case "conflicts":
        case "databaseaccount":
        case "dbs":
        case "docs":
        case "media":
        case "offers":
        case "partitions":
        case "permissions":
        case "pkranges":
        case "postsplitaction":
        case "presplitaction":
        case "ridranges":
        case "schemas":
        case "snapshots":
        case "sprocs":
        case "topology":
        case "triggers":
        case "udfs":
        case "udts":
        case "users":
        case "vectorclock":
          return true;
        default:
          return false;
      }
    }

    private static bool IsRootOperation(string operationSegment, string operationTypeSegment)
    {
      if (string.IsNullOrEmpty(operationSegment) || string.IsNullOrEmpty(operationTypeSegment) || string.Compare(operationSegment, "operations", StringComparison.OrdinalIgnoreCase) != 0)
        return false;
      switch (operationTypeSegment.ToLowerInvariant())
      {
        case "batchreportthroughpututilization":
        case "controllerbatchgetoutput":
        case "controllerbatchreportcharges":
        case "crash":
        case "getconfiguration":
        case "getdatabaseaccountconfigurations":
        case "getfederationconfigurations":
        case "getstorageaccountkey":
        case "getunwrappeddek":
        case "pause":
        case "readreplicafrommasterpartition":
        case "recycle":
        case "reportthroughpututilization":
        case "resume":
        case "stop":
          return true;
        default:
          return false;
      }
    }

    private static bool IsTopLevelOperationOperation(string replicaSegment, string addressSegment) => string.IsNullOrEmpty(replicaSegment) && (string.Compare(addressSegment, "xpreplicatoraddreses", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(addressSegment, "computegatewaycharge", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(addressSegment, "serviceReservation", StringComparison.OrdinalIgnoreCase) == 0);

    internal static bool IsNameBased(string resourceIdOrFullName) => !string.IsNullOrEmpty(resourceIdOrFullName) && resourceIdOrFullName.Length > 4 && resourceIdOrFullName[3] == '/';

    internal static int IndexOfNth(this string str, char value, int n)
    {
      if (string.IsNullOrEmpty(str) || n <= 0 || n > str.Length)
        return -1;
      int num = n;
      for (int index = 0; index < str.Length; ++index)
      {
        if ((int) str[index] == (int) value && --num == 0)
          return index;
      }
      return -1;
    }

    internal static bool ValidateResourceFullName(
      ResourceType resourceType,
      string resourceFullName)
    {
      string[] strArray = resourceFullName.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      string[] resourcePathArray = PathsHelper.GetResourcePathArray(resourceType);
      if (resourcePathArray == null || strArray.Length != resourcePathArray.Length * 2)
        return false;
      for (int index = 0; index < resourcePathArray.Length; ++index)
      {
        if (string.Compare(resourcePathArray[index], strArray[2 * index], StringComparison.Ordinal) != 0)
          return false;
      }
      return true;
    }

    private static string[] GetResourcePathArray(ResourceType resourceType)
    {
      List<string> stringList = new List<string>();
      if (resourceType == ResourceType.Snapshot)
      {
        stringList.Add("snapshots");
        return stringList.ToArray();
      }
      stringList.Add("dbs");
      if (resourceType == ResourceType.Permission || resourceType == ResourceType.User)
      {
        stringList.Add("users");
        if (resourceType == ResourceType.Permission)
          stringList.Add("permissions");
      }
      else
      {
        switch (resourceType)
        {
          case ResourceType.UserDefinedType:
            stringList.Add("udts");
            break;
          case ResourceType.ClientEncryptionKey:
            stringList.Add("clientencryptionkeys");
            break;
          default:
            if (resourceType == ResourceType.Collection || resourceType == ResourceType.StoredProcedure || resourceType == ResourceType.UserDefinedFunction || resourceType == ResourceType.Trigger || resourceType == ResourceType.Conflict || resourceType == ResourceType.Attachment || resourceType == ResourceType.Document || resourceType == ResourceType.PartitionKeyRange || resourceType == ResourceType.Schema)
            {
              stringList.Add("colls");
              switch (resourceType)
              {
                case ResourceType.Conflict:
                  stringList.Add("conflicts");
                  break;
                case ResourceType.StoredProcedure:
                  stringList.Add("sprocs");
                  break;
                case ResourceType.Trigger:
                  stringList.Add("triggers");
                  break;
                case ResourceType.UserDefinedFunction:
                  stringList.Add("udfs");
                  break;
                case ResourceType.Schema:
                  stringList.Add("schemas");
                  break;
                default:
                  if (resourceType == ResourceType.Document || resourceType == ResourceType.Attachment)
                  {
                    stringList.Add("docs");
                    if (resourceType == ResourceType.Attachment)
                    {
                      stringList.Add("attachments");
                      break;
                    }
                    break;
                  }
                  if (resourceType == ResourceType.PartitionKeyRange)
                  {
                    stringList.Add("pkranges");
                    break;
                  }
                  break;
              }
            }
            else
            {
              if (resourceType != ResourceType.Database)
                return (string[]) null;
              break;
            }
            break;
        }
      }
      return stringList.ToArray();
    }

    internal static bool ValidateResourceId(ResourceType resourceType, string resourceId)
    {
      switch (resourceType)
      {
        case ResourceType.Database:
          return PathsHelper.ValidateDatabaseId(resourceId);
        case ResourceType.Collection:
          return PathsHelper.ValidateDocumentCollectionId(resourceId);
        case ResourceType.Document:
          return PathsHelper.ValidateDocumentId(resourceId);
        case ResourceType.Attachment:
          return PathsHelper.ValidateAttachmentId(resourceId);
        case ResourceType.User:
          return PathsHelper.ValidateUserId(resourceId);
        case ResourceType.Permission:
          return PathsHelper.ValidatePermissionId(resourceId);
        case ResourceType.Conflict:
          return PathsHelper.ValidateConflictId(resourceId);
        case ResourceType.StoredProcedure:
          return PathsHelper.ValidateStoredProcedureId(resourceId);
        case ResourceType.Trigger:
          return PathsHelper.ValidateTriggerId(resourceId);
        case ResourceType.UserDefinedFunction:
          return PathsHelper.ValidateUserDefinedFunctionId(resourceId);
        case ResourceType.Schema:
          return PathsHelper.ValidateSchemaId(resourceId);
        case ResourceType.UserDefinedType:
          return PathsHelper.ValidateUserDefinedTypeId(resourceId);
        case ResourceType.Snapshot:
          return PathsHelper.ValidateSnapshotId(resourceId);
        case ResourceType.ClientEncryptionKey:
          return PathsHelper.ValidateClientEncryptionKeyId(resourceId);
        default:
          return false;
      }
    }

    internal static bool ValidateDatabaseId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Database > 0U;
    }

    internal static bool ValidateDocumentCollectionId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.DocumentCollection > 0U;
    }

    internal static bool ValidateDocumentId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Document > 0UL;
    }

    internal static bool ValidateConflictId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Conflict > 0UL;
    }

    internal static bool ValidateAttachmentId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Attachment > 0U;
    }

    internal static bool ValidatePermissionId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Permission > 0UL;
    }

    internal static bool ValidateStoredProcedureId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.StoredProcedure > 0UL;
    }

    internal static bool ValidateTriggerId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Trigger > 0UL;
    }

    internal static bool ValidateUserDefinedFunctionId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.UserDefinedFunction > 0UL;
    }

    internal static bool ValidateUserId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.User > 0U;
    }

    internal static bool ValidateClientEncryptionKeyId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.ClientEncryptionKey > 0U;
    }

    internal static bool ValidateUserDefinedTypeId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.UserDefinedType > 0U;
    }

    internal static bool ValidateSchemaId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Schema > 0UL;
    }

    internal static bool ValidateSnapshotId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Snapshot > 0UL;
    }

    internal static bool IsPublicResource(Type resourceType) => (object) resourceType == (object) typeof (Database) || (object) resourceType == (object) typeof (ClientEncryptionKey) || (object) resourceType == (object) typeof (DocumentCollection) || (object) resourceType == (object) typeof (StoredProcedure) || (object) resourceType == (object) typeof (UserDefinedFunction) || (object) resourceType == (object) typeof (Trigger) || (object) resourceType == (object) typeof (Conflict) || CustomTypeExtensions.IsAssignableFrom(typeof (Attachment), resourceType) || (object) resourceType == (object) typeof (User) || CustomTypeExtensions.IsAssignableFrom(typeof (Permission), resourceType) || CustomTypeExtensions.IsAssignableFrom(typeof (Document), resourceType) || (object) resourceType == (object) typeof (Offer) || (object) resourceType == (object) typeof (Schema) || (object) resourceType == (object) typeof (Snapshot);

    internal static void ParseCollectionSelfLink(
      string collectionSelfLink,
      out string databaseId,
      out string collectionId)
    {
      string[] strArray = collectionSelfLink.Split(RuntimeConstants.Separators.Url, StringSplitOptions.RemoveEmptyEntries);
      databaseId = strArray.Length == 4 && string.Equals(strArray[0], "dbs", StringComparison.OrdinalIgnoreCase) && string.Equals(strArray[2], "colls", StringComparison.OrdinalIgnoreCase) ? strArray[1] : throw new ArgumentException(RMResources.BadUrl, nameof (collectionSelfLink));
      collectionId = strArray[3];
    }
  }
}
