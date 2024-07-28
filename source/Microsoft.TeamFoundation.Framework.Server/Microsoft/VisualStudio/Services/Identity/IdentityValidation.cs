// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityValidation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityValidation
  {
    internal static void CheckDescriptor(IdentityDescriptor descriptor, string parameterName)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, parameterName);
      if (descriptor.IsUnknownIdentityType())
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), parameterName + ".IdentityType");
      if (string.IsNullOrEmpty(descriptor.Identifier))
        throw new ArgumentException(TFCommonResources.EmptyStringNotAllowed(), parameterName + ".Identifier");
    }

    internal static bool IsNonEmptyDescriptor(IdentityDescriptor descriptor) => !(descriptor == (IdentityDescriptor) null) && !(descriptor.IdentityType == "Microsoft.VisualStudio.Services.Identity.UnknownIdentity") && !string.IsNullOrEmpty(descriptor.Identifier);

    internal static void CheckTeamFoundationType(IdentityDescriptor descriptor)
    {
      if (!IdentityValidation.IsTeamFoundationType(descriptor))
        throw new NotApplicationGroupException();
    }

    internal static bool IsTeamFoundationType(IdentityDescriptor descriptor) => !(descriptor == (IdentityDescriptor) null) && descriptor.IdentityType != null && descriptor.IsTeamFoundationType() && descriptor.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase);

    internal static bool IsBindPendingType(IdentityDescriptor descriptor) => string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase);

    internal static bool IsUnauthenticatedType(IdentityDescriptor descriptor) => string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.UnauthenticatedIdentity", StringComparison.OrdinalIgnoreCase);

    internal static void CheckFactorAndValue(IdentitySearchFilter factor, ref string factorValue)
    {
      switch (factor)
      {
        case IdentitySearchFilter.AccountName:
          IdentityValidation.CheckAccountName(ref factorValue);
          break;
        case IdentitySearchFilter.DisplayName:
          TFCommonUtil.CheckDisplayName(ref factorValue);
          break;
        case IdentitySearchFilter.AdministratorsGroup:
          TFCommonUtil.CheckProjectUri(ref factorValue, false);
          break;
        case IdentitySearchFilter.Identifier:
          IdentityValidation.CheckIdentifier(factorValue);
          break;
        case IdentitySearchFilter.MailAddress:
          IdentityValidation.CheckMailAddress(factorValue);
          break;
        case IdentitySearchFilter.General:
          IdentityValidation.CheckGeneral(factorValue);
          break;
        case IdentitySearchFilter.Alias:
          IdentityValidation.CheckAlias(ref factorValue);
          break;
        case IdentitySearchFilter.DirectoryAlias:
          IdentityValidation.CheckDirectoryAlias(factorValue);
          break;
        case IdentitySearchFilter.LocalGroupName:
          IdentityValidation.CheckGroupName(ref factorValue);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (factor));
      }
    }

    internal static void CheckMailAddress(string mailAddress) => ArgumentUtility.CheckStringForNullOrEmpty(mailAddress, nameof (mailAddress));

    internal static void CheckGeneral(string generalSearchValue) => ArgumentUtility.CheckStringForNullOrEmpty(generalSearchValue, nameof (generalSearchValue));

    internal static void CheckGroupName(ref string groupName) => IdentityValidation.CheckAccountName(ref groupName);

    internal static void CheckAccountName(ref string accountName)
    {
      ArgumentUtility.CheckForNull<string>(accountName, nameof (accountName));
      accountName = accountName.Trim();
      if (accountName.Length == 0 || ArgumentUtility.IsInvalidString(accountName, false))
        throw new ArgumentException(TFCommonResources.BAD_ACCOUNT_NAME((object) accountName));
      if (!TFCommonUtil.IsLegalIdentity(accountName))
        throw new IllegalIdentityException(accountName);
    }

    internal static void CheckAlias(ref string alias)
    {
      ArgumentUtility.CheckForNull<string>(alias, nameof (alias));
      alias = alias.Trim();
      if (alias.Length == 0 || alias.Length > 256 || ArgumentUtility.IsInvalidString(alias, false))
        throw new IllegalAliasException(TFCommonResources.BAD_ALIAS((object) alias));
      if (!TFCommonUtil.IsAlphaNumString(alias))
        throw new IllegalAliasException(TFCommonResources.BAD_ALIAS_NOT_ALPHANUM((object) alias));
    }

    internal static void CheckDirectoryAlias(string directoryAlias)
    {
      ArgumentUtility.CheckForNull<string>(directoryAlias, nameof (directoryAlias));
      directoryAlias = directoryAlias.Trim();
      if (directoryAlias.Length == 0 || ArgumentUtility.IsInvalidString(directoryAlias))
        throw new ArgumentException(TFCommonResources.BAD_DIRECTORY_ALIAS((object) directoryAlias));
      if (!TFCommonUtil.IsLegalIdentity(directoryAlias))
        throw new IllegalIdentityException(directoryAlias);
    }

    internal static void CheckIdentifier(string identifier) => ArgumentUtility.CheckStringForNullOrEmpty(identifier, nameof (identifier));
  }
}
