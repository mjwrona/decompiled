// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ServiceInterfaces
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  public static class ServiceInterfaces
  {
    public const string AdministrationService = "AdministrationService";
    public const string AccessControlService = "AccessControlService";
    public const string CatalogService = "CatalogService";
    public const string EventService = "Eventing";
    public const string JobService = "JobService";
    public const string LocationService = "LocationService";
    public const string RegistryService = "RegistryService";
    public const string SecurityService = "SecurityService";
    public const string SecurityService2 = "SecurityService2";
    public const string FileHandlerService = "FileHandlerService";
    public const string PropertyService = "PropertyService";
    public const string MessageQueueService = "MessageQueueService";
    public const string MessageQueueService2 = "MessageQueueService2";
    public const string IdentityService = "IdentityService";
    public const string IdentityManagementService = "IdentityManagementService";
    public const string IdentityManagementService2 = "IdentityManagementService2";
    public const string IdentityManagementWeb = "IdentityManagementWeb";
    public const string SecurityManagementWeb = "SecurityManagementWeb";
    public const string TeamProjectCollectionService = "TeamProjectCollectionService";
    public const string UtilizationUsageSummary = "UtilizationUsageSummary";
    public const string UtilizationUserUsageSummary = "UtilizationUserUsageSummary";
    public const string VersionControl = "IRepository";
    public const string WorkItem = "WorkitemService";
    public const string WorkItem2 = "WorkitemService2";
    public const string WorkItem3 = "WorkitemService3";
    public const string WorkItem4 = "WorkitemService4";
    public const string WorkItem5 = "WorkitemService5";
    public const string WorkItem6 = "WorkitemService6";
    public const string WorkItem7 = "WorkitemService7";
    public const string WorkItem8 = "WorkitemService8";
    public const string WorkItemAttachmentHandler = "WorkitemAttachmentHandler";
    public const string DataSourceServer = "DataSourceServer";
    public const string MethodologyUpload = "MethodologyUpload";
    public const string Methodology = "Methodology";
    public const string TestResultsService = "TestResultsService";
    public const string ProcessConfigurationService = "ProcessConfigurationService";
    public const string ProcessConfigurationService2 = "ProcessConfigurationService2";
    public const string ProcessConfigurationService3 = "ProcessConfigurationService3";
    public const string ProcessConfigurationService4 = "ProcessConfigurationService4";
    public const string TeamConfigurationService = "TeamConfigurationService";
    public const string AreasManagementWeb = "AreasManagementWeb";
    public const string IterationsManagementWeb = "IterationsManagementWeb";
    public const string ProjectAlertsWeb = "ProjectAlertsWeb";
    public const string ProjectTaskBoardWeb = "ProjectTaskBoardWeb";
    public const string NewQueryInWeb = "NewQueryInWeb";
    public const string OpenQueryInWeb = "OpenQueryInWeb";
    public const string OpenTempQueryInWeb = "OpenTempQueryInWeb";
    public const string NewWorkItem = "NewWorkItem";
    public const string OpenWorkItem = "OpenWorkItem";
    public const string OpenWorkItemWithProjectContext = "OpenWorkItemWithProjectContext";
    public const string OpenWorkItemWithQueryId = "OpenWorkItemWithQueryId";
    public const string OpenWorkItemWithQueryContext = "OpenWorkItemWithQueryContext";
    public const string OpenPTMInWeb = "OpenPTMInWeb";
    public const string WorkItemsHub = "WorkItemsHub";
    public const string SearchInWeb = "SearchInWeb";
    public const string CommitDetailsWeb = "CommitDetailsWeb";
    public const string GitRefDetailsWeb = "GitRefDetailsWeb";
    public const string CollectionRoomsHubWeb = "CollectionRoomsHubWeb";
    public const string NewTeamProjectWeb = "NewTeamProjectWeb";
    public const string MeteringService = "IOfferSubscriptionService";
    public const string PullRequestLandingWeb = "PullRequestLandingWeb";
    public const string PullRequestCreateWeb = "PullRequestCreateWeb";
    public const string PullRequestDetailsWeb = "PullRequestDetailsWeb";
    public const string GitRepoWeb = "GitRepoWeb";
    public const string ProjectWeb = "ProjectWeb";
    public const string GitRepoClone = "GitRepoClone";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string SignOutWeb = "SignOutWeb";
    public const string TestResultsServiceEx = "TestResultsServiceEx";
    public const string TestManagementWebService = "TestManagementWebService";
    public const string TestManagementWebService2 = "TestManagementWebService2";
    public const string TestManagementWebService3 = "TestManagementWebService3";
    public const string TestManagementWebService4 = "TestManagementWebService4";
    public const string TestManagementWebAccessService = "TestManagementWebAccessService";
    public const string SharedParameterDataSetsService = "SharedParameterDataSetsService";
    public const string TestManagementAttachmentDownloadHandler = "AttachmentDownload";
    public const string TestImpactService = "TestImpactService";
    public const string ServerStatus = "ServerStatus";
    public const string DiscussionWebService = "DiscussionWebService";
    public const string StrongBoxService = "StrongBoxService";
    public const string StrongBoxFileUpload = "StrongBoxHttpUploadHandler";
    public const string StrongBoxFileDownload = "StrongBoxHttpDownloadHandler";
    public const string SyncService = "SyncService";
    public const string SyncService4 = "SyncService4";
    public const string ConnectedServicesService = "ConnectedServicesService";
    public const string FileContainersResource = "FileContainersResource";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string RegistrationService = "RegistrationService";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string CommonStructure = "CommonStructure";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string CommonStructure3 = "CommonStructure3";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string CommonStructure4 = "CommonStructure4";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string GroupSecurity = "GroupSecurity";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string GroupSecurity2 = "GroupSecurity2";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string Authorization = "Authorization";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProcessTemplate = "ProcessTemplate";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProjectMaintenance = "IProjectMaintenance";
    public const string TswaHome = "TSWAHome";
    public const string TswaWorkItemEditor = "WorkItemEditor";
    public const string TswaChangesetDetail = "ChangesetDetail";
    public const string TswaDifference = "Difference";
    public const string TswaViewItem = "ViewItem";
    public const string ProjectServerPwaSite = "ProjectServerPwaSite";
    public const string ProjectServerSspSite = "ProjectServerSspSite";
    public const string TswaShelvesetDetail = "ShelvesetDetail";
    public const string TswaQueryResults = "QueryResults";
    public const string TswaAnnotate = "Annotate";
    public const string TswaSourceExplorer = "SourceExplorer";
    public const string TswaOpenWorkItem = "OpenWorkItem";
    public const string TswaCreateWorkItem = "CreateWorkItem";
    public const string TswaViewServerQueryResults = "ViewServerQueryResults";
    public const string TswaViewWiqlQueryResults = "ViewWiqlQueryResults";
    public const string TswaExploreSourceControlPath = "ExploreSourceControlPath";
    public const string TswaFindShelveset = "FindShelveset";
    public const string TswaViewShelvesetDetails = "ViewShelvesetDetails";
    public const string TswaFindChangeset = "FindChangeset";
    public const string TswaViewChangesetDetails = "ViewChangesetDetails";
    public const string TswaViewSourceControlItem = "ViewSourceControlItem";
    public const string TswaDownloadSourceControlItem = "DownloadSourceControlItem";
    public const string TswaDiffSourceControlItems = "DiffSourceControlItems";
    public const string TswaAnnotateSourceControlItem = "AnnotateSourceControlItem";
    public const string TswaViewSourceControlItemHistory = "ViewSourceControlItemHistory";
    public const string TswaViewBuildDetails = "ViewBuildDetails";
    public const string TswaViewSourceControlShelvedItem = "ViewSourceControlShelvedItem";
    public const string TswaDiffSourceControlShelvedItem = "DiffSourceControlShelvedItem";
    public static readonly Guid GuidTswaHome = new Guid("0F9CED5D-89F9-4743-BAB8-FA511FF09A8C");
    public static readonly Guid GuidTswaWorkItemEditor = new Guid("7BBE4C9C-268B-4175-8979-A06878149AEF");
    public static readonly Guid GuidTswaChangesetDetail = new Guid("D40EF625-CCA7-4e73-B9EC-86CBE1534CE0");
    public static readonly Guid GuidTswaDifference = new Guid("2B84D900-1F08-486c-9C47-0E6AF371D03C");
    public static readonly Guid GuidTswaShelvesetDetail = new Guid("B5C6E965-CA8D-4dc6-A6FC-F25AF0C71D19");
    public static readonly Guid GuidTswaViewItem = new Guid("3B2CEA6D-C926-46c5-8660-E0D265705BE0");
    public static readonly Guid GuidTswaQueryResults = new Guid("42ACDF9B-F814-4e10-ABAA-0F7B5D5DF45F");
    public static readonly Guid GuidTswaAnnotate = new Guid("74B15E02-0AC2-414f-A9B9-30268659D3B5");
    public static readonly Guid GuidTswaSourceExplorer = new Guid("56B61720-A7E1-4962-AF6C-A1484BDFA92C");
    public static readonly Guid GuidTswaOpenWorkItem = new Guid("85A61FF8-0AF0-44F1-8D9A-2FABD351A26A");
    public static readonly Guid GuidTswaCreateWorkItem = new Guid("14CD69C6-88F9-4C8C-A259-D2441D77D1AF");
    public static readonly Guid GuidTswaViewServerQueryResults = new Guid("062AD1B2-B1E6-4F72-BA32-391B5F5474E4");
    public static readonly Guid GuidTswaViewWiqlQueryResults = new Guid("93CD691B-1812-4D0B-8C8A-52646C04B00A");
    public static readonly Guid GuidTswaExploreSourceControlPath = new Guid("AC0770BC-1DD6-4B8E-A811-5A03690DF44F");
    public static readonly Guid GuidTswaFindShelveset = new Guid("5043F416-D446-473A-9235-B86C6A8A731D");
    public static readonly Guid GuidTswaViewShelvesetDetails = new Guid("5F9A6D4F-766E-4A70-9DDF-BFDE6C90741E");
    public static readonly Guid GuidTswaFindChangeset = new Guid("E6ACA69E-EE72-4645-8A93-85B9A11D5C0A");
    public static readonly Guid GuidTswaViewChangesetDetails = new Guid("91F567E1-087B-4DED-AD2B-54099A60FDAE");
    public static readonly Guid GuidTswaViewSourceControlItem = new Guid("0FDC7B8F-0294-43EC-A98F-CA65213914DA");
    public static readonly Guid GuidTswaDownloadSourceControlItem = new Guid("0FB20EEA-ABEB-439D-80FA-935938C00B4E");
    public static readonly Guid GuidTswaDiffSourceControlItems = new Guid("5E91C4DA-0013-4EBB-943D-CC77F5ADB82D");
    public static readonly Guid GuidTswaAnnotateSourceControlItem = new Guid("D271E722-C261-4BC2-B0F7-1C8A9E13F907");
    public static readonly Guid GuidTswaViewSourceControlItemHistory = new Guid("EE15E514-D6C7-4AAC-96F4-7C334C9459FC");
    public static readonly Guid GuidTswaViewBuildDetails = new Guid("3A90493B-068D-4F1E-AD35-6F43C967A0D8");
    public static readonly Guid GuidTswaViewSourceControlShelvedItem = new Guid("4C81A44D-67AB-4D23-9CBE-339C9102993B");
    public static readonly Guid GuidTswaDiffSourceControlShelvedItem = new Guid("57768903-455F-4001-A956-BAFF869FEF83");
    public const string BaseSiteUrl = "BaseSiteUrl";
    public const string BaseSiteUnc = "BaseSiteUnc";
    public const string BaseServerUrl = "BaseServerUrl";
    public const string WssAdminService = "WssAdminService";
    public const string WssRootUrl = "WssRootUrl";
    public const string WssAdminUrl = "WssAdminUrl";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProjectPortalUrl = "{0}:Portal";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProjectPortalPredefinedType = "{0}:PortalType";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProjectProcessGuidanceUrl = "{0}:ProcessGuidance";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProjectProcessGuidancePredefinedType = "{0}:ProcessGuidanceType";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ProjectReportFolderPath = "{0}:ReportFolder";
    public const string BaseReportsUrl = "BaseReportsUrl";
    public const string ReportsService = "ReportsService";
    public const string ReportManagerUrl = "ReportManagerUrl";
    public const string ReportWebServiceUrl = "ReportWebServiceUrl";
  }
}
