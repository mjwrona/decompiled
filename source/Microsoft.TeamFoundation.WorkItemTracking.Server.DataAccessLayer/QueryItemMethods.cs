// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItemMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class QueryItemMethods
  {
    private static SecurityIdentifier s_creatorOwnerIdentifier = new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, (SecurityIdentifier) null);
    private static string s_cssProjectUriHerder = "vstfs:///Classification/TeamProject/";

    internal static AccessControlListMetadata[] GetAclMetadata(IVssRequestContext tfRequestContext) => new AccessControlListMetadata[2]
    {
      QueryItemMethods.GetAclMetadata(tfRequestContext, "STORED_QUERY"),
      QueryItemMethods.GetAclMetadata(tfRequestContext, "STORED_QUERY_FOLDER")
    };

    internal static AccessControlListMetadata GetAclMetadata(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item)
    {
      string objectClassId = "STORED_QUERY";
      if (item != null && item.Existing.IsFolder)
        objectClassId = "STORED_QUERY_FOLDER";
      return QueryItemMethods.GetAclMetadata(tfRequestContext, objectClassId);
    }

    internal static AccessControlListMetadata GetAclMetadata(
      IVssRequestContext tfRequestContext,
      string objectClassId)
    {
      AccessControlListMetadata aclMetadata = new AccessControlListMetadata();
      aclMetadata.ObjectClassId = objectClassId;
      aclMetadata.FullSelectionPermission = "FullControl";
      aclMetadata.IrrevocableAdminPermissions = new string[2]
      {
        "Read",
        "ManagePermissions"
      };
      Type enumType = typeof (QueryItemPermissionsInternal);
      List<string> stringList = new List<string>();
      for (int index = 0; index < 32; ++index)
      {
        int num = 1 << index;
        if (Enum.IsDefined(enumType, (object) num))
          stringList.Add(Enum.GetName(enumType, (object) num));
      }
      aclMetadata.PermissionNames = stringList.ToArray();
      if (!string.IsNullOrEmpty(objectClassId))
      {
        string[] strArray1 = new string[stringList.Count];
        string[] strArray2 = new string[stringList.Count];
        string[] strArray3 = new string[stringList.Count];
        for (int index = 0; index < stringList.Count; ++index)
        {
          string resourceName1 = objectClassId + "_Permission_" + stringList[index];
          strArray2[index] = DalResourceStrings.Get(resourceName1);
          string resourceName2 = objectClassId + "_PermissionDescription_" + stringList[index];
          strArray3[index] = DalResourceStrings.Get(resourceName2);
        }
        aclMetadata.PermissionDisplayStrings = strArray2;
        aclMetadata.PermissionDescriptions = strArray3;
      }
      aclMetadata.PermissionRequirements = new RequiredPermissions[3]
      {
        new RequiredPermissions("Contribute", 1),
        new RequiredPermissions("Delete", 3),
        new RequiredPermissions("ManagePermissions", 1)
      };
      return aclMetadata;
    }

    internal static void ValidatePermissions(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      ExtendedAccessControlListData aclData)
    {
      int num = (1 << QueryItemMethods.GetAclMetadata(tfRequestContext, item).PermissionNames.Length) - 1;
      IdentityDescriptor creatorOwnerDescriptor = QueryItemMethods.GetCreatorOwnerDescriptor();
      List<AccessControlEntryData> controlEntryDataList = new List<AccessControlEntryData>();
      foreach (AccessControlEntryData permission in aclData.Permissions)
      {
        if (IdentityDescriptorComparer.Instance.Equals(permission.Descriptor, creatorOwnerDescriptor))
        {
          tfRequestContext.Trace(900514, TraceLevel.Info, "DataAccessLayer", nameof (QueryItemMethods), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Ignoring permissions for CREATOR OWNER SID on query item {0} ({1}) since creator / owner permission restrictions are no longer supported", (object) item.Id, (object) item.QueryName));
          controlEntryDataList.Add(permission);
        }
        if ((permission.Allow & ~num) != 0 || (permission.Deny & ~num) != 0)
        {
          if (!tfRequestContext.Items.ContainsKey("WIT.FromRest"))
            throw new LegacyValidationException(tfRequestContext, DalResourceStrings.Manager, 600287, "InvalidPermissionCombination", "QueryHierarchyInvalidPermissionsSet", (object[]) null);
          if ((bool) tfRequestContext.Items["WIT.FromRest"])
            throw new WorkItemTrackingQueryUnauthorizedAccessException(item.QueryName, AccessType.Write);
          throw new WorkItemTrackingQueryUnauthorizedAccessException(item.QueryName, AccessType.Delete);
        }
        if ((permission.Allow & permission.Deny) != 0)
        {
          if (!tfRequestContext.Items.ContainsKey("WIT.FromRest"))
            throw new LegacyValidationException(tfRequestContext, DalResourceStrings.Manager, 600287, "InvalidPermissionCombination", "QueryHierarchyInvalidPermissionsSet", (object[]) null);
          if ((bool) tfRequestContext.Items["WIT.FromRest"])
            throw new WorkItemTrackingQueryUnauthorizedAccessException(item.QueryName, AccessType.Write);
          throw new WorkItemTrackingQueryUnauthorizedAccessException(item.QueryName, AccessType.Delete);
        }
      }
      foreach (AccessControlEntryData controlEntryData in controlEntryDataList)
        aclData.Permissions.Remove(controlEntryData);
    }

    private static bool IsIrrevocableAdminPermission(int permission) => permission == 1 || permission == 8;

    internal static string GetPermissionDisplayStrings(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      int permissionMask)
    {
      return QueryItemMethods.GetPermissionDisplayStrings(QueryItemMethods.GetAclMetadata(tfRequestContext, item).PermissionDisplayStrings, permissionMask);
    }

    private static string GetPermissionDisplayStrings(
      string[] allPermissionDisplayStrings,
      int permissionMask)
    {
      List<string> stringList = new List<string>();
      for (int index = 0; index < allPermissionDisplayStrings.Length; ++index)
      {
        if ((permissionMask & 1 << index) != 0)
          stringList.Add(allPermissionDisplayStrings[index]);
      }
      return string.Join(", ", stringList.ToArray());
    }

    internal static void PopulateSecurityInfo(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item)
    {
      QueryItemMethods.PopulateSecurityInfo(tfRequestContext, new Dictionary<Guid, ServerQueryItem>()
      {
        {
          item.Id,
          item
        }
      });
    }

    internal static void PopulateSecurityInfo(
      IVssRequestContext tfRequestContext,
      Dictionary<Guid, ServerQueryItem> queryItems)
    {
      IEnumerable<Guid> itemIds;
      IEnumerable<int> projectIds;
      QueryItemMethods.GetSecurityInfoRequest((IEnumerable<ServerQueryItem>) queryItems.Values, out itemIds, out projectIds);
      bool isAnonymousOrPublicUser = !CommonWITUtils.HasCrossProjectQueryPermission(tfRequestContext);
      DalPopulateSecurityInfoElement element;
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(tfRequestContext))
      {
        element = DalSqlElement.GetElement<DalPopulateSecurityInfoElement>(sqlBatch);
        element.JoinBatch(tfRequestContext, (IVssIdentity) tfRequestContext.GetUserIdentity(), itemIds, projectIds, isAnonymousOrPublicUser);
        sqlBatch.ExecuteBatch();
      }
      QueryItemMethods.PopulateSecurityInfoFromResults(tfRequestContext, element.GetResults(), queryItems);
    }

    private static void GetSecurityInfoRequest(
      IEnumerable<ServerQueryItem> queryItems,
      out IEnumerable<Guid> itemIds,
      out IEnumerable<int> projectIds)
    {
      HashSet<int> intSet = new HashSet<int>();
      List<Guid> guidList = new List<Guid>();
      foreach (ServerQueryItem queryItem in queryItems)
      {
        if ((queryItem.Action == PersistenceAction.Insert && queryItem.New.ParentId.Equals(Guid.Empty) && queryItem.New.ProjectId > 0 || queryItem.Action == PersistenceAction.Update && queryItem.New.ParentId.Equals(Guid.Empty) && queryItem.Existing.ProjectId > 0) && queryItem.New.IsPublic.HasValue && queryItem.New.IsPublic.Value)
        {
          int num = queryItem.Action == PersistenceAction.Insert ? queryItem.New.ProjectId : queryItem.Existing.ProjectId;
          if (!intSet.Contains(num))
            intSet.Add(num);
        }
        else
          guidList.Add(queryItem.Id);
      }
      itemIds = (IEnumerable<Guid>) guidList;
      projectIds = (IEnumerable<int>) intSet;
    }

    private static void PopulateSecurityInfoFromResults(
      IVssRequestContext tfRequestContext,
      PayloadTable results,
      Dictionary<Guid, ServerQueryItem> queryItems)
    {
      IdentityService service = tfRequestContext.GetService<IdentityService>();
      string field = results.Columns.Contains("OwnerIdentifier") ? "OwnerIdentifier" : "Owner";
      Dictionary<int, ServerQueryItem> dictionary = new Dictionary<int, ServerQueryItem>();
      foreach (PayloadTable.PayloadRow row in results.Rows)
      {
        if (row["ID"] != null)
        {
          Guid guid = (Guid) row["ID"];
          ServerQueryItem serverQueryItem;
          queryItems.TryGetValue(guid, out serverQueryItem);
          bool flag = true;
          if (serverQueryItem == null)
          {
            serverQueryItem = new ServerQueryItem(guid);
            flag = false;
          }
          serverQueryItem.IsLoaded = true;
          serverQueryItem.SecurityToken = row["SecurityToken"] as string;
          serverQueryItem.Existing.IsFolder = (bool) row["fFolder"];
          serverQueryItem.Existing.IsPublic = new bool?((bool) row["fPublic"]);
          string ownerIdentifier = row[field] as string;
          serverQueryItem.Existing.Owner = QueryItemMethods.ConvertOwnerIdToDescriptor(tfRequestContext, service, ownerIdentifier, serverQueryItem.Existing.IsPublic);
          serverQueryItem.Existing.QueryName = row["Name"] as string;
          if (results.Columns.Contains("DataspaceId"))
          {
            Guid dataspaceIdentifier = tfRequestContext.GetService<IDataspaceService>().QueryDataspace(tfRequestContext, (int) row["DataspaceId"]).DataspaceIdentifier;
            serverQueryItem.Existing.ProjectId = tfRequestContext.WitContext().TreeService.LegacyGetTreeNode(dataspaceIdentifier).Id;
          }
          else
            serverQueryItem.Existing.ProjectId = (int) row["ProjectID"];
          serverQueryItem.Existing.ParentId = row["ParentID"] == null ? Guid.Empty : (Guid) row["ParentID"];
          serverQueryItem.Existing.QueryName = row["Name"] as string;
          if (!flag && serverQueryItem.Existing.IsPublic.Value && serverQueryItem.Existing.ParentId.Equals(Guid.Empty))
            dictionary.Add(serverQueryItem.Existing.ProjectId, serverQueryItem);
        }
      }
      foreach (ServerQueryItem serverQueryItem1 in queryItems.Values)
      {
        if (!serverQueryItem1.IsLoaded && (serverQueryItem1.Action == PersistenceAction.Insert && serverQueryItem1.New.ParentId.Equals(Guid.Empty) && serverQueryItem1.New.ProjectId > 0 || serverQueryItem1.Action == PersistenceAction.Update && serverQueryItem1.New.ParentId.Equals(Guid.Empty) && serverQueryItem1.Existing.ProjectId > 0))
        {
          bool? isPublic = serverQueryItem1.New.IsPublic;
          if (isPublic.HasValue)
          {
            isPublic = serverQueryItem1.New.IsPublic;
            if (isPublic.Value)
            {
              int key = serverQueryItem1.Action == PersistenceAction.Insert ? serverQueryItem1.New.ProjectId : serverQueryItem1.Existing.ProjectId;
              ServerQueryItem serverQueryItem2;
              if (dictionary.TryGetValue(key, out serverQueryItem2))
              {
                serverQueryItem1.New.Parent = serverQueryItem2;
                serverQueryItem1.New.NearestPersistedParent = serverQueryItem2;
              }
            }
          }
        }
      }
    }

    private static IdentityDescriptor ConvertOwnerIdToDescriptor(
      IVssRequestContext tfRequestContext,
      IdentityService identityService,
      string ownerIdentifier,
      bool? isPublic)
    {
      string identityType = "System.Security.Principal.WindowsIdentity";
      if (string.IsNullOrEmpty(ownerIdentifier) && isPublic.HasValue && !isPublic.Value)
        return tfRequestContext.UserContext;
      Guid result;
      if (Guid.TryParse(ownerIdentifier, out result) && result != Guid.Empty)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = identityService.ReadIdentities(tfRequestContext, (IList<Guid>) new Guid[1]
        {
          result
        }, QueryMembership.None, (IEnumerable<string>) null);
        if (identityList.Count > 0 && identityList[0] != null)
          return identityList[0].Descriptor;
      }
      if (VssStringComparer.IdentityDescriptor.Equals(ownerIdentifier, tfRequestContext.UserContext.Identifier))
        identityType = tfRequestContext.UserContext.IdentityType;
      return !string.IsNullOrEmpty(ownerIdentifier) ? new IdentityDescriptor(identityType, ownerIdentifier) : (IdentityDescriptor) null;
    }

    internal static bool HasPermission(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      int requestedPermissions)
    {
      tfRequestContext.TraceEnter(900413, "DataAccessLayer", nameof (QueryItemMethods), nameof (HasPermission));
      tfRequestContext.Trace(900414, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "item: {0} {1}, requestedPermissions: {2}.", (object) item.Id, item.IsLoaded ? (object) item.Existing.QueryName : (object) item.New.QueryName, (object) requestedPermissions);
      bool alwaysAllowAdministrators = QueryItemMethods.IsIrrevocableAdminPermission(requestedPermissions);
      if (!alwaysAllowAdministrators && item.AccessControlList != null && (requestedPermissions & 8) == 0 && QueryItemMethods.HasPermission(tfRequestContext, item, 8))
      {
        AccessControlEntryData controlEntryData = QueryItemMethods.QueryPermission(item.AccessControlList, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
        alwaysAllowAdministrators = controlEntryData != null && (controlEntryData.Allow & requestedPermissions) != 0;
      }
      tfRequestContext.Trace(900415, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "alwaysAllowAdmins: {0}.", (object) alwaysAllowAdministrators);
      IVssSecurityNamespace queryItemSecurity = (IVssSecurityNamespace) tfRequestContext.GetService<WorkItemTrackingService>().QueryItemSecurity;
      IdentityDescriptor userContext = tfRequestContext.UserContext;
      IdentityDescriptor x = (IdentityDescriptor) null;
      bool? nullable = new bool?(true);
      if (item.Action == PersistenceAction.Insert)
      {
        x = userContext;
        if (item.New.Owner != (IdentityDescriptor) null && !string.IsNullOrEmpty(item.New.Owner.Identifier))
          x = item.New.Owner;
        nullable = item.New.IsPublic;
      }
      else if (item.IsLoaded)
      {
        if (item.Existing.Owner != (IdentityDescriptor) null && !string.IsNullOrEmpty(item.Existing.Owner.Identifier))
          x = item.Existing.Owner;
        nullable = item.Existing.IsPublic;
      }
      tfRequestContext.Trace(900416, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "itemOwner: {0}, isPublic: {1}.", x != (IdentityDescriptor) null ? (object) x.Identifier : (object) string.Empty, nullable.HasValue ? (object) nullable.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : (object) string.Empty);
      bool flag = false;
      if (nullable.HasValue && !nullable.Value)
      {
        flag = x != (IdentityDescriptor) null && IdentityDescriptorComparer.Instance.Equals(x, userContext);
        tfRequestContext.Trace(900417, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "Check permission for private item. allowed: {0}.", (object) flag);
      }
      else
      {
        if (item.SecurityToken == null)
          throw new LegacyValidationException(tfRequestContext, DalResourceStrings.Manager, 600289, "DeniedOrNotExist", "QueryHierarchyItemDoesNotExist", (object[]) null);
        if (queryItemSecurity.HasPermission(tfRequestContext, item.SecurityToken, requestedPermissions, alwaysAllowAdministrators))
          flag = true;
        tfRequestContext.Trace(900418, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "Check permission for public item. allowed: {0}.", (object) flag);
      }
      if (!flag)
      {
        ServerQueryItem nearestPersistedParent = item.New.NearestPersistedParent;
        if ((item.Action == PersistenceAction.Update || item.Action == PersistenceAction.None && item.IsLoaded) && item.Existing.ParentId.Equals(Guid.Empty) && (!item.Existing.IsPublic.HasValue || item.Existing.IsPublic.Value) && (item.Existing.Owner == (IdentityDescriptor) null || string.IsNullOrEmpty(item.Existing.Owner.Identifier)) || item.Action == PersistenceAction.Insert && nearestPersistedParent != null && nearestPersistedParent.Existing.ParentId.Equals(Guid.Empty) && (!nearestPersistedParent.Existing.IsPublic.HasValue || nearestPersistedParent.Existing.IsPublic.Value) && (nearestPersistedParent.Existing.Owner == (IdentityDescriptor) null || string.IsNullOrEmpty(nearestPersistedParent.Existing.Owner.Identifier)))
        {
          tfRequestContext.Trace(900419, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "Work around project creation scenarios.");
          string projectGuid = (string) null;
          flag = QueryItemMethods.GetProjectGuidFromQueryItem(item, out projectGuid) && DataAccessLayerImpl.IsCreatingProject(tfRequestContext, QueryItemMethods.s_cssProjectUriHerder + projectGuid);
        }
      }
      tfRequestContext.Trace(900431, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "allowed: {0}.", (object) flag);
      tfRequestContext.TraceLeave(900432, "DataAccessLayer", nameof (QueryItemMethods), nameof (HasPermission));
      return flag;
    }

    private static AccessControlEntryData QueryPermission(
      ExtendedAccessControlListData aclData,
      IdentityDescriptor descriptor)
    {
      foreach (AccessControlEntryData permission in aclData.Permissions)
      {
        if (IdentityDescriptorComparer.Instance.Equals(permission.Descriptor, descriptor))
          return permission;
      }
      return (AccessControlEntryData) null;
    }

    internal static void CheckPermission(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      int requestedPermissions)
    {
      if (QueryItemMethods.HasPermission(tfRequestContext, item, requestedPermissions))
        return;
      QueryItemMethods.ThrowException(tfRequestContext, item, requestedPermissions, "QueryHierarchyItemAccessException");
    }

    internal static void CheckPermissionForAllChildren(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      int requestedPermissions)
    {
      tfRequestContext.TraceEnter(900423, "DataAccessLayer", nameof (QueryItemMethods), nameof (CheckPermissionForAllChildren));
      try
      {
        tfRequestContext.Trace(900424, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "item: {0} {1}, requestedPermissions: {2}.", (object) item.Id, item.IsLoaded ? (object) item.Existing.QueryName : (object) item.New.QueryName, (object) requestedPermissions);
        bool alwaysAllowAdministrators = QueryItemMethods.IsIrrevocableAdminPermission(requestedPermissions);
        IVssSecurityNamespace queryItemSecurity = (IVssSecurityNamespace) tfRequestContext.GetService<WorkItemTrackingService>().QueryItemSecurity;
        QueryItemMethods.CheckPermission(tfRequestContext, item, requestedPermissions);
        if ((item.Action != PersistenceAction.Insert || !item.New.IsPublic.HasValue || item.New.IsPublic.Value ? (item.Action == PersistenceAction.Insert || !item.Existing.IsPublic.HasValue ? 0 : (!item.Existing.IsPublic.Value ? 1 : 0)) : 1) != 0)
        {
          tfRequestContext.TraceLeave(900435, "DataAccessLayer", nameof (QueryItemMethods), nameof (CheckPermissionForAllChildren));
        }
        else
        {
          tfRequestContext.Trace(900425, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "Checking permission for the query item.");
          if (!queryItemSecurity.HasPermission(tfRequestContext, item.SecurityToken, requestedPermissions, alwaysAllowAdministrators))
            QueryItemMethods.ThrowException(tfRequestContext, item, requestedPermissions, "QueryHierarchyChildrenAccessException");
          tfRequestContext.Trace(900426, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "Checking permission for all children of the query item.");
          if (queryItemSecurity.HasPermissionForAllChildren(tfRequestContext, item.SecurityToken, requestedPermissions, alwaysAllowAdministrators: alwaysAllowAdministrators))
            return;
          QueryItemMethods.ThrowException(tfRequestContext, item, requestedPermissions, "QueryHierarchyChildrenAccessException");
        }
      }
      finally
      {
        tfRequestContext.TraceLeave(900436, "DataAccessLayer", nameof (QueryItemMethods), nameof (CheckPermissionForAllChildren));
      }
    }

    internal static bool HasPermissionForAnyChildren(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      int requestedPermissions)
    {
      tfRequestContext.TraceEnter(900429, "DataAccessLayer", nameof (QueryItemMethods), nameof (HasPermissionForAnyChildren));
      tfRequestContext.Trace(900430, TraceLevel.Verbose, "DataAccessLayer", nameof (QueryItemMethods), "item: {0} {1}, requestedPermissions: {2}.", (object) item.Id, item.IsLoaded ? (object) item.Existing.QueryName : (object) item.New.QueryName, (object) requestedPermissions);
      bool alwaysAllowAdministrators = QueryItemMethods.IsIrrevocableAdminPermission(requestedPermissions);
      IVssSecurityNamespace queryItemSecurity = (IVssSecurityNamespace) tfRequestContext.GetService<WorkItemTrackingService>().QueryItemSecurity;
      if (QueryItemMethods.HasPermission(tfRequestContext, item, requestedPermissions))
      {
        tfRequestContext.TraceLeave(900437, "DataAccessLayer", nameof (QueryItemMethods), nameof (HasPermissionForAnyChildren));
        return true;
      }
      if ((item.Action != PersistenceAction.Insert || !item.New.IsPublic.HasValue || item.New.IsPublic.Value ? (item.Action == PersistenceAction.Insert || !item.Existing.IsPublic.HasValue ? 0 : (!item.Existing.IsPublic.Value ? 1 : 0)) : 1) != 0)
      {
        tfRequestContext.TraceLeave(900438, "DataAccessLayer", nameof (QueryItemMethods), nameof (HasPermissionForAnyChildren));
        return true;
      }
      if (queryItemSecurity.HasPermission(tfRequestContext, item.SecurityToken, requestedPermissions, alwaysAllowAdministrators) || queryItemSecurity.HasPermissionForAnyChildren(tfRequestContext, item.SecurityToken, requestedPermissions, alwaysAllowAdministrators: alwaysAllowAdministrators))
      {
        tfRequestContext.TraceLeave(900439, "DataAccessLayer", nameof (QueryItemMethods), nameof (HasPermissionForAnyChildren));
        return true;
      }
      tfRequestContext.TraceLeave(900441, "DataAccessLayer", nameof (QueryItemMethods), nameof (HasPermissionForAnyChildren));
      return false;
    }

    internal static void CheckPermissionForAnyChildren(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      int requestedPermissions)
    {
      if (QueryItemMethods.HasPermissionForAnyChildren(tfRequestContext, item, requestedPermissions))
        return;
      QueryItemMethods.ThrowException(tfRequestContext, item, requestedPermissions, "QueryHierarchyItemAccessException");
    }

    private static SecurityIdentifier GetCreatorOwnerIdentifier() => QueryItemMethods.s_creatorOwnerIdentifier;

    private static IdentityDescriptor GetCreatorOwnerDescriptor() => IdentityHelper.CreateWindowsDescriptor(QueryItemMethods.GetCreatorOwnerIdentifier());

    private static void ThrowException(
      IVssRequestContext tfRequestContext,
      ServerQueryItem item,
      int requestedPermissions,
      string messageResourceName)
    {
      int clientVersion = tfRequestContext.GetClientVersion();
      if (clientVersion < 3)
        throw new LegacyValidationException(tfRequestContext, DalResourceStrings.Manager, 600171, "AccessException", messageResourceName, new object[3]
        {
          (object) tfRequestContext.DomainUserName,
          (object) QueryItemMethods.GetPermissionDisplayStrings(tfRequestContext, item, requestedPermissions),
          (object) item.Existing.QueryName
        });
      if (clientVersion < 4)
        throw new LegacyValidationException(tfRequestContext, DalResourceStrings.Manager, 600288, "AccessException", messageResourceName, new object[3]
        {
          (object) tfRequestContext.DomainUserName,
          (object) QueryItemMethods.GetPermissionDisplayStrings(tfRequestContext, item, requestedPermissions),
          (object) item.Existing.QueryName
        });
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = tfRequestContext.GetUserIdentity();
      if (tfRequestContext.Items.ContainsKey("WIT.FromRest"))
      {
        if ((bool) tfRequestContext.Items["WIT.FromRest"])
          throw new WorkItemTrackingQueryUnauthorizedAccessException(item.QueryName, AccessType.Write);
        throw new WorkItemTrackingQueryUnauthorizedAccessException(item.QueryName, AccessType.Delete);
      }
      throw new LegacyValidationException(tfRequestContext, DalResourceStrings.Manager, 600288, "AccessException", messageResourceName, new object[3]
      {
        (object) userIdentity.DisplayName,
        (object) QueryItemMethods.GetPermissionDisplayStrings(tfRequestContext, item, requestedPermissions),
        (object) item.Existing.QueryName
      });
    }

    private static bool GetProjectGuidFromQueryItem(ServerQueryItem item, out string projectGuid)
    {
      string[] strArray = item.SecurityToken.Split(new char[2]
      {
        '$',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 0)
      {
        projectGuid = strArray[0];
        return true;
      }
      projectGuid = (string) null;
      return false;
    }
  }
}
