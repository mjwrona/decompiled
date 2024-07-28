// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.HostingResources
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class HostingResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (HostingResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => HostingResources.s_resMgr;

    private static string Get(string resourceName) => HostingResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? HostingResources.Get(resourceName) : HostingResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) HostingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? HostingResources.GetInt(resourceName) : (int) HostingResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) HostingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? HostingResources.GetBool(resourceName) : (bool) HostingResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => HostingResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = HostingResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string OneOfTwoRequiredArgumentsIsMissing(object arg0, object arg1) => HostingResources.Format(nameof (OneOfTwoRequiredArgumentsIsMissing), arg0, arg1);

    public static string OneOfTwoRequiredArgumentsIsMissing(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (OneOfTwoRequiredArgumentsIsMissing), culture, arg0, arg1);
    }

    public static string WarningFailedToSyncIdentity(object arg0) => HostingResources.Format(nameof (WarningFailedToSyncIdentity), arg0);

    public static string WarningFailedToSyncIdentity(object arg0, CultureInfo culture) => HostingResources.Format(nameof (WarningFailedToSyncIdentity), culture, arg0);

    public static string WarningFailedToSyncIdentityWithDisplayName(object arg0, object arg1) => HostingResources.Format(nameof (WarningFailedToSyncIdentityWithDisplayName), arg0, arg1);

    public static string WarningFailedToSyncIdentityWithDisplayName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (WarningFailedToSyncIdentityWithDisplayName), culture, arg0, arg1);
    }

    public static string WarningExceptionSyncingIdentity(object arg0, object arg1) => HostingResources.Format(nameof (WarningExceptionSyncingIdentity), arg0, arg1);

    public static string WarningExceptionSyncingIdentity(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (WarningExceptionSyncingIdentity), culture, arg0, arg1);
    }

    public static string stmt_GetCyclicGroupMemberships() => HostingResources.Get(nameof (stmt_GetCyclicGroupMemberships));

    public static string stmt_GetCyclicGroupMemberships(CultureInfo culture) => HostingResources.Get(nameof (stmt_GetCyclicGroupMemberships), culture);

    public static string InvalidContinuationToken(object arg0) => HostingResources.Format(nameof (InvalidContinuationToken), arg0);

    public static string InvalidContinuationToken(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidContinuationToken), culture, arg0);

    public static string MustSpecifyPagingResults() => HostingResources.Get(nameof (MustSpecifyPagingResults));

    public static string MustSpecifyPagingResults(CultureInfo culture) => HostingResources.Get(nameof (MustSpecifyPagingResults), culture);

    public static string MinGroupSequenceIdError(object arg0, object arg1) => HostingResources.Format(nameof (MinGroupSequenceIdError), arg0, arg1);

    public static string MinGroupSequenceIdError(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (MinGroupSequenceIdError), culture, arg0, arg1);

    public static string ShardingOngoingBlocksTransferIdentityRights() => HostingResources.Get(nameof (ShardingOngoingBlocksTransferIdentityRights));

    public static string ShardingOngoingBlocksTransferIdentityRights(CultureInfo culture) => HostingResources.Get(nameof (ShardingOngoingBlocksTransferIdentityRights), culture);

    public static string ShardingBlocksAddOrUpdateNewFrameworkUser() => HostingResources.Get(nameof (ShardingBlocksAddOrUpdateNewFrameworkUser));

    public static string ShardingBlocksAddOrUpdateNewFrameworkUser(CultureInfo culture) => HostingResources.Get(nameof (ShardingBlocksAddOrUpdateNewFrameworkUser), culture);

    public static string ShardingBlocksTransferIdentityRights() => HostingResources.Get(nameof (ShardingBlocksTransferIdentityRights));

    public static string ShardingBlocksTransferIdentityRights(CultureInfo culture) => HostingResources.Get(nameof (ShardingBlocksTransferIdentityRights), culture);

    public static string CannotAddServicePrincipalToGroup(object arg0) => HostingResources.Format(nameof (CannotAddServicePrincipalToGroup), arg0);

    public static string CannotAddServicePrincipalToGroup(object arg0, CultureInfo culture) => HostingResources.Format(nameof (CannotAddServicePrincipalToGroup), culture, arg0);

    public static string OneOfThreeRequiredArgumentsIsMissing(
      object arg0,
      object arg1,
      object arg2)
    {
      return HostingResources.Format(nameof (OneOfThreeRequiredArgumentsIsMissing), arg0, arg1, arg2);
    }

    public static string OneOfThreeRequiredArgumentsIsMissing(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (OneOfThreeRequiredArgumentsIsMissing), culture, arg0, arg1, arg2);
    }

    public static string RestoreGroupScopeValidationError(object arg0) => HostingResources.Format(nameof (RestoreGroupScopeValidationError), arg0);

    public static string RestoreGroupScopeValidationError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (RestoreGroupScopeValidationError), culture, arg0);

    public static string UpdateGroupScopeVisibilityError(object arg0, object arg1) => HostingResources.Format(nameof (UpdateGroupScopeVisibilityError), arg0, arg1);

    public static string UpdateGroupScopeVisibilityError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (UpdateGroupScopeVisibilityError), culture, arg0, arg1);
    }

    public static string RestoreGroupScopeExecutionError(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (RestoreGroupScopeExecutionError), arg0, arg1, arg2);

    public static string RestoreGroupScopeExecutionError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (RestoreGroupScopeExecutionError), culture, arg0, arg1, arg2);
    }

    public static string CannotManageGroupMembershipInEnterpriseScope(object arg0, object arg1) => HostingResources.Format(nameof (CannotManageGroupMembershipInEnterpriseScope), arg0, arg1);

    public static string CannotManageGroupMembershipInEnterpriseScope(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (CannotManageGroupMembershipInEnterpriseScope), culture, arg0, arg1);
    }
  }
}
