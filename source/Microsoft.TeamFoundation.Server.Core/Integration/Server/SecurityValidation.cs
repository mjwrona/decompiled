// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.SecurityValidation
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal static class SecurityValidation
  {
    private const int GroupDescriptionMaxLength = 256;

    internal static void CheckFactorAndValue(
      SearchFactor factor,
      ref string factorValue,
      string factorParamName,
      string valueParamName)
    {
      switch (factor)
      {
        case SearchFactor.Sid:
          SecurityValidation.CheckSid(ref factorValue, valueParamName);
          break;
        case SearchFactor.AccountName:
          SecurityValidation.CheckAccountName(ref factorValue, valueParamName);
          break;
        case SearchFactor.DistinguishedName:
          SecurityValidation.CheckDistinguishedName(ref factorValue, valueParamName);
          break;
        case SearchFactor.AdministrativeApplicationGroup:
          SecurityValidation.CheckProjectUri(ref factorValue, true, valueParamName);
          break;
        case SearchFactor.ServiceApplicationGroup:
          break;
        case SearchFactor.EveryoneApplicationGroup:
          break;
        default:
          throw new ArgumentException(TFCommonResources.GSS_ARGUMENT_EXCEPTION((object) factorParamName), factorParamName);
      }
    }

    internal static void CheckFactorAndValueArray(
      SearchFactor factor,
      ref string[] factorValues,
      string factorParamName,
      string valueParamName)
    {
      switch (factor)
      {
        case SearchFactor.Sid:
          SecurityValidation.CheckSidArray(factorValues, valueParamName);
          break;
        case SearchFactor.AccountName:
          SecurityValidation.CheckAccountNameArray(factorValues, valueParamName);
          break;
        case SearchFactor.DistinguishedName:
          SecurityValidation.CheckDistinguishedNameArray(factorValues, valueParamName);
          break;
        case SearchFactor.AdministrativeApplicationGroup:
          SecurityValidation.CheckProjectUriArray(factorValues, true, valueParamName);
          break;
        case SearchFactor.ServiceApplicationGroup:
          break;
        case SearchFactor.EveryoneApplicationGroup:
          break;
        default:
          throw new ArgumentException(TFCommonResources.GSS_ARGUMENT_EXCEPTION((object) factorParamName), factorParamName);
      }
    }

    internal static void CheckSid(ref string sid, string paramName)
    {
      if (sid == null)
        throw new ArgumentNullException(paramName);
      sid = sid.Trim();
      if (sid.Length == 0)
        throw new ArgumentException(TFCommonResources.BAD_SID((object) sid, (object) paramName), paramName);
    }

    internal static void CheckSidArray(string[] sids, string paramName)
    {
      if (sids == null)
        throw new ArgumentNullException(paramName);
      if (sids.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_SID_ARRAY((object) paramName), paramName);
      for (int index = 0; index < sids.Length; ++index)
        SecurityValidation.CheckSid(ref sids[index], string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", (object) paramName, (object) index));
    }

    internal static void CheckAccountName(ref string accountName, string paramName)
    {
      if (accountName == null)
        throw new ArgumentNullException(paramName);
      accountName = accountName.Trim();
      if (accountName.Length == 0 || ArgumentUtility.IsInvalidString(accountName))
        throw new ArgumentException(TFCommonResources.BAD_ACCOUNT_NAME((object) accountName));
    }

    internal static void CheckAccountNameArray(string[] accountNames, string paramName)
    {
      if (accountNames == null)
        throw new ArgumentNullException(paramName);
      if (accountNames.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_ACCOUNT_NAME_ARRAY((object) paramName), paramName);
      for (int index = 0; index < accountNames.Length; ++index)
        SecurityValidation.CheckAccountName(ref accountNames[index], string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", (object) paramName, (object) index));
    }

    internal static void CheckDistinguishedName(ref string distinguishedName, string paramName)
    {
      if (distinguishedName == null)
        throw new ArgumentNullException(paramName);
      distinguishedName = distinguishedName.Trim();
      if (distinguishedName.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_DISTINGUISHED_NAME((object) distinguishedName));
    }

    internal static void CheckDistinguishedNameArray(string[] distinguishedNames, string paramName)
    {
      if (distinguishedNames == null)
        throw new ArgumentNullException(paramName);
      if (distinguishedNames.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_DISTINGUISHED_NAME_ARRAY((object) paramName), paramName);
      for (int index = 0; index < distinguishedNames.Length; ++index)
        SecurityValidation.CheckDistinguishedName(ref distinguishedNames[index], string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", (object) paramName, (object) index));
    }

    internal static void CheckProjectUri(
      ref string projectUri,
      bool allowNullOrEmpty,
      string paramName)
    {
      if (projectUri == null & allowNullOrEmpty)
      {
        projectUri = string.Empty;
      }
      else
      {
        if (projectUri == null)
          throw new ArgumentNullException(paramName);
        projectUri = projectUri.Trim();
        if (!allowNullOrEmpty && projectUri.Length == 0)
          throw new ArgumentException(TFCommonResources.GSS_BAD_PROJECT_URI((object) projectUri, (object) paramName), paramName);
      }
    }

    internal static void CheckProjectUriArray(
      string[] projectUris,
      bool allowNullOrEmpty,
      string paramName)
    {
      if (projectUris == null)
        throw new ArgumentNullException(paramName);
      if (projectUris.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_PROJECT_URI_ARRAY((object) paramName), paramName);
      for (int index = 0; index < projectUris.Length; ++index)
        SecurityValidation.CheckProjectUri(ref projectUris[index], allowNullOrEmpty, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", (object) paramName, (object) index));
    }

    internal static void CheckQueryMembership(QueryMembership queryMembership, string paramName)
    {
      if (queryMembership != QueryMembership.None && queryMembership != QueryMembership.Direct && queryMembership != QueryMembership.Expanded)
        throw new ArgumentException(TFCommonResources.GSS_ARGUMENT_EXCEPTION((object) paramName), paramName);
    }

    internal static void CheckSequenceId(int sequence_id, string paramName)
    {
      if (sequence_id < 0)
        throw new ArgumentException(TFCommonResources.GSS_ARGUMENT_EXCEPTION((object) paramName), paramName);
    }

    internal static void CheckGroupName(ref string groupName, string paramName)
    {
      if (groupName == null)
        throw new ArgumentNullException(paramName);
      groupName = groupName.Trim();
      if (groupName.Length == 0 || groupName.Length > 256 || groupName.EndsWith(".", StringComparison.Ordinal) || groupName.IndexOfAny(SidIdentityHelper.IllegalNameChars) >= 0 || ArgumentUtility.IsInvalidString(groupName) || VssStringComparer.ReservedGroupName.Equals(groupName, PermissionNamespaces.Global) || VssStringComparer.ReservedGroupName.Equals(groupName, GroupWellKnownShortNames.NamespaceAdministratorsGroup) || VssStringComparer.ReservedGroupName.Equals(groupName, GroupWellKnownShortNames.ServiceUsersGroup) || VssStringComparer.ReservedGroupName.Equals(groupName, GroupWellKnownShortNames.EveryoneGroup))
        throw new ArgumentException(TFCommonResources.BAD_GROUP_NAME((object) groupName));
    }

    internal static void CheckGroupDescription(ref string groupDescription, string paramName)
    {
      if (groupDescription == null)
        groupDescription = string.Empty;
      groupDescription = groupDescription.Trim();
      if (groupDescription.Length > 256)
        throw new ArgumentException(TFCommonResources.BAD_GROUP_DESCRIPTION_TOO_LONG((object) 256), paramName);
      if (ArgumentUtility.IsInvalidString(groupDescription))
        throw new ArgumentException(TFCommonResources.BAD_GROUP_DESCRIPTION((object) groupDescription), paramName);
    }

    internal static void CheckApplicationGroupPropertyAndValue(
      ApplicationGroupProperty property,
      ref string value,
      string propertyParamName,
      string valueParamName)
    {
      if (property != ApplicationGroupProperty.Name)
      {
        if (property != ApplicationGroupProperty.Description)
          throw new ArgumentException(TFCommonResources.GSS_ARGUMENT_EXCEPTION((object) propertyParamName), propertyParamName);
        SecurityValidation.CheckGroupDescription(ref value, valueParamName);
      }
      else
        SecurityValidation.CheckGroupName(ref value, valueParamName);
    }

    internal static void CheckObjectId(
      ref string objectId,
      bool allowNullOrEmpty,
      string paramName)
    {
      if (objectId == null & allowNullOrEmpty)
      {
        objectId = string.Empty;
      }
      else
      {
        if (objectId == null)
          throw new ArgumentNullException(paramName);
        objectId = objectId.Trim();
        if (!allowNullOrEmpty && objectId.Length == 0)
          throw new ArgumentException(TFCommonResources.GSS_BAD_OBJECTID((object) objectId, (object) paramName), paramName);
      }
    }

    internal static void CheckParentObjectIdNotSelf(
      string objectId,
      string parentObjectId,
      string parentParamName)
    {
      if (TFStringComparer.ObjectId.Equals(objectId, parentObjectId))
        throw new ArgumentException(TFCommonResources.GSS_BAD_PARENTOBJECTID_SELFPARENT((object) parentParamName, (object) objectId), parentParamName);
    }

    internal static void CheckObjectIdArray(string[] objectIds, string paramName)
    {
      if (objectIds == null)
        throw new ArgumentNullException(paramName);
      if (objectIds.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_OBJECTID_ARRAY((object) paramName), paramName);
      for (int index = 0; index < objectIds.Length; ++index)
        SecurityValidation.CheckObjectId(ref objectIds[index], false, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", (object) paramName, (object) index));
    }

    internal static void CheckObjectClassId(ref string objectClassId, string paramName)
    {
      if (objectClassId == null)
        throw new ArgumentNullException(paramName);
      objectClassId = objectClassId.Trim();
      if (objectClassId.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_OBJECT_CLASS_ID((object) objectClassId, (object) paramName), paramName);
    }

    internal static void CheckActionId(ref string actionId, string paramName)
    {
      if (actionId == null)
        throw new ArgumentNullException(paramName);
      actionId = actionId.Trim();
      if (actionId.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_ACTIONID((object) actionId, (object) paramName), paramName);
    }

    internal static void CheckActionIdArray(string[] actionIds, string paramName)
    {
      if (actionIds == null)
        throw new ArgumentNullException(paramName);
      if (actionIds.Length == 0)
        throw new ArgumentException(TFCommonResources.GSS_BAD_ACTIONID_ARRAY((object) paramName), paramName);
      for (int index = 0; index < actionIds.Length; ++index)
        SecurityValidation.CheckActionId(ref actionIds[index], string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", (object) paramName, (object) index));
    }

    internal static void CheckAccessControlEntry(AccessControlEntry ace, string paramName)
    {
      if (ace == null)
        throw new ArgumentNullException(paramName);
      SecurityValidation.CheckActionId(ref ace.ActionId, paramName + ".ActionId");
      SecurityValidation.CheckSid(ref ace.Sid, paramName + ".Sid");
    }

    internal static void CheckAccessControlEntryArray(AccessControlEntry[] acls, string paramName)
    {
      if (acls == null)
        throw new ArgumentNullException(paramName);
      for (int index = 0; index < acls.Length; ++index)
        SecurityValidation.CheckAccessControlEntry(acls[index], string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", (object) paramName, (object) index));
    }
  }
}
