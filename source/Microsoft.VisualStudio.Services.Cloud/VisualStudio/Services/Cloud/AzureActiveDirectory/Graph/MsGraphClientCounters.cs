// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphClientCounters
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Aad;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  internal static class MsGraphClientCounters
  {
    internal static readonly IAadPerfCounter All = MsGraphClientCounters.GetCounter(nameof (All));
    internal static readonly IAadPerfCounter GetAncestorIds = MsGraphClientCounters.GetCounter(nameof (GetAncestorIds));
    internal static readonly IAadPerfCounter GetAncestors = MsGraphClientCounters.GetCounter(nameof (GetAncestors));
    internal static readonly IAadPerfCounter GetDescendants = MsGraphClientCounters.GetCounter(nameof (GetDescendants));
    internal static readonly IAadPerfCounter GetDescendantIds = MsGraphClientCounters.GetCounter(nameof (GetDescendantIds));
    internal static readonly IAadPerfCounter GetGroups = MsGraphClientCounters.GetCounter(nameof (GetGroups));
    internal static readonly IAadPerfCounter GetGroupsWithIds = MsGraphClientCounters.GetCounter(nameof (GetGroupsWithIds));
    internal static readonly IAadPerfCounter GetTenant = MsGraphClientCounters.GetCounter(nameof (GetTenant));
    internal static readonly IAadPerfCounter GetResourceTenants = MsGraphClientCounters.GetCounter(nameof (GetResourceTenants));
    internal static readonly IAadPerfCounter GetUsers = MsGraphClientCounters.GetCounter(nameof (GetUsers));
    internal static readonly IAadPerfCounter GetUsersWithIds = MsGraphClientCounters.GetCounter(nameof (GetUsersWithIds));
    internal static readonly IAadPerfCounter GetUserStatusWithId = MsGraphClientCounters.GetCounter(nameof (GetUserStatusWithId));
    internal static readonly IAadPerfCounter GetUserThumbnail = MsGraphClientCounters.GetCounter(nameof (GetUserThumbnail));
    internal static readonly IAadPerfCounter GetServicePrincipalsByIds = MsGraphClientCounters.GetCounter(nameof (GetServicePrincipalsByIds));
    internal static readonly IAadPerfCounter GetServicePrincipals = MsGraphClientCounters.GetCounter(nameof (GetServicePrincipals));
    internal static readonly IAadPerfCounter GetDirectoryRoles = MsGraphClientCounters.GetCounter(nameof (GetDirectoryRoles));
    internal static readonly IAadPerfCounter GetDirectoryRoleMembers = MsGraphClientCounters.GetCounter(nameof (GetDirectoryRoleMembers));
    internal static readonly IAadPerfCounter GetDirectoryRolesWithIds = MsGraphClientCounters.GetCounter(nameof (GetDirectoryRolesWithIds));
    internal static readonly IAadPerfCounter GetSoftDeletedObjects = MsGraphClientCounters.GetCounter(nameof (GetSoftDeletedObjects));
    internal static readonly IAadPerfCounter GetUserRolesAndGroups = MsGraphClientCounters.GetCounter(nameof (GetUserRolesAndGroups));
    internal static readonly IAadPerfCounter GetApplicationById = MsGraphClientCounters.GetCounter(nameof (GetApplicationById));
    internal static readonly IAadPerfCounter CreateApplication = MsGraphClientCounters.GetCounter(nameof (CreateApplication));
    internal static readonly IAadPerfCounter UpdateApplication = MsGraphClientCounters.GetCounter(nameof (UpdateApplication));
    internal static readonly IAadPerfCounter DeleteApplication = MsGraphClientCounters.GetCounter(nameof (DeleteApplication));
    internal static readonly IAadPerfCounter CreateServicePrincipal = MsGraphClientCounters.GetCounter(nameof (CreateServicePrincipal));
    internal static readonly IAadPerfCounter DeleteServicePrincipal = MsGraphClientCounters.GetCounter(nameof (DeleteServicePrincipal));
    internal static readonly IAadPerfCounter AddApplicationPassword = MsGraphClientCounters.GetCounter(nameof (AddApplicationPassword));
    internal static readonly IAadPerfCounter AddApplicationFederatedCredentials = MsGraphClientCounters.GetCounter(nameof (AddApplicationFederatedCredentials));
    internal static readonly IAadPerfCounter RemoveApplicationFederatedCredentials = MsGraphClientCounters.GetCounter(nameof (RemoveApplicationFederatedCredentials));
    internal static readonly IAadPerfCounter RemoveApplicationPassword = MsGraphClientCounters.GetCounter(nameof (RemoveApplicationPassword));
    internal static readonly IAadPerfCounter GetOrganizationDataBoundary = MsGraphClientCounters.GetCounter(nameof (GetOrganizationDataBoundary));
    internal static readonly IAadPerfCounter GetProfileData = MsGraphClientCounters.GetCounter(nameof (GetProfileData));

    private static IAadPerfCounter GetCounter(string operation) => (IAadPerfCounter) AadPerfCounter.GetCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.MsGraphClient", operation);
  }
}
