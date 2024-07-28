// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.AadGraphClientCounters
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  internal static class AadGraphClientCounters
  {
    internal static readonly IAadPerfCounter All = AadGraphClientCounters.GetCounter(nameof (All));
    internal static readonly IAadPerfCounter GetAncestorIds = AadGraphClientCounters.GetCounter(nameof (GetAncestorIds));
    internal static readonly IAadPerfCounter GetAncestors = AadGraphClientCounters.GetCounter(nameof (GetAncestors));
    internal static readonly IAadPerfCounter GetDescendants = AadGraphClientCounters.GetCounter(nameof (GetDescendants));
    internal static readonly IAadPerfCounter GetGroups = AadGraphClientCounters.GetCounter(nameof (GetGroups));
    internal static readonly IAadPerfCounter GetGroupsWithIds = AadGraphClientCounters.GetCounter(nameof (GetGroupsWithIds));
    internal static readonly IAadPerfCounter GetTenant = AadGraphClientCounters.GetCounter(nameof (GetTenant));
    internal static readonly IAadPerfCounter GetTenantsByAltSecId = AadGraphClientCounters.GetCounter(nameof (GetTenantsByAltSecId));
    internal static readonly IAadPerfCounter GetTenantsByKey = AadGraphClientCounters.GetCounter(nameof (GetTenantsByKey));
    internal static readonly IAadPerfCounter GetUsers = AadGraphClientCounters.GetCounter(nameof (GetUsers));
    internal static readonly IAadPerfCounter GetUsersWithIds = AadGraphClientCounters.GetCounter(nameof (GetUsersWithIds));

    private static IAadPerfCounter GetCounter(string operation) => (IAadPerfCounter) AadPerfCounter.GetCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadGraphClient", operation);
  }
}
