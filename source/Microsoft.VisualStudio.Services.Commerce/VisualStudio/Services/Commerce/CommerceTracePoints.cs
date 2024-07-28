// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceTracePoints
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommerceTracePoints
  {
    private const int CommerceTracePointsStart = 5103000;
    public const int GetAllResourcesInCloudServiceEnter = 5103000;
    public const int GetAllResourceInCloudServiceTraceInput = 5103002;
    public const int GetAllResourceInCloudServiceTraceCsmPassthrough = 5103003;
    public const int GetAllResourceInCloudServiceTraceOutput = 5103048;
    public const int GetAllResourcesInCloudServiceException = 5103049;
    public const int GetAllResourcesInCloudServiceLeave = 5103050;
    public const int DeleteCloudServiceEnter = 5103051;
    public const int DeleteCloudServiceException = 5103099;
    public const int DeleteCloudServiceLeave = 5103100;
    public const int HandleSubscriptionNotificationsEnter = 5103201;
    public const int HandleSubscriptionNotificationsTraceInput = 5103202;
    public const int HandleSubscriptionNotificationsFormatterLogger = 5103204;
    public const int HandleSubscriptionNotificationsTraceOutput = 5103248;
    public const int HandleSubscriptionNotificationsException = 5103249;
    public const int HandleSubscriptionNotificationsLeave = 5103250;
    public const int GetResourceEnter = 5103401;
    public const int GetResourceTraceOutput = 5103448;
    public const int GetResourceException = 5103449;
    public const int GetResourceLeave = 5103450;
    public const int ProvisionOrUpdateResourceEnter = 5103451;
    public const int ProvisionOrUpdateResourceTraceInput = 5103452;
    public const int ProvisionOrUpdateResourceTraceMode = 5103454;
    public const int ProvisionOrUpdateOwnerCheckTrace = 5103455;
    public const int ProvisionOrUpdateExceptionTrace = 5103456;
    public const int ProvisionOrUpdateResourceTraceOutput = 5103496;
    public const int ProvisionOrUpdateResourceResourceNotFound = 5103498;
    public const int ProvisionOrUpdateResourceException = 5103499;
    public const int ProvisionOrUpdateResourceLeave = 5103500;
    public const int DeleteResourceEnter = 5103501;
    public const int DeleteResourceTraceInput = 5103502;
    public const int DeleteResourceRemovedSubscriptionRelationship = 5103505;
    public const int DeleteResourceException = 5103549;
    public const int DeleteResourceLeave = 5103550;
    public const int PerformResourceTypeOperationsEnter = 5103601;
    public const int PerformResourceTypeOperationsException = 5103649;
    public const int PerformResourceTypeOperationsLeave = 5103650;
    public const int FrameworkMeteringServiceStart = 5103801;
    public const int FrameworkMeteringServiceEnd = 5104000;
    public const int MeteringControllerStart = 5104001;
    public const int MeteringControllerEnd = 5104100;
    public const int ReportingEventsControllerStart = 5104301;
    public const int ReportingEventsControllerEnd = 5104400;
    public const int RegionControllerStart = 5104101;
    public const int RegionControllerException = 5104102;
    public const int RegionControllerEnd = 5104200;
    public const int PlatformMeteringServiceStart = 5104201;
    public const int GetOfferSubscriptionEnter = 5104202;
    public const int GetOfferSubscriptionException = 5104203;
    public const int GetOfferSubscriptionLeave = 5104204;
    public const int GetOfferSubscriptionInfo = 5104213;
    public const int ReportUsageEnter = 5104211;
    public const int ReportUsageInfo = 5104212;
    public const int ReportUsageException = 5104219;
    public const int ReportUsageLeave = 5104220;
    public const int SendOfferSubscriptionUpgradeDowngradeMessageException = 5104222;
    public const int GetOfferSubscriptionsEnter = 5104205;
    public const int GetOfferSubscriptionsException = 5104206;
    public const int GetOfferSubscriptionsLeave = 5104207;
    public const int GetOfferSubscriptionsInfo = 5104208;
    public const int PlatformMeteringServiceQueueResetJob = 5104250;
    public const int PlatformMeteringServiceQueueResetJobException = 5104251;
    public const int PlatformMeteringServiceQueueResetJobDeleteOrgLevelJobDefinitionException = 5104252;
    public const int PlatformMeteringServiceQueueResetJobNotInfrastructureHost = 5109344;
    public const int PlatformMeteringManualBillingStart = 5104260;
    public const int PlatformMeteringManualBillingStatus = 5104261;
    public const int PlatformMeteringManualBillingSubscription = 5104264;
    public const int PlatformMeteringManualBillingException = 5104265;
    public const int PlatformMeteringManualBillingLeave = 5104266;
    public const int PlatformMeteringManualBillingInfo = 5104267;
    public const int PlatformMeteringServiceGetNextCalendarDayResetTimeException = 5104268;
    public const int PlatformMeteringServiceEnd = 5104400;
    public const int ConnectedServerServiceCreateStart = 5104500;
    public const int ConnectedServerServiceCreateEnd = 5104501;
    public const int ConnectedServerServiceCreateAccount = 5104502;
    public const int ConnectedServerServiceLinkSubscription = 5104503;
    public const int ConnectedServerServiceConnectStart = 5104505;
    public const int ConnectedServerServiceConnectEnd = 5104506;
    public const int ConnectedServerServiceProvisionServiceIdentityFound = 5104507;
    public const int ConnectedServerServiceProvisionServiceIdentityInActive = 5104508;
    public const int ConnectedServerServiceProvisionServiceIdentityAddedToGroup = 5104509;
    public const int ConnectedServerServiceProvisionServiceIdentityNotAddedToGroup = 5104510;
    public const int ConnectedServerServiceProvisionServiceIdentityNew = 5104511;
    public const int ConnectedServerServiceProvisionServiceIdentitySuccess = 5104512;
    public const int ConnectedServerServiceCreateAccountSuccess = 5104513;
    public const int ConnectedServerServiceLinkSubscriptionSuccess = 5104514;
    public const int ConnectedServerServiceCreateRegistration = 5104515;
    public const int ConnectedServerServiceAuthorizeHost = 5104516;
    public const int ConnectedServerServiceAuthorizeHostCompleted = 5104517;
    public const int ConnectedServerServiceSubscriptionAadCheck = 5104518;
    public const int ConnectedServerServiceSubscriptionAdminCheck = 5104519;
    public const int ConnectedServerServiceHostAlreadyAuthorized = 5104520;
    public const int ConnectedServerServiceOrganizationTenantIdAlreadySet = 5104521;
    public const int ConnectedServerServiceSubscriptionAlreadyLinked = 5104522;
    public const int ConnectedServerServiceHostAlreadyCreated = 5104523;
    public const int ConnectedServerServiceHostCreated = 5104524;
    public const int AggregationJobStart = 5105001;
    public const int AggregationJobEnd = 5105200;
    public const int PlatformAzureIntegrationServiceStart = 5105211;
    public const int PlatformAzureIntegrationServiceLoadPluginsStart = 5105212;
    public const int PlatformAzureIntegrationServiceLoadPluginsComplete = 5105213;
    public const int PlatformAzureIntegrationServiceEnd = 5105220;
    public const int GetAzureResourceAccountEnter = 5105221;
    public const int GetAzureResourceAccountException = 5105229;
    public const int GetAzureResourceAccountLeave = 5105230;
    public const int GetAzureResourceAccountByAccountIdEnter = 5105231;
    public const int GetAzureResourceAccountByAccountIdException = 5105239;
    public const int GetAzureResourceAccountByAccountIdLeave = 5105240;
    public const int QueryForAzureResourceAccountsBySubscriptionIdEnter = 5105241;
    public const int QueryForAzureResourceAccountsBySubscriptionIdException = 5105249;
    public const int QueryForAzureResourceAccountsBySubscriptionIdLeave = 5105250;
    public const int GetAzureResourceAccountsCountBySubscriptionIdEnter = 5105251;
    public const int GetAzureResourceAccountsCountBySubscriptionIdException = 5105259;
    public const int GetAzureResourceAccountsCountBySubscriptionIdLeave = 5105260;
    public const int CreateAzureResourceAccountEnter = 5105261;
    public const int CreateAzureResourceAccountException = 5105269;
    public const int CreateAzureResourceAccountLeave = 5105270;
    public const int UpdateAzureResourceAccountEnter = 5105271;
    public const int UpdateAzureResourceAccountLeave = 5105280;
    public const int DeleteAzureResourceAccountEnter = 5105281;
    public const int DeleteAzureResourceAccountException = 5105289;
    public const int DeleteAzureResourceAccountLeave = 5105290;
    public const int GetAzureSubscriptionEnter = 5105300;
    public const int GetAzureSubscriptionException = 5105309;
    public const int GetAzureSubscriptionLeave = 5105310;
    public const int UnLinkAccountNotificationCall = 5105315;
    public const int CreateAzureSubscriptionEnter = 5105320;
    public const int CreateAzureSubscriptionOnDemand = 5105328;
    public const int CreateAzureSubscriptionException = 5105329;
    public const int CreateAzureSubscriptionLeave = 5105330;
    public const int CreateAzureSubscriptionOnDemandSubscriptionDoesNotExist = 5105335;
    public const int CreateAzureSubscriptionInfo = 5108884;
    public const int UpdateAzureSubscriptionEnter = 5105331;
    public const int UpdateAzureSubscriptionPublishNotification = 5105332;
    public const int UpdateAzureSubscriptionArgumentError = 5105333;
    public const int UpdateAzureSubscriptionOfferSubscriptionEventSent = 5105334;
    public const int UpdateAzureSubscriptionException = 5105339;
    public const int UpdateAzureSubscriptionLeave = 5105340;
    public const int IsAccountLinkedEnter = 5105341;
    public const int IsAccountLinkedLeave = 5105350;
    public const int LinkCollectionInternalEnter = 5105351;
    public const int LinkCollectionInternalCall = 5105352;
    public const int LinkCollectionInternalNotificationCall = 5105355;
    public const int LinkCollectionInternalException = 5105359;
    public const int LinkCollectionInternalExit = 5105360;
    public const int UpdateAzureSubscriptionBillingInfo = 5105370;
    public const int LinkCollectionEnter = 5108885;
    public const int LinkCollectionLeave = 5108886;
    public const int LinkCollectionInfo = 5108887;
    public const int LinkInfrastructureCollection = 5108889;
    public const int DisableLinkCollection = 5108888;
    public const int LinkCollectionCheckOwnerIdentityValue = 5109365;
    public const int LinkCollectionOwnerIdentityReturnValue = 5109366;
    public const int LinkCollectionTenantRetrieval = 5109367;
    public const int GetOrganizationObject = 5109368;
    public const int UnlinkCollectionEnter = 5108890;
    public const int UnlinkCollectionLeave = 5108891;
    public const int UnlinkCollectionInfo = 5108892;
    public const int LinkCollectionByNameEnter = 5105361;
    public const int LinkCollectionByNameLeave = 5105370;
    public const int CreateAndLinkAccountEnter = 5105371;
    public const int CreateAndLinkAccount = 5105372;
    public const int DeleteOnCreateAndLinkAccountException = 5105378;
    public const int CreateAndLinkAccountException = 5105379;
    public const int CreateAndLinkAccountLeave = 5105380;
    public const int ReportUsageMonthlyResource = 5105384;
    public const int UpdateResourceSettingsEnter = 5105390;
    public const int UpdateResourceSettingsException = 5105399;
    public const int UpdateResourceSettingsLeave = 5105400;
    public const int CheckForMissingContentTypeException = 5105304;
    public const int CheckResourceTypeIsAccountException = 5105305;
    public const int SqlComponentGetAzureSubscriptionEnter = 5105401;
    public const int SqlComponentGetAzureSubscriptionPrcCall = 5105402;
    public const int SqlComponentGetAzureSubscriptionExceptionReason = 5105403;
    public const int SqlComponentGetAzureSubscriptionException = 5105408;
    public const int SqlComponentGetAzureSubscriptionComplete = 5105409;
    public const int SqlComponentGetAzureSubscriptionLeave = 5105410;
    public const int SqlComponentAddAzureSubscriptionEnter = 5105411;
    public const int SqlComponentAddAzureSubscriptionPrcCall = 5105412;
    public const int SqlComponentAddAzureSubscriptionExceptionReason = 5105413;
    public const int SqlComponentAddAzureSubscriptionException = 5105418;
    public const int SqlComponentAddAzureSubscriptionComplete = 5105419;
    public const int SqlComponentAddAzureSubscriptionLeave = 5105420;
    public const int SqlComponentGetAzureResourceAccountsCountBySubscriptionIdEnter = 5105431;
    public const int SqlComponentGetAzureResourceAccountsCountBySubscriptionIdPrcCall = 5105432;
    public const int SqlComponentGetAzureResourceAccountsCountBySubscriptionIdExceptionReason = 5105433;
    public const int SqlComponentGetAzureResourceAccountsCountBySubscriptionIdException = 5105438;
    public const int SqlComponentGetAzureResourceAccountsCountBySubscriptionIdComplete = 5105439;
    public const int SqlComponentGetAzureResourceAccountsCountBySubscriptionIdLeave = 5105440;
    public const int SqlComponentGetAzureResourceAccountByAccountIdEnter = 5105451;
    public const int SqlComponentGetAzureResourceAccountByAccountIdPrcCall = 5105452;
    public const int SqlComponentGetAzureResourceAccountByAccountIdExceptionReason = 5105453;
    public const int SqlComponentGetAzureResourceAccountByAccountIdException = 5105458;
    public const int SqlComponentGetAzureResourceAccountByAccountIdComplete = 5105459;
    public const int SqlComponentGetAzureResourceAccountByAccountIdLeave = 5105460;
    public const int SqlComponentGetAzureResourceAccountsEnter = 5105461;
    public const int SqlComponentGetAzureResourceAccountsPrcCall = 5105462;
    public const int SqlComponentGetAzureResourceAccountsExceptionReason = 5105463;
    public const int SqlComponentGetAzureResourceAccountsException = 5105468;
    public const int SqlComponentGetAzureResourceAccountsComplete = 5105469;
    public const int SqlComponentGetAzureResourceAccountsLeave = 5105470;
    public const int SqlComponentAddAzureResourceAccountEnter = 5105471;
    public const int SqlComponentAddAzureResourceAccountPrcCall = 5105472;
    public const int SqlComponentAddAzureResourceAccountExceptionReason = 5105473;
    public const int SqlComponentAddAzureResourceAccountException = 5105478;
    public const int SqlComponentAddAzureResourceAccountComplete = 5105479;
    public const int SqlComponentAddAzureResourceAccountLeave = 5105480;
    public const int SqlComponentRemoveAzureResourceAccountEnter = 5105491;
    public const int SqlComponentRemoveAzureResourceAccountPrcCall = 5105492;
    public const int SqlComponentRemoveAzureResourceAccountExceptionReason = 5105493;
    public const int SqlComponentRemoveAzureResourceAccountException = 5105498;
    public const int SqlComponentRemoveAzureResourceAccountComplete = 5105499;
    public const int SqlComponentRemoveAzureResourceAccountLeave = 5105500;
    public const int SqlComponentUpdateAccountQuantitiesEnter = 5105501;
    public const int SqlComponentUpdateAccountQuantitiesPrcCall = 5105502;
    public const int SqlComponentUpdateAccountQuantitiesExceptionReason = 5105503;
    public const int SqlComponentUpdateAccountQuantitiesException = 5105508;
    public const int SqlComponentUpdateAccountQuantitiesComplete = 5105509;
    public const int SqlComponentUpdateAccountQuantitiesLeave = 5105510;
    public const int SqlComponentUpdatePaidBillingEnter = 5105511;
    public const int SqlComponentUpdatePaidBillingPrcCall = 5105512;
    public const int SqlComponentUpdatePaidBillingExceptionReason = 5105513;
    public const int SqlComponentUpdatePaidBillingException = 5105518;
    public const int SqlComponentUpdatePaidBillingComplete = 5105519;
    public const int SqlComponentUpdatePaidBillingLeave = 5105520;
    public const int SqlComponentAggregateUsageEventsEnter = 5105521;
    public const int SqlComponentAggregateUsageEventsPrcCall = 5105522;
    public const int SqlComponentAggregateUsageEventsExceptionReason = 5105523;
    public const int SqlComponentAggregateUsageEventsException = 5105528;
    public const int SqlComponentAggregateUsageEventsComplete = 5105529;
    public const int SqlComponentAggregateUsageEventsLeave = 5105530;
    public const int GetNotificationPublisherEnter = 5105541;
    public const int GetNotificationPublisherProviderName = 5105542;
    public const int GetNotificationPublisherException = 5105549;
    public const int GetNotificationPublisherLeave = 5105550;
    public const int CheckAccountOwnershipException = 5105569;
    public const int QueryAccountsByOwnerEnter = 5105571;
    public const int QueryAccountsByOwnerTraceInput = 5105572;
    public const int QueryAccountsByOwnerTraceOutput = 5105573;
    public const int QueryAccountsByOwnerException = 5105574;
    public const int QueryAccountsByOwnerLeave = 5105575;
    public const int QueryAccountsByOwnerInfo = 5108825;
    public const int QueryAccountsByOwnerWithTenantEnter = 5105576;
    public const int QueryAccountsByOwnerWithTenantTraceOutput = 5105578;
    public const int QueryAccountsByOwnerWithTenantException = 5105579;
    public const int QueryAccountsByOwnerWithTenantLeave = 5105580;
    public const int SqlComponentRemoveCloudServiceEnter = 5105581;
    public const int SqlComponentRemoveCloudServicePrcCall = 5105582;
    public const int SqlComponentRemoveCloudServiceExceptionReason = 5105583;
    public const int SqlComponentRemoveCloudServiceException = 5105588;
    public const int SqlComponentRemoveCloudServiceComplete = 5105589;
    public const int SqlComponentRemoveCloudServiceLeave = 5105590;
    public const int DeleteAzureCloudServiceEnter = 5105591;
    public const int DeleteAzureCloudServiceException = 5105599;
    public const int DeleteAzureCloudServiceLeave = 5105600;
    public const int SqlComponentResetResourceUsageEnter = 5105601;
    public const int SqlComponentResetResourceUsagePrcCall = 5105602;
    public const int SqlComponentResetResourceUsageExceptionReason = 5105603;
    public const int SqlComponentResetResourceUsageException = 5105608;
    public const int SqlComponentResetResourceUsageComplete = 5105609;
    public const int SqlComponentResetResourceUsageLeave = 5105610;
    public static int SqlComponentUpdateAzureResourceAccountEnter = 5108855;
    public static int SqlComponentUpdateAzureResourceAccountPrcCall = 5108856;
    public static int SqlComponentUpdateAzureResourceAccountExceptionReason = 5108857;
    public static int SqlComponentUpdateAzureResourceAccountException = 5108858;
    public static int SqlComponentUpdateAzureResourceAccountComplete = 5108859;
    public static int SqlComponentUpdateAzureResourceAccountLeave = 5108860;
    public const int MeterResetJobStart = 5105701;
    public const int MeterResetJobRunResetProcedure = 5105702;
    public const int MeterResetJobCompletedResetProcedure = 5105703;
    public const int MeterResetJobPublishNotification = 5105705;
    public const int MeterResetJobCompletedPublishNotification = 5105706;
    public const int MeterResetJobBeginSendEvents = 5105708;
    public const int MeterResetJobSaveBillableEvents = 5105709;
    public const int MeterResetBIEntry = 5105710;
    public const int MeterResetJobNoBillableEvents = 5105714;
    public const int MeterResetJobAlreadyBilledThisMonth = 5105715;
    public const int MeterResetJobFreeTierSubscription = 5105716;
    public const int MeterResetSubscriptionNotActive = 5105717;
    public const int MeterResetJobNotQueuedFreeAccount = 5105718;
    public const int MeterResetJobNotQueuedNoQuantity = 5105719;
    public const int MeterResetJobException = 5105739;
    public const int MeterResetJobEnd = 5105740;
    public const int MeterResetJobReportingException = 5105780;
    public const int MeterResetJobSendCIEventsBeforeReset = 5105783;
    public const int MeterResetJobNotRunNotInfrastructureHost = 5109343;
    public const int AssignmentBillingJobException = 5109241;
    public const int IsAssignmentBillingEnabled = 5109242;
    public const int SkipMeterResetForAssignmentBilling = 5109243;
    public const int SetQuantitiesUponLinkForAssignmentBilling = 5109244;
    public const int ResetQuantitiesUponUnlinkForAssignmentBilling = 5109245;
    public const int AssignmentBillingLicensesInfo = 5109246;
    public const int SqlComponentGetOfferSubscriptionQuantityEnter = 5109247;
    public const int SqlComponentGetOfferSubscriptionQuantityLeave = 5109248;
    public const int SqlComponentGetOfferSubscriptionQuantityComplete = 5109249;
    public const int SqlComponentGetOfferSubscriptionQuantityException = 5109250;
    public const int SqlComponentGetOfferSubscriptionQuantityPrcCall = 5109251;
    public const int AssignmentFetchJobException = 5109252;
    public const int AssignmentFetchLicensesInfo = 5109253;
    public const int AssignmentBillingAccountNull = 5109340;
    public const int AssignmentBillingEnabledSubscriptionActive = 5109341;
    public const int AssignmentBillingResult = 5109342;
    public const int DailyBillingJobException = 5109254;
    public const int DailyBillingLicensesInfo = 5109255;
    public const int PutCloudServiceEnter = 5105751;
    public const int PutCloudServiceException = 5105799;
    public const int PutCloudServiceLeave = 5105800;
    public const int EventServiceNotificationPublisherPublishEnter = 5105901;
    public const int EventServiceNotificationPublisherPublishException = 5105909;
    public const int EventServiceNotificationPublisherPublishLeave = 5105950;
    public const int TraceCheckPermissionsEnter = 5105951;
    public const int TraceCheckPermissionsInfo = 5105952;
    public const int TraceCheckPermissionsLeave = 5105953;
    public const int TraceCheckPermissionsServiceStart = 5105954;
    public const int TraceCheckPermissionsServiceEnd = 5105955;
    public const int GetUsagesEnter = 5106001;
    public const int GetUsagesTraceInput = 5106002;
    public const int GetUsagesTraceOutput = 5106007;
    public const int GetUsagesException = 5106008;
    public const int GetUsagesLeave = 5106010;
    public const int GetMetricsByNameTraceInput = 5106012;
    public const int GetMetricsByNameTraceOutput = 5106017;
    public const int GetMetricsEnter = 5106031;
    public const int GetMetricsTraceInput = 5106032;
    public const int GetMetricsTraceOutput = 5106037;
    public const int GetMetricsException = 5106038;
    public const int GetMetricsLeave = 5106040;
    public const int GetMetricsDefinitionEnter = 5106041;
    public const int GetMetricsDefinitionException = 5106044;
    public const int GetMetricsDefinitionLeave = 5106050;
    public const int TraceSetResetEarlyAdopterResourceQuantitiesEnter = 5106061;
    public const int TraceSetResetEarlyAdopterResourceQuantitiesInfo = 5106062;
    public const int TraceSetResetEarlyAdopterResourceQuantitiesLeave = 5106063;
    public const int TraceResourceQuantityUpdaterServiceStart = 5106064;
    public const int TraceResourceQuantityUpdaterServiceEnd = 5106065;
    public const int TraceSetAccountQuantityEnter = 5106066;
    public const int TraceSetAccountQuantityException = 5106067;
    public const int TraceSetAccountQuantityLeave = 5106068;
    public const int TraceUpdateResourceQuantity = 5106069;
    public const int TraceGetEarlyAdopterResourceQuantitiesEnter = 5106070;
    public const int TraceGetEarlyAdopterResourceQuantitiesLeave = 5106074;
    public const int UsageAggregatorException = 5106079;
    public const int PutResourceEnter = 5106081;
    public const int PutResourceException = 5106085;
    public const int PutResourceCallingUser = 5106086;
    public const int PutResourceLeave = 5106087;
    public const int ResourceControllerPutMethodVerboseMode = 5106088;
    public const int ResourceControllerUpdateAccountResourceNotFound = 5106089;
    public const int PutResourcePropertyReadException = 5106090;
    public const int PutResourcePropertyInfo = 5106092;
    public const int ResourceControllerCIException = 5106096;
    public const int PutResourceOwnerIdProvided = 5106097;
    public const int PutResourceOwnerIdValidationResult = 5106098;
    public const int PutResourceOwnerIdValidationFailed = 5106099;
    public const int PatchResourceEnter = 5109206;
    public const int PatchResourceException = 5109207;
    public const int PatchResourceCallingUser = 5106088;
    public const int PatchResourceLeave = 5109209;
    public const int CsmV2DeleteResourceEnter = 5106101;
    public const int CsmV2DeleteResourceWasCalled = 5106102;
    public const int CsmV2DeleteResourceException = 5106105;
    public const int CsmV2DeleteResourceLeave = 5106106;
    public const int CsmV2GetResourceEnter = 5106107;
    public const int CsmV2GetResourceWasCalled = 5106108;
    public const int CsmV2GetResourceException = 5106111;
    public const int CsmV2GetResourceLeave = 5106112;
    public const int SubHandleNotificationEnter = 5106113;
    public const int SubHandleNotificationData = 5106114;
    public const int SubHandleNotificationValidationError = 5106116;
    public const int SubHandleNotificationException = 5106117;
    public const int SubHandleNotificationLeave = 5106118;
    public const int GetResourceStatusEnter = 5106121;
    public const int GetResourceStatusException = 5106124;
    public const int GetResourceStatusLeave = 5106140;
    public const int SqlComponentGetSubscriptionsForAccountsEnter = 5106141;
    public const int SqlComponentGetSubscriptionsForAccountsPrcCall = 5106142;
    public const int SqlComponentGetSubscriptionsForAccountsExceptionReason = 5106143;
    public const int SqlComponentGetSubscriptionsForAccountsException = 5106144;
    public const int SqlComponentGetSubscriptionsForAccountsComplete = 5106145;
    public const int SqlComponentGetSubscriptionsForAccountsLeave = 5106146;
    public const int PlatformGetAccountsOwnedByIdentityEnter = 5106147;
    public const int PlatformGetAccountsOwnedByIdentityCall = 5106148;
    public const int PlatformGetAccountsOwnedByIdentityException = 5106149;
    public const int PlatformGetAccountsOwnedByIdentityLeave = 5106150;
    public const int AzureSubscriptionAccountBinderCtorEnter = 5106151;
    public const int AzureSubscriptionAccountBinderCtorLeave = 5106152;
    public const int AzureSubscriptionAccountBinderBindEnter = 5106153;
    public const int AzureSubscriptionAccountBinderBindException = 5106154;
    public const int AzureSubscriptionAccountBinderBindLeave = 5106155;
    public const int PlatformInterfaceLinkAccountCall = 5106156;
    public const int KpiHelperLoggingException = 5106158;
    public const int PublishAccountServiceHostRequest = 5106202;
    public const int PublishAccountLocationService = 5106203;
    public const int SqlComponentGetAzureSubscriptionAccountsEnter = 5106211;
    public const int SqlComponentGetAzureSubscriptionAccountsException = 5106212;
    public const int SqlComponentGetAzureSubscriptionAccountsLeave = 5106213;
    public const int SqlGetAzureResourceAccountsInResourceGroupEnter = 5106225;
    public const int SqlGetAzureResourceAccountsInResourceGroupPrcCall = 5106226;
    public const int SqlGetAzureResourceAccountsInResourceGroupLeave = 5106230;
    public const int PssGetAzureResourceAccountsInResourceGroupEnter = 5106231;
    public const int PssGetAzureResourceAccountsInResourceGroupException = 5106232;
    public const int PssGetAzureResourceAccountsInResourceGroupLeave = 5106233;
    public const int CsmV2ResourceGroupGetResourcesWasCalled = 5106234;
    public const int CsmV2ResourceGroupGetResourcesEnter = 5106235;
    public const int CsmV2ResourceGroupGetResourcesException = 5106238;
    public const int CsmV2ResourceGroupGetResourcesLeave = 5106239;
    public const int HydrateAzureResourceAccountEnter = 5106310;
    public const int HydrateAzureResourceAccountException = 5106311;
    public const int HydrateAzureResourceAccountLeave = 5106312;
    public const int GetRegionListEnter = 5106320;
    public const int GetRegionListException = 5106321;
    public const int GetRegionListTraceInput = 5106322;
    public const int GetRegionListTraceOutput = 5106323;
    public const int GetRegionListLeave = 5106324;
    public const int CreateResourceOutputTraceEnter = 5106331;
    public const int CreateResourceOutputGetAccountEnter = 5106332;
    public const int CreateResourceOutputGetAccountLeave = 5106333;
    public const int CreateResourceOutputGetTenantEnter = 5106334;
    public const int CreateResourceOutputGetTenantLeave = 5106335;
    public const int CreateResourceOutputException = 5106339;
    public const int CreateResourceOutputTraceLeave = 5106340;
    public const int GetDefaultUsageMetersEnter = 5106341;
    public const int GetDefaultUsageMetersLeave = 5106350;
    public const int GetUsageMetersEnter = 5106351;
    public const int GetUsageMetersLeave = 5106360;
    public const int GetAccountUrlEnter = 5106361;
    public const int GetAccountUrlLeave = 5106370;
    public const int RequestIsCsmPassthroughEnter = 5106371;
    public const int RequestIsCsmPassthroughLeave = 5106380;
    public const int QueryAccountsByOwnerWithTenantExtendedEnter = 5106440;
    public const int QueryAccountsByOwnerWithTenantTraceExtendedOutput = 5106442;
    public const int QueryAccountsByOwnerWithTenantExtendedException = 5106449;
    public const int QueryAccountsByOwnerWithTenantExtendedLeave = 5106450;
    public const int QueryAccountsByOwnerWithTenantExtendedInfo = 5108826;
    public const int PssSubscriptionReadPermissionCheckFailed = 5106254;
    public const int SqlComponentCleanupAzureResourceAccountsEnter = 5106500;
    public const int SqlComponentCleanupAzureResourceAccountsPrcCall = 5106501;
    public const int SqlComponentCleanupAzureResourceAccountsException = 5106502;
    public const int SqlComponentCleanupAzureResourceAccountsComplete = 5106503;
    public const int SqlComponentCleanupAzureResourceAccountsLeave = 5106504;
    public const int CleanupAzureResourceAccountsJobEnter = 5106520;
    public const int CleanupAzureResourceAccountsJobInvokeSqlComponent = 5106521;
    public const int CleanupAzureResourceAccountsJobException = 5106523;
    public const int CleanupAzureResourceAccountsJobLeave = 5106524;
    public const int StartHydrationEnter = 5106540;
    public const int StartHydrationException = 5106541;
    public const int StartHydrationLeave = 5106542;
    public const int StartDehydrationEnter = 5106543;
    public const int StartDehydrationException = 5106544;
    public const int StartDehydrationLeave = 5106545;
    public const int CommerceCacheTryGetHit = 5106451;
    public const int CommerceCacheTryGetMiss = 5106452;
    public const int CommerceCacheServiceStart = 5106453;
    public const int CommerceCacheServiceEnd = 5106454;
    public const int CommerceCacheInvalidateSuccess = 5106455;
    public const int CommerceCacheInvalidateException = 5106456;
    public const int CommerceCacheTryGetException = 5106458;
    public const int CommerceCacheTrySetException = 5106459;
    public const int CommerceCacheDisabled = 5106460;
    public const int CommerceCacheTrySetHit = 5106461;
    public const int AzureUsageEventStoreStorageException = 5106600;
    public const int AzureUsageEventStoreDataValidation = 5106602;
    public const int InvalidOperationWhileFetchingAccountUrl = 5106611;
    public const int AzureGetPricingEnter = 5106701;
    public const int AzureGetPricingLeave = 5106702;
    public const int AzureGetPricingException = 5106703;
    public const int AzureResourceHelperUserNotAadException = 5106705;
    public const int AzureResourceHelperGetPricingNonStandardOfferType = 5106704;
    public const int AzureResourceHelperGetMeterPricingException = 5106706;
    public const int AzureGetStaticPrice = 5106707;
    public const int AzureResourceHelperRegionCheckEnter = 5106708;
    public const int AzureResourceHelperRegionInfo = 5106709;
    public const int AzureGetPriceFromArmAdapter = 5106710;
    public const int AzureResourceHelperRegionCheckLeave = 5108827;
    public const int AzureGetSubscriptionsEnter = 5106711;
    public const int AzureGetSubscriptionsLeave = 5106712;
    public const int AzureGetSubscriptionsException = 5106713;
    public const int GetSecureJwtTokenException = 5106714;
    public const int AzureGetSubscriptionEnter = 5106715;
    public const int AzureGetSubscriptionLeave = 5106716;
    public const int AzureGetSubscriptionException = 5106717;
    public const int GetSecureJwtToken = 5106718;
    public const int AzureResponseError = 5106719;
    public const int AzureGetAggregateUsageEnter = 5106720;
    public const int AzureGetAggregateUsageLeave = 5106721;
    public const int AzureGetAggregateUsageError = 5106722;
    public const int AzureGetBillingUsageUri = 5106723;
    public const int TraceSecureUserContextProps = 5106724;
    public const int GetSecureJwtTokenAADCredentialNotFoundException = 5109101;
    public const int AzureGetSubscriptionAuthorizationEnter = 5106731;
    public const int AzureGetSubscriptionAuthorizationLeave = 5106732;
    public const int AzureGetSubscriptionAuthorizationException = 5106733;
    public const int AzureGetSubscriptionAuthorizationInfo = 5109092;
    public const int SubscriptionUserAdminCoAdminCheckException = 5106734;
    public const int AzureGetRoleDefinitionsEnter = 5106735;
    public const int AzureGetRoleDefinitionsLeave = 5106736;
    public const int AzureGetRoleDefinitionsException = 5106737;
    public const int AzureGetRoleAssignmentsEnter = 5106738;
    public const int AzureGetRoleAssignmentsLeave = 5106739;
    public const int AzureGetRoleAssignmentsException = 5106740;
    public const int AzureResourceManagerGetPricing = 5106752;
    public const int AzureResourceManagerGetSubAuthorization = 5106753;
    public const int GetCertificateFromStrongBox = 5106755;
    public const int AzureResourceGetPricingException = 5106756;
    public const int AzureResourceManagerGetOwnerContributor = 5108929;
    public const int AzureResourceManagerGetOwnerContributorException = 5108930;
    public const int AzureResourceManagerGetOwnerContributorInfoRoleDefinitions = 5109091;
    public const int AzureResourceManagerGetOwnerContributorInfoRoleAssignments = 5109225;
    public const int AzureResourceManagerGetOwnerContributorInfoOwnerRoleDefinitions = 5109226;
    public const int AzureResourceManagerGetOwnerContributorInfoContributorRoleDefinitions = 5109227;
    public const int GetOfferSubscriptionsForAllAzureSubscriptionEnter = 5106761;
    public const int GetOfferSubscriptionsForAllAzureSubscriptionLeave = 5106762;
    public const int GetOfferSubscriptionsForAllAzureSubscriptionException = 5106763;
    public const int GetOfferSubscriptionsForAllAzureSubscriptionInfo = 5106764;
    public const int GetSubscriptionAccountBySubscriptionIdInfo = 5109364;
    public const int GetOfferSubscriptionsForGalleryItemEnter = 5106771;
    public const int GetOfferSubscriptionsForGalleryItemLeave = 5106772;
    public const int GetOfferSubscriptionsForGalleryItemException = 5106773;
    public const int GetOfferSubscriptionsForGalleryItemInfo = 5106774;
    public const int AzureStoreRatingApiEnter = 5106781;
    public const int AzureStoreRatingApiLeave = 5106782;
    public const int AzureStoreRatingApiException = 5106783;
    public const int AzureStoreRatingApiPriceError = 5106784;
    public const int AzureStoreRatingApiPriceCount = 5106785;
    public const int AzureStoreRatingApiPriceCurrency = 5106786;
    public const int AzureStoreGetAgreementApiEnter = 5106791;
    public const int AzureStoreGetAgreementApiLeave = 5106792;
    public const int AzureStoreGetAgreementApiException = 5106793;
    public const int AzureStoreCancelAgreementApiEnter = 5106811;
    public const int AzureStoreCancelAgreementApiLeave = 5106812;
    public const int AzureStoreCancelAgreementApiException = 5106813;
    public const int AzureStoreSignAgreementApiEnter = 5106821;
    public const int AzureStoreSignAgreementApiLeave = 5106822;
    public const int AzureStoreSignAgreementApiException = 5106823;
    public const int AzureGetHttpResponseMessageEnter = 5106725;
    public const int AzureGetHttpResponseMessageLeave = 5106726;
    public const int GetArmResponseObjectPerformance = 5106727;
    public const int PutArmResponseObjectPerformance = 5106728;
    public const int LoadApiVersionsUsingConfigFramework = 5106729;
    public const int RegisterSubscriptionAgainstRPEnter = 5106815;
    public const int RegisterSubscriptionAgainstRPException = 5106816;
    public const int RegisterSubscriptionAgainstRPLeave = 5106817;
    public const int RegisterSubscriptionAgainstRPRegistrationStatus = 5106818;
    public const int EnablePaidBillingModeOnLinkAccountEnter = 5106981;
    public const int EnablePaidBillingModeOnLinkAccountLeave = 5106982;
    public const int EnablePaidBillingModeOnLinkAccountException = 5106983;
    public const int GetAzureSubscriptionForUserInternalEnter = 5106990;
    public const int GetAzureSubscriptionForUserInternalLeave = 5106991;
    public const int GetAzureSubscriptionForUserInternalException = 5106992;
    public const int GetAzureSubscriptionsForTenantsEnter = 5106993;
    public const int GetAzureSubscriptionsForTenants = 5106994;
    public const int GetAzureSubscriptionsForTenantsLeave = 5106995;
    public const int GetAzureSubscriptionsForTenantsPerformance = 5106984;
    public const int GetTenantsEnter = 5106996;
    public const int GetTenants = 5106997;
    public const int GetTenantsLeave = 5106998;
    public const int GetTenantsException = 5106999;
    public const int OfferMeterControllerGetOfferMetersEnter = 5107220;
    public const int OfferMeterControllerGetOfferMetersLeave = 5107229;
    public const int CreateOfferMeterDefinitionEnter = 5107270;
    public const int CreateOfferMeterDefinitionException = 5107271;
    public const int SqlCreateOfferMeterDefinitionEnter = 5107273;
    public const int SqlCreateOfferMeterDefinitionException = 5107274;
    public const int SqlCreateOfferMeterDefinitionExceptionReason = 5107275;
    public const int SqlCreateOfferMeterDefinitionPrcInfo = 5107276;
    public const int SqlCreateOfferMeterDefinitionLeave = 5107277;
    public const int SqlCreateOfferMeterDefinitionPlanEnter = 5107278;
    public const int SqlCreateOfferMeterDefinitionPlanException = 5107279;
    public const int SqlCreateOfferMeterDefinitionPlanExceptionReason = 5107280;
    public const int SqlCreateOfferMeterDefinitionPlanPrcInfo = 5107281;
    public const int SqlCreateOfferMeterDefinitionPlanLeave = 5107282;
    public const int SqlCreateOfferMeterException = 5107283;
    public const int CreateOfferMeterDefinitionPlanChangeInfo = 5107284;
    public const int CreateOfferMeterDefinitionPricingException = 5107285;
    public const int CreateOfferMeterDefinitionLeave = 5107289;
    public const int SqlUpdateOfferMeterDefinitionNameEnter = 5107291;
    public const int SqlUpdateOfferMeterDefinitionNameExceptionReason = 5107292;
    public const int SqlUpdateOfferMeterDefinitionNameException = 5107293;
    public const int SqlUpdateOfferMeterNameException = 5107294;
    public const int SqlUpdateOfferMeterDefinitionNameLeave = 5107295;
    public const int SqlUpdateOfferMeterDefinitionNamePrcInfo = 5107296;
    public const int MarketplaceOfferControllerEnter = 5107350;
    public const int MarketplaceOfferControllerException = 5107351;
    public const int MarketplaceOfferControllerInfo1 = 5107352;
    public const int MarketplaceOfferControllerCreateOfferInput = 5107354;
    public const int MarketplaceOfferControllerCreateOfferPlans = 5107355;
    public const int MarketplaceOfferControllerLeave = 5107359;
    public const int MarketplaceOfferControllerUSDPriceInserted = 5107360;
    public const int MarketplaceOfferControllerPriceDualWriteToCommerce = 5107361;
    public const int TrialEmailNotificationFormatter = 5107380;
    public const int TrialEmailNotificationException = 5107420;
    public const int TrialEmailNotificationEmailAddress = 5107421;
    public const int TrialEmailNotificationOfferMeterName = 5107422;
    public const int TrialEmailNotificationFormatterParams = 5107423;
    public const int MarketplaceOffer2ControllerEnter = 5107370;
    public const int MarketplaceOffer2ControllerException = 5107371;
    public const int MarketplaceOffer2ControllerInfo = 5107372;
    public const int MarketplaceOffer2ControllerCreateOfferInput = 5107374;
    public const int MarketplaceOffer2ControllerCreateOfferPlans = 5107375;
    public const int MarketplaceOffer2ControllerLeave = 5107376;
    public const int MarketplaceOffer2ControllerUSDPriceInserted = 5107377;
    public const int SendSubscriptionRenewalNotificationEmail = 5107430;
    public const int SendSubscriptionRenewalNotificationEmailException = 5107431;
    public const int SendSubscriptionRenewalNotificationEmailParams = 5107432;
    public const int SendSubscriptionRenewalNotificationEmailAddress = 5107433;
    public const int SqlComponentGetAzureSubscriptionsForUpdatesEnter = 5107381;
    public const int SqlComponentGetAzureSubscriptionsForUpdatesPrcCall = 5107382;
    public const int SqlComponentGetAzureSubscriptionsForUpdatesExceptionReason = 5107383;
    public const int SqlComponentGetAzureSubscriptionsForUpdatesException = 5107384;
    public const int SqlComponentGetAzureSubscriptionsForUpdatesComplete = 5107385;
    public const int SqlComponentGetAzureSubscriptionsForUpdatesLeave = 5107386;
    public const int GetCommercePackageEnter = 5107390;
    public const int GetCommercePackageMeterGap = 5107391;
    public const int GetCommercePackageForwarding = 5107392;
    public const int GetCommercePackageMismatch = 5107393;
    public const int GetCommercePackageProxy = 5107394;
    public const int GetCommercePackageException = 5107398;
    public const int GetCommercePackageLeave = 5107399;
    public const int PopulateOfferMeterPriceJobException = 5107391;
    public const int SubscriptionStatusSyncJobExtensionEnter = 5107400;
    public const int SubscriptionStatusSyncJobExtensionException = 5107401;
    public const int SubscriptionStatusSyncJobExtensionLeave = 5107402;
    public const int SubscriptionStatusSyncJobExtensionLastUpdatedGuid = 5107403;
    public const int SubscriptionStatusSyncJobExtensionBillingContext = 5107404;
    public const int SubscriptionStatusSyncJobExtensionACISSubscriptionStatusId = 5107405;
    public const int SubscriptionStatusSyncJobExtensionAzureSubscriptionStatusIdUpdate = 5107406;
    public const int SubscriptionStatusSyncJobExtensionUpdateAzureSubscriptionEnter = 5107407;
    public const int SubscriptionStatusSyncJobExtensionUpdateAzureSubscriptionException = 5107408;
    public const int SubscriptionStatusSyncJobExtensionUpdateAzureSubscriptionLeave = 5107409;
    public const int SubscriptionStatusSyncJobExtensionUpdateAzureSubscriptionRetryException = 5107410;
    public const int SendNotificationEmailOnTrialExpiryJobExtensionEnter = 5107411;
    public const int SendNotificationEmailOnTrialExpiryJobExtensionException = 5107412;
    public const int SendNotificationEmailOnTrialExpiryJobExtensionLeave = 5107413;
    public const int SendNotificationEmailOnTrialExpiryJobData = 5107414;
    public const int SendNotificationEmailOnTrialExpiryJobRequeue = 5107415;
    public const int SendNotificationEmailOnTrialExpiryJobTrialExpiryDate = 5107416;
    public const int SendNotificationEmailOnTrialExpiryJobSend7DaysToTrialExpiryEmail = 5107417;
    public const int SendNotificationEmailOnTrialExpiryJobRequeueFor7DaysToTrialExpiryEmail = 5107418;
    public const int SendNotificationEmailOnTrialExpiryJobRequeueForTrialExpiryEmail = 5107419;
    public const int SendNotificationEmailOnTrialExpiryJobSendTrialExpiryEmail = 5107424;
    public const int PlatformOfferSubscriptionServiceEnableTrialOrPreviewEnter = 5107440;
    public const int PlatformOfferSubscriptionServiceEnableTrialOrPreviewException = 5107441;
    public const int PlatformOfferSubscriptionServiceEnableTrialOrPreviewLeave = 5107442;
    public const int PublishMeterResetJobCompletedEventNotification = 5105781;
    public const int PublishMeterResetJobCompletedEventNotificationPublishException = 5105782;
    public const int AzureStorageFallbackNoDataException = 5108410;
    public const int AzureStorageFallbackJobNotScheduleException = 5108412;
    public const int AzureStorageFallbackEnter = 5108413;
    public const int AzureStorageFallbackLeave = 5108414;
    public const int AzureStorageFallbackJobEnter = 5108415;
    public const int AzureStorageFallbackJobLeave = 5108416;
    public const int AzureStorageFallbackIncorrectDataException = 5108417;
    public const int AzureStorageFallbackInitializationException = 5108418;
    public const int AzureStorageFallbackJobCreation = 5108419;
    public const int AzureStorageFallbackBillingEventFire = 5108420;
    public const int AzureStorageFallbackUsageBillingEventFire = 5108421;
    public const int AzureStorageFallbackReportingEventFire = 5108941;
    public const int UpgradeBuildMinEnter = 5108422;
    public const int UpgradeBuildMinExit = 5108423;
    public const int UpgradeBuildMinException = 5108424;
    public const int UpgradeBuildMinUpdateQuantities = 5108425;
    public const int SqlComponentGetMetersEnter = 5108426;
    public const int SqlComponentGetMetersException = 5108427;
    public const int SqlComponentGetMetersLeave = 5108428;
    public const int OfferMeterControllerGetOfferMeterEnter = 5108429;
    public const int OfferMeterControllerGetOfferMeterLeave = 5108431;
    public const int IsSubscriptionEligibleForPurchaseEnter = 5108442;
    public const int IsSubscriptionEligibleForPurchaseException = 5108443;
    public const int IsSubscriptionEligibleForPurchaseLeave = 5108444;
    public const int AzureSubscriptionIdTrace = 5108445;
    public const int AzureSubscriptionsInfoTrace = 5109363;
    public const int PurchaseEmailNotification = 5108446;
    public const int PurchaseEmailNotificationPriceNotAvailable = 5108447;
    public const int PurchaseEmailNotificationInfoMessageFormatter = 5108448;
    public const int PurchaseEmailNotificationEmailAddress = 5108449;
    public const int PurchaseEmailNotificationEmailListNullEmpty = 5108450;
    public const int SqlComponentGetOfferMeterPriceEnter = 5108451;
    public const int SqlComponentGetOfferMeterPriceException = 5108452;
    public const int SqlComponentGetOfferMeterPriceLeave = 5108453;
    public const int SqlAddOfferMeterPriceEnter = 5108454;
    public const int SqlAddOfferMeterPriceException = 5108455;
    public const int SqlAddOfferMeterPriceExceptionReason = 5108456;
    public const int SqlAddOfferMeterPricePrcInfo = 5108457;
    public const int SqlAddOfferMeterPriceLeave = 5108458;
    public const int SqlAddOfferMeterPricePopulatorException = 5108459;
    public const int SqlAddOfferMeterPricePrcRegionInfo = 5108460;
    public const int GetAzureSubscriptionForUserEnter = 5108461;
    public const int GetAzureSubscriptionForUserLeave = 5108462;
    public const int GetAzureSubscriptionForPurchaseEnter = 5108464;
    public const int GetAzureSubscriptionForPurchaseLeave = 5108465;
    public const int GetAzureSubscriptionForPurchaseException = 5108466;
    public const int AzureBillingServiceGetTenantEnter = 5108467;
    public const int AzureBillingServiceGetTenantException = 5108468;
    public const int AzureBillingServiceGetTenantLeave = 5108469;
    public const int AzureBillingServiceGetTenantData = 5108470;
    public const int GetSubscriptionsGeoLocationEnter = 5108471;
    public const int GetSubscriptionsGeoLocationLeave = 5108472;
    public const int GetSubscriptionsGeoLocationException = 5108473;
    public const int OfferMeterControllerGetOfferMeterPriceEnter = 5108480;
    public const int OfferMeterControllerGetOfferMeterPriceException = 5108481;
    public const int OfferMeterControllerGetOfferMeterPriceLeave = 5108482;
    public const int SqlRemoveOfferMeterDefinitionEnter = 5108483;
    public const int SqlRemoveOfferMeterDefinitionException = 5108484;
    public const int SqlRemoveOfferMeterDefinitionLeave = 5108485;
    public const int Meters2ControllerStart = 5108490;
    public const int Meters2ControllerEnd = 5108590;
    public const int UsageEventsControllerStart = 5108591;
    public const int UsageEventsControllerEnd = 5108691;
    public const int OfferMeterControllerGetPurchasableOfferMeterEnter = 5108692;
    public const int OfferMeterControllerGetPurchasableOfferMeterException = 5108693;
    public const int OfferMeterControllerGetPurchasableOfferMeterLeave = 5108694;
    public const int OfferMeterControllerGetPurchasableOfferMeterInfo = 5108908;
    public const int OfferMeterControllerUpdatePricingEnter = 5109096;
    public const int OfferMeterControllerUpdatePricingException = 5109097;
    public const int OfferMeterControllerUpdatePricingLeave = 5109098;
    public const int OfferMeterControllerUpdatePricingInfo = 5109099;
    public const int PlatformMeteringServiceGetAzureSubscriptionEnter = 5108695;
    public const int PlatformMeteringServiceGetAzureSubscriptionLeave = 5108907;
    public const int PlatformMeteringServiceApplyRulesOneResource = 5108697;
    public const int ExtensionResourceBaseController_EmitExtensionPurchaseCustomerIntelligenceEvent_Enter = 5108726;
    public const int ExtensionResourceBaseController_EmitExtensionPurchaseCustomerIntelligenceEvent_Exception = 5108728;
    public const int ExtensionResourceBaseController_EmitExtensionPurchaseCustomerIntelligenceEvent_Leave = 5108729;
    public const int CaptureExternalOperationIdHeaderAttribute_Info = 5108740;
    public const int CaptureExternalOperationIdHeaderAttribute_Null = 5108741;
    public const int CaptureExternalOperationIdHeaderAttribute_Exception = 5108742;
    public const int ExtensionResourceBaseController_EmitExtensionPurchaseCustomerIntelligenceEvent_OrderNumberMissing = 5108743;
    public const int ExtensionResourceBaseController_EmitExtensionPurchaseCustomerIntelligenceEvent_PartNumberMissing = 5108744;
    public const int SqlComponentGetAzureResourceAccountByCollectionIdEnter = 5108756;
    public const int SqlComponentGetAzureResourceAccountByCollectionIdPrcCall = 5108757;
    public const int SqlComponentGetAzureResourceAccountByCollectionIdExceptionReason = 5108758;
    public const int SqlComponentGetAzureResourceAccountByCollectionIdException = 5108759;
    public const int SqlComponentGetAzureResourceAccountByCollectionIdComplete = 5108760;
    public const int SqlComponentGetAzureResourceAccountByCollectionIdLeave = 5108761;
    public const int SqlComponentGetSubscriptionsForCollectionsEnter = 5108762;
    public const int SqlComponentGetSubscriptionsForCollectionsPrcCall = 5108763;
    public const int SqlComponentGetSubscriptionsForCollectionsExceptionReason = 5108764;
    public const int SqlComponentGetSubscriptionsForCollectionsException = 5108765;
    public const int SqlComponentGetSubscriptionsForCollectionsComplete = 5108766;
    public const int SqlComponentGetSubscriptionsForCollectionsLeave = 5108767;
    public const int GetAzureResourceAccountByCollectionIdEnter = 5108768;
    public const int GetAzureResourceAccountByCollectionIdException = 5108769;
    public const int GetAzureResourceAccountByCollectionIdLeave = 5108770;
    public const int SqlMigrateOfferSubscriptionEnter = 5108771;
    public const int SqlMigrateOfferSubscriptionException = 5108772;
    public const int SqlMigrateOfferSubscriptionLeave = 5108773;
    public const int SqlMigrateAzureResourceAccountEnter = 5108774;
    public const int SqlMigrateAzureResourceAccountException = 5108775;
    public const int SqlMigrateAzureResourceAccountLeave = 5108776;
    public const int WithCollectionContextEnter = 5108822;
    public const int WithCollectionContextException = 5108823;
    public const int WithCollectionContextLeave = 5108824;
    public const int WithCollectionContextInfo = 5108878;
    public const int CollectionHelperGetDefaultCollectionIdError = 5108827;
    public const int CollectionHelperGetCollectionDetailsInfo = 5108828;
    public const int CollectionHelperTryGetHostPropertiesError = 5108833;
    public const int WithCollectionContextEnter2 = 5108825;
    public const int WithCollectionContextException2 = 5108826;
    public const int WithCollectionContextLeave2 = 5108830;
    public const int CommerceNewBillingPipeline_UpdateStorageTable_Enter = 5108798;
    public const int CommerceNewBillingPipeline_UpdateStorageTable_Leave = 5108799;
    public const int AzureStorageConnectionStringEmpty = 5108800;
    public const int AzureStorageQueueInitializationException = 5108801;
    public const int AzureStorageTableRecordInsertionFailure = 5108802;
    public const int TraceUpdateStorageTableWithNewBillingEvents = 5108803;
    public const int AzureStorageEnqueuingFailure = 5108804;
    public const int LastSuccessfulRecordEventTimeRegistryEntryFailure = 5108805;
    public const int CommerceNewBillingPipeline_SetRegistry_Enter = 5108806;
    public const int CommerceNewBillingPipeline_SetRegistry_Leave = 5108807;
    public const int CommerceNewBillingPipeline_EnqueueUsageBilling_Enter = 5108808;
    public const int CommerceNewBillingPipeline_EnqueueUsageBilling_Leave = 5108809;
    public const int EnqueueNotificationForPushAgentFailure = 5108810;
    public const int DequeueErrorNotificationsFailure = 5108811;
    public const int BillingRecordsSyntaxFailure = 5108812;
    public const int ProcessErrorRecordsFailure = 5108813;
    public const int BillingPipelineJobExtensionFailure = 5108814;
    public const int BillingPipelineJobExtensionEntry = 5108815;
    public const int BillingPipelineJobExtensionExit = 5108816;
    public const int LastSuccessfulRecordEventTimeRegistryRetrievalFailed = 5108817;
    public const int BillingPipelineErrorHandlingJobExtensionEntry = 5108818;
    public const int BillingPipelineErrorHandlingJobExtensionFailure = 5108819;
    public const int BillingPipelineErrorHandlingJobExtensionExit = 5108820;
    public const int BillingPipelineErrorQueueInfo = 5108821;
    public const int EnqueueNotificationForPushAgent_Enter = 5108785;
    public const int EnqueueNotificationForPushAgent_Leave = 5108786;
    public const int PeekQueueForUnprocessedNotificationsFailure = 5108787;
    public const int GetUnprocessedNotificationsFromQueueFailure = 5108866;
    public const int CleanTablesNotifications = 5108788;
    public const int CleanTablesNotifications_Enter = 5108789;
    public const int CleanTablesNotifications_Leave = 5108790;
    public const int CleanTablesNotificationsRem = 5108791;
    public const int CleanTablesNotificationsFailure = 5108792;
    public const int DequeueUsageTriggerFailure = 5108793;
    public const int CleanUpTablesJobExtension_Enter = 5108794;
    public const int CleanUpTablesJobExtension_Leave = 5108795;
    public const int CleanUpTablesJobExtensionFailure = 5108796;
    public const int EnqueueErrorNotificationFailure = 5108797;
    public const int MeterResetJobSaveBillableEventsFailure = 5108822;
    public const int PlatformMeteringManualBillingStatusFailure = 5108823;
    public const int PlatformMeteringServiceFailure = 5108824;
    public const int CalculateAndSendBillingEventsToAzureFailure = 5108825;
    public const int AzureStorageFallbackBillingEvent2Fire = 5108826;
    public const int BillingPipelineJobExtensionBillingQueueEmpty = 5108931;
    public const int BillingPipelineJobExtensionEnqueueSucceeded = 5108932;
    public const int GetOfferAvailableRegions_AmbiguousOffer = 5108838;
    public const int ThirdPartyPurchaseInvalidRegion = 5108839;
    public const int ThirdPartyPurchaseOfferRegionEnter = 5108840;
    public const int PaymentInstrumentValidationEnter = 5108841;
    public const int PaymentInstrumentValidationInvalidInstrument = 5108842;
    public const int SubscriptionUserAdminCoAdminCheckResultNull = 5108863;
    public const int SubscriptionUserAdminCoAdminCheckLoginNotFound = 5108864;
    public const int SubscriptionUserAdminCoAdminCheckDoesNotContainEligibleRole = 5108865;
    public const int SubscriptionUserRoleDefinitionResultNull = 5109032;
    public const int SubscriptionUserRoleAssignmentResultNull = 5109033;
    public const int GetIdentityIdsWithIdenticalUpn = 5109239;
    public const int AccountServicePropertyMigratorStepPerformerInfo = 5108867;
    public const int JobHelperDoesJobDefinitionExist = 5108868;
    public const int JobHelperCreateJobDefinition = 5108869;
    public const int JobHelperIsJobScheduled = 5108870;
    public const int JobHelperQueueDelayedJob = 5108871;
    public const int MeterResetJobExtensionQueueSelf = 5108872;
    public const int GetSubscriptionAccountEnter = 5108873;
    public const int GetSubscriptionAccountLeave = 5108874;
    public const int GetSubscriptionAccountInfo = 5108875;
    public const int GetSubscriptionAccountError = 5108876;
    public const int GetAccountsForSubscriptionIdEnter = 5108878;
    public const int GetAccountsForSubscriptionIdLeave = 5108883;
    public const int GetAccountsEnter = 5108893;
    public const int GetAccountsLeave = 5108894;
    public const int ConstructSubscriptionAccountInfo = 5108898;
    public const int WithInfrastructureCollectionContextEnter = 5108899;
    public const int WithInfrastructureCollectionContextLeave = 5108900;
    public const int WithInfrastructureCollectionContextInvalidHostState = 5108921;
    public const int CreateInfrastructureCollectionEnter = 5108924;
    public const int CreateInfrastructureCollectionLeave = 5108925;
    public const int CreateInfrastructureCollectionException = 5108926;
    public const int CreateInfrastructureCollectionInfo = 5108927;
    public const int InfrastructureHostHelperCopyRequestContextItems = 5108928;
    public const int CreateInfrastructureOrganizationForceRouted = 5108929;
    public const int CreateBillableEventInfo = 5108904;
    public const int CreateUsageEventInfo = 5108905;
    public const int CreateSubscriptionResourceInfo = 5108906;
    public const int CreateReportingEventInfo = 5108914;
    public const int SynchronizeCommerceDataException = 5108877;
    public const int GetOfferMeterPriceEnter = 5108910;
    public const int GetOfferMeterPriceException = 5108911;
    public const int GetOfferMeterPriceLeave = 5108912;
    public const int GetOfferMeterPriceInfo = 5108913;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailJobExtensionEnter = 5115100;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailJobExtensionException = 5115101;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailJobExtensionLeave = 5115102;
    public const int SendAnnualSubscriptionPurchaseExpiryJobData = 5115103;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailSubscriptionData = 5115104;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailARMData = 5115105;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationSubscriptionAuth = 5115106;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailAddress = 5115107;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailSubscriptionDataError = 5115108;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailARMDataError = 5115109;
    public const int SendAnnualSubscriptionPurchaseExpiryNotificationEmailJobDelayError = 5115110;
    public const int PlatformOfferSubscriptionServiceExtendTrialDateEnter = 5115120;
    public const int PlatformOfferSubscriptionServiceExtendTrialDateException = 5115121;
    public const int PlatformOfferSubscriptionServiceExtendTrialDateLeave = 5115122;
    public const int PlatformOfferSubscriptionServiceExtendTrialDateData = 5115123;
    public const int PlatformOfferSubscriptionServiceExtendTrialDateEndDateError = 5115124;
    public const int StrongBoxReadFailureFromRunTimeKey = 5109021;
    public const int StrongBoxReadFailureFromOldKey = 5109022;
    public const int StrongBoxConnectionStringReadException = 5109023;
    public const int AzureReportingEventStoreStorageException = 5108940;
    public const int CommerceReportingEventJobStart = 5108990;
    public const int CommerceReportingEventJobException = 5108991;
    public const int CommerceReportingEventJobEnd = 5108999;
    public const int SaveReportingEventEnter = 5108960;
    public const int SaveReportingEventException = 5108961;
    public const int SaveReportingEventLeave = 5108969;
    public const int PublishReportingEventsEnter = 5108970;
    public const int PublishReportingEventsException = 5108971;
    public const int PublishReportingEventsLeave = 5108979;
    public const int GetReportingEventsEnter = 5108980;
    public const int GetReportingEventsException = 5108981;
    public const int GetReportingEventsLeave = 5108989;
    public const int UpdateWatermarkException = 5109000;
    public const int DecreaseResourceQuantityEnter = 5109001;
    public const int DecreaseResourceQuantityException = 5109002;
    public const int DecreaseResourceQuantityLeave = 5109003;
    public const int DecreaseResourceQuantity = 5109019;
    public const int DecreaseResourceQuantityInternal = 5109020;
    public const int IsProjectCollectionAdminException = 5109031;
    public const int AddCIEventForCommitmentLicensesException = 5109034;
    public const int AzureBillingVerificationJobDataLog = 5109035;
    public const int AzureBillingVerificationAzureDataLog = 5109036;
    public const int CsmForwardingExecuteLocalHandler = 5109096;
    public const int CsmForwardingStartRemoteRequest = 5109097;
    public const int CsmForwardingException = 5109098;
    public const int CsmDualWriteException = 5109100;
    public const int CollectionMigrationStepPerformerInfo = 5108779;
    public const int CollectionMigrationStepPerformerException = 5108784;
    public const int ResourceMigrationControllerMigrateOfferSubscriptionsEnter = 5109102;
    public const int ResourceMigrationControllerMigrateOfferSubscriptionsLeave = 5109103;
    public const int ResourceMigrationControllerMigrateAzureResourceAccountsEnter = 5109113;
    public const int ResourceMigrationControllerMigrateAzureResourceAccountsLeave = 5109114;
    public const int ResourceMigrationControllerMigrateResourcesEnter = 5109115;
    public const int ResourceMigrationControllerMigrateResourcesLeave = 5109116;
    public const int ResourceMigrationControllerMigrateSubscriptionsEnter = 5109138;
    public const int ResourceMigrationControllerMigrateSubscriptionsLeave = 5109139;
    public const int AccountLinkDualWriteAdd = 5109102;
    public const int AccountLinkDualWriteUpdate = 5109103;
    public const int AccountUnlinkDualWriteDelete = 5109104;
    public const int AccountUnlinkDualWriteReset = 5109105;
    public const int PlatformSubscriptionServiceMoveCollectionEnter = 5109110;
    public const int PlatformSubscriptionServiceMoveCollectionException = 5109111;
    public const int PlatformSubscriptionServiceMoveCollectionLeave = 5109112;
    public const int CreateInfrastructureOrganizationEnter = 5109143;
    public const int CreateInfrastructureOrganizationLeave = 5109144;
    public const int CreateInfrastructureOrganization = 5109145;
    public const int CreateInfrastructureOrganizationProperties = 5109146;
    public const int SlowCommand = 5109162;
    public const int CommerceNotificationPublishNotificationEvent = 5109163;
    public const int CommerceNotificationNullOfferMeterName = 5109164;
    public const int CommerceNotificationSendPurchaseNotificationEmail = 5109210;
    public const int CommerceNotificationNullIdentity = 5109211;
    public const int PlatformEmailNotificationServiceStartStart = 5109165;
    public const int PlatformEmailNotificationServiceStartException = 5109166;
    public const int PlatformEmailNotificationServiceStartEnd = 5109167;
    public const int PlatformEmailNotificationServiceEndStart = 5109168;
    public const int PlatformEmailNotificationServiceEndException = 5109169;
    public const int PlatformEmailNotificationServiceEndEnd = 5109170;
    public const int PlatformEmailNotificationSendEmailNotificationStart = 5109171;
    public const int PlatformEmailNotificationSendEmailNotificationException = 5109172;
    public const int PlatformEmailNotificationSendEmailNotificationEnd = 5109173;
    public const int CustomerIntelligenceCommunicatorTraceSendEmailException = 5109174;
    public const int EmailNotificationSettingsManagerStartStart = 5109175;
    public const int EmailNotificationSettingsManagerStartException = 5109176;
    public const int EmailNotificationSettingsManagerStartEnd = 5109177;
    public const int EmailNotificationSettingsManagerStopStart = 5109178;
    public const int EmailNotificationSettingsManagerStopException = 5109179;
    public const int EmailNotificationSettingsManagerStopEnd = 5109180;
    public const int EmailNotificationSettingsManagerOnRegistryChangedStart = 5109181;
    public const int EmailNotificationSettingsManagerOnRegistryChangedException = 5109182;
    public const int EmailNotificationSettingsManagerOnRegistryChangedEnd = 5109183;
    public const int PopulateSettingsHeaderTemplatesException = 5109184;
    public const int PopulateSettingsFooterTemplatesException = 5109185;
    public const int EmailNotificationHelperGetEmailBodyStart = 5109186;
    public const int EmailNotificationHelperGetEmailBodyException = 5109187;
    public const int EmailNotificationHelperGetEmailBodyEnd = 5109188;
    public const int PopulateDefaultSettingsFooterTemplates = 5109189;
    public const int PopulateDefaultSettingsHeaderTemplates = 5109190;
    public static int ReceiveMessageNotificationError = 5109192;
    public static int ReceiveMessageNotificationLegacy = 5109193;
    public static int ReceiveMessageNotificationExtension = 5109194;
    public static int ReceiveMessageNotificationWrongSU = 5109300;
    public static int IsSubscriptionEligibleForBundlePurchase = 5109195;
    public static int IsExistingPurchase = 5109213;
    public static int GetSubscriptionForUser = 5109196;
    public const int PurchaseRequestEmailNotificationEmailAddress = 5109197;
    public const int PurchaseRequestEmailNotificationEmailParams = 5109198;
    public const int PurchaseRequestEmailNotificationEmailAdminCoAdminDetails = 5109199;
    public const int PurchaseRequestEmailNotificationEmailPCADetails = 5109200;
    public const int PurchaseRequestNotificationAdminCoAdminDetails = 5109201;
    public const int PurchaseRequestNotificationAdminCoAdminIdentity = 5109202;
    public const int PurchaseRequestNotificationAdminCoAdminNullIdentity = 5109203;
    public const int PurchaseRequestNotificationEmailPCADetails = 5109204;
    public static int GetBillingSubscriptionInfoPerformance = 5109205;
    public const int SendPurchaseConfirmationEmailException = 5109212;
    public const int GetRiskEvaluationInfo = 5109228;
    public const int GetRiskEvaluationFailureStatus = 5109229;
    public const int GetRiskEvaluationPayloadNullTenantIdOrObjectId = 5109230;
    public const int WriteRiskEvaluationResultCustomerEventException = 5109231;
    public const int GetRiskEvaluationPayload = 5109232;
    public const int GetRiskEvaluationPerformance = 5109233;
    public const int GetRiskEvaluationResult = 5109234;
    public const int GetRiskEvaluationResultException = 5109235;
    public const int GetRiskEvaluationResultReject = 5109236;
    public const int GetRiskEvaluationResponseContent = 5109237;
    public const int CreateAndSendMessage = 5109261;
    public const int GetSubscriptionBillingInfoException = 5109262;
    public const int GetAccountsForUser = 5109240;
    public const int TryParsePuidToDecimalFormat = 5109238;
    public const int GetSubscriptionAccountForUser = 5109263;
    public const int GetSubscriptionAccountForUserCspIdentity = 5109268;
    public const int IsBundleEligibleForPurchaseEnter = 5109264;
    public const int IsBundleEligibleForPurchaseException = 5109265;
    public const int IsBundleEligibleForPurchaseLeave = 5109266;
    public const int IsBundleEligibleForPurchaseInfo = 5109267;
    public const int ReportingArtifactsUsage = 5109269;
    public const int CreatingArtifactsBillableEvent = 5109270;
    public const int SkipArtifactsBillingFF = 5109271;
    public const int SkipArtifactsReported = 5109272;
    public const int SkipArtifactsBillingExistingOrg = 5109277;
    public const int IsExistingArtifactsOrgException = 5109278;
    public const int IsExistingArtifactsOrg = 5109291;
    public const int IsInternalOrganization = 5109279;
    public const int SkipInternalBuildBillingFF = 5109280;
    public const int BlockInternalBuildFF = 5109281;
    public const int GetResourceLicenseLevelEnter = 5109285;
    public const int BillingEventsManipulationStart = 5109288;
    public const int BillingEventsManipulationEnd = 5109286;
    public const int ManipulateResetBillableEvents = 5109287;
    public const int CommerceHostLogicalDeleteExtension = 5109292;
    public const int CommerceHostLogicalDeleteExtensionException = 5109293;
    public const int OfferSubscriptionManipulationStart = 5109315;
    public const int OfferSubscriptionManipulationEnd = 5109316;
    public const int ManipulateOfferSubscriptionEvents = 5109317;
    public const int PlatformOfferSubscriptionServiceResetCloudLoadTestEnter = 5109345;
    public const int PlatformOfferSubscriptionServiceResetCloudLoadTestException = 5109346;
    public const int PlatformOfferSubscriptionServiceResetCloudLoadTestLeave = 5109347;
    public const int PlatformOfferSubscriptionServiceResetCloudLoadTestData = 5109348;
    public const int PlatformOfferSubscriptionServiceResetCloudLoadTestLastResetDate = 5109354;
    public const int LogLimitUpdateData = 5109355;
    public const int LogLimitUpdateException = 5109356;
    public const int LogSubscriptionLinkData = 5109357;
    public const int LogSubscriptionLinkException = 5109358;
    public const int LogSubscriptionUnlinkData = 5109359;
    public const int LogSubscriptionUnlinkException = 5109360;
    public const int LogSubscriptionUpdateData = 5109361;
    public const int LogSubscriptionUpdateException = 5109362;
    private const int NextCommerceTracePoint = 5109369;
    private const int CommerceTracePointEnd = 5114999;
    public const int SynchronizeOfferMeterPriceJobExtensionEnter = 5200000;
    public const int SynchronizeOfferMeterPriceJobExtensionException = 5200001;
    public const int SynchronizeOfferMeterPriceJobExtensionLeave = 5200002;
    public const int SynchronizeOfferMeterPriceJobExtensionMeterInfo = 5200003;
    public const int SynchronizeOfferMeterPriceJobExtensionAzureMeterResourceInfo = 5200004;
    public const int SynchronizeOfferMeterPriceJobExtensionAzureMeterResourceNullOrNullEmptyMeterRates = 5200005;
    public const int SynchronizeOfferMeterPriceJobExtensionAzureRateCardMetersNull = 5200006;

    public static class Meters2Controller
    {
      public static class ExecuteWithForwarding
      {
        public const int Proxy = 5104006;
        public const int DualExecution = 5104001;
        public const int Mismatch = 5104002;
      }

      public static class GetResourceStatus
      {
        public const int Exception = 5104003;
      }

      public static class GetResourceStatusByResourceName
      {
        public const int Exception = 5104004;
      }

      public static class UpdateMeter
      {
        public const int Exception = 5104005;
      }
    }

    public static class ReportingEventsController
    {
      public static class ExecuteWithForwarding
      {
        public const int Proxy = 5104303;
        public const int DualExecution = 5104301;
        public const int Mismatch = 5104302;
      }

      public static class GetReportingEvents
      {
        public const int Exception = 5104304;
      }
    }

    public static class PlatformOfferSubscriptionService
    {
      public const int InsightsException = 5108837;

      public static class ReportUsage
      {
        public const int ReportUsageDetailedInformation = 5106800;
      }

      public static class CreateOfferSubscription
      {
        public const int CreateHiddenAccount = 5106901;
        public const int FoundHiddenAccount = 5106902;
        public const int TraceCreateOfferSubscription = 5106903;
        public const int SignAgreement = 5106904;
        public const int CreateResourceGroup = 5106905;
        public const int CreateTemplateDeployment = 5106906;
        public const int GetExistingResourceGroup = 5106907;
        public const int OverwriteAccountRegion = 5106908;
        public const int ExtensionResourceName = 5106908;
        public static int CreateAccountResource = 5108835;
        public static int CreateExtensionResource = 5108836;
      }

      public static class CancelOfferSubscription
      {
        public const int Enter = 5107901;
        public const int Leave = 5107902;
        public const int TraceException = 5107903;
        public const int DeleteResourceGroup = 5107904;
        public const int DeleteTags = 5109127;
      }

      public static class GetPurchasableOfferMeter
      {
        public const int Enter = 5108901;
        public const int Info = 5108915;
        public const int GetConfiguration = 5108902;
        public const int LocaleCurrencyCode = 5108903;
        public const int ReturnDefault = 5108904;
        public const int ReturningFromCache = 5108905;
        public const int Exception = 5108909;
        public const int Leave = 5108910;
      }

      public static class WriteOfferSubscriptionCustomerEvent
      {
        public const int Exception = 5106969;
      }

      public static class CreatePurchaseRequest
      {
        public const int Start = 5109070;
        public const int RequestValues = 5109071;
        public const int Exception = 5109079;
        public const int Leave = 5109080;
      }

      public static class UpdatePurchaseRequest
      {
        public const int Start = 5109081;
        public const int RequestValues = 5109082;
        public const int Exception = 5109089;
        public const int Leave = 5109090;
      }
    }

    public static class OfferSubscriptionController
    {
      public static class CreateOfferSubscription
      {
        public const int Enter = 5106900;
        public const int Leave = 5106910;
        public const int Exception = 5108780;
      }

      public static class GetOfferSubscriptions
      {
        public const int Enter = 5106911;
        public const int InfoTrace = 5106912;
        public const int Exception = 5106919;
        public const int Leave = 5106920;
      }

      public static class GetAllOfferSubscriptionsForUser
      {
        public const int Enter = 5106921;
        public const int InfoTrace = 5106922;
        public const int Exception = 5106929;
        public const int Leave = 5106930;
      }

      public static class GetOfferSubscriptionsForGalleryItem
      {
        public const int Enter = 5106931;
        public const int InfoTrace = 5106932;
        public const int Exception = 5106939;
        public const int Leave = 5106940;
      }

      public static class GetOfferSubscription
      {
        public const int Enter = 5106941;
        public const int InfoTrace = 5106942;
        public const int Exception = 5106949;
        public const int Leave = 5106950;
      }

      public static class UpdateOfferSubscription
      {
        public const int Enter = 5106951;
        public const int Exception = 5106959;
        public const int Leave = 5106960;
      }

      public static class SetAccountQuantity
      {
        public const int Enter = 5117001;
        public const int Exception = 5117002;
        public const int Leave = 5117003;
      }

      public static class DecreaseResourceQuantity
      {
        public const int Enter = 5109011;
        public const int Exception = 5109012;
        public const int Leave = 5109013;
      }

      public static class CancelOfferSubscription
      {
        public const int Enter = 5106961;
        public const int Leave = 5106962;
        public const int Exception = 5108781;
      }

      public static class CreateTrialOrPreviewOfferSubscription
      {
        public const int Enter = 5106963;
        public const int Leave = 5106964;
        public const int Exception = 5108782;
      }

      public static class EnableExtensionOfTrialOfferSubscription
      {
        public const int Enter = 5115140;
        public const int Leave = 5115142;
        public const int Exception = 5115141;
      }

      public static class ReportUsage
      {
        public const int Enter = 5106965;
        public const int Leave = 5106966;
        public const int Exception = 5108783;
      }

      public static class EnableTrialOrPreviewOfferSubscription
      {
        public const int Enter = 5106975;
        public const int Leave = 5106976;
      }

      public static class ExecuteWithForwarding
      {
        public const int Proxy = 5106989;
        public const int DualExecution = 5106985;
        public const int Mismatch = 5106986;
      }

      public static class GetDefaultLicenseLevel
      {
        public const int Enter = 5107005;
        public const int InfoTrace = 5107006;
        public const int Exception = 5107007;
        public const int Leave = 5107008;
      }
    }

    public static class SubscriptionController
    {
      public static class ExecuteWithForwarding
      {
        public const int Proxy = 5106974;
        public const int DualExecution = 5106987;
        public const int Mismatch = 5106988;
      }

      public static class GetSubscriptionAccountByName
      {
        public const int Enter = 5109214;
      }

      public static class LinkAccount
      {
        public const int Exception = 5109117;
      }

      public static class UnlinkAccount
      {
        public const int Exception = 5109118;
      }

      public static class GetAccountsByIdentityForOfferId
      {
        public const int Exception = 5109119;
      }

      public static class GetSubscriptionAccount
      {
        public const int Exception = 5109120;
      }

      public static class ChangeSubscriptionAccount
      {
        public const int Exception = 5109121;
      }

      public static class GetAzureSubscriptionForPurchase
      {
        public const int Exception = 5109122;
      }

      public static class GetAzureSubscriptionForUser
      {
        public const int Exception = 5109123;
      }

      public static class GetAccounts
      {
        public const int Exception = 5109124;
      }

      public static class GetAzureSubscriptions
      {
        public const int Exception = 5109125;
      }

      public static class GetAccountsByIdentity
      {
        public const int Exception = 5109126;
      }

      public static class IsPortalStaticPageEnabled
      {
        public const int Exception = 5109274;
        public const int Enter = 5109275;
        public const int Result = 5109276;
      }
    }

    public class AccountResourceController
    {
      public static class ExecuteWithForwarding
      {
        public const int Enter = 5109128;
        public const int Mismatch = 5109129;
        public const int TraceResponse = 5109191;
      }

      public static class Accounts_CreateOrUpdate
      {
        public const int Exception = 5109130;
      }

      public static class Accounts_Delete
      {
        public const int Exception = 5109131;
      }

      public static class Accounts_Get
      {
        public const int Exception = 5109132;
      }
    }

    public static class ExtensionResourceController
    {
      public class Extensions_Create
      {
        public const int Exception = 5109134;
      }

      public class Extensions_Delete
      {
        public const int Exception = 5109135;
      }

      public class Extensions_Get
      {
        public const int Exception = 5109136;
      }

      public class Extensions_Update
      {
        public const int Exception = 5109139;
      }

      public class ExecuteWithForwarding
      {
        public const int Enter = 5109137;
        public const int Mismatch = 5109138;
      }

      public static class PutResource
      {
        public const int Enter = 5107290;
        public const int Exception = 5107298;
        public const int Leave = 5107299;
      }

      public static class SetResourceUsage
      {
        public const int EnterVerbose = 5107301;
        public const int CreateOfferReportUsage = 5107302;
        public const int CancelWithoutWrite = 5107303;
        public const int OfferMeterNotFound = 5107306;
      }

      public static class DeleteResource
      {
        public const int Enter = 5107310;
        public const int Exception = 5107318;
        public const int Leave = 5107319;
      }

      public static class GetResource
      {
        public const int Enter = 5107320;
        public const int Exception = 5107328;
        public const int Leave = 5107329;
      }

      public static class PatchResource
      {
        public const int Enter = 5108730;
        public const int Exception = 5108738;
        public const int Leave = 5108739;
      }
    }

    public class AccountResourceGroupController
    {
      public static class Accounts_ListByResourceGroup
      {
        public const int Exception = 5109133;
      }

      public static class ExecuteWithForwarding
      {
        public const int Enter = 5109140;
        public const int Mismatch = 5109141;
      }
    }

    public class ResourceGroupsController
    {
      public static class MoveResources
      {
        public const int Enter = 5109106;
        public const int Exception = 5109107;
        public const int Leave = 5109108;
        public const int ValidationFailure = 5109109;
      }

      public static class ExecuteWithForwarding
      {
        public const int Enter = 5109149;
        public const int Mismatch = 5109150;
      }
    }

    public class ResourceOperationsController
    {
      public static class Operations_List
      {
        public const int Exception = 5109151;
      }

      public static class ExecuteWithForwarding
      {
        public const int Enter = 5109152;
        public const int Mismatch = 5109153;
      }
    }

    public class SubscriptionEventsController
    {
      public static class HandleNotification
      {
        public const int Exception = 5109154;
      }

      public static class ExecuteWithForwarding
      {
        public const int Enter = 5109155;
        public const int Mismatch = 5109156;
      }
    }

    public class SubscriptionResourceController
    {
      public static class SubscriptionResources_List
      {
        public const int Enter = 5108745;
        public const int Exception = 5108754;
        public const int Leave = 5108755;
      }

      public static class ExecuteWithForwarding
      {
        public const int Enter = 5109157;
        public const int Mismatch = 5109158;
      }
    }

    public static class OfferMeterPriceBinder
    {
      public const int ConstructorEnter = 5106830;
      public const int ConstructorLeave = 5106839;
      public const int GetResourceFromReaderEnter = 5106840;
      public const int GetResourceFromReaderException = 5106848;
      public const int GetResourceFromReaderLeave = 5106849;
    }

    public static class OfferMeterPricePopulator
    {
      public const int ParseException = 5106851;
      public const int PricePlanException = 5106852;
    }

    public static class OfferSubscriptionCachedAccessService
    {
      public static class OnOfferMeterChanged
      {
        public const int InvalidatedCache = 5107001;
        public const int InvalidCacheKey = 5107002;
        public const int ExceptionOccurred = 5107003;
        public const int NoArgumentsPassed = 5107004;
      }
    }

    public static class CommerceMeteringSqlComponent
    {
      public const int RemoveTrialForPaidOfferSubscriptionEnter = 5109014;
      public const int RemoveTrialForPaidOfferSubscriptionPrcCall = 5109015;
      public const int RemoveTrialForPaidOfferSubscriptionExceptionReason = 5109016;
      public const int RemoveTrialForPaidOfferSubscriptionException = 5109017;
      public const int RemoveTrialForPaidOfferSubscriptionLeave = 5109018;
      public const int UpdateCommittedAndCurrentQuantitiesEnter = 5109004;
      public const int UpdateCommittedAndCurrentQuantitiesPrcCall = 5109005;
      public const int UpdateCommittedAndCurrentQuantitiesExceptionReason = 5109006;
      public const int UpdateCommittedAndCurrentQuantitiesException = 5109007;
      public const int UpdateCommittedAndCurrentQuantitiesComplete = 5109008;
      public const int UpdateCommittedAndCurrentQuantitiesLeave = 5109009;

      public static class AddTrialOfferSubscription
      {
        public const int Enter = 5107201;
        public const int PrcCall = 5107202;
        public const int ExceptionReason = 5107203;
        public const int Exception = 5107204;
        public const int Leave = 5107205;
      }

      public static class ExtendTrialOfferSubscription
      {
        public const int Enter = 5115130;
        public const int PrcCall = 5115131;
        public const int ExceptionReason = 5115132;
        public const int Exception = 5115133;
        public const int Leave = 5115134;
        public const int Data = 5115135;
      }

      public static class ResetCloudLoadTestUsage
      {
        public const int Enter = 5109349;
        public const int PrcCall = 5109350;
        public const int Exception = 5109351;
        public const int Leave = 5109352;
        public const int Data = 5109353;
      }
    }

    public static class FirstBillingEventForTrialOfferJobExtension
    {
      public static class Run
      {
        public const int Enter = 5107210;
        public const int Exception = 5107211;
        public const int Leave = 5107212;
      }

      public static class CalculateAndSendBillingEventsToAzure
      {
        public const int SentBillingEvent = 5107213;
        public const int SentBIEvent = 5107214;
      }

      public static class IsBillable
      {
        public const int NoQuantity = 5107215;
        public const int FreeTier = 5107216;
        public const int SubNotActive = 5107217;
      }

      public static class QueueResetJobForNextMonth
      {
        public const int JobQueued = 5107218;
        public const int SendNotification = 5107219;
      }
    }

    public static class OfferMeterAzurePlanBinder
    {
      public const int ConstructorEnter = 5107230;
      public const int ConstructorLeave = 5107239;
      public const int GetResourceFromReaderEnter = 5107240;
      public const int GetResourceFromReaderException = 5107248;
      public const int GetResourceFromReaderLeave = 5107249;
    }

    public static class ExtensionResourceGroupController
    {
      public static class GetResources
      {
        public const int Enter = 5107330;
        public const int Exception = 5107338;
        public const int Leave = 5107339;
      }

      public static class Extensions_ListByAccount
      {
        public const int Exception = 5109145;
      }

      public static class ExecuteReadWithForwarding
      {
        public const int Enter = 5109142;
        public const int Mismatch = 5109143;
      }
    }

    public static class NameAvailabilityController
    {
      public static class Accounts_CheckNameAvailability
      {
        public const int Enter = 5109021;
        public const int Exception = 5109023;
        public const int Leave = 5109022;
      }

      public static class ExecuteReadWithForwarding
      {
        public const int Enter = 5109147;
        public const int Mismatch = 5109148;
      }
    }

    public static class OfferMeterPriceCachedAccessService
    {
      public static class OnOfferMeterPriceChanged
      {
        public const int InvalidatedCache = 5108401;
        public const int InvalidCacheKey = 5108402;
        public const int ExceptionOccured = 5108403;
        public const int NoArgumentsPassed = 5108404;
        public const int InputCacheKey = 5108405;
      }
    }

    public static class CommerceCacheBase
    {
      public static class TryGetCachedItem
      {
        public const int CheckL1 = 5108474;
        public const int CheckL2 = 5108475;
        public const int NotFound = 5108476;
      }

      public static class SendMemoryCacheInvalidationSqlNotification
      {
        public const int SentNotification = 5108477;
      }

      public static class SetCacheItem
      {
        public const int ObjectProperties = 5108495;
      }
    }

    public static class OfferMeterCachedAccessService
    {
      public static class OnOfferMeterChanged
      {
        public const int InvalidatedCache = 5108478;
        public const int ExceptionOccurred = 5108479;
      }

      public static class InvalidateOfferMeterFrameworkCache
      {
        public const int InvalidatedCache = 5109095;
      }
    }

    public static class UsageEventsController
    {
      public static class ExecuteWithForwarding
      {
        public const int Proxy = 5108650;
        public const int DualExecution = 5108651;
        public const int Mismatch = 5108652;
      }

      public static class ReportUsage
      {
        public const int Exception = 5108653;
      }

      public static class GetUsage
      {
        public const int Exception = 5108654;
      }
    }

    public static class ResourceTaggingService
    {
      public static class ServiceStartEnd
      {
        public const int ServiceStart = 5108699;
        public const int ServiceEnd = 5108700;
      }

      public static class SaveTags
      {
        public const int Enter = 5108701;
        public const int TagList = 5108702;
        public const int Leave = 5108703;
      }

      public static class DeleteTags
      {
        public const int Enter = 5108704;
        public const int Leave = 5108705;
      }

      public static class GetTags
      {
        public const int Enter = 5108706;
        public const int TagList = 5108707;
        public const int Leave = 5108708;
      }
    }

    public static class PlatformOfferMeterService
    {
      public const int GetOfferMeterEnter = 5108709;
      public const int GetOfferMeterLeave = 5108710;
      public const int GetOfferMeterException = 5108711;
      public const int GetOfferMetersEnter = 5108712;
      public const int GetOfferMetersLeave = 5108713;
      public const int GetOfferMetersException = 5108714;
      public const int CreateOfferMeterDefinitionEnter = 5108715;
      public const int CreateOfferMeterDefinitionLeave = 5108716;
      public const int CreateOfferMeterDefinitionException = 5108717;
      public const int GetPurchasableOfferMeterEnter = 5108718;
      public const int GetPurchasableOfferMeterLeave = 5108719;
      public const int GetOfferMeterPriceEnter = 5108720;
      public const int GetOfferMeterPriceLeave = 5108721;
      public const int GetServiceEnter = 5108722;
      public const int GetServiceLeave = 5108723;
      public const int GetServiceInfo = 5108724;
    }

    public static class OrganizationBillingController
    {
      public static class IsDailyBillingEnabled
      {
        public const int Enter = 5109289;
        public const int Result = 5109290;
      }
    }

    public static class EnterpriseBillingService
    {
      public static class IsEnterpriseBillingEnabled
      {
        public const int Enter = 5109310;
        public const int Result = 5109311;
        public const int Exception = 5109312;
      }
    }

    public static class DefaultAccessLevelService
    {
      public static class GetDefaultAccessLevel
      {
        public const int Enter = 5109330;
        public const int Exception = 5109331;
        public const int Result = 5109332;
      }

      public static class SetDefaultAccessLevel
      {
        public const int Enter = 5109335;
        public const int Result = 5109336;
        public const int Exception = 5109337;
      }
    }

    public static class PurchaseRequestController
    {
      public static class CreatePurchaseRequest
      {
        public const int Exception = 5109159;
      }

      public static class UpdatePurchaseRequest
      {
        public const int Exception = 5109160;
      }

      public static class ExecuteWithForwarding
      {
        public const int Enter = 5109161;
      }
    }

    public static class PlatformSubscriptionService
    {
      public static int ChangeAzureResourceAccountCollectionCSSToolSPDeploymentLevelPermissionCheckSucceeded = 5200020;
      public static int ChangeAzureResourceCollectionAccountAdminPCAUserCollectionLevelPermissionCheckSucceeded = 5200021;
      public static int ChangeAzureResouceAccountCollectionWhenAccessCheckExceptionEncounteredCollectionLevelPermissionCheckSucceeded = 5200022;

      public static class ChangeSubscriptionCollection
      {
        public static int AccountLinkingException = 5108844;
        public static int ChangeSubscriptionCollectionEnter = 5108845;
        public static int ChangeSubscriptionCollectionLeave = 5108846;
        public static int ChangeSubscriptionCollectionException = 5108847;
        public static int OwnerIdentityNotFound = 5108851;
        public static int DehydrationTracing = 5108852;
        public static int HydrationTracing = 5108853;
        public static int HasAnyThirdPartyPurchasesTracing = 5108861;
        public static int CancellingThirdPartyExtensions = 5109093;
        public static int CancelledThirdPartyExtensions = 5109094;
      }

      public static class ChangeAzureResourceAccountCollection
      {
        public static int UpdateAzureResourceAccountWithNewSubscriptionResourceGroupLeave = 5108849;
        public static int UpdateAzureResourceAccountWithNewSubscriptionResourceGroupException = 5108850;
        public static int UpdateAzureResourceAccountWithNewSubscriptionResourceGroupEnter = 5108848;
        public static int SubscriptionSwapTracing = 5108854;
        public static int ChangeAzureResourceAccountCollectionEnter = 5109129;
        public static int ChangeAzureResourceAccountCollectionLeave = 5109130;
        public static int ChangeAzureResourceAccountCollectionException = 5109131;
      }

      public static class GetAzureSubscriptionFromName
      {
        public static int Enter = 5109220;
        public static int RetrieveAccountException = 5109221;
      }

      public static class IsAssignmentBillingEnabled
      {
        public static int PermissionCheckException = 5109273;
      }
    }

    public static class BillingMessageHelper
    {
      public const int TrialStarted = 5108862;
      public const int TrialEnded = 5108922;
      public const int TrialExtended = 5108923;
      public const int MessagePublishFailed = 5108951;
    }

    public static class CleanUpOrgLevelMeterResetJobsStepPerformer
    {
      public const int PerformCleanupOfOrgLevelMeterResetJobsOrgLevelJobInfo = 5109043;
      public const int PerformCleanupOfOrgLevelMeterResetJobsCollectionLevelJobInfo = 5109044;
      public const int PerformCleanupOfOrgLevelMeterResetJobsException = 5109044;
      public const int CreateCollectionLevelJobDefinition = 5109045;
      public const int QueueCollectionLevelJob = 5109046;
      public const int AdjustQueueTimeCollectionLevelJob = 5109047;
      public const int DeleteOrgLevelJob = 5109048;
      public const int OrgLevelJobNotDeleted = 5109049;
      public const int NoOperationPerformedForCollectionId = 5109050;
      public const int PerformCleanupOfOrgLevelMeterResetJobsColectionJobRunningOrAboutToRun = 5109051;
      public const int CleanUpOrgLevelMeterResetJobsStepPerformerDeleteJobDefinitionOnly = 5109052;
    }

    public static class DualWrites
    {
      public const int SubscriptionResourceUsage = 5109117;
      public const int SubscriptionResourceUsageException = 5109118;
      public const int AzureResourceAccount = 5109119;
      public const int AzureResourceAccountException = 5109120;
      public const int SubscriptionResourceUsageEnter = 5109121;
      public const int SubscriptionResourceUsageLeave = 5109122;
      public const int AzureResourceAccountEnter = 5109123;
      public const int AzureResourceAccountLeave = 5109124;
      public const int InvalidateAzureResourceAccount = 5109133;
      public const int InvalidateAzureResourceAccountException = 5109134;
      public const int InvalidateSubscriptionResourceUsage = 5109135;
      public const int InvalidateSubscriptionResourceUsageException = 5109136;
      public const int CreateAzureSubscriptionForDualWrites = 5109137;
      public const int CreateAzureSubscriptionEnter = 5109140;
      public const int CreateAzureSubscriptionLeave = 5109141;
      public const int CreateAzureSubscriptionException = 5109142;
    }

    public static class AzCommMigration
    {
      public const int DualWriteSubscription = 5109282;
      public const int DualWriteResourceAccount = 5109283;
      public const int DualWriteResourceUsage = 5109284;
      public const int DualWriteAccountTags = 5109295;
      public const int DualWriteDefaultAccessLevel = 5109296;
    }
  }
}
