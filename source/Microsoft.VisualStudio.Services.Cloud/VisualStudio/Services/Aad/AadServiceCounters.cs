// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServiceCounters
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Aad
{
  internal static class AadServiceCounters
  {
    internal static readonly IAadPerfCounter All = AadServiceCounters.GetCounter(nameof (All));
    internal static readonly IAadPerfCounter GetAncestorIds = AadServiceCounters.GetCounter(nameof (GetAncestorIds));
    internal static readonly IAadPerfCounter GetAncestors = AadServiceCounters.GetCounter(nameof (GetAncestors));
    internal static readonly IAadPerfCounter GetDescendantIds = AadServiceCounters.GetCounter(nameof (GetDescendantIds));
    internal static readonly IAadPerfCounter GetDescendants = AadServiceCounters.GetCounter(nameof (GetDescendants));
    internal static readonly IAadPerfCounter GetGroups = AadServiceCounters.GetCounter(nameof (GetGroups));
    internal static readonly IAadPerfCounter GetGroupsWithIds = AadServiceCounters.GetCounter(nameof (GetGroupsWithIds));
    internal static readonly IAadPerfCounter GetTenant = AadServiceCounters.GetCounter(nameof (GetTenant));
    internal static readonly IAadPerfCounter GetTenants = AadServiceCounters.GetCounter(nameof (GetTenants));
    internal static readonly IAadPerfCounter GetUsers = AadServiceCounters.GetCounter(nameof (GetUsers));
    internal static readonly IAadPerfCounter GetUsersWithIds = AadServiceCounters.GetCounter(nameof (GetUsersWithIds));
    internal static readonly IAadPerfCounter GetUserStatusWithId = AadServiceCounters.GetCounter(nameof (GetUserStatusWithId));
    internal static readonly IAadPerfCounter GetServicePrincipalsByIds = AadServiceCounters.GetCounter(nameof (GetServicePrincipalsByIds));
    internal static readonly IAadPerfCounter GetServicePrincipals = AadServiceCounters.GetCounter(nameof (GetServicePrincipals));
    internal static readonly IAadPerfCounter GetUserRolesAndGroups = AadServiceCounters.GetCounter(nameof (GetUserRolesAndGroups));
    internal static readonly IAadPerfCounter GetUserThumbnail = AadServiceCounters.GetCounter(nameof (GetUserThumbnail));
    internal static readonly IAadPerfCounter GetDirectoryRoles = AadServiceCounters.GetCounter(nameof (GetDirectoryRoles));
    internal static readonly IAadPerfCounter GetDirectoryRoleMembers = AadServiceCounters.GetCounter(nameof (GetDirectoryRoleMembers));
    internal static readonly IAadPerfCounter GetDirectoryRolesWithIds = AadServiceCounters.GetCounter(nameof (GetDirectoryRolesWithIds));
    internal static readonly IAadPerfCounter GetSoftDeletedObjects = AadServiceCounters.GetCounter(nameof (GetSoftDeletedObjects));
    internal static readonly IAadPerfCounter GetApplicationById = AadServiceCounters.GetCounter(nameof (GetApplicationById));
    internal static readonly IAadPerfCounter CreateApplication = AadServiceCounters.GetCounter(nameof (CreateApplication));
    internal static readonly IAadPerfCounter UpdateApplication = AadServiceCounters.GetCounter(nameof (UpdateApplication));
    internal static readonly IAadPerfCounter DeleteApplication = AadServiceCounters.GetCounter(nameof (DeleteApplication));
    internal static readonly IAadPerfCounter CreateServicePrincipal = AadServiceCounters.GetCounter(nameof (CreateServicePrincipal));
    internal static readonly IAadPerfCounter DeleteServicePrincipal = AadServiceCounters.GetCounter(nameof (DeleteServicePrincipal));
    internal static readonly IAadPerfCounter AddApplicationPassword = AadServiceCounters.GetCounter(nameof (AddApplicationPassword));
    internal static readonly IAadPerfCounter AddApplicationFederatedCredentials = AadServiceCounters.GetCounter(nameof (AddApplicationFederatedCredentials));
    internal static readonly IAadPerfCounter RemoveApplicationFederatedCredentials = AadServiceCounters.GetCounter(nameof (RemoveApplicationFederatedCredentials));
    internal static readonly IAadPerfCounter RemoveApplicationPassword = AadServiceCounters.GetCounter(nameof (RemoveApplicationPassword));
    internal static readonly IAadPerfCounter GetOrganizationDataBoundary = AadServiceCounters.GetCounter(nameof (GetOrganizationDataBoundary));
    internal static readonly IAadPerfCounter GetProfileData = AadServiceCounters.GetCounter(nameof (GetProfileData));

    private static IAadPerfCounter GetCounter(string operation) => (IAadPerfCounter) AadPerfCounter.GetCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadService", operation);
  }
}
