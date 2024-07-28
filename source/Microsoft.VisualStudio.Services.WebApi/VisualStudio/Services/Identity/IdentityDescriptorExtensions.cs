// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityDescriptorExtensions
  {
    public static bool IsTeamFoundationType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.TeamFoundation.Identity");

    public static bool IsWindowsType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("System.Security.Principal.WindowsIdentity");

    public static bool IsUnknownIdentityType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.VisualStudio.Services.Identity.UnknownIdentity");

    public static bool IsSystemServicePrincipalType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("System:ServicePrincipal");

    public static bool IsClaimsIdentityType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.IdentityModel.Claims.ClaimsIdentity");

    public static bool IsAadServicePrincipalType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.VisualStudio.Services.Claims.AadServicePrincipal");

    public static bool IsImportedIdentityType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.TeamFoundation.ImportedIdentity");

    public static bool IsServiceIdentityType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.TeamFoundation.ServiceIdentity");

    public static bool IsBindPendingType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.TeamFoundation.BindPendingIdentity");

    public static bool IsAggregateIdentityType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.TeamFoundation.AggregateIdentity");

    public static bool IsUnauthenticatedIdentity(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.TeamFoundation.UnauthenticatedIdentity");

    public static bool IsSubjectStoreType(this IdentityDescriptor identityDescriptor) => (object) identityDescriptor.IdentityType == (object) "System:License" || (object) identityDescriptor.IdentityType == (object) "System:Scope" || (object) identityDescriptor.IdentityType == (object) "System:ServicePrincipal" || (object) identityDescriptor.IdentityType == (object) "System:WellKnownGroup" || (object) identityDescriptor.IdentityType == (object) "System:CspPartner";

    public static bool IsIdentityType(
      this IdentityDescriptor identityDescriptor,
      IEnumerable<string> identityTypes)
    {
      return identityTypes.Any<string>((Func<string, bool>) (id => StringComparer.OrdinalIgnoreCase.Equals(identityDescriptor.IdentityType, id)));
    }

    public static bool IsIdentityType(
      this IdentityDescriptor identityDescriptor,
      string identityType)
    {
      return StringComparer.OrdinalIgnoreCase.Equals(identityDescriptor.IdentityType, identityType);
    }

    public static bool IsCspPartnerIdentityType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.TeamFoundation.Claims.CspPartnerIdentity");

    public static bool IsGroupScopeType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.VisualStudio.Services.Graph.GraphScope");

    public static bool IsSystemLicenseType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("System:License");

    public static bool IsSystemScopeType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("System:Scope");

    public static bool IsSystemPublicAccessType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("System:PublicAccess");

    public static bool IsSystemAccessControlType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("System:AccessControl");

    public static bool IsServerTestIdentityType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("Microsoft.VisualStudio.Services.Identity.ServerTestIdentity");

    public static bool IsSystemCspPartnerType(this IdentityDescriptor identityDescriptor) => identityDescriptor.IsIdentityType("System:CspPartner");
  }
}
