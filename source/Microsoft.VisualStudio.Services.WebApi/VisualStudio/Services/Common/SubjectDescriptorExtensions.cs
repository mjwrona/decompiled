// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SubjectDescriptorExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Graph;
using System;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class SubjectDescriptorExtensions
  {
    internal static Guid GetMasterScopeId(this SubjectDescriptor subjectDescriptor)
    {
      if (!subjectDescriptor.IsGroupScopeType())
        throw new InvalidSubjectTypeException(subjectDescriptor.SubjectType);
      Guid result;
      if (!Guid.TryParse(subjectDescriptor.Identifier, out result))
        throw new ArgumentException("Parameter subjectDescriptor does not have a valid master scope ID");
      return result;
    }

    internal static Guid GetCuid(this SubjectDescriptor subjectDescriptor)
    {
      if (!subjectDescriptor.IsCuidBased())
        throw new InvalidSubjectTypeException(subjectDescriptor.SubjectType);
      Guid result;
      if (!Guid.TryParse(subjectDescriptor.Identifier, out result))
        throw new ArgumentException("Parameter subjectDescriptor does not have a valid CUID");
      return result;
    }

    public static bool IsWindowsType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "win");

    public static bool IsGroupType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsAadGroupType() || subjectDescriptor.IsVstsGroupType();

    public static bool IsAadGroupType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "aadgp");

    public static bool IsVstsGroupType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "vssgp");

    public static bool IsClaimsUserType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsAadUserType() || subjectDescriptor.IsMsaUserType() || subjectDescriptor.IsAadServicePrincipalType();

    public static bool IsAadUserType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "aad");

    public static bool IsMsaUserType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "msa");

    public static bool IsBindPendingUserType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "bnd");

    public static bool IsUnauthenticatedIdentityType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "uauth");

    public static bool IsServiceIdentityType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "svc");

    public static bool IsAggregateIdentityType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "agg");

    public static bool IsImportedIdentityType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "imp");

    public static bool IsGroupScopeType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "scp");

    public static bool IsServerTestIdentityType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "tst");

    public static bool IsAadServicePrincipalType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "aadsp");

    public static bool IsSystemServicePrincipalType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "s2s");

    public static bool IsSystemScopeType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "sscp");

    public static bool IsSystemCspPartnerType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "scsp");

    public static bool IsSystemLicenseType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "slic");

    public static bool IsSystemPublicAccessType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "spa");

    public static bool IsSystemAccessControlType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "sace");

    public static bool IsSystemType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsSystemServicePrincipalType() || subjectDescriptor.IsSystemScopeType() || subjectDescriptor.IsSystemLicenseType() || subjectDescriptor.IsSystemCspPartnerType() || subjectDescriptor.IsSystemPublicAccessType() || subjectDescriptor.IsSystemAccessControlType();

    public static bool IsSubjectStoreType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsSystemServicePrincipalType() || subjectDescriptor.IsSystemScopeType() || subjectDescriptor.IsSystemLicenseType() || subjectDescriptor.IsSystemCspPartnerType();

    public static bool IsCspPartnerIdentityType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "csp");

    public static bool IsUnknownSubjectType(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "ukn") || StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "ungrp") || StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "unusr");

    public static bool IsCuidBased(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsClaimsUserType() || subjectDescriptor.IsCspPartnerIdentityType();

    public static bool IsUserType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsClaimsUserType() || subjectDescriptor.IsCspPartnerIdentityType() || subjectDescriptor.IsBindPendingUserType() || subjectDescriptor.IsServiceIdentityType();

    public static bool IsPubliclyAvailableGraphSubjectType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor == new SubjectDescriptor() || subjectDescriptor.IsUserType() || subjectDescriptor.IsGroupType() || subjectDescriptor.IsGroupScopeType();
  }
}
