// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.CustomSearchDomainContext
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

namespace Microsoft.AzureAd.Icm.Types
{
  public enum CustomSearchDomainContext
  {
    Invalid = -1, // 0xFFFFFFFF
    Incidents = 0,
    Pirs = 1,
    IncidentTenants = 2,
    SuppressedIncidents = 3,
    SuppressionRules = 4,
    RoutingRules = 5,
    CorrelationRules = 6,
    AutoPostRules = 7,
    PirsCustomFields = 8,
  }
}
