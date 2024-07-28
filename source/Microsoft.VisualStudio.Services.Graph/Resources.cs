// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Resources
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Graph.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Graph.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Graph.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Graph.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Graph.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Graph.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Graph.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Graph.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Graph.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Graph.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Graph.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Graph.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Graph.Resources.Get(resourceName, culture);
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

    public static string CannotCreateSpecialGroup(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (CannotCreateSpecialGroup), arg0);

    public static string CannotCreateSpecialGroup(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (CannotCreateSpecialGroup), culture, arg0);

    public static string CannotMaterializeGraphSubject(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (CannotMaterializeGraphSubject), arg0, arg1);

    public static string CannotMaterializeGraphSubject(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (CannotMaterializeGraphSubject), culture, arg0, arg1);
    }

    public static string CannotModifyScopes() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (CannotModifyScopes));

    public static string CannotModifyScopes(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (CannotModifyScopes), culture);

    public static string CannotParseParameter(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (CannotParseParameter), arg0, arg1);

    public static string CannotParseParameter(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (CannotParseParameter), culture, arg0, arg1);

    public static string CheckScopeMembershipUnsupported() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (CheckScopeMembershipUnsupported));

    public static string CheckScopeMembershipUnsupported(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (CheckScopeMembershipUnsupported), culture);

    public static string DisabledUsersByScopeUnsupported() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (DisabledUsersByScopeUnsupported));

    public static string DisabledUsersByScopeUnsupported(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (DisabledUsersByScopeUnsupported), culture);

    public static string GetScopeMembershipUnsupported() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GetScopeMembershipUnsupported));

    public static string GetScopeMembershipUnsupported(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GetScopeMembershipUnsupported), culture);

    public static string GetValidWithPersistentIdOrDescriptor() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GetValidWithPersistentIdOrDescriptor));

    public static string GetValidWithPersistentIdOrDescriptor(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GetValidWithPersistentIdOrDescriptor), culture);

    public static string GraphApiIsUnavailable() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphApiIsUnavailable));

    public static string GraphApiIsUnavailable(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphApiIsUnavailable), culture);

    public static string ProviderInfoApiIsUnavailable() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (ProviderInfoApiIsUnavailable));

    public static string ProviderInfoApiIsUnavailable(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (ProviderInfoApiIsUnavailable), culture);

    public static string GraphGroupMissingRequiredFields() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphGroupMissingRequiredFields));

    public static string GraphGroupMissingRequiredFields(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphGroupMissingRequiredFields), culture);

    public static string GraphUserMissingRequiredFields() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphUserMissingRequiredFields));

    public static string GraphUserMissingRequiredFields(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphUserMissingRequiredFields), culture);

    public static string GraphServicePrincipalMissingCreateContext() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMissingCreateContext));

    public static string GraphServicePrincipalMissingCreateContext(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMissingCreateContext), culture);

    public static string GraphServicePrincipalMissingRequiredFields() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMissingRequiredFields));

    public static string GraphServicePrincipalMissingRequiredFields(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMissingRequiredFields), culture);

    public static string GraphServicePrincipalMissingUpdateContext() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMissingUpdateContext));

    public static string GraphServicePrincipalMissingUpdateContext(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMissingUpdateContext), culture);

    public static string GroupCreationContextMissingRequiredField(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GroupCreationContextMissingRequiredField), arg0);

    public static string GroupCreationContextMissingRequiredField(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GroupCreationContextMissingRequiredField), culture, arg0);

    public static string IllegalSid(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (IllegalSid), arg0);

    public static string IllegalSid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (IllegalSid), culture, arg0);

    public static string OnlyVstsGroupsInCustomScope() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (OnlyVstsGroupsInCustomScope));

    public static string OnlyVstsGroupsInCustomScope(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (OnlyVstsGroupsInCustomScope), culture);

    public static string ScopeCreationContextMissingRequiredField(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (ScopeCreationContextMissingRequiredField), arg0);

    public static string ScopeCreationContextMissingRequiredField(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (ScopeCreationContextMissingRequiredField), culture, arg0);

    public static string SubjectDoesNotMatchType(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (SubjectDoesNotMatchType), arg0, arg1);

    public static string SubjectDoesNotMatchType(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (SubjectDoesNotMatchType), culture, arg0, arg1);

    public static string UpdateSourceTenant(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UpdateSourceTenant), arg0);

    public static string UpdateSourceTenant(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UpdateSourceTenant), culture, arg0);

    public static string MemberCreationContextMissingRequiredField(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (MemberCreationContextMissingRequiredField), arg0);

    public static string MemberCreationContextMissingRequiredField(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (MemberCreationContextMissingRequiredField), culture, arg0);

    public static string MemberUpdateContextMissingRequiredField(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (MemberUpdateContextMissingRequiredField), arg0);

    public static string MemberUpdateContextMissingRequiredField(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (MemberUpdateContextMissingRequiredField), culture, arg0);

    public static string TraversalDepthNotSupported(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraversalDepthNotSupported), arg0, arg1);

    public static string TraversalDepthNotSupported(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraversalDepthNotSupported), culture, arg0, arg1);

    public static string TraversalDirectionNotSupported(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraversalDirectionNotSupported), arg0, arg1);

    public static string TraversalDirectionNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraversalDirectionNotSupported), culture, arg0, arg1);
    }

    public static string AadTraversalExtensionNotFound() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (AadTraversalExtensionNotFound));

    public static string AadTraversalExtensionNotFound(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (AadTraversalExtensionNotFound), culture);

    public static string AadGroupExpansionLimitReached() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (AadGroupExpansionLimitReached));

    public static string AadGroupExpansionLimitReached(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (AadGroupExpansionLimitReached), culture);

    public static string TraversedDecendantsLimitReached(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraversedDecendantsLimitReached), arg0);

    public static string TraversedDecendantsLimitReached(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraversedDecendantsLimitReached), culture, arg0);

    public static string TraverseDescendantsDepthNotSupported(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraverseDescendantsDepthNotSupported), arg0, arg1);

    public static string TraverseDescendantsDepthNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (TraverseDescendantsDepthNotSupported), culture, arg0, arg1);
    }

    public static string TraverseDescendantsFeatureIsUnavailable() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (TraverseDescendantsFeatureIsUnavailable));

    public static string TraverseDescendantsFeatureIsUnavailable(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (TraverseDescendantsFeatureIsUnavailable), culture);

    public static string UnsupportedRemoteDirectoryTraversal() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (UnsupportedRemoteDirectoryTraversal));

    public static string UnsupportedRemoteDirectoryTraversal(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (UnsupportedRemoteDirectoryTraversal), culture);

    public static string UnsupportedTraversedDescendantsDepth(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UnsupportedTraversedDescendantsDepth), arg0);

    public static string UnsupportedTraversedDescendantsDepth(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UnsupportedTraversedDescendantsDepth), culture, arg0);

    public static string MultipleAadTraversalExtensionsFound() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (MultipleAadTraversalExtensionsFound));

    public static string MultipleAadTraversalExtensionsFound(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (MultipleAadTraversalExtensionsFound), culture);

    public static string GraphSubjectForDescriptorNotFound(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphSubjectForDescriptorNotFound), arg0);

    public static string GraphSubjectForDescriptorNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphSubjectForDescriptorNotFound), culture, arg0);

    public static string FailedToRetrieveLocalDescendants(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (FailedToRetrieveLocalDescendants), arg0);

    public static string FailedToRetrieveLocalDescendants(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (FailedToRetrieveLocalDescendants), culture, arg0);

    public static string FailedToRetrieveStorageKeysInScope(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (FailedToRetrieveStorageKeysInScope), arg0);

    public static string FailedToRetrieveStorageKeysInScope(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (FailedToRetrieveStorageKeysInScope), culture, arg0);

    public static string UnsupportedMegaTenantStorageKeysInScopeRetrievalGraphException() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (UnsupportedMegaTenantStorageKeysInScopeRetrievalGraphException));

    public static string UnsupportedMegaTenantStorageKeysInScopeRetrievalGraphException(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (UnsupportedMegaTenantStorageKeysInScopeRetrievalGraphException), culture);
    }

    public static string GraphMemberForDescriptorNotFound(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberForDescriptorNotFound), arg0);

    public static string GraphMemberForDescriptorNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberForDescriptorNotFound), culture, arg0);

    public static string GetStorageKeysInScopeFeatureIsUnavailable() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GetStorageKeysInScopeFeatureIsUnavailable));

    public static string GetStorageKeysInScopeFeatureIsUnavailable(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GetStorageKeysInScopeFeatureIsUnavailable), culture);

    public static string FailedToRetrieveDescendants() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (FailedToRetrieveDescendants));

    public static string FailedToRetrieveDescendants(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (FailedToRetrieveDescendants), culture);

    public static string UnsupportedSubjectTypeForMembershipTraversal(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UnsupportedSubjectTypeForMembershipTraversal), arg0);

    public static string UnsupportedSubjectTypeForMembershipTraversal(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UnsupportedSubjectTypeForMembershipTraversal), culture, arg0);
    }

    public static string GraphMemberForCuidNotFound(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberForCuidNotFound), arg0);

    public static string GraphMemberForCuidNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberForCuidNotFound), culture, arg0);

    public static string UnsupportStorageKeyAssignment(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UnsupportStorageKeyAssignment), arg0, arg1);

    public static string UnsupportStorageKeyAssignment(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (UnsupportStorageKeyAssignment), culture, arg0, arg1);
    }

    public static string UnsupportedGraphTraversalDirection() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (UnsupportedGraphTraversalDirection));

    public static string UnsupportedGraphTraversalDirection(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (UnsupportedGraphTraversalDirection), culture);

    public static string GroupCannotBeModified() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GroupCannotBeModified));

    public static string GroupCannotBeModified(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GroupCannotBeModified), culture);

    public static string GraphAvatarNotFound(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphAvatarNotFound), arg0);

    public static string GraphAvatarNotFound(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphAvatarNotFound), culture, arg0);

    public static string GraphAvatarTooBig() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphAvatarTooBig));

    public static string GraphAvatarTooBig(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphAvatarTooBig), culture);

    public static string GraphAvatarUnsupportedSubject() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphAvatarUnsupportedSubject));

    public static string GraphAvatarUnsupportedSubject(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphAvatarUnsupportedSubject), culture);

    public static string GraphServicePrincipalMappingToServicePrincipalsAllowedOnly() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMappingToServicePrincipalsAllowedOnly));

    public static string GraphServicePrincipalMappingToServicePrincipalsAllowedOnly(
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphServicePrincipalMappingToServicePrincipalsAllowedOnly), culture);
    }

    public static string GraphUserServicePrincipalsMappingNotAllowed() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphUserServicePrincipalsMappingNotAllowed));

    public static string GraphUserServicePrincipalsMappingNotAllowed(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphUserServicePrincipalsMappingNotAllowed), culture);

    public static string GraphAadGetAccessTokenFailed() => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphAadGetAccessTokenFailed));

    public static string GraphAadGetAccessTokenFailed(CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Get(nameof (GraphAadGetAccessTokenFailed), culture);

    public static string GraphMemberSubjectTypeIsNotAllowed(object arg0) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberSubjectTypeIsNotAllowed), arg0);

    public static string GraphMemberSubjectTypeIsNotAllowed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberSubjectTypeIsNotAllowed), culture, arg0);

    public static string GraphMemberIncorrectSubjectType(object arg0, object arg1) => Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberIncorrectSubjectType), arg0, arg1);

    public static string GraphMemberIncorrectSubjectType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Graph.Resources.Format(nameof (GraphMemberIncorrectSubjectType), culture, arg0, arg1);
    }
  }
}
