// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.CustomSearchFieldValue
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

namespace Microsoft.AzureAd.Icm.Types
{
  public enum CustomSearchFieldValue
  {
    Invalid = 0,
    IncidentTitle = 1,
    MinIncidentsField = 1,
    Environment = 2,
    Created = 3,
    AssignedTo = 4,
    DeviceName = 5,
    DeviceGroup = 6,
    Team = 7,
    Severity = 8,
    DataCenter = 9,
    SlaImpacting = 10, // 0x0000000A
    SecurityRisk = 11, // 0x0000000B
    Noise = 12, // 0x0000000C
    IncidentId = 13, // 0x0000000D
    ResolvedBy = 14, // 0x0000000E
    RootCauseTitle = 15, // 0x0000000F
    BugNumber = 16, // 0x00000010
    IncidentStatus = 17, // 0x00000011
    Acknowledged = 18, // 0x00000012
    RootCauseOption = 19, // 0x00000013
    ResolvedDate = 20, // 0x00000014
    RootCauseNeedsInvestigation = 21, // 0x00000015
    Keywords = 22, // 0x00000016
    TeamTrackingList = 23, // 0x00000017
    MonitorId = 24, // 0x00000018
    SubscriptionId = 25, // 0x00000019
    SupportTicketId = 26, // 0x0000001A
    IncidentType = 27, // 0x0000001B
    IncidentSubType = 28, // 0x0000001C
    HowFixed = 29, // 0x0000001D
    Origin = 30, // 0x0000001E
    TenantResponsible = 31, // 0x0000001F
    TsgId = 32, // 0x00000020
    ImpactedTenants = 33, // 0x00000021
    ImpactedTeams = 34, // 0x00000022
    SourceCreated = 35, // 0x00000023
    CustomerName = 36, // 0x00000024
    ServiceInstanceId = 37, // 0x00000025
    CreatedBy = 38, // 0x00000026
    LastModifiedBy = 39, // 0x00000027
    SiloId = 40, // 0x00000028
    MaxIncidentsField = 50, // 0x00000032
    OwningTenant = 51, // 0x00000033
    OriginatingTenant = 52, // 0x00000034
    EverOwnedTenant = 53, // 0x00000035
    MaxIncidentAssociations = 100, // 0x00000064
    Tenant = 101, // 0x00000065
    Enabled = 102, // 0x00000066
    RoutingId = 103, // 0x00000067
    CorrelationId = 104, // 0x00000068
    RaisingEnvironment = 105, // 0x00000069
    RaisingDataCenter = 106, // 0x0000006A
    RaisingDeviceGroup = 107, // 0x0000006B
    RaisingDeviceName = 108, // 0x0000006C
    SuppressedIncidentStatus = 109, // 0x0000006D
    Modified = 110, // 0x0000006E
    StartDate = 111, // 0x0000006F
    EndDate = 112, // 0x00000070
    RuleId = 113, // 0x00000071
    Description = 114, // 0x00000072
    RuleEnvironment = 115, // 0x00000073
    RuleDataCenter = 116, // 0x00000074
    OriginatingAlertSource = 117, // 0x00000075
    OriginatingAlertSourceType = 118, // 0x00000076
    RuleModifiedBy = 119, // 0x00000077
    RuleCategoryId = 122, // 0x0000007A
    RuleSiloId = 123, // 0x0000007B
    RuleSeverity = 124, // 0x0000007C
    MaxSuppressionRuleAssociation = 200, // 0x000000C8
    PirTitle = 201, // 0x000000C9
    PirOwningTenant = 202, // 0x000000CA
    PirSeverity = 203, // 0x000000CB
    PirOwner = 204, // 0x000000CC
    PirOwningTeam = 205, // 0x000000CD
    PirStatus = 206, // 0x000000CE
    PirCreatedDate = 207, // 0x000000CF
    PirModifiedDate = 208, // 0x000000D0
    PirImpactStartDate = 209, // 0x000000D1
    PirDetectionTime = 210, // 0x000000D2
    CustomFieldStartIndex = 10000, // 0x00002710
  }
}
