// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.Common.AOConstants
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Admin.Common
{
  public static class AOConstants
  {
    public const int MaxPort = 65535;
    public const int MinPort = 1;

    public static class Accounts
    {
      public const string NetworkServiceNonLocalized = "NT Authority\\NetworkService";
      public const string LocalServiceNonLocalized = "NT Authority\\LocalService";
      public const string SystemNonLocalized = "NT Authority\\System";
    }

    public static class LegacyUpgrade
    {
      public const string UpgradeInstanceRegistryHint = "UpgradeInstanceHint";
    }

    public static class Folders
    {
      public const string Queues = "Message Queue";
      public const string WebServices = "Web Services";
      public const string WebAccess = "Web Access";
    }

    public static class ConfigurationSettings
    {
      public const string PreviousFailedConfiguration = "PreviousFailedConfiguration";
      public const string Iis7InstallationFailed = "IIS7InstallationFailed";
    }

    public static class EnvironmentVariables
    {
      public const string TfsCheckCaseStaticResources = "TFS_CHECK_CASE_STATIC_RESOURCES";
      public const string ESJavaHome = "ES_JAVA_HOME";
    }

    public static class Databases
    {
      public const string ExecRole = "TFSEXECROLE";
      public const string IntegrationName = "TfsIntegration";
    }

    public static class WebSite
    {
      public const string QueueVdir = "queue";
      public const string WebVdir = "";
      public const string ATSiteName = "Azure DevOps Server";
      public const int DefaultATPort = 80;
      public const int DefaultProxyPort = 8081;
      public const string ProxySiteName = "Azure DevOps Server Proxy";
      public const string FirewallRuleNamePrefix = "Azure DevOps Server";
      public const string DefaultWebSiteName = "Default Web Site";
      public const string DefaultWebSiteBinding = "http:*:80:";
      public const string DefaultWebSitePath = "%SystemDrive%\\inetpub\\wwwroot";
      public const string DefaultApplicationPoolName = "DefaultAppPool";
    }

    public static class ApplicationPools
    {
      public const string AppTierName = "Azure DevOps Server Application Pool";
      public const string MessageQueueName = "Azure DevOps Server Message Queue Application Pool";
      public const string ProxyName = "Azure DevOps Server Proxy Application Pool";
    }

    public static class WorkerProcessGroups
    {
      public const string ApplicationTier = "TFS_APPTIER_SERVICE_WPG";
      public const string Proxy = "TFS_PROXY_SERVICE_WPG";
    }

    public static class Services
    {
      public const string DBBaseInstanceServiceName = "MSSQL";
      public const string DBDefaultServiceName = "MSSQLSERVER";
      public const string DBServiceLabel = "SQL Server Database Services";
      public const string IisAdminServiceName = "IISAdmin";
      public const string TfsJobAgent = "TFSJobAgent";
      public const string TfsSshService = "TeamFoundationSshService";
      public const string TfsGvfsService = "GVFS Cache Maintenance Service";
      public const string W3ServiceName = "w3svc";
      public const string W3ServiceLabel = "IIS";
    }

    public static class LogTokens
    {
      public const string Unknown = "UNK";
      public const string Configuration = "CFG";
      public const string TeamProjectCollection = "TPC";
      public const string Error = "ERR";
      public const string Deploy = "DPLY";
      public const string Upgrade = "UPG";
      public const string Account = "ACCT";
      public const string Settings = "SET";
      public const string Servicing = "SVC";
      public const string Diagnose = "DIAG";
      public const string FeatureFlags = "FLAGS";
      public const string ApplicationTier = "AT";
      public const string Proxy = "PRXY";
    }

    public static class ServicingData
    {
      public const string AnalyticsAssemblyName = "Microsoft.VisualStudio.Services.Analytics.Servicing.dll";
      public const string ASTableAssemblyName = "Microsoft.VisualStudio.Services.ASTable.Servicing.dll";
      public const string BlobStoreAssemblyName = "Microsoft.VisualStudio.Services.BlobStore.Servicing.dll";
      public const string CodeReviewAssemblyName = "Microsoft.VisualStudio.Services.CodeReview.Sdk.Servicing.dll";
      public const string DataAssemblyName = "Microsoft.TeamFoundation.Servicing.dll";
      public const string DeploymentAssemblyName = "Microsoft.Azure.Pipelines.Deployment.Servicing.dll";
      public const string DistributedTaskAssemblyName = "Microsoft.TeamFoundation.DistributedTask.Servicing.dll";
      public const string DistributedTaskSdkAssemblyName = "Microsoft.TeamFoundation.DistributedTask.Orchestration.Servicing.dll";
      public const string ExtensionManagementAssemblyName = "Microsoft.VisualStudio.Services.ExtensionManagement.Servicing.dll";
      public const string FavoritesAssemblyName = "Microsoft.VisualStudio.Services.Favorites.Servicing.dll";
      public const string SocialAssemblyName = "Microsoft.VisualStudio.Services.Social.Servicing.dll";
      public const string FeedAssemblyName = "Microsoft.VisualStudio.Services.Feed.Servicing.dll";
      public const string GalleryAssemblyName = "Microsoft.VisualStudio.Services.Gallery.Servicing.dll";
      public const string NameResolutionaAssemblyName = "Microsoft.VisualStudio.Services.NameResolution.Servicing.dll";
      public const string NotificationsAssemblyName = "Microsoft.VisualStudio.Services.Notifications.Servicing.dll";
      public const string PackagingAssemblyName = "Microsoft.VisualStudio.Services.Packaging.Servicing.dll";
      public const string ReleaseManagementAssemblyName = "Microsoft.VisualStudio.Services.ReleaseManagement.Servicing.dll";
      public const string SdkDataAssemblyName = "Microsoft.VisualStudio.Services.Sdk.Servicing.dll";
      public const string SearchManagementAssemblyName = "Microsoft.VisualStudio.Services.Search.Servicing.dll";
      public const string AlmSearchManagementAssemblyName = "Microsoft.VisualStudio.Services.AlmSearch.Servicing.dll";
      public const string ServiceHooksAssemblyName = "Microsoft.Visualstudio.Services.Servicehooks.Servicing.dll";
      public const string SignalRAssemblyName = "Microsoft.VisualStudio.Services.SignalR.Servicing.dll";
      public const string SpsDataAssemblyName = "Microsoft.VisualStudio.Services.Sps.Servicing.dll";
      public const string AnalyticsSdkAssemblyName = "Microsoft.VisualStudio.Services.Analytics.Sdk.Servicing.dll";
      public const string ServiceEndpointsAssemblyName = "Microsoft.Azure.DevOps.ServiceEndpoints.Servicing.dll";
      public const string PipelinePolicyAssemblyName = "Microsoft.Azure.Pipelines.Policy.Servicing.dll";
      public const string TagsAssemblyName = "Microsoft.Azure.Devops.Tags.Servicing.dll";
      public const string TcmAssemblyName = "Microsoft.VisualStudio.Services.Tcm.Servicing.dll";
      public static readonly string[] AllDataAssemblies = new string[28]
      {
        "Microsoft.VisualStudio.Services.Analytics.Servicing.dll",
        "Microsoft.VisualStudio.Services.ASTable.Servicing.dll",
        "Microsoft.VisualStudio.Services.BlobStore.Servicing.dll",
        "Microsoft.VisualStudio.Services.CodeReview.Sdk.Servicing.dll",
        "Microsoft.TeamFoundation.Servicing.dll",
        "Microsoft.TeamFoundation.DistributedTask.Servicing.dll",
        "Microsoft.TeamFoundation.DistributedTask.Orchestration.Servicing.dll",
        "Microsoft.VisualStudio.Services.ExtensionManagement.Servicing.dll",
        "Microsoft.VisualStudio.Services.Favorites.Servicing.dll",
        "Microsoft.VisualStudio.Services.Social.Servicing.dll",
        "Microsoft.VisualStudio.Services.Feed.Servicing.dll",
        "Microsoft.VisualStudio.Services.Gallery.Servicing.dll",
        "Microsoft.VisualStudio.Services.Notifications.Servicing.dll",
        "Microsoft.VisualStudio.Services.NameResolution.Servicing.dll",
        "Microsoft.VisualStudio.Services.Packaging.Servicing.dll",
        "Microsoft.VisualStudio.Services.ReleaseManagement.Servicing.dll",
        "Microsoft.VisualStudio.Services.Sdk.Servicing.dll",
        "Microsoft.VisualStudio.Services.Search.Servicing.dll",
        "Microsoft.VisualStudio.Services.AlmSearch.Servicing.dll",
        "Microsoft.Visualstudio.Services.Servicehooks.Servicing.dll",
        "Microsoft.VisualStudio.Services.SignalR.Servicing.dll",
        "Microsoft.VisualStudio.Services.Sps.Servicing.dll",
        "Microsoft.VisualStudio.Services.Analytics.Sdk.Servicing.dll",
        "Microsoft.Azure.DevOps.ServiceEndpoints.Servicing.dll",
        "Microsoft.Azure.Pipelines.Policy.Servicing.dll",
        "Microsoft.Azure.Pipelines.Deployment.Servicing.dll",
        "Microsoft.Azure.Devops.Tags.Servicing.dll",
        "Microsoft.VisualStudio.Services.Tcm.Servicing.dll"
      };
      public static readonly string[] UploadDataAssemblies = new string[26]
      {
        "Microsoft.VisualStudio.Services.Analytics.Servicing.dll",
        "Microsoft.VisualStudio.Services.ASTable.Servicing.dll",
        "Microsoft.VisualStudio.Services.BlobStore.Servicing.dll",
        "Microsoft.VisualStudio.Services.CodeReview.Sdk.Servicing.dll",
        "Microsoft.TeamFoundation.Servicing.dll",
        "Microsoft.TeamFoundation.DistributedTask.Servicing.dll",
        "Microsoft.TeamFoundation.DistributedTask.Orchestration.Servicing.dll",
        "Microsoft.VisualStudio.Services.ExtensionManagement.Servicing.dll",
        "Microsoft.VisualStudio.Services.Favorites.Servicing.dll",
        "Microsoft.VisualStudio.Services.Social.Servicing.dll",
        "Microsoft.VisualStudio.Services.Feed.Servicing.dll",
        "Microsoft.VisualStudio.Services.Notifications.Servicing.dll",
        "Microsoft.VisualStudio.Services.Packaging.Servicing.dll",
        "Microsoft.VisualStudio.Services.ReleaseManagement.Servicing.dll",
        "Microsoft.VisualStudio.Services.Sdk.Servicing.dll",
        "Microsoft.VisualStudio.Services.Search.Servicing.dll",
        "Microsoft.VisualStudio.Services.AlmSearch.Servicing.dll",
        "Microsoft.Visualstudio.Services.Servicehooks.Servicing.dll",
        "Microsoft.VisualStudio.Services.SignalR.Servicing.dll",
        "Microsoft.VisualStudio.Services.Sps.Servicing.dll",
        "Microsoft.VisualStudio.Services.Analytics.Sdk.Servicing.dll",
        "Microsoft.Azure.DevOps.ServiceEndpoints.Servicing.dll",
        "Microsoft.Azure.Pipelines.Policy.Servicing.dll",
        "Microsoft.Azure.Pipelines.Deployment.Servicing.dll",
        "Microsoft.Azure.Devops.Tags.Servicing.dll",
        "Microsoft.VisualStudio.Services.Tcm.Servicing.dll"
      };
    }

    public static class ManifestResources
    {
      public const string FrameworkResourceAssemblyName = "Microsoft.TeamFoundation.Framework.Server.dll";
    }

    public static class ConfigurationTokens
    {
      public const string ApplicationTierFileCacheFolder = "@FILE_CACHE_FOLDER@";
      public const string CacheRoot = "@CACHEROOT@";
      public const string ConfigurationDbConnectionString = "@CONFIGURATION_DB_CONNECTION_STRING@";
      public const string FrameworkResourceAssembly = "@FRAMEWORK_RESOURCE_ASSEMBLY@";
      public const string MessageQueueAuthenticationScheme = "@MESSAGE_QUEUE_AUTHENTICATION_SCHEME@";
      public const string RedirectHttpToHttps = "@REDIRECT_HTTP_TO_HTTPS@";
      public const string UseSsl = "@USE_SSL@";
      public const string ViewStateDecryptionKey = "@VIEW_STATE_DECRYPTION_KEY@";
      public const string ViewStateValidationKey = "@VIEW_STATE_VALIDATION_KEY@";
    }

    public static class ApplicationServicingTokenConstants
    {
      public const string ApplicationTier = "ApplicationTier";
      public const string LCID = "LCID";
    }

    public static class Windows
    {
      public static readonly Version MaxSupportedVersion = new Version(10, int.MaxValue, int.MaxValue, int.MaxValue);
    }

    public static class SqlServer
    {
      public static readonly string SqlAzureEdition = "SQL Azure";
      public static readonly string SqlExpress2022InstallFolder = "SqlExpress2022";
      public static readonly string SqlInstanceFeedbackKey = "Software\\Microsoft\\Microsoft Sql Server\\MSSQL{0}.{1}\\CPE";
      public static readonly string SqlTopLevelFeedbackKey = "Software\\Microsoft\\Microsoft Sql Server\\{0}";
      internal static readonly Version SqlServer2022CU12 = new Version(16, 0, 4115, 4);
      internal static readonly Version SqlServer2022RTM = new Version(16, 0, 1000, 6);
      internal static readonly Version SqlServer2022RC1 = new Version(16, 0, 950, 9);
      internal static readonly Version SqlServer2022RC0 = new Version(16, 0, 900, 6);
      internal static readonly Version SqlServer2019RTM = new Version(15, 0, 2000, 5);
      internal static readonly Version SqlServer2019RC1 = new Version(15, 0, 1900, 25);
      internal static readonly Version SqlServer2019CTP32 = new Version(15, 0, 1800, 32);
      internal static readonly Version SqlServer2019CTP31 = new Version(15, 0, 1700, 37);
      internal static readonly Version SqlServer2019CTP30 = new Version(15, 0, 1600, 8);
      internal static readonly Version SqlServer2019CTP25 = new Version(15, 0, 1500, 28);
      internal static readonly Version SqlServer2019CTP24 = new Version(15, 0, 1400, 75);
      internal static readonly Version SqlServer2019CTP23 = new Version(15, 0, 1300, 359);
      internal static readonly Version SqlServer2019CTP22 = new Version(15, 0, 1200, 24);
      internal static readonly Version SqlServer2019CTP21 = new Version(15, 0, 1100, 94);
      internal static readonly Version SqlServer2019CTP2 = new Version(15, 0, 1000, 34);
      internal static readonly Version SqlServer2017CTP1 = new Version(14, 0, 1, 246);
      internal static readonly Version SqlServer2017CTP11 = new Version(14, 0, 100, 187);
      internal static readonly Version SqlServer2017CTP12 = new Version(14, 0, 200, 24);
      internal static readonly Version SqlServer2017CTP13 = new Version(14, 0, 304, 138);
      internal static readonly Version SqlServer2017CTP14 = new Version(14, 0, 405, 198);
      internal static readonly Version SqlServer2017CTP2 = new Version(14, 0, 500, 272);
      internal static readonly Version SqlServer2017CTP21 = new Version(14, 0, 600, 250);
      internal static readonly Version SqlServer2017RC1 = new Version(14, 0, 800, 90);
      internal static readonly Version SqlServer2017RC2 = new Version(14, 0, 900, 75);
      internal static readonly Version SqlServer2017RTM = new Version(14, 0, 1000, 169);
      internal static readonly Version SqlServer2017Max = new Version(14, 0, int.MaxValue);
      internal static readonly Version SqlServer2016RTM = new Version(13, 0, 1601, 5);
      internal static readonly Version SqlServer2016SP1 = new Version(13, 0, 4001, 0);
      internal static readonly Version SqlServer2016SP2 = new Version(13, 0, 5026, 0);
      internal static readonly Version SqlServer2016SP3 = new Version(13, 0, 6300, 2);
      internal static readonly Version SqlServer2016Max = new Version(13, 0, int.MaxValue);
      internal static readonly Version SqlServer2014RTM = new Version(12, 0, 2000, 8);
      internal static readonly Version SqlServer2014SP1 = new Version(12, 0, 4100, 1);
      internal static readonly Version SqlServer2014SP2 = new Version(12, 0, 5000, 0);
      internal static readonly Version SqlServer2014SP3 = new Version(12, 0, 6024, 0);
      internal static readonly Version SqlServer2014Max = new Version(12, 0, int.MaxValue);
      internal static readonly Version SqlServer2012RTM = new Version(11, 0, 2100, 60);
      internal static readonly Version SqlServer2012SP1 = new Version(11, 0, 3000, 0);
      internal static readonly Version SqlServer2012SP2 = new Version(11, 0, 5058, 0);
      internal static readonly Version SqlServer2012SP3 = new Version(11, 0, 6020, 0);
      internal static readonly Version SqlServer2012SP4 = new Version(11, 0, 7001, 0);
      internal static readonly Version SqlServer2012Max = new Version(11, 0, int.MaxValue);
      internal static readonly Version SqlServer2008R2RTM = new Version(10, 50, 1600, 1);
      internal static readonly Version SqlServer2008R2SP1 = new Version(10, 50, 2500, 0);
      internal static readonly Version SqlServer2008R2SP2 = new Version(10, 50, 4000, 0);
      internal static readonly Version SqlServer2008R2SP3 = new Version(10, 50, 6000, 34);
      internal static readonly Version SqlServer2008R2Max = new Version(10, 50, int.MaxValue);
      internal static readonly Version SqlServer2008RTM = new Version(10, 0, 1600, 22);
      internal static readonly Version SqlServer2008SP1 = new Version(10, 0, 2531, 0);
      internal static readonly Version SqlServer2008SP2 = new Version(10, 0, 4000, 0);
      internal static readonly Version SqlServer2008SP3 = new Version(10, 0, 5500, 0);
      internal static readonly Version SqlServer2008SP4 = new Version(10, 0, 6000, 29);
      internal static readonly Version SqlServer2008Max = new Version(10, 0, int.MaxValue);
      internal static readonly Version SqlServer2005RTM = new Version(9, 0, 1399, 6);
      internal static readonly Version SqlServer2005SP1 = new Version(9, 0, 2047);
      internal static readonly Version SqlServer2005SP2 = new Version(9, 0, 3042);
      internal static readonly Version SqlServer2005SP3 = new Version(9, 0, 4035);
      internal static readonly Version SqlServer2005SP4 = new Version(9, 0, 5000);
      internal static readonly Version SqlServer2005Max = new Version(9, 0, int.MaxValue);
      internal static readonly Version SqlServer2000RTM = new Version(8, 0, 194);
      private static readonly Version[] s_supportedExpressVersions = new Version[2]
      {
        AOConstants.SqlServer.SqlServer2019RTM,
        AOConstants.SqlServer.SqlServer2022RTM
      };
      private static readonly Version[] s_supportedVersions = new Version[2]
      {
        AOConstants.SqlServer.SqlServer2019RTM,
        AOConstants.SqlServer.SqlServer2022RTM
      };
      private static readonly Version[] s_autoUpgradable2022Versions = new Version[7]
      {
        AOConstants.SqlServer.SqlServer2012SP4,
        AOConstants.SqlServer.SqlServer2014SP3,
        AOConstants.SqlServer.SqlServer2016SP3,
        AOConstants.SqlServer.SqlServer2017RTM,
        AOConstants.SqlServer.SqlServer2019RTM,
        AOConstants.SqlServer.SqlServer2022RC0,
        AOConstants.SqlServer.SqlServer2022RC1
      };
      private static readonly Dictionary<Version, string> s_sqlVersionFriendlyNames = new Dictionary<Version, string>()
      {
        {
          new Version(16, 0),
          "SQL Server 2022"
        },
        {
          new Version(15, 0),
          "SQL Server 2019"
        },
        {
          new Version(14, 0),
          "SQL Server 2017"
        },
        {
          new Version(13, 0),
          "SQL Server 2016"
        },
        {
          new Version(12, 0),
          "SQL Server 2014"
        },
        {
          new Version(11, 0),
          "SQL Server 2012"
        },
        {
          new Version(10, 50),
          "SQL Server 2008 R2"
        },
        {
          new Version(10, 0),
          "SQL Server 2008"
        },
        {
          new Version(9, 0),
          "SQL Server 2005"
        },
        {
          new Version(8, 0),
          "SQL Server 2000"
        },
        {
          new Version(7, 0),
          "SQL Server 7"
        }
      };
      private static readonly Tuple<Version, string>[] s_sqlReleasesFriendlyNames = new Tuple<Version, string>[60]
      {
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2022RTM, "SQL Server 2022 RTM"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2022RC1, "SQL Server 2022 RC1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2022RC0, "SQL Server 2022 RC0"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019RTM, "SQL Server 2019 RTM"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019RC1, "SQL Server 2019 RC1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP32, "SQL Server 2019 CTP 3.2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP31, "SQL Server 2019 CTP 3.1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP30, "SQL Server 2019 CTP 3.0"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP25, "SQL Server 2019 CTP 2.5"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP24, "SQL Server 2019 CTP 2.4"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP23, "SQL Server 2019 CTP 2.3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP22, "SQL Server 2019 CTP 2.2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP21, "SQL Server 2019 CTP 2.1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP21, "SQL Server 2019 CTP 2.1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2019CTP2, "SQL Server 2019 CTP 2.0"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017RTM, "SQL Server 2017 RTM"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017RC2, "SQL Server 2017 RC2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017RC1, "SQL Server 2017 RC1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017CTP21, "SQL Server 2017 CTP 2.1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017CTP2, "SQL Server 2017 CTP 2.0"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017CTP14, "SQL Server 2017 CTP 1.4"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017CTP13, "SQL Server 2017 CTP 1.3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017CTP12, "SQL Server 2017 CTP 1.2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017CTP11, "SQL Server 2017 CTP 1.1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2017CTP1, "SQL Server 2017 CTP 1"),
        Tuple.Create<Version, string>(new Version(14, 0), AdminCommonResources.SqlServer2017PreRelease()),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2016SP3, "SQL Server 2016 SP3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2016SP2, "SQL Server 2016 SP2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2016SP1, "SQL Server 2016 SP1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2016RTM, "SQL Server 2016 RTM"),
        Tuple.Create<Version, string>(new Version(13, 0), AdminCommonResources.SqlServer2016PreRelease()),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2014SP3, "SQL Server 2014 SP3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2014SP2, "SQL Server 2014 SP2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2014SP1, "SQL Server 2014 SP1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2014RTM, "SQL Server 2014 RTM"),
        Tuple.Create<Version, string>(new Version(12, 0), AdminCommonResources.SqlServer2014PreRelease()),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2012SP4, "SQL Server 2012 SP4"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2012SP3, "SQL Server 2012 SP3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2012SP2, "SQL Server 2012 SP2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2012SP1, "SQL Server 2012 SP1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2012RTM, "SQL Server 2012 RTM"),
        Tuple.Create<Version, string>(new Version(11, 0), AdminCommonResources.SqlServer2012PreRelease()),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008R2SP3, "SQL Server 2008 R2 SP3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008R2SP2, "SQL Server 2008 R2 SP2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008R2SP1, "SQL Server 2008 R2 SP1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008R2RTM, "SQL Server 2008 R2 RTM"),
        Tuple.Create<Version, string>(new Version(10, 50), AdminCommonResources.SqlServer2008R2PreRelease()),
        Tuple.Create<Version, string>(new Version(10, 0, 6000, 0), "SQL Server 2008 SP4"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008SP3, "SQL Server 2008 SP3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008SP2, "SQL Server 2008 SP2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008SP1, "SQL Server 2008 SP1"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2008RTM, "SQL Server 2008 RTM"),
        Tuple.Create<Version, string>(new Version(10, 0), AdminCommonResources.SqlServer2008PreRelease()),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2005SP4, "SQL Server 2005 SP4"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2005SP3, "SQL Server 2005 SP3"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2005SP2, "SQL Server 2005 SP2"),
        Tuple.Create<Version, string>(AOConstants.SqlServer.SqlServer2005SP1, "SQL Server 2005 SP1"),
        Tuple.Create<Version, string>(new Version(9, 0), "SQL Server 2005"),
        Tuple.Create<Version, string>(new Version(8, 0), "SQL Server 2000"),
        Tuple.Create<Version, string>(new Version(7, 0), "SQL Server 7.0")
      };

      public static Version[] SupportedExpressVersions => AOConstants.SqlServer.s_supportedExpressVersions;

      public static bool IsSupportedExpressVersion(Version productVersion) => AOConstants.SqlServer.IsSupportedVersion(AOConstants.SqlServer.s_supportedExpressVersions, productVersion);

      public static bool IsSupportedVersion(Version productVersion) => AOConstants.SqlServer.IsSupportedVersion(AOConstants.SqlServer.s_supportedVersions, productVersion);

      internal static bool IsSupportedVersion(Version[] supportedVersions, Version productVersion)
      {
        AOConstants.SqlServer.CheckProductVersion(productVersion);
        return ((IEnumerable<Version>) supportedVersions).FirstOrDefault<Version>((Func<Version, bool>) (v => v.Major == productVersion.Major && v.Minor == productVersion.Minor && v <= productVersion)) != (Version) null;
      }

      public static Version[] SupportedVersions => AOConstants.SqlServer.s_supportedVersions;

      public static Version[] AutoUpgradableExpressVersions => AOConstants.SqlServer.GetAutoUpgradableExpressVersions(OSDetails.MajorVersion, OSDetails.MinorVersion);

      public static bool IsAutoUpgradableExpressVersion(Version productVersion) => AOConstants.SqlServer.IsAutoUpgradableExpressVersion(AOConstants.SqlServer.AutoUpgradableExpressVersions, AOConstants.SqlServer.GetBundledExpressVersion(OSDetails.MajorVersion, OSDetails.MinorVersion), productVersion);

      internal static bool IsAutoUpgradableExpressVersion(
        Version[] autoupgradableVersions,
        Version bundledVersion,
        Version productVersion)
      {
        AOConstants.SqlServer.CheckProductVersion(productVersion);
        return !(productVersion >= bundledVersion) && ((IEnumerable<Version>) autoupgradableVersions).FirstOrDefault<Version>((Func<Version, bool>) (v => v.Major == productVersion.Major && v.Minor == productVersion.Minor && v <= productVersion)) != (Version) null;
      }

      internal static Version[] GetAutoUpgradableExpressVersions(
        int osMajorVerion,
        int osMinorVersion)
      {
        return AOConstants.SqlServer.s_autoUpgradable2022Versions;
      }

      internal static Version GetBundledExpressVersion(int osMajorVerion, int osMinorVersion) => AOConstants.SqlServer.BundledExpressVersion;

      public static Dictionary<Version, string> SqlVersionFriendlyNames => AOConstants.SqlServer.s_sqlVersionFriendlyNames;

      public static string GetFriendlyNameForSqlVersion(Version productVersion)
      {
        AOConstants.SqlServer.CheckProductVersion(productVersion);
        if (productVersion > ((IEnumerable<Tuple<Version, string>>) AOConstants.SqlServer.s_sqlReleasesFriendlyNames).Max<Tuple<Version, string>, Version>((Func<Tuple<Version, string>, Version>) (t => t.Item1)))
          return string.Format("SQL Server v{0}", (object) productVersion);
        for (int index = 0; index < AOConstants.SqlServer.s_sqlReleasesFriendlyNames.Length; ++index)
        {
          if (productVersion >= AOConstants.SqlServer.s_sqlReleasesFriendlyNames[index].Item1)
            return AOConstants.SqlServer.s_sqlReleasesFriendlyNames[index].Item2;
        }
        return productVersion.ToString();
      }

      public static Version BundledExpressVersion => AOConstants.SqlServer.SqlServer2022RTM;

      public static Version BundledExpressRtmVersion => AOConstants.SqlServer.SqlServer2022RTM;

      public static string BundledExpressReleaseName => "SQL Server Express 2022 RTM";

      public static string SqlServerLogsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server\\160\\Setup Bootstrap\\Log");

      public static int InstalledExpressMajorVersion => AOConstants.SqlServer.BundledExpressVersion.Major;

      internal static Tuple<Version, string>[] SqlReleasesFriendlyNames => AOConstants.SqlServer.s_sqlReleasesFriendlyNames;

      private static void CheckProductVersion(Version productVersion)
      {
        if (productVersion.Minor % 50 != 0)
          throw new ArgumentException(string.Format("{0} is not a product version.", (object) productVersion), nameof (productVersion));
      }
    }

    public static class Upgrade
    {
      public const string MinimumUpgradableServiceLevel = "Dev14.M62";
    }

    public static class Azure
    {
      public static class SQL
      {
        public static readonly string BackupDocsUrl = "https://aka.ms/AZSQLBackup";
      }
    }
  }
}
