// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.AnalyticsStagingCustomTracePoints
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public enum AnalyticsStagingCustomTracePoints
  {
    WorkItemFieldNamesLoaded = 14010001, // 0x00D5C691
    FieldSkipped = 14010002, // 0x00D5C692
    EventSubscriberFailed = 14010003, // 0x00D5C693
    TeamProjectGuidCouldNotBeResolved = 14010004, // 0x00D5C694
    AreaGuidCouldNotBeResolved = 14010005, // 0x00D5C695
    IterationGuidCouldNotBeResolved = 14010006, // 0x00D5C696
    CouldNotGetFieldValues = 14010007, // 0x00D5C697
    InvalidFieldNameDetected = 14010008, // 0x00D5C698
    InvalidateShard = 14010009, // 0x00D5C699
    PersonGuidCouldNotBeResolved = 14010010, // 0x00D5C69A
    ProjectProcessSettingsNotConfigured = 14010011, // 0x00D5C69B
    GetWorkItemFieldValuesProbeFailed = 14010012, // 0x00D5C69C
    GetFieldEntries_Enter = 14010013, // 0x00D5C69D
    GetFieldEntries_Leave = 14010014, // 0x00D5C69E
    GetChangedRevisions_Enter = 14010015, // 0x00D5C69F
    GetChangedRevisions_Leave = 14010016, // 0x00D5C6A0
    GetWorkItemFieldValues_Enter = 14010017, // 0x00D5C6A1
    GetWorkItemFieldValues_Leave = 14010018, // 0x00D5C6A2
    FieldsSnapshotRefreshed = 14010019, // 0x00D5C6A3
    FieldEntryNotFound = 14010020, // 0x00D5C6A4
    CreateRecords_Enter = 14010021, // 0x00D5C6A5
    CreateRecords_Leave = 14010022, // 0x00D5C6A6
    ResolveClassificationNodes_Enter = 14010023, // 0x00D5C6A7
    ResolveClassificationNodes_Leave = 14010024, // 0x00D5C6A8
    PersonGuidResolver_Resolve_Enter = 14010025, // 0x00D5C6A9
    PersonGuidResolver_Resolve_Leave = 14010026, // 0x00D5C6AA
    CreateRecord_GetProjectGuid_Enter = 14010027, // 0x00D5C6AB
    CreateRecord_GetProjectGuid_Leave = 14010028, // 0x00D5C6AC
    WorkItemLinksAnalyticsJob_CreatedByIdCoundNotBeResolved = 14010029, // 0x00D5C6AD
    WorkItemLinksAnalyticsJob_DeletedByIdCoundNotBeResolved = 14010030, // 0x00D5C6AE
    MissingJobDefinition = 14010031, // 0x00D5C6AF
    CouldNotGetProcessFromProject = 14010032, // 0x00D5C6B0
    MissingRevisionInFieldValues = 14010033, // 0x00D5C6B1
    MissingProcess = 14010034, // 0x00D5C6B2
    InvalidWatermark = 14010035, // 0x00D5C6B3
    ProcessJobDetails = 14010036, // 0x00D5C6B4
    TeamSettingsInvalidateTable = 14010037, // 0x00D5C6B5
    ProcessInvalidateTable = 14010038, // 0x00D5C6B6
    SkippedChangedNode = 14010039, // 0x00D5C6B7
    GitHubServiceConnectionRepositoryRefShardIdLocator = 14010040, // 0x00D5C6B8
    GitHubRateLimit = 14010041, // 0x00D5C6B9
    GitHubJob = 14010042, // 0x00D5C6BA
    GitHubRequest = 14010043, // 0x00D5C6BB
    LimitedConcurrencyAnalyticsJobCommandLimit = 14010044, // 0x00D5C6BC
    GitHubRequestBackoff = 14010045, // 0x00D5C6BD
    PipelineJobCommitAnalyticsJobStart = 14010046, // 0x00D5C6BE
    PipelineJobCommitAnalyticsJobEnd = 14010055, // 0x00D5C6C7
    WorkItemRevisionGetCounts = 14010056, // 0x00D5C6C8
    WorkItemRevisionWitHistoryCounts = 14010057, // 0x00D5C6C9
  }
}
