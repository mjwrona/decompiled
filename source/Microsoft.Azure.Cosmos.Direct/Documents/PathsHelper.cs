// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PathsHelper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class PathsHelper
  {
    private const char ForwardSlash = '/';
    private static readonly StringSegment[] EmptyArray = new StringSegment[0];
    private static readonly char[] PathSeparatorArray = new char[1]
    {
      '/'
    };
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
      if (!string.IsNullOrEmpty(resourceUrl) && resourceUrl.Contains("operations") && (resourceUrl.Contains("partitionkeydelete") || resourceUrl.Contains("collectiontruncate")))
      {
        isFeed = false;
        string collectionPath = PathsHelper.GetCollectionPath(resourceUrl);
        if (collectionPath == null || collectionPath.Length < 1)
        {
          resourcePath = string.Empty;
          resourceIdOrFullName = string.Empty;
          isNameBased = false;
          return false;
        }
        resourceIdOrFullName = collectionPath[0] == '/' ? collectionPath.Substring(1) : collectionPath;
        resourcePath = "colls";
        isNameBased = true;
        return true;
      }
      if (string.IsNullOrEmpty(resourceUrl) || !resourceUrl.Contains("operations") || !resourceUrl.Contains("metadatacheckaccess"))
        return PathsHelper.TryParsePathSegmentsWithDatabaseAndCollectionNames(resourceUrl, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased, out databaseName, out collectionName, clientVersion);
      isFeed = false;
      resourceIdOrFullName = string.Empty;
      resourcePath = "/";
      isNameBased = true;
      return true;
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
      return PathsHelper.TryParsePathSegmentsWithDatabaseAndCollectionAndDocumentNames(resourceUrl, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased, out databaseName, out collectionName, out string _, clientVersion, parseDatabaseAndCollectionNames);
    }

    public static bool TryParsePathSegmentsWithDatabaseAndCollectionAndDocumentNames(
      string resourceUrl,
      out bool isFeed,
      out string resourcePath,
      out string resourceIdOrFullName,
      out bool isNameBased,
      out string databaseName,
      out string collectionName,
      out string documentName,
      string clientVersion = "",
      bool parseDatabaseAndCollectionNames = false)
    {
      resourcePath = string.Empty;
      resourceIdOrFullName = string.Empty;
      isFeed = false;
      isNameBased = false;
      databaseName = string.Empty;
      collectionName = string.Empty;
      documentName = string.Empty;
      if (string.IsNullOrEmpty(resourceUrl))
        return false;
      string[] segments = resourceUrl.Split(PathsHelper.PathSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
      if (segments == null || segments.Length < 1)
        return false;
      int length = segments.Length;
      StringSegment stringSegment1 = new StringSegment(segments[length - 1]);
      StringSegment stringSegment2 = stringSegment1.Trim(PathsHelper.PathSeparatorArray);
      StringSegment stringSegment3 = new StringSegment(string.Empty);
      if (length >= 2)
      {
        stringSegment1 = new StringSegment(segments[length - 2]);
        stringSegment3 = stringSegment1.Trim(PathsHelper.PathSeparatorArray);
      }
      if (PathsHelper.IsRootOperation(in stringSegment3, in stringSegment2) || PathsHelper.IsTopLevelOperationOperation(in stringSegment3, in stringSegment2))
      {
        isFeed = false;
        resourceIdOrFullName = string.Empty;
        resourcePath = "/";
        return true;
      }
      if (length >= 2)
      {
        if (segments[segments.Length - 1].Equals("retriablewritecachedresponse", StringComparison.OrdinalIgnoreCase))
        {
          isNameBased = true;
          isFeed = false;
          resourcePath = segments[segments.Length - 1];
          StringSegment path = (StringSegment) resourceUrl;
          ref string local = ref resourceIdOrFullName;
          stringSegment1 = UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(path));
          string str = Uri.UnescapeDataString(stringSegment1.GetString());
          local = str;
          PathsHelper.ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(segments, out databaseName, out collectionName, out string _);
          return true;
        }
        string str1 = segments[0];
        if (str1.Equals("dbs", StringComparison.OrdinalIgnoreCase))
        {
          ResourceId rid;
          if (!ResourceId.TryParse(segments[1], out rid) || !rid.IsDatabaseId)
            isNameBased = true;
        }
        else if (str1.Equals("encryptionscopes", StringComparison.OrdinalIgnoreCase))
        {
          ResourceId rid;
          if (!ResourceId.TryParse(segments[1], out rid) || !rid.IsEncryptionScopeId)
            isNameBased = true;
        }
        else if (str1.Equals("snapshots", StringComparison.OrdinalIgnoreCase))
        {
          ResourceId rid;
          if (!ResourceId.TryParse(segments[1], out rid) || !rid.IsSnapshotId)
            isNameBased = true;
        }
        else if (str1.Equals("roledefinitions", StringComparison.OrdinalIgnoreCase))
        {
          ResourceId rid;
          if (!ResourceId.TryParse(segments[1], out rid) || !rid.IsRoleDefinitionId)
            isNameBased = true;
        }
        else if (str1.Equals("roleassignments", StringComparison.OrdinalIgnoreCase))
        {
          ResourceId rid;
          if (!ResourceId.TryParse(segments[1], out rid) || !rid.IsRoleAssignmentId)
            isNameBased = true;
        }
        else if (str1.Equals("interopusers", StringComparison.OrdinalIgnoreCase))
        {
          ResourceId rid;
          if (!ResourceId.TryParse(segments[1], out rid) || !rid.IsInteropUserId)
            isNameBased = true;
        }
        else
        {
          ResourceId rid;
          if (str1.Equals("authpolicyelements", StringComparison.OrdinalIgnoreCase) && (!ResourceId.TryParse(segments[1], out rid) || !rid.IsAuthPolicyElementId))
            isNameBased = true;
        }
        if (isNameBased)
          return PathsHelper.TryParseNameSegments(resourceUrl, segments, out isFeed, out resourcePath, out resourceIdOrFullName, out databaseName, out collectionName, out documentName, parseDatabaseAndCollectionNames);
      }
      if (length % 2 != 0 && PathsHelper.IsResourceType(in stringSegment2))
      {
        isFeed = true;
        resourcePath = stringSegment2.GetString();
        if (!stringSegment2.Equals("dbs", StringComparison.OrdinalIgnoreCase))
          resourceIdOrFullName = stringSegment3.GetString();
      }
      else
      {
        if (!PathsHelper.IsResourceType(in stringSegment3))
          return false;
        isFeed = false;
        resourcePath = stringSegment3.GetString();
        resourceIdOrFullName = stringSegment2.GetString();
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
      PathsHelper.ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(segments, out databaseName, out collectionName, out string _);
    }

    public static void ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(
      string[] segments,
      out string databaseName,
      out string collectionName,
      out string documentName)
    {
      databaseName = string.Empty;
      collectionName = string.Empty;
      documentName = string.Empty;
      if (segments == null || segments.Length < 2 || !string.Equals(segments[0], "dbs", StringComparison.OrdinalIgnoreCase))
        return;
      ref string local1 = ref databaseName;
      StringSegment stringSegment = UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(segments[1])));
      string str1 = Uri.UnescapeDataString(stringSegment.GetString());
      local1 = str1;
      if (segments.Length >= 4 && string.Equals(segments[2], "colls", StringComparison.OrdinalIgnoreCase))
      {
        ref string local2 = ref collectionName;
        stringSegment = UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(segments[3])));
        string str2 = Uri.UnescapeDataString(stringSegment.GetString());
        local2 = str2;
      }
      if (segments.Length < 6 || !string.Equals(segments[4], "docs", StringComparison.OrdinalIgnoreCase))
        return;
      ref string local3 = ref documentName;
      stringSegment = UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(segments[5])));
      string str3 = Uri.UnescapeDataString(stringSegment.GetString());
      local3 = str3;
    }

    public static bool TryParsePathSegmentsWithDatabaseAndCollectionAndOperationNames(
      string resourceUrl,
      out string resourcePath,
      out string resourceIdOrFullName,
      out bool isNameBased,
      out string databaseName,
      out string collectionName,
      out ResourceType resourceType,
      out OperationType operationType)
    {
      resourcePath = string.Empty;
      resourceIdOrFullName = string.Empty;
      isNameBased = false;
      databaseName = string.Empty;
      collectionName = string.Empty;
      resourceType = ResourceType.Unknown;
      operationType = OperationType.Invalid;
      if (string.IsNullOrEmpty(resourceUrl))
        return false;
      string[] strArray = resourceUrl.Split(PathsHelper.PathSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
      if (strArray == null || strArray.Length != 6 || !strArray[0].Equals("dbs", StringComparison.OrdinalIgnoreCase) || !strArray[2].Equals("colls", StringComparison.OrdinalIgnoreCase) || !strArray[4].Equals("operations", StringComparison.OrdinalIgnoreCase))
        return false;
      switch (strArray[5])
      {
        case "partitionkeydelete":
          resourceType = ResourceType.PartitionKey;
          operationType = OperationType.Delete;
          break;
        case "collectiontruncate":
          resourceType = ResourceType.Collection;
          operationType = OperationType.CollectionTruncate;
          break;
        default:
          return false;
      }
      resourcePath = "colls";
      databaseName = Uri.UnescapeDataString(strArray[1]);
      collectionName = Uri.UnescapeDataString(strArray[3]);
      ResourceId rid;
      if (!ResourceId.TryParse(strArray[3], out rid) || !rid.IsDocumentCollectionId)
      {
        resourceIdOrFullName = string.Format("{0}{1}{2}{3}{4}{5}{6}", (object) strArray[0], (object) '/', (object) strArray[1], (object) '/', (object) strArray[2], (object) '/', (object) strArray[3]);
        isNameBased = true;
      }
      else
        resourceIdOrFullName = strArray[3];
      return true;
    }

    private static bool TryParseNameSegments(
      string resourceUrl,
      string[] segments,
      out bool isFeed,
      out string resourcePath,
      out string resourceFullName,
      out string databaseName,
      out string collectionName,
      out string documentName,
      bool parseDatabaseAndCollectionNames)
    {
      isFeed = false;
      resourcePath = string.Empty;
      resourceFullName = string.Empty;
      databaseName = string.Empty;
      collectionName = string.Empty;
      documentName = string.Empty;
      if (segments == null || segments.Length < 1)
        return false;
      if (segments.Length % 2 == 0)
      {
        if (PathsHelper.IsResourceType((StringSegment) segments[segments.Length - 2]))
        {
          resourcePath = segments[segments.Length - 2];
          resourceFullName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(new StringSegment(resourceUrl))).GetString());
          if (parseDatabaseAndCollectionNames)
            PathsHelper.ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(segments, out databaseName, out collectionName, out documentName);
          return true;
        }
      }
      else if (PathsHelper.IsResourceType((StringSegment) segments[segments.Length - 1]))
      {
        isFeed = true;
        resourcePath = segments[segments.Length - 1];
        StringSegment path1 = (StringSegment) resourceUrl;
        StringSegment path2 = path1.Substring(0, UrlUtility.RemoveTrailingSlashes(path1).LastIndexOf("/"[0]));
        resourceFullName = Uri.UnescapeDataString(UrlUtility.RemoveTrailingSlashes(UrlUtility.RemoveLeadingSlashes(path2)).GetString());
        if (parseDatabaseAndCollectionNames)
          PathsHelper.ParseDatabaseNameAndCollectionAndDocumentNameFromUrlSegments(segments, out databaseName, out collectionName, out string _);
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
        case "authpolicyelements":
          return ResourceType.AuthPolicyElement;
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
        case "encryptionscopes":
          return ResourceType.EncryptionScope;
        case "interopusers":
          return ResourceType.InteropUser;
        case "media":
          return ResourceType.Media;
        case "offers":
          return ResourceType.Offer;
        case "partitionedsystemdocuments":
          return ResourceType.PartitionedSystemDocument;
        case "permissions":
          return ResourceType.Permission;
        case "pkranges":
          return ResourceType.PartitionKeyRange;
        case "roleassignments":
          return ResourceType.RoleAssignment;
        case "roledefinitions":
          return ResourceType.RoleDefinition;
        case "schemas":
          return ResourceType.Schema;
        case "snapshots":
          return ResourceType.Snapshot;
        case "sprocs":
          return ResourceType.StoredProcedure;
        case "systemdocuments":
          return ResourceType.SystemDocument;
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
        case ResourceType.Record:
        case ResourceType.BatchApply:
        case ResourceType.DatabaseAccount:
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
        case ResourceType.Schema:
          return "schemas";
        case ResourceType.PartitionKeyRange:
          return "pkranges";
        case ResourceType.UserDefinedType:
          return "udts";
        case ResourceType.Snapshot:
          return "snapshots";
        case ResourceType.PartitionedSystemDocument:
          return "partitionedsystemdocuments";
        case ResourceType.ClientEncryptionKey:
          return "clientencryptionkeys";
        case ResourceType.Transaction:
          return "transaction";
        case ResourceType.RoleDefinition:
          return "roledefinitions";
        case ResourceType.RoleAssignment:
          return "roleassignments";
        case ResourceType.SystemDocument:
          return "systemdocuments";
        case ResourceType.InteropUser:
          return "interopusers";
        case ResourceType.AuthPolicyElement:
          return "authpolicyelements";
        case ResourceType.RetriableWriteCachedResponse:
          return "retriablewritecachedresponse";
        case ResourceType.EncryptionScope:
          return "encryptionscopes";
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
      return request.IsNameBased ? PathsHelper.GeneratePathForNameBased(resourceType, request.ResourceAddress, isFeed, request.OperationType, notRequireValidation) : PathsHelper.GeneratePath(resourceType, request.ResourceId, isFeed, request.OperationType);
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
      if (resourceType == typeof (Database))
        return "dbs/" + resourceName;
      if (resourceType == typeof (Snapshot))
        return "snapshots/" + resourceName;
      if (resourceOwnerFullName == null)
        return (string) null;
      if (resourceType == typeof (DocumentCollection))
        return resourceOwnerFullName + "/colls/" + resourceName;
      if (resourceType == typeof (ClientEncryptionKey))
        return resourceOwnerFullName + "/clientencryptionkeys/" + resourceName;
      if (resourceType == typeof (StoredProcedure))
        return resourceOwnerFullName + "/sprocs/" + resourceName;
      if (resourceType == typeof (UserDefinedFunction))
        return resourceOwnerFullName + "/udfs/" + resourceName;
      if (resourceType == typeof (Trigger))
        return resourceOwnerFullName + "/triggers/" + resourceName;
      if (resourceType == typeof (Conflict))
        return resourceOwnerFullName + "/conflicts/" + resourceName;
      if (typeof (Attachment).IsAssignableFrom(resourceType))
        return resourceOwnerFullName + "/attachments/" + resourceName;
      if (resourceType == typeof (User))
        return resourceOwnerFullName + "/users/" + resourceName;
      if (resourceType == typeof (UserDefinedType))
        return resourceOwnerFullName + "/udts/" + resourceName;
      if (typeof (Permission).IsAssignableFrom(resourceType))
        return resourceOwnerFullName + "/permissions/" + resourceName;
      if (typeof (Document).IsAssignableFrom(resourceType))
        return resourceOwnerFullName + "/docs/" + resourceName;
      if (resourceType == typeof (Offer))
        return "offers/" + resourceName;
      if (resourceType == typeof (Schema))
        return resourceOwnerFullName + "/schemas/" + resourceName;
      if (resourceType == typeof (SystemDocument))
        return resourceOwnerFullName + "/systemdocuments/" + resourceName;
      if (resourceType == typeof (PartitionedSystemDocument))
        return resourceOwnerFullName + "/partitionedsystemdocuments/" + resourceName;
      if (typeof (Resource).IsAssignableFrom(resourceType))
        return (string) null;
      throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, (object) resourceType.ToString()));
    }

    public static string GeneratePathForNamedBasedInternalResources(
      ResourceType resourceType,
      string resourceName)
    {
      if (resourceName == null)
        return (string) null;
      string internalResources;
      switch (resourceType)
      {
        case ResourceType.RoleDefinition:
          internalResources = "roledefinitions/" + resourceName;
          break;
        case ResourceType.RoleAssignment:
          internalResources = "roleassignments/" + resourceName;
          break;
        case ResourceType.InteropUser:
          internalResources = "interopusers/" + resourceName;
          break;
        case ResourceType.AuthPolicyElement:
          internalResources = "authpolicyelements/" + resourceName;
          break;
        default:
          internalResources = (string) null;
          break;
      }
      return internalResources;
    }

    internal static void SetClientSidevalidation(bool validation) => PathsHelper.isClientSideValidationEnabled = validation;

    private static string GeneratePathForNameBased(
      ResourceType resourceType,
      string resourceFullName,
      bool isFeed,
      OperationType operationType,
      bool notRequireValidation = false)
    {
      if (isFeed && string.IsNullOrEmpty(resourceFullName) && resourceType != ResourceType.Database && resourceType != ResourceType.EncryptionScope && resourceType != ResourceType.Snapshot && resourceType != ResourceType.RoleDefinition && resourceType != ResourceType.RoleAssignment && resourceType != ResourceType.InteropUser && resourceType != ResourceType.AuthPolicyElement)
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, (object) resourceType));
      ResourceType resourceType1;
      string pathForNameBased;
      if (resourceType == ResourceType.PartitionKey && operationType == OperationType.Delete)
      {
        resourceType1 = resourceType;
        resourceFullName += "/operations/partitionkeydelete";
        pathForNameBased = resourceFullName;
      }
      else if (resourceType == ResourceType.Collection && operationType == OperationType.CollectionTruncate)
      {
        resourceType1 = ResourceType.Collection;
        pathForNameBased = resourceFullName + "/operations/collectiontruncate";
      }
      else if (!isFeed)
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
          case ResourceType.PartitionedSystemDocument:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/partitionedsystemdocuments";
            break;
          case ResourceType.ClientEncryptionKey:
            resourceType1 = ResourceType.Database;
            pathForNameBased = resourceFullName + "/clientencryptionkeys";
            break;
          case ResourceType.RoleDefinition:
            return "roledefinitions";
          case ResourceType.RoleAssignment:
            return "roleassignments";
          case ResourceType.SystemDocument:
            resourceType1 = ResourceType.Collection;
            pathForNameBased = resourceFullName + "/systemdocuments";
            break;
          case ResourceType.InteropUser:
            return "interopusers";
          case ResourceType.AuthPolicyElement:
            return "authpolicyelements";
          case ResourceType.EncryptionScope:
            return "encryptionscopes";
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
      bool isFeed,
      OperationType operationType = OperationType.Create)
    {
      if (isFeed && string.IsNullOrEmpty(ownerOrResourceId) && resourceType != ResourceType.Database && resourceType != ResourceType.EncryptionScope && resourceType != ResourceType.Offer && resourceType != ResourceType.DatabaseAccount && resourceType != ResourceType.Snapshot && resourceType != ResourceType.RoleAssignment && resourceType != ResourceType.RoleDefinition && resourceType != ResourceType.InteropUser && resourceType != ResourceType.AuthPolicyElement)
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnexpectedResourceType, (object) resourceType));
      if (isFeed && resourceType == ResourceType.EncryptionScope)
        return "encryptionscopes";
      if (resourceType == ResourceType.EncryptionScope)
        return "encryptionscopes/" + ownerOrResourceId.ToString();
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
      if (resourceType == ResourceType.PartitionKey && operationType == OperationType.Delete)
      {
        ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
        return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/operations/partitionkeydelete";
      }
      if (isFeed && resourceType == ResourceType.RoleAssignment)
        return "roleassignments";
      if (isFeed && resourceType == ResourceType.RoleDefinition)
        return "roledefinitions";
      if (isFeed && resourceType == ResourceType.AuthPolicyElement)
        return "authpolicyelements";
      switch (resourceType)
      {
        case ResourceType.RoleDefinition:
          return "roledefinitions/" + ownerOrResourceId.ToString();
        case ResourceType.RoleAssignment:
          return "roleassignments/" + ownerOrResourceId.ToString();
        case ResourceType.AuthPolicyElement:
          return "authpolicyelements/" + ownerOrResourceId.ToString();
        default:
          if (isFeed && resourceType == ResourceType.SystemDocument)
          {
            ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
            return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/systemdocuments";
          }
          if (resourceType == ResourceType.SystemDocument)
          {
            ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
            return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/systemdocuments/" + resourceId.SystemDocumentId.ToString();
          }
          if (isFeed && resourceType == ResourceType.PartitionedSystemDocument)
          {
            ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
            return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/partitionedsystemdocuments";
          }
          if (resourceType == ResourceType.PartitionedSystemDocument)
          {
            ResourceId resourceId = ResourceId.Parse(ownerOrResourceId);
            return "dbs/" + resourceId.DatabaseId.ToString() + "/colls/" + resourceId.DocumentCollectionId.ToString() + "/partitionedsystemdocuments/" + resourceId.PartitionedSystemDocumentId.ToString();
          }
          if (isFeed && resourceType == ResourceType.InteropUser)
            return "interopusers";
          if (resourceType == ResourceType.InteropUser)
            return "interopusers/" + ownerOrResourceId.ToString();
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnknownResourceType, (object) resourceType.ToString()));
      }
    }

    public static string GenerateRootOperationPath(OperationType operationType) => throw new NotFoundException();

    private static bool IsResourceType(in StringSegment resourcePathSegment)
    {
      if (resourcePathSegment.IsNullOrEmpty())
        return false;
      return resourcePathSegment.Equals("attachments", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("colls", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("dbs", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("permissions", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("users", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("clientencryptionkeys", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("storageauthtoken", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("udts", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("docs", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("sprocs", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("triggers", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("udfs", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("conflicts", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("media", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("offers", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("partitions", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("databaseaccount", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("topology", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("pkranges", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("presplitaction", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("postsplitaction", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("schemas", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("ridranges", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("vectorclock", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("addresses", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("snapshots", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("partitionedsystemdocuments", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("roledefinitions", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("roleassignments", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("transaction", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("systemdocuments", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("interopusers", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("authpolicyelements", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("systemdocuments", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("encryptionscopes", StringComparison.OrdinalIgnoreCase) || resourcePathSegment.Equals("retriablewritecachedresponse", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRootOperation(
      in StringSegment operationSegment,
      in StringSegment operationTypeSegment)
    {
      if (operationSegment.IsNullOrEmpty() || operationTypeSegment.IsNullOrEmpty() || operationSegment.Compare("operations", StringComparison.OrdinalIgnoreCase) != 0)
        return false;
      return operationTypeSegment.Equals("pause", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("resume", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("stop", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("recycle", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("crash", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("reportthroughpututilization", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("batchreportthroughpututilization", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("controllerbatchgetoutput", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("controllerbatchreportcharges", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getfederationconfigurations", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getstorageserviceconfigurations", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getconfiguration", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getstorageaccountkey", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getstorageaccountsas", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getdatabaseaccountconfigurations", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getunwrappeddek", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getcustomermanagedkeystatus", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("readreplicafrommasterpartition", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("readreplicafromserverpartition", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("masterinitiatedprogresscoordination", StringComparison.OrdinalIgnoreCase) || operationTypeSegment.Equals("getaadgroups", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTopLevelOperationOperation(
      in StringSegment replicaSegment,
      in StringSegment addressSegment)
    {
      return replicaSegment.IsNullOrEmpty() && (addressSegment.Compare("xpreplicatoraddreses", StringComparison.OrdinalIgnoreCase) == 0 || addressSegment.Compare("computegatewaycharge", StringComparison.OrdinalIgnoreCase) == 0 || addressSegment.Compare("serviceReservation", StringComparison.OrdinalIgnoreCase) == 0);
    }

    internal static bool IsNameBased(string resourceIdOrFullName) => !string.IsNullOrEmpty(resourceIdOrFullName) && (resourceIdOrFullName.Length > 4 && resourceIdOrFullName[3] == '/' || resourceIdOrFullName.StartsWith("interopusers", StringComparison.OrdinalIgnoreCase));

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

    internal static string[] GetResourcePathArray(ResourceType resourceType)
    {
      List<string> stringList = new List<string>();
      switch (resourceType)
      {
        case ResourceType.Address:
          stringList.Add("addresses");
          return stringList.ToArray();
        case ResourceType.Offer:
          stringList.Add("offers");
          return stringList.ToArray();
        case ResourceType.Snapshot:
          stringList.Add("snapshots");
          return stringList.ToArray();
        case ResourceType.RoleDefinition:
          stringList.Add("roledefinitions");
          return stringList.ToArray();
        case ResourceType.RoleAssignment:
          stringList.Add("roleassignments");
          return stringList.ToArray();
        case ResourceType.InteropUser:
          stringList.Add("interopusers");
          return stringList.ToArray();
        case ResourceType.AuthPolicyElement:
          stringList.Add("authpolicyelements");
          return stringList.ToArray();
        case ResourceType.EncryptionScope:
          stringList.Add("encryptionscopes");
          return stringList.ToArray();
        default:
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
                if (resourceType == ResourceType.Collection || resourceType == ResourceType.StoredProcedure || resourceType == ResourceType.UserDefinedFunction || resourceType == ResourceType.Trigger || resourceType == ResourceType.Conflict || resourceType == ResourceType.Attachment || resourceType == ResourceType.Document || resourceType == ResourceType.PartitionKeyRange || resourceType == ResourceType.Schema || resourceType == ResourceType.PartitionedSystemDocument || resourceType == ResourceType.SystemDocument)
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
                      switch (resourceType)
                      {
                        case ResourceType.PartitionKeyRange:
                          stringList.Add("pkranges");
                          break;
                        case ResourceType.PartitionedSystemDocument:
                          stringList.Add("partitionedsystemdocuments");
                          break;
                        case ResourceType.SystemDocument:
                          stringList.Add("systemdocuments");
                          break;
                      }
                      break;
                  }
                }
                else
                {
                  switch (resourceType)
                  {
                    case ResourceType.Database:
                      break;
                    case ResourceType.PartitionKey:
                      stringList.Add("colls");
                      stringList.Add("operations");
                      break;
                    default:
                      return (string[]) null;
                  }
                }
                break;
            }
          }
          return stringList.ToArray();
      }
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
        case ResourceType.PartitionedSystemDocument:
          return PathsHelper.ValidatePartitionedSystemDocumentId(resourceId);
        case ResourceType.ClientEncryptionKey:
          return PathsHelper.ValidateClientEncryptionKeyId(resourceId);
        case ResourceType.RoleDefinition:
          return PathsHelper.ValidateRoleDefinitionId(resourceId);
        case ResourceType.RoleAssignment:
          return PathsHelper.ValidateRoleAssignmentId(resourceId);
        case ResourceType.SystemDocument:
          return PathsHelper.ValidateSystemDocumentId(resourceId);
        case ResourceType.InteropUser:
          return PathsHelper.ValidateInteropUserId(resourceId);
        case ResourceType.AuthPolicyElement:
          return PathsHelper.ValidateAuthPolicyElementId(resourceId);
        case ResourceType.EncryptionScope:
          return PathsHelper.ValidateEncryptionScopeId(resourceId);
        default:
          return false;
      }
    }

    internal static bool ValidateDatabaseId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.Database > 0U;
    }

    internal static bool ValidateEncryptionScopeId(string resourceIdString)
    {
      ResourceId rid;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.EncryptionScope > 0UL;
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

    internal static bool ValidateRoleAssignmentId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.RoleAssignment > 0UL;
    }

    internal static bool ValidateRoleDefinitionId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.RoleDefinition > 0UL;
    }

    internal static bool ValidateAuthPolicyElementId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.AuthPolicyElement > 0UL;
    }

    internal static bool ValidateSystemDocumentId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.SystemDocument > 0UL;
    }

    internal static bool ValidatePartitionedSystemDocumentId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.PartitionedSystemDocument > 0UL;
    }

    internal static bool ValidateInteropUserId(string resourceIdString)
    {
      ResourceId rid = (ResourceId) null;
      return ResourceId.TryParse(resourceIdString, out rid) && rid.InteropUser > 0UL;
    }

    internal static bool IsPublicResource(Type resourceType) => resourceType == typeof (Database) || resourceType == typeof (ClientEncryptionKey) || resourceType == typeof (DocumentCollection) || resourceType == typeof (StoredProcedure) || resourceType == typeof (UserDefinedFunction) || resourceType == typeof (Trigger) || resourceType == typeof (Conflict) || typeof (Attachment).IsAssignableFrom(resourceType) || resourceType == typeof (User) || typeof (Permission).IsAssignableFrom(resourceType) || typeof (Document).IsAssignableFrom(resourceType) || resourceType == typeof (Offer) || resourceType == typeof (Schema) || resourceType == typeof (Snapshot);

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
