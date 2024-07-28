// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTracingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Components;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.WebPlatform;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationTracingService : IVssFrameworkService
  {
    public static readonly Guid TracingServiceGuid = new Guid("{565ACAC7-5B24-40BE-8F9E-0CA0D7E9E3AF}");
    private bool m_updateRawTraces;
    private static EuiiDetectionService s_euiiDetectionService;
    private int m_maxTracesPerTraceFilter;
    private bool m_monitorTracesPerTraceFilter;
    private Microsoft.VisualStudio.Services.WebApi.TraceFilter[] m_traces;
    private ConcurrentDictionary<Guid, TeamFoundationTracingService.ThreadSafeCounter> m_traceCounts;
    private readonly ConcurrentDictionary<Guid, bool>[] m_userImpactedTime = new ConcurrentDictionary<Guid, bool>[60];
    internal static Microsoft.VisualStudio.Services.WebApi.TraceFilter[] s_rawTraces = Array.Empty<Microsoft.VisualStudio.Services.WebApi.TraceFilter>();
    private static readonly TeamFoundationTracingService.ThreadSafeCounter s_lowPriorityTraceCounter = new TeamFoundationTracingService.ThreadSafeCounter();
    private bool m_servicingStepDetailEnabled = true;
    private bool m_jobAndActivityLogTracingEnabled = true;
    private bool m_surveyEventTracingEnabled = true;
    private static bool s_enableUserImpactTracing;
    private static bool s_euiiAssertOnDetection;
    private static bool s_euiiGatesEnabled;
    private static bool s_euiiGatesFeatureFlagEnabled;
    internal static bool s_collectTracePublishedVsSkippedMetricEnabled;
    internal static bool s_enableLowPriorityProductTrace;
    internal static int s_lowPriorityProductTracePercentage = 100;
    internal static TraceLevel s_lowPriorityProductTraceMaxLevel = TraceLevel.Info;
    private static bool s_enableActivityLogMapping = true;
    private static readonly TeamFoundationTracingService.TraceProvider s_eventProvider;
    private const string c_tracingService = "TracingService";
    private const string c_serviceLayer = "IVssFrameworkService";
    private const string c_euiiDetectionService = "EuiiDetectionService";
    private const string c_tracingServiceLayer = "TeamFoundationTracingService";
    private static readonly string s_processName;
    private static readonly sbyte s_webSiteId;
    private static readonly byte[] c_byteArray = new byte[8];
    private static readonly string[] s_tagPerformance = new string[1]
    {
      "performance"
    };
    internal const string ProviderGuid = "{80761876-6844-47D5-8106-F8ED2AA8687B}";
    private static readonly TraceFilterEqualityComparer s_traceFilterEqualityComparer = new TraceFilterEqualityComparer();
    internal static readonly DateTime s_zeroDate = DateTime.SpecifyKind(new DateTime(1601, 1, 1), DateTimeKind.Utc);
    internal const int c_maxTextLength = 8192;
    private static readonly RegistryQuery[] s_notificationFilters = new RegistryQuery[11]
    {
      new RegistryQuery(FrameworkServerConstants.TracingServiceJobAndActivityLogTracingEnabled),
      new RegistryQuery(FrameworkServerConstants.TracingServiceMaxTracesPerTraceFilter),
      new RegistryQuery(FrameworkServerConstants.TracingServiceServicingStepDetailEnabled),
      new RegistryQuery(FrameworkServerConstants.TracingServiceSurveyEventsTracingEnabled),
      new RegistryQuery(FrameworkServerConstants.TracingServiceEuiiGatesEnabled),
      new RegistryQuery(FrameworkServerConstants.TracingServiceEuiiAssertOnDetection),
      new RegistryQuery(FrameworkServerConstants.TracingServiceEnableActivityLogMapping),
      new RegistryQuery(FrameworkServerConstants.TracingServiceEnableCollectTracePublishedVsSkippedMetric),
      new RegistryQuery(FrameworkServerConstants.TracingServiceEnableLowPriorityProductTrace),
      new RegistryQuery(FrameworkServerConstants.TracingServiceLowPriorityProductTracePercentage),
      new RegistryQuery(FrameworkServerConstants.TracingServiceLowPriorityProductTraceMaxLevel)
    };
    private static readonly string[] s_mdmDimensions = new string[1]
    {
      "Tracepoint"
    };

    public virtual bool TraceDatabaseDetails(
      Guid executionId,
      int databaseId,
      string serverName,
      string databaseName,
      long version,
      string serviceLevel,
      string poolName,
      int poolMaxDatabaseLimit,
      int tenants,
      int maxTenants,
      string status,
      string statusReason,
      DateTime statusChangedDate,
      TeamFoundationDatabaseFlags flags,
      string minServiceObjective,
      string maxServiceObjective,
      int retentionDays,
      string connectionString,
      DateTime createdOn,
      string serviceObjective,
      string backupStorageRedundancy,
      bool isZoneRedundant,
      string collation,
      string location,
      string defaultSecondaryLocation,
      string readScale,
      int highAvailabilityReplicaCount,
      double maxSizeInGB,
      double maxLogSizeInGB,
      string kind)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref serviceLevel);
      TeamFoundationTracingService.NormalizeString(ref poolName);
      TeamFoundationTracingService.NormalizeString(ref status);
      TeamFoundationTracingService.NormalizeString(ref statusReason);
      TeamFoundationTracingService.NormalizeString(ref minServiceObjective);
      TeamFoundationTracingService.NormalizeString(ref maxServiceObjective);
      TeamFoundationTracingService.NormalizeDateTime(ref statusChangedDate);
      TeamFoundationTracingService.NormalizeString(ref connectionString);
      TeamFoundationTracingService.NormalizeDateTime(ref createdOn);
      try
      {
        if (TeamFoundationTracingService.s_eventProvider != null)
          return TeamFoundationTracingService.s_eventProvider.TraceDatabaseDetails(executionId, databaseId, serverName, databaseName, version, serviceLevel, poolName, poolMaxDatabaseLimit, tenants, maxTenants, status, statusReason, statusChangedDate, flags.ToString().Replace(", ", "|"), minServiceObjective, maxServiceObjective, retentionDays, connectionString, createdOn, serviceObjective ?? "", backupStorageRedundancy ?? "", isZoneRedundant, collation ?? "", location ?? "", defaultSecondaryLocation ?? "", readScale ?? "", highAvailabilityReplicaCount, maxSizeInGB, maxLogSizeInGB, kind ?? "");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceDatabaseDetails), nameof (TeamFoundationTracingService), ex);
      }
      return false;
    }

    public virtual bool TraceOrganizationTenant(
      Guid hostId,
      int hostType,
      string hostName,
      Guid? parentHostId,
      int? parentHostType,
      string parentHostName,
      Guid? tenantId,
      DateTime? tenantLastModified,
      string preferredRegion)
    {
      TeamFoundationTracingService.NormalizeString(ref hostName);
      TeamFoundationTracingService.NormalizeString(ref parentHostName);
      DateTime universalTime = tenantLastModified.GetValueOrDefault().ToUniversalTime();
      TeamFoundationTracingService.NormalizeDateTime(ref universalTime);
      try
      {
        if (TeamFoundationTracingService.s_eventProvider != null)
          return TeamFoundationTracingService.s_eventProvider.TraceOrganizationTenant(hostId, (byte) hostType, hostName, parentHostId.GetValueOrDefault(), (byte) parentHostType.GetValueOrDefault(), parentHostName, tenantId.GetValueOrDefault(), universalTime, preferredRegion);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationTenant), nameof (TeamFoundationTracingService), ex);
      }
      return false;
    }

    public static void TraceAccountUserLicensingChanges(
      Guid accountID,
      Guid parentHostId,
      TeamFoundationHostType hostType,
      Guid userID,
      Guid userCUID,
      int userStatus,
      int? licenseSourceId,
      int? licenseId,
      string userStatusName,
      string licenseSourceName,
      string licenseName,
      DateTime assignmentDate,
      DateTime dateCreated,
      DateTime modifiedDate,
      string changeType)
    {
      try
      {
        string str = (accountID == Guid.Empty ? "accountID " : string.Empty) + (userID == Guid.Empty ? nameof (userID) : string.Empty) + (modifiedDate == DateTime.MinValue ? nameof (modifiedDate) : string.Empty) + (string.IsNullOrEmpty(changeType) ? nameof (changeType) : string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountUserLicensingChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          TeamFoundationTracingService.NormalizeDateTime(ref assignmentDate);
          TeamFoundationTracingService.NormalizeDateTime(ref dateCreated);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              "ParentHostID",
              (object) parentHostId
            },
            {
              "HostType",
              (object) hostType
            },
            {
              "UserID",
              (object) userID
            },
            {
              "UserCUID",
              (object) userCUID
            },
            {
              "UserStatus",
              (object) userStatus
            },
            {
              "LicenseSourceId",
              (object) licenseSourceId
            },
            {
              "LicenseId",
              (object) licenseId
            },
            {
              "UserStatusName",
              (object) userStatusName
            },
            {
              "LicenseSourceName",
              (object) licenseSourceName
            },
            {
              "LicenseName",
              (object) licenseName
            },
            {
              "AssignmentDate",
              (object) assignmentDate
            },
            {
              "DateCreated",
              (object) dateCreated
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            },
            {
              "ChangeType",
              (object) changeType
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceLicensingDataFeed("AccountUserLicensingChanges", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountUserLicensingChanges), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceLicensingEvent(
      Guid hostId,
      Guid userId,
      string eventTypeFamily,
      string eventTypeDescriptor,
      object eventData)
    {
      try
      {
        string str = (hostId == new Guid() ? "hostId " : string.Empty) + (eventData == null ? "eventData " : string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceLicensingEvent), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              nameof (hostId),
              (object) hostId
            },
            {
              nameof (userId),
              (object) userId
            },
            {
              nameof (eventTypeFamily),
              (object) eventTypeFamily
            },
            {
              nameof (eventTypeDescriptor),
              (object) eventTypeDescriptor
            },
            {
              nameof (eventData),
              eventData
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceLicensingDataFeed("LicensingEvent", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceLicensingEvent), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceActiveLicenses(
      Guid organizationId,
      Guid userCuid,
      Guid userVsid,
      string license,
      DateTime assignmentDate,
      DateTime dateCreated,
      DateTime lastAccessedDate)
    {
      try
      {
        string str = (organizationId == Guid.Empty ? "organizationId " : string.Empty) + (userCuid == Guid.Empty ? "userCuid " : string.Empty) + (userVsid == Guid.Empty ? "userVsid " : string.Empty) + (string.IsNullOrWhiteSpace(license) ? "license " : string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceActiveLicenses), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          TeamFoundationTracingService.NormalizeDateTime(ref assignmentDate);
          TeamFoundationTracingService.NormalizeDateTime(ref dateCreated);
          TeamFoundationTracingService.NormalizeDateTime(ref lastAccessedDate);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "OrganizationID",
              (object) organizationId
            },
            {
              "UserCUID",
              (object) userCuid
            },
            {
              "UserVSID",
              (object) userVsid
            },
            {
              "License",
              (object) license
            },
            {
              "AssignmentDate",
              (object) assignmentDate
            },
            {
              "DateCreated",
              (object) dateCreated
            },
            {
              "LastAccessedDate",
              (object) lastAccessedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceLicensingDataFeed("ActiveLicenses", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceActiveLicenses), nameof (TeamFoundationTracingService), ex);
      }
    }

    internal bool TraceSurveyEvents(
      Guid uniqueIdentifier,
      string anonymousIdentifier,
      Guid tenantId,
      Guid hostId,
      Guid parentHostId,
      byte hostType,
      Guid vsid,
      Guid cuid,
      string area,
      string feature,
      string userAgent,
      string properties,
      string dataspaceType,
      string dataspaceId,
      string dataspaceVisibility,
      byte supportsPublicAccess)
    {
      TeamFoundationTracingService.NormalizeString(ref anonymousIdentifier);
      TeamFoundationTracingService.NormalizeString(ref area);
      TeamFoundationTracingService.NormalizeString(ref feature);
      TeamFoundationTracingService.NormalizeString(ref userAgent);
      TeamFoundationTracingService.NormalizeString(ref properties);
      try
      {
        return this.m_surveyEventTracingEnabled && TeamFoundationTracingService.s_eventProvider.TraceSurveyEvents(uniqueIdentifier, anonymousIdentifier, tenantId, hostId, parentHostId, hostType, vsid, cuid, area, feature, userAgent, properties, dataspaceType, dataspaceId, dataspaceVisibility, supportsPublicAccess);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    internal bool TraceClientTrace(
      string properties,
      Guid uniqueIdentifier,
      string anonymousIdentifier,
      Guid hostId,
      Guid parentHostId,
      byte hostType,
      Guid vsid,
      string area,
      string feature,
      string userAgent,
      Guid cuid,
      string method,
      string component,
      string message,
      string exceptionType,
      Guid E2EID,
      Guid tenantId,
      Guid providerid,
      Level level,
      DateTime startTime)
    {
      TeamFoundationTracingService.NormalizeString(ref properties);
      TeamFoundationTracingService.NormalizeString(ref anonymousIdentifier);
      TeamFoundationTracingService.NormalizeString(ref area);
      TeamFoundationTracingService.NormalizeString(ref feature);
      TeamFoundationTracingService.NormalizeString(ref userAgent);
      TeamFoundationTracingService.NormalizeString(ref method);
      TeamFoundationTracingService.NormalizeString(ref component);
      TeamFoundationTracingService.NormalizeString(ref message);
      TeamFoundationTracingService.NormalizeString(ref exceptionType);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceClientTrace(properties, uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, hostType, vsid, area, feature, userAgent, cuid, method, component, message, exceptionType, E2EID, tenantId, providerid, level, startTime);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static void TraceOrganizationCreate(
      Guid organizationID,
      string organizationName,
      Guid tenantID,
      int organizationType,
      int organizationStatus,
      string organizationStatusAsString,
      bool isActivated,
      DateTime creationDate,
      DateTime lastUpdated)
    {
      try
      {
        if (organizationID == Guid.Empty || string.IsNullOrEmpty(organizationName) || string.IsNullOrEmpty(organizationStatusAsString) || creationDate == DateTime.MinValue || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(organizationID == Guid.Empty ? "organizationID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationName) ? "organizationName is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationStatusAsString) ? "organizationStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(creationDate == DateTime.MinValue ? "creationDate is invalid. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationCreate), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "OrganizationID",
              (object) organizationID
            },
            {
              "OrganizationName",
              (object) organizationName
            },
            {
              "TenantID",
              (object) tenantID
            },
            {
              "OrganizationType",
              (object) organizationType
            },
            {
              "OrganizationStatus",
              (object) organizationStatus
            },
            {
              "OrganizationStatusAsString",
              (object) organizationStatusAsString
            },
            {
              "IsActivated",
              (object) isActivated
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceOrganizationDataFeed("OrganizationCreate", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationCreate), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceOrganizationModification(
      Guid organizationID,
      string organizationName,
      Guid tenantID,
      int organizationType,
      int organizationStatus,
      string organizationStatusAsString,
      bool isActivated,
      DateTime creationDate,
      DateTime lastUpdated)
    {
      try
      {
        if (organizationID == Guid.Empty || string.IsNullOrEmpty(organizationName) || string.IsNullOrEmpty(organizationStatusAsString) || creationDate == DateTime.MinValue || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(organizationID == Guid.Empty ? "organizationID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationName) ? "organizationName is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationStatusAsString) ? "organizationStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(creationDate == DateTime.MinValue ? "creationDate is invalid. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationModification), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "OrganizationID",
              (object) organizationID
            },
            {
              "OrganizationName",
              (object) organizationName
            },
            {
              "TenantID",
              (object) tenantID
            },
            {
              "OrganizationType",
              (object) organizationType
            },
            {
              "OrganizationStatus",
              (object) organizationStatus
            },
            {
              "OrganizationStatusAsString",
              (object) organizationStatusAsString
            },
            {
              "IsActivated",
              (object) isActivated
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceOrganizationDataFeed("OrganizationModification", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationModification), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceOrganizationDelete(
      Guid organizationID,
      string organizationName,
      int organizationStatus,
      string organizationStatusAsString,
      DateTime lastUpdated)
    {
      try
      {
        if (organizationID == Guid.Empty || string.IsNullOrEmpty(organizationName) || string.IsNullOrEmpty(organizationStatusAsString) || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(organizationID == Guid.Empty ? "organizationID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationName) ? "organizationName is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationStatusAsString) ? "organizationStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationDelete), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "OrganizationID",
              (object) organizationID
            },
            {
              "OrganizationName",
              (object) organizationName
            },
            {
              "OrganizationStatus",
              (object) organizationStatus
            },
            {
              "OrganizationStatusAsString",
              (object) organizationStatusAsString
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceOrganizationDataFeed("OrganizationDelete", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationDelete), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountReconciliation(
      Guid accountID,
      Guid parentHostID,
      string accountName,
      Guid accountOwner,
      Guid accountOwnerCUID,
      DateTime creationDate,
      int accountStatus,
      string accountStatusAsString,
      DateTime lastUpdated)
    {
      try
      {
        if (accountID == Guid.Empty || parentHostID == Guid.Empty || string.IsNullOrEmpty(accountName) || accountOwner == Guid.Empty || accountOwnerCUID == Guid.Empty || creationDate == DateTime.MinValue || string.IsNullOrEmpty(accountStatusAsString) || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(accountID == Guid.Empty ? "accountID is NULL. " : string.Empty);
          stringBuilder.Append(parentHostID == Guid.Empty ? "parentHostID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountName) ? "accountName is NULL or empty. " : string.Empty);
          stringBuilder.Append(accountOwner == Guid.Empty ? "accountOwner is NULL. " : string.Empty);
          stringBuilder.Append(accountOwnerCUID == Guid.Empty ? "accountOwnerCUID is NULL. " : string.Empty);
          stringBuilder.Append(creationDate == DateTime.MinValue ? "creationDate is invalid. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountStatusAsString) ? "accountStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountReconciliation), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              "ParentHostID",
              (object) parentHostID
            },
            {
              "AccountName",
              (object) accountName
            },
            {
              "AccountOwner",
              (object) accountOwner
            },
            {
              "AccountOwnerCUID",
              (object) accountOwnerCUID
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "AccountStatus",
              (object) accountStatus
            },
            {
              "AccountStatusAsString",
              (object) accountStatusAsString
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountReconciliation", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountReconciliation), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceOrganizationReconciliation(
      Guid organizationID,
      string organizationName,
      int organizationType,
      int organizationStatus,
      string organizationStatusAsString,
      bool isActivated,
      Guid tenantID,
      DateTime creationDate,
      DateTime lastUpdated)
    {
      try
      {
        if (organizationID == Guid.Empty || string.IsNullOrEmpty(organizationName) || string.IsNullOrEmpty(organizationStatusAsString) || creationDate == DateTime.MinValue || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(organizationID == Guid.Empty ? "organizationID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationName) ? "organizationName is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationStatusAsString) ? "organizationStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(creationDate == DateTime.MinValue ? "creationDate is invalid. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationReconciliation), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "OrganizationID",
              (object) organizationID
            },
            {
              "OrganizationName",
              (object) organizationName
            },
            {
              "OrganizationType",
              (object) organizationType
            },
            {
              "OrganizationStatus",
              (object) organizationStatus
            },
            {
              "OrganizationStatusAsString",
              (object) organizationStatusAsString
            },
            {
              "IsActivated",
              (object) isActivated
            },
            {
              "TenantID",
              (object) tenantID
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceOrganizationDataFeed("OrganizationReconciliation", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationReconciliation), nameof (TeamFoundationTracingService), ex);
      }
    }

    internal void TracePreferredRegionUpdate(
      Guid hostId,
      string preferredRegion,
      string regionUpdateType)
    {
      try
      {
        if (TeamFoundationTracingService.s_eventProvider == null)
          return;
        TeamFoundationTracingService.NormalizeString(ref preferredRegion);
        TeamFoundationTracingService.NormalizeString(ref regionUpdateType);
        TeamFoundationTracingService.s_eventProvider.TraceHostPreferredRegionUpdate(hostId, preferredRegion, regionUpdateType);
      }
      catch (Exception ex)
      {
      }
    }

    internal void TracePreferredGeographyUpdate(
      Guid hostId,
      string preferredGeography,
      string geographyUpdateType)
    {
      try
      {
        if (TeamFoundationTracingService.s_eventProvider == null)
          return;
        TeamFoundationTracingService.NormalizeString(ref preferredGeography);
        TeamFoundationTracingService.NormalizeString(ref geographyUpdateType);
        TeamFoundationTracingService.s_eventProvider.TraceHostPreferredRegionUpdate(hostId, preferredGeography, geographyUpdateType);
      }
      catch (Exception ex)
      {
      }
    }

    public static void TraceSubscriptionAssignmentHashed(
      Guid subscriptionGuid,
      string hashedUpn,
      string subscriptionState,
      DateTime lastAssignedDate,
      DateTime lastRemovedDate,
      DateTime modifiedDate)
    {
      try
      {
        if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || subscriptionGuid == Guid.Empty || string.IsNullOrEmpty(subscriptionState) || lastAssignedDate == DateTime.MinValue || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
          stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
          stringBuilder.Append(subscriptionGuid == Guid.Empty ? "subscriptionGuid is empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(subscriptionState) ? "subscriptionState is NULL or empty. " : string.Empty);
          stringBuilder.Append(lastAssignedDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceSubscriptionAssignmentHashed), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          TeamFoundationTracingService.NormalizeDateTime(ref lastRemovedDate);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "SubscriptionGuid",
              (object) subscriptionGuid
            },
            {
              "HashedUPN",
              (object) hashedUpn
            },
            {
              "SubscriptionState",
              (object) subscriptionState
            },
            {
              "LastAssignedDate",
              (object) lastAssignedDate
            },
            {
              "LastRemovedDate",
              (object) lastRemovedDate
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("SubscriptionAssignmentHashed", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceSubscriptionAssignmentHashed), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceSubscriptionUpdateHashed(
      Guid subscriptionGuid,
      string hashedUpn,
      string purchaseIDNumber,
      string channel,
      string programType,
      string subscriptionLevelID,
      string partnerSubscriptionID,
      string country,
      string language,
      string orderNumber,
      bool canDownload,
      string companyID,
      string companyName,
      DateTime expirationDate,
      DateTime modifiedDate)
    {
      try
      {
        if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || subscriptionGuid == Guid.Empty || string.IsNullOrEmpty(programType) || string.IsNullOrEmpty(subscriptionLevelID) || string.IsNullOrEmpty(channel) || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
          stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
          stringBuilder.Append(subscriptionGuid == Guid.Empty ? "subscriptionGuid is empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(programType) ? "programType is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(subscriptionLevelID) ? "subscriptionLevelID is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(channel) ? "channel is NULL or empty. " : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceSubscriptionUpdateHashed), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          TeamFoundationTracingService.NormalizeString(ref purchaseIDNumber);
          TeamFoundationTracingService.NormalizeString(ref partnerSubscriptionID);
          TeamFoundationTracingService.NormalizeString(ref country);
          TeamFoundationTracingService.NormalizeString(ref language);
          TeamFoundationTracingService.NormalizeString(ref orderNumber);
          TeamFoundationTracingService.NormalizeString(ref companyID);
          TeamFoundationTracingService.NormalizeString(ref companyName);
          TeamFoundationTracingService.NormalizeDateTime(ref expirationDate);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "SubscriptionGuid",
              (object) subscriptionGuid
            },
            {
              "HashedUPN",
              (object) hashedUpn
            },
            {
              "PurchaseIDNumber",
              (object) purchaseIDNumber
            },
            {
              "Channel",
              (object) channel
            },
            {
              "SubscriptionLevelID",
              (object) subscriptionLevelID
            },
            {
              "PartnerSubscriptionID",
              (object) partnerSubscriptionID
            },
            {
              "Country",
              (object) country
            },
            {
              "Language",
              (object) language
            },
            {
              "ProgramType",
              (object) programType
            },
            {
              "OrderNumber",
              (object) orderNumber
            },
            {
              "CanDownload",
              (object) canDownload
            },
            {
              "CompanyID",
              (object) companyID
            },
            {
              "CompanyName",
              (object) companyName
            },
            {
              "ExpirationDate",
              (object) expirationDate
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("SubscriptionUpdateHashed", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceSubscriptionUpdateHashed), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceDevEssentialsUpdateHashed(
      Guid subscriptionGuid,
      string hashedUpn,
      string state,
      string subscriptionLevelID,
      DateTime pendingAcceptanceDate,
      DateTime lastOptInDate,
      DateTime lastOptOutDate,
      DateTime modifiedDate)
    {
      try
      {
        if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || subscriptionGuid == Guid.Empty || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(subscriptionLevelID) || pendingAcceptanceDate == DateTime.MinValue || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
          stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
          stringBuilder.Append(subscriptionGuid == Guid.Empty ? "subscriptionGuid is empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(state) ? "state is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(subscriptionLevelID) ? "subscriptionLevelID is NULL or empty. " : string.Empty);
          stringBuilder.Append(pendingAcceptanceDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceDevEssentialsUpdateHashed), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          TeamFoundationTracingService.NormalizeDateTime(ref lastOptInDate);
          TeamFoundationTracingService.NormalizeDateTime(ref lastOptOutDate);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "SubscriptionGuid",
              (object) subscriptionGuid
            },
            {
              "HashedUPN",
              (object) hashedUpn
            },
            {
              "State",
              (object) state
            },
            {
              "SubscriptionLevelID",
              (object) subscriptionLevelID
            },
            {
              "PendingAcceptanceDate",
              (object) pendingAcceptanceDate
            },
            {
              "LastOptInDate",
              (object) lastOptInDate
            },
            {
              "LastOptOutDate",
              (object) lastOptOutDate
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("DevEssentialsUpdateHashed", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceDevEssentialsUpdateHashed), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceEntitlementEngagementHashed(
      string hashedUpn,
      string site,
      string scope,
      Guid vsid,
      Guid cuid,
      DateTime engagementDate)
    {
      if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || vsid == Guid.Empty || string.IsNullOrEmpty(site) || string.IsNullOrEmpty(scope) || engagementDate == DateTime.MinValue)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
        stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
        stringBuilder.Append(vsid == Guid.Empty ? "vsid is empty. " : string.Empty);
        stringBuilder.Append(string.IsNullOrEmpty(site) ? "state is NULL or empty. " : string.Empty);
        stringBuilder.Append(string.IsNullOrEmpty(scope) ? "subscriptionLevelID is NULL or empty. " : string.Empty);
        stringBuilder.Append(engagementDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
        TeamFoundationTracingService.TraceExceptionRaw(0, "TraceSubscriberOrAdminEngagementHashed", nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
      }
      else
        TeamFoundationTracingService.TraceEngagementHashed(hashedUpn, site, scope, vsid, cuid, engagementDate);
    }

    public static void TraceSubscriberOrAdminEngagementHashed(
      string hashedUpn,
      string site,
      string scope,
      Guid vsid,
      Guid cuid,
      DateTime engagementDate)
    {
      if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || vsid == Guid.Empty || cuid == Guid.Empty || string.IsNullOrEmpty(site) || string.IsNullOrEmpty(scope) || engagementDate == DateTime.MinValue)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
        stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
        stringBuilder.Append(vsid == Guid.Empty ? "vsid is empty. " : string.Empty);
        stringBuilder.Append(cuid == Guid.Empty ? "cuid is empty. " : string.Empty);
        stringBuilder.Append(string.IsNullOrEmpty(site) ? "state is NULL or empty. " : string.Empty);
        stringBuilder.Append(string.IsNullOrEmpty(scope) ? "subscriptionLevelID is NULL or empty. " : string.Empty);
        stringBuilder.Append(engagementDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceSubscriberOrAdminEngagementHashed), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
      }
      else
        TeamFoundationTracingService.TraceEngagementHashed(hashedUpn, site, scope, vsid, cuid, engagementDate);
    }

    private static void TraceEngagementHashed(
      string hashedUpn,
      string site,
      string scope,
      Guid vsid,
      Guid cuid,
      DateTime engagementDate)
    {
      try
      {
        string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
        {
          {
            "HashedUPN",
            (object) hashedUpn
          },
          {
            "Site",
            (object) site
          },
          {
            "Scope",
            (object) scope
          },
          {
            "VSID",
            (object) vsid
          },
          {
            "CUID",
            (object) cuid
          },
          {
            "EngagementDate",
            (object) engagementDate
          }
        });
        if (string.IsNullOrEmpty(json))
          return;
        TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("SubscriberOrAdminEngagementHashed", json);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, "TraceSubscriberOrAdminEngagementHashed", nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceEntitlementEngagementHashed(string hashedUpn, DateTime engagementDate)
    {
      try
      {
        if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || engagementDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
          stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
          stringBuilder.Append(engagementDate == DateTime.MinValue ? "timestamp is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, "TraceEV3EngagementHashed", nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "HashedUPN",
              (object) hashedUpn
            },
            {
              "EngagementDate",
              (object) engagementDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("EntitlementEngagementHashed", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceEntitlementEngagementHashed), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAdminAssignmentHashed(
      string hashedUpn,
      string purchaseIDNumber,
      string state,
      DateTime assignedDate,
      DateTime removedDate,
      DateTime modifiedDate)
    {
      try
      {
        if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || string.IsNullOrEmpty(purchaseIDNumber) || string.IsNullOrEmpty(state) || assignedDate == DateTime.MinValue || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
          stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(purchaseIDNumber) ? "purchaseIDNumber is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(state) ? "adminState is NULL or empty. " : string.Empty);
          stringBuilder.Append(assignedDate == DateTime.MinValue ? "assignedDate is invalid. " : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "modifiedDate is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAdminAssignmentHashed), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          TeamFoundationTracingService.NormalizeDateTime(ref removedDate);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "HashedUPN",
              (object) hashedUpn
            },
            {
              "PurchaseIDNumber",
              (object) purchaseIDNumber
            },
            {
              "State",
              (object) state
            },
            {
              "AssignedDate",
              (object) assignedDate
            },
            {
              "RemovedDate",
              (object) removedDate
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("AdminAssignmentHashed", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAdminAssignmentHashed), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAdminUpdateHashed(
      string hashedUpn,
      string purchaseIDNumber,
      string emailLanguage,
      string channel,
      string programType,
      string country,
      bool isSuperAdmin,
      DateTime modifiedDate)
    {
      try
      {
        if (string.IsNullOrEmpty(hashedUpn) || hashedUpn.IndexOf('@') != -1 || string.IsNullOrEmpty(purchaseIDNumber) || string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(programType) || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(string.IsNullOrEmpty(hashedUpn) ? "hashedUpn is NULL or empty. " : string.Empty);
          stringBuilder.Append(hashedUpn.IndexOf('@') != -1 ? "hashedUpn should not contain an '@' sign. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(purchaseIDNumber) ? "purchaseIDNumber is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(channel) ? "channel is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(programType) ? "programType is NULL or empty. " : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "modifiedDate is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAdminUpdateHashed), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          TeamFoundationTracingService.NormalizeString(ref emailLanguage);
          TeamFoundationTracingService.NormalizeString(ref country);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "HashedUPN",
              (object) hashedUpn
            },
            {
              "PurchaseIDNumber",
              (object) purchaseIDNumber
            },
            {
              "EmailLanguage",
              (object) emailLanguage
            },
            {
              "Channel",
              (object) channel
            },
            {
              "ProgramType",
              (object) programType
            },
            {
              "Country",
              (object) country
            },
            {
              "IsSuperAdmin",
              (object) isSuperAdmin
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("AdminUpdateHashed", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAdminUpdateHashed), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAgreement(
      string purchaseIDNumber,
      string orderNumber,
      string channel,
      string programType,
      string masterAgreementNumber,
      DateTime agreementStartDate,
      DateTime agreementEndDate,
      string agreementCountry,
      string publicCustomerNumber,
      string legalEntityName,
      DateTime migrationDate,
      DateTime operationTimestamp)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (string.IsNullOrEmpty(purchaseIDNumber))
        stringBuilder.Append("purchaseIDNumber is NULL or empty. ");
      if (string.IsNullOrEmpty(programType))
        stringBuilder.Append("programType is NULL or empty. ");
      else if (programType == "OLP" && string.IsNullOrEmpty(orderNumber))
        stringBuilder.Append("orderNumber should not be NULL or empty when programType is \"OLP\". ");
      else if (programType != "OLP" && !string.IsNullOrEmpty(orderNumber))
        stringBuilder.Append("orderNumber should be NULL or empty when programType is not \"OLP\". ");
      if (string.IsNullOrEmpty(channel))
        stringBuilder.Append("channel is NULL or empty. ");
      if (operationTimestamp == DateTime.MinValue)
        stringBuilder.Append("operationTimestamp is invalid. ");
      if (stringBuilder.Length > 0)
        throw new ArgumentException(string.Join(",", (object) stringBuilder));
      TeamFoundationTracingService.NormalizeString(ref orderNumber);
      TeamFoundationTracingService.NormalizeString(ref masterAgreementNumber);
      TeamFoundationTracingService.NormalizeDateTime(ref agreementStartDate);
      TeamFoundationTracingService.NormalizeDateTime(ref agreementEndDate);
      TeamFoundationTracingService.NormalizeString(ref agreementCountry);
      TeamFoundationTracingService.NormalizeString(ref publicCustomerNumber);
      TeamFoundationTracingService.NormalizeString(ref legalEntityName);
      TeamFoundationTracingService.NormalizeDateTime(ref migrationDate);
      string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
      {
        {
          "PurchaseIDNumber",
          (object) purchaseIDNumber
        },
        {
          "OrderNumber",
          (object) orderNumber
        },
        {
          "Channel",
          (object) channel
        },
        {
          "ProgramType",
          (object) programType
        },
        {
          "MasterAgreementNumber",
          (object) masterAgreementNumber
        },
        {
          "AgreementStartDate",
          (object) agreementStartDate
        },
        {
          "AgreementEndDate",
          (object) agreementEndDate
        },
        {
          "AgreementCountry",
          (object) agreementCountry
        },
        {
          "PublicCustomerNumber",
          (object) publicCustomerNumber
        },
        {
          "LegalEntityName",
          (object) legalEntityName
        },
        {
          "MigrationDate",
          (object) migrationDate
        },
        {
          "OperationTimestamp",
          (object) operationTimestamp
        }
      });
      if (string.IsNullOrEmpty(json))
        throw new Exception("Unable to serialize the agreement event to Json");
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("Agreement", json);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAgreement), nameof (TeamFoundationTracingService), ex);
        throw;
      }
    }

    public static void TraceAgreementQuantities(
      string purchaseIDNumber,
      string orderNumber,
      string subscriptionLevel,
      string programType,
      int qtyOrdered,
      int qtyAssigned,
      int? qtyAssignedHighWaterMark,
      DateTime operationTimestamp)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (string.IsNullOrEmpty(purchaseIDNumber))
        stringBuilder.Append("purchaseIDNumber is NULL or empty. ");
      if (string.IsNullOrEmpty(programType))
        stringBuilder.Append("programType is NULL or empty. ");
      else if (programType == "OLP" && string.IsNullOrEmpty(orderNumber))
        stringBuilder.Append("orderNumber should not be NULL or empty when programType is \"OLP\". ");
      else if (programType != "OLP" && !string.IsNullOrEmpty(orderNumber))
        stringBuilder.Append("orderNumber should be NULL or empty when programType is not \"OLP\". ");
      if (string.IsNullOrEmpty(subscriptionLevel))
        stringBuilder.Append("subscriptionLevel is NULL or empty. ");
      if (operationTimestamp == DateTime.MinValue)
        stringBuilder.Append("operationTimestamp is invalid. ");
      if (stringBuilder.Length > 0)
        throw new ArgumentException(string.Join(",", (object) stringBuilder));
      TeamFoundationTracingService.NormalizeString(ref orderNumber);
      string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
      {
        {
          "PurchaseIDNumber",
          (object) purchaseIDNumber
        },
        {
          "OrderNumber",
          (object) orderNumber
        },
        {
          "SubscriptionLevel",
          (object) subscriptionLevel
        },
        {
          "ProgramType",
          (object) programType
        },
        {
          "QtyOrdered",
          (object) qtyOrdered
        },
        {
          "QtyAssigned",
          (object) qtyAssigned
        },
        {
          "QtyAssignedHighWaterMark",
          (object) qtyAssignedHighWaterMark
        },
        {
          "OperationTimestamp",
          (object) operationTimestamp
        }
      });
      if (string.IsNullOrEmpty(json))
        throw new Exception("Unable to serialize the agreement quantities event to Json");
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceSubscriptionDataFeed("AgreementQuantities", json);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAgreementQuantities), nameof (TeamFoundationTracingService), ex);
        throw;
      }
    }

    static TeamFoundationTracingService()
    {
      TeamFoundationTracingService.s_eventProvider = new TeamFoundationTracingService.TraceProvider();
      AppDomain.CurrentDomain.DomainUnload += new EventHandler(TeamFoundationTracingService.CurrentDomain_DomainUnload);
      using (Process currentProcess = Process.GetCurrentProcess())
        TeamFoundationTracingService.s_processName = currentProcess.ProcessName;
      TeamFoundationTracingService.s_webSiteId = TeamFoundationTracingService.GetWebSiteId();
    }

    private static void CurrentDomain_DomainUnload(object sender, EventArgs e) => TeamFoundationTracingService.s_eventProvider.Dispose();

    internal static string ProcessName => TeamFoundationTracingService.s_processName;

    public static void TraceExceptionRaw(
      int tracepoint,
      string area,
      string layer,
      Exception exception)
    {
      TeamFoundationTracingService.TraceExceptionRaw(tracepoint, TraceLevel.Error, area, layer, (string) null, exception, "{0}", (object) exception);
    }

    public static void TraceExceptionRaw(
      int tracepoint,
      string area,
      string layer,
      string method,
      Exception exception)
    {
      TeamFoundationTracingService.TraceExceptionRaw(tracepoint, TraceLevel.Error, area, layer, method, exception, "{0}", (object) exception);
    }

    public static void TraceExceptionRaw(
      int tracepoint,
      TraceLevel tracelevel,
      string area,
      string layer,
      Exception exception)
    {
      TeamFoundationTracingService.TraceExceptionRaw(tracepoint, tracelevel, area, layer, (string) null, exception, "{0}", (object) exception);
    }

    public static void TraceExceptionRaw(
      int tracepoint,
      TraceLevel tracelevel,
      string area,
      string layer,
      string method,
      Exception exception)
    {
      TeamFoundationTracingService.TraceExceptionRaw(tracepoint, tracelevel, area, layer, method, exception, "{0}", (object) exception);
    }

    public static void TraceExceptionRaw(
      int tracepoint,
      TraceLevel tracelevel,
      string area,
      string layer,
      Exception exception,
      string format,
      params object[] args)
    {
      TeamFoundationTracingService.TraceExceptionRaw(tracepoint, tracelevel, area, layer, (string) null, exception, format, args);
    }

    public static void TraceExceptionRaw(
      int tracepoint,
      TraceLevel tracelevel,
      string area,
      string layer,
      string method,
      Exception exception,
      string format,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, tracelevel, area, layer, method, (string[]) null, exception.GetType().FullName, Guid.Empty, Guid.Empty, Guid.Empty, (string) null, Guid.Empty, format, args);
    }

    public static void TraceCatchRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception)
    {
      if (exception == null)
        return;
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, (string[]) null, exception.GetType().FullName, Guid.Empty, Guid.Empty, Guid.Empty, (string) null, Guid.Empty, "Exception caught and handled: {0}", (object) exception);
    }

    internal static void TraceEnterRaw(
      int tracepoint,
      string area,
      string layer,
      string methodName,
      Guid? vsid = null,
      Guid? cuid = null,
      Guid? e2eId = null,
      Guid? uniqueIdentifier = null,
      string orchestrationId = null)
    {
      int tracepoint1 = tracepoint;
      string area1 = area;
      string layer1 = layer;
      string[] tagPerformance = TeamFoundationTracingService.s_tagPerformance;
      Guid? nullable = vsid;
      Guid vsid1 = nullable ?? Guid.Empty;
      nullable = cuid;
      Guid cuid1 = nullable ?? Guid.Empty;
      nullable = e2eId;
      Guid e2eId1 = nullable ?? Guid.Empty;
      nullable = uniqueIdentifier;
      Guid guid = nullable ?? Guid.Empty;
      string orchestrationId1 = orchestrationId;
      Guid uniqueIdentifier1 = guid;
      object[] objArray = new object[1]
      {
        (object) methodName
      };
      TeamFoundationTracingService.TraceRawCore(tracepoint1, TraceLevel.Verbose, area1, layer1, tagPerformance, (string) null, vsid1, cuid1, e2eId1, orchestrationId1, uniqueIdentifier1, "Entering {0}", objArray);
      if (!SqlStatisticsContext.CollectingStatistics)
        return;
      SqlStatisticsContext.Enter(methodName);
    }

    internal static void TraceEnterRaw(
      int tracepoint,
      string area,
      string layer,
      string methodName,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, TraceLevel.Verbose, area, layer, TeamFoundationTracingService.s_tagPerformance, (string) null, Guid.Empty, Guid.Empty, Guid.Empty, (string) null, Guid.Empty, "Entering " + methodName, args);
      if (!SqlStatisticsContext.CollectingStatistics)
        return;
      SqlStatisticsContext.Enter(methodName);
    }

    internal static void TraceLeaveRaw(
      int tracepoint,
      string area,
      string layer,
      string methodName,
      Guid? vsid = null,
      Guid? cuid = null,
      Guid? e2eId = null,
      Guid? uniqueIdentifier = null,
      string orchestrationId = null)
    {
      if (SqlStatisticsContext.CollectingStatistics)
        SqlStatisticsContext.Leave(methodName);
      int tracepoint1 = tracepoint;
      string area1 = area;
      string layer1 = layer;
      string[] tagPerformance = TeamFoundationTracingService.s_tagPerformance;
      Guid? nullable = vsid;
      Guid vsid1 = nullable ?? Guid.Empty;
      nullable = cuid;
      Guid cuid1 = nullable ?? Guid.Empty;
      nullable = e2eId;
      Guid e2eId1 = nullable ?? Guid.Empty;
      nullable = uniqueIdentifier;
      Guid guid = nullable ?? Guid.Empty;
      string orchestrationId1 = orchestrationId;
      Guid uniqueIdentifier1 = guid;
      object[] objArray = new object[1]
      {
        (object) methodName
      };
      TeamFoundationTracingService.TraceRawCore(tracepoint1, TraceLevel.Verbose, area1, layer1, tagPerformance, (string) null, vsid1, cuid1, e2eId1, orchestrationId1, uniqueIdentifier1, "Leaving {0}", objArray);
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message)
    {
      TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, Guid.Empty, Guid.Empty, message);
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      Guid uniqueIdentifier,
      string message)
    {
      TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, e2eId, (string) null, uniqueIdentifier, message);
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string message)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, (string[]) null, (string) null, Guid.Empty, Guid.Empty, e2eId, orchestrationId, uniqueIdentifier, message, (object[]) null);
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, Guid.Empty, Guid.Empty, message, args);
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      Guid uniqueIdentifier,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, e2eId, (string) null, uniqueIdentifier, message, args);
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, (string[]) null, (string) null, Guid.Empty, Guid.Empty, e2eId, orchestrationId, uniqueIdentifier, message, args);
    }

    public static void TraceRaw(
      bool traceAlways,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      Guid uniqueIdentifier,
      string message,
      params object[] args)
    {
      if (traceAlways)
        TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, e2eId, uniqueIdentifier, (string[]) null, message, args);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, e2eId, uniqueIdentifier, message, args);
    }

    public static void TraceRawConditionally(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> message)
    {
      if (level != TraceLevel.Error && !TeamFoundationTracingService.IsRawTracingEnabled(tracepoint, level, area, layer, (string[]) null))
        return;
      string message1;
      try
      {
        message1 = message();
      }
      catch (Exception ex)
      {
        message1 = "Exception thrown while generating trace message: " + ex.ToReadableStackTrace();
      }
      TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, message1);
    }

    public static void TraceRawConditionally(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      Func<string> message)
    {
      if (level != TraceLevel.Error && !TeamFoundationTracingService.IsRawTracingEnabled(tracepoint, level, area, layer, (string[]) null))
        return;
      string message1;
      try
      {
        message1 = message();
      }
      catch (Exception ex)
      {
        message1 = "Exception thrown while generating trace message: " + ex.ToReadableStackTrace();
      }
      TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, e2eId, orchestrationId, uniqueIdentifier, message1);
    }

    internal static void TraceRawAlwaysOn(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, (string[]) null, message, args);
    }

    internal static void TraceRawAlwaysOn(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      Guid uniqueIdentifier,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, e2eId, uniqueIdentifier, (string[]) null, message, args);
    }

    internal static void TraceRawAlwaysOn(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, e2eId, orchestrationId, uniqueIdentifier, (string[]) null, message, args);
    }

    internal static void TraceRawAlwaysOn(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, Guid.Empty, Guid.Empty, tags, message, args);
    }

    internal static void TraceRawAlwaysOn(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      Guid uniqueIdentifier,
      string[] tags,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, e2eId, (string) null, uniqueIdentifier, tags, message, args);
    }

    internal static void TraceRawAlwaysOn(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string[] tags,
      string message,
      params object[] args)
    {
      TraceEvent trace = new TraceEvent(message, args);
      TeamFoundationTracingService.GetTraceEvent(ref trace, tracepoint, level, area, layer, Guid.Empty, Guid.Empty, e2eId, orchestrationId, uniqueIdentifier, tags, (string) null);
      TeamFoundationTracingService.WriteEvent(TeamFoundationTracingService.s_eventProvider, Guid.Empty, trace.Tracepoint, trace.ServiceHost, trace.ContextId, trace.Level, trace.ProcessName, trace.UserLogin, trace.Service, trace.Method, trace.Area, trace.Layer, trace.UserAgent, trace.Uri, trace.Path, trace.Tags, trace.ExceptionType, trace.VSID, trace.CUID, trace.TenantId, trace.ProviderId, trace.GetMessage(), trace.UniqueIdentifier, trace.E2EId, trace.OrchestrationId);
      TeamFoundationTracingService.CollectTracePublishedVsSkippedMetric(true, trace.Tracepoint);
    }

    public static bool IsRawTracingEnabled(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags)
    {
      if (level == TraceLevel.Error)
        return true;
      if (TeamFoundationTracingService.s_rawTraces != null)
      {
        TraceEvent traceEvent = new TraceEvent();
        TeamFoundationTracingService.GetTraceEvent(ref traceEvent, tracepoint, level, area, layer, tags, (string) null);
        foreach (Microsoft.VisualStudio.Services.WebApi.TraceFilter rawTrace in TeamFoundationTracingService.s_rawTraces)
        {
          if (rawTrace.IsMatch(ref traceEvent))
            return true;
        }
      }
      return false;
    }

    internal static void TraceRaw(ref TraceEvent trace, bool traceAlways = false)
    {
      try
      {
        bool flag = false;
        if (trace.Level == TraceLevel.Error | traceAlways)
        {
          TeamFoundationTracingService.WriteEvent(TeamFoundationTracingService.s_eventProvider, new Guid(1, (short) 1, (short) 1, TeamFoundationTracingService.c_byteArray), trace.Tracepoint, trace.ServiceHost, trace.ContextId, trace.Level, trace.ProcessName, trace.UserLogin, trace.Service, trace.Method, trace.Area, trace.Layer, trace.UserAgent, trace.Uri, trace.Path, trace.Tags, trace.ExceptionType, trace.VSID, trace.CUID, trace.TenantId, trace.ProviderId, trace.GetMessage(), trace.UniqueIdentifier, trace.E2EId, trace.OrchestrationId);
          TeamFoundationTracingService.CollectTracePublishedVsSkippedMetric(true, trace.Tracepoint);
          flag = true;
        }
        if (!flag && TeamFoundationTracingService.s_rawTraces != null)
        {
          foreach (Microsoft.VisualStudio.Services.WebApi.TraceFilter rawTrace in TeamFoundationTracingService.s_rawTraces)
          {
            if (rawTrace.IsMatch(ref trace))
            {
              TeamFoundationTracingService.WriteEvent(TeamFoundationTracingService.s_eventProvider, rawTrace.TraceId, trace.Tracepoint, trace.ServiceHost, trace.ContextId, trace.Level, trace.ProcessName, trace.UserLogin, trace.Service, trace.Method, trace.Area, trace.Layer, trace.UserAgent, trace.Uri, trace.Path, trace.Tags, trace.ExceptionType, trace.VSID, trace.CUID, trace.TenantId, trace.ProviderId, trace.GetMessage(), trace.UniqueIdentifier, trace.E2EId, trace.OrchestrationId);
              TeamFoundationTracingService.CollectTracePublishedVsSkippedMetric(true, trace.Tracepoint);
              flag = true;
              break;
            }
          }
        }
        if (flag)
          return;
        if (TeamFoundationTracingService.s_enableLowPriorityProductTrace)
          TeamFoundationTracingService.WriteEventLowPriority((TeamFoundationTracingService.ILowPriorityTraceProvider) TeamFoundationTracingService.s_eventProvider, ref trace);
        TeamFoundationTracingService.CollectTracePublishedVsSkippedMetric(false, trace.Tracepoint);
      }
      catch (Exception ex)
      {
      }
    }

    internal static void GetTraceEvent(
      ref TraceEvent trace,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string exceptionType)
    {
      TeamFoundationTracingService.GetTraceEvent(ref trace, tracepoint, level, (string) null, area, layer, Guid.Empty, Guid.Empty, Guid.Empty, (string) null, Guid.Empty, tags, exceptionType);
    }

    internal static void GetTraceEvent(
      ref TraceEvent trace,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Guid vsid,
      Guid cuid,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string[] tags,
      string exceptionType)
    {
      TeamFoundationTracingService.GetTraceEvent(ref trace, tracepoint, level, (string) null, area, layer, vsid, cuid, e2eId, (string) null, uniqueIdentifier, tags, exceptionType);
    }

    internal static void GetTraceEvent(
      ref TraceEvent trace,
      int tracepoint,
      TraceLevel level,
      string method,
      string area,
      string layer,
      Guid vsid,
      Guid cuid,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string[] tags,
      string exceptionType)
    {
      trace.Tracepoint = tracepoint;
      trace.ServiceHost = Guid.Empty;
      trace.ContextId = 0L;
      trace.Level = level;
      trace.UserLogin = string.Empty;
      trace.Method = method;
      trace.Area = area;
      trace.Layer = layer;
      trace.VSID = vsid;
      trace.CUID = cuid;
      trace.E2EId = e2eId;
      trace.OrchestrationId = orchestrationId;
      trace.UniqueIdentifier = uniqueIdentifier;
      trace.UserAgent = string.Empty;
      trace.Uri = string.Empty;
      trace.Path = string.Empty;
      trace.Tags = tags;
      trace.ExceptionType = exceptionType;
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string message)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, tags, (string) null, Guid.Empty, Guid.Empty, Guid.Empty, (string) null, Guid.Empty, message, (object[]) null);
    }

    public static void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, tags, (string) null, Guid.Empty, Guid.Empty, Guid.Empty, (string) null, Guid.Empty, message, args);
    }

    internal static void TraceRawCore(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string exceptionType,
      Guid vsid,
      Guid cuid,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string message,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, (string) null, tags, exceptionType, vsid, cuid, e2eId, orchestrationId, uniqueIdentifier, message, args);
    }

    internal static void TraceRawCore(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string method,
      string[] tags,
      string exceptionType,
      Guid vsid,
      Guid cuid,
      Guid e2eId,
      string orchestrationId,
      Guid uniqueIdentifier,
      string message,
      params object[] args)
    {
      TraceEvent trace = new TraceEvent(message, args);
      TeamFoundationTracingService.GetTraceEvent(ref trace, tracepoint, level, method, area, layer, vsid, cuid, e2eId, orchestrationId, uniqueIdentifier, tags, exceptionType);
      TeamFoundationTracingService.TraceRaw(ref trace);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        systemRequestContext.CheckDeploymentRequestContext();
        this.m_traces = Array.Empty<Microsoft.VisualStudio.Services.WebApi.TraceFilter>();
        this.m_traceCounts = new ConcurrentDictionary<Guid, TeamFoundationTracingService.ThreadSafeCounter>();
        for (int index = 0; index < this.m_userImpactedTime.Length; ++index)
          this.m_userImpactedTime[index] = new ConcurrentDictionary<Guid, bool>();
        if (systemRequestContext.ServiceHost.HasDatabaseAccess)
        {
          if (!systemRequestContext.IsServicingContext)
            this.m_updateRawTraces = true;
          TeamFoundationSqlNotificationService service = systemRequestContext.GetService<TeamFoundationSqlNotificationService>();
          service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.RegistrySettingsChanged, new SqlNotificationCallback(this.ReloadRegistrySettings), false);
          this.LoadRegistrySettings(systemRequestContext);
          service.RegisterNotification(systemRequestContext, "Default", TeamFoundationTracingService.TracingServiceGuid, new SqlNotificationCallback(this.ReloadConfiguration), false);
          this.ReloadConfiguration(systemRequestContext, Guid.Empty, string.Empty);
          if (!systemRequestContext.IsServicingContext)
            service.RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.PublishTestAlertRequest, new SqlNotificationCallback(this.OnPublishTestAlertRequested), false);
        }
        else
          this.m_traces = new Microsoft.VisualStudio.Services.WebApi.TraceFilter[1]
          {
            new Microsoft.VisualStudio.Services.WebApi.TraceFilter()
            {
              TraceId = new Guid("ce29475b-def5-4c85-a795-e9589c6f791b")
            }
          };
        TeamFoundationTracingService.s_euiiDetectionService = systemRequestContext.GetService<EuiiDetectionService>();
        this.IsStarted = true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(5000, TraceLevel.Error, "TracingService", "IVssFrameworkService", "Caught Exception {0} while attempting start", (object) ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.IsStarted = false;

    public bool IsTraceEnabled(IVssRequestContext requestContext, ref TraceEvent traceSet)
    {
      if (this.m_traces == null || traceSet.Level == TraceLevel.Error)
        return true;
      foreach (Microsoft.VisualStudio.Services.WebApi.TraceFilter trace in this.m_traces)
      {
        if (trace.IsMatch(ref traceSet))
          return true;
      }
      return false;
    }

    public void Trace(IVssRequestContext requestContext, ref TraceEvent traceSet, bool traceAlways = false)
    {
      try
      {
        Guid userId = requestContext.RootContext.GetUserId();
        if (traceSet.VSID == Guid.Empty && userId != Guid.Empty)
          traceSet.VSID = userId;
        IdentityTracingItems identityTracingItems = requestContext.RootContext.GetUserIdentityTracingItems();
        if (identityTracingItems != null)
        {
          traceSet.CUID = identityTracingItems.Cuid;
          traceSet.TenantId = identityTracingItems.TenantId;
          traceSet.ProviderId = identityTracingItems.ProviderId;
        }
        if (traceSet.UniqueIdentifier == Guid.Empty && requestContext.UniqueIdentifier != Guid.Empty)
          traceSet.UniqueIdentifier = requestContext.UniqueIdentifier;
        bool flag = false;
        if (traceSet.Level == TraceLevel.Error | traceAlways)
        {
          TeamFoundationTracingService.WriteEvent(TeamFoundationTracingService.s_eventProvider, new Guid(1, (short) 1, (short) 1, TeamFoundationTracingService.c_byteArray), traceSet.Tracepoint, traceSet.ServiceHost, traceSet.ContextId, traceSet.Level, traceSet.ProcessName, traceSet.UserLogin, traceSet.Service, traceSet.Method, traceSet.Area, traceSet.Layer, traceSet.UserAgent, traceSet.Uri, traceSet.Path, traceSet.Tags, traceSet.ExceptionType, traceSet.VSID, traceSet.CUID, traceSet.TenantId, traceSet.ProviderId, traceSet.GetMessage(), traceSet.UniqueIdentifier, traceSet.E2EId, traceSet.OrchestrationId);
          TeamFoundationTracingService.CollectTracePublishedVsSkippedMetric(true, traceSet.Tracepoint);
          flag = true;
        }
        if (flag)
          return;
        foreach (Microsoft.VisualStudio.Services.WebApi.TraceFilter trace in this.m_traces)
        {
          if (trace.IsMatch(ref traceSet))
          {
            TeamFoundationTracingService.ThreadSafeCounter threadSafeCounter;
            if (this.m_monitorTracesPerTraceFilter && this.m_traceCounts.TryGetValue(trace.TraceId, out threadSafeCounter) && threadSafeCounter.Increment() == this.m_maxTracesPerTraceFilter)
            {
              trace.IsEnabled = false;
              try
              {
                IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
                TeamFoundationTaskService service;
                using (requestContext.AcquireExemptionLock())
                  service = vssRequestContext.GetService<TeamFoundationTaskService>();
                service.AddTask(vssRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.DisableTrace), (object) trace.TraceId, 0));
              }
              catch (Exception ex)
              {
                TeamFoundationTracingService.TraceExceptionRaw(9006, "TracingService", "IVssFrameworkService", ex);
              }
            }
            TeamFoundationTracingService.WriteEvent(TeamFoundationTracingService.s_eventProvider, trace.TraceId, traceSet.Tracepoint, traceSet.ServiceHost, traceSet.ContextId, traceSet.Level, traceSet.ProcessName, traceSet.UserLogin, traceSet.Service, traceSet.Method, traceSet.Area, traceSet.Layer, traceSet.UserAgent, traceSet.Uri, traceSet.Path, traceSet.Tags, traceSet.ExceptionType, traceSet.VSID, traceSet.CUID, traceSet.TenantId, traceSet.ProviderId, traceSet.GetMessage(), traceSet.UniqueIdentifier, traceSet.E2EId, traceSet.OrchestrationId);
            TeamFoundationTracingService.CollectTracePublishedVsSkippedMetric(true, traceSet.Tracepoint);
            flag = true;
          }
        }
        if (flag)
          return;
        if (TeamFoundationTracingService.s_enableLowPriorityProductTrace)
          TeamFoundationTracingService.WriteEventLowPriority((TeamFoundationTracingService.ILowPriorityTraceProvider) TeamFoundationTracingService.s_eventProvider, ref traceSet);
        TeamFoundationTracingService.CollectTracePublishedVsSkippedMetric(false, traceSet.Tracepoint);
      }
      catch (Exception ex)
      {
      }
    }

    internal void DisableTrace(IVssRequestContext rc, object traceId)
    {
      TeamFoundationTracingService.TraceRaw(9003, TraceLevel.Error, "TracingService", "IVssFrameworkService", "Disabling Trace {0}.", traceId);
      using (TracingComponent component = rc.CreateComponent<TracingComponent>())
        component.DisableTrace(rc, (Guid) traceId);
    }

    public void TraceSql(
      IVssRequestContext requestContext,
      int tracepoint,
      string dataSource,
      string database,
      string operation,
      short retries,
      bool success,
      int totalTime,
      int connectTime,
      int executionTime,
      int waitTime,
      int sqlErrorCode,
      string sqlErrorMessage)
    {
      TeamFoundationTracingService.NormalizeString(ref dataSource);
      TeamFoundationTracingService.NormalizeString(ref database);
      TeamFoundationTracingService.NormalizeString(ref operation);
      TeamFoundationTracingService.NormalizeString(ref sqlErrorMessage);
      try
      {
        TraceEvent traceSet = new TraceEvent()
        {
          Tracepoint = 64039,
          Path = database,
          Level = TraceLevel.Verbose
        };
        if (!this.IsTraceEnabled(requestContext, ref traceSet))
          return;
        TeamFoundationTracingService.s_eventProvider?.TraceSQL(dataSource, database, operation, retries, success, totalTime, connectTime, executionTime, waitTime, sqlErrorCode, sqlErrorMessage);
      }
      catch (Exception ex)
      {
      }
    }

    public static void TraceSqlRaw(
      int tracepoint,
      string dataSource,
      string database,
      string operation,
      short retries,
      bool success,
      int totalTime,
      int connectTime,
      int executionTime,
      int waitTime,
      int sqlErrorCode,
      string sqlErrorMessage)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(64039, TraceLevel.Verbose, operation, "TeamFoundationSqlResourceComponent", JsonConvert.SerializeObject((object) new
        {
          tracepoint = tracepoint,
          dataSource = dataSource,
          database = database,
          operation = operation,
          retries = retries,
          success = success,
          totalTime = totalTime,
          connectTime = connectTime,
          executionTime = executionTime,
          waitTime = waitTime,
          sqlErrorCode = sqlErrorCode,
          sqlErrorMessage = sqlErrorMessage
        }));
      }
      catch (Exception ex)
      {
      }
    }

    public bool TraceEuii(
      string properties,
      Guid uniqueIdentifier,
      string anonymousIdentifier,
      Guid hostId,
      Guid parentHostId,
      byte hostType,
      Guid vsid,
      string area,
      string feature,
      string userAgent,
      Guid cuid,
      string method,
      string uri,
      string component,
      string message,
      string exceptionType,
      Guid E2EID,
      Guid tenantId,
      Guid providerid,
      TraceLevel level,
      DateTime startTime)
    {
      TeamFoundationTracingService.NormalizeString(ref properties);
      TeamFoundationTracingService.NormalizeString(ref anonymousIdentifier);
      TeamFoundationTracingService.NormalizeString(ref area);
      TeamFoundationTracingService.NormalizeString(ref feature);
      TeamFoundationTracingService.NormalizeString(ref userAgent);
      TeamFoundationTracingService.NormalizeString(ref method);
      TeamFoundationTracingService.NormalizeString(ref uri);
      TeamFoundationTracingService.NormalizeString(ref component);
      TeamFoundationTracingService.NormalizeString(ref message);
      TeamFoundationTracingService.NormalizeString(ref exceptionType);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceEuii(properties, uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, hostType, vsid, area, feature, userAgent, cuid, method, uri, component, message, exceptionType, E2EID, tenantId, providerid, level, startTime);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    internal bool TraceKpi(string metrics)
    {
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceKpi(metrics);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    internal void TraceKpiMetric(
      DateTime eventTime,
      Guid hostId,
      string area,
      string scope,
      string displayName,
      string description,
      string kpiMetricName,
      double kpiMetricValue)
    {
      TeamFoundationTracingService.NormalizeDateTime(ref eventTime);
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceKpiMetric(eventTime, hostId, area, scope, displayName, description, kpiMetricName, kpiMetricValue);
      }
      catch (Exception ex)
      {
      }
    }

    internal static void TraceEventMetric(
      DateTime eventTime,
      int databaseId,
      string deploymentId,
      Guid hostId,
      string machineName,
      string roleInstanceId,
      string eventSource,
      string scope,
      string eventType,
      int eventId,
      string metricName,
      double metricValue)
    {
      TeamFoundationTracingService.NormalizeDateTime(ref eventTime);
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceEventMetric(eventTime, databaseId, deploymentId, hostId, machineName, roleInstanceId, eventSource, scope, eventType, eventId, metricName, metricValue);
      }
      catch (Exception ex)
      {
      }
    }

    internal static void TraceDetectedEUIIEvent(
      DateTime eventTime,
      string source,
      EuiiType euiiType,
      string message)
    {
      TeamFoundationTracingService.NormalizeDateTime(ref eventTime);
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceDetectedEUIIEvent(eventTime, source, (int) euiiType, message);
      }
      catch (Exception ex)
      {
      }
    }

    internal bool TraceCustomerIntelligence(
      string properties,
      Guid uniqueIdentifier,
      string anonymousIdentifier,
      Guid hostId,
      Guid parentHostId,
      byte hostType,
      Guid vsid,
      Guid cuid,
      string area,
      string feature,
      string userAgent,
      string dataspaceType,
      string dataspaceId,
      string dataspaceVisibility,
      byte supportsPublicAccess)
    {
      TeamFoundationTracingService.NormalizeString(ref properties, true);
      TeamFoundationTracingService.NormalizeString(ref anonymousIdentifier, true);
      TeamFoundationTracingService.NormalizeString(ref area, true);
      TeamFoundationTracingService.NormalizeString(ref feature, true);
      TeamFoundationTracingService.NormalizeString(ref userAgent, true);
      TeamFoundationTracingService.NormalizeString(ref dataspaceType, true);
      TeamFoundationTracingService.NormalizeString(ref dataspaceId, true);
      TeamFoundationTracingService.NormalizeString(ref dataspaceVisibility, true);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceCustomerIntelligence(properties, uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, hostType, vsid, cuid, area, feature, userAgent, dataspaceType, dataspaceId, dataspaceVisibility, supportsPublicAccess);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    internal void TraceDatabaseCounters(
      string serverName,
      string databaseName,
      Guid hostId,
      Guid projectId,
      string counterName,
      long counterValue,
      int leftOverPercent)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName, true);
      TeamFoundationTracingService.NormalizeString(ref databaseName, true);
      TeamFoundationTracingService.NormalizeString(ref counterName, true);
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceDatabaseCounters(serverName, databaseName, hostId, projectId, counterName, counterValue, leftOverPercent);
      }
      catch (Exception ex)
      {
      }
    }

    internal void TraceDatabaseIdentityColumns(
      string serverName,
      string databaseName,
      string schemaName,
      string tableName,
      string identityColumnName,
      long identityColumnValue,
      string identityColumnDatatype,
      int leftOverPercent)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName, true);
      TeamFoundationTracingService.NormalizeString(ref databaseName, true);
      TeamFoundationTracingService.NormalizeString(ref schemaName, true);
      TeamFoundationTracingService.NormalizeString(ref tableName, true);
      TeamFoundationTracingService.NormalizeString(ref identityColumnName, true);
      TeamFoundationTracingService.NormalizeString(ref identityColumnDatatype, true);
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceDatabaseIdentityColumns(serverName, databaseName, schemaName, tableName, identityColumnName, identityColumnValue, identityColumnDatatype, leftOverPercent);
      }
      catch (Exception ex)
      {
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void TraceMachinePoolRequestHistory(
      IVssRequestContext requestContext,
      string poolType,
      string poolName,
      string poolRegion,
      string imageVersion,
      string instanceName,
      long requestId,
      Guid hostId,
      string inputs,
      string outcome,
      string outputs,
      DateTime queueTime,
      DateTime assignTime,
      DateTime startTime,
      DateTime finishTime,
      DateTime unassignTime,
      Guid traceActivityId,
      int maxParallelism,
      string imageLabel,
      long timeoutSeconds,
      long slaSeconds,
      DateTime slaStartTime,
      string tags,
      Guid subscriptionId,
      string requiredResourceVersion,
      string suspiciousActivity,
      string orchestrationId)
    {
      try
      {
        TeamFoundationTracingService.NormalizeString(ref poolType);
        TeamFoundationTracingService.NormalizeString(ref poolName);
        TeamFoundationTracingService.NormalizeString(ref poolRegion);
        TeamFoundationTracingService.NormalizeString(ref imageVersion);
        TeamFoundationTracingService.NormalizeString(ref instanceName);
        TeamFoundationTracingService.NormalizeString(ref inputs);
        TeamFoundationTracingService.NormalizeString(ref outcome);
        TeamFoundationTracingService.NormalizeString(ref outputs);
        TeamFoundationTracingService.NormalizeString(ref imageLabel);
        TeamFoundationTracingService.NormalizeString(ref tags);
        TeamFoundationTracingService.NormalizeString(ref requiredResourceVersion);
        TeamFoundationTracingService.NormalizeString(ref suspiciousActivity);
        TeamFoundationTracingService.NormalizeString(ref orchestrationId);
        TeamFoundationTracingService.NormalizeDateTime(ref queueTime);
        TeamFoundationTracingService.NormalizeDateTime(ref assignTime);
        TeamFoundationTracingService.NormalizeDateTime(ref startTime);
        TeamFoundationTracingService.NormalizeDateTime(ref finishTime);
        TeamFoundationTracingService.NormalizeDateTime(ref unassignTime);
        TeamFoundationTracingService.NormalizeDateTime(ref slaStartTime);
        TeamFoundationTracingService.s_eventProvider.TraceMachinePoolRequestHistory(poolType, poolName, poolRegion, imageVersion, instanceName, requestId, hostId, inputs, outcome, outputs, queueTime, assignTime, startTime, finishTime, unassignTime, traceActivityId, maxParallelism, imageLabel, timeoutSeconds, slaSeconds, slaStartTime, tags, subscriptionId, requiredResourceVersion, suspiciousActivity, orchestrationId);
      }
      catch (Exception ex)
      {
      }
    }

    internal static void TraceServiceHostHistory(
      Guid hostId,
      DateTime modifiedDate,
      short actionType,
      Guid parentHostId,
      string serverName,
      string databaseName,
      int databaseId,
      int storageAccountId,
      string name,
      short status,
      string statusReason,
      short hostType,
      DateTime lastUserAccess,
      int subStatus)
    {
      try
      {
        TeamFoundationTracingService.NormalizeString(ref serverName);
        TeamFoundationTracingService.NormalizeString(ref databaseName);
        TeamFoundationTracingService.NormalizeString(ref name);
        TeamFoundationTracingService.NormalizeString(ref statusReason);
        TeamFoundationTracingService.NormalizeDateTime(ref lastUserAccess);
        TeamFoundationTracingService.NormalizeDateTime(ref modifiedDate);
        TeamFoundationTracingService.s_eventProvider.TraceHostHistory(hostId, modifiedDate, actionType, parentHostId, serverName, databaseName, databaseId, storageAccountId, name, status, statusReason, hostType, lastUserAccess, subStatus);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, "Tracing", "TracingService", ex);
      }
    }

    internal void TraceServiceHostExtended(RequestDetails sh) => this.TraceServiceHostExtended(sh.InstanceId, (byte) sh.HostType, sh.ParentHostId, sh.HostName, sh.DatabaseServerName, sh.DatabaseName);

    internal void TraceServiceHostExtended(
      Guid hostId,
      byte hostType,
      Guid parentHostId,
      string hostName,
      string serverName,
      string databaseName,
      short status = -1,
      string statusReason = "",
      DateTime? lastUserAccess = null,
      bool isDeleted = false)
    {
      try
      {
        if (!this.m_jobAndActivityLogTracingEnabled)
          return;
        TeamFoundationTracingService.NormalizeString(ref hostName);
        TeamFoundationTracingService.NormalizeString(ref serverName);
        TeamFoundationTracingService.NormalizeString(ref databaseName);
        TeamFoundationTracingService.NormalizeString(ref statusReason);
        DateTime lastUserAccess1 = lastUserAccess ?? DateTime.UtcNow;
        TeamFoundationTracingService.s_eventProvider.TraceServiceHostExtended(hostId, hostType, parentHostId, hostName, serverName, databaseName, status, statusReason, lastUserAccess1, isDeleted);
      }
      catch (Exception ex)
      {
      }
    }

    public static void TraceCommerceAzureSubscription(
      Guid subscriptionId,
      DateTime registrationDate,
      string subscriptionSource,
      string subscriptionState,
      Guid accountId,
      Guid parentHostId,
      TeamFoundationHostType hostType,
      string accountLinkSource,
      string quotaId,
      DateTime eventDate)
    {
      try
      {
        string str = (registrationDate == DateTime.MinValue ? "registrationDate " : string.Empty) + (string.IsNullOrEmpty(subscriptionSource) ? "subscriptionSource " : string.Empty) + (string.IsNullOrEmpty(subscriptionState) ? "subscriptionState " : string.Empty) + (eventDate == DateTime.MinValue ? nameof (eventDate) : string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceCommerceAzureSubscription), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          TeamFoundationTracingService.NormalizeString(ref accountLinkSource);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "SubscriptionId",
              (object) subscriptionId
            },
            {
              "RegistrationDate",
              (object) registrationDate
            },
            {
              "SubscriptionSource",
              (object) subscriptionSource
            },
            {
              "SubscriptionState",
              (object) subscriptionState
            },
            {
              "AccountId",
              (object) accountId
            },
            {
              "ParentHostId",
              (object) parentHostId
            },
            {
              "HostType",
              (object) hostType
            },
            {
              "AccountLinkSource",
              (object) accountLinkSource
            },
            {
              "QuotaId",
              (object) quotaId
            },
            {
              "EventDate",
              (object) eventDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceCommerceDataFeed("AzureSubscription", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceCommerceAzureSubscription), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceCommerceMeteredResource(
      Guid accountId,
      Guid parentHostId,
      TeamFoundationHostType hostType,
      Guid eventUserId,
      Guid eventUserCUID,
      string meterName,
      Guid meterGuid,
      string renewalGroup,
      int includedQuantity,
      int committedQuantity,
      int? priorCommittedQuantity,
      int currentQuantity,
      int maximumQuantity,
      int changedPurchaseQuantity,
      bool isPaidBilling,
      string billingPeriod,
      double billedQuantity,
      int usedQuantity,
      string source,
      Guid subscriptionId,
      string meterCategory,
      DateTime eventDate,
      string eventType)
    {
      try
      {
        string str = (accountId == Guid.Empty ? "accountId " : string.Empty) + (string.IsNullOrEmpty(meterName) ? "meterName " : string.Empty) + (string.IsNullOrEmpty(billingPeriod) ? "billingPeriod " : string.Empty) + (string.IsNullOrEmpty(source) ? "source " : string.Empty) + (string.IsNullOrEmpty(meterCategory) ? "meterCategory " : string.Empty) + (eventDate == DateTime.MinValue ? nameof (eventDate) : string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceCommerceMeteredResource), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountId",
              (object) accountId
            },
            {
              "ParentHostId",
              (object) parentHostId
            },
            {
              "HostType",
              (object) hostType
            },
            {
              "EventUserId",
              (object) eventUserId
            },
            {
              "EventUserCUID",
              (object) eventUserCUID
            },
            {
              "MeterName",
              (object) meterName
            },
            {
              "MeterGuid",
              (object) meterGuid
            },
            {
              "RenewalGroup",
              (object) renewalGroup
            },
            {
              "IncludedQuantity",
              (object) includedQuantity
            },
            {
              "CommittedQuantity",
              (object) committedQuantity
            },
            {
              "PriorCommittedQuantity",
              (object) priorCommittedQuantity
            },
            {
              "CurrentQuantity",
              (object) currentQuantity
            },
            {
              "MaximumQuantity",
              (object) maximumQuantity
            },
            {
              "ChangedPurchaseQuantity",
              (object) changedPurchaseQuantity
            },
            {
              "IsPaidBilling",
              (object) isPaidBilling
            },
            {
              "BillingPeriod",
              (object) billingPeriod
            },
            {
              "BilledQuantity",
              (object) billedQuantity
            },
            {
              "UsedQuantity",
              (object) usedQuantity
            },
            {
              "Source",
              (object) source
            },
            {
              "EventType",
              (object) eventType
            },
            {
              "SubscriptionId",
              (object) subscriptionId
            },
            {
              "MeterCategory",
              (object) meterCategory
            },
            {
              "EventDate",
              (object) eventDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceCommerceDataFeed("MeteredResource", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceCommerceMeteredResource), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceProfileHistory(
      Guid vsid,
      Guid cuid,
      string descriptor,
      UserOperation op,
      DateTime modifiedDate,
      string profileName,
      string preferredEmail,
      string countryName,
      string contactWithOffers,
      string phoneNumber,
      DateTime createdDate,
      string culture)
    {
      try
      {
        string str = (vsid == Guid.Empty ? "vsid " : string.Empty) + (cuid == Guid.Empty ? "cuid " : string.Empty) + (string.IsNullOrEmpty(descriptor) ? "descriptor " : string.Empty) + (modifiedDate == DateTime.MinValue ? "modifiedDate " : string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceProfileHistory), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          TeamFoundationTracingService.NormalizeString(ref descriptor);
          TeamFoundationTracingService.NormalizeString(ref profileName);
          TeamFoundationTracingService.NormalizeString(ref preferredEmail);
          TeamFoundationTracingService.NormalizeString(ref countryName);
          TeamFoundationTracingService.NormalizeString(ref contactWithOffers);
          TeamFoundationTracingService.NormalizeString(ref phoneNumber);
          TeamFoundationTracingService.NormalizeString(ref culture);
          TeamFoundationTracingService.NormalizeDateTime(ref createdDate);
          string json1 = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "VSID",
              (object) vsid
            },
            {
              "CUID",
              (object) cuid
            },
            {
              "Descriptor",
              (object) descriptor
            },
            {
              "Operation",
              (object) op.ToString()
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            },
            {
              "CountryName",
              (object) countryName
            },
            {
              "ContactWithOffers",
              (object) contactWithOffers
            },
            {
              "CreatedDate",
              (object) createdDate
            },
            {
              "Culture",
              (object) culture
            }
          });
          string json2 = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "VSID",
              (object) vsid
            },
            {
              "CUID",
              (object) cuid
            },
            {
              "Descriptor",
              (object) descriptor
            },
            {
              "Operation",
              (object) op.ToString()
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            },
            {
              "ProfileName",
              (object) profileName
            },
            {
              "PreferredEmail",
              (object) preferredEmail
            },
            {
              "CountryName",
              (object) countryName
            },
            {
              "ContactWithOffers",
              (object) contactWithOffers
            },
            {
              "PhoneNumber",
              (object) phoneNumber
            },
            {
              "CreatedDate",
              (object) createdDate
            },
            {
              "Culture",
              (object) culture
            }
          });
          if (string.IsNullOrEmpty(json2))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceUserDataFeed("Profile", json1);
          TeamFoundationTracingService.s_eventProvider.TraceEuiiUserDataFeed("Profile", json2);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceProfileHistory), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceIdentityFederatedProviderData(
      Guid vsid,
      Guid cuid,
      IReadOnlyList<string> providerNames,
      DateTime modifiedDate)
    {
      try
      {
        if (vsid == Guid.Empty || cuid == Guid.Empty || providerNames == null || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(vsid == Guid.Empty ? "vsid is empty GUID." : string.Empty);
          stringBuilder.Append(cuid == Guid.Empty ? "cuid is empty GUID" : string.Empty);
          stringBuilder.Append(providerNames == null ? "providerNames is NULL." : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "modifiedDate is invalid." : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentityFederatedProviderData), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "VSID",
              (object) vsid
            },
            {
              "CUID",
              (object) cuid
            },
            {
              "ProviderNames",
              (object) providerNames
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceUserDataFeed("IdentityFederatedProviderData", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentityFederatedProviderData), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceIdentityChanges(
      Guid vsid,
      Guid cuid,
      string identityCategory,
      string identityProvider,
      string identityProviderID,
      string identityProviderTenantID,
      DateTime modifiedDate)
    {
      try
      {
        if (vsid == Guid.Empty || string.IsNullOrEmpty(identityCategory) || string.IsNullOrEmpty(identityProvider) || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(vsid == Guid.Empty ? "vsid is NULL." : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(identityCategory) ? "identityCategory is NULL or empty." : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(identityProvider) ? "identityProvider is NULL or empty." : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "modifiedDate is invalid." : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentityChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          TeamFoundationTracingService.NormalizeString(ref identityProviderID);
          TeamFoundationTracingService.NormalizeString(ref identityProviderTenantID);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "VSID",
              (object) vsid
            },
            {
              "CUID",
              (object) cuid
            },
            {
              "IdentityCategory",
              (object) identityCategory
            },
            {
              "IdentityProvider",
              (object) identityProvider
            },
            {
              "IdentityProviderID",
              (object) identityProviderID
            },
            {
              "IdentityProviderTenantID",
              (object) identityProviderTenantID
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceUserDataFeed("Identity", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentityChanges), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceIdentitySessionTokenOperation(
      string operation,
      string tokenType = "",
      string error = "",
      Guid clientId = default (Guid),
      Guid accessId = default (Guid),
      Guid authorizationId = default (Guid),
      Guid hostAuthorizationId = default (Guid),
      Guid userId = default (Guid),
      DateTime validFrom = default (DateTime),
      DateTime validTo = default (DateTime),
      string displayName = "",
      string scope = "",
      string targetAccounts = "",
      bool isValid = false,
      bool isPublic = false,
      string publicData = "",
      string source = "")
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceIdentitySessionTokenDataFeed(operation ?? "Empty operation", tokenType ?? string.Empty, error ?? string.Empty, clientId, accessId, authorizationId, hostAuthorizationId, userId, validFrom, validTo, displayName ?? string.Empty, scope ?? string.Empty, targetAccounts ?? string.Empty, isValid, isPublic, publicData ?? string.Empty, source ?? string.Empty);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentitySessionTokenOperation), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceIdentityReadsOperation(
      string className,
      string flavor,
      string identifier,
      string queryMembership,
      string propertyNameFilters,
      string options,
      string callStack = "")
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceIdentityReadDataFeed(className, flavor, identifier, queryMembership, propertyNameFilters, options, callStack);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentityReadsOperation), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceIdentityTokenOperation(
      string className,
      string header,
      string claims,
      string nonce)
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceIdentityTokenDataFeed(className, header, claims, nonce);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentityTokenOperation), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceIdentityCacheChangesOperation(
      string cacheType,
      string eventType,
      string searchFilter,
      string hostDomain,
      string eventValue,
      string queryMembership,
      string cacheReadResult,
      string callStack = "")
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceIdentityCacheChangesFeed(cacheType, eventType, searchFilter, hostDomain, eventValue, queryMembership, cacheReadResult, callStack);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentityCacheChangesOperation), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceIdentitySqlChangesOperation(
      string eventType,
      string hostDomain,
      string eventValue,
      string queryMembership,
      string callStack = "")
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceIdentitySqlChangesFeed(eventType, hostDomain, eventValue, queryMembership, callStack);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceIdentitySqlChangesOperation), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceDirectoryMemberVsdIdentity(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      Microsoft.VisualStudio.Services.Identity.Identity vsdIdentity)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (vsdIdentity),
          (object) vsdIdentity
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberVsdIdentity(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      Microsoft.VisualStudio.Services.Identity.Identity vsdIdentity,
      bool updated)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (updated),
          (object) updated
        },
        {
          nameof (vsdIdentity),
          (object) vsdIdentity
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberAadObject(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      object aadObject)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (aadObject),
          aadObject
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberMsaUser(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      object msaUser)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (msaUser),
          msaUser
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberGitHubUser(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      object gitHubUser)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (gitHubUser),
          gitHubUser
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberTargetIdForCreateOrUpdate(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      Guid targetIdForCreateOrUpdate,
      bool isTargetIdValid,
      string expectedDomain,
      string expectedOriginId,
      IReadOnlyList<VsdIdentityResult> missesWhereAllPropertiesMatch,
      IReadOnlyList<VsdIdentityResult> missesWhereUnchangeablePropertiesMismatch,
      IReadOnlyDictionary<Guid, VsdIdentityResult> missesWhereChangeablePropertiesMismatch,
      IReadOnlyList<VsdIdentityResult> missesRequiringNewIdentityId)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        },
        {
          nameof (targetIdForCreateOrUpdate),
          (object) targetIdForCreateOrUpdate
        },
        {
          nameof (isTargetIdValid),
          (object) isTargetIdValid
        },
        {
          nameof (expectedDomain),
          (object) expectedDomain
        },
        {
          nameof (expectedOriginId),
          (object) expectedOriginId
        },
        {
          nameof (missesWhereAllPropertiesMatch),
          (object) missesWhereAllPropertiesMatch
        },
        {
          nameof (missesWhereUnchangeablePropertiesMismatch),
          (object) missesWhereUnchangeablePropertiesMismatch
        },
        {
          nameof (missesWhereChangeablePropertiesMismatch),
          (object) missesWhereChangeablePropertiesMismatch
        },
        {
          nameof (missesRequiringNewIdentityId),
          (object) missesRequiringNewIdentityId
        }
      });
    }

    public static void TraceDirectoryMemberIdentityRightsTransfer(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      IdentityRightsTransfer identityRightsTransfer)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (identityRightsTransfer),
          (object) identityRightsTransfer
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberStorageKeyProvisioning(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        },
        {
          nameof (subjectDescriptor),
          (object) subjectDescriptor
        },
        {
          nameof (storageKey),
          (object) storageKey
        }
      });
    }

    public static void TraceDirectoryMemberAccountEntitlement(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      AccountEntitlement accountEntitlement,
      bool isPreexisting)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (accountEntitlement),
          (object) accountEntitlement
        },
        {
          nameof (isPreexisting),
          (object) isPreexisting
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberGroupMembership(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      IdentityDescriptor groupDescriptor,
      bool added)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (added),
          (object) added
        },
        {
          nameof (groupDescriptor),
          (object) groupDescriptor
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberProfile(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      Microsoft.VisualStudio.Services.Profile.Profile profile)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (profile),
          (object) profile
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberUser(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      User user)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (user),
          (object) user
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    public static void TraceDirectoryMemberResult(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      DirectoryEntityResult result)
    {
      TeamFoundationTracingService.TraceDirectoryMemberEvent(eventType, new Dictionary<string, object>()
      {
        {
          nameof (hostId),
          (object) hostId
        },
        {
          nameof (result),
          (object) result
        },
        {
          nameof (directoryMember),
          (object) directoryMember
        }
      });
    }

    internal static void TraceDirectoryMemberEvent(
      string eventType,
      Dictionary<string, object> directoryMemberEvent)
    {
      try
      {
        string json = TeamFoundationTracingService.DataFieldsToJson(directoryMemberEvent);
        if (string.IsNullOrEmpty(json))
          return;
        TeamFoundationTracingService.s_eventProvider.TraceDirectoryMemberDataFeed(eventType, json);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceDirectoryMemberEvent), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountCreate(
      Guid accountID,
      Guid parentHostID,
      string accountName,
      Guid accountOwner,
      Guid accountOwnerCUID,
      Guid tenantID,
      DateTime creationDate,
      int accountStatus,
      string accountStatusAsString,
      string campaignId,
      string entryPoint,
      DateTime lastUpdated,
      string acquisitionId)
    {
      try
      {
        if (accountID == Guid.Empty || parentHostID == Guid.Empty || string.IsNullOrEmpty(accountName) || accountOwner == Guid.Empty || creationDate == DateTime.MinValue || string.IsNullOrEmpty(accountStatusAsString) || lastUpdated == DateTime.MinValue || string.IsNullOrEmpty(acquisitionId))
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(accountID == Guid.Empty ? "accountID is NULL. " : string.Empty);
          stringBuilder.Append(parentHostID == Guid.Empty ? "parentHostID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountName) ? "accountName is NULL or empty. " : string.Empty);
          stringBuilder.Append(accountOwner == Guid.Empty ? "accountOwner is NULL. " : string.Empty);
          stringBuilder.Append(creationDate == DateTime.MinValue ? "creationDate is invalid. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountStatusAsString) ? "accountStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(acquisitionId) ? "acquisitionId is NULL or empty. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountCreate), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          TeamFoundationTracingService.NormalizeString(ref campaignId);
          TeamFoundationTracingService.NormalizeString(ref entryPoint);
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              nameof (parentHostID),
              (object) parentHostID
            },
            {
              "AccountName",
              (object) accountName
            },
            {
              "AccountOwner",
              (object) accountOwner
            },
            {
              "TenantID",
              (object) tenantID
            },
            {
              "AccountOwnerCUID",
              (object) accountOwnerCUID
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "AccountStatus",
              (object) accountStatus
            },
            {
              "AccountStatusAsString",
              (object) accountStatusAsString
            },
            {
              "CampaignId",
              (object) campaignId
            },
            {
              "EntryPoint",
              (object) entryPoint
            },
            {
              "LastUpdated",
              (object) lastUpdated
            },
            {
              "AcquisitionId",
              (object) acquisitionId
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountCreate", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountCreate), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountUpdate(
      Guid accountID,
      Guid parentHostID,
      string accountName,
      Guid accountOwner,
      Guid accountOwnerCUID,
      DateTime creationDate,
      int accountStatus,
      string accountStatusAsString,
      DateTime lastUpdated)
    {
      try
      {
        if (accountID == Guid.Empty || parentHostID == Guid.Empty || string.IsNullOrEmpty(accountName) || accountOwner == Guid.Empty || creationDate == DateTime.MinValue || string.IsNullOrEmpty(accountStatusAsString) || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(accountID == Guid.Empty ? "accountID is NULL. " : string.Empty);
          stringBuilder.Append(parentHostID == Guid.Empty ? "parentHostID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountName) ? "accountName is NULL or empty. " : string.Empty);
          stringBuilder.Append(accountOwner == Guid.Empty ? "accountOwner is NULL. " : string.Empty);
          stringBuilder.Append(creationDate == DateTime.MinValue ? "creationDate is invalid. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountStatusAsString) ? "accountStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountUpdate), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              nameof (parentHostID),
              (object) parentHostID
            },
            {
              "AccountName",
              (object) accountName
            },
            {
              "AccountOwner",
              (object) accountOwner
            },
            {
              "AccountOwnerCUID",
              (object) accountOwnerCUID
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "AccountStatus",
              (object) accountStatus
            },
            {
              "AccountStatusAsString",
              (object) accountStatusAsString
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountModification", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountUpdate), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountDelete(
      Guid accountID,
      Guid parentHostID,
      string accountName,
      int accountStatus,
      string accountStatusAsString,
      int? gracePeriod,
      bool violatedTerms,
      DateTime lastUpdated)
    {
      try
      {
        if (accountID == Guid.Empty || parentHostID == Guid.Empty || string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountStatusAsString) || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(accountID == Guid.Empty ? "accountID is NULL. " : string.Empty);
          stringBuilder.Append(parentHostID == Guid.Empty ? "parentHostID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountName) ? "accountName is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(accountStatusAsString) ? "accountStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountDelete), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              nameof (parentHostID),
              (object) parentHostID
            },
            {
              "AccountName",
              (object) accountName
            },
            {
              "AccountStatus",
              (object) accountStatus
            },
            {
              "AccountStatusAsString",
              (object) accountStatusAsString
            },
            {
              "LastUpdated",
              (object) lastUpdated
            },
            {
              "GracePeriod",
              (object) gracePeriod
            },
            {
              "ViolatedTerms",
              (object) violatedTerms
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountDelete", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountDelete), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountInternal(Guid accountID, bool isInternal, DateTime lastUpdated)
    {
      try
      {
        if (accountID == Guid.Empty || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(accountID == Guid.Empty ? "accountID is NULL. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountInternal), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              "IsInternal",
              (object) isInternal
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountInternal", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountInternal), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceTenantLinking(
      Guid parentHostId,
      Guid tenantID,
      Dictionary<Guid, Guid> vsidMapping,
      IList<KeyValuePair<Guid, Guid>> cuidMapping,
      string linkingType,
      DateTime modifiedDate)
    {
      try
      {
        if (parentHostId == Guid.Empty || string.IsNullOrEmpty(linkingType) || modifiedDate == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(parentHostId == Guid.Empty ? "parentHostId is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(linkingType) ? "linkingType is NULL or empty. " : string.Empty);
          stringBuilder.Append(modifiedDate == DateTime.MinValue ? "modifiedDate is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceTenantLinking), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "ParentHostId",
              (object) parentHostId
            },
            {
              "TenantID",
              (object) tenantID
            },
            {
              "VSIDMapping",
              (object) vsidMapping
            },
            {
              "CUIDMapping",
              (object) cuidMapping
            },
            {
              "LinkType",
              (object) linkingType
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceOrganizationDataFeed("TenantLinking", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceTenantLinking), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceOrganizationChanges(
      Guid organizationID,
      string organizationName,
      int organizationType,
      int organizationStatus,
      string organizationStatusAsString,
      string actionType,
      DateTime creationDate,
      DateTime lastUpdated)
    {
      try
      {
        if (organizationID == Guid.Empty || string.IsNullOrEmpty(organizationName) || string.IsNullOrEmpty(organizationStatusAsString) || string.IsNullOrEmpty(actionType) || creationDate == DateTime.MinValue || lastUpdated == DateTime.MinValue)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(organizationID == Guid.Empty ? "organizationID is NULL. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationName) ? "organizationName is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(organizationStatusAsString) ? "organizationStatusAsString is NULL or empty. " : string.Empty);
          stringBuilder.Append(string.IsNullOrEmpty(actionType) ? "actionType is NULL or empty. " : string.Empty);
          stringBuilder.Append(creationDate == DateTime.MinValue ? "creationDate is invalid. " : string.Empty);
          stringBuilder.Append(lastUpdated == DateTime.MinValue ? "lastUpdated is invalid. " : string.Empty);
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(stringBuilder.ToString()));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "OrganizationID",
              (object) organizationID
            },
            {
              "OrganizationName",
              (object) organizationName
            },
            {
              "OrganizationType",
              (object) organizationType
            },
            {
              "OrganizationStatus",
              (object) organizationStatus
            },
            {
              "OrganizationStatusAsString",
              (object) organizationStatusAsString
            },
            {
              "ActionType",
              (object) actionType
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "LastUpdated",
              (object) lastUpdated
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceOrganizationDataFeed("OrganizationChange", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceOrganizationChanges), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountChanges(
      Guid accountID,
      string accountName,
      Guid accountOwner,
      Guid createdBy,
      DateTime creationDate,
      int accountStatus,
      string accountStatusAsString,
      DateTime lastUpdated,
      Guid aadTenantId)
    {
      try
      {
        string str = string.Format("{0}{1}{2}{3}{4}{5}{6}", accountID == Guid.Empty ? (object) "accountID " : (object) string.Empty, string.IsNullOrEmpty(accountName) ? (object) nameof (accountName) : (object) string.Empty, accountOwner == Guid.Empty ? (object) "accountOwner " : (object) string.Empty, createdBy == Guid.Empty ? (object) nameof (createdBy) : (object) string.Empty, creationDate == DateTime.MinValue ? (object) nameof (creationDate) : (object) string.Empty, lastUpdated == DateTime.MinValue ? (object) "lastModified" : (object) string.Empty, string.IsNullOrEmpty(accountStatusAsString) ? (object) nameof (accountStatusAsString) : (object) string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              "AccountName",
              (object) accountName
            },
            {
              "AccountOwner",
              (object) accountOwner
            },
            {
              "CreatedBy",
              (object) createdBy
            },
            {
              "CreationDate",
              (object) creationDate
            },
            {
              "AccountStatus",
              (object) accountStatus
            },
            {
              "AccountStatusAsString",
              (object) accountStatusAsString
            },
            {
              "LastUpdated",
              (object) lastUpdated
            },
            {
              "AADTenantId",
              (object) aadTenantId
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountUpdate", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountChanges), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountLinkingChanges(
      Guid accountID,
      Guid tenantID,
      Guid[] vsids,
      Guid[] cuids,
      Guid[] mappedvsid,
      Guid[] mappedCuids,
      DateTime modifiedDate)
    {
      try
      {
        if (vsids.Length != mappedvsid.Length)
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountLinkingChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Different Array Lengths: vsids {0} vs mappedvsid {1}", (object) vsids.Length, (object) mappedvsid.Length)));
        else if (cuids.Length != mappedCuids.Length)
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountLinkingChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Different Array Lengths: cuids {0} vs mappedCuids {1}", (object) cuids.Length, (object) mappedCuids.Length)));
        else if (vsids.Length != cuids.Length)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountLinkingChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Different Array Lengths: vsids {0} vs cuids {1}", (object) vsids.Length, (object) cuids.Length)));
        }
        else
        {
          Dictionary<string, object> eventProperties = new Dictionary<string, object>();
          for (int index = 0; index < vsids.Length; ++index)
            eventProperties.Add(vsids[index].ToString(), (object) mappedvsid[index]);
          string json1 = TeamFoundationTracingService.DataFieldsToJson(eventProperties);
          List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
          for (int index = 0; index < cuids.Length; ++index)
            keyValuePairList.Add(new KeyValuePair<string, object>(cuids[index].ToString(), (object) mappedCuids[index]));
          string str = (accountID == Guid.Empty ? "accountID " : string.Empty) + (json1 == string.Empty ? "vsidMapping " : string.Empty) + (cuids == null || cuids.Length == 0 ? "cuidMapping " : string.Empty) + (modifiedDate == DateTime.MinValue ? nameof (modifiedDate) : string.Empty);
          if (str != string.Empty)
          {
            TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountLinkingChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
          }
          else
          {
            string json2 = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
            {
              {
                "AccountID",
                (object) accountID
              },
              {
                nameof (tenantID),
                (object) tenantID
              },
              {
                "vsidMapping",
                (object) json1
              },
              {
                "cuidMapping",
                (object) keyValuePairList
              },
              {
                "ModifiedDate",
                (object) modifiedDate
              }
            });
            if (string.IsNullOrEmpty(json2))
              return;
            TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountLinkingChanges", json2);
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountLinkingChanges), nameof (TeamFoundationTracingService), ex);
      }
    }

    public static void TraceAccountInternalChanges(
      Guid accountID,
      bool isInternal,
      DateTime modifiedDate)
    {
      try
      {
        string str = (accountID == Guid.Empty ? "accountID " : string.Empty) + (modifiedDate == DateTime.MinValue ? nameof (modifiedDate) : string.Empty);
        if (str != string.Empty)
        {
          TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountInternalChanges), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
        }
        else
        {
          string json = TeamFoundationTracingService.DataFieldsToJson(new Dictionary<string, object>()
          {
            {
              "AccountID",
              (object) accountID
            },
            {
              "IsInternal",
              (object) isInternal
            },
            {
              "ModifiedDate",
              (object) modifiedDate
            }
          });
          if (string.IsNullOrEmpty(json))
            return;
          TeamFoundationTracingService.s_eventProvider.TraceAccountDataFeed("AccountInternalChanges", json);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, nameof (TraceAccountInternalChanges), nameof (TeamFoundationTracingService), ex);
      }
    }

    private static string DataFieldsToJson(Dictionary<string, object> eventProperties)
    {
      string str = (eventProperties == null ? "parameter eventProperties is null. " : string.Empty) + (eventProperties.Count == 0 ? "dictionary eventProperties has no keys/values." : string.Empty);
      if (!(str != string.Empty))
        return JsonConvert.SerializeObject((object) eventProperties);
      TeamFoundationTracingService.TraceExceptionRaw(0, nameof (DataFieldsToJson), nameof (TeamFoundationTracingService), (Exception) new ArgumentException(string.Format("Invalid parameters: {0}", (object) str)));
      return string.Empty;
    }

    internal void TraceActivityLog(RequestDetails requestContext)
    {
      try
      {
        string serviceName = requestContext.ServiceName;
        string title = requestContext.Title;
        int num1 = requestContext.Status == null ? 0 : -1;
        DateTime startTime1 = requestContext.StartTime;
        long delayTime1 = requestContext.DelayTime;
        long concurrencySemaphoreTime1 = requestContext.ConcurrencySemaphoreTime;
        long executionTime1 = requestContext.ExecutionTime;
        int count = requestContext.Count;
        Guid vsid1 = requestContext.VSID;
        string str1 = !(requestContext.AuthenticatedUserName == requestContext.DomainUserName) ? string.Format("{0} ({1})", (object) requestContext.AuthenticatedUserName, (object) requestContext.DomainUserName) : requestContext.AuthenticatedUserName;
        string remoteIpAddress = requestContext.RemoteIPAddress;
        Guid uniqueIdentifier = requestContext.UniqueIdentifier;
        string userAgent1 = requestContext.UserAgent;
        string command1 = requestContext.Command;
        Guid activityId1 = requestContext.ActivityId;
        string str2 = (string) null;
        string str3 = (string) null;
        if (requestContext.Status != null)
        {
          str2 = TeamFoundationTracingService.GetActivityLoggingExceptionName(requestContext.Status);
          str3 = requestContext.Status.Message;
        }
        Guid instanceId = requestContext.InstanceId;
        long contextId = requestContext.ContextId;
        string application = serviceName;
        string command2 = title;
        int status = num1;
        int executionCount = count;
        DateTime startTime2 = startTime1;
        long delayTime2 = delayTime1;
        long concurrencySemaphoreTime2 = concurrencySemaphoreTime1;
        long executionTime2 = executionTime1;
        string identityName = str1;
        string ipAddress = remoteIpAddress;
        Guid uniqueIdentifer = uniqueIdentifier;
        string userAgent2 = userAgent1;
        string commandIdentifier = command1;
        string exceptionType = str2;
        string exceptionMessage = str3;
        Guid activityId2 = activityId1;
        int responseCode = requestContext.ResponseCode;
        Guid vsid2 = vsid1;
        IdentityTracingItems identityTracingItems1 = requestContext.IdentityTracingItems;
        Guid cuid = identityTracingItems1 != null ? identityTracingItems1.Cuid : Guid.Empty;
        IdentityTracingItems identityTracingItems2 = requestContext.IdentityTracingItems;
        Guid tenantId = identityTracingItems2 != null ? identityTracingItems2.TenantId : Guid.Empty;
        IdentityTracingItems identityTracingItems3 = requestContext.IdentityTracingItems;
        Guid providerId = identityTracingItems3 != null ? identityTracingItems3.ProviderId : Guid.Empty;
        long timeToFirstPage = requestContext.TimeToFirstPage;
        int activityStatus = (int) (short) requestContext.ActivityStatus;
        string authenticationMechanism = requestContext.AuthenticationMechanism;
        long executionTimeThreshold = requestContext.ExecutionTimeThreshold;
        int num2 = requestContext.IsExceptionExpected ? 1 : 0;
        int logicalReads = requestContext.LogicalReads;
        int physicalReads = requestContext.PhysicalReads;
        int cpuTime = requestContext.CpuTime;
        int elapsedTime = requestContext.ElapsedTime;
        string feature = requestContext.Feature;
        DateTime hostStartTime = requestContext.HostStartTime;
        int hostType = (int) (byte) requestContext.HostType;
        Guid parentHostId = requestContext.ParentHostId;
        string anonymousIdentifier = requestContext.AnonymousIdentifier;
        int sqlExecutionTime = requestContext.SqlExecutionTime;
        int sqlExecutionCount = requestContext.SqlExecutionCount;
        int redisExecutionTime = requestContext.RedisExecutionTime;
        int redisExecutionCount = requestContext.RedisExecutionCount;
        int graphExecutionTime = requestContext.AadGraphExecutionTime;
        int graphExecutionCount = requestContext.AadGraphExecutionCount;
        int tokenExecutionTime = requestContext.AadTokenExecutionTime;
        int tokenExecutionCount = requestContext.AadTokenExecutionCount;
        int storageExecutionTime1 = requestContext.BlobStorageExecutionTime;
        int storageExecutionCount1 = requestContext.BlobStorageExecutionCount;
        int storageExecutionTime2 = requestContext.TableStorageExecutionTime;
        int storageExecutionCount2 = requestContext.TableStorageExecutionCount;
        int busExecutionTime = requestContext.ServiceBusExecutionTime;
        int busExecutionCount = requestContext.ServiceBusExecutionCount;
        int clientExecutionTime = requestContext.VssClientExecutionTime;
        int clientExecutionCount = requestContext.VssClientExecutionCount;
        int retryExecutionTime = requestContext.SqlRetryExecutionTime;
        int retryExecutionCount = requestContext.SqlRetryExecutionCount;
        int onlyExecutionTime = requestContext.SqlReadOnlyExecutionTime;
        int onlyExecutionCount = requestContext.SqlReadOnlyExecutionCount;
        long cpuCycles = requestContext.CPUCycles;
        int commandExecutionTime = requestContext.FinalSqlCommandExecutionTime;
        Guid e2Eid = requestContext.E2EId;
        Guid persistentSessionId = requestContext.PersistentSessionId;
        Guid authenticationSessionId1 = requestContext.PendingAuthenticationSessionId;
        Guid authenticationSessionId2 = requestContext.CurrentAuthenticationSessionId;
        long queueTime = requestContext.QueueTime;
        double tstUs = requestContext.TSTUs;
        string throttleReason = requestContext.ThrottleReason;
        string referrer = requestContext.Referrer;
        string uriStem = requestContext.UriStem;
        int supportsPublicAccess = (int) requestContext.SupportsPublicAccess;
        Guid authorizationId = requestContext.AuthorizationId;
        long informationTimeout = requestContext.MethodInformationTimeout;
        long preControllerTime = requestContext.PreControllerTime;
        long controllerTime = requestContext.ControllerTime;
        long postControllerTime = requestContext.PostControllerTime;
        string orchestrationId = requestContext.OrchestrationId;
        int docDbExecutionTime = requestContext.DocDBExecutionTime;
        int dbExecutionCount = requestContext.DocDBExecutionCount;
        int docDbrUsConsumed = requestContext.DocDBRUsConsumed;
        long allocatedBytes = requestContext.AllocatedBytes;
        string smartRouterStatus = requestContext.SmartRouterStatus;
        string smartRouterReason = requestContext.SmartRouterReason;
        string smartRouterTarget = requestContext.SmartRouterTarget;
        Guid oauthAppId = requestContext.OAuthAppId;
        this.TraceActivityLog(instanceId, contextId, application, command2, status, executionCount, startTime2, delayTime2, concurrencySemaphoreTime2, executionTime2, identityName, ipAddress, uniqueIdentifer, userAgent2, commandIdentifier, exceptionType, exceptionMessage, activityId2, responseCode, vsid2, cuid, tenantId, providerId, timeToFirstPage, activityStatus, authenticationMechanism, executionTimeThreshold, num2 != 0, logicalReads, physicalReads, cpuTime, elapsedTime, feature, hostStartTime, (byte) hostType, parentHostId, anonymousIdentifier, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, graphExecutionTime, graphExecutionCount, tokenExecutionTime, tokenExecutionCount, storageExecutionTime1, storageExecutionCount1, storageExecutionTime2, storageExecutionCount2, busExecutionTime, busExecutionCount, clientExecutionTime, clientExecutionCount, retryExecutionTime, retryExecutionCount, onlyExecutionTime, onlyExecutionCount, cpuCycles, commandExecutionTime, e2Eid, persistentSessionId, authenticationSessionId1, authenticationSessionId2, queueTime, tstUs, throttleReason, referrer, uriStem, (byte) supportsPublicAccess, authorizationId, informationTimeout, preControllerTime, controllerTime, postControllerTime, orchestrationId, docDbExecutionTime, dbExecutionCount, docDbrUsConsumed, allocatedBytes, smartRouterStatus, smartRouterReason, smartRouterTarget, oauthAppId);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceActivityLog(
      Guid hostId,
      long contextId,
      string application,
      string command,
      int status,
      int executionCount,
      DateTime startTime,
      long delayTime,
      long concurrencySemaphoreTime,
      long executionTime,
      string identityName,
      string ipAddress,
      Guid uniqueIdentifer,
      string userAgent,
      string commandIdentifier,
      string exceptionType,
      string exceptionMessage,
      Guid activityId,
      int responseCode,
      Guid vsid,
      Guid cuid,
      Guid tenantId,
      Guid providerId,
      long timeToFirstPage,
      int activityStatus,
      string authenticationMechanism,
      long executionTimeThreshold,
      bool isExceptionExpected,
      int logicalReads,
      int physicalReads,
      int cpuTime,
      int elapsedTime,
      string feature,
      DateTime hostStartTime,
      byte hostType,
      Guid parentHostId,
      string anonymousIdentifier,
      int sqlExecutionTime,
      int sqlExecutionCount,
      int redisExecutionTime,
      int redisExecutionCount,
      int aadGraphExecutionTime,
      int aadGraphExecutionCount,
      int aadTokenExecutionTime,
      int aadTokenExecutionCount,
      int blobStorageExecutionTime,
      int blobStorageExecutionCount,
      int tableStorageExecutionTime,
      int tableStorageExecutionCount,
      int serviceBusExecutionTime,
      int serviceBusExecutionCount,
      int vssClientExecutionTime,
      int vssClientExecutionCount,
      int sqlRetryExecutionTime,
      int sqlRetryExecutionCount,
      int sqlReadOnlyExecutionTime,
      int sqlReadOnlyExecutionCount,
      long cpuCycles,
      int finalSqlCommandExecutionTime,
      Guid E2EId,
      Guid persistentSessionId,
      Guid pendingAuthenticationSessionId,
      Guid currentAuthenticationSessionId,
      long queueTime,
      double TSTUs,
      string throttleReason,
      string referrer,
      string uriStem,
      byte supportsPublicAccess,
      Guid authorizationId,
      long methodInformationTimeout,
      long preControllerTime,
      long controllerTime,
      long postControllerTime,
      string orchestrationId,
      int docDBExecutionTime,
      int docDBExecutionCount,
      int docDBRUsConsumed,
      long allocatedBytes,
      string smartRouterStatus,
      string smartRouterReason,
      string smartRouterTarget,
      Guid oAuthAppId)
    {
      TeamFoundationTracingService.NormalizeString(ref application);
      TeamFoundationTracingService.NormalizeString(ref command);
      TeamFoundationTracingService.NormalizeString(ref identityName);
      TeamFoundationTracingService.NormalizeString(ref ipAddress);
      TeamFoundationTracingService.NormalizeAndEscapeString(ref userAgent);
      TeamFoundationTracingService.NormalizeString(ref commandIdentifier);
      TeamFoundationTracingService.NormalizeString(ref exceptionType);
      TeamFoundationTracingService.NormalizeAndEscapeString(ref exceptionMessage);
      TeamFoundationTracingService.NormalizeString(ref feature);
      TeamFoundationTracingService.NormalizeString(ref anonymousIdentifier);
      TeamFoundationTracingService.NormalizeString(ref authenticationMechanism);
      TeamFoundationTracingService.NormalizeString(ref throttleReason);
      TeamFoundationTracingService.NormalizeString(ref referrer);
      TeamFoundationTracingService.NormalizeString(ref uriStem);
      TeamFoundationTracingService.NormalizeDateTime(ref startTime);
      TeamFoundationTracingService.NormalizeDateTime(ref hostStartTime);
      TeamFoundationTracingService.NormalizeString(ref orchestrationId);
      TeamFoundationTracingService.NormalizeString(ref smartRouterStatus);
      TeamFoundationTracingService.NormalizeString(ref smartRouterReason);
      TeamFoundationTracingService.NormalizeString(ref smartRouterTarget);
      try
      {
        if (this.m_jobAndActivityLogTracingEnabled)
        {
          TeamFoundationTracingService.s_eventProvider.TraceActivityLog(hostId, contextId, application, command, status, executionCount, startTime, delayTime, concurrencySemaphoreTime, executionTime, identityName, ipAddress, uniqueIdentifer, userAgent, commandIdentifier, exceptionType, exceptionMessage, activityId, responseCode, vsid, cuid, tenantId, providerId, timeToFirstPage, activityStatus, authenticationMechanism, executionTimeThreshold, isExceptionExpected, logicalReads, physicalReads, cpuTime, elapsedTime, feature, hostStartTime, hostType, parentHostId, anonymousIdentifier, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime, aadGraphExecutionCount, aadTokenExecutionTime, aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, cpuCycles, finalSqlCommandExecutionTime, E2EId, persistentSessionId, pendingAuthenticationSessionId, currentAuthenticationSessionId, queueTime, TSTUs, throttleReason, referrer, uriStem, supportsPublicAccess, authorizationId, methodInformationTimeout, preControllerTime, controllerTime, postControllerTime, orchestrationId, docDBExecutionTime, docDBExecutionCount, docDBRUsConsumed, allocatedBytes, smartRouterStatus, smartRouterReason, smartRouterTarget, oAuthAppId);
          TeamFoundationTracingService.s_eventProvider.TraceActivityLogCore(hostId, application, command, status, executionCount, startTime, executionTime, userAgent, exceptionType, vsid, cuid, tenantId, timeToFirstPage, activityStatus, isExceptionExpected, feature, hostType, parentHostId, anonymousIdentifier, activityId, uniqueIdentifer, cpuTime, elapsedTime, delayTime, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime, aadGraphExecutionCount, aadTokenExecutionTime, aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, supportsPublicAccess, pendingAuthenticationSessionId, uriStem);
          if (TeamFoundationTracingService.s_enableActivityLogMapping)
            TeamFoundationTracingService.s_eventProvider.TraceActivityLogMapping(activityId, E2EId, identityName, ipAddress, anonymousIdentifier, cuid, vsid, startTime);
        }
        if (!TeamFoundationTracingService.s_enableUserImpactTracing || activityStatus == 0)
          return;
        this.m_userImpactedTime[DateTime.UtcNow.Minute].TryAdd(cuid, true);
      }
      catch (Exception ex)
      {
      }
    }

    public bool TraceOrchestrationLog(
      string orchestrationId,
      DateTime startTime,
      DateTime endTime,
      long executionTimeThreshold,
      byte orchestrationStatus,
      string application,
      string feature,
      string command,
      string exceptionType,
      string exceptionMessage,
      bool isExceptionExpected,
      byte hostType,
      Guid hostId,
      Guid parentHostId,
      Guid vsid,
      Guid cuid,
      string anonymousIdentifier)
    {
      TeamFoundationTracingService.NormalizeString(ref orchestrationId);
      TeamFoundationTracingService.NormalizeDateTime(ref startTime);
      TeamFoundationTracingService.NormalizeDateTime(ref endTime);
      TeamFoundationTracingService.NormalizeString(ref application);
      TeamFoundationTracingService.NormalizeString(ref feature);
      TeamFoundationTracingService.NormalizeString(ref command);
      TeamFoundationTracingService.NormalizeString(ref exceptionType);
      TeamFoundationTracingService.NormalizeString(ref anonymousIdentifier);
      if (!string.IsNullOrEmpty(exceptionMessage) && exceptionMessage.Length > 8192)
        exceptionMessage = exceptionMessage.Substring(0, 8192);
      TeamFoundationTracingService.NormalizeString(ref exceptionMessage);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceOrchestrationLogEvent(orchestrationId, startTime, endTime, executionTimeThreshold, orchestrationStatus, application, feature, command, exceptionType, exceptionMessage, isExceptionExpected, hostType, hostId, parentHostId, vsid, cuid, anonymousIdentifier);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public bool TraceServiceBusMetrics(
      Guid executionId,
      string serviceBusNamespace,
      string skuTier,
      DateTime startingIntervalUTC,
      double totalSuccessfulRequests,
      double totalServerErrors,
      double totalUserErrors,
      double totalThrottledRequests,
      double totalIncomingRequests,
      double totalIncomingMessages,
      double totalOutgoingMessages,
      double totalActiveConnections,
      double averageSizeInBytes,
      double averageMessages,
      double averageActiveMessages)
    {
      TeamFoundationTracingService.NormalizeDateTime(ref startingIntervalUTC);
      TeamFoundationTracingService.NormalizeString(ref serviceBusNamespace);
      TeamFoundationTracingService.NormalizeString(ref skuTier);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceServiceBusMetrics(executionId, serviceBusNamespace, skuTier, startingIntervalUTC, totalSuccessfulRequests, totalServerErrors, totalUserErrors, totalThrottledRequests, totalIncomingRequests, totalIncomingMessages, totalOutgoingMessages, totalActiveConnections, averageSizeInBytes, averageMessages, averageActiveMessages);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public bool TraceRedisCacheMetrics(
      Guid executionId,
      string redisCacheInstance,
      string skuTier,
      DateTime startingIntervalUTC,
      double totalConnectedClients,
      double totalCommandsProcessed,
      double totalCacheHits,
      double totalCacheMisses,
      double totalUsedMemory,
      double totalUsedMemoryRss,
      double totalServerLoad,
      double totalProcessorTime,
      double totalOperationsPerSecond,
      double totalGetCommands,
      double totalSetCommands,
      double totalEvictedKeys,
      double totalTotalKeys,
      double totalExpiredKeys,
      double totalUsedMemoryPercentage,
      double totalCacheRead,
      double totalErrors)
    {
      TeamFoundationTracingService.NormalizeDateTime(ref startingIntervalUTC);
      TeamFoundationTracingService.NormalizeString(ref redisCacheInstance);
      TeamFoundationTracingService.NormalizeString(ref skuTier);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceRedisCacheMetrics(executionId, redisCacheInstance, skuTier, startingIntervalUTC, totalConnectedClients, totalCommandsProcessed, totalCacheHits, totalCacheMisses, totalUsedMemory, totalUsedMemoryRss, totalServerLoad, totalProcessorTime, totalOperationsPerSecond, totalGetCommands, totalSetCommands, totalEvictedKeys, totalTotalKeys, totalExpiredKeys, totalUsedMemoryPercentage, totalCacheRead, totalErrors);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public bool TraceAzureSearchMetrics(
      Guid executionId,
      string azureSearchInstance,
      string skuTier,
      DateTime startingIntervalUTC,
      double searchLatency,
      double queriesPerSecond,
      double throttledSearchQueriesPercentage)
    {
      TeamFoundationTracingService.NormalizeDateTime(ref startingIntervalUTC);
      TeamFoundationTracingService.NormalizeString(ref azureSearchInstance);
      TeamFoundationTracingService.NormalizeString(ref skuTier);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceAzureSearchMetrics(executionId, azureSearchInstance, skuTier, startingIntervalUTC, searchLatency, queriesPerSecond, throttledSearchQueriesPercentage);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public bool TraceDocDBStorageMetrics(
      Guid executionId,
      string accountName,
      string databaseCategory,
      string databaseId,
      string collectionId,
      long collectionSizeUsage,
      long collectionSizeQuota,
      string partitionKeyRangeId,
      long partitionKeyRangeDocumentCount,
      long partitionKeyRangeSize,
      string partitionKeyRangeDominantPartitionKeys)
    {
      TeamFoundationTracingService.NormalizeString(ref accountName);
      TeamFoundationTracingService.NormalizeString(ref databaseCategory);
      TeamFoundationTracingService.NormalizeString(ref databaseId);
      TeamFoundationTracingService.NormalizeString(ref collectionId);
      TeamFoundationTracingService.NormalizeString(ref partitionKeyRangeId);
      TeamFoundationTracingService.NormalizeString(ref partitionKeyRangeDominantPartitionKeys);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceDocDBStorageMetrics(executionId, accountName, databaseCategory, databaseId, collectionId, collectionSizeUsage, collectionSizeQuota, partitionKeyRangeId, partitionKeyRangeDocumentCount, partitionKeyRangeSize, partitionKeyRangeDominantPartitionKeys);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public virtual bool TraceDocDBRUMetrics(
      string dbAccountName,
      string databaseCategory,
      string databaseId,
      string collectionId,
      string documentType,
      long consumedRUs)
    {
      TeamFoundationTracingService.NormalizeString(ref dbAccountName);
      TeamFoundationTracingService.NormalizeString(ref databaseCategory);
      TeamFoundationTracingService.NormalizeString(ref databaseId);
      TeamFoundationTracingService.NormalizeString(ref collectionId);
      TeamFoundationTracingService.NormalizeString(ref documentType);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceDocDBRUMetrics(dbAccountName, databaseCategory, databaseId, collectionId, documentType, consumedRUs);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public bool TraceQDS(
      long runId,
      string serverName,
      string listener,
      string databaseName,
      DateTime start,
      DateTime end,
      string queryText,
      long queryId,
      string objectName,
      long queryTextId,
      long planId,
      long totalPhysicalReads,
      long totalCpuTime,
      long averageRowCount,
      long totalExecutions,
      long totalLogicalReads,
      long averageCpuTime,
      long totalAborted,
      long totalExceptions,
      long totalLogicalWrites,
      double averageDop,
      long averageQueryMaxUsedMemory,
      long queryHash,
      long queryPlanHash)
    {
      TeamFoundationTracingService.NormalizeString(ref queryText);
      TeamFoundationTracingService.NormalizeString(ref objectName);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref listener);
      TeamFoundationTracingService.s_eventProvider.TraceQDS(runId, serverName, listener, databaseName, start, end, queryText, queryId, objectName, queryTextId, planId, totalPhysicalReads, totalCpuTime, averageRowCount, totalExecutions, totalLogicalReads, averageCpuTime, totalAborted, totalExceptions, totalLogicalWrites, averageDop, averageQueryMaxUsedMemory, queryHash, queryPlanHash);
      return true;
    }

    public void TraceJobAgentHistory(
      IVssRequestContext requestContext,
      string plugin,
      string jobName,
      Guid jobSource,
      Guid jobId,
      DateTime queueTime,
      DateTime startTime,
      long executionTime,
      Guid agentId,
      int result,
      string message,
      int queuedReasons,
      int queueFlags,
      short priority,
      int logicalReads,
      int physicalReads,
      int cpuTime,
      int elapsedTime,
      int sqlExecutionTime,
      int sqlExecutionCount,
      int redisExecutionTime,
      int redisExecutionCount,
      int aadGraphExecutionTime,
      int aadGraphExecutionCount,
      int aadTokenExecutionTime,
      int aadTokenExecutionCount,
      int blobStorageExecutionTime,
      int blobStorageExecutionCount,
      int tableStorageExecutionTime,
      int tableStorageExecutionCount,
      int serviceBusExecutionTime,
      int serviceBusExecutionCount,
      int vssClientExecutionTime,
      int vssClientExecutionCount,
      int sqlRetryExecutionTime,
      int sqlRetryExecutionCount,
      int sqlReadOnlyExecutionTime,
      int sqlReadOnlyExecutionCount,
      long cpuCycles,
      int finalSqlCommandExecutionTime,
      Guid E2EId,
      int docDBExecutionTime,
      int docDBExecutionCount,
      int docDBRUsConsumed,
      long allocatedBytes,
      Guid requesterActivityId,
      Guid requesterVsid,
      long cpuyclesAsync,
      long allocatedBytesAsync)
    {
      if (!this.m_jobAndActivityLogTracingEnabled)
        return;
      string jobFeature = TeamFoundationTracingService.GetJobFeature(requestContext, plugin);
      TeamFoundationTracingService.TraceJobAgentHistoryRaw(plugin, jobName, jobSource, jobId, queueTime, startTime, executionTime, agentId, result, message, queuedReasons, queueFlags, priority, logicalReads, physicalReads, cpuTime, elapsedTime, jobFeature, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime, aadGraphExecutionCount, aadTokenExecutionTime, aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, cpuCycles, finalSqlCommandExecutionTime, E2EId, docDBExecutionTime, docDBExecutionCount, docDBRUsConsumed, allocatedBytes, requesterActivityId, requesterVsid, cpuyclesAsync, allocatedBytesAsync);
    }

    public bool TraceFeatureFlagStatus(
      long runId,
      string FeatureName,
      string EffectiveState,
      string ExplicitState,
      string HostId = null,
      string VSID = null)
    {
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceFeatureFlagStatus(runId, FeatureName, EffectiveState, ExplicitState, HostId, VSID);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static string GetJobFeature(IVssRequestContext requestContext, string plugin)
    {
      string feature;
      try
      {
        if (!requestContext.To(TeamFoundationHostType.Deployment).GetService<FeatureMappingService>().TryGetJobFeatureMapping(requestContext, plugin, out feature))
          feature = JobFeatureResult.Unknown;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(15040005, TraceLevel.Error, "TracingService", "IVssFrameworkService", "Caught exception while calling FeatureMappingService: {0}", (object) ex);
        feature = JobFeatureResult.Unknown;
      }
      return feature;
    }

    internal static void TraceJobAgentHistoryRaw(
      string plugin,
      string jobName,
      Guid jobSource,
      Guid jobId,
      DateTime queueTime,
      DateTime startTime,
      long executionTime,
      Guid agentId,
      int result,
      string message,
      int queuedReasons,
      int queueFlags,
      short priority,
      int logicalReads,
      int physicalReads,
      int cpuTime,
      int elapsedTime,
      string feature,
      int sqlExecutionTime,
      int sqlExecutionCount,
      int redisExecutionTime,
      int redisExecutionCount,
      int aadGraphExecutionTime,
      int aadGraphExecutionCount,
      int aadTokenExecutionTime,
      int aadTokenExecutionCount,
      int blobStorageExecutionTime,
      int blobStorageExecutionCount,
      int tableStorageExecutionTime,
      int tableStorageExecutionCount,
      int serviceBusExecutionTime,
      int serviceBusExecutionCount,
      int vssClientExecutionTime,
      int vssClientExecutionCount,
      int sqlRetryExecutionTime,
      int sqlRetryExecutionCount,
      int sqlReadOnlyExecutionTime,
      int sqlReadOnlyExecutionCount,
      long cpuCycles,
      int finalSqlCommandExecutionTime,
      Guid E2EId,
      int docDBExecutionTime,
      int docDBExecutionCount,
      int docDBRUsConsumed,
      long allocatedBytes,
      Guid requesterActivityId,
      Guid requesterVsid,
      long cpuyclesAsync,
      long allocatedBytesAsync)
    {
      TeamFoundationTracingService.NormalizeString(ref message);
      TeamFoundationTracingService.NormalizeString(ref feature);
      try
      {
        TraceLevel traceLevel;
        switch (result)
        {
          case 1:
          case 5:
            traceLevel = TraceLevel.Warning;
            break;
          case 2:
            traceLevel = TraceLevel.Error;
            break;
          default:
            traceLevel = TraceLevel.Info;
            break;
        }
        TeamFoundationTracingService.s_eventProvider.TraceJobAgentHistory(traceLevel, plugin, jobName, jobSource, jobId, queueTime, startTime, executionTime, agentId, result, message, queuedReasons, queueFlags, priority, logicalReads, physicalReads, cpuTime, elapsedTime, feature, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime, aadGraphExecutionCount, aadTokenExecutionTime, aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, cpuCycles, finalSqlCommandExecutionTime, E2EId, docDBExecutionTime, docDBExecutionCount, docDBRUsConsumed, allocatedBytes, requesterActivityId, requesterVsid, cpuyclesAsync, allocatedBytesAsync);
        TeamFoundationTracingService.s_eventProvider.TraceJobHistoryCore(traceLevel, plugin, jobName, jobSource, jobId, queueTime, startTime, executionTime, agentId, result, queuedReasons, queueFlags, priority, logicalReads, physicalReads, cpuTime, elapsedTime, feature, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime, aadGraphExecutionCount, aadTokenExecutionTime, aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, cpuCycles, finalSqlCommandExecutionTime, E2EId);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceJobAgentJobStarted(
      IVssRequestContext requestContext,
      string plugin,
      string jobName,
      Guid jobSource,
      Guid jobId,
      DateTime queueTime,
      DateTime startTime,
      Guid agentId,
      int queuedReasons,
      int queueFlags,
      short priority,
      Guid E2EId,
      Guid requesterActivityId,
      Guid requesterVsid)
    {
      if (!this.m_jobAndActivityLogTracingEnabled)
        return;
      string feature;
      if (!requestContext.To(TeamFoundationHostType.Deployment).GetService<FeatureMappingService>().TryGetJobFeatureMapping(requestContext, plugin, out feature))
        feature = JobFeatureResult.Unknown;
      TeamFoundationTracingService.TraceJobAgentJobStartedRaw(plugin, jobName, jobSource, jobId, queueTime, startTime, agentId, queuedReasons, queueFlags, priority, feature, E2EId, requesterActivityId, requesterVsid);
    }

    internal static void TraceJobAgentJobStartedRaw(
      string plugin,
      string jobName,
      Guid jobSource,
      Guid jobId,
      DateTime queueTime,
      DateTime startTime,
      Guid agentId,
      int queuedReasons,
      int queueFlags,
      short priority,
      string feature,
      Guid E2EId,
      Guid requesterActivityId,
      Guid requesterVsid)
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider?.TraceJobAgentJobStarted(plugin, jobName, jobSource, jobId, queueTime, startTime, agentId, queuedReasons, queueFlags, priority, feature, E2EId, requesterActivityId, requesterVsid);
      }
      catch (Exception ex)
      {
      }
    }

    public static void TraceEventNotification(
      DateTime createdDate,
      string eventTaskName,
      Guid hostId,
      string eventType,
      string identifier,
      Guid activityId,
      string dataFeed)
    {
      try
      {
        TeamFoundationTracingService.NormalizeDateTime(ref createdDate);
        TeamFoundationTracingService.NormalizeString(ref eventTaskName);
        TeamFoundationTracingService.NormalizeString(ref eventType);
        TeamFoundationTracingService.NormalizeString(ref identifier);
        TeamFoundationTracingService.NormalizeString(ref dataFeed);
        TeamFoundationTracingService.s_eventProvider.TraceEventNotification("EventNotification", createdDate, eventTaskName, hostId, eventType, identifier, dataFeed);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(6303000, nameof (TraceEventNotification), nameof (TeamFoundationTracingService), ex);
      }
    }

    public void TraceSqlRunningStatus(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      short sessionId,
      int seconds,
      double elapsedTime,
      string command,
      short blockingSessionId,
      short headBlockerSesssionId,
      int blockingLevel,
      string text,
      string statement,
      string blockerQueryText,
      string waitType,
      int waitTime,
      string lastWaitType,
      string waitResource,
      long reads,
      long writes,
      long logicalReads,
      int cpuTime,
      int grantedQueryMemory,
      long requestedMemory,
      long maxUsedMemory,
      short dop,
      string queryPlan,
      bool isReadOnly,
      string loginName)
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceSqlRunningStatus(executionId, serverName, listener, databaseName, sessionId, seconds, elapsedTime, command, blockingSessionId, headBlockerSesssionId, blockingLevel, text, statement, blockerQueryText, waitType, waitTime, lastWaitType, waitResource, reads, writes, logicalReads, cpuTime, grantedQueryMemory, requestedMemory, maxUsedMemory, dop, queryPlan, isReadOnly, loginName);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceServiceBusSubscriberActivity(
      IVssRequestContext requestContext,
      string sbNamespace,
      string topicName,
      string plugin,
      string messageId,
      string contentType,
      Guid sourceInstanceId,
      Guid sourceInstanceType,
      bool success,
      Exception ex,
      DateTime startTime,
      int logicalReads,
      int physicalReads,
      int cpuTime,
      long cpuCycles,
      int elapsedTime,
      ref WellKnownPerformanceTimings timings)
    {
      if (!this.m_jobAndActivityLogTracingEnabled)
        return;
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceServiceBusSubscriberActivity(requestContext, sbNamespace, topicName, plugin, messageId, contentType, sourceInstanceId, sourceInstanceType, success, ex, startTime, logicalReads, physicalReads, cpuTime, cpuCycles, elapsedTime, ref timings);
      }
      catch (Exception ex1)
      {
      }
    }

    public void TraceServiceBusMetadataActivity(
      IVssRequestContext requestContext,
      Guid hostId,
      byte hostType,
      string sbNamespace,
      string topicName,
      string messageId,
      string contentType,
      string targetScaleUnits,
      bool success,
      DateTime startTime,
      long publishTimeMs)
    {
      if (!this.m_jobAndActivityLogTracingEnabled)
        return;
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceServiceBusMetadataActivity(requestContext, hostId, hostType, sbNamespace, topicName, messageId, contentType, targetScaleUnits, success, startTime, publishTimeMs);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceDatabasePerformanceStatistics(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      DateTime periodStart,
      Decimal averageCpuPercentage,
      Decimal maximumCpuPercentage,
      Decimal averageDataIOPercentage,
      Decimal maximumDataIOPercentage,
      Decimal averageLogWriteUtilizationPercentage,
      Decimal maximumLogWriteUtilizationPercentage,
      Decimal averageMemoryUsagePercentage,
      Decimal maximumMemoryUsagePercentage,
      string serviceObjective,
      Decimal averageWorkerPercent,
      Decimal maximumWorkerPercent,
      Decimal averageSessionPercent,
      Decimal maximumSessionPercent,
      short dtuLimit,
      string poolName,
      bool isReadOnly,
      Decimal averageXtpStoragePercent,
      Decimal maximumXtpStoragePercent,
      string resourceVersion,
      int schedulers,
      Decimal averageInstanceCpuPercenatage,
      Decimal averageInstanceMemoryPercentage,
      short replicaRole,
      short compatibilityLevel,
      long redoQueueSize,
      long redoRate,
      Decimal secondaryLagSeconds,
      short synchronizationHealth,
      string serviceObjectiveHardware)
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceDatabasePerformanceStatistics(executionId, serverName, listener, databaseName, periodStart, averageCpuPercentage, maximumCpuPercentage, averageDataIOPercentage, maximumDataIOPercentage, averageLogWriteUtilizationPercentage, maximumLogWriteUtilizationPercentage, averageMemoryUsagePercentage, maximumMemoryUsagePercentage, serviceObjective, averageWorkerPercent, maximumWorkerPercent, averageSessionPercent, maximumSessionPercent, dtuLimit, poolName, isReadOnly, averageXtpStoragePercent, maximumXtpStoragePercent, resourceVersion, schedulers, averageInstanceCpuPercenatage, averageInstanceMemoryPercentage, replicaRole, compatibilityLevel, redoQueueSize, redoRate, secondaryLagSeconds, synchronizationHealth, serviceObjectiveHardware);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceServicingJobDetail(ServicingJobDetail jobDetail)
    {
      try
      {
        TraceLevel traceLevel;
        switch (jobDetail.JobStatus)
        {
          case ServicingJobStatus.Complete:
            switch (jobDetail.Result)
            {
              case ServicingJobResult.Failed:
                traceLevel = TraceLevel.Error;
                break;
              case ServicingJobResult.PartiallySucceeded:
                traceLevel = TraceLevel.Warning;
                break;
              default:
                traceLevel = TraceLevel.Info;
                break;
            }
            break;
          case ServicingJobStatus.Failed:
            traceLevel = TraceLevel.Error;
            break;
          default:
            traceLevel = TraceLevel.Info;
            break;
        }
        double durationInSeconds = 0.0;
        if (jobDetail.EndTime != new DateTime() && jobDetail.StartTime != new DateTime())
          durationInSeconds = (jobDetail.EndTime - jobDetail.StartTime).TotalSeconds;
        string operations = string.Empty;
        if (jobDetail.Operations != null)
          operations = string.Join(",", jobDetail.Operations);
        TeamFoundationTracingService.s_eventProvider?.TraceServicingJobDetail(traceLevel, jobDetail.HostId, jobDetail.JobId, jobDetail.OperationClass ?? string.Empty, operations, jobDetail.JobStatusValue, EnumUtility.ToString<ServicingJobStatus>(jobDetail.JobStatus), jobDetail.ResultValue, EnumUtility.ToString<ServicingJobResult>(jobDetail.Result), jobDetail.QueueTime, jobDetail.StartTime, jobDetail.EndTime, durationInSeconds, jobDetail.CompletedStepCount, jobDetail.TotalStepCount);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceServicingStepDetail(
      Guid jobId,
      DateTime queueTime,
      ServicingStepDetail stepDetail)
    {
      string empty = string.Empty;
      if (!this.m_servicingStepDetailEnabled)
        return;
      try
      {
        TraceLevel traceLevel;
        string message;
        if (stepDetail is ServicingStepStateChange servicingStepStateChange)
        {
          switch (servicingStepStateChange.StepState)
          {
            case ServicingStepState.ValidatedWithWarnings:
            case ServicingStepState.PassedWithWarnings:
              traceLevel = TraceLevel.Warning;
              break;
            case ServicingStepState.Failed:
              traceLevel = TraceLevel.Error;
              break;
            default:
              traceLevel = TraceLevel.Info;
              break;
          }
          message = servicingStepStateChange.Message;
        }
        else
        {
          ServicingStepLogEntry servicingStepLogEntry = (ServicingStepLogEntry) stepDetail;
          switch (servicingStepLogEntry.EntryKind)
          {
            case ServicingStepLogEntryKind.Warning:
              traceLevel = TraceLevel.Warning;
              break;
            case ServicingStepLogEntryKind.Error:
              traceLevel = TraceLevel.Error;
              break;
            default:
              traceLevel = TraceLevel.Info;
              break;
          }
          empty = EnumUtility.ToString<ServicingStepLogEntryKind>(servicingStepLogEntry.EntryKind);
          message = servicingStepLogEntry.Message;
        }
        TeamFoundationTracingService.s_eventProvider.TraceServicingStepDetail(traceLevel, stepDetail.DetailTime, message, stepDetail.ServicingOperation ?? string.Empty, stepDetail.ServicingStepGroupId ?? string.Empty, stepDetail.ServicingStepId ?? string.Empty, empty, jobId, queueTime);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceTableSpaceUsage(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      TableSpaceUsage tableSpaceUsage)
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceTableSpaceUsage(executionId, serverName, listener, databaseName, tableSpaceUsage);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceStorageMetricsTransactions(
      Guid executionId,
      string storageAccount,
      string storageService,
      DateTime startingIntervalUTC,
      long totalIngress,
      long totalEgress,
      long totalRequests,
      long totalBillableRequests,
      double availability,
      double averageE2ELatency,
      double averageServerLatency,
      double percentSuccess,
      double percentThrottlingError,
      double percentTimeoutError,
      double percentServerOtherError,
      double percentClientOtherError,
      double percentAuthorizationError,
      double percentNetworkError,
      long success,
      long anonymousSuccess,
      long sasSuccess,
      long throttlingError,
      long anonymousThrottlingError,
      long sasThrottlingError,
      long clientTimeoutError,
      long anonymousClientTimeoutError,
      long sasClientTimeoutError,
      long serverTimeoutError,
      long anonymousServerTimeoutError,
      long sasServerTimeoutError,
      long clientOtherError,
      long sasClientOtherError,
      long anonymousClientOtherError,
      long serverOtherError,
      long anonymousServerOtherError,
      long sasServerOtherError,
      long authorizationError,
      long anonymousAuthorizationError,
      long sasAuthorizationError,
      long networkError,
      long anonymousNetworkError,
      long sasNetworkError,
      string operationType,
      string storageCluster,
      string storageKind,
      string storageSku,
      DateTime blobGeoLastSyncTime,
      DateTime tableGeoLastSyncTime,
      DateTime queueGeoLastSyncTime)
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceStorageMetricsTransactions(executionId, storageAccount, storageService, startingIntervalUTC, totalIngress, totalEgress, totalRequests, totalBillableRequests, availability, averageE2ELatency, averageServerLatency, percentSuccess, percentThrottlingError, percentTimeoutError, percentServerOtherError, percentClientOtherError, percentAuthorizationError, percentNetworkError, success, anonymousSuccess, sasSuccess, throttlingError, anonymousThrottlingError, sasThrottlingError, clientTimeoutError, anonymousClientTimeoutError, sasClientTimeoutError, serverTimeoutError, anonymousServerTimeoutError, sasServerTimeoutError, clientOtherError, sasClientOtherError, anonymousClientOtherError, serverOtherError, anonymousServerOtherError, sasServerOtherError, authorizationError, anonymousAuthorizationError, sasAuthorizationError, networkError, anonymousNetworkError, sasNetworkError, operationType, storageCluster, storageKind, storageSku, blobGeoLastSyncTime, tableGeoLastSyncTime, queueGeoLastSyncTime);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceStorageAnalyticsLogs(
      Guid executionId,
      string storageAccount,
      string storageService,
      string versionNumber,
      DateTime requestStartTime,
      string operationType,
      string requestStatus,
      string httpStatusCode,
      string endtoEndLatencyInMs,
      string serverLatencyInMs,
      string authenticationType,
      string requesterAccountName,
      string ownerAccountName,
      string requestUrl,
      string requestedObjectKey,
      Guid requestIdHeader,
      long operationCount,
      string requesterIpAddress,
      string requestVersionHeader,
      long requestHeaderSize,
      long requestPacketSize,
      long responseHeaderSize,
      long responsePacketSize,
      long requestContentLength,
      string requestMd5,
      string serverMd5,
      string etagIdentifier,
      DateTime lastModifiedTime,
      string conditionsUsed,
      string userAgentHeader,
      string referrerHeader,
      string clientRequestId)
    {
      try
      {
        TeamFoundationTracingService.s_eventProvider.TraceStorageAnalyticsLogs(executionId, storageAccount, storageService, versionNumber, requestStartTime, operationType, requestStatus, httpStatusCode, endtoEndLatencyInMs, serverLatencyInMs, authenticationType, requesterAccountName, ownerAccountName, requestUrl, requestedObjectKey, requestIdHeader, operationCount, requesterIpAddress, requestVersionHeader, requestHeaderSize, requestPacketSize, responseHeaderSize, responsePacketSize, requestContentLength, requestMd5, serverMd5, etagIdentifier, lastModifiedTime, conditionsUsed, userAgentHeader, referrerHeader, clientRequestId);
      }
      catch (Exception ex)
      {
      }
    }

    public void TraceResourceUtilization(
      IVssRequestContext requestContext,
      int channel,
      string dataFeed)
    {
      TeamFoundationTracingService.s_eventProvider.TraceResourceUtilization(requestContext.StartTime(), requestContext.ServiceHost.InstanceId, requestContext.GetUserId(), requestContext.GetUserCuid(), requestContext.ActivityId, channel, dataFeed);
    }

    public void TraceHostingServiceCertInformation(
      Guid executionId,
      string source,
      string thumbprint,
      string serialNumber,
      string subjectName,
      string issuerName,
      string signatureAlgorithm,
      DateTime created,
      DateTime expiry)
    {
      TeamFoundationTracingService.s_eventProvider.TraceHostingServiceCertInformation(executionId, source, thumbprint, serialNumber, subjectName, issuerName, signatureAlgorithm, created, expiry);
    }

    public void TraceSqlServerAlwaysOnHealthStatsInformation(
      Guid executionId,
      string listenerName,
      Guid groupId,
      string groupName,
      Guid replicaId,
      string replicaServerName,
      Guid replicaDatabaseId,
      string databaseName,
      string connectedStateDesc,
      string availabilityModeDesc,
      string synchronizationStateDesc,
      string replicaRoleDesc,
      int isLocal,
      int isJoined,
      int isSuspended,
      string suspendReasonDesc,
      int isFailoverReady,
      int estimatedDataLossIfFailoverNotReadyInSec,
      double estimatedRecoveryTime,
      long fileStreamSendRate,
      long logSendQueueSize,
      long logSendRate,
      long redoQueueSize,
      long redoRate,
      double synchronizationPerformance,
      string lastCommitLsn,
      DateTime lastCommitTime,
      string lastHardenedLsn,
      DateTime lastHardenedTime,
      string lastReceivedLsn,
      DateTime lastReceivedTime,
      string lastSentLsn,
      DateTime lastSentTime,
      string lastRedoneLsn,
      DateTime lastRedoneTime,
      string endOfLogLsn,
      string recoveryLsn,
      string truncationLsn,
      DateTime collectionTime)
    {
      TeamFoundationTracingService.s_eventProvider.TraceSqlServerAlwaysOnHealthStatsInformation(executionId, listenerName, groupId, groupName, replicaId, replicaServerName, replicaDatabaseId, databaseName, connectedStateDesc, availabilityModeDesc, synchronizationStateDesc, replicaRoleDesc, isLocal, isJoined, isSuspended, suspendReasonDesc, isFailoverReady, estimatedDataLossIfFailoverNotReadyInSec, estimatedRecoveryTime, fileStreamSendRate, logSendQueueSize, logSendRate, redoQueueSize, redoRate, synchronizationPerformance, lastCommitLsn, lastCommitTime, lastHardenedLsn, lastHardenedTime, lastReceivedLsn, lastReceivedTime, lastSentLsn, lastSentTime, lastRedoneLsn, lastRedoneTime, endOfLogLsn, recoveryLsn, truncationLsn, collectionTime);
    }

    public void TraceXEventData(
      DateTime eventTime,
      int sequenceNumber,
      Guid activityId,
      Guid uniqueIdentifier,
      Guid hostId,
      Guid vsid,
      ContextType type,
      string objectName,
      string actions,
      string fields,
      string serverName,
      string databaseName,
      string xeventTypeName)
    {
      TeamFoundationTracingService.s_eventProvider.TraceXEventData(eventTime, sequenceNumber, activityId, uniqueIdentifier, hostId, vsid, type, objectName, actions, fields, serverName, databaseName, xeventTypeName);
    }

    public void TraceXEventDataRPCCompleted(
      DateTime eventTime,
      int sequenceNumber,
      Guid activityId,
      Guid uniqueIdentifier,
      Guid hostId,
      Guid vsid,
      ContextType type,
      bool isGoverned,
      string objectName,
      ulong cpuTime,
      ulong duration,
      ulong physicalReads,
      ulong logicalReads,
      ulong writes,
      string result,
      ulong rowCount,
      string connectionResetOption,
      string statement,
      double TSTUs,
      string serverName,
      string databaseName,
      bool isReadScaleOut)
    {
      TeamFoundationTracingService.s_eventProvider.TraceXEventDataRPCCompleted(eventTime, sequenceNumber, activityId, uniqueIdentifier, hostId, vsid, type, isGoverned, objectName, cpuTime, duration, physicalReads, logicalReads, writes, result, rowCount, connectionResetOption, statement, TSTUs, serverName, databaseName, isReadScaleOut);
    }

    public void TraceMemoryClerks(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      string clerkName,
      long size,
      bool isReadonly)
    {
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref clerkName);
      TeamFoundationTracingService.NormalizeString(ref listener);
      TeamFoundationTracingService.s_eventProvider.TraceMemoryClerks(executionId, serverName, listener, databaseName, clerkName, size, isReadonly);
    }

    public void TracePackagingTraces(
      IVssRequestContext requestContext,
      Guid activityId,
      string collectionHostName,
      Guid hostId,
      string protocol,
      string command,
      string feedId,
      string feedName,
      string viewId,
      string viewName,
      string packageName,
      string packageVersion,
      string packageStorageId,
      int responseCode,
      bool isSlow,
      bool isTimingTraceEnabled,
      long timeToFirstPageInMs,
      long executionTimeInMs,
      long queueTimeInMs,
      bool isFailed,
      string exceptionType,
      string exceptionMessage,
      string identityName,
      string userAgent,
      string clientSessionId,
      string refererHeader,
      string sourceIp,
      string hostIp,
      string buildNumber,
      string commitId,
      string dataCurrentVersion,
      string dataDestinationVersion,
      string dataMigrationState,
      string featureFlagsOn,
      string featureFlagsOff,
      string stackTrace,
      string uri,
      string httpMethod,
      Guid relatedActivityId,
      Guid e2EID,
      string properties,
      Guid projectId,
      string projectVisibility,
      string orchestrationId)
    {
      string timingsTrace = (string) null;
      if (isSlow | isTimingTraceEnabled)
        timingsTrace = (requestContext.RequestTimer as ITimeRequestInternal).GetDiagnosticDumper(requestContext.ServiceHost.ServiceHostInternal().TotalExecutionElapsedThreshold * 1000L).ToString();
      TeamFoundationTracingService.s_eventProvider.TracePackagingTraces(activityId, collectionHostName, hostId, protocol, command, feedId, feedName, viewId, viewName, packageName, packageVersion, packageStorageId, responseCode, isSlow, timeToFirstPageInMs, executionTimeInMs, queueTimeInMs, isFailed, exceptionType, exceptionMessage, identityName, userAgent, clientSessionId, refererHeader, sourceIp, hostIp, buildNumber, commitId, dataCurrentVersion, dataDestinationVersion, dataMigrationState, featureFlagsOn, featureFlagsOff, stackTrace, timingsTrace, uri, httpMethod, relatedActivityId, e2EID, properties, projectId, projectVisibility, orchestrationId);
    }

    public void TraceResourceSemaphores(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      int resourceSemaphoreId,
      long targetMemoryKB,
      long maxTargetMemoryKB,
      long totalMemoryKB,
      long availableMemoryKB,
      long grantedMemoryKB,
      long usedMemoryKB,
      int granteeCount,
      int waiterCount,
      long timeoutErrorCount,
      long forcedGrantCount,
      int poolId,
      bool isReadonly)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref listener);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.s_eventProvider.TraceResourceSemaphores(executionId, serverName, listener, databaseName, resourceSemaphoreId, targetMemoryKB, maxTargetMemoryKB, totalMemoryKB, availableMemoryKB, grantedMemoryKB, usedMemoryKB, granteeCount, waiterCount, timeoutErrorCount, forcedGrantCount, poolId, isReadonly);
    }

    public void TraceSQLSpinlock(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      string name,
      long collisions,
      long spins,
      float spinsPerCollision,
      long sleepTime,
      long backoffs,
      bool isReadOnly)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref listener);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref name);
      TeamFoundationTracingService.s_eventProvider.TraceSQLSpinlock(executionId, serverName, listener, databaseName, name, collisions, spins, spinsPerCollision, sleepTime, backoffs, isReadOnly);
    }

    public void TraceVirtualFileStats(
      Guid executionId,
      string serverName,
      string databaseName,
      bool isReadOnly,
      short databaseId,
      short fileId,
      long sampleMs,
      long numReads,
      long numBytesRead,
      long ioStallReadMs,
      long numWrites,
      long numBytesWritten,
      long ioStallWriteMs,
      long ioStall,
      long sizeOnDiskBytes,
      long ioStallQueuedReadMs,
      long ioStallQueueWriteMs)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.s_eventProvider.TraceVirtualFileStats(executionId, serverName, databaseName, isReadOnly, databaseId, fileId, sampleMs, numReads, numBytesRead, ioStallReadMs, numWrites, numBytesWritten, ioStallWriteMs, ioStall, sizeOnDiskBytes, ioStallQueuedReadMs, ioStallQueueWriteMs);
    }

    public void TraceDatabaseConnectionInfo(
      Guid executionId,
      string serverName,
      string databaseName,
      bool isReadOnly,
      string hostName,
      string programName,
      int hostProcessId,
      int count,
      int inactiveCount,
      string sampleText)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref hostName);
      TeamFoundationTracingService.NormalizeString(ref programName);
      TeamFoundationTracingService.NormalizeString(ref sampleText);
      TeamFoundationTracingService.s_eventProvider.TraceDatabaseConnectionInfo(executionId, serverName, databaseName, isReadOnly, hostName, programName, hostProcessId, count, inactiveCount, sampleText);
    }

    public void TraceSqlRowLockInfo(
      Guid executionId,
      string serverName,
      string databaseName,
      bool isReadOnly,
      string schemaName,
      string tableName,
      string indexName,
      long hobtId,
      int objectId)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref schemaName);
      TeamFoundationTracingService.NormalizeString(ref tableName);
      TeamFoundationTracingService.NormalizeString(ref indexName);
      TeamFoundationTracingService.s_eventProvider.TraceSqlRowLockInfo(executionId, serverName, databaseName, isReadOnly, schemaName, tableName, indexName, hobtId, objectId);
    }

    public void TraceDatabasePrincipal(
      string serverName,
      string databaseName,
      int principalId,
      string principalName,
      string roleName,
      string permissions,
      string typeDesc,
      DateTime createTime,
      DateTime modifyTime)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref principalName);
      TeamFoundationTracingService.NormalizeString(ref roleName);
      TeamFoundationTracingService.NormalizeString(ref permissions);
      TeamFoundationTracingService.NormalizeString(ref typeDesc);
      TeamFoundationTracingService.s_eventProvider.TraceDatabasePrincipal(serverName, databaseName, principalId, principalName, roleName, permissions, typeDesc, createTime, modifyTime);
    }

    public void TraceXEventSession(
      string serverName,
      string databaseName,
      int sessionId,
      string sessionName,
      bool isEventFileTruncated,
      int buffersLogged,
      int buffersDropped,
      string eventFileName)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref sessionName);
      TeamFoundationTracingService.NormalizeString(ref eventFileName);
      TeamFoundationTracingService.s_eventProvider.TraceXEventSession(serverName, databaseName, sessionId, sessionName, isEventFileTruncated, buffersLogged, buffersDropped, eventFileName);
    }

    public void TraceGitThrottlingSettings(int size, int tarpit, int workunitSize) => TeamFoundationTracingService.s_eventProvider.TraceGitThrottlingSettings(size, tarpit, workunitSize);

    public bool TraceOrchestrationActivityLog(
      Guid hostId,
      string orchestrationId,
      string name,
      string version,
      DateTime queueTime,
      DateTime startTime,
      long executionTime,
      long cpuExecutionTime,
      string exceptionType,
      string exceptionMessage,
      Guid activityId,
      Guid e2eId,
      long cpuCycles,
      long allocatedBytes)
    {
      TeamFoundationTracingService.NormalizeString(ref orchestrationId);
      TeamFoundationTracingService.NormalizeString(ref name);
      TeamFoundationTracingService.NormalizeString(ref version);
      TeamFoundationTracingService.NormalizeDateTime(ref queueTime);
      TeamFoundationTracingService.NormalizeDateTime(ref startTime);
      TeamFoundationTracingService.NormalizeString(ref exceptionType);
      if (!string.IsNullOrEmpty(exceptionMessage) && exceptionMessage.Length > 8192)
        exceptionMessage = exceptionMessage.Substring(0, 8192);
      TeamFoundationTracingService.NormalizeString(ref exceptionMessage);
      try
      {
        return TeamFoundationTracingService.s_eventProvider.TraceOrchestrationActivityLogEvent(hostId, orchestrationId, name, version, queueTime, startTime, executionTime, cpuExecutionTime, exceptionType, exceptionMessage, activityId, e2eId, cpuCycles, allocatedBytes);
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public void TraceTuningRecommendations(
      long runId,
      string serverName,
      string listener,
      string databaseName,
      string name,
      string type,
      string reason,
      DateTime validSince,
      DateTime lastRefresh,
      string state,
      bool isExecutableAction,
      bool isRevertableAction,
      TuningAction executeAction,
      TuningAction revertAction,
      int score,
      string details)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref listener);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.NormalizeString(ref name);
      TeamFoundationTracingService.NormalizeString(ref type);
      TeamFoundationTracingService.NormalizeString(ref reason);
      TeamFoundationTracingService.NormalizeString(ref state);
      TeamFoundationTracingService.NormalizeString(ref details);
      TeamFoundationTracingService.s_eventProvider.TraceTuningRecommendation(runId, serverName, listener, databaseName, name, type, reason, validSince, lastRefresh, state, isExecutableAction, isRevertableAction, executeAction, revertAction, score, details);
    }

    public void TraceQueryOptimizerMemoryGateways(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      int poolId,
      int maxCount,
      int activeCount,
      int waiterCount,
      long thresholdFactor,
      long threshold,
      bool isActive,
      bool isReadonly)
    {
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref listener);
      TeamFoundationTracingService.NormalizeString(ref databaseName);
      TeamFoundationTracingService.s_eventProvider.TraceQueryOptimizerMemoryGateways(executionId, serverName, listener, databaseName, poolId, maxCount, activeCount, waiterCount, thresholdFactor, threshold, isActive, isReadonly);
    }

    public void TraceSqlPerformanceCounter(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      string counterName,
      long counterValue,
      bool isReadonly)
    {
      TeamFoundationTracingService.NormalizeString(ref counterName);
      TeamFoundationTracingService.NormalizeString(ref serverName);
      TeamFoundationTracingService.NormalizeString(ref listener);
      TeamFoundationTracingService.s_eventProvider.TraceSqlPerformanceCounter(executionId, serverName, listener, databaseName, counterName, counterValue, isReadonly);
    }

    public static void TraceHttpOutgoingRequest(
      DateTime startTime,
      int timeTaken,
      string httpClientName,
      string httpMethod,
      string urlHost,
      string urlPath,
      int responseCode,
      string errorMessage,
      Guid e2eId,
      string afdRefInfo,
      string requestPriority,
      Guid calledActivityId,
      string requestPhase,
      string orchestrationId)
    {
      TeamFoundationTracingService.s_eventProvider.TraceHttpOutgoingRequest(startTime, timeTaken, httpClientName ?? "", httpMethod ?? "", urlHost ?? "", urlPath ?? "", responseCode, errorMessage ?? "", e2eId, afdRefInfo ?? "", requestPriority ?? "", calledActivityId, requestPhase ?? "", orchestrationId ?? "", 0, 0, 0, 0, 0, 0, 0);
    }

    public static void TraceHttpOutgoingRequest(
      DateTime startTime,
      int timeTaken,
      string httpClientName,
      string httpMethod,
      string urlHost,
      string urlPath,
      int responseCode,
      string errorMessage,
      Guid e2eId,
      string afdRefInfo,
      string requestPriority,
      Guid calledActivityId,
      string requestPhase,
      string orchestrationId,
      int tokenRetries,
      int handlerStartTime,
      int bufferedRequestTime,
      int requestSendTime,
      int responseContentTime,
      int getTokenTime,
      int trailingTime)
    {
      TeamFoundationTracingService.s_eventProvider.TraceHttpOutgoingRequest(startTime, timeTaken, httpClientName ?? "", httpMethod ?? "", urlHost ?? "", urlPath ?? "", responseCode, errorMessage ?? "", e2eId, afdRefInfo ?? "", requestPriority ?? "", calledActivityId, requestPhase ?? "", orchestrationId ?? "", tokenRetries, handlerStartTime, bufferedRequestTime, requestSendTime, responseContentTime, getTokenTime, trailingTime);
    }

    public void TraceGeoReplicationLinkStatus(
      Guid executionId,
      string serverName,
      string listener,
      string databaseName,
      Guid linkGuid,
      string partnerServer,
      string partnerDatabase,
      DateTimeOffset lastReplication,
      int replicationLagSec,
      byte replicationState,
      string replicationStateDescription,
      byte role,
      string roleDescription,
      byte secondaryAllowConnections,
      string secondaryAllowConnectionsDescription,
      DateTimeOffset lastCommit)
    {
      TeamFoundationTracingService.s_eventProvider.TraceGeoReplicationLinkStatus(executionId, serverName, listener, databaseName, linkGuid, partnerServer, partnerDatabase, lastReplication, replicationLagSec, replicationState, replicationStateDescription, role, roleDescription, secondaryAllowConnections, secondaryAllowConnectionsDescription, lastCommit);
    }

    public void TraceCloudServiceRoleDetails(
      Guid executionId,
      Guid azureSubscriptionId,
      Guid azureSubscriptionAadTenantId,
      string roleType,
      int roleCountMin,
      int roleCount,
      int roleCountMax,
      string roleSize,
      int roleCores,
      long roleMemoryMB,
      string hostedServiceDnsName,
      string buildNumber,
      string osImageVersion,
      string osVersion,
      string appliedHotfixJson,
      bool encryptionAtHost,
      string securityType,
      bool secureBootEnabled,
      bool vTpmEnabled,
      string osDiskStorageAccountType,
      int osDiskSizeGB,
      string deploymentRing,
      int weekdayPeakRoleCountMin,
      int weekdayPeakRoleCountMax,
      string weekdayPeakStartTime,
      string weekdayPeakEndTime,
      int weekendPeakRoleCountMin,
      int weekendPeakRoleCountMax,
      string weekendPeakStartTime,
      string weekendPeakEndTime,
      string zones)
    {
      TeamFoundationTracingService.NormalizeString(ref roleType);
      TeamFoundationTracingService.NormalizeString(ref roleSize);
      TeamFoundationTracingService.NormalizeString(ref hostedServiceDnsName);
      TeamFoundationTracingService.NormalizeString(ref buildNumber);
      TeamFoundationTracingService.NormalizeString(ref osImageVersion);
      TeamFoundationTracingService.NormalizeString(ref osVersion);
      TeamFoundationTracingService.NormalizeString(ref appliedHotfixJson);
      TeamFoundationTracingService.NormalizeString(ref securityType);
      TeamFoundationTracingService.NormalizeString(ref osDiskStorageAccountType);
      TeamFoundationTracingService.NormalizeString(ref deploymentRing);
      TeamFoundationTracingService.NormalizeString(ref weekendPeakStartTime);
      TeamFoundationTracingService.NormalizeString(ref weekendPeakEndTime);
      TeamFoundationTracingService.NormalizeString(ref weekdayPeakStartTime);
      TeamFoundationTracingService.NormalizeString(ref weekdayPeakEndTime);
      TeamFoundationTracingService.NormalizeString(ref zones);
      TeamFoundationTracingService.s_eventProvider.TraceCloudServiceRoleDetails(executionId, azureSubscriptionId, azureSubscriptionAadTenantId, roleType, roleCountMin, roleCount, roleCountMax, roleSize, roleCores, roleMemoryMB, hostedServiceDnsName, buildNumber, osImageVersion, osVersion, appliedHotfixJson, encryptionAtHost, securityType, secureBootEnabled, vTpmEnabled, osDiskStorageAccountType, osDiskSizeGB, deploymentRing, weekdayPeakRoleCountMin, weekdayPeakRoleCountMax, weekdayPeakStartTime, weekdayPeakEndTime, weekendPeakRoleCountMin, weekendPeakRoleCountMax, weekendPeakStartTime, weekendPeakEndTime, zones);
    }

    public void TraceServicePrincipalIsMember(
      Guid servicePrincipalId,
      string groupSid,
      byte hostType,
      string stackTrace,
      int executionCount)
    {
      TeamFoundationTracingService.s_eventProvider.TraceServicePrincipalIsMember(servicePrincipalId, groupSid, hostType, stackTrace, executionCount);
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.WebApi.TraceFilter> QueryTraces(
      IVssRequestContext requestContext,
      Guid? ownerId = null)
    {
      using (TracingComponent component = requestContext.CreateComponent<TracingComponent>())
        return component.QueryTraces(requestContext, ownerId);
    }

    public virtual Microsoft.VisualStudio.Services.WebApi.TraceFilter QueryTrace(
      IVssRequestContext requestContext,
      Guid traceId)
    {
      ArgumentUtility.CheckForEmptyGuid(traceId, nameof (traceId));
      using (TracingComponent component = requestContext.CreateComponent<TracingComponent>())
        return component.QueryTrace(requestContext, traceId);
    }

    public virtual Guid StartTrace(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.WebApi.TraceFilter filters)
    {
      if (!filters.IsEnabled)
        filters.IsEnabled = true;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<TeamFoundationTracingService>().StartTrace(vssRequestContext, filters);
      }
      if (filters.OwnerId == new Guid())
        filters.OwnerId = requestContext.GetUserId();
      Guid guid = filters.TraceId;
      if (guid.Equals(Guid.Empty))
        guid = Guid.NewGuid();
      filters.TraceId = guid;
      using (TracingComponent component = requestContext.CreateComponent<TracingComponent>())
        component.CreateTrace(requestContext, filters);
      return guid;
    }

    public virtual void StopTrace(IVssRequestContext requestContext, Guid guid)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<TeamFoundationTracingService>().StopTrace(vssRequestContext, guid);
      }
      else
      {
        using (TracingComponent component = requestContext.CreateComponent<TracingComponent>())
          component.DeleteTrace(requestContext, guid);
      }
    }

    private static bool WriteEvent(
      TeamFoundationTracingService.TraceProvider provider,
      Guid traceId,
      int tracepoint,
      Guid serviceHost,
      long contextId,
      TraceLevel level,
      string processName,
      string username,
      string service,
      string method,
      string area,
      string layer,
      string userAgent,
      string uri,
      string path,
      string[] tags,
      string exceptionType,
      Guid vsid,
      Guid cuid,
      Guid tenantId,
      Guid providerId,
      string message,
      Guid uniqueIdentifier,
      Guid E2EId,
      string orchestrationId)
    {
      TeamFoundationTracingService.NormalizeString(ref processName);
      TeamFoundationTracingService.NormalizeString(ref username);
      TeamFoundationTracingService.NormalizeString(ref service);
      TeamFoundationTracingService.NormalizeString(ref method);
      TeamFoundationTracingService.NormalizeString(ref area);
      TeamFoundationTracingService.NormalizeString(ref layer);
      TeamFoundationTracingService.NormalizeString(ref userAgent);
      TeamFoundationTracingService.NormalizeString(ref uri);
      TeamFoundationTracingService.NormalizeString(ref path);
      TeamFoundationTracingService.NormalizeString(ref message);
      TeamFoundationTracingService.NormalizeString(ref exceptionType);
      TeamFoundationTracingService.NormalizeString(ref orchestrationId);
      TeamFoundationTracingService.TraceAndRedactDetectedEUII(tracepoint, ref message);
      string userDefined = tags == null || tags.Length == 0 ? string.Empty : (tags.Length != 1 ? string.Join(" : ", tags) : tags[0]);
      return provider.Trace(level, traceId, tracepoint, serviceHost, contextId, processName, username, service, method, area, layer, userAgent, uri, path, userDefined, exceptionType, vsid, cuid, tenantId, providerId, message, uniqueIdentifier, E2EId, orchestrationId);
    }

    private static void TraceAndRedactDetectedEUII(int tracepoint, ref string message)
    {
      if (TeamFoundationTracingService.s_euiiDetectionService == null)
        return;
      EuiiDetectionResult euiiDetectionResult = TeamFoundationTracingService.s_euiiDetectionService.DetectAndRedactEuii(tracepoint, ref message);
      if (!euiiDetectionResult.ShouldTraceCounterInfo)
        return;
      TeamFoundationTracingService.TraceRawAlwaysOn(323300, TraceLevel.Info, "EuiiDetectionService", nameof (TeamFoundationTracingService), (string[]) null, "Checked {0} traces, {1} {2} email leaks, spent {3} cpu cycles", euiiDetectionResult.RedactionEnabled ? (object) "masked" : (object) "detected", (object) euiiDetectionResult.TracesCheckedCount, (object) euiiDetectionResult.EmailLeakCount, (object) euiiDetectionResult.CPUCycles);
    }

    internal static bool WriteEventLowPriority(
      TeamFoundationTracingService.ILowPriorityTraceProvider provider,
      ref TraceEvent trace)
    {
      if (provider == null || trace.Level > TeamFoundationTracingService.s_lowPriorityProductTraceMaxLevel || TeamFoundationTracingService.s_lowPriorityProductTracePercentage == 0 || TeamFoundationTracingService.s_lowPriorityProductTracePercentage < 100 && Math.Abs(TeamFoundationTracingService.s_lowPriorityTraceCounter.Increment() % 100) >= TeamFoundationTracingService.s_lowPriorityProductTracePercentage)
        return false;
      string message = trace.GetMessage();
      string processName = trace.ProcessName;
      string userLogin = trace.UserLogin;
      string service = trace.Service;
      string method = trace.Method;
      string area = trace.Area;
      string layer = trace.Layer;
      string userAgent = trace.UserAgent;
      string uri = trace.Uri;
      string path = trace.Path;
      string exceptionType = trace.ExceptionType;
      string orchestrationId = trace.OrchestrationId;
      string[] tags = trace.Tags;
      TeamFoundationTracingService.NormalizeString(ref processName);
      TeamFoundationTracingService.NormalizeString(ref userLogin);
      TeamFoundationTracingService.NormalizeString(ref service);
      TeamFoundationTracingService.NormalizeString(ref method);
      TeamFoundationTracingService.NormalizeString(ref area);
      TeamFoundationTracingService.NormalizeString(ref layer);
      TeamFoundationTracingService.NormalizeString(ref userAgent);
      TeamFoundationTracingService.NormalizeString(ref uri);
      TeamFoundationTracingService.NormalizeString(ref path);
      TeamFoundationTracingService.NormalizeString(ref message);
      TeamFoundationTracingService.NormalizeString(ref exceptionType);
      TeamFoundationTracingService.NormalizeString(ref orchestrationId);
      string userDefined = string.Empty;
      if (tags != null && tags.Length != 0)
        userDefined = tags.Length != 1 ? string.Join(" : ", tags) : tags[0];
      return provider.TraceLowPriority(trace.Level, trace.Tracepoint, trace.ServiceHost, trace.ContextId, processName, userLogin, service, method, area, layer, userAgent, uri, path, userDefined, exceptionType, trace.VSID, trace.CUID, trace.TenantId, trace.ProviderId, message, trace.UniqueIdentifier, trace.E2EId, orchestrationId);
    }

    public int GetImpactedUsersMetric(int minute) => this.m_userImpactedTime[minute].Count;

    public void ClearStaleImpactedUsersMetric(int minute)
    {
      int num = (minute + 1) % 60;
      for (int index = 0; index < 60; ++index)
      {
        if (index != minute && index != num)
          this.m_userImpactedTime[index].Clear();
      }
    }

    internal static void NormalizeString(ref string message, bool forceEuiiAssert = false)
    {
      if (message == null)
        message = string.Empty;
      else
        TeamFoundationTracingService.CheckEuiiGates(message, forceEuiiAssert);
    }

    private static void CheckEuiiGates(string message, bool forceAssert)
    {
      if (!TeamFoundationTracingService.s_euiiGatesEnabled || !TeamFoundationTracingService.s_euiiGatesFeatureFlagEnabled || !EuiiUtility.ContainsEmail(message, TeamFoundationTracingService.s_euiiAssertOnDetection | forceAssert))
        return;
      TeamFoundationTracingService.TraceDetectedEUIIEvent(DateTime.UtcNow, new StackTrace().GetFrame(1).GetMethod().Name, EuiiType.Email, message);
    }

    internal static void NormalizeAndEscapeString(ref string message)
    {
      if (string.IsNullOrEmpty(message))
        message = string.Empty;
      else
        message = message.Replace("\0", "\\0");
    }

    internal static void NormalizeDateTime(ref DateTime dateTime)
    {
      if (!(dateTime < TeamFoundationTracingService.s_zeroDate))
        return;
      dateTime = TeamFoundationTracingService.s_zeroDate;
    }

    internal static string AnonymizeIpAddressString(string ipAddressString)
    {
      if (ipAddressString.IndexOf(',') < 0)
      {
        IPAddress address;
        if (IPAddress.TryParse(ipAddressString.Trim(), out address))
        {
          byte[] addressBytes = address.GetAddressBytes();
          if (address.AddressFamily == AddressFamily.InterNetwork)
            return string.Format("{0}.{1}.{2}.0", (object) addressBytes[0], (object) addressBytes[1], (object) addressBytes[2]);
          if (address.AddressFamily == AddressFamily.InterNetworkV6)
          {
            addressBytes[10] = (byte) 0;
            addressBytes[11] = (byte) 0;
            addressBytes[12] = (byte) 0;
            addressBytes[13] = (byte) 0;
            addressBytes[14] = (byte) 0;
            addressBytes[15] = (byte) 0;
            return new IPAddress(addressBytes).ToString();
          }
        }
        return ipAddressString;
      }
      string[] strArray = ipAddressString.Split(',');
      for (int index = 0; index < strArray.Length; ++index)
      {
        IPAddress address;
        if (IPAddress.TryParse(strArray[index].Trim(), out address))
        {
          byte[] addressBytes = address.GetAddressBytes();
          if (address.AddressFamily == AddressFamily.InterNetwork)
            addressBytes[3] = (byte) 0;
          else if (address.AddressFamily == AddressFamily.InterNetworkV6)
          {
            addressBytes[10] = (byte) 0;
            addressBytes[11] = (byte) 0;
            addressBytes[12] = (byte) 0;
            addressBytes[13] = (byte) 0;
            addressBytes[14] = (byte) 0;
            addressBytes[15] = (byte) 0;
          }
          strArray[index] = new IPAddress(addressBytes).ToString();
        }
      }
      return string.Join(", ", strArray);
    }

    internal static string GetActivityLoggingExceptionName(Exception exception)
    {
      if (exception == null)
        return (string) null;
      if (exception is HttpException httpException)
      {
        int httpCode = httpException.GetHttpCode();
        if (httpCode >= 400 && httpCode < 500)
          return exception.GetType().Name + httpCode.ToString();
      }
      return exception.GetType().Name;
    }

    internal void ReloadConfiguration(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        Microsoft.VisualStudio.Services.WebApi.TraceFilter[] array;
        using (TracingComponent component = requestContext.CreateComponent<TracingComponent>())
        {
          array = component.QueryTraces(requestContext).Where<Microsoft.VisualStudio.Services.WebApi.TraceFilter>((Func<Microsoft.VisualStudio.Services.WebApi.TraceFilter, bool>) (tf => tf.IsEnabled)).ToArray<Microsoft.VisualStudio.Services.WebApi.TraceFilter>();
          if (array.Length > 50)
            TeamFoundationTracingService.TraceRaw(9004, TraceLevel.Error, "TracingService", "IVssFrameworkService", "There are more than 50 enabled Team Foundation TraceFilters; this will cause poor performance.");
        }
        HashSet<Microsoft.VisualStudio.Services.WebApi.TraceFilter> traceFilterSet1 = new HashSet<Microsoft.VisualStudio.Services.WebApi.TraceFilter>((IEnumerable<Microsoft.VisualStudio.Services.WebApi.TraceFilter>) array, (IEqualityComparer<Microsoft.VisualStudio.Services.WebApi.TraceFilter>) TeamFoundationTracingService.s_traceFilterEqualityComparer);
        HashSet<Microsoft.VisualStudio.Services.WebApi.TraceFilter> traceFilterSet2 = new HashSet<Microsoft.VisualStudio.Services.WebApi.TraceFilter>((IEnumerable<Microsoft.VisualStudio.Services.WebApi.TraceFilter>) array, (IEqualityComparer<Microsoft.VisualStudio.Services.WebApi.TraceFilter>) TeamFoundationTracingService.s_traceFilterEqualityComparer);
        traceFilterSet2.SymmetricExceptWith((IEnumerable<Microsoft.VisualStudio.Services.WebApi.TraceFilter>) this.m_traces);
        foreach (Microsoft.VisualStudio.Services.WebApi.TraceFilter traceFilter in traceFilterSet2)
        {
          if (traceFilterSet1.Contains(traceFilter))
          {
            TeamFoundationTracingService.TraceRaw(9000, TraceLevel.Info, "TracingService", "IVssFrameworkService", "Started Trace {0}", (object) traceFilter.TraceId);
            this.m_traceCounts.TryAdd(traceFilter.TraceId, new TeamFoundationTracingService.ThreadSafeCounter());
          }
          else
          {
            TeamFoundationTracingService.TraceRaw(9001, TraceLevel.Info, "TracingService", "IVssFrameworkService", "Stopped Trace {0}", (object) traceFilter.TraceId);
            this.m_traceCounts.TryRemove(traceFilter.TraceId, out TeamFoundationTracingService.ThreadSafeCounter _);
          }
        }
        foreach (Microsoft.VisualStudio.Services.WebApi.TraceFilter trace in this.m_traces)
        {
          if (!trace.IsEnabled && traceFilterSet1.Contains(trace))
          {
            TeamFoundationTracingService.TraceRaw(9005, TraceLevel.Info, "TracingService", "IVssFrameworkService", "Resetting Trace Counter for Trace {0}", (object) trace.TraceId);
            this.m_traceCounts.AddOrUpdate(trace.TraceId, new TeamFoundationTracingService.ThreadSafeCounter(), (Func<Guid, TeamFoundationTracingService.ThreadSafeCounter, TeamFoundationTracingService.ThreadSafeCounter>) ((traceId, oldCounter) => new TeamFoundationTracingService.ThreadSafeCounter()));
          }
        }
        this.m_traces = array;
        if (!this.m_updateRawTraces)
          return;
        TeamFoundationTracingService.s_rawTraces = array;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(9002, TraceLevel.Error, "TracingService", "IVssFrameworkService", "Exception while loading traces: {0}", (object) ex);
      }
    }

    private void ReloadRegistrySettings(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      IEnumerable<RegistryItem> entries = SqlRegistryService.DeserializeSqlNotification(requestContext, eventData);
      if (!((IEnumerable<RegistryQuery>) TeamFoundationTracingService.s_notificationFilters).SelectMany<RegistryQuery, RegistryItem>((Func<RegistryQuery, IEnumerable<RegistryItem>>) (s => entries.Filter(s))).Any<RegistryItem>())
        return;
      this.LoadRegistrySettings(requestContext);
    }

    internal void LoadRegistrySettings(IVssRequestContext requestContext)
    {
      SqlRegistryService service1 = requestContext.GetService<SqlRegistryService>();
      TeamFoundationFeatureAvailabilityService service2 = requestContext.GetService<TeamFoundationFeatureAvailabilityService>();
      this.m_jobAndActivityLogTracingEnabled = service1.GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.TracingServiceJobAndActivityLogTracingEnabled, true);
      this.m_surveyEventTracingEnabled = service1.GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.TracingServiceSurveyEventsTracingEnabled, true);
      SqlRegistryService registryService1 = service1;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceMaxTracesPerTraceFilter;
      ref RegistryQuery local1 = ref registryQuery;
      int defaultValue = requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? -1 : 10000;
      this.m_maxTracesPerTraceFilter = registryService1.GetValue<int>(requestContext1, in local1, defaultValue);
      this.m_monitorTracesPerTraceFilter = this.m_maxTracesPerTraceFilter > 0;
      SqlRegistryService registryService2 = service1;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceServicingStepDetailEnabled;
      ref RegistryQuery local2 = ref registryQuery;
      this.m_servicingStepDetailEnabled = registryService2.GetValue<bool>(requestContext2, in local2, true);
      SqlRegistryService registryService3 = service1;
      IVssRequestContext requestContext3 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceEnableActivityLogMapping;
      ref RegistryQuery local3 = ref registryQuery;
      TeamFoundationTracingService.s_enableActivityLogMapping = registryService3.GetValue<bool>(requestContext3, in local3, true);
      SqlRegistryService registryService4 = service1;
      IVssRequestContext requestContext4 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceEnableImpactedUsersMetric;
      ref RegistryQuery local4 = ref registryQuery;
      TeamFoundationTracingService.s_enableUserImpactTracing = registryService4.GetValue<bool>(requestContext4, in local4, false);
      SqlRegistryService registryService5 = service1;
      IVssRequestContext requestContext5 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceEuiiGatesEnabled;
      ref RegistryQuery local5 = ref registryQuery;
      TeamFoundationTracingService.s_euiiGatesEnabled = registryService5.GetValue<bool>(requestContext5, in local5, false);
      IVssRequestContext requestContext6 = requestContext;
      string featureFlagEnabled = FrameworkServerConstants.TracingServiceEuiiGatesFeatureFlagEnabled;
      TeamFoundationTracingService.s_euiiGatesFeatureFlagEnabled = service2.IsFeatureEnabled(requestContext6, featureFlagEnabled);
      SqlRegistryService registryService6 = service1;
      IVssRequestContext requestContext7 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceEuiiAssertOnDetection;
      ref RegistryQuery local6 = ref registryQuery;
      TeamFoundationTracingService.s_euiiAssertOnDetection = registryService6.GetValue<bool>(requestContext7, in local6, false);
      SqlRegistryService registryService7 = service1;
      IVssRequestContext requestContext8 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceEnableCollectTracePublishedVsSkippedMetric;
      ref RegistryQuery local7 = ref registryQuery;
      TeamFoundationTracingService.s_collectTracePublishedVsSkippedMetricEnabled = registryService7.GetValue<bool>(requestContext8, in local7, false);
      SqlRegistryService registryService8 = service1;
      IVssRequestContext requestContext9 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceEnableLowPriorityProductTrace;
      ref RegistryQuery local8 = ref registryQuery;
      TeamFoundationTracingService.s_enableLowPriorityProductTrace = registryService8.GetValue<bool>(requestContext9, in local8, false);
      SqlRegistryService registryService9 = service1;
      IVssRequestContext requestContext10 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceLowPriorityProductTracePercentage;
      ref RegistryQuery local9 = ref registryQuery;
      TeamFoundationTracingService.s_lowPriorityProductTracePercentage = registryService9.GetValue<int>(requestContext10, in local9, 100);
      SqlRegistryService registryService10 = service1;
      IVssRequestContext requestContext11 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.TracingServiceLowPriorityProductTraceMaxLevel;
      ref RegistryQuery local10 = ref registryQuery;
      TeamFoundationTracingService.s_lowPriorityProductTraceMaxLevel = registryService10.GetValue<TraceLevel>(requestContext11, in local10, TraceLevel.Info);
    }

    private static sbyte GetWebSiteId()
    {
      sbyte webSiteId = -1;
      if (string.Equals(TeamFoundationTracingService.s_processName, "w3wp", StringComparison.OrdinalIgnoreCase))
      {
        string baseDirectory = AppContext.BaseDirectory;
        if (baseDirectory != null)
        {
          int num = baseDirectory.IndexOf("sitesroot\\", StringComparison.OrdinalIgnoreCase);
          sbyte result;
          if (num > 0 && sbyte.TryParse(baseDirectory[baseDirectory.Length - 1] != '\\' ? baseDirectory.Substring(num + "sitesroot\\".Length) : baseDirectory.Substring(num + "sitesroot\\".Length, baseDirectory.Length - (num + "sitesroot\\".Length + 1)), out result))
            webSiteId = result;
        }
      }
      return webSiteId;
    }

    private void OnPublishTestAlertRequested(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(9490, "AlertPublishing", nameof (TeamFoundationTracingService), nameof (OnPublishTestAlertRequested));
      try
      {
        PublishTestAlertRequest testAlertRequest = TeamFoundationSerializationUtility.Deserialize<PublishTestAlertRequest>(eventData);
        bool flag;
        if (testAlertRequest.ProcessId == Guid.Empty)
        {
          flag = true;
        }
        else
        {
          TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
          flag = testAlertRequest.ProcessId == service.ProcessId;
        }
        if (flag)
        {
          string message = string.Format("Test alert via sql notification. Target ProcessId: {0}; Message: {1}; EventValues: {2}", (object) testAlertRequest.ProcessId, (object) testAlertRequest.Message, (object) testAlertRequest.EventValues);
          string[] strArray;
          if (testAlertRequest.EventValues != null)
            strArray = testAlertRequest.EventValues.Split(',');
          else
            strArray = (string[]) null;
          TeamFoundationEventLog.Default.Log(requestContext, message, testAlertRequest.EventId, testAlertRequest.EventType, (object[]) strArray);
          requestContext.Trace(9491, TraceLevel.Info, "AlertPublishing", nameof (TeamFoundationTracingService), "Alert logged and possibly evaluated for publishing: {0}", (object) testAlertRequest);
        }
        else
          requestContext.Trace(9492, TraceLevel.Verbose, "AlertPublishing", nameof (TeamFoundationTracingService), "Alert not logged or published: {0}", (object) testAlertRequest);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9498, "AlertPublishing", nameof (TeamFoundationTracingService), ex);
      }
      finally
      {
        requestContext.TraceLeave(9499, "TracingService", "IVssFrameworkService", "OnPingRequestCreated");
      }
    }

    public bool IsStarted { get; private set; }

    private static double PageCountToMB(long pageCount) => (double) (pageCount * 8L) / 1024.0;

    private static void CollectTracePublishedVsSkippedMetric(bool isTracePublished, int tracepoint)
    {
      if (!TeamFoundationTracingService.s_collectTracePublishedVsSkippedMetricEnabled)
        return;
      string[] dimensionValues = new string[1]
      {
        tracepoint.ToString()
      };
      MdmService.PublishMetricRaw(isTracePublished ? "TracePublished" : "TraceSkipped", 1L, TeamFoundationTracingService.s_mdmDimensions, dimensionValues);
    }

    private class ThreadSafeCounter
    {
      private int m_count;

      public int Increment() => Interlocked.Increment(ref this.m_count);
    }

    internal interface ILowPriorityTraceProvider
    {
      bool TraceLowPriority(
        TraceLevel traceLevel,
        int tracepoint,
        Guid serviceHost,
        long contextId,
        string processName,
        string username,
        string service,
        string method,
        string area,
        string layer,
        string userAgent,
        string uri,
        string path,
        string userDefined,
        string exceptionType,
        Guid vsid,
        Guid cuid,
        Guid tenantId,
        Guid providerId,
        string message,
        Guid uniqueIdentifier,
        Guid E2EId,
        string orchestrationId);
    }

    public class TraceProvider : IDisposable, TeamFoundationTracingService.ILowPriorityTraceProvider
    {
      private EventProviderVersionTwo m_provider = new EventProviderVersionTwo(new Guid("{80761876-6844-47D5-8106-F8ED2AA8687B}"));
      private const byte c_eventVersion = 0;
      private const byte c_eventChannel = 16;
      private const byte c_eventOpcode = 10;
      private const int c_eventTask = 1;
      private static System.Diagnostics.Eventing.EventDescriptor s_activityLogEventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(6, (byte) 37, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
      private static System.Diagnostics.Eventing.EventDescriptor s_sqlEventDecriptor = new System.Diagnostics.Eventing.EventDescriptor(4, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
      private static System.Diagnostics.Eventing.EventDescriptor s_xEventSessionsEventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(85, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
      private static System.Diagnostics.Eventing.EventDescriptor s_databaseDetailsEventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(77, (byte) 4, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
      private static System.Diagnostics.Eventing.EventDescriptor s_databaseConnectionInfoEventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(78, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
      private static System.Diagnostics.Eventing.EventDescriptor s_sqlRowLockInfoEventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(83, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);

      internal static byte ConvertLevelToETW(TraceLevel traceLevel) => (byte) (traceLevel + 1);

      public bool TraceSQL(
        string dataSource,
        string database,
        string operation,
        short retries,
        bool success,
        int totalTime,
        int connectTime,
        int executionTime,
        int waitTime,
        int sqlErrorCode,
        string sqlErrorMessage)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              return this.m_provider.TemplateSQL(ref TeamFoundationTracingService.TraceProvider.s_sqlEventDecriptor, database, dataSource, operation, retries, success, totalTime, connectTime, executionTime, waitTime, sqlErrorCode, sqlErrorMessage);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceMachinePoolRequestHistory(
        string poolType,
        string poolName,
        string poolRegion,
        string imageVersion,
        string instanceName,
        long requestId,
        Guid hostId,
        string inputs,
        string outcome,
        string outputs,
        DateTime queueTime,
        DateTime assignTime,
        DateTime startTime,
        DateTime finishTime,
        DateTime unassignTime,
        Guid traceActivityId,
        int maxParallelism,
        string imageLabel,
        long timeoutSeconds,
        long slaSeconds,
        DateTime slaStartTime,
        string tags,
        Guid subscriptionId,
        string requiredResourceVersion,
        string suspiciousActivity,
        string orchestrationId)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(9, (byte) 6, (byte) 16, (byte) 4, (byte) 12, 1, -9223372036854775807L);
              return this.m_provider.TemplateMachinePoolRequestHistory7(ref eventDescriptor, poolType, poolName, instanceName, requestId, hostId, queueTime, assignTime, startTime, finishTime, unassignTime, outcome, inputs, outputs, traceActivityId, maxParallelism, imageLabel, timeoutSeconds, slaSeconds, slaStartTime, tags, subscriptionId, requiredResourceVersion, suspiciousActivity, orchestrationId, poolRegion, imageVersion);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceActivityLog(
        Guid hostId,
        long contextId,
        string application,
        string command,
        int status,
        int executionCount,
        DateTime startTime,
        long delayTime,
        long concurrencySemaphoreTime,
        long executionTime,
        string identityName,
        string ipAddress,
        Guid uniqueIdentifier,
        string userAgent,
        string commandIdentifier,
        string exceptionType,
        string exceptionMessage,
        Guid activityId,
        int responseCode,
        Guid vsid,
        Guid cuid,
        Guid tenantId,
        Guid providerId,
        long timeToFirstPage,
        int activityStatus,
        string authenticationMechanism,
        long executionTimeThreshold,
        bool isExceptionExpected,
        int logicalReads,
        int physicalReads,
        int cpuTime,
        int elapsedTime,
        string feature,
        DateTime hostStartTime,
        byte hostType,
        Guid parentHostId,
        string anonymousIdentifier,
        int sqlExecutionTime,
        int sqlExecutionCount,
        int redisExecutionTime,
        int redisExecutionCount,
        int aadGraphExecutionTime,
        int aadGraphExecutionCount,
        int aadTokenExecutionTime,
        int aadTokenExecutionCount,
        int blobStorageExecutionTime,
        int blobStorageExecutionCount,
        int tableStorageExecutionTime,
        int tableStorageExecutionCount,
        int serviceBusExecutionTime,
        int serviceBusExecutionCount,
        int vssClientExecutionTime,
        int vssClientExecutionCount,
        int sqlRetryExecutionTime,
        int sqlRetryExecutionCount,
        int sqlReadOnlyExecutionTime,
        int sqlReadOnlyExecutionCount,
        long cpuCycles,
        int finalSqlCommandExecutionTime,
        Guid E2EId,
        Guid persistentSessionId,
        Guid pendingAuthenticationSessionId,
        Guid currentAuthenticationSessionId,
        long queueTime,
        double TSTUs,
        string throttleReason,
        string referrer,
        string uriStem,
        byte supportsPublicAccess,
        Guid authorizationId,
        long methodInformationTimeout,
        long preControllerTime,
        long controllerTime,
        long postControllerTime,
        string orchestrationId,
        int docDBExecutionTime,
        int docDBExecutionCount,
        int docDBRUsConsumed,
        long allocatedBytes,
        string smartRouterStatus,
        string smartRouterReason,
        string smartRouterTarget,
        Guid oAuthAppId)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              EventProviderVersionTwo provider = this.m_provider;
              ref System.Diagnostics.Eventing.EventDescriptor local = ref TeamFoundationTracingService.TraceProvider.s_activityLogEventDescriptor;
              Guid HostId = hostId;
              long ContextId = contextId;
              string Application = application;
              string Command = command;
              int Status = status;
              int Count = executionCount;
              DateTime StartTime = startTime;
              long ExecutionTime = executionTime;
              string empty = string.Empty;
              string IPAddress = TeamFoundationTracingService.AnonymizeIpAddressString(ipAddress);
              Guid UniqueIdentifier = uniqueIdentifier;
              string UserAgent = userAgent;
              string CommandIdentifier = commandIdentifier;
              string ExceptionType = exceptionType;
              string ExceptionMessage = exceptionMessage;
              Guid ActivityId = activityId;
              int ResponseCode = responseCode;
              Guid VSID = vsid;
              long TimeToFirstPage = timeToFirstPage;
              int ActivityStatus = activityStatus;
              long ExecutionTimeThreshold = executionTimeThreshold;
              int num1 = isExceptionExpected ? 1 : 0;
              long DelayTime = delayTime;
              Guid RelatedActivityId = uniqueIdentifier;
              int LogicalReads = logicalReads;
              int PhysicalReads = physicalReads;
              int CPUTime = cpuTime;
              int ElapsedTime = elapsedTime;
              string Feature = feature;
              DateTime HostStartTime = hostStartTime;
              int HostType = (int) hostType;
              Guid ParentHostId = parentHostId;
              string AnonymousIdentifier = anonymousIdentifier;
              int SqlExecutionTime = sqlExecutionTime;
              int SqlExecutionCount = sqlExecutionCount;
              int RedisExecutionTime = redisExecutionTime;
              int RedisExecutionCount = redisExecutionCount;
              int AadExecutionTime = aadGraphExecutionTime + aadTokenExecutionTime;
              int AadExecutionCount = aadGraphExecutionCount + aadTokenExecutionCount;
              int num2 = aadGraphExecutionTime;
              int num3 = aadGraphExecutionCount;
              int num4 = aadTokenExecutionTime;
              int num5 = aadTokenExecutionCount;
              int BlobStorageExecutionTime = blobStorageExecutionTime;
              int BlobStorageExecutionCount = blobStorageExecutionCount;
              int TableStorageExecutionTime = tableStorageExecutionTime;
              int TableStorageExecutionCount = tableStorageExecutionCount;
              int ServiceBusExecutionTime = serviceBusExecutionTime;
              int ServiceBusExecutionCount = serviceBusExecutionCount;
              int VssClientExecutionTime = vssClientExecutionTime;
              int VssClientExecutionCount = vssClientExecutionCount;
              int SqlRetryExecutionTime = sqlRetryExecutionTime;
              int SqlRetryExecutionCount = sqlRetryExecutionCount;
              int SqlReadOnlyExecutionTime = sqlReadOnlyExecutionTime;
              int SqlReadOnlyExecutionCount = sqlReadOnlyExecutionCount;
              long CPUCycles = cpuCycles;
              int FinalSqlCommandExecutionTime = finalSqlCommandExecutionTime;
              Guid E2EID = E2EId;
              Guid PersistentSessionId = persistentSessionId;
              Guid PendingAuthenticationSessionId = pendingAuthenticationSessionId;
              Guid CurrentAuthenticationSessionId = currentAuthenticationSessionId;
              Guid CUID = cuid;
              Guid TenantId = tenantId;
              Guid ProviderId = providerId;
              long QueueTime = queueTime;
              string AuthenticationMechanism = authenticationMechanism;
              double TSTUs1 = TSTUs;
              int AadGraphExecutionTime = num2;
              int AadGraphExecutionCount = num3;
              int AadTokenExecutionTime = num4;
              int AadTokenExecutionCount = num5;
              string ThrottleReason = throttleReason;
              string Referrer = referrer;
              string UriStem = uriStem;
              int SupportsPublicAccess = (int) supportsPublicAccess;
              long ConcurrencySemaphoreTime = concurrencySemaphoreTime;
              Guid AuthorizationId = authorizationId;
              long MethodInformationTimeout = methodInformationTimeout;
              long PreControllerTime = preControllerTime;
              long ControllerTime = controllerTime;
              long PostControllerTime = postControllerTime;
              string OrchestrationId = orchestrationId;
              int DocDBExecutionTime = docDBExecutionTime;
              int DocDBExecutionCount = docDBExecutionCount;
              int DocDBRUsConsumed = docDBRUsConsumed;
              long AllocatedBytes = allocatedBytes;
              string SmartRouterStatus = smartRouterStatus;
              string SmartRouterReason = smartRouterReason;
              string SmartRouterTarget = smartRouterTarget;
              Guid OAuthAppId = oAuthAppId;
              int webSiteId = (int) TeamFoundationTracingService.s_webSiteId;
              return provider.TemplateActivityLog38(ref local, HostId, ContextId, Application, Command, Status, Count, StartTime, ExecutionTime, empty, IPAddress, UniqueIdentifier, UserAgent, CommandIdentifier, ExceptionType, ExceptionMessage, ActivityId, ResponseCode, VSID, TimeToFirstPage, ActivityStatus, ExecutionTimeThreshold, num1 != 0, DelayTime, RelatedActivityId, LogicalReads, PhysicalReads, CPUTime, ElapsedTime, Feature, HostStartTime, (byte) HostType, ParentHostId, AnonymousIdentifier, SqlExecutionTime, SqlExecutionCount, RedisExecutionTime, RedisExecutionCount, AadExecutionTime, AadExecutionCount, BlobStorageExecutionTime, BlobStorageExecutionCount, TableStorageExecutionTime, TableStorageExecutionCount, ServiceBusExecutionTime, ServiceBusExecutionCount, VssClientExecutionTime, VssClientExecutionCount, SqlRetryExecutionTime, SqlRetryExecutionCount, SqlReadOnlyExecutionTime, SqlReadOnlyExecutionCount, CPUCycles, FinalSqlCommandExecutionTime, E2EID, PersistentSessionId, PendingAuthenticationSessionId, CurrentAuthenticationSessionId, CUID, TenantId, ProviderId, QueueTime, AuthenticationMechanism, TSTUs1, AadGraphExecutionTime, AadGraphExecutionCount, AadTokenExecutionTime, AadTokenExecutionCount, ThrottleReason, Referrer, UriStem, (byte) SupportsPublicAccess, ConcurrencySemaphoreTime, AuthorizationId, MethodInformationTimeout, PreControllerTime, ControllerTime, PostControllerTime, OrchestrationId, DocDBExecutionTime, DocDBExecutionCount, DocDBRUsConsumed, AllocatedBytes, SmartRouterStatus, SmartRouterReason, SmartRouterTarget, OAuthAppId, (sbyte) webSiteId);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceActivityLogCore(
        Guid hostId,
        string application,
        string command,
        int status,
        int executionCount,
        DateTime startTime,
        long executionTime,
        string userAgent,
        string exceptionType,
        Guid vsid,
        Guid cuid,
        Guid tenantId,
        long timeToFirstPage,
        int activityStatus,
        bool isExceptionExpected,
        string feature,
        byte hostType,
        Guid parentHostId,
        string anonymousIdentifier,
        Guid activityId,
        Guid uniqueIdentifier,
        int cpuTime,
        int elapsedTime,
        long delayTime,
        int sqlExecutionTime,
        int sqlExecutionCount,
        int redisExecutionTime,
        int redisExecutionCount,
        int aadGraphExecutionTime,
        int aadGraphExecutionCount,
        int aadTokenExecutionTime,
        int aadTokenExecutionCount,
        int blobStorageExecutionTime,
        int blobStorageExecutionCount,
        int tableStorageExecutionTime,
        int tableStorageExecutionCount,
        int serviceBusExecutionTime,
        int serviceBusExecutionCount,
        int vssClientExecutionTime,
        int vssClientExecutionCount,
        int sqlRetryExecutionTime,
        int sqlRetryExecutionCount,
        int sqlReadOnlyExecutionTime,
        int sqlReadOnlyExecutionCount,
        byte supportsPublicAccess,
        Guid pendingAuthenticationSessionId,
        string uriStem)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(56, (byte) 5, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateActivityLogCore6(ref eventDescriptor, hostId, application, command, status, executionCount, startTime, executionTime, userAgent, exceptionType, vsid, timeToFirstPage, activityStatus, isExceptionExpected, feature, hostType, parentHostId, anonymousIdentifier, cuid, tenantId, activityId, uniqueIdentifier, cpuTime, elapsedTime, delayTime, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime + aadTokenExecutionTime, aadGraphExecutionCount + aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, supportsPublicAccess, pendingAuthenticationSessionId, uriStem);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceActivityLogMapping(
        Guid activityId,
        Guid e2eId,
        string identityName,
        string ipAddress,
        string anonymousIdentifier,
        Guid cuid,
        Guid vsid,
        DateTime startTime)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(59, (byte) 3, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateActivityLogMapping4(ref eventDescriptor, activityId, e2eId, identityName, ipAddress, anonymousIdentifier, cuid, vsid, startTime);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceQDS(
        long runId,
        string serverName,
        string listener,
        string databaseName,
        DateTime start,
        DateTime end,
        string queryText,
        long queryId,
        string objectName,
        long queryTextId,
        long planId,
        long totalPhysicalReads,
        long totalCpuTime,
        long averageRowCount,
        long totalExecutions,
        long totalLogicalReads,
        long averageCpuTime,
        long totalAborted,
        long totalExceptions,
        long totalLogicalWrites,
        double averageDop,
        long averageQueryMaxUsedMemory,
        long queryHash,
        long queryPlanHash)
      {
        System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(28, (byte) 2, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
        return this.m_provider.TemplateQDS3(ref eventDescriptor, runId, serverName, databaseName, start, end, queryText, queryId, objectName, queryTextId, planId, totalPhysicalReads, totalCpuTime, averageRowCount, totalExecutions, totalLogicalReads, averageCpuTime, totalAborted, totalExceptions, totalLogicalWrites, averageDop, averageQueryMaxUsedMemory, queryHash, queryPlanHash, listener);
      }

      internal bool TraceJobAgentHistory(
        TraceLevel traceLevel,
        string plugin,
        string jobName,
        Guid jobSource,
        Guid jobId,
        DateTime queueTime,
        DateTime startTime,
        long executionTime,
        Guid agentId,
        int result,
        string message,
        int queuedReasons,
        int queueFlags,
        short priority,
        int logicalReads,
        int physicalReads,
        int cpuTime,
        int elapsedTime,
        string feature,
        int sqlExecutionTime,
        int sqlExecutionCount,
        int redisExecutionTime,
        int redisExecutionCount,
        int aadGraphExecutionTime,
        int aadGraphExecutionCount,
        int aadTokenExecutionTime,
        int aadTokenExecutionCount,
        int blobStorageExecutionTime,
        int blobStorageExecutionCount,
        int tableStorageExecutionTime,
        int tableStorageExecutionCount,
        int serviceBusExecutionTime,
        int serviceBusExecutionCount,
        int vssClientExecutionTime,
        int vssClientExecutionCount,
        int sqlRetryExecutionTime,
        int sqlRetryExecutionCount,
        int sqlReadOnlyExecutionTime,
        int sqlReadOnlyExecutionCount,
        long cpuCycles,
        int finalSqlCommandExecutionTime,
        Guid E2EId,
        int docDBExecutionTime,
        int docDBExecutionCount,
        int docDBRUsConsumed,
        long allocatedBytes,
        Guid requesterActivityId,
        Guid requesterVsid,
        long cpuyclesAsync,
        long allocatedBytesAsync)
      {
        byte etw = TeamFoundationTracingService.TraceProvider.ConvertLevelToETW(traceLevel);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(etw, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(5, (byte) 14, (byte) 16, etw, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateJobAgentHistory15(ref eventDescriptor, plugin, jobName, jobSource, jobId, queueTime, startTime, executionTime, agentId, result, message, queuedReasons, queueFlags, priority, logicalReads, physicalReads, cpuTime, elapsedTime, feature, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime + aadTokenExecutionTime, aadGraphExecutionCount + aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, cpuCycles, finalSqlCommandExecutionTime, E2EId, aadGraphExecutionTime, aadGraphExecutionCount, aadTokenExecutionTime, aadTokenExecutionCount, docDBExecutionTime, docDBExecutionCount, docDBRUsConsumed, allocatedBytes, requesterActivityId, requesterVsid, cpuyclesAsync, allocatedBytesAsync);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceJobHistoryCore(
        TraceLevel traceLevel,
        string plugin,
        string jobName,
        Guid jobSource,
        Guid jobId,
        DateTime queueTime,
        DateTime startTime,
        long executionTime,
        Guid agentId,
        int result,
        int queuedReasons,
        int queueFlags,
        short priority,
        int logicalReads,
        int physicalReads,
        int cpuTime,
        int elapsedTime,
        string feature,
        int sqlExecutionTime,
        int sqlExecutionCount,
        int redisExecutionTime,
        int redisExecutionCount,
        int aadGraphExecutionTime,
        int aadGraphExecutionCount,
        int aadTokenExecutionTime,
        int aadTokenExecutionCount,
        int blobStorageExecutionTime,
        int blobStorageExecutionCount,
        int tableStorageExecutionTime,
        int tableStorageExecutionCount,
        int serviceBusExecutionTime,
        int serviceBusExecutionCount,
        int vssClientExecutionTime,
        int vssClientExecutionCount,
        int sqlRetryExecutionTime,
        int sqlRetryExecutionCount,
        int sqlReadOnlyExecutionTime,
        int sqlReadOnlyExecutionCount,
        long cpuCycles,
        int finalSqlCommandExecutionTime,
        Guid E2EId)
      {
        byte etw = TeamFoundationTracingService.TraceProvider.ConvertLevelToETW(traceLevel);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(etw, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(66, (byte) 0, (byte) 16, etw, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateJobHistoryCore(ref eventDescriptor, plugin, jobName, jobSource, jobId, queueTime, startTime, executionTime, agentId, result, queuedReasons, queueFlags, priority, logicalReads, physicalReads, cpuTime, elapsedTime, feature, sqlExecutionTime, sqlExecutionCount, redisExecutionTime, redisExecutionCount, aadGraphExecutionTime + aadTokenExecutionTime, aadGraphExecutionCount + aadTokenExecutionCount, blobStorageExecutionTime, blobStorageExecutionCount, tableStorageExecutionTime, tableStorageExecutionCount, serviceBusExecutionTime, serviceBusExecutionCount, vssClientExecutionTime, vssClientExecutionCount, sqlRetryExecutionTime, sqlRetryExecutionCount, sqlReadOnlyExecutionTime, sqlReadOnlyExecutionCount, cpuCycles, finalSqlCommandExecutionTime, E2EId, aadGraphExecutionTime, aadGraphExecutionCount, aadTokenExecutionTime, aadTokenExecutionCount);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceJobAgentJobStarted(
        string plugin,
        string jobName,
        Guid jobSource,
        Guid jobId,
        DateTime queueTime,
        DateTime startTime,
        Guid agentId,
        int queuedReasons,
        int queueFlags,
        short priority,
        string feature,
        Guid E2EId,
        Guid requesterActivityId,
        Guid requesterVsid)
      {
        TeamFoundationTracingService.NormalizeString(ref plugin);
        TeamFoundationTracingService.NormalizeString(ref jobName);
        TeamFoundationTracingService.NormalizeString(ref feature);
        TeamFoundationTracingService.NormalizeDateTime(ref queueTime);
        TeamFoundationTracingService.NormalizeDateTime(ref startTime);
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(50, (byte) 1, (byte) 16, level, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateJobAgentJobStarted2(ref eventDescriptor, plugin, jobName, jobSource, jobId, queueTime, startTime, agentId, queuedReasons, queueFlags, priority, feature, E2EId, requesterActivityId, requesterVsid);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceServiceBusSubscriberActivity(
        IVssRequestContext requestContext,
        string sbNamespace,
        string topicName,
        string plugin,
        string messageId,
        string contentType,
        Guid sourceInstanceId,
        Guid sourceInstanceType,
        bool success,
        Exception ex,
        DateTime startTime,
        int logicalReads,
        int physicalReads,
        int cpuTime,
        long cpuCycles,
        int elapsedTime,
        ref WellKnownPerformanceTimings timings)
      {
        System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(54, (byte) 3, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
        return this.m_provider.TemplateServiceBusActivity4(ref eventDescriptor, requestContext.ServiceHost.InstanceId, (byte) requestContext.ServiceHost.HostType, topicName, plugin, sourceInstanceId, sourceInstanceType, success, ex?.GetType().FullName ?? string.Empty, ex?.Message ?? string.Empty, startTime, logicalReads, physicalReads, requestContext.ActivityId, requestContext.E2EId, requestContext.UniqueIdentifier, cpuTime, elapsedTime, cpuCycles, timings.SqlExecutionTime, timings.SqlExecutionCount, timings.RedisExecutionTime, timings.RedisExecutionCount, timings.AadGraphExecutionTime, timings.AadGraphExecutionCount, timings.AadTokenExecutionTime, timings.AadTokenExecutionCount, timings.BlobStorageExecutionTime, timings.BlobStorageExecutionCount, timings.TableStorageExecutionTime, timings.TableStorageExecutionCount, timings.ServiceBusExecutionTime, timings.ServiceBusExecutionCount, timings.VssClientExecutionTime, timings.VssClientExecutionCount, timings.SqlRetryExecutionTime, timings.SqlRetryExecutionCount, timings.SqlReadOnlyExecutionTime, timings.SqlReadOnlyExecutionCount, timings.FinalSqlCommandExecutionTime, messageId ?? string.Empty, sbNamespace ?? string.Empty, contentType ?? string.Empty);
      }

      public bool TraceServiceBusMetadataActivity(
        IVssRequestContext requestContext,
        Guid hostId,
        byte hostType,
        string sbNamespace,
        string topicName,
        string messageId,
        string contentType,
        string targetScaleUnits,
        bool status,
        DateTime startTime,
        long publishTimeMs)
      {
        System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(61, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
        return this.m_provider.TemplateServiceBusPublishMetadata2(ref eventDescriptor, hostId, hostType, topicName, messageId ?? string.Empty, targetScaleUnits ?? string.Empty, status, startTime, publishTimeMs, requestContext.ActivityId, requestContext.E2EId, requestContext.UniqueIdentifier, sbNamespace ?? string.Empty, contentType ?? string.Empty);
      }

      public bool TraceSqlRunningStatus(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        short sessionId,
        int seconds,
        double elapsedTime,
        string command,
        short blockingSessionId,
        short headBlockerSessionId,
        int blockingLevel,
        string text,
        string statement,
        string blockerQueryText,
        string waitType,
        int waitTime,
        string lastWaitType,
        string waitResource,
        long reads,
        long writes,
        long logicalReads,
        int cpuTime,
        int grantedQueryMemory,
        long requestedMemory,
        long maxUsedMemory,
        short dop,
        string queryPlan,
        bool isReadOnly,
        string loginName)
      {
        TeamFoundationTracingService.NormalizeString(ref serverName);
        TeamFoundationTracingService.NormalizeString(ref listener);
        TeamFoundationTracingService.NormalizeString(ref databaseName);
        TeamFoundationTracingService.NormalizeString(ref command);
        TeamFoundationTracingService.NormalizeString(ref text);
        TeamFoundationTracingService.NormalizeString(ref statement);
        TeamFoundationTracingService.NormalizeString(ref blockerQueryText);
        TeamFoundationTracingService.NormalizeString(ref waitType);
        TeamFoundationTracingService.NormalizeString(ref lastWaitType);
        TeamFoundationTracingService.NormalizeString(ref waitResource);
        TeamFoundationTracingService.NormalizeString(ref queryPlan);
        TeamFoundationTracingService.NormalizeString(ref loginName);
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(11, (byte) 5, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateSqlRunningStatus6(ref eventDescriptor, executionId, serverName, databaseName, sessionId, seconds, elapsedTime, command, blockingSessionId, headBlockerSessionId, blockingLevel, text, statement, blockerQueryText, waitType, waitTime, lastWaitType, waitResource, reads, writes, logicalReads, cpuTime, grantedQueryMemory, requestedMemory, maxUsedMemory, dop, queryPlan, isReadOnly, listener, loginName);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceTableSpaceUsage(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        TableSpaceUsage tableSpaceUsage)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(24, (byte) 2, (byte) 16, level, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateTableSpaceUsage3(ref eventDescriptor, executionId, serverName, databaseName, tableSpaceUsage.SchemaName, tableSpaceUsage.TableName, tableSpaceUsage.IndexName ?? "", tableSpaceUsage.IndexType, tableSpaceUsage.Compression, tableSpaceUsage.IndexId, tableSpaceUsage.RowCount, TeamFoundationTracingService.PageCountToMB(tableSpaceUsage.UsedPageCount), TeamFoundationTracingService.PageCountToMB(tableSpaceUsage.ReservedPageCount), TeamFoundationTracingService.PageCountToMB(tableSpaceUsage.InRowUsedPageCount), TeamFoundationTracingService.PageCountToMB(tableSpaceUsage.InRowReservedPageCount), TeamFoundationTracingService.PageCountToMB(tableSpaceUsage.LobUsedPageCount), TeamFoundationTracingService.PageCountToMB(tableSpaceUsage.LobReservedPageCount), listener, tableSpaceUsage.PartitionNumber, tableSpaceUsage.PartitionBoundary ?? "");
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceDatabasePerformanceStatistics(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        DateTime periodStart,
        Decimal averageCpuPercentage,
        Decimal maximumCpuPercentage,
        Decimal averageDataIOPercentage,
        Decimal maximumDataIOPercentage,
        Decimal averageLogWriteUtilizationPercentage,
        Decimal maximumLogWriteUtilizationPercentage,
        Decimal averageMemoryUsagePercentage,
        Decimal maximumMemoryUsagePercentage,
        string serviceObjective,
        Decimal averageWorkerPercentage,
        Decimal maximumWorkerPercentage,
        Decimal averageSessionPercentage,
        Decimal maximumSessionPercentage,
        short dtuLimit,
        string poolName,
        bool isReadOnly,
        Decimal averageXtpStoragePercent,
        Decimal maximumXtpStoragePercent,
        string resourceVersion,
        int schedulers,
        Decimal averageInstanceCpuPercentage,
        Decimal averageInstanceMemoryPercentage,
        short replicaRole,
        short compatibilityLevel,
        long redoQueueSize,
        long redoRate,
        Decimal secondaryLagSeconds,
        short synchronizationHealth,
        string serviceObjectiveHardware)
      {
        TeamFoundationTracingService.NormalizeString(ref serverName);
        TeamFoundationTracingService.NormalizeString(ref listener);
        TeamFoundationTracingService.NormalizeString(ref databaseName);
        TeamFoundationTracingService.NormalizeString(ref poolName);
        TeamFoundationTracingService.NormalizeString(ref serviceObjective);
        TeamFoundationTracingService.NormalizeString(ref resourceVersion);
        TeamFoundationTracingService.NormalizeString(ref serviceObjectiveHardware);
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(12, (byte) 13, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateDatabasePerformanceStatistics14(ref eventDescriptor, executionId, serverName, databaseName, periodStart, Convert.ToDouble(averageCpuPercentage), Convert.ToDouble(maximumCpuPercentage), Convert.ToDouble(averageDataIOPercentage), Convert.ToDouble(maximumDataIOPercentage), Convert.ToDouble(averageLogWriteUtilizationPercentage), Convert.ToDouble(maximumLogWriteUtilizationPercentage), Convert.ToDouble(averageMemoryUsagePercentage), Convert.ToDouble(maximumMemoryUsagePercentage), serviceObjective, Convert.ToDouble(averageWorkerPercentage), Convert.ToDouble(maximumWorkerPercentage), Convert.ToDouble(averageSessionPercentage), Convert.ToDouble(maximumSessionPercentage), dtuLimit, poolName, isReadOnly, Convert.ToDouble(averageXtpStoragePercent), Convert.ToDouble(maximumXtpStoragePercent), resourceVersion, schedulers, listener, Convert.ToDouble(averageInstanceCpuPercentage), Convert.ToDouble(averageInstanceMemoryPercentage), replicaRole, compatibilityLevel, redoQueueSize, redoRate, Convert.ToDouble(secondaryLagSeconds), synchronizationHealth, serviceObjectiveHardware);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceStorageMetricsTransactions(
        Guid executionId,
        string storageAccount,
        string storageService,
        DateTime startingIntervalUTC,
        long totalIngress,
        long totalEgress,
        long totalRequests,
        long totalBillableRequests,
        double availability,
        double averageE2ELatency,
        double averageServerLatency,
        double percentSuccess,
        double percentThrottlingError,
        double percentTimeoutError,
        double percentServerOtherError,
        double percentClientOtherError,
        double percentAuthorizationError,
        double percentNetworkError,
        long success,
        long anonymousSuccess,
        long sasSuccess,
        long throttlingError,
        long anonymousThrottlingError,
        long sasThrottlingError,
        long clientTimeoutError,
        long anonymousClientTimeoutError,
        long sasClientTimeoutError,
        long serverTimeoutError,
        long anonymousServerTimeoutError,
        long sasServerTimeoutError,
        long clientOtherError,
        long sasClientOtherError,
        long anonymousClientOtherError,
        long serverOtherError,
        long anonymousServerOtherError,
        long sasServerOtherError,
        long authorizationError,
        long anonymousAuthorizationError,
        long sasAuthorizationError,
        long networkError,
        long anonymousNetworkError,
        long sasNetworkError,
        string operationType,
        string storageCluster,
        string storageKind,
        string storageSku,
        DateTime blobGeoLastSyncTime,
        DateTime tableGeoLastSyncTime,
        DateTime queueGeoLastSyncTime)
      {
        TeamFoundationTracingService.NormalizeString(ref storageAccount);
        TeamFoundationTracingService.NormalizeString(ref storageCluster);
        TeamFoundationTracingService.NormalizeString(ref storageService);
        TeamFoundationTracingService.NormalizeDateTime(ref startingIntervalUTC);
        TeamFoundationTracingService.NormalizeString(ref operationType);
        TeamFoundationTracingService.NormalizeString(ref storageKind);
        TeamFoundationTracingService.NormalizeString(ref storageSku);
        TeamFoundationTracingService.NormalizeDateTime(ref blobGeoLastSyncTime);
        TeamFoundationTracingService.NormalizeDateTime(ref tableGeoLastSyncTime);
        TeamFoundationTracingService.NormalizeDateTime(ref queueGeoLastSyncTime);
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(35, (byte) 4, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateStorageMetricsTransactions5(ref eventDescriptor, executionId, storageAccount, storageService, startingIntervalUTC, totalIngress, totalEgress, totalRequests, totalBillableRequests, Convert.ToDouble(availability), Convert.ToDouble(averageE2ELatency), Convert.ToDouble(averageServerLatency), Convert.ToDouble(percentSuccess), Convert.ToDouble(percentThrottlingError), Convert.ToDouble(percentTimeoutError), Convert.ToDouble(percentServerOtherError), Convert.ToDouble(percentClientOtherError), Convert.ToDouble(percentAuthorizationError), Convert.ToDouble(percentNetworkError), success, anonymousSuccess, sasSuccess, throttlingError, anonymousThrottlingError, sasThrottlingError, clientTimeoutError, anonymousClientTimeoutError, sasClientTimeoutError, serverTimeoutError, anonymousServerTimeoutError, sasServerTimeoutError, clientOtherError, sasClientOtherError, anonymousClientOtherError, serverOtherError, anonymousServerOtherError, sasServerOtherError, authorizationError, anonymousAuthorizationError, sasAuthorizationError, networkError, anonymousNetworkError, sasNetworkError, operationType, storageCluster, storageKind, storageSku, blobGeoLastSyncTime, tableGeoLastSyncTime, queueGeoLastSyncTime);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceStorageAnalyticsLogs(
        Guid executionId,
        string storageAccount,
        string storageService,
        string versionNumber,
        DateTime requestStartTime,
        string operationType,
        string requestStatus,
        string httpStatusCode,
        string endtoEndLatencyInMs,
        string serverLatencyInMs,
        string authenticationType,
        string requesterAccountName,
        string ownerAccountName,
        string requestUrl,
        string requestedObjectKey,
        Guid requestIdHeader,
        long operationCount,
        string requesterIpAddress,
        string requestVersionHeader,
        long requestHeaderSize,
        long requestPacketSize,
        long responseHeaderSize,
        long responsePacketSize,
        long requestContentLength,
        string requestMd5,
        string serverMd5,
        string etagIdentifier,
        DateTime lastModifiedTime,
        string conditionsUsed,
        string userAgentHeader,
        string referrerHeader,
        string clientRequestId)
      {
        TeamFoundationTracingService.NormalizeString(ref storageAccount);
        TeamFoundationTracingService.NormalizeString(ref storageService);
        TeamFoundationTracingService.NormalizeString(ref versionNumber);
        TeamFoundationTracingService.NormalizeString(ref operationType);
        TeamFoundationTracingService.NormalizeString(ref requestStatus);
        TeamFoundationTracingService.NormalizeString(ref httpStatusCode);
        TeamFoundationTracingService.NormalizeString(ref endtoEndLatencyInMs);
        TeamFoundationTracingService.NormalizeString(ref serverLatencyInMs);
        TeamFoundationTracingService.NormalizeString(ref authenticationType);
        TeamFoundationTracingService.NormalizeString(ref requesterAccountName);
        TeamFoundationTracingService.NormalizeString(ref ownerAccountName);
        TeamFoundationTracingService.NormalizeString(ref requestUrl);
        TeamFoundationTracingService.NormalizeString(ref requestedObjectKey);
        TeamFoundationTracingService.NormalizeString(ref requesterIpAddress);
        TeamFoundationTracingService.NormalizeString(ref requestVersionHeader);
        TeamFoundationTracingService.NormalizeString(ref requestMd5);
        TeamFoundationTracingService.NormalizeString(ref serverMd5);
        TeamFoundationTracingService.NormalizeString(ref etagIdentifier);
        TeamFoundationTracingService.NormalizeString(ref conditionsUsed);
        TeamFoundationTracingService.NormalizeString(ref userAgentHeader);
        TeamFoundationTracingService.NormalizeString(ref referrerHeader);
        TeamFoundationTracingService.NormalizeString(ref clientRequestId);
        TeamFoundationTracingService.NormalizeDateTime(ref lastModifiedTime);
        TeamFoundationTracingService.NormalizeDateTime(ref requestStartTime);
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(36, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateStorageAnalyticsLogs(ref eventDescriptor, executionId, storageAccount, storageService, versionNumber, requestStartTime, operationType, requestStatus, httpStatusCode, endtoEndLatencyInMs, serverLatencyInMs, authenticationType, requesterAccountName, ownerAccountName, requestUrl, requestedObjectKey, requestIdHeader, operationCount, requesterIpAddress, requestVersionHeader, requestHeaderSize, requestPacketSize, responseHeaderSize, responsePacketSize, requestContentLength, requestMd5, serverMd5, etagIdentifier, lastModifiedTime, conditionsUsed, userAgentHeader, referrerHeader, clientRequestId);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceResourceUtilization(
        DateTime startTime,
        Guid hostId,
        Guid vsid,
        Guid cuid,
        Guid activityId,
        int channel,
        string dataFeed)
      {
        TeamFoundationTracingService.NormalizeDateTime(ref startTime);
        TeamFoundationTracingService.NormalizeString(ref dataFeed);
        try
        {
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(37, (byte) 2, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
          return this.m_provider.TemplateResourceUtilization3(ref eventDescriptor, startTime, hostId, vsid, activityId, dataFeed, cuid, channel);
        }
        catch (Exception ex)
        {
          return false;
        }
      }

      public bool TraceHostingServiceCertInformation(
        Guid executionId,
        string source,
        string thumbprint,
        string serialNumber,
        string subjectName,
        string issuerName,
        string signatureAlgorithm,
        DateTime created,
        DateTime expiry)
      {
        TeamFoundationTracingService.NormalizeString(ref source);
        TeamFoundationTracingService.NormalizeString(ref thumbprint);
        TeamFoundationTracingService.NormalizeString(ref serialNumber);
        TeamFoundationTracingService.NormalizeString(ref subjectName);
        TeamFoundationTracingService.NormalizeString(ref issuerName);
        TeamFoundationTracingService.NormalizeString(ref signatureAlgorithm);
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(39, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateHostingServiceCertficates(ref eventDescriptor, executionId, source, thumbprint, serialNumber, subjectName, issuerName, signatureAlgorithm, created, expiry);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceSqlServerAlwaysOnHealthStatsInformation(
        Guid executionId,
        string listenerName,
        Guid groupId,
        string groupName,
        Guid replicaId,
        string replicaServerName,
        Guid replicaDatabaseId,
        string databaseName,
        string connectedStateDesc,
        string availabilityModeDesc,
        string synchronizationStateDesc,
        string replicaRoleDesc,
        int isLocal,
        int isJoined,
        int isSuspended,
        string suspendReasonDesc,
        int isFailoverReady,
        int estimatedDataLossIfFailoverNotReadyInSec,
        double estimatedRecoveryTime,
        long fileStreamSendRate,
        long logSendQueueSize,
        long logSendRate,
        long redoQueueSize,
        long redoRate,
        double synchronizationPerformance,
        string lastCommitLsn,
        DateTime lastCommitTime,
        string lastHardenedLsn,
        DateTime lastHardenedTime,
        string lastReceivedLsn,
        DateTime lastReceivedTime,
        string lastSentLsn,
        DateTime lastSentTime,
        string lastRedoneLsn,
        DateTime lastRedoneTime,
        string endOfLogLsn,
        string recoveryLsn,
        string truncationLsn,
        DateTime collectionTime)
      {
        TeamFoundationTracingService.NormalizeString(ref groupName);
        TeamFoundationTracingService.NormalizeString(ref replicaServerName);
        TeamFoundationTracingService.NormalizeString(ref databaseName);
        TeamFoundationTracingService.NormalizeString(ref connectedStateDesc);
        TeamFoundationTracingService.NormalizeString(ref availabilityModeDesc);
        TeamFoundationTracingService.NormalizeString(ref synchronizationStateDesc);
        TeamFoundationTracingService.NormalizeString(ref replicaRoleDesc);
        TeamFoundationTracingService.NormalizeString(ref suspendReasonDesc);
        TeamFoundationTracingService.NormalizeString(ref lastCommitLsn);
        TeamFoundationTracingService.NormalizeString(ref lastHardenedLsn);
        TeamFoundationTracingService.NormalizeString(ref lastReceivedLsn);
        TeamFoundationTracingService.NormalizeString(ref lastSentLsn);
        TeamFoundationTracingService.NormalizeString(ref lastRedoneLsn);
        TeamFoundationTracingService.NormalizeString(ref endOfLogLsn);
        TeamFoundationTracingService.NormalizeString(ref recoveryLsn);
        TeamFoundationTracingService.NormalizeString(ref truncationLsn);
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(40, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateSqlServerAlwaysOnHealthStats(ref eventDescriptor, executionId, listenerName, groupId, groupName, replicaId, replicaServerName, replicaDatabaseId, databaseName, connectedStateDesc, availabilityModeDesc, synchronizationStateDesc, replicaRoleDesc, isLocal, isJoined, isSuspended, suspendReasonDesc, isFailoverReady, estimatedDataLossIfFailoverNotReadyInSec, estimatedRecoveryTime, fileStreamSendRate, logSendQueueSize, logSendRate, redoQueueSize, redoRate, synchronizationPerformance, lastCommitLsn, lastCommitTime, lastHardenedLsn, lastHardenedTime, lastReceivedLsn, lastReceivedTime, lastSentLsn, lastSentTime, lastRedoneLsn, lastRedoneTime, endOfLogLsn, recoveryLsn, truncationLsn, collectionTime);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceXEventData(
        DateTime eventTime,
        int sequenceNumber,
        Guid activityId,
        Guid uniqueIdentifier,
        Guid hostId,
        Guid vsid,
        ContextType type,
        string objectName,
        string actions,
        string fields,
        string serverName,
        string databaseName,
        string xeventTypeName)
      {
        TeamFoundationTracingService.NormalizeString(ref objectName);
        TeamFoundationTracingService.NormalizeString(ref actions);
        TeamFoundationTracingService.NormalizeString(ref fields);
        TeamFoundationTracingService.NormalizeString(ref xeventTypeName);
        if (!this.m_provider.IsEnabled((byte) 4, -9223372036854775793L))
          return true;
        try
        {
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(41, (byte) 5, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
          return this.m_provider.TemplateXEventData6(ref eventDescriptor, eventTime, sequenceNumber, activityId, uniqueIdentifier, hostId, vsid, Convert.ToByte((object) type), objectName, actions, fields, serverName, databaseName, xeventTypeName);
        }
        catch (Exception ex)
        {
          return false;
        }
      }

      public bool TraceXEventDataRPCCompleted(
        DateTime eventTime,
        int sequenceNumber,
        Guid activityId,
        Guid uniqueIdentifier,
        Guid hostId,
        Guid vsid,
        ContextType type,
        bool isGoverned,
        string objectName,
        ulong cpuTime,
        ulong duration,
        ulong physicalReads,
        ulong logicalReads,
        ulong writes,
        string result,
        ulong rowCount,
        string connectionResetOption,
        string statement,
        double TSTUs,
        string serverName,
        string databaseName,
        bool isReadScaleOut)
      {
        TeamFoundationTracingService.NormalizeString(ref objectName);
        TeamFoundationTracingService.NormalizeString(ref result);
        TeamFoundationTracingService.NormalizeString(ref connectionResetOption);
        TeamFoundationTracingService.NormalizeString(ref statement);
        if (!this.m_provider.IsEnabled((byte) 4, -9223372036854775793L))
          return true;
        try
        {
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(49, (byte) 4, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
          return this.m_provider.TemplateXEventDataRPCCompleted5(ref eventDescriptor, eventTime, sequenceNumber, activityId, uniqueIdentifier, hostId, vsid, Convert.ToByte((object) type), objectName, cpuTime, duration, physicalReads, logicalReads, writes, result, rowCount, connectionResetOption, statement, TSTUs, serverName, databaseName, isGoverned, isReadScaleOut);
        }
        catch (Exception ex)
        {
          return false;
        }
      }

      public bool TraceCloudServiceRoleDetails(
        Guid executionId,
        Guid azureSubscriptionId,
        Guid azureSubscriptionAadTenantId,
        string roleType,
        int roleCountMin,
        int roleCount,
        int roleCountMax,
        string roleSize,
        int roleCores,
        long roleMemoryMB,
        string hostedServiceDnsName,
        string buildNumber,
        string osImageVersion,
        string osVersion,
        string appliedHotfixJson,
        bool encryptionAtHost,
        string securityType,
        bool secureBootEnabled,
        bool vTpmEnabled,
        string osDiskStorageAccountType,
        int osDiskSizeGB,
        string deploymentRing,
        int weekdayPeakRoleCountMin,
        int weekdayPeakRoleCountMax,
        string weekdayPeakStartTime,
        string weekdayPeakEndTime,
        int weekendPeakRoleCountMin,
        int weekendPeakRoleCountMax,
        string weekendPeakStartTime,
        string weekendPeakEndTime,
        string zones)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(51, (byte) 9, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateCloudServiceRoleDetails10(ref eventDescriptor, executionId, azureSubscriptionId, azureSubscriptionAadTenantId, roleType, roleCountMin, roleCount, roleCountMax, roleSize, roleCores, roleMemoryMB, hostedServiceDnsName, buildNumber, osImageVersion, osVersion, appliedHotfixJson, encryptionAtHost, securityType, secureBootEnabled, vTpmEnabled, osDiskStorageAccountType, osDiskSizeGB, deploymentRing, weekdayPeakRoleCountMin, weekdayPeakRoleCountMax, weekdayPeakStartTime, weekdayPeakEndTime, weekendPeakRoleCountMin, weekendPeakRoleCountMax, weekendPeakStartTime, weekendPeakEndTime, zones);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceServicePrincipalIsMember(
        Guid servicePrincipalId,
        string groupSid,
        byte hostType,
        string stackTrace,
        int executionCount)
      {
        TeamFoundationTracingService.NormalizeString(ref groupSid);
        TeamFoundationTracingService.NormalizeString(ref stackTrace);
        try
        {
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(60, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
          return this.m_provider.TemplateServicePrincipalIsMember(ref eventDescriptor, servicePrincipalId, groupSid, hostType, stackTrace, executionCount);
        }
        catch (Exception ex)
        {
          return false;
        }
      }

      public bool Trace(
        TraceLevel traceLevel,
        Guid traceId,
        int tracepoint,
        Guid serviceHost,
        long contextId,
        string processName,
        string username,
        string service,
        string method,
        string area,
        string layer,
        string userAgent,
        string uri,
        string path,
        string userDefined,
        string exceptionType,
        Guid vsid,
        Guid cuid,
        Guid tenantId,
        Guid providerId,
        string message,
        Guid uniqueIdentifier,
        Guid E2EId,
        string orchestrationId)
      {
        byte etw = TeamFoundationTracingService.TraceProvider.ConvertLevelToETW(traceLevel);
        System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(0, (byte) 7, (byte) 16, etw, (byte) 10, 1, -9223372036854775807L);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(etw, -9223372036854775793L))
          {
            try
            {
              if (message.Length > 8192)
                message = message.Substring(0, 8192);
              return this.m_provider.TemplateInfo8(ref eventDescriptor, traceId, tracepoint, serviceHost, contextId, processName, string.Empty, service, method, area, layer, userAgent, uri, path, userDefined, exceptionType, message, vsid, uniqueIdentifier, E2EId, cuid, tenantId, providerId, orchestrationId, TeamFoundationTracingService.s_webSiteId);
            }
            catch (FormatException ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceLowPriority(
        TraceLevel traceLevel,
        int tracepoint,
        Guid serviceHost,
        long contextId,
        string processName,
        string username,
        string service,
        string method,
        string area,
        string layer,
        string userAgent,
        string uri,
        string path,
        string userDefined,
        string exceptionType,
        Guid vsid,
        Guid cuid,
        Guid tenantId,
        Guid providerId,
        string message,
        Guid uniqueIdentifier,
        Guid E2EId,
        string orchestrationId)
      {
        byte etw = TeamFoundationTracingService.TraceProvider.ConvertLevelToETW(traceLevel);
        System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(75, (byte) 0, (byte) 16, etw, (byte) 10, 1, -9223372036854775807L);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(etw, -9223372036854775793L))
          {
            try
            {
              if (message.Length > 8192)
                message = message.Substring(0, 8192);
              return this.m_provider.TemplateInfo8(ref eventDescriptor, Guid.Empty, tracepoint, serviceHost, contextId, processName, string.Empty, service, method, area, layer, userAgent, uri, path, userDefined, exceptionType, message, vsid, uniqueIdentifier, E2EId, cuid, tenantId, providerId, orchestrationId, TeamFoundationTracingService.s_webSiteId);
            }
            catch (FormatException ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      public bool TraceEuii(
        string properties,
        Guid uniqueIdentifier,
        string anonymousIdentifier,
        Guid hostId,
        Guid parentHostId,
        byte hostType,
        Guid vsid,
        string area,
        string feature,
        string userAgent,
        Guid cuid,
        string method,
        string uri,
        string component,
        string message,
        string exceptionType,
        Guid E2EID,
        Guid tenantId,
        Guid providerid,
        TraceLevel level,
        DateTime startTime)
      {
        byte etw = TeamFoundationTracingService.TraceProvider.ConvertLevelToETW(level);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(etw, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(57, (byte) 0, (byte) 16, etw, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateEuiiTrace(ref eventDescriptor, properties, uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, hostType, vsid, area, feature, userAgent, cuid, method, uri, component, message, exceptionType, E2EID, tenantId, providerid, startTime);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceKpi(string metrics)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(8, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateKpi(ref eventDescriptor, 1, metrics);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceCustomerIntelligence(
        string properties,
        Guid uniqueIdentifier,
        string anonymousIdentifier,
        Guid hostId,
        Guid parentHostId,
        byte hostType,
        Guid vsid,
        Guid cuid,
        string area,
        string feature,
        string userAgent,
        string dataspaceType,
        string dataspaceId,
        string dataspaceVisibility,
        byte supportsPublicAccess)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(7, (byte) 4, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateCustomerIntelligence5(ref eventDescriptor, 1, properties, uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, hostType, vsid, area, feature, userAgent, cuid, dataspaceType, dataspaceId, dataspaceVisibility, supportsPublicAccess);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceClientTrace(
        string properties,
        Guid uniqueIdentifier,
        string anonymousIdentifier,
        Guid hostId,
        Guid parentHostId,
        byte hostType,
        Guid vsid,
        string area,
        string feature,
        string userAgent,
        Guid cuid,
        string method,
        string component,
        string message,
        string exceptionType,
        Guid E2EID,
        Guid tenantId,
        Guid providerid,
        Level level,
        DateTime startTime)
      {
        byte level1 = (byte) (level + 1);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level1, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(55, (byte) 1, (byte) 16, level1, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateClientTrace2(ref eventDescriptor, properties, uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, hostType, vsid, area, feature, userAgent, cuid, method, component, message, exceptionType, E2EID, tenantId, providerid, startTime);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceHostHistory(
        Guid hostId,
        DateTime modifiedDate,
        short actionType,
        Guid parentHostId,
        string serverName,
        string databaseName,
        int databaseId,
        int storageAccountId,
        string name,
        short status,
        string statusReason,
        short hostType,
        DateTime lastUserAccess,
        int subStatus)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(10, (byte) 2, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateHostHistory3(ref eventDescriptor, hostId, modifiedDate, actionType, parentHostId, serverName, databaseName, string.Empty, status, statusReason, (short) 0, hostType, lastUserAccess, name, databaseId, storageAccountId, subStatus);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceHostPreferredRegionUpdate(
        Guid hostId,
        string preferredRegion,
        string regionUpdateType)
      {
        byte level = 4;
        int id = 74;
        long keywords = -9223372036854775807;
        byte version = 0;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, version, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateHostPreferredRegionUpdate(ref eventDescriptor, hostId, preferredRegion, regionUpdateType);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceHostPreferredGeographyUpdate(
        Guid hostId,
        string preferredGeography,
        string geographyUpdateType)
      {
        byte level = 4;
        int id = 87;
        long keywords = -9223372036854775807;
        byte version = 0;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, version, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateHostPreferredGeographyUpdate(ref eventDescriptor, hostId, preferredGeography, geographyUpdateType);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceServiceHostExtended(
        Guid hostId,
        byte hostType,
        Guid parentHostId,
        string hostName,
        string serverName,
        string databaseName,
        short status,
        string statusReason,
        DateTime lastUserAccess,
        bool isDeleted)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(level, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(68, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateServiceHostExtended(ref eventDescriptor, hostId, hostType, parentHostId, hostName, serverName, databaseName, status, statusReason, lastUserAccess, isDeleted);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceServicingJobDetail(
        TraceLevel traceLevel,
        Guid hostId,
        Guid jobId,
        string operationClass,
        string operations,
        int jobStatus,
        string jobStatusDesc,
        int result,
        string resultDesc,
        DateTime queueTime,
        DateTime startTime,
        DateTime endTime,
        double durationInSeconds,
        short completedStepCount,
        short totalStepCount)
      {
        byte etw = TeamFoundationTracingService.TraceProvider.ConvertLevelToETW(traceLevel);
        System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(14, (byte) 0, (byte) 16, etw, (byte) 10, 1, -9223372036854775807L);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(etw, -9223372036854775793L))
          {
            try
            {
              return this.m_provider.TemplateServicingJobDetail(ref eventDescriptor, hostId, jobId, operationClass, operations, jobStatus, jobStatusDesc, result, resultDesc, queueTime, startTime, endTime, durationInSeconds, completedStepCount, totalStepCount);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal bool TraceServicingStepDetail(
        TraceLevel traceLevel,
        DateTime detailTime,
        string message,
        string operationName,
        string groupName,
        string stepName,
        string entryKind,
        Guid jobId,
        DateTime queueTime)
      {
        byte etw = TeamFoundationTracingService.TraceProvider.ConvertLevelToETW(traceLevel);
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled(etw, -9223372036854775793L))
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(15, (byte) 1, (byte) 16, etw, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateServicingStepDetail2(ref eventDescriptor, detailTime, message, operationName, groupName, stepName, entryKind, jobId, queueTime);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return true;
      }

      internal void TraceKpiMetric(
        DateTime eventTime,
        Guid hostId,
        string area,
        string scope,
        string displayName,
        string description,
        string kpiMetricName,
        double metricValue)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 16;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 1, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateKpiMetric2(ref eventDescriptor, eventTime, area, scope, kpiMetricName, metricValue, hostId, displayName, description);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceEventMetric(
        DateTime eventTime,
        int databaseId,
        string deploymentId,
        Guid hostId,
        string machineName,
        string roleInstanceId,
        string eventSource,
        string scope,
        string eventType,
        int eventId,
        string metricName,
        double metricValue)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 17;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 1, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateEventMetric2(ref eventDescriptor, eventTime, eventSource, metricName, metricValue, databaseId, deploymentId, hostId, machineName, roleInstanceId, scope, eventType, eventId);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceCommerceDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 19;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateCommerce(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceUserDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 20;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateUser(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceEuiiUserDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 62;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateUser(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceAccountDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 22;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateAccount(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceSubscriptionDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 23;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateSubscription(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceLicensingDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 25;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateLicensing(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceIdentitySessionTokenDataFeed(
        string operation,
        string tokenType,
        string error,
        Guid clientId,
        Guid accessId,
        Guid authorizationId,
        Guid hostAuthorizationId,
        Guid userId,
        DateTime validFrom,
        DateTime validTo,
        string displayName,
        string scope,
        string targetAccounts,
        bool isValid,
        bool isPublic,
        string publicData,
        string source)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 26;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateIdentity_SessionTokens(ref eventDescriptor, operation, tokenType, error, clientId, accessId, authorizationId, hostAuthorizationId, userId, validFrom, validTo, displayName, scope, targetAccounts, isValid, isPublic, publicData, source);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceIdentityReadDataFeed(
        string className,
        string flavor,
        string identifier,
        string queryMembership,
        string propertyNameFilters,
        string options,
        string callStack)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 29;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateIdentity_Reads(ref eventDescriptor, className, flavor, identifier, queryMembership, propertyNameFilters, options, callStack);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceIdentityTokenDataFeed(
        string className,
        string header,
        string claims,
        string nonce)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 30;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateIdentity_Token(ref eventDescriptor, className, header, claims, nonce);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceIdentityCacheChangesFeed(
        string cacheType,
        string eventType,
        string searchFilter,
        string domainId,
        string eventValue,
        string queryMembership,
        string cacheReadResult,
        string callStack)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 31;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateIdentity_Cache_Changes(ref eventDescriptor, cacheType, eventType, searchFilter, domainId, eventValue, queryMembership, cacheReadResult, callStack);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceIdentitySqlChangesFeed(
        string eventType,
        string domainId,
        string eventValue,
        string queryMembership,
        string callStack)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 32;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateIdentity_Sql_Changes(ref eventDescriptor, eventType, domainId, eventValue, queryMembership, callStack);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceEventNotification(
        string entityTypeName,
        DateTime createdDate,
        string eventTaskName,
        Guid hostId,
        string eventType,
        string identifier,
        string eventData)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 27;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateNotification(ref eventDescriptor, createdDate, eventTaskName, hostId, eventType, identifier, eventData);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceOrganizationDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 33;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateOrganization(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceDirectoryMemberDataFeed(string entityTypeName, string dataFeed)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 34;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateDirectory_Member(ref eventDescriptor, entityTypeName, dataFeed);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceMemoryClerks(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        string clerkName,
        long size,
        bool isReadonly)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 42;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 2, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateMemoryClerks3(ref eventDescriptor, executionId, serverName, databaseName, clerkName, size, isReadonly, listener);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceResourceSemaphores(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        int resourceSemaphoreId,
        long targetMemoryKB,
        long maxTargetMemoryKB,
        long totalMemoryKB,
        long availableMemoryKB,
        long grantedMemoryKB,
        long usedMemoryKB,
        int granteeCount,
        int waiterCount,
        long timeoutErrorCount,
        long forcedGrantCount,
        int poolId,
        bool isReadonly)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 43;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 2, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateResourceSemaphores3(ref eventDescriptor, executionId, serverName, databaseName, resourceSemaphoreId, targetMemoryKB, maxTargetMemoryKB, totalMemoryKB, availableMemoryKB, grantedMemoryKB, usedMemoryKB, granteeCount, waiterCount, timeoutErrorCount, forcedGrantCount, poolId, isReadonly, listener);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceQueryOptimizerMemoryGateways(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        int poolId,
        int maxCount,
        int activeCount,
        int waiterCount,
        long thresholdFactor,
        long threshold,
        bool isActive,
        bool isReadonly)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 44;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 2, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateQueryOptimizerMemoryGateways3(ref eventDescriptor, executionId, serverName, databaseName, poolId, maxCount, activeCount, waiterCount, thresholdFactor, threshold, isActive, isReadonly, listener);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceSqlPerformanceCounter(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        string counterName,
        long counterValue,
        bool isReadonly)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          int id = 45;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 2, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateSQLPerformanceCounters3(ref eventDescriptor, executionId, serverName, databaseName, counterName, counterValue, isReadonly, listener);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceHttpOutgoingRequest(
        DateTime startTime,
        int timeTaken,
        string httpClientName,
        string httpMethod,
        string urlHost,
        string urlPath,
        int responseCode,
        string errorMessage,
        Guid e2eId,
        string afdRefInfo,
        string requestPriority,
        Guid calledActivityId,
        string requestPhase,
        string orchestrationId,
        int tokenRetries,
        int handlerStartTime,
        int bufferedRequestTime,
        int requestSendTime,
        int responseContentTime,
        int getTokenTime,
        int trailingTime)
      {
        byte level = 4;
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(46, (byte) 5, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateHttpOutgoingRequests6(ref eventDescriptor, startTime, timeTaken, httpClientName, httpMethod, urlHost, urlPath, responseCode, errorMessage, e2eId, afdRefInfo, requestPriority, calledActivityId, requestPhase, orchestrationId, tokenRetries, handlerStartTime, bufferedRequestTime, requestSendTime, responseContentTime, getTokenTime, trailingTime);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceGeoReplicationLinkStatus(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        Guid linkGuid,
        string partnerServer,
        string partnerDatabase,
        DateTimeOffset lastReplication,
        int replicationLagSec,
        byte replicationState,
        string replicationStateDescription,
        byte role,
        string roleDescription,
        byte secondaryAllowConnections,
        string secondaryAllowConnectionsDescription,
        DateTimeOffset lastCommit)
      {
        byte level = 4;
        if (this.m_provider == null || !this.m_provider.IsEnabled())
          return;
        TeamFoundationTracingService.NormalizeString(ref serverName);
        TeamFoundationTracingService.NormalizeString(ref listener);
        TeamFoundationTracingService.NormalizeString(ref databaseName);
        TeamFoundationTracingService.NormalizeString(ref partnerDatabase);
        TeamFoundationTracingService.NormalizeString(ref partnerServer);
        TeamFoundationTracingService.NormalizeString(ref roleDescription);
        TeamFoundationTracingService.NormalizeString(ref secondaryAllowConnectionsDescription);
        try
        {
          int id = 48;
          long keywords = -9223372036854775807;
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 1, (byte) 16, level, (byte) 10, 1, keywords);
          this.m_provider.TemplateGeoReplicationLinkStatus2(ref eventDescriptor, executionId, serverName, databaseName, linkGuid, partnerServer, partnerDatabase, lastReplication.UtcDateTime, replicationLagSec, (int) replicationState, replicationStateDescription, role, roleDescription, secondaryAllowConnections, secondaryAllowConnectionsDescription, lastCommit.UtcDateTime, listener);
        }
        catch (Exception ex)
        {
        }
      }

      public void Dispose()
      {
        if (this.m_provider == null)
          return;
        this.m_provider.Dispose();
        this.m_provider = (EventProviderVersionTwo) null;
      }

      internal void TracePackagingTraces(
        Guid activityId,
        string collectionHostName,
        Guid hostId,
        string protocol,
        string command,
        string feedId,
        string feedName,
        string viewId,
        string viewName,
        string packageName,
        string packageVersion,
        string packageStorageId,
        int responseCode,
        bool isSlow,
        long timeToFirstPageInMs,
        long executionTimeInMs,
        long queueTimeInMs,
        bool isFailed,
        string exceptionType,
        string exceptionMessage,
        string identityName,
        string userAgent,
        string clientSessionId,
        string refererHeader,
        string sourceIp,
        string hostIp,
        string buildNumber,
        string commitId,
        string dataCurrentVersion,
        string dataDestinationVersion,
        string dataMigrationState,
        string featureFlagsOn,
        string featureFlagsOff,
        string stackTrace,
        string timingsTrace,
        string uri,
        string httpMethod,
        Guid relatedActivityId,
        Guid e2EID,
        string properties,
        Guid projectId,
        string projectVisibility,
        string orchestrationId)
      {
        TeamFoundationTracingService.NormalizeString(ref collectionHostName);
        TeamFoundationTracingService.NormalizeString(ref protocol);
        TeamFoundationTracingService.NormalizeString(ref command);
        TeamFoundationTracingService.NormalizeString(ref feedId);
        TeamFoundationTracingService.NormalizeString(ref feedName);
        TeamFoundationTracingService.NormalizeString(ref viewId);
        TeamFoundationTracingService.NormalizeString(ref viewName);
        TeamFoundationTracingService.NormalizeString(ref packageName);
        TeamFoundationTracingService.NormalizeString(ref packageVersion);
        TeamFoundationTracingService.NormalizeString(ref packageStorageId);
        TeamFoundationTracingService.NormalizeString(ref exceptionType);
        TeamFoundationTracingService.NormalizeString(ref exceptionMessage);
        TeamFoundationTracingService.NormalizeString(ref identityName);
        TeamFoundationTracingService.NormalizeString(ref userAgent);
        TeamFoundationTracingService.NormalizeString(ref clientSessionId);
        TeamFoundationTracingService.NormalizeString(ref refererHeader);
        TeamFoundationTracingService.NormalizeString(ref sourceIp);
        TeamFoundationTracingService.NormalizeString(ref hostIp);
        TeamFoundationTracingService.NormalizeString(ref buildNumber);
        TeamFoundationTracingService.NormalizeString(ref commitId);
        TeamFoundationTracingService.NormalizeString(ref dataCurrentVersion);
        TeamFoundationTracingService.NormalizeString(ref dataDestinationVersion);
        TeamFoundationTracingService.NormalizeString(ref dataMigrationState);
        TeamFoundationTracingService.NormalizeString(ref featureFlagsOn);
        TeamFoundationTracingService.NormalizeString(ref featureFlagsOff);
        TeamFoundationTracingService.NormalizeString(ref stackTrace);
        TeamFoundationTracingService.NormalizeString(ref timingsTrace);
        TeamFoundationTracingService.NormalizeString(ref uri);
        TeamFoundationTracingService.NormalizeString(ref httpMethod);
        TeamFoundationTracingService.NormalizeString(ref properties);
        TeamFoundationTracingService.NormalizeString(ref projectVisibility);
        TeamFoundationTracingService.NormalizeString(ref orchestrationId);
        try
        {
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(47, (byte) 3, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
          EventProviderVersionTwo provider = this.m_provider;
          ref System.Diagnostics.Eventing.EventDescriptor local = ref eventDescriptor;
          Guid ActivityId = activityId;
          string CollectionHostName = collectionHostName;
          Guid HostId = hostId;
          string str1 = protocol;
          string str2 = command;
          string str3 = feedId;
          string str4 = feedName;
          string str5 = viewId;
          string str6 = viewName;
          string str7 = packageName;
          string str8 = packageVersion;
          string str9 = packageStorageId;
          int num1 = responseCode;
          bool flag1 = isSlow;
          long num2 = timeToFirstPageInMs;
          long num3 = executionTimeInMs;
          long num4 = queueTimeInMs;
          bool flag2 = isFailed;
          string str10 = exceptionType;
          string str11 = exceptionMessage;
          string str12 = identityName;
          string str13 = userAgent;
          string str14 = clientSessionId;
          string str15 = refererHeader;
          string str16 = sourceIp;
          string str17 = hostIp;
          string str18 = buildNumber;
          string str19 = commitId;
          string str20 = dataCurrentVersion;
          string str21 = dataDestinationVersion;
          string str22 = dataMigrationState;
          string str23 = featureFlagsOn;
          string str24 = featureFlagsOff;
          string str25 = stackTrace;
          string str26 = timingsTrace;
          string str27 = uri;
          string str28 = httpMethod;
          Guid guid1 = relatedActivityId;
          Guid guid2 = e2EID;
          string str29 = properties;
          Guid ProjectId = projectId;
          string Protocol = str1;
          string Command = str2;
          string FeedId = str3;
          string FeedName = str4;
          string ViewId = str5;
          string ViewName = str6;
          string PackageName = str7;
          string PackageVersion = str8;
          int ResponseCode = num1;
          int num5 = flag1 ? 1 : 0;
          long TimeToFirstPageInMs = num2;
          long ExecutionTimeInMs = num3;
          long QueueTimeInMs = num4;
          int num6 = flag2 ? 1 : 0;
          string ExceptionType = str10;
          string ExceptionMessage = str11;
          string IdentityName = str12;
          string UserAgent = str13;
          string ClientSessionId = str14;
          string RefererHeader = str15;
          string SourceIp = str16;
          string HostIp = str17;
          string BuildNumber = str18;
          string CommitId = str19;
          string DataCurrentVersion = str20;
          string DataDestinationVersion = str21;
          string DataMigrationState = str22;
          string FeatureFlagsOn = str23;
          string FeatureFlagsOff = str24;
          string StackTrace = str25;
          string TimingsTrace = str26;
          string Uri = str27;
          string HttpMethod = str28;
          Guid RelatedActivityId = guid1;
          Guid E2EID = guid2;
          string Properties = str29;
          string PackageStorageId = str9;
          string ProjectVisibility = projectVisibility;
          string OrchestrationId = orchestrationId;
          provider.TemplatePackagingTraces3(ref local, ActivityId, CollectionHostName, HostId, ProjectId, Protocol, Command, FeedId, FeedName, ViewId, ViewName, PackageName, PackageVersion, ResponseCode, num5 != 0, TimeToFirstPageInMs, ExecutionTimeInMs, QueueTimeInMs, num6 != 0, ExceptionType, ExceptionMessage, IdentityName, UserAgent, ClientSessionId, RefererHeader, SourceIp, HostIp, BuildNumber, CommitId, DataCurrentVersion, DataDestinationVersion, DataMigrationState, FeatureFlagsOn, FeatureFlagsOff, StackTrace, TimingsTrace, Uri, HttpMethod, RelatedActivityId, E2EID, Properties, PackageStorageId, ProjectVisibility, OrchestrationId);
        }
        catch (Exception ex)
        {
        }
      }

      internal bool TraceTuningRecommendation(
        long runId,
        string serverName,
        string listener,
        string databaseName,
        string name,
        string type,
        string reason,
        DateTime validSince,
        DateTime lastRefresh,
        string state,
        bool isExecutableAction,
        bool isRevertableAction,
        TuningAction executeAction,
        TuningAction revertAction,
        int score,
        string details)
      {
        if (this.m_provider == null || !this.m_provider.IsEnabled())
          return false;
        System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(53, (byte) 1, (byte) 0, (byte) 4, (byte) 10, 1, -9223372036854775807L);
        return this.m_provider.TemplateTuningRecommendation2(ref eventDescriptor, runId, serverName, databaseName, name, type, reason, validSince, lastRefresh, state, isExecutableAction, isRevertableAction, executeAction.StartTime, executeAction.DurationMilliseconds, executeAction.InitiatedBy ?? string.Empty, executeAction.InitiatedTime, revertAction.StartTime, revertAction.DurationMilliseconds, revertAction.InitiatedBy ?? string.Empty, revertAction.InitiatedTime, score, details, listener);
      }

      internal bool TraceSurveyEvents(
        Guid uniqueIdentifier,
        string anonymousIdentifier,
        Guid tenantId,
        Guid hostId,
        Guid parentHostId,
        byte hostType,
        Guid vsid,
        Guid cuid,
        string area,
        string feature,
        string userAgent,
        string properties,
        string dataspaceType,
        string dataspaceId,
        string dataspaceVisibility,
        byte supportsPublicAccess)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              int id = 52;
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 1, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateSurveyEvent2(ref eventDescriptor, uniqueIdentifier, anonymousIdentifier, tenantId, hostId, parentHostId, hostType, vsid, cuid, area, feature, userAgent, properties, dataspaceType, dataspaceId, dataspaceVisibility, supportsPublicAccess);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceDetectedEUIIEvent(
        DateTime eventTime,
        string source,
        int euiiType,
        string message)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              int id = 58;
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateDetectedEuiiEvent(ref eventDescriptor, eventTime, source, euiiType, message);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceOrchestrationLogEvent(
        string orchestrationId,
        DateTime startTime,
        DateTime endTime,
        long executionTimeThreshold,
        byte orchestrationStatus,
        string application,
        string feature,
        string command,
        string exceptionType,
        string exceptionMessage,
        bool isExceptionExpected,
        byte hostType,
        Guid hostId,
        Guid parentHostId,
        Guid vsid,
        Guid cuid,
        string anonymousIdentifier)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              int id = 63;
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 2, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateOrchestrationLog3(ref eventDescriptor, orchestrationId, startTime, endTime, executionTimeThreshold, orchestrationStatus, application, feature, command, exceptionType, exceptionMessage, isExceptionExpected, hostType, hostId, parentHostId, vsid, cuid, anonymousIdentifier);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceVirtualFileStats(
        Guid executionId,
        string serverName,
        string databaseName,
        bool isReadOnly,
        short databaseId,
        short fileId,
        long sampleMs,
        long numReads,
        long numBytesRead,
        long ioStallReadMs,
        long numWrites,
        long numBytesWritten,
        long ioStallWriteMs,
        long ioStall,
        long sizeOnDiskBytes,
        long ioStallQueuedReadMs,
        long ioStallQueueWriteMs)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(73, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateVirtualFileStats(ref eventDescriptor, executionId, serverName, databaseName, isReadOnly, databaseId, fileId, sampleMs, numReads, numBytesRead, ioStallReadMs, numWrites, numBytesWritten, ioStallWriteMs, ioStall, sizeOnDiskBytes, ioStallQueuedReadMs, ioStallQueueWriteMs);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceSQLSpinlock(
        Guid executionId,
        string serverName,
        string listener,
        string databaseName,
        string name,
        long collisions,
        long spins,
        float spinsPerCollision,
        long sleepTime,
        long backoffs,
        bool isReadOnly)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(65, (byte) 2, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateSQLSpinlocks3(ref eventDescriptor, executionId, serverName, databaseName, name, collisions, spins, spinsPerCollision, sleepTime, backoffs, isReadOnly, listener);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceServiceBusMetrics(
        Guid executionId,
        string serviceBusNamespace,
        string skuTier,
        DateTime startingIntervalUTC,
        double totalSuccessfulRequests,
        double totalServerErrors,
        double totalUserErrors,
        double totalThrottledRequests,
        double totalIncomingRequests,
        double totalIncomingMessages,
        double totalOutgoingMessages,
        double totalActiveConnections,
        double averageSizeInBytes,
        double averageMessages,
        double averageActiveMessages)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(64, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateServiceBusMetrics(ref eventDescriptor, executionId, serviceBusNamespace, skuTier, startingIntervalUTC, totalSuccessfulRequests, totalServerErrors, totalUserErrors, totalThrottledRequests, totalIncomingRequests, totalIncomingMessages, totalOutgoingMessages, totalActiveConnections, averageSizeInBytes, averageMessages, averageActiveMessages);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceRedisCacheMetrics(
        Guid executionId,
        string redisCacheInstance,
        string skuTier,
        DateTime startingIntervalUTC,
        double totalConnectedClients,
        double totalCommandsProcessed,
        double totalCacheHits,
        double totalCacheMisses,
        double totalUsedMemory,
        double totalUsedMemoryRss,
        double totalServerLoad,
        double totalProcessorTime,
        double totalOperationsPerSecond,
        double totalGetCommands,
        double totalSetCommands,
        double totalEvictedKeys,
        double totalTotalKeys,
        double totalExpiredKeys,
        double totalUsedMemoryPercentage,
        double totalCacheRead,
        double totalErrors)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(67, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateRedisCacheMetrics(ref eventDescriptor, executionId, redisCacheInstance, skuTier, startingIntervalUTC, totalConnectedClients, totalCommandsProcessed, totalCacheHits, totalCacheMisses, totalUsedMemory, totalUsedMemoryRss, totalServerLoad, totalProcessorTime, totalOperationsPerSecond, totalGetCommands, totalSetCommands, totalEvictedKeys, totalTotalKeys, totalExpiredKeys, totalUsedMemoryPercentage, totalCacheRead, totalErrors);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceFeatureFlagStatus(
        long runId,
        string name,
        string effectiveState,
        string explicitState,
        string hostId,
        string vsid)
      {
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(69, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateFeatureFlagStatusHostVSID(ref eventDescriptor, runId, name, effectiveState, explicitState, hostId ?? string.Empty, vsid ?? string.Empty);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceOrganizationTenant(
        Guid hostId,
        byte hostType,
        string hostName,
        Guid parentHostId,
        byte parentHostType,
        string parentHostName,
        Guid tenantId,
        DateTime tenantLastModified,
        string preferredRegion)
      {
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(76, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
              return this.m_provider.TemplateOrganizationTenant2(ref eventDescriptor, hostId, hostType, hostName, parentHostId, parentHostType, parentHostName, tenantId, tenantLastModified, preferredRegion);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceDatabaseDetails(
        Guid executionId,
        int databaseId,
        string serverName,
        string databaseName,
        long version,
        string serviceLevel,
        string poolName,
        int poolMaxDatabaseLimit,
        int tenants,
        int maxTenants,
        string status,
        string statusReason,
        DateTime statusChangedDate,
        string flags,
        string minServiceObjective,
        string maxServiceObjective,
        int retentionDays,
        string connectionString,
        DateTime createdOn,
        string serviceObjective,
        string backupStorageRedundancy,
        bool isZoneRedundant,
        string collation,
        string location,
        string defaultSecondaryLocation,
        string readScale,
        int highAvailabilityReplicaCount,
        double maxSizeInGB,
        double maxLogSizeInGB,
        string kind)
      {
        EventProviderVersionTwo provider = this.m_provider;
        if ((provider != null ? (!provider.IsEnabled() ? 1 : 0) : 1) != 0)
          return false;
        try
        {
          return this.m_provider.TemplateDatabaseDetails5(ref TeamFoundationTracingService.TraceProvider.s_databaseDetailsEventDescriptor, executionId, databaseId, serverName, databaseName, version, serviceLevel, poolName, poolMaxDatabaseLimit, tenants, maxTenants, status, statusReason, statusChangedDate, flags, minServiceObjective, maxServiceObjective, retentionDays, connectionString, createdOn, serviceObjective, backupStorageRedundancy, isZoneRedundant, collation, location, defaultSecondaryLocation, readScale, highAvailabilityReplicaCount, maxSizeInGB, maxLogSizeInGB, kind);
        }
        catch (Exception ex)
        {
          return false;
        }
      }

      internal bool TraceDatabaseConnectionInfo(
        Guid executionId,
        string serverName,
        string databaseName,
        bool isReadOnly,
        string hostName,
        string programName,
        int hostProcessId,
        int count,
        int inactiveCount,
        string sampleText)
      {
        EventProviderVersionTwo provider = this.m_provider;
        if ((provider != null ? (provider.IsEnabled() ? 1 : 0) : 0) != 0)
        {
          try
          {
            return this.m_provider.TemplateDatabaseConnectionInfo(ref TeamFoundationTracingService.TraceProvider.s_databaseConnectionInfoEventDescriptor, executionId, serverName, databaseName, isReadOnly, hostName, programName, hostProcessId, count, inactiveCount, sampleText);
          }
          catch (Exception ex)
          {
          }
        }
        return false;
      }

      internal bool TraceSqlRowLockInfo(
        Guid executionId,
        string serverName,
        string databaseName,
        bool isReadOnly,
        string schemaName,
        string tableName,
        string indexName,
        long hobtId,
        int objectId)
      {
        EventProviderVersionTwo provider = this.m_provider;
        if ((provider != null ? (provider.IsEnabled() ? 1 : 0) : 0) != 0)
        {
          try
          {
            return this.m_provider.TemplateSqlRowLockInfo2(ref TeamFoundationTracingService.TraceProvider.s_sqlRowLockInfoEventDescriptor, executionId, serverName, databaseName, isReadOnly, schemaName, tableName, indexName, hobtId, objectId);
          }
          catch (Exception ex)
          {
          }
        }
        return false;
      }

      internal bool TraceDatabasePrincipal(
        string serverName,
        string databaseName,
        int principalId,
        string principalName,
        string roleName,
        string permissions,
        string typeDesc,
        DateTime createTime,
        DateTime modifytime)
      {
        EventProviderVersionTwo provider = this.m_provider;
        if ((provider != null ? (provider.IsEnabled() ? 1 : 0) : 0) != 0)
        {
          try
          {
            System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(84, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
            return this.m_provider.TemplateDatabasePrincipals(ref eventDescriptor, serverName, databaseName, principalId, principalName, roleName, permissions, typeDesc, createTime, modifytime);
          }
          catch (Exception ex)
          {
          }
        }
        return false;
      }

      internal bool TraceXEventSession(
        string serverName,
        string databaseName,
        int sessionId,
        string sessionName,
        bool isEventFileTruncated,
        int buffersLogged,
        int buffersDropped,
        string eventFileName)
      {
        EventProviderVersionTwo provider1 = this.m_provider;
        if ((provider1 != null ? (provider1.IsEnabled() ? 1 : 0) : 0) != 0)
        {
          try
          {
            EventProviderVersionTwo provider2 = this.m_provider;
            ref System.Diagnostics.Eventing.EventDescriptor local = ref TeamFoundationTracingService.TraceProvider.s_xEventSessionsEventDescriptor;
            string str1 = serverName;
            string str2 = databaseName;
            int SessionId = sessionId;
            string ServerName = str1;
            string DatabaseName = str2;
            string SessionName = sessionName;
            int num = isEventFileTruncated ? 1 : 0;
            int BuffersLogged = buffersLogged;
            int BuffersDropped = buffersDropped;
            string EventFileName = eventFileName;
            return provider2.TemplateXEventSessions(ref local, SessionId, ServerName, DatabaseName, SessionName, num != 0, BuffersLogged, BuffersDropped, EventFileName);
          }
          catch (Exception ex)
          {
          }
        }
        return false;
      }

      internal bool TraceGitThrottlingSettings(int size, int tarpit, int workunitSize)
      {
        EventProviderVersionTwo provider = this.m_provider;
        if ((provider != null ? (provider.IsEnabled() ? 1 : 0) : 0) != 0)
        {
          try
          {
            System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(86, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
            return this.m_provider.TemplateGitThrottlingSettings(ref eventDescriptor, size, tarpit, workunitSize);
          }
          catch (Exception ex)
          {
          }
        }
        return false;
      }

      internal bool TraceOrchestrationActivityLogEvent(
        Guid hostId,
        string orchestrationId,
        string name,
        string version,
        DateTime queueTime,
        DateTime startTime,
        long executionTime,
        long cpuExecutionTime,
        string exceptionType,
        string exceptionMessage,
        Guid activityId,
        Guid e2eId,
        long cpuCycles,
        long allocatedBytes)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              int id = 79;
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(id, (byte) 1, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateOrchestrationActivityLog1(ref eventDescriptor, hostId, orchestrationId, name, version, queueTime, startTime, executionTime, cpuExecutionTime, exceptionType, exceptionMessage, activityId, e2eId, cpuCycles, allocatedBytes);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceDocDBStorageMetrics(
        Guid executionId,
        string accountName,
        string databaseCategory,
        string databaseId,
        string collectionId,
        long collectionSizeUsage,
        long collectionSizeQuota,
        string partitionKeyRangeId,
        long partitionKeyRangeDocumentCount,
        long partitionKeyRangeSize,
        string partitionKeyRangeDominantPartitionKeys)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(70, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateDocDBStorageMetrics(ref eventDescriptor, executionId, accountName, databaseCategory, databaseId, collectionId, collectionSizeUsage, collectionSizeQuota, partitionKeyRangeId, partitionKeyRangeDocumentCount, partitionKeyRangeSize, partitionKeyRangeDominantPartitionKeys);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceAzureSearchMetrics(
        Guid executionId,
        string azureSearchInstance,
        string skuTier,
        DateTime startingIntervalUTC,
        double searchLatency,
        double queriesPerSecond,
        double throttledSearchQueriesPercentage)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(71, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateAzureSearchMetrics(ref eventDescriptor, executionId, azureSearchInstance, skuTier, startingIntervalUTC, searchLatency, queriesPerSecond, throttledSearchQueriesPercentage);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal bool TraceDocDBRUMetrics(
        string dbAccountName,
        string databaseCategory,
        string databaseId,
        string collectionId,
        string documentType,
        long consumedRUs)
      {
        byte level = 4;
        if (this.m_provider != null)
        {
          if (this.m_provider.IsEnabled())
          {
            try
            {
              long keywords = -9223372036854775807;
              System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(72, (byte) 0, (byte) 16, level, (byte) 10, 1, keywords);
              return this.m_provider.TemplateDocDBRUMetrics(ref eventDescriptor, dbAccountName, databaseCategory, databaseId, collectionId, documentType, consumedRUs);
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
        return false;
      }

      internal void TraceDatabaseCounters(
        string serverName,
        string databaseName,
        Guid hostId,
        Guid projectId,
        string counterName,
        long counterValue,
        int leftOverPercent)
      {
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(80, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
          this.m_provider.TemplateDatabaseCounters(ref eventDescriptor, serverName, databaseName, hostId, projectId, counterName, counterValue, leftOverPercent);
        }
        catch (Exception ex)
        {
        }
      }

      internal void TraceDatabaseIdentityColumns(
        string serverName,
        string databaseName,
        string schemaName,
        string tableName,
        string identityColumnName,
        long identityColumnValue,
        string identityColumnDatatype,
        int leftOverPercent)
      {
        if (this.m_provider == null)
          return;
        if (!this.m_provider.IsEnabled())
          return;
        try
        {
          System.Diagnostics.Eventing.EventDescriptor eventDescriptor = new System.Diagnostics.Eventing.EventDescriptor(81, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
          this.m_provider.TemplateDatabaseIdentityColumns(ref eventDescriptor, serverName, databaseName, schemaName, tableName, identityColumnName, identityColumnValue, identityColumnDatatype, leftOverPercent);
        }
        catch (Exception ex)
        {
        }
      }
    }

    private static class EtwTraceLevel
    {
      public const byte Off = 1;
      public const byte Error = 2;
      public const byte Warning = 3;
      public const byte Info = 4;
      public const byte Verbose = 5;
    }
  }
}
