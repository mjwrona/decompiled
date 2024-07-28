// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IcmConstants
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.AzureAd.Icm.Types
{
  public static class IcmConstants
  {
    public static class V3Workflow
    {
      public static class NotificationActionTypes
      {
        public const string Email = "Email";
        public const string Orchestration = "Orchestration";
      }

      public static class EntityRefIdTypes
      {
        public const string IcmV2NotificationId = "IcmV2NotificationId";
        public const string V3NotificationIdType = "V3WorkflowNotificationId";
        public const string IcmContactAliasType = "IcmContactAlias";
        public const string EmailAddress = "EmailAddress";
        public const string EmailAlias = "EmailAlias";
        public const string IcmTeamIdType = "IcmTeamId";
        public const string IcmServiceIdType = "IcmServiceId";
        public const string IcmContactIdType = "IcmContactId";
      }

      public static class NotificationCancellationReasonCodes
      {
        public const string Transferred = "Transferred";
        public const string Mitigated = "Mitigated";
        public const string Resolved = "Resolved";
        public const string SevDowngraded = "SevDowngraded";
      }
    }

    public static class UpsFlights
    {
      public static class Scopes
      {
        public const string Incidents = "incidents";
        public const string Notification = "Notification";
        public const string Contacts = "contacts";
        public const string Administration = "administration";
      }

      public static class FeatureNames
      {
        public const string ShouldEnableHitCountEvent = "ShouldEnableHitCountEvent";
        public const string ShouldV3WorkflowProcessAnyNotification = "V3Workflow_ShouldProcessAnyNotification";
        public const string SendMitigateResolveEmailsFromV3Workflow = "V3Workflow_ShouldSendMitigateResolveEmails";
        public const string MandatoryCustomFieldsForCreate = "mandatoryCustomFieldCreate";
        public const string SendTransferNotificationsFromV3Workflow = "V3Workflow_ShouldSendTransferNotifications";
        public const string SendSeverityChangeNotificationsFromV3Workflow = "V3Workflow_ShouldSendSeverityChangeNotifications";
        public const string SendReactivationNotificationsFromV3Workflow = "V3Workflow_ShouldSendReactivationNotifications";
        public const string SendUnresolveNotificationsFromV3Workflow = "V3Workflow_ShouldSendUnresolveNotifications";
        public const string SendCreateNotificationsFromV3Workflow = "V3Workflow_ShouldSendCreateNotifications";
        public const string ShowPrivateContactsCheckbox = "privateContacts";
        public const string ShowManagedCertAuth = "managedCertAuth";
        public const string MonitorThrottleCheckbox = "MonitorThrottle";
        public const string ODataWriteThrottlingFeature = "ODataWriteThrottle";
        public const string ConnectorUpdateThrottleCheckbox = "ConnectorUpdateThrottle";
        public const string UseOnlyRulesForMitigateResolutionNotify = "UseOnlyRulesForMitigateResolutionNotify";
        public const string EnableEscalationEmail = "EnableEscalationEmail";
        public const string SuppressBeforeThrottle = "SuppressBeforeThrottle";
        public const string OutageOrchestratorAutoPost = "OutageOrchestratorAutoPost";
        public const string SecondaryIncidentToPIRLinking = "secondaryIncidentToPIRLinking";
        public const string SendIcmIncidentToV3Service = "V3Incident_ShouldSendIncidentEvent";
        public const string SendIcmSimulatedTrafficToV3Service = "V3Incident_ShouldSendSimulatedEvent";
        public const string GetRepairItemFromExternalLinkService = "repairItemFromExternalLinkService";
        public const string PropagateEditToChildren = "ShouldPropagateEditToChildren";
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class AutoPost
    {
      public const string TemplateId = "Autopost Advisory";
      public const string NotifyNotes = "Initiated by Auto Post rules";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class ManagedCert
    {
      public const string DsmsAkvCert = "dSMS or AKV";
      public const string ApPkiCert = "AP PKI";
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
      public static readonly IList<string> ManagedCertTypes = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "AP PKI",
        "dSMS or AKV"
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class MaxTextLengths
    {
      public const int Keywords = 64;
      public const int IncidentType = 256;
      public const int Component = 512;
      public const int ServiceInstanceId = 64;
      public const int CorrelationId = 500;
      public const int DeviceGroup = 64;
      public const int Environment = 32;
      public const int RoutingId = 200;
      public const int DataCenter = 32;
      public const int DeviceName = 128;
      public const int DescriptionChangedBy = 128;
      public const int DescriptionAction = 128;
      public const int DescriptionTotal = 20971520;
      public const int DescriptionEntry = 2097152;
      public const int IncidentSummary = 2097152;
      public const int IncidentTitle = 512;
      public const int ReproSteps = 2097152;
      public const int SourceIncidentId = 128;
      public const int SourceCreatedBy = 128;
      public const int SourceOrigin = 128;
      public const int BridgeId = 64;
      public const int ContactNamePart = 64;
      public const int CompanyName = 32;
      public const int PhoneNumber = 32;
      public const int PhoneNumberLabel = 16;
      public const int PhoneType = 10;
      public const int Domain = 64;
      public const int TenantName = 64;
      public const int TeamName = 128;
      public const int Alias = 32;
      public const int EmailAddress = 64;
      public const int Status = 10;
      public const int RootCauseTitle = 512;
      public const int RootCauseDescription = 2097152;
      public const int RootCauseMitigation = 40979;
      public const int RootCauseFix = 40979;
      public const int IncidentMitigation = 4097;
      public const int MitigatedBy = 128;
      public const int PirTitle = 512;
      public const int PirSummary = 12291;
      public const int PirFindings = 12291;
      public const int PirCustomerImpact = 12291;
      public const int PirSupportingDocumentReference = 4096;
      public const int IncidentList = 256;
      public const int ContactAlias = 128;
      public const int ViewLinkRootCauseTitle = 16;
      public const int CustomerName = 256;
      public const int PirNotificationSource = 4096;
      public const int PirServiceImpacted = 4096;
      public const int PirClusterDataCenter = 128;
      public const int PirEventParticipants = 4096;
      public const int PirReceivedImpact = 12291;
      public const int PirQosImpact = 12291;
      public const int AlertSourceInstanceName = 128;
      public const int AlertSourceType = 128;
      public const int CertThumbprint = 256;
      public const int ConnectorName = 128;
      public const int OwnerTag = 128;
      public const int CategoryId = 64;
      public const int BugOwner = 128;
      public const int BugType = 128;
      public const int BugSource = 128;
      public const int ExternalBugId = 128;
      public const int BugDelivery = 128;
      public const int HowFixed = 32;
      public const int IncidentSubType = 32;
      public const int MonitorId = 64;
      public const int SupportTicketId = 64;
      public const int SubscriptionId = 128;
      public const int AimsId = 32;
      public const int HealthResourceId = 256;
      public const int DiagnosticsLink = 512;
      public const int ChangeList = 2048;
      public const int TeamDescription = 500;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class Dates
    {
      public const string DefaultTimeZoneId = "Pacific Standard Time";
      public const string DefaultTimeZoneShortName = "PST";
      public const string ShortTimeFormatNoSec = "HH:mm";
      public const string ShortTimeFormat = "HH:mm:ss";
      public const string ShortDateFormat = "yyyy-MM-dd";
      public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
      public const string AmPmHourFormat = "h tt";
      public const int MinutesPerDay = 1440;
      public const int DaysPerWeek = 7;
      public const int SubsProhibitionWindowMins = 15;
      public static readonly DateTime RotationStartDateNoTimeZone = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
      public static readonly DateTime RotationStartDate = new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Local);
      public static readonly DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
      public static readonly DateTime MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      public static readonly TimeSpan RotationNotificationTime = new TimeSpan(6, 0, 0);
      public static readonly TimeSpan CalendarInviteNotificationTime = new TimeSpan(8, 0, 0);
      public static readonly TimeSpan RotationStartTime = new TimeSpan(11, 0, 0);
      public static readonly TimeSpan LatestTime = new TimeSpan(23, 59, 59);
      public static readonly int DefaultOnCallReminderDaysBeforeRotation = 3;
      public static readonly int DefaultCalendarInviteDaysBeforeRotation = 7;
      public static readonly int DefaultOnCallReminderRotationCycles = 2;
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
      public static readonly IEnumerable<string> HoursList = (IEnumerable<string>) new ReadOnlyCollection<string>((IList<string>) new string[24]
      {
        "12 AM",
        "1 AM",
        "2 AM",
        "3 AM",
        "4 AM",
        "5 AM",
        "6 AM",
        "7 AM",
        "8 AM",
        "9 AM",
        "10 AM",
        "11 AM",
        "12 PM",
        "1 PM",
        "2 PM",
        "3 PM",
        "4 PM",
        "5 PM",
        "6 PM",
        "7 PM",
        "8 PM",
        "9 PM",
        "10 PM",
        "11 PM"
      });

      public static DayOfWeek GetPreviousDay(DayOfWeek dow)
      {
        switch (dow)
        {
          case DayOfWeek.Sunday:
            return DayOfWeek.Saturday;
          case DayOfWeek.Monday:
            return DayOfWeek.Sunday;
          case DayOfWeek.Tuesday:
            return DayOfWeek.Monday;
          case DayOfWeek.Wednesday:
            return DayOfWeek.Tuesday;
          case DayOfWeek.Thursday:
            return DayOfWeek.Wednesday;
          case DayOfWeek.Friday:
            return DayOfWeek.Thursday;
          case DayOfWeek.Saturday:
            return DayOfWeek.Friday;
          default:
            throw new ArgumentException("Unknown day of week found", nameof (dow));
        }
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class Severity
    {
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly IList<long> NewIncidentSeverityList = (IList<long>) new ReadOnlyCollection<long>((IList<long>) new List<long>()
      {
        1L,
        2L,
        3L,
        4L
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly IList<long> SeverityList = (IList<long>) new ReadOnlyCollection<long>((IList<long>) new List<long>()
      {
        0L,
        1L,
        2L,
        3L,
        4L
      });
      public static readonly string DefaultText = "2";
      public static readonly short Default = 2;
      public static readonly short Min = 0;
      public static readonly short Max = 4;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class ComponentName
    {
      public const string IncidentBridges = "IncidentBridges";
      public const string CacheRebuild = "CacheRebuild";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class ComponentType
    {
      public const string Portal = "PORTAL";
      public const string Task = "TASK";
      public const string DataAccess = "DATAACCESS";
      public const string WebService = "WEBSERVICE";
      public const string RestApi = "RESTAPI";
      public const string Connector = "CONNECTOR";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class FieldNames
    {
      public const string RuleId = "RuleId";
      public const string Description = "Description";
      public const string Priority = "Priority";
      public const string ForceSeverity = "ForceSeverity";
      public const string TeamId = "TeamId";
      public const string ForceTeam = "ForceTeam";
      public const string ContactAlias = "ContactAlias";
      public const string ContactId = "ContactId";
      public const string SeverityField = "Severity";
      public const string OwningTenantName = "OwningTenantName";
      public const string OwningTenantId = "OwningTenantId";
      public const string CategoryId = "CategoryId";
      public const string RoutingId = "RoutingId";
      public const string MatchOperator = "MatchOperator";
      public const string CorrelationId = "CorrelationId";
      public const string DeviceGroup = "DeviceGroup";
      public const string DeviceName = "DeviceName";
      public const string ServiceInstanceId = "ServiceInstanceId";
      public const string Environment = "Environment";
      public const string DataCenter = "DataCenter";
      public const string Cause = "Cause";
      public const string ImpactedScenarios = "ImpactedScenarios";
      public const string Component = "Component";
      public const string RootCauseCategory = "RootCauseCategory";
      public const string BridgeUriFormat = "BridgeUriFormat";
      public const string TrackingBugTypes = "TrackingBugTypes";
      public const string Resources = "Resources";
      public const string PirResources = "PirResources";
      public const string NotificationSourceType = "NotificationSourceType";
      public const string Mode = "Mode";
      public const string ModifiedBy = "ModifiedBy";
      public const string StartDate = "StartDate";
      public const string EndDate = "EndDate";
      public const string MatchRaisingLocation = "MatchRaisingLocation";
      public const string IncidentType = "IncidentType";
      public const string TfsProfile = "TFSProfile";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class AlertSourceFormatStrings
    {
      public const string DefaultFormatString = "{SOURCE:NAME}: {SOURCE:ALERTID}";
      public const string SourceName = "{SOURCE:NAME}";
      public const string SourceId = "{SOURCE:ALERTID}";
      public const string CreatedBy = "{SOURCE:CREATEDBY}";
      public const string IncidentIdFormatString = "{INCIDENT:ID}";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class Regex
    {
      public const string AlertSourceInstanceName = "^[^\\s]+$";
      public const string CertThumbprint = "^[A-Fa-f0-9]{40}$";
      public const string ConnectorName = "^[^\\s]+$";
      public const string AimsId = "^[^\\s]+$";
      public const string PhoneUSContact = "^[\\+1?\\(? ]*[2-9][0-8][0-9]\\)?[ ]*[-.]?[ ]*[2-9][0-9]{2}[ ]*[-.]?[ ]*[0-9]{4}$";
      public const string InternationalPhone = "^\\+{1}[0-9]{1,30}$";
      public const string AliasRegexText = "^[a-zA-Z0-9][a-zA-Z0-9_\\.-]*$";
      public const string EmailAddressRegexText = "^[a-zA-Z0-9][a-zA-Z0-9_\\.-]*@[a-zA-Z0-9_]+([.][a-zA-Z0-9]+)+$";
      public const string PublicPirRequestValidEmailDomainRegexText = "(?<=@)[^\\s]+";
      public const string PhoneNumberRegexText = "^[\\d]+$";
      public const string FriendlyIdRegexText = "^[a-zA-Z0-9\\-]+$";
      public const string FrontEndCategoryNameRegexText = "^[a-zA-Z0-9\\-\\(\\)/ ,]+$";
      public static readonly System.Text.RegularExpressions.Regex Alias = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9][a-zA-Z0-9_\\.-]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
      public static readonly System.Text.RegularExpressions.Regex EmailAddress = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9][a-zA-Z0-9_\\.-]*@[a-zA-Z0-9_]+([.][a-zA-Z0-9]+)+$", RegexOptions.Compiled);
      public static readonly System.Text.RegularExpressions.Regex EmailAddressInvCulture = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9][a-zA-Z0-9_\\.-]*@[a-zA-Z0-9_]+([.][a-zA-Z0-9]+)+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
      public static readonly System.Text.RegularExpressions.Regex PublicPirRequestValidEmailDomainInvCulture = new System.Text.RegularExpressions.Regex("(?<=@)[^\\s]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
      public static readonly System.Text.RegularExpressions.Regex Url = new System.Text.RegularExpressions.Regex("^(www.|(ht|f)tp(s)?://)", RegexOptions.Compiled);
      public static readonly System.Text.RegularExpressions.Regex NewLine = new System.Text.RegularExpressions.Regex("([\\r]|[\\n]|[\\r\\n])", RegexOptions.Compiled);
      public static readonly System.Text.RegularExpressions.Regex Domain = new System.Text.RegularExpressions.Regex("[a-zA-Z0-9_]+([.][a-zA-Z0-9]+)+", RegexOptions.Compiled);
      public static readonly System.Text.RegularExpressions.Regex TenantName = new System.Text.RegularExpressions.Regex("^([a-zA-Z0-9.][a-zA-Z0-9_\\.\\,\\+\\-\\&\\@\\:\\[\\]\\(\\)\\/\\\\ ]*){3}$", RegexOptions.Compiled);
      public static readonly System.Text.RegularExpressions.Regex FriendlyId = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9\\-]+$", RegexOptions.Compiled);
      public static readonly System.Text.RegularExpressions.Regex FrontEndCategoryNameRegex = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9\\-\\(\\)/ ,]+$", RegexOptions.Compiled);
    }

    public static class IfxInstrumentation
    {
      public const string LogsDirEnvVariable = "IFX_LogsDirectory";
      public const string CorrelationHeader = "X-CorrelationVector";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class QueryString
    {
      public const string PirId = "pirReportId";
      public const string SelectedTab = "selectedTab";
      public const string PirType = "pirType";
      public const string Title = "ttl";
      public const string Keywords = "kw";
      public const string ImpactedService = "is";
      public const string ImpactedComponent = "ic";
      public const string OwningService = "os";
      public const string Team = "tm";
      public const string Severity = "sev";
      public const string IncidentType = "it";
      public const string Environment = "env";
      public const string Datacenter = "dc";
      public const string CustomerImpacting = "ci";
      public const string SecurityRisk = "sr";
      public const string Description = "ds";
      public const string InstanceOrClusterSet = "ics";
      public const string SupportTicketId = "sti";
      public const string SubscriptionId = "spi";
      public const string CustomerName = "cn";
      public const string Id = "id";
      public const string Tab = "tab";
      public const string Action = "action";
      public const string DeepLink = "deeplink";
      public const string TenantId = "tenantid";
      public const string TeamId = "teamid";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class RelativeUrl
    {
      public const string PirUrl = "IncidentReportDetails2.aspx?pirReportId=";
      public const string PublicPirUrl = "PublicPostmortems.aspx?id=";
      public const string V3PirUrl = "v3/incidents/postmortem/";
      public const string V3PublicPirUrl = "v3/incidents/publicpostmortem/";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class Users
    {
      public const string IcmSystem = "IcMSystem";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class Bug
    {
      public const string InvalidBugSource = "0";
      public const string InvalidBugType = "0";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class RootCause
    {
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
      public static readonly IDictionary<int, string> RootCauseStatusMapping = (IDictionary<int, string>) new ReadOnlyDictionary<int, string>((IDictionary<int, string>) new Dictionary<int, string>()
      {
        {
          2,
          "Not Required"
        },
        {
          1,
          "Needs Investigation"
        },
        {
          3,
          "Specified"
        }
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class CommunicationsManager
    {
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
      public static readonly IDictionary<string, string> NotEngagedOptions = (IDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "Not Required",
          "NOTREQUIRED"
        },
        {
          "Used Lync",
          "LYNC"
        },
        {
          "Other",
          "OTHER"
        }
      });

      public static class EngagementModeDisplayValues
      {
        public const string IcmOption = "Used IcM";
        public const string LyncOption = "Used Lync";
        public const string OtherOption = "Other";
        public const string NotRequiredOption = "Not Required";
      }

      public static class EngagementModeStorageValues
      {
        public const string IcmOption = "ICM";
        public const string LyncOption = "LYNC";
        public const string OtherOption = "OTHER";
        public const string NotRequiredOption = "NOTREQUIRED";
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class Pir
    {
      public const int LatestVersionNumber = 2;
      public const int DefaultTimeInMinutesToExpire = 4320;
      public const int DefaultTimeInMinutesToSendReminder = 2880;
      public const string IncidentManagerTeam = "Incident Manager";
      public const string CommunicationManagerTeam = "Communication Manager";
      public const string PlaceholderIdTemplate = "ph_{0}";
      public const string PlaceholderTemplate = "<asp:PlaceHolder ID=\"ph_{0}\" runat=\"server\"></asp:PlaceHolder>";
      public const string HiddenInputTemplate = "<input type=\"hidden\" name=\"{0}\" id=\"{1}\" value=\"{2}\"/>";
      public const string PlaceholderIdPrefix = "ph_";
      public const char PlaceholderIdSeparator = '_';
      public const string ModeIndicator = "ModeIndicator";
      public const string EditValue = "Edit";
      public const string TemplatePath = "~\\Template\\IcMTemplate.xml";
      public const string DefaultMultilineBoxLength = "500px";
      public const string DefaultLabelLength = "200px";
      public const string ModeAttributeName = "mode";
      public const string ModeDisplay = "display";
      public const string ModeHidden = "hidden";
      public const string TimelineViewName = "PirResponseView";
      public const string PirResponseReadModeView = "pirResponseView";
      public const string PirResponseEditModeView = "pirResponseEdit";
      public const string BugBuilderReadModeView = "bugBuilderView";
      public const string BugBuilderEditModeView = "bugBuilderEdit";
      public const string PirResourceBuilderReadModeView = "pirResourceView";
      public const string PirResourceBuilderEditModeView = "pirResourceEdit";
      public const string PirIncidentReadModeView = "pirIncidentView";
      public const string PirIncidentEditModeView = "pirIncidentEdit";
      public const string PirAttachmentsCtrl = "AttachmentCtrl";
      public const string PirAttachmentsCtrlEdit = "AttachmentCtrlEdit";
      public const string DefaultImpactStartDescription = "The time when customer/SLA impact began";
      public const string IcmIncidentSourceName = "IcM";
      public const string SourceAlertIdFmt = "{SOURCE:ALERTID}";
      public const string SourceNameFmt = "{SOURCE:NAME}";
      public const string PublicPirPublicPortalLinkFmt = "https://{0}/portal/publicpostmortem/Details/{1}";
      public const string PublicPirLinkFmt = "https://{0}/imp/PublicPostmortems.aspx?id={1}";
      public const string DefaultDetectionDescription = "The time the issue was filed into the ticketing system (either manually or automated)";
      public const string DefaultTriageDescription = "The time when the issue was triaged / impact understood";
      public const string DefaultEngEngagedDescription = "The time when the proper engineering team was engaged and actively investigating to resolve the issue (not when they were)";
      public const string DefaultDiagnosisDescription = "The time when the issue was understood";
      public const string DefaultMitigationDescription = "The time when impact to customers was mitigated (e.g. failed over to secondary instance)";
      public const string DefaultRecoveryDescription = "The time when the incident was fully resolved/recovered";
      public const string Description = "Description";
      public const string HeaderSectionName = "Header";
      public const string EventDetailsSectionName = "EventDetails";
      public const string RootCauseSectionName = "RootCause";
      public const string TrackingBugsSectionName = "TrackingBugs";
      public const string TestImprovementAnalysisSectionName = "TestImprovementAnalysis";
      public const string AdditionalResourcesSectionName = "AdditionalResources";
      public const string CustomFieldsSectionName = "CustomFields";
      public const string TimeLineSectionName = "TimeLine";
      public const string PageVersion1 = "V1";
      public const string PageVersion2 = "V2";
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "the strings added to the list are all constants")]
      public static readonly IList<string> SectionList = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "Header",
        "EventDetails",
        "RootCause",
        "TimeLine",
        "TestImprovementAnalysis",
        "TrackingBugs",
        "AdditionalResources",
        "CustomFields"
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "the strings added to the list are all constants")]
      public static readonly IList<PirReportStatusEnumText> ReportStatusEditOptionsList = (IList<PirReportStatusEnumText>) new ReadOnlyCollection<PirReportStatusEnumText>((IList<PirReportStatusEnumText>) new List<PirReportStatusEnumText>()
      {
        new PirReportStatusEnumText("Completed", PirStatus.Completed),
        new PirReportStatusEnumText("In Progress", PirStatus.InProgress),
        new PirReportStatusEnumText("Ready For Review", PirStatus.ReadyForReview)
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The list consists of immutable reference types")]
      public static readonly IList<PirReportStatusEnumText> InternalPirStatusList = (IList<PirReportStatusEnumText>) new ReadOnlyCollection<PirReportStatusEnumText>((IList<PirReportStatusEnumText>) new List<PirReportStatusEnumText>()
      {
        new PirReportStatusEnumText("Abandoned", PirStatus.Abandoned),
        new PirReportStatusEnumText("Completed", PirStatus.Completed),
        new PirReportStatusEnumText("In Progress", PirStatus.InProgress),
        new PirReportStatusEnumText("Not Started", PirStatus.NotStarted),
        new PirReportStatusEnumText("Ready For Review", PirStatus.ReadyForReview)
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The list consists of immutable reference types")]
      public static readonly IList<PirReportStatusEnumText> PirStatusList = (IList<PirReportStatusEnumText>) new ReadOnlyCollection<PirReportStatusEnumText>((IList<PirReportStatusEnumText>) new List<PirReportStatusEnumText>()
      {
        new PirReportStatusEnumText("Abandoned", PirStatus.Abandoned),
        new PirReportStatusEnumText("Completed", PirStatus.Completed),
        new PirReportStatusEnumText("In Progress", PirStatus.InProgress),
        new PirReportStatusEnumText("Not Started", PirStatus.NotStarted),
        new PirReportStatusEnumText("Ready For Review", PirStatus.ReadyForReview),
        new PirReportStatusEnumText("Approved", PirStatus.Approved),
        new PirReportStatusEnumText("Published", PirStatus.Published)
      });

      public static Dictionary<string, string> PostMortemPageVersions => new Dictionary<string, string>()
      {
        {
          "V1",
          "IncidentReportDetails.aspx"
        },
        {
          "V2",
          "IncidentReportDetails2.aspx"
        }
      };

      public static ReadOnlyCollection<string> ResponseTimelinePropertyInSequence => new ReadOnlyCollection<string>((IList<string>) new string[9]
      {
        "ImpactStart",
        "Detection",
        "Triage",
        "EngEngaged",
        "CommsEngaged",
        "Dashboard",
        "Mitigation",
        "Diagnosis",
        "Recovery"
      });

      public static Dictionary<string, string> PropertyHintMap => new Dictionary<string, string>()
      {
        {
          "ReportOwner",
          "The owner of the report"
        },
        {
          "ReportTitle",
          "Report title"
        },
        {
          "ImpactDuration",
          "How long the impact lasted until it was resolved"
        },
        {
          "NotificationSourceType",
          "How was this impact detected"
        },
        {
          "NotificationSource",
          "Name of the source that detected impact (ex. Gomez)"
        },
        {
          "ServiceImpacted",
          "Which services were impacted (ex. MSODS, AADUX)"
        },
        {
          "ClusterDataCenter",
          "Data Center/Region impacted"
        },
        {
          "ServiceResponsible",
          "The service responsible for the impact"
        },
        {
          "EventParticipants",
          "People who worked the incident, were part of the bridge etc."
        },
        {
          "OwningTeam",
          "The feature team that resolved the incident"
        },
        {
          "PerceivedImpact",
          "How was the customer/SLA impacted, what scenarios were affected?"
        },
        {
          "QosImpact",
          "Describe the impact as measured by Quality of Service or most appropriate metric"
        },
        {
          "RootCauseCategory",
          "The overall category the Root Cause falls under"
        },
        {
          "RepeatOutage",
          "Has this Root Cause caused an impact before?"
        },
        {
          "RepeatOutageDetail",
          "Incident Id, PIR id of the previous outage"
        },
        {
          "RootCauseDescription",
          "Details/Description of the Root Cause"
        },
        {
          "RootCauseMitigation",
          "What steps were taken to immediately mitigate the issue"
        },
        {
          "RootCauseFix",
          "What is the long term fix to address this Root Cause"
        },
        {
          "KnownTestCase",
          "Was this a test case that test could have known to address?"
        },
        {
          "PossibleTest",
          "Was it possible to execute the scenario and recognize the failure in testing?"
        },
        {
          "TestingIssues",
          "Describe why this was not found in tests and testing challenges for the issue"
        }
      };

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class HtmlConstants
      {
        public const string DetailsUpdate = "DetailsUpdate";
        public const string DetailsEdit = "DetailsEdit";
        public const string DetailsCancel = "DetailsCancel";
        public const string Update = "Update";
        public const string Cancel = "Cancel";
        public const string SaveChanges = "Save Changes";
        public const string EditDetails = "Edit Details";
        public const string EditCommand = "Edit";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class EditableProperties
      {
        public const string Title = "ReportTitle";
        public const string Owner = "ReportOwner";
        public const string Severity = "Severity";
        public const string IncidentDate = "IncidentRaisedDate";
        public const string Status = "ReportStatus";
        public const string Incidents = "Incidents";
        public const string NotificationSourceType = "NotificationSourceType";
        public const string NotificationSource = "NotificationSource";
        public const string ImpactDuration = "ImpactDuration";
        public const string ClusterDataCenter = "ClusterDataCenter";
        public const string ServiceImpacted = "ServiceImpacted";
        public const string ServiceResponsible = "ServiceResponsible";
        public const string EventParticipants = "EventParticipants";
        public const string OwningTeam = "OwningTeam";
        public const string PerceivedImpact = "PerceivedImpact";
        public const string QosImpact = "QosImpact";
        public const string NoOfCustomersImpacted = "NoOfCustomersImpacted";
        public const string NoOfCustomersNotified = "NoOfCustomersNotified";
        public const string RepeatOutage = "RepeatOutage";
        public const string CausedByChange = "CausedByChange";
        public const string PirResources = "PirResources";
        public const string RepeatOutageDetail = "RepeatOutageDetail";
        public const string RootCauseTitle = "RootCauseTitle";
        public const string RootCauseDetails = "RootCauseDescription";
        public const string RootCauseCategory = "RootCauseCategory";
        public const string RootCauseMitigation = "RootCauseMitigation";
        public const string RootCauseFix = "RootCauseFix";
        public const string RepairDescription = "RepairDescription";
        public const string ResponseTimeline = "ResponseTimeline";
        public const string TestCaseKnown = "KnownTestCase";
        public const string PossibleTest = "PossibleTest";
        public const string TestingIssues = "TestingIssues";
        public const string ImpactDurationHiddenField = "ImpactDurationHiddenField";
        public const string PirType = "PostmortemType";
        public const string SREOrSEOwner = "SREOrSEOwner";
        public const string IncidentManager = "IncidentManager";
        public const string CommunicationManager = "CommunicationManager";
        public const string AssignedDev = "AssignedDev";
        public const string AssignedPM = "AssignedPM";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class TimelineProperties
      {
        public const string ImpactStart = "ImpactStart";
        public const string Detection = "Detection";
        public const string Triage = "Triage";
        public const string EngineerEngaged = "EngineerEngaged";
        public const string Diagnosis = "Diagnosis";
        public const string Recovery = "Recovery";
        public const string Mitigation = "Mitigation";
        public const string Other1 = "Other1";
        public const string Other2 = "Other2";
        public const string Other3 = "Other3";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class ControlIds
      {
        public const string BugBuilder = "bugBuilderControl";
        public const string ResponseTimeline = "responseTimelineControl";
        public const string GeneralErrorLabel = "GeneralErrorLabel";
        public const string GeneralErrorLabel2 = "GeneralErrorLabel2";
        public const string HeaderUpdateErrorLabel = "HeaderUpdateErrorLabel";
        public const string StatusUpdateCompulsoryFieldsErrorLabel = "StatusUpdateCompulsoryFieldsErrorLabel";
        public const string EventDetailsUpdateErrorLabel = "EventDetailsUpdateErrorLabel";
        public const string TimelineUpdateErrorLabel = "TimelineUpdateErrorLabel";
        public const string TestImprovementAnalysisErrorLabel = "TestImprovementAnalysisErrorLabel";
        public const string TrackingBugsErrorLabel = "TrackingBugsErrorLabel";
        public const string CustomFieldsErrorLabel = "CustomFieldsErrorLabel";
        public const string AdditionalResourcesErrorLabel = "AdditionalResourcesErrorLabel";
        public const string RootCauseUpdateErrorLabel = "RootCauseUpdateErrorLabel";
        public const string SaveButton = "DetailsUpdate";
        public const string SaveButton2 = "DetailsUpdate2";
        public const string PirTimelineError = "pirResponseErrorLabel";
        public const string ParamIndicatorLabel = "ParamIndicatorLabel";
        public const string HeaderSectionLockStatusLabel = "HeaderSectionLockStatusLabel";
        public const string EventDetailsSectionLockStatusLabel = "EventDetailsSectionLockStatusLabel";
        public const string RootCauseSectionLockStatusLabel = "RootCauseSectionLockStatusLabel";
        public const string TimeLineSectionLockStatusLabel = "TimeLineSectionLockStatusLabel";
        public const string TrackingBugsSectionLockStatusLabel = "TrackingBugsSectionLockStatusLabel";
        public const string TestImprovementAnalysisSectionLockStatusLabel = "TestImprovementAnalysisSectionLockStatusLabel";
        public const string AdditionalResourcesSectionLockStatusLabel = "AdditionalResourcesSectionLockStatusLabel";
        public const string CustomFieldsSectionLockStatusLabel = "CustomFieldsSectionLockStatusLabel";
        public const string HeaderEditButton = "HeaderEditButton";
        public const string EventDetailsEditButton = "EventDetailsEditButton";
        public const string RootCauseEditButton = "RootCauseEditButton";
        public const string TimelineEditButton = "TimelineEditButton";
        public const string TrackingBugsEditButton = "TrackingBugsEditButton";
        public const string TestImprovementAnalysisEditButton = "TestImprovementAnalysisEditButton";
        public const string AdditionalResourcesEditButton = "AdditionalResourcesEditButton";
        public const string CustomFieldsEditButton = "CustomFieldsEditButton";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class ControlPrefix
      {
        public const string HyperLink = "hyperLink_";
        public const string TextBox = "txt_";
        public const string DropDownList = "dropdownlist_";
        public const string ErrorLabel = "errorLabel_";
        public const string SuccessLabel = "successLabel_";
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
    public static class PublicPir
    {
      private static IDictionary<string, long> publicPirSourceNameToTypeIdMapping = (IDictionary<string, long>) new ReadOnlyDictionary<string, long>((IDictionary<string, long>) new Dictionary<string, long>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "Icm",
          0L
        },
        {
          "Tfs",
          1L
        }
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "the strings added to the list are all constants")]
      public static readonly IList<PirReportStatusEnumText> StatusEditOptionsList = (IList<PirReportStatusEnumText>) new ReadOnlyCollection<PirReportStatusEnumText>((IList<PirReportStatusEnumText>) new List<PirReportStatusEnumText>()
      {
        new PirReportStatusEnumText("In Progress", PirStatus.InProgress),
        new PirReportStatusEnumText("Ready For Review", PirStatus.ReadyForReview),
        new PirReportStatusEnumText("Approved", PirStatus.Approved),
        new PirReportStatusEnumText("Published", PirStatus.Published)
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The list consists of immutable reference types")]
      public static readonly IList<PirReportStatusEnumText> StatusEnumTextList = (IList<PirReportStatusEnumText>) new ReadOnlyCollection<PirReportStatusEnumText>((IList<PirReportStatusEnumText>) new List<PirReportStatusEnumText>()
      {
        new PirReportStatusEnumText("Abandoned", PirStatus.Abandoned),
        new PirReportStatusEnumText("Not Started", PirStatus.NotStarted),
        new PirReportStatusEnumText("In Progress", PirStatus.InProgress),
        new PirReportStatusEnumText("Ready For Review", PirStatus.ReadyForReview),
        new PirReportStatusEnumText("Approved", PirStatus.Approved),
        new PirReportStatusEnumText("Published", PirStatus.Published)
      });
      public static readonly PirStatus AllEditablePirStatus = PirStatus.NotStarted | PirStatus.InProgress | PirStatus.ReadyForReview | PirStatus.Approved | PirStatus.Published;

      public static IDictionary<string, long> PublicPirSourceNameToTypeIdMapping => IcmConstants.PublicPir.publicPirSourceNameToTypeIdMapping;

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class EditableProperties
      {
        public const string Title = "PublicPirTitle";
        public const string Owner = "PublicPirOwner";
        public const string Status = "PublicPirStatus";
        public const string Team = "PublicPirTeam";
        public const string ServicesImpacted = "PublicPirServicesImpacted";
        public const string IncidentStartDate = "PublicPirIncidentStartDate";
        public const string ServiceRestoredDate = "PublicPirServiceRestoredDate";
        public const string Workaround = "PublicPirWorkaround";
        public const string AffectedSubRegions = "PublicPirAffectedSubRegions";
        public const string Summary = "PublicPirSummary";
        public const string CustomerImpact = "PublicPirCustomerImpact";
        public const string RootCause = "PublicPirRootCause";
        public const string NextSteps = "PublicPirNextSteps";
        public const string SendForReview = "PublicPirSendForReview";
        public const string Approve = "PublicPirApprove";
        public const string Reject = "PublicPirReject";
        public const string Publish = "PublicPirPublish";
        public const string ResourceBuilderReadModeView = "publicPirResourceView";
        public const string ResourceBuilderEditModeView = "publicPirResourceEdit";
        public const string TimelineReadModeView = "publicPirTimelineView";
        public const string TimelineEditModeView = "publicPirTimelineEdit";
        public const string IncidentReadModeView = "publicPirIncidentView";
        public const string IncidentEditModeView = "publicPirIncidentEdit";
        public const string ErrorLabel = "PublicPirErrorLabel";
        public const string UpdateStatus = "updateStatus";
        public const string ApproversRoleName = "PublicPostmortemApprovers";
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class TenantNotConfiguredValues
    {
      public const string TrackingBugType = "Not Applicable";
      public const string RootCauseCategory = "Not Applicable";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class TfsSync
    {
      public const string NotMonitoredNodeValue = "Non RD Services (Not Monitored)";
      public const string NotMonitoredNodeName = "-- UNMONITORED NODE --";
      public const int DefaultRoutingPriority = 1;
      public const int DefaultRoutingSeverity = 3;
      public const string RoutingRuleDescription = "Created for sync to TFS. Do not modify without consulting the IcM Team. If you need to edit/delete this rule, please edit TFS sync settings from the Manage Teams page.";
      public const string RoutingIdCategory = "AutoGenerated-For-Rd-TFS-Connector-Do-Not-Edit";
      public const string TfsTrackingTeam = "TrackingTeam";
      public const string TfsAssignedTeam = "Assigned Team";
    }

    public static class RoleDefinitions
    {
      public const string TenantAdminRoleDefName = "TenantAdmin";
      public const string UsersRoleDefName = "Users";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class Misc
    {
      private static readonly string ThrottleStatusText = 429.ToString();
      public const long NoHistoryEntryForIncidentEvent = -1;
      public const long TenantInstanceMappingNotPerformed = -1;
      public const string AimsIdKeyForConnectors = "AimsId";
      public const int ThrottledStatusCode = 429;
      public const string BogusEmailAlias = "ENTER_TEAM_ALIAS";
      public const int MinAutoUpdateFrequency = 10;
      public const int LyncRetries = 3;
      public const string PortalAlertSourceName = "ICMPortal";
      public const string ApiAlertSourceName = "ICM REST API";
      public const string PublicPortalAlertSourceName = "ICM Public Portal";
      public const string Unknown = "Unknown";
      public const int IncidentChangesDefaultOwnershipMins = 10;
      public const int IncidentChangesMaxOwnershipMins = 60;
      public const string DefaultResolveDescription = "Resolved Incident";
      public const string DefaultEditResolvedIncidentDescription = "Edited a resolved incident";
      public const string DefaultEditMitigatedIncidentDescription = "Edited a mitigated incident";
      public const string MitigationLabel = "Mitigation: ";
      public const long MaxRuleFilters = 5;
      public const int MaxDescriptionEntriesCount = 250;
      public const int EarlyDescriptionEntriesCount = 5;
      public const string FeedbackEmailAddress = "icmsupport@microsoft.com";
      public const string PhoneAppOnboardingUrl = "http://aka.ms/icmphone";
      public const string True = "1";
      public const string False = "0";
      public const string UploadsPath = "c:\\icm\\uploads";
      public const long TenantAdminRole = 9223372036848484351;
      public const int AutoInviteMaxDelayTimeInMinutes = 120;
      public const int AutoInviteDelayPeriod = 5;
      public const string AutoInviteTenantSource = "TENANT";
      public const string AutoInviteTeamSource = "TEAM";
      public const int MaxBackIntervalForAutoInviteInMinutes = 240;
      public const string IncidentNodeFaultSubType = "Node Fault";

      public static string ThrottledStatusCodeText => IcmConstants.Misc.ThrottleStatusText;

      public enum AutoInviteSettings
      {
        UseServiceLevelSettings,
        ReplaceServiceLevelSettings,
        AddToServiceLevelSettings,
      }

      public enum SourceTypeOptions
      {
        Invalid,
        Tenant,
        Team,
      }

      public enum AutoBridgeSettings
      {
        UseServiceLevelSettings,
        OverrideServiceLevelSettings,
        InAdditionToServiceLevelSettings,
        Disabled,
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class Paging
    {
      public const long RulesStartRow = 0;
      public const int RulesPageDefaultSize = 500;
      public const long RulesPageMaxSize = 2000;
      public const int DirectoryPageDefaultSize = 100;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class CustomField
    {
      public const int MaxShortStringLength = 512;
      public const int MaxBigStringLength = 10000;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class CustomSearch
    {
      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class SpecialText
      {
        public const string Any = "(Any)";
        public const string AnyOld = "Any";
        public const string CurrentUser = "@Me";
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
        public static readonly IEnumerable<char> TextSeparator = (IEnumerable<char>) new ReadOnlyCollection<char>((IList<char>) new char[1]
        {
          ':'
        });
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
        public static readonly IEnumerable<char> ValueSeparator = (IEnumerable<char>) new ReadOnlyCollection<char>((IList<char>) new char[1]
        {
          '>'
        });
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class QueryParameters
      {
        public const string LogicalOperator = "ao";
        public const string Field = "fid";
        public const string Operator = "op";
        public const string Value = "val";
        public const string Query = "query";
        public const string Clauses = "clauses";
        public const string Tenant = "tenantId";
        public const string DomainContext = "domainContext";
        public const string ExecuteQuery = "execute";
        public const string ControlTypeId = "controlTypeId";
        public const string FieldValueTypes = "fieldValueTypes";
        public const string VisitorTimeZone = "visitorTimeZone";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class FormatStrings
      {
        public const string IncidentDateTime = "MM/dd/yy hh:mm:ss";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class Regex
      {
        public static readonly System.Text.RegularExpressions.Regex Today = new System.Text.RegularExpressions.Regex("^@today(\\s*([+-])\\s*(\\d+)|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly System.Text.RegularExpressions.Regex DateTime = new System.Text.RegularExpressions.Regex("(^\\d{1,2}/\\d{1,2}/\\d{2,4}\\s*(\\d{2}:\\d{2}:\\d{2})?$)|(^\\d{4}-\\d{1,2}-\\d{1,2}\\s*(\\d{2}:\\d{2}:\\d{2})?$)", RegexOptions.Compiled);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class Incidents
    {
      public const string DescriptionTrimmedNoticeMessage = "Description trimmed by ICM (Cause: Too much description content)";
      public const string ThrottlingIncidentDescription = "This connector's new incident creation requests have gone over the allowed limit and entered a throttling state in order of preventing a flood. <br/>For more information about connector throttling go to <a href=\"https://microsoft.sharepoint.com/teams/WAG/EngSys/IncidentManagement/IcM%20User%20Guide/Throttling.aspx\">https://microsoft.sharepoint.com/teams/WAG/EngSys/IncidentManagement/IcM%20User%20Guide/Throttling.aspx</a>";
      public const string ThrottlingIncidentUpdateDescriptionFormat = "This connector's incident update requests have gone over the allowed limit and entered a throttling state in order of preventing a flood. <br/>For more information about connector throttling go to <a href=\"https://microsoft.sharepoint.com/teams/WAG/EngSys/IncidentManagement/IcM%20User%20Guide/Throttling.aspx\">https://microsoft.sharepoint.com/teams/WAG/EngSys/IncidentManagement/IcM%20User%20Guide/Throttling.aspx</a><br/> <br/>Please go to the below link to view incident updates that you have recently posted to find why you have been throttled. <br/><a href=\"https://jarvis-west.dc.ad.msft.net/?page=logs&be=DGrep&time={0}&offset=-2&offsetUnit=Minutes&UTC=true&ep=Diagnostics%20PROD&ns=IcmProdIMP&en=ConnectorIncidentSummary&conditions=[[%22ThrottlingKey%22,%22%3d%3d%22,%22{1}%22]]\">https://jarvis-west.dc.ad.msft.net/?page=logs&be=DGrep&time={0}&offset=-2&offsetUnit=Minutes&UTC=true&ep=Diagnostics PROD&ns=IcmProdIMP&en=ConnectorIncidentSummary&conditions=[[\"ThrottlingKey\",\"=\",\"{1}\"]]</a> <br/>If you do not have permission please go to <a href=\"https://idweb/identitymanagement/default.aspx\">https://idweb/identitymanagement/default.aspx</a> and request to join the \"Connector Incident Viewers\" security group";
      public const string MonitorThrottlingIncidentDescriptionFormat = "We have identified a faulty monitor <span style=\"font-weight: bold;\">'{0}'</span> that has exceeded the allowed limit of 10 repetitions of over 100 requests per minute in the past 10 minute and hence we have throttled this monitor for the next 10 minutes. This monitor has been throttled to ensure that your other monitors are not starved during this time. This monitor will be automatically unthrottled after 10 minutes. To learn more about the monitor throttling in IcM, refer to the monitor throttling section in IcM documentation: <a href=\"https://microsoft.sharepoint.com/teams/WAG/EngSys/IncidentManagement/IcM%20User%20Guide/Throttling.aspx\">https://microsoft.sharepoint.com/teams/WAG/EngSys/IncidentManagement/IcM%20User%20Guide/Throttling.aspx</a><br/> <br/>Following links to the incident data in IcM and Jarvis will help you identify and resolve the issues with the faulty monitor and thereby avoid being throttled further.<br/> <br/> 1. Use the following link in IcM to view the incidents created recently by this monitor. You can use this data to view the similar incidents created by the same monitor in the past and identify the monitor.<br/><a href=\"https://{1}/imp/v3/incidents/omnisearch?searchString={0}\">https://{1}/imp/v3/incidents/omnisearch?searchString={0}</a> <br/><br/> 2. Use the following link in Jarvis to view the raw incident data for the incidents that were recently created by this monitor. This includes the throttled incident data and will help you determine what is being throttled.<br/><a href=\"https://jarvis-west.dc.ad.msft.net/?page=logs&be=DGrep&time={2}&offset=-2&offsetUnit=Minutes&UTC=true&ep=Diagnostics%20PROD&ns=IcmProdIMP&en=ConnectorIncidentSummary&conditions=[[%22ThrottlingKey%22,%22%3d%3d%22,%22{3}%22]]\">https://jarvis-west.dc.ad.msft.net/?page=logs&be=DGrep&time={2}&offset=-2&offsetUnit=Minutes&UTC=true&ep=Diagnostics PROD&ns=IcmProdIMP&en=ConnectorIncidentSummary&conditions=[[\"ThrottlingKey\",\"=\",\"{3}\"]]</a> <br/><br/><span style=\"font-weight: bold;\">NOTE:</span> If you do not have permission to view this page, go to <a href=\"https://idweb/identitymanagement/default.aspx\">https://idweb/identitymanagement/default.aspx</a> and request to join the \"Connector Incident Viewers\" security group.";
      public const string ThrottlingGlobalIncidentDescription = "<br/> <br/>Please go to the below link to view incidents that you have created recently to find why you have been throttled. <br/><a href=\"https://jarvis-west.dc.ad.msft.net/?page=logs&be=DGrep&time={0}&offset=-2&offsetUnit=Minutes&UTC=true&ep=Diagnostics%20PROD&ns=IcmProdIMP&en=ConnectorIncidentSummary&conditions=[[%22ThrottlingKey%22,%22%3d%3d%22,%22{1}%22]]\">https://jarvis-west.dc.ad.msft.net/?page=logs&be=DGrep&time={0}&offset=-2&offsetUnit=Minutes&UTC=true&ep=Diagnostics PROD&ns=IcmProdIMP&en=ConnectorIncidentSummary&conditions=[[\"ThrottlingKey\",\"=\",\"{1}\"]]</a> <br/>If you do not have permission please go to <a href=\"https://idweb/identitymanagement/default.aspx\">https://idweb/identitymanagement/default.aspx</a> and request to join the \"Connector Incident Viewers\" security group";
      public const string DefaultSourceOrigin = "Other";
      public const string IncidentLinkTargetTypeIcM = "IcM";
      public const string IncidentLinkTargetTypeTfs = "Tfs";
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly IList<IncidentStatus> StatusList = (IList<IncidentStatus>) new ReadOnlyCollection<IncidentStatus>((IList<IncidentStatus>) new IncidentStatus[4]
      {
        IncidentStatus.Active,
        IncidentStatus.Resolved,
        IncidentStatus.Mitigating,
        IncidentStatus.Mitigated
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly IList<IncidentStatus> SuppressedStatusList = (IList<IncidentStatus>) new ReadOnlyCollection<IncidentStatus>((IList<IncidentStatus>) new IncidentStatus[2]
      {
        IncidentStatus.Suppressed,
        IncidentStatus.Resolved
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly ReadOnlyCollection<string> IncidentSubTypeList = new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "Availability",
        "Capacity",
        "Functionality Loss",
        "Node Fault",
        "Performance",
        "Root Cause",
        "Security",
        "Telemetry",
        "BuildOut",
        "Privacy",
        "Other"
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly ReadOnlyCollection<string> HowFixedList = new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "By Design",
        "Customer Error",
        "External",
        "False Alarm",
        "Transient",
        "Fixed with Ad-Hoc steps",
        "Fixed with TSG",
        "Fixed with Hotfix",
        "Fixed By Automation",
        "Unable To Reproduce",
        "Won't Fix",
        "Other",
        "False Alarm/Transient"
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly ReadOnlyCollection<string> SourceOriginList = new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "Monitor",
        "Customer",
        "Manual",
        "Partner",
        "Performance Counter",
        "Runner",
        "Forum/DL",
        "Deployment",
        "Workflow",
        "Email",
        "Other"
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is readonly")]
      public static readonly ReadOnlyCollection<string> IncidentLinkTargetTypeList = new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "IcM",
        "Tfs"
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class Environments
    {
      public const string Dogfood = "DOGFOOD";
      public const string Prod = "PROD";
      public const string PPE = "PPE";
      public const string Int = "INT";
      public const string Staging = "STAGING";
      public const string Test = "TEST";
      public const string Other = "OTHER";
      public const string Lx = "LX";
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
      public static readonly IList<string> EnvironmentList = (IList<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
      {
        "DOGFOOD",
        "INT",
        nameof (PPE),
        "PROD",
        "STAGING",
        "TEST"
      });
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Uses read-only wrapper")]
      public static readonly IDictionary<string, string> EnvironmentMap = (IDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
      {
        {
          "Production",
          "PROD"
        },
        {
          "GRN001",
          "PROD"
        },
        {
          "GRN002",
          "PROD"
        },
        {
          "ME",
          "PROD"
        },
        {
          "AAD_ACS_CHINA",
          "PROD"
        },
        {
          "AAC_ACS_CHINA_2",
          "PROD"
        },
        {
          "AAC_ACS_CHINA_3",
          "PROD"
        },
        {
          "AAD_ACS_PROD",
          "PROD"
        },
        {
          "AAD_ESTS_PROD",
          "PROD"
        },
        {
          "AAD_ESTS_PROD_2",
          "PROD"
        },
        {
          "PHX",
          "PROD"
        },
        {
          "GALLATIN",
          "PROD"
        },
        {
          "GRN002.MSO.CN.GBL",
          "PROD"
        },
        {
          "PRODFYI",
          "PROD"
        },
        {
          "Prd",
          "PROD"
        },
        {
          "eDog",
          "DOGFOOD"
        },
        {
          "DF",
          "DOGFOOD"
        },
        {
          "PPE/eDog",
          nameof (PPE)
        },
        {
          "eDog/PPE",
          nameof (PPE)
        },
        {
          "GRNPPE",
          nameof (PPE)
        },
        {
          "AAD_ACS_PPE",
          nameof (PPE)
        },
        {
          "Stage",
          "STAGING"
        },
        {
          "STAGEFYI",
          "STAGING"
        },
        {
          "Testing",
          "TEST"
        }
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class BugProperties
    {
      public const string Id = "id";
      public const string BugType = "type";
      public const string BugSource = "src";
      public const string BugDescription = "repairDescription";
      public const string BugId = "bugId";
      public const string BugOwner = "owner";
      public const string BugDelivery = "delivery";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class BugFieldNames
    {
      public const string BugType = "Type";
      public const string BugSource = "Source";
      public const string BugDescription = "Description";
      public const string BugId = "Bud ID";
      public const string BugOwner = "Owner";
      public const string BugDelivery = "Delivery";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class PirResponseTimelineFields
    {
      public const string ImpactStart = "ImpactStart";
      public const string Detection = "Detection";
      public const string Triage = "Triage";
      public const string EngEngaged = "EngEngaged";
      public const string CommsEngaged = "CommsEngaged";
      public const string FirstCustomerAdvisory = "FirstCustomerAdvisory";
      public const string DetailedCustomerAdvisory = "DetailedCustomerAdvisory";
      public const string Dashboard = "Dashboard";
      public const string Mitigation = "Mitigation";
      public const string Diagnosis = "Diagnosis";
      public const string Recovery = "Recovery";
      public const string Other1 = "Other1";
      public const string Other2 = "Other2";
      public const string Other3 = "Other3";
      public const string OutageDeclaredDate = "OutageDeclaredDate";
      public const string ImpactStartDescription = "ImpactStartDescription";
      public const string DetectionDescription = "DetectionDescription";
      public const string TriageDescription = "TriageDescription";
      public const string EngEngagedDescription = "EngEngagedDescription";
      public const string CommsEngagedDescription = "CommsEngagedDescription";
      public const string FirstCustomerAdvisoryDescription = "FirstCustomerAdvisoryDescription";
      public const string DetailedCustomerAdvisoryDescription = "DetailedCustomerAdvisoryDescription";
      public const string DashboardDescription = "DashboardDescription";
      public const string MitigationDescription = "MitigationDescription";
      public const string DiagnosisDescription = "DiagnosisDescription";
      public const string RecoveryDescription = "RecoveryDescription";
      public const string Other1Description = "Other1Description";
      public const string Other2Description = "Other2Description";
      public const string Other3Description = "Other3Description";
      public const string OutageDeclaredDescription = "OutageDeclaredDescription";
      public const string ReadOnly = "ReadOnly";
      public const string PirResponseTimeline = "PirResponseTimeline";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class MiscFormatStrings
    {
      public const string AckFormatString = "===== {0} Assigned by {1} to {1} =====";
      public const string TenantIdFormatString = "N";
      public const string HyperlinkFormat = "<a href=\"{0}\" target=\"_blank\">{1}</a>";
      public const string TeamSuffixFormat = "{0}-{1}(NotifyGrp)";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class ResourceAccess
    {
      public const int PastRequestDays = 30;
      public const int MaxExpirationDurationHours = 168;
      public const string DefaultRejectMessage = "Your request has been rejected.";
      public const string NotificationPrefix = "RESOURCEACCESS";
      public const string IncidentTitleFormat = "Error processing resource access request {0} with status {1}";
      public const string PendingErrMsgFormat = "Illegal 'pending' status returned from call to CheckAccess. Additional information: {0}.";
      public const string UserNotInGroupFormat = "The user {0} is no longer in the group {1}.";
      public const string TaskName = "ResourceAccessRequestStateManager";
      public const string RotationActiveState = "Active";

      public enum Status
      {
        New,
        PendingApproval,
        PendingRevocation,
        Approved,
        Rejected,
        Expired,
        Revoked,
        FailedApproval,
        FailedRevocation,
      }

      public enum Provider
      {
        SecurityGroupAccessProvider = 1,
        MockResourceAccessProvider = 2,
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
      public static class ResourceAccessProviderAssembly
      {
        public const string SecurityGroup = "Microsoft.AzureAd.Icm.JustInTimeAccessProvider";
        public const string Mock = "Microsoft.AzureAd.Icm.MockResourceAccessProvider";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant classes")]
      public static class ResourceAccessProviderType
      {
        public const string SecurityGroup = "Microsoft.AzureAd.Icm.ResourceAccessProviders.JustInTimeAccessProvider";
        public const string Mock = "Microsoft.AzureAd.Icm.ResourceAccessProviders.MockResourceAccessProvider";
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class ExceptionCategoryTofuTagIds
    {
      public static readonly int IcmAccessDeniedException = 808939636;
      public static readonly int HttpRequestValidationException = 808939832;
      public static readonly int SecurityTokenValidationException = 808939833;
      public static readonly int InvalidCookieException = 808939880;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class DataCollectionTagIds
    {
      public const int ConnectorWebServiceApiCall = 809006391;
      public const int RulesWebServiceApiCall = 809006392;
      public const int IncidentSubmit = 809006390;
      public const int OnCallWebServiceApiCall = 811689071;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class Instrumentation
    {
      public const string OpMessage = "[OpType: {0} | User: {1} | Tenant: {2} | Time: {3}]";
      public const string OpDurationMessage = "[OpType: {0} | Duration: {1} | Message : {2}]";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class NotificationProfile
    {
      public const short MinContactWaitTimeInMinutes = 3;
      public const short MaxContactWaitTimeInMinutes = 20;
      public const short DefaultContactWaitTimeInMinutes = 9;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Approved")]
    public static class OnCall
    {
      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class Constants
      {
        public const int MaxAllowedShifts = 5;
        public const int MaxAllowedBackups = 6;
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class ParameterNames
      {
        public const string ShiftId = "ShiftId";
        public const string ShiftName = "ShiftName";
        public const string ShiftDuration = "ShiftDuration";
        public const string RotationList = "RotationList";
        public const string ContactsIds = "ContactIds";
        public const string BackupContactsCount = "BackupContactsCount";
        public const string ShiftsEnabled = "ShiftsEnabled";
        public const string RotationType = "RotationType";
        public const string RotationStartTime = "RotationStartTime";
        public const string RotationTimeZone = "RotationTimeZone";
        public const string ShiftGroupId = "GroupId";
        public const string ShiftGroupName = "GroupName";
        public const string ShiftGroupDeprecated = "GroupDeprecated";
        public const string ReferencedTeamsCount = "TeamsCount";
        public const string ShiftsList = "Shifts";
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class HtmlTags
    {
      public const string NewLine = "<br />";
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class VoiceConfiguration
    {
      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class Defaults
      {
        public const int SmsVoiceSeparationDelay = 0;
        public const int MinsBetweenCalls = 3;
        public const int MaxCallsPeriod = 60;
        public const int MaxAgeMins = 60;
        public const int MaxSeverity = 1;
        public const int MaxCallsPerPeriodSpecial = 50;
        public const int MaxCallsPerPeriod = 5;
        public const short DisableVoiceCallsSeverity = -1;
        public const string NewIncidentMessage = "A severity {MSG-PARAMETER:Severity} alert has been raised for the {MSG-PARAMETER:Team} area. The incident number is {MSG-PARAMETER:Id}. Please respond to prevent further phone calls. For more details about this incident see http://{MSG-PARAMETER:WebServerDnsName}.";
        public const string AssistanceRequestMessage = "A severity {MSG-PARAMETER:Severity} alert has been raised by the and your assistance is requested to help investigate the issue. The incident number is {MSG-PARAMETER:Id}. Please respond to prevent further phone calls. For more details about this escalation see http://{MSG-PARAMETER:WebServerDnsName}.";
        public const string AutoInviteMessage = "A severity {MSG-PARAMETER:Severity} alert has been raised and your assistance has been automatically requested to help investigate the issue. The incident number is {MSG-PARAMETER:Id}. Please respond to prevent further phone calls. For more details about this escalation see http://{MSG-PARAMETER:WebServerDnsName}.";
        public const string UpgradeMessage = "An incident has been upgraded to severity {MSG-PARAMETER:Severity} for the {MSG-PARAMETER:Team}  area.  The incident number is {MSG-PARAMETER:Id}. Please respond to prevent further phone calls. For more details about this escalation see https://{MSG-PARAMETER:WebServerDnsName}";
        public const string NewIncidentSubject = "On-call alert severity {MSG-PARAMETER:Severity} notification for incident {MSG-PARAMETER:Id} assigned to team {MSG-PARAMETER:Team}";
        public const string AssistanceRequestSubject = "Request for assistance for severity {MSG-PARAMETER:Severity} incident {MSG-PARAMETER:Id} investigation";
        public const string AutoInviteSubject = "Automatic request for assistance for severity {MSG-PARAMETER:Severity} incident {MSG-PARAMETER:Id} investigation";
        public const string UpgradeSubject = "Incident {MSG-PARAMETER:Id} upgraded to {MSG-PARAMETER:Severity} assigned to team {MSG-PARAMETER:Team}";
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
    public static class PerformanceMetrics
    {
      public const char MetricNameReplacementCharacter = '_';
      private static readonly ReadOnlyCollection<char> MetricNameForbiddenChars = new ReadOnlyCollection<char>((IList<char>) new List<char>((IEnumerable<char>) new char[4]
      {
        '\\',
        '/',
        ':',
        '?'
      }));

      public static ReadOnlyCollection<char> MetricNameForbiddenCharacters => IcmConstants.PerformanceMetrics.MetricNameForbiddenChars;

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class ConfigurationElements
      {
        public const string MetricsSection = "MetricsSection";
        public const string MetricsComponent = "Component";
        public const string MetricsEvent = "Event";
        public const string Metric = "Metric";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class CommonConfigurationAttributes
      {
        public const string Name = "Name";
      }

      [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Nested constant objects")]
      public static class MetricConfigurationAttributes
      {
        public const string ReportedName = "ReportedName";
        public const string Multiplier = "Multiplier";
      }
    }

    public static class EntityOperations
    {
      public static class Incidents
      {
        public const string LinkRootCause = "LinkRootCause";
        public const string ClearRootCause = "ClearRootCause";
        public const string Edit = "Edit";
        public const string Invite = "Invite";
        public const string Transfer = "Transfer";
        public const string Mitigate = "Mitigate";
        public const string Resolve = "Resolve";
        public const string LinkIncident = "Link";
        public const string TrackIncident = "Track";
        public const string Acknowledge = "Acknowledge";
        public const string AcknowledgeIncidentNotification = "AcknowledgeIncidentNotification";
        public const string Reactivate = "Reactivate";
        public const string RequestAssistance = "RequestAssistance";
        public const string Unresolve = "Unresolve";
        public const string AddPir = "AddPir";
        public const string AddIncidentRelationship = "AddIncidentRelationship";
        public const string RemoveIncidentRelationship = "RemoveIncidentRelationship";
        public const string AddExternalItemRelationship = "AddExternalItemRelationship";
        public const string RemoveExternalItemRelationship = "RemoveExternalItemRelationship";
      }

      public static class RootCauses
      {
        public const string Create = "Create";
        public const string Edit = "Edit";
        public const string Link = "Link";
      }
    }

    public class ConfigKeyNames
    {
      public const string MaxDescriptionLength = "MaxDescriptionLength";
      public const string MaxDescriptionEntriesCount = "MaxDescriptionEntriesCount";
    }

    public class AzureStorageCategories
    {
      public const long AimsConnectorAccounts = 1;
      public const long AttachmentsAccounts = 2;
      public const long ActivityAttachmentsAccounts = 3;
    }

    public static class SerializationTypes
    {
      public static class FieldNames
      {
        public static class Acknowledgement
        {
          public const string IsAcknowledged = "IsAcknowledged";
          public const string AcknowledgeDate = "AcknowledgeDate";
          public const string AcknowledgeContactAlias = "AcknowledgeContactAlias";
        }

        public static class Incident
        {
          public const string IsOutage = "IsOutage";
          public const string HealthResourceId = "HealthResourceId";
          public const string DiagnosticsLink = "DiagnosticsLink";
          public const string ChangeList = "ChangeList";
          public const string IncidentManagerContactId = "IncidentManagerContactId";
          public const string CommunicationsManagerContactId = "CommunicationsManagerContactId";
          public const string SiteReliabilityContactId = "SiteReliabilityContactId";
          public const string SiloId = "SiloId";
          public const string Id = "Id";
          public const string CreateDate = "CreateDate";
          public const string ModifiedDate = "ModifiedDate";
          public const string Severity = "Severity";
          public const string Status = "Status";
          public const string Source = "Source";
          public const string CorrelationId = "CorrelationId";
          public const string RoutingId = "RoutingId";
          public const string HitCount = "HitCount";
          public const string RaisingLocation = "RaisingLocation";
          public const string IncidentLocation = "IncidentLocation";
          public const string OwningTenantId = "OwningTenantId";
          public const string OwningTeamId = "OwningTeamId";
          public const string OwningContactAlias = "OwningContactAlias";
          public const string MitigationData = "MitigationData";
          public const string MitigateTime = "MitigateTime";
          public const string ResolutionData = "ResolutionData";
          public const string ResolveData = "ResolveData";
          public const string IsCustomerImpacting = "IsCustomerImpacting";
          public const string IsNoise = "IsNoise";
          public const string IsSecurityRisk = "IsSecurityRisk";
          public const string Title = "Title";
          public const string Description = "Description";
          public const string ReproSteps = "ReproSteps";
          public const string TsgId = "TsgId";
          public const string Component = "Component";
          public const string CustomerName = "CustomerName";
          public const string AssignedTo = "AssignedTo";
          public const string CommitDate = "CommitDate";
          public const string Keywords = "Keywords";
          public const string DescriptionEntries = "DescriptionEntries";
          public const string NewDescriptionEntry = "NewDescriptionEntry";
          public const string AcknowledgementData = "AcknowledgementData";
          public const string ImpactStartDate = "ImpactStartDate";
          public const string ImpactStartTime = "ImpactStartTime";
          public const string IncidentType = "IncidentType";
          public const string OriginatingTenantId = "OriginatingTenantId";
          public const string ParentIncidentId = "ParentIncidentId";
          public const string RelatedLinksCount = "RelatedLinksCount";
          public const string ExternalLinksCount = "ExternalLinksCount";
          public const string LastCorrelationDate = "LastCorrelationDate";
          public const string ChildCount = "ChildCount";
          public const string ReactivationData = "ReactivationData";
          public const string CustomFieldGroups = "CustomFieldGroups";
          public const string SubscriptionId = "SubscriptionId";
          public const string SupportTicketId = "SupportTicketId";
          public const string MonitorId = "MonitorId";
          public const string IncidentSubType = "IncidentSubType";
          public const string HowFixed = "HowFixed";
          public const string TsgOutput = "TsgOutput";
          public const string SourceOrigin = "SourceOrigin";
          public const string ResponsibleTenantId = "ResponsibleTenantId";
          public const string ResponsibleTeamId = "ResponsibleTeamId";
          public const string ImpactedServicesIds = "ImpactedServicesIds";
          public const string ImpactedTeamsPublicIds = "ImpactedTeamsPublicIds";
          public const string ImpactedComponents = "ImpactedComponents";
          public const string RootCause = "RootCause";
          public const string ParentIncident = "ParentIncident";
          public const string ChildIncidents = "ChildIncidents";
          public const string RelatedIncidents = "RelatedIncidents";
          public const string ExternalIncidents = "ExternalIncidents";
          public const string ResponsibleIncidents = "ResponsibleIncidents";
          public const string Bridges = "Bridges";
          public const string Type = "Type";
          public const string SubType = "SubType";
          public const string LastCorrelationTime = "LastCorrelationTime";
          public const string CloudInstanceId = "CloudInstanceId";
          public const string TsgLink = "TsgLink";
          public const string TsgInfo = "TsgInfo";
          public const string Summary = "Summary";
        }

        public static class MitigationData
        {
          public const string Date = "Date";
          public const string ChangedBy = "ChangedBy";
          public const string Mitigation = "Mitigation";
        }

        public static class ResolutionData
        {
          public const string Date = "Date";
          public const string ChangedBy = "ChangedBy";
          public const string CreatePostmortem = "CreatePostmortem";
        }

        public static class ReactivationData
        {
          public const string DisableVoiceNotifications = "DisableVoiceNotifications";
        }

        public static class BridgeData
        {
          public const string BridgeURI = "BridgeURI";
          public const string Id = "Id";
          public const string BridgeConfId = "BridgeConfId";
          public const string ExpirationDate = "ExpirationDate";
          public const string PhoneNumber = "PhoneNumber";
        }

        public static class CustomFields
        {
          public const string ShortString = "SSTR";
          public const string BigString = "BSTR";
          public const string EnumValue = "ENUM";
          public const string DateTimeValue = "DATE";
          public const string IntegerValue = "BINT";
          public const string RichText = "RTTR";
          public const string BooleanValue = "BOOL";
          public static readonly IDictionary<string, IncidentCustomFieldType> CustomFieldTypeMapping = (IDictionary<string, IncidentCustomFieldType>) new Dictionary<string, IncidentCustomFieldType>()
          {
            {
              "SSTR",
              IncidentCustomFieldType.ShortString
            },
            {
              "BSTR",
              IncidentCustomFieldType.BigString
            },
            {
              "ENUM",
              IncidentCustomFieldType.Enum
            },
            {
              "DATE",
              IncidentCustomFieldType.DateTime
            },
            {
              "BINT",
              IncidentCustomFieldType.Integer
            },
            {
              "RTTR",
              IncidentCustomFieldType.RichText
            },
            {
              "BOOL",
              IncidentCustomFieldType.Boolean
            }
          };
          public static readonly IDictionary<IncidentCustomFieldType, string> CustomFieldDatabaseTypeMapping = (IDictionary<IncidentCustomFieldType, string>) new Dictionary<IncidentCustomFieldType, string>()
          {
            {
              IncidentCustomFieldType.ShortString,
              "SSTR"
            },
            {
              IncidentCustomFieldType.BigString,
              "BSTR"
            },
            {
              IncidentCustomFieldType.Enum,
              "ENUM"
            },
            {
              IncidentCustomFieldType.DateTime,
              "DATE"
            },
            {
              IncidentCustomFieldType.Integer,
              "BINT"
            },
            {
              IncidentCustomFieldType.RichText,
              "RTTR"
            },
            {
              IncidentCustomFieldType.Boolean,
              "BOOL"
            }
          };
        }
      }
    }

    public static class ExcelExportImport
    {
      public static class RoutingRules
      {
        public const string DefaultRulesSheetName = "Default Routing Rules";
        public const string RulesSheetName = "Routing Rules";
        public const string DeletedRulesSheetName = "Deleted Routing Rules";

        public static class ColumnNames
        {
          public const string OwningTenant = "Owning Service";
          public const string DefaultSeverity = "Default Severity";
          public const string EnforceDefaultSeverity = "Enforce Default Severity";
          public const string EscalateToTenantName = "Escalate to Service";
          public const string EscalateToTeamName = "Escalate to Team";
          public const string OverrideTeam = "Override Team";
          public const string PrefixMatching = "Prefix Matching";
          public const string CategoryId = "Category Id";
          public const string Description = "Description";
          public const string LastModifiedTime = "Last Modified";
          public const string ModifiedBy = "Modified By";
        }

        public static class ValidationErrors
        {
          public const string CouldNotLocateSiloFmt = "Could not locate silo with name: ";
          public const string FoundMultipleSilosFmt = "Found more than one silo with name: ";
          public const string FoundMultipleTeamsFmt = "Found more than one active team with name: ";
          public const string CouldNotLocateServiceFmt = "Could not locate service with name: ";
          public const string CouldNotLocateTeamFmt = "Could not locate an active team with name: ";
          public const string RulesMustBelongToSingleServiceFmt = "Rules must be imported for a single service at a time. Detected services: ";
          public const string ServiceNameRequiredForEveryRule = "Service name must be populated for every entry.";
          public const string NoDefaultRuleFound = "Could not locate existing default routing rule";
          public const string MoreThanOneDefaultRuleExists = "Found more than one existing default routing rule";
          public const string NoRulesFoundInSheet = "Could not find any rules to process from Excel sheet.";
          public const string NoRulesToProcess = "Rules in sheet are identical to the existing rules. No changes detected.";
          public const string OnlyOneDefaultRule = "Detected more than one default rule in the spreadsheet.";
          public const string DuplicateRulesFoundFmt = "Detected duplicate rules in sheet '{0}' with condition: {1}";
        }
      }
    }

    public static class ValidationConstants
    {
      public static readonly ReadOnlyCollection<string> AcceptedEmailDomains = new ReadOnlyCollection<string>((IList<string>) new string[2]
      {
        "microsoft.com",
        "21vianet.com"
      });
    }
  }
}
