// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationScopeDefinitions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationScopeDefinitions
  {
    public const string UserImpersonationScope = "user_impersonation";
    public const string SignoutScope = "signout";
    public const string AppTokenScope = "app_token";
    private static readonly Lazy<AuthorizationScopeDefinitions> DefaultContainer = new Lazy<AuthorizationScopeDefinitions>(new Func<AuthorizationScopeDefinitions>(AuthorizationScopeDefinitions.CreateDefault));
    private static readonly string[] ClientOmBasePatterns = new string[7]
    {
      "/Services/*/LocationService.asmx#POST",
      "/*/Services/*/LocationService.asmx#POST",
      "/DefaultCollection/*/Services/*/LocationService.asmx#POST",
      "/*/Administration/*/LocationService.asmx#POST",
      "/Services/*/Registration.asmx#POST",
      "/*/Services/*/Registration.asmx#POST",
      "/*/*/Administration/*/LocationService.asmx#POST"
    };

    public static AuthorizationScopeDefinitions Default => AuthorizationScopeDefinitions.DefaultContainer.Value;

    public static IDictionary<string, string> LocalizedTitleMap => AuthorizationScopeDefinitions.GetLocalizedTitleMap();

    private static IDictionary<string, string> GetLocalizedTitleMap() => (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "AdvancedSecurity (read)",
        FrameworkResources.ScopeAdvancedSecurityRead()
      },
      {
        "AdvancedSecurity (read and write)",
        FrameworkResources.ScopeAdvancedSecurityReadWrite()
      },
      {
        "AdvancedSecurity (read, write, and manage)",
        FrameworkResources.ScopeAdvancedSecurityReadWriteManage()
      },
      {
        "Agent Pools (read)",
        FrameworkResources.ScopeAgentPoolsRead()
      },
      {
        "Agent Pools (read, manage)",
        FrameworkResources.ScopeAgentPoolsReadManage()
      },
      {
        "Analytics (read)",
        FrameworkResources.ScopeAnalyticsRead()
      },
      {
        "Audit Read Log",
        FrameworkResources.AuditReadLogScope()
      },
      {
        "Audit Manage Streams",
        FrameworkResources.AuditManageStreamsScope()
      },
      {
        "Build (read)",
        FrameworkResources.ScopeBuildRead()
      },
      {
        "Build (read and execute)",
        FrameworkResources.ScopeBuildReadExecute()
      },
      {
        "Code (read)",
        FrameworkResources.ScopeCodeRead()
      },
      {
        "Code (read and write)",
        FrameworkResources.ScopeCodeReadWrite()
      },
      {
        "Code (read, write, and manage)",
        FrameworkResources.ScopeCodeReadWriteManage()
      },
      {
        "Code (full)",
        FrameworkResources.ScopeCodeFull()
      },
      {
        "Code (status)",
        FrameworkResources.ScopeCodeStatus()
      },
      {
        "Connected Server",
        FrameworkResources.ScopeConnectedServer()
      },
      {
        "Deployment group (read, manage)",
        FrameworkResources.ScopeDeploymentGroupReadManage()
      },
      {
        "Entitlements (Read)",
        FrameworkResources.ScopeEntlmntsRead()
      },
      {
        "Environment (read, manage)",
        FrameworkResources.ScopeEnvironmentReadManage()
      },
      {
        "Extension data (read)",
        FrameworkResources.ScopeExtnDataRead()
      },
      {
        "Extension data (read and write)",
        FrameworkResources.ScopeExtnDataReadWrite()
      },
      {
        "Extensions (read)",
        FrameworkResources.ScopeExtnsRead()
      },
      {
        "Extensions (read and manage)",
        FrameworkResources.ScopeExtnsReadManage()
      },
      {
        "Graph (read)",
        FrameworkResources.ScopeGraphRead()
      },
      {
        "Graph (manage)",
        FrameworkResources.ScopeGraphManage()
      },
      {
        "Identity (read)",
        FrameworkResources.ScopeIdentityRead()
      },
      {
        "Marketplace",
        FrameworkResources.ScopeMarketplace()
      },
      {
        "Marketplace (acquire)",
        FrameworkResources.ScopeMarketplaceAcquire()
      },
      {
        "Marketplace (manage)",
        FrameworkResources.ScopeMarketplaceManage()
      },
      {
        "Marketplace (publish)",
        FrameworkResources.ScopeMarketplacePublish()
      },
      {
        "Notifications (diagnostics)",
        FrameworkResources.ScopeNotificationDiagnostics()
      },
      {
        "Notifications (manage)",
        FrameworkResources.ScopeNotificationsManage()
      },
      {
        "Notifications (read)",
        FrameworkResources.ScopeNotificationsRead()
      },
      {
        "Notifications (write)",
        FrameworkResources.ScopeNotificationsWrite()
      },
      {
        "MemberEntitlement Management (read)",
        FrameworkResources.ScopeMemberEntitlementManagementRead()
      },
      {
        "MemberEntitlement Management (write)",
        FrameworkResources.ScopeMemberEntitlementManagementWrite()
      },
      {
        "Packaging (read)",
        FrameworkResources.ScopePackagingRead()
      },
      {
        "Packaging (read and write)",
        FrameworkResources.ScopePackagingReadWrite()
      },
      {
        "Packaging (read, write, and manage)",
        FrameworkResources.ScopePackagingReadWriteManage()
      },
      {
        "Pipeline Resources (use)",
        FrameworkResources.ScopePipelineResourcesUse()
      },
      {
        "Pipeline Resources (use and manage)",
        FrameworkResources.ScopePipelineResourcesUseManage()
      },
      {
        "PR threads",
        FrameworkResources.PrThreads()
      },
      {
        "Project and team (read and write)",
        FrameworkResources.ScopePjTmReadWrite()
      },
      {
        "Project and team (read, write and manage)",
        FrameworkResources.ScopePjTmReadWriteManage()
      },
      {
        "Project and team (read)",
        FrameworkResources.ScopeProjectTeamRead()
      },
      {
        "Release (read)",
        FrameworkResources.ScopeReleaseRead()
      },
      {
        "Release (read, write and execute)",
        FrameworkResources.ScopeReleaseReadWriteExecute()
      },
      {
        "Release (read, write, execute and manage)",
        FrameworkResources.ScopeReleaseReadWriteExecuteManage()
      },
      {
        "Secure Files (read)",
        FrameworkResources.SecureFilesRead()
      },
      {
        "Secure Files (read, create)",
        FrameworkResources.SecureFilesReadCreate()
      },
      {
        "Secure Files (read, create, and manage)",
        FrameworkResources.SecureFilesReadCreateManage()
      },
      {
        "Security (manage)",
        FrameworkResources.ScopeSecurityManage()
      },
      {
        "Service Endpoints (read)",
        FrameworkResources.ScopeSvcEndptsRead()
      },
      {
        "Service Endpoints (read and query)",
        FrameworkResources.ScopeSvcEndptsReadQuery()
      },
      {
        "Service Endpoints (read, query and manage)",
        FrameworkResources.ScopeSvcEndptsReadQueryManage()
      },
      {
        "Symbols (read)",
        FrameworkResources.ScopesSymbolsRead()
      },
      {
        "Symbols (read and write)",
        FrameworkResources.ScopesSymbolsReadWrite()
      },
      {
        "Symbols (read, write and manage)",
        FrameworkResources.ScopesSymbolsReadWriteManage()
      },
      {
        "Team dashboards (manage)",
        FrameworkResources.ScopeTeamDshbdsManage()
      },
      {
        "Team dashboards (read)",
        FrameworkResources.ScopeTeamDshbdsRead()
      },
      {
        "Test management (read)",
        FrameworkResources.ScopeTestMgmtRead()
      },
      {
        "Test management (read and write)",
        FrameworkResources.ScopeTestMgmtReadWrite()
      },
      {
        "Task Groups (read)",
        FrameworkResources.ScopeTskGrpsRead()
      },
      {
        "Task Groups (read, create)",
        FrameworkResources.ScopeTskGrpsReadCreate()
      },
      {
        "Task Groups (read, create and manage)",
        FrameworkResources.ScopeTskGrpsReadCreateManage()
      },
      {
        "User profile (read)",
        FrameworkResources.ScopeUserProfileRead()
      },
      {
        "User profile (write)",
        FrameworkResources.ScopeUserProfileWrite()
      },
      {
        "Variable Groups (read)",
        FrameworkResources.ScopeVarGrpsRead()
      },
      {
        "Variable Groups (read, create)",
        FrameworkResources.ScopeVarGrpsReadCreate()
      },
      {
        "Variable Groups (read, create and manage)",
        FrameworkResources.ScopeVarGrpsReadCreateManage()
      },
      {
        "Work items (read)",
        FrameworkResources.ScopeWorkItemsRead()
      },
      {
        "Work items (read and write)",
        FrameworkResources.ScopeWorkItemsReadWrite()
      },
      {
        "Work items (full)",
        FrameworkResources.ScopeWorkItemsFull()
      },
      {
        "Identity (manage)",
        FrameworkResources.ScopeIdentityManage()
      },
      {
        "Wiki (read)",
        FrameworkResources.ScopeWikiRead()
      },
      {
        "Wiki (read and write)",
        FrameworkResources.ScopeWikiReadWrite()
      },
      {
        "Token Administration",
        FrameworkResources.ScopeTokenAdministration()
      },
      {
        "Delegated Authorization Tokens",
        FrameworkResources.ScopeTokens()
      }
    };

    public static IDictionary<string, AuthorizationScopeGroupings> Groupings => AuthorizationScopeDefinitions.GetGroupings();

    private static IDictionary<string, AuthorizationScopeGroupings> GetGroupings() => (IDictionary<string, AuthorizationScopeGroupings>) new Dictionary<string, AuthorizationScopeGroupings>()
    {
      {
        "advancedSecurity",
        new AuthorizationScopeGroupings()
        {
          Key = "advancedSecurity",
          Name = ScopeResources.AdvancedSecurity(),
          Description = ScopeResources.AdvancedSecurityDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.advsec",
            "vso.advsec_write",
            "vso.advsec_manage"
          }
        }
      },
      {
        "agentPools",
        new AuthorizationScopeGroupings()
        {
          Key = "agentPools",
          Name = ScopeResources.AgentPools(),
          Description = ScopeResources.AgentPoolsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.agentpools",
            "vso.agentpools_manage"
          }
        }
      },
      {
        "analytics",
        new AuthorizationScopeGroupings()
        {
          Key = "analytics",
          Name = ScopeResources.Analytics(),
          Description = ScopeResources.AnalyticsDescription(),
          ScopeKeys = new List<string>() { "vso.analytics" }
        }
      },
      {
        "auditing",
        new AuthorizationScopeGroupings()
        {
          Key = "auditing",
          Name = ScopeResources.Auditing(),
          Description = ScopeResources.AuditingDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.auditlog",
            "vso.auditstreams_manage",
            "vso.auditstreams_delete"
          }
        }
      },
      {
        "build",
        new AuthorizationScopeGroupings()
        {
          Key = "build",
          Name = ScopeResources.Build(),
          Description = ScopeResources.BuildDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.build",
            "vso.build_execute"
          }
        }
      },
      {
        "buildCache",
        new AuthorizationScopeGroupings()
        {
          Key = "buildCache",
          Name = ScopeResources.BuildCache(),
          Description = ScopeResources.BuildCacheDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.buildcache",
            "vso.buildcache_write"
          }
        }
      },
      {
        "code",
        new AuthorizationScopeGroupings()
        {
          Key = "code",
          Name = ScopeResources.Code(),
          Description = ScopeResources.CodeDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.code",
            "vso.code_write",
            "vso.code_full",
            "vso.code_status",
            "vso.code_manage"
          }
        }
      },
      {
        "prThreads",
        new AuthorizationScopeGroupings()
        {
          Key = "prThreads",
          Name = ScopeResources.PrThreads(),
          Description = ScopeResources.PrThreadsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.threads_full"
          }
        }
      },
      {
        "codeSearch",
        new AuthorizationScopeGroupings()
        {
          Key = "codeSearch",
          Name = ScopeResources.CodeSearch(),
          Description = ScopeResources.CodeSearchDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.codesearch",
            "vso.connected_server"
          }
        }
      },
      {
        "connectedServer",
        new AuthorizationScopeGroupings()
        {
          Key = "connectedServer",
          Name = ScopeResources.ConnectedServer(),
          Description = ScopeResources.ConnectedServerDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.agentpools",
            "vso.agentpools_manage"
          }
        }
      },
      {
        "drop",
        new AuthorizationScopeGroupings()
        {
          Key = "drop",
          Name = ScopeResources.Drop(),
          Description = ScopeResources.DropDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.drop",
            "vso.drop_write",
            "vso.drop_manage"
          }
        }
      },
      {
        "machineGroup",
        new AuthorizationScopeGroupings()
        {
          Key = "machineGroup",
          Name = ScopeResources.MachineGroup(),
          Description = ScopeResources.MachineGroupDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.machinegroup_manage"
          }
        }
      },
      {
        "entitlements",
        new AuthorizationScopeGroupings()
        {
          Key = "entitlements",
          Name = ScopeResources.Entitlements(),
          Description = ScopeResources.Entitlements(),
          ScopeKeys = new List<string>()
          {
            "vso.entitlements"
          }
        }
      },
      {
        "environment",
        new AuthorizationScopeGroupings()
        {
          Key = "environment",
          Name = ScopeResources.Environment(),
          Description = ScopeResources.EnvironmentDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.environment_manage"
          }
        }
      },
      {
        "extensionData",
        new AuthorizationScopeGroupings()
        {
          Key = "extensionData",
          Name = ScopeResources.ExtensionData(),
          Description = ScopeResources.ExtensionDataDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.extension.data",
            "vso.extension.data_write"
          }
        }
      },
      {
        "extensions",
        new AuthorizationScopeGroupings()
        {
          Key = "extensions",
          Name = ScopeResources.Extensions(),
          Description = ScopeResources.ExtensionsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.extension",
            "vso.extension_manage"
          }
        }
      },
      {
        "githubConnections",
        new AuthorizationScopeGroupings()
        {
          Key = "githubConnections",
          Name = ScopeResources.GitHubConnections(),
          Description = ScopeResources.GitHubConnectionsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.githubconnections",
            "vso.githubconnections_manage"
          }
        }
      },
      {
        "graph",
        new AuthorizationScopeGroupings()
        {
          Key = "graph",
          Name = ScopeResources.Graph(),
          Description = ScopeResources.GraphDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.graph",
            "vso.graph_manage"
          }
        }
      },
      {
        "identity",
        new AuthorizationScopeGroupings()
        {
          Key = "identity",
          Name = ScopeResources.Identity(),
          Description = ScopeResources.IdentityDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.identity",
            "vso.identity_manage"
          }
        }
      },
      {
        "loadTest",
        new AuthorizationScopeGroupings()
        {
          Key = "loadTest",
          Name = ScopeResources.LoadTest(),
          Description = ScopeResources.LoadTestDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.loadtest",
            "vso.loadtest_write"
          }
        }
      },
      {
        "marketplace",
        new AuthorizationScopeGroupings()
        {
          Key = "marketplace",
          Name = ScopeResources.Marketplace(),
          Description = ScopeResources.MarketplaceDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.gallery",
            "vso.gallery_acquire",
            "vso.gallery_manage",
            "vso.gallery_publish"
          }
        }
      },
      {
        "memberEntitlementManagement",
        new AuthorizationScopeGroupings()
        {
          Key = "memberEntitlementManagement",
          Name = ScopeResources.MemberEntitlementManagement(),
          Description = ScopeResources.MemberEntitlementManagementDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.memberentitlementmanagement",
            "vso.memberentitlementmanagement_write"
          }
        }
      },
      {
        "notifications",
        new AuthorizationScopeGroupings()
        {
          Key = "notifications",
          Name = ScopeResources.Notifications(),
          Description = ScopeResources.NotificationsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.notification",
            "vso.notification_write",
            "vso.notification_manage",
            "vso.notification_diagnostics"
          }
        }
      },
      {
        "packaging",
        new AuthorizationScopeGroupings()
        {
          Key = "packaging",
          Name = ScopeResources.Packaging(),
          Description = ScopeResources.PackagingDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.packaging",
            "vso.packaging_write",
            "vso.packaging_manage"
          }
        }
      },
      {
        "pipelineResources",
        new AuthorizationScopeGroupings()
        {
          Key = "pipelineResources",
          Name = ScopeResources.PipelineResources(),
          Description = ScopeResources.PipelineResourcesDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.pipelineresources_use",
            "vso.pipelineresources_manage"
          }
        }
      },
      {
        "projectAndTeam",
        new AuthorizationScopeGroupings()
        {
          Key = "projectAndTeam",
          Name = ScopeResources.ProjectAndTeam(),
          Description = ScopeResources.ProjectAndTeamDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.project",
            "vso.project_write",
            "vso.project_manage"
          }
        }
      },
      {
        "release",
        new AuthorizationScopeGroupings()
        {
          Key = "release",
          Name = ScopeResources.Release(),
          Description = ScopeResources.ReleaseDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.release",
            "vso.release_execute",
            "vso.release_manage"
          }
        }
      },
      {
        "secureFiles",
        new AuthorizationScopeGroupings()
        {
          Key = "secureFiles",
          Name = ScopeResources.SecureFiles(),
          Description = ScopeResources.SecureFilesDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.securefiles_read",
            "vso.securefiles_write",
            "vso.securefiles_manage"
          }
        }
      },
      {
        "security",
        new AuthorizationScopeGroupings()
        {
          Key = "security",
          Name = ScopeResources.Security(),
          Description = ScopeResources.SecurityDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.security_manage"
          }
        }
      },
      {
        "serviceEndpoints",
        new AuthorizationScopeGroupings()
        {
          Key = "serviceEndpoints",
          Name = ScopeResources.ServiceEndpoints(),
          Description = ScopeResources.ServiceEndpointsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.serviceendpoint",
            "vso.serviceendpoint_query",
            "vso.serviceendpoint_manage"
          }
        }
      },
      {
        "symbols",
        new AuthorizationScopeGroupings()
        {
          Key = "symbols",
          Name = ScopeResources.Symbols(),
          Description = ScopeResources.SymbolsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.symbols",
            "vso.symbols_write",
            "vso.symbols_manage"
          }
        }
      },
      {
        "taskGroups",
        new AuthorizationScopeGroupings()
        {
          Key = "taskGroups",
          Name = ScopeResources.TaskGroups(),
          Description = ScopeResources.TaskGroupsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.taskgroups_read",
            "vso.taskgroups_write",
            "vso.taskgroups_manage"
          }
        }
      },
      {
        "teamDashboards",
        new AuthorizationScopeGroupings()
        {
          Key = "teamDashboards",
          Name = ScopeResources.TeamDashboard(),
          Description = ScopeResources.TeamDashboardDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.dashboards",
            "vso.dashboards_manage"
          }
        }
      },
      {
        "testManagement",
        new AuthorizationScopeGroupings()
        {
          Key = "testManagement",
          Name = ScopeResources.TestManagement(),
          Description = ScopeResources.TestManagementDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.test",
            "vso.test_write"
          }
        }
      },
      {
        "userProfile",
        new AuthorizationScopeGroupings()
        {
          Key = "userProfile",
          Name = ScopeResources.UserProfile(),
          Description = ScopeResources.UserProfileDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.profile",
            "vso.profile_write"
          }
        }
      },
      {
        "variableGroups",
        new AuthorizationScopeGroupings()
        {
          Key = "variableGroups",
          Name = ScopeResources.VariableGroups(),
          Description = ScopeResources.VariableGroupsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.variablegroups_read",
            "vso.variablegroups_write",
            "vso.variablegroups_manage"
          }
        }
      },
      {
        "wiki",
        new AuthorizationScopeGroupings()
        {
          Key = "wiki",
          Name = ScopeResources.Wiki(),
          Description = ScopeResources.WikiDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.wiki",
            "vso.wiki_write"
          }
        }
      },
      {
        "workItemSearch",
        new AuthorizationScopeGroupings()
        {
          Key = "workItemSearch",
          Name = ScopeResources.WorkItemSearch(),
          Description = ScopeResources.WorkItemSearchDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.workitemsearch"
          }
        }
      },
      {
        "workItems",
        new AuthorizationScopeGroupings()
        {
          Key = "workItems",
          Name = ScopeResources.WorkItems(),
          Description = ScopeResources.WorkItemsDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.work",
            "vso.work_write",
            "vso.work_full"
          }
        }
      },
      {
        "tokens",
        new AuthorizationScopeGroupings()
        {
          Key = "tokens",
          Name = ScopeResources.Tokens(),
          Description = ScopeResources.TokensDescription(),
          ScopeKeys = new List<string>() { "vso.tokens" }
        }
      },
      {
        "tokenadmin",
        new AuthorizationScopeGroupings()
        {
          Key = "tokenadmin",
          Name = ScopeResources.TokenAdministration(),
          Description = ScopeResources.TokenAdministrationDescription(),
          ScopeKeys = new List<string>()
          {
            "vso.tokenadministration",
            "vso.tokens"
          }
        }
      }
    };

    public static IDictionary<string, string> ScopeToAccessRights => AuthorizationScopeDefinitions.GetScopeToAccessRights();

    private static IDictionary<string, string> GetScopeToAccessRights() => (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "vso.advsec",
        ScopeResources.Read()
      },
      {
        "vso.advsec_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.advsec_manage",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.agentpools",
        ScopeResources.Read()
      },
      {
        "vso.agentpools_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.analytics",
        ScopeResources.Read()
      },
      {
        "vso.auditlog",
        ScopeResources.ReadAuditLog()
      },
      {
        "vso.auditstreams_manage",
        ScopeResources.ManageStreams()
      },
      {
        "vso.auditstreams_delete",
        ScopeResources.DeleteStreams()
      },
      {
        "vso.build",
        ScopeResources.Read()
      },
      {
        "vso.build_execute",
        ScopeResources.ReadAndExecute()
      },
      {
        "vso.buildcache",
        ScopeResources.Read()
      },
      {
        "vso.buildcache_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.code",
        ScopeResources.Read()
      },
      {
        "vso.code_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.code_full",
        ScopeResources.Full()
      },
      {
        "vso.code_manage",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.code_status",
        ScopeResources.Status()
      },
      {
        "vso.codesearch",
        ScopeResources.Read()
      },
      {
        "vso.threads_full",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.connected_server",
        ScopeResources.ConnectedServer()
      },
      {
        "vso.drop",
        ScopeResources.Read()
      },
      {
        "vso.drop_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.drop_manage",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.machinegroup_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.entitlements",
        ScopeResources.Read()
      },
      {
        "vso.environment_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.extension.data",
        ScopeResources.Read()
      },
      {
        "vso.extension.data_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.extension",
        ScopeResources.Read()
      },
      {
        "vso.extension_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.features",
        ScopeResources.Read()
      },
      {
        "vso.features_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.githubconnections",
        ScopeResources.Read()
      },
      {
        "vso.githubconnections_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.graph",
        ScopeResources.Read()
      },
      {
        "vso.graph_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.identity",
        ScopeResources.Read()
      },
      {
        "vso.identity_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.loadtest",
        ScopeResources.Read()
      },
      {
        "vso.loadtest_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.gallery",
        ScopeResources.Read()
      },
      {
        "vso.gallery_acquire",
        ScopeResources.Acquire()
      },
      {
        "vso.gallery_manage",
        ScopeResources.Manage()
      },
      {
        "vso.gallery_publish",
        ScopeResources.Publish()
      },
      {
        "vso.memberentitlementmanagement",
        ScopeResources.Read()
      },
      {
        "vso.memberentitlementmanagement_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.notification",
        ScopeResources.Read()
      },
      {
        "vso.notification_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.notification_manage",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.notification_diagnostics",
        ScopeResources.Diagnostics()
      },
      {
        "vso.packaging",
        ScopeResources.Read()
      },
      {
        "vso.packaging_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.packaging_manage",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.pipelineresources_use",
        ScopeResources.Use()
      },
      {
        "vso.pipelineresources_manage",
        ScopeResources.UseAndManage()
      },
      {
        "vso.project",
        ScopeResources.Read()
      },
      {
        "vso.project_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.project_manage",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.release",
        ScopeResources.Read()
      },
      {
        "vso.release_execute",
        ScopeResources.ReadWriteAndExecute()
      },
      {
        "vso.release_manage",
        ScopeResources.ReadWriteExecuteAndManage()
      },
      {
        "vso.securefiles_read",
        ScopeResources.Read()
      },
      {
        "vso.securefiles_write",
        ScopeResources.ReadAndCreate()
      },
      {
        "vso.securefiles_manage",
        ScopeResources.ReadCreateAndManage()
      },
      {
        "vso.security_manage",
        ScopeResources.Manage()
      },
      {
        "vso.serviceendpoint",
        ScopeResources.Read()
      },
      {
        "vso.serviceendpoint_query",
        ScopeResources.ReadAndQuery()
      },
      {
        "vso.serviceendpoint_manage",
        ScopeResources.ReadQueryAndManage()
      },
      {
        "vso.symbols",
        ScopeResources.Read()
      },
      {
        "vso.symbols_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.symbols_manage",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.taskgroups_read",
        ScopeResources.Read()
      },
      {
        "vso.taskgroups_write",
        ScopeResources.ReadAndCreate()
      },
      {
        "vso.taskgroups_manage",
        ScopeResources.ReadCreateAndManage()
      },
      {
        "vso.dashboards",
        ScopeResources.Read()
      },
      {
        "vso.dashboards_manage",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.test",
        ScopeResources.Read()
      },
      {
        "vso.test_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.profile",
        ScopeResources.Read()
      },
      {
        "vso.profile_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.variablegroups_read",
        ScopeResources.Read()
      },
      {
        "vso.variablegroups_write",
        ScopeResources.ReadAndCreate()
      },
      {
        "vso.variablegroups_manage",
        ScopeResources.ReadCreateAndManage()
      },
      {
        "vso.wiki",
        ScopeResources.Read()
      },
      {
        "vso.wiki_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.workitemsearch",
        ScopeResources.Read()
      },
      {
        "vso.work",
        ScopeResources.Read()
      },
      {
        "vso.work_write",
        ScopeResources.ReadAndWrite()
      },
      {
        "vso.work_full",
        ScopeResources.ReadWriteAndManage()
      },
      {
        "vso.tokenadministration",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.tokens",
        ScopeResources.ReadAndManage()
      },
      {
        "vso.msdnsubadmin",
        ScopeResources.Full()
      },
      {
        "vso.subscriptions",
        ScopeResources.Full()
      }
    };

    private static AuthorizationScopeDefinitions CreateDefault()
    {
      AuthorizationScopeDefinitions scopeDefinitions = new AuthorizationScopeDefinitions();
      scopeDefinitions.scopes = new AuthorizationScopeDefinition[141]
      {
        new AuthorizationScopeDefinition()
        {
          scope = "preview_api_all",
          title = "Full access to all resources",
          description = "Provides this application the ability to act on your behalf with full access (read and write) to work items, source code, builds, and other resources in all Azure DevOps Services accounts you can access.",
          availability = AuthorizationScopeAvailability.Deprecated,
          patterns = new string[209]
          {
            "/_apis#OPTIONS",
            "/DefaultCollection/_apis#OPTIONS",
            "/_apis/connectiondata#GET",
            "/DefaultCollection/_apis/connectiondata#GET",
            "/_apis/ServiceDefinitions#GET",
            "/_apis/build/builds#GET+PATCH+DELETE",
            "/_apis/build/qualities#GET+PUT+DELETE",
            "/_apis/build/requests#GET+POST+PATCH+DELETE",
            "/_apis/build/definitions",
            "/_apis/build/queues",
            "/DefaultCollection/_apis/build/builds#GET+PATCH+DELETE",
            "/DefaultCollection/_apis/build/qualities#GET+PUT+DELETE",
            "/DefaultCollection/_apis/build/requests#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/build/definitions",
            "/DefaultCollection/_apis/build/queues",
            "/DefaultCollection/_apis/resources/Containers#GET",
            "/DefaultCollection/*/_apis/build/builds#GET+PATCH+DELETE+PUT+POST",
            "/DefaultCollection/*/_apis/build/folders#GET+POST+PUT+DELETE",
            "/DefaultCollection/*/_apis/build/definitions#GET+POST+PUT+DELETE+PATCH",
            "/DefaultCollection/*/_apis/build/requests#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/build/qualities#GET+PUT+DELETE",
            "/DefaultCollection/*/_apis/build/tags#GET",
            "/DefaultCollection/*/_apis/build/options#GET",
            "/DefaultCollection/_apis/build/queues#GET",
            "/DefaultCollection/_apis/build/options#GET",
            "/DefaultCollection/_apis/build/controllers#GET",
            "/_apis/accounts",
            "/_apis/profile/profiles",
            "/_apis/projectCollections",
            "/_apis/tagging#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/projects#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/tagging#GET+POST+PATCH+DELETE",
            "/_apis/notifications/*/eventdefinitions",
            "/_apis/hooks/consumers",
            "/_apis/hooks/publishers",
            "/_apis/hooks/subscriptions#GET+POST+PUT+DELETE",
            "/_apis/hooks/inputValuesQuery#POST",
            "/_apis/hooks/notificationsQuery#POST",
            "/_apis/hooks/subscriptionsQuery#POST",
            "/_apis/hooks/publishersQuery#POST",
            "/DefaultCollection/_apis/hooks/consumers",
            "/DefaultCollection/_apis/hooks/publishers",
            "/DefaultCollection/_apis/hooks/subscriptions#GET+POST+PUT+DELETE",
            "/DefaultCollection/_apis/hooks/inputValuesQuery#POST",
            "/DefaultCollection/_apis/hooks/notificationsQuery#POST",
            "/DefaultCollection/_apis/hooks/subscriptionsQuery#POST",
            "/DefaultCollection/_apis/hooks/publishersQuery#POST",
            "/_apis/chat/rooms/*/messages#GET+POST+PUT+PATCH+DELETE",
            "/_apis/chat/rooms#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/chat/rooms/*/messages#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/chat/rooms#GET+POST+PUT+PATCH+DELETE",
            "/_apis/tfvc/branches",
            "/_apis/tfvc/changesets",
            "/_apis/tfvc/labels",
            "/_apis/tfvc/shelvesets",
            "/_apis/tfvc/workspaces#GET+POST",
            "/_apis/tfvc/changesetsBatch#POST",
            "/_apis/tfvc/itemBatch#POST",
            "/DefaultCollection/_apis/tfvc/branches",
            "/DefaultCollection/_apis/tfvc/changesets",
            "/DefaultCollection/_apis/tfvc/items",
            "/DefaultCollection/_apis/tfvc/labels",
            "/DefaultCollection/_apis/tfvc/shelvesets",
            "/DefaultCollection/_apis/tfvc/workspaces#GET+POST",
            "/DefaultCollection/_apis/tfvc/changesetsBatch#POST",
            "/DefaultCollection/_apis/tfvc/itemBatch#POST",
            "/DefaultCollection/*/_apis/tfvc#GET+POST",
            "/_apis/git/repositories#GET+POST",
            "/_apis/git/repositories/*/commits",
            "/_apis/git/*/repositories/*/commits",
            "/_apis/git/repositories/*/commits/*/statuses#GET+POST",
            "/_apis/git/*/repositories/*/commits/*/statuses#GET+POST",
            "/_apis/git/repositories/*/forks/*#GET",
            "/_apis/git/*/repositories/*/forks/*#GET",
            "/_apis/git/repositories/*/forkSyncRequests#GET+POST",
            "/_apis/git/repositories/*/forkSyncRequests/*#GET",
            "/_apis/git/*/repositories/*/forkSyncRequests#GET+POST",
            "/_apis/git/*/repositories/*/forkSyncRequests/*#GET",
            "/_apis/git/repositories/*/pushes#GET+POST",
            "/_apis/git/*/repositories/*/pushes#GET+POST",
            "/_apis/git/repositories/*/pushes/*",
            "/_apis/git/*/repositories/*/pushes/*",
            "/_apis/git/repositories/*#GET+PATCH+DELETE",
            "/_apis/git/*/repositories/*#GET+PATCH+DELETE",
            "/_apis/git/*/repositories#GET+POST",
            "/DefaultCollection/_apis/git/repositories#GET+POST",
            "/DefaultCollection/_apis/git/repositories/*/commits",
            "/DefaultCollection/_apis/git/*/repositories/*/commits",
            "/DefaultCollection/_apis/git/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/_apis/git/*/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/_apis/git/repositories/*/forks/*#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/forks/*#GET",
            "/DefaultCollection/_apis/git/repositories/*/forkSyncRequests#GET+POST",
            "/DefaultCollection/_apis/git/repositories/*/forkSyncRequests/*#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/forkSyncRequests#GET+POST",
            "/DefaultCollection/_apis/git/*/repositories/*/forkSyncRequests/*#GET",
            "/DefaultCollection/_apis/git/repositories/*/pushes#GET+POST",
            "/DefaultCollection/_apis/git/*/repositories/*/pushes#GET+POST",
            "/DefaultCollection/_apis/git/repositories/*/pushes/*",
            "/DefaultCollection/_apis/git/*/repositories/*/pushes/*",
            "/DefaultCollection/_apis/git/repositories/*#GET+PATCH+DELETE+POST",
            "/DefaultCollection/_apis/git/*/repositories/*#GET+PATCH+DELETE+POST",
            "/DefaultCollection/_apis/git/*/repositories#GET+POST",
            "/DefaultCollection/_apis/git/repositories/*/pullrequests#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/git/*/repositories/*/pullrequests#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/repositories#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/codereview/reviews#GET",
            "/DefaultCollection/*/_apis/codereview/reviews#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/codereview/reviewsbatch#POST",
            "/DefaultCollection/*/_apis/codereview/settings#GET+POST+PUT",
            "/DefaultCollection/_apis/visits/artifactVisits#PUT",
            "/DefaultCollection/_apis/visits/artifactVisitsBatch#POST",
            "/DefaultCollection/_apis/visits/artifactStatsBatch#POST",
            "/DefaultCollection/*/_apis/policy/Evaluations#GET+PATCH",
            "/DefaultCollection/*/_apis/policy/configurations",
            "/DefaultCollection/*/_apis/policy/configurations/*/revisions",
            "/DefaultCollection/*/_apis/policy/types",
            "/DefaultCollection/*/_apis/git/policy/configurations",
            "/_apis/wit/attachments#GET+POST+PUT",
            "/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/_apis/wit/tempQueries#POST",
            "/DefaultCollection/_apis/wit/attachments#GET+POST+PUT",
            "/DefaultCollection/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/tempQueries#POST",
            "/DefaultCollection/*/*/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/_apis/wit/fields#GET+POST",
            "/_apis/wit/sendMail#POST",
            "/_apis/wit/wiql#GET+POST",
            "/_apis/wit/workitemrelationtypes#GET",
            "/_apis/wit/workitems#GET+POST+PATCH",
            "/_apis/wit/workitemtypecategories#GET",
            "/_apis/wit/workitemtypes#GET",
            "/_apis/wit/$ruleEngine#POST",
            "/_apis/wit/$batch#POST+PATCH",
            "/_apis/wit/artifactlinktypes#GET",
            "/_apis/wit/artifacturiquery#POST",
            "/DefaultCollection/_apis/wit/artifactlinktypes#GET",
            "/DefaultCollection/_apis/wit/artifacturiquery#POST",
            "/DefaultCollection/_apis/wit/fields#GET+POST",
            "/DefaultCollection/_apis/wit/sendMail#POST",
            "/DefaultCollection/_apis/wit/wiql#GET+POST",
            "/DefaultCollection/_apis/wit/workitemrelationtypes#GET",
            "/DefaultCollection/_apis/wit/workitems#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/$ruleEngine#POST",
            "/DefaultCollection/_apis/wit/$batch#POST+PATCH",
            "/DefaultCollection/_apis/wit/workitemtypetemplate#GET+POST",
            "/DefaultCollection/_apis/work/processes#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/work/processdefinitions#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/workItemsdelete#POST",
            "/DefaultCollection/*/_apis/wit/fields#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/classificationnodes#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/tempQueries#POST",
            "/DefaultCollection/*/_apis/wit/sendMail#POST",
            "/DefaultCollection/*/_apis/wit/wiql#GET+POST",
            "/DefaultCollection/*/_apis/wit/workitems#GET+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/workitems#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/workitemtypecategories#GET",
            "/DefaultCollection/*/_apis/wit/workitemtypes#GET",
            "/DefaultCollection/*/_apis/wit/workitemtypetemplate#GET+POST",
            "/DefaultCollection/*/*/_apis/wit/templates#GET+PUT+POST+DELETE",
            "/DefaultCollection/*/_apis/wit/projectProcessMigration#POST",
            "/DefaultCollection/*/_apis/work/predefinedQueries#GET",
            "/DefaultCollection/*/_apis/wit/workItemsdelete#POST",
            "/DefaultCollection/*/_apis/wit/workItemFieldAllowedValues#GET",
            "/DefaultCollection/*/*/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/*/_apis/wit/tempQueries#POST",
            "/DefaultCollection/*/*/_apis/wit/wiql#HEAD+GET+POST",
            "/DefaultCollection/_apis/resources/Containers/*",
            "/_apis/resources/Containers/*",
            "/_api/_wit/teamProjects",
            "/DefaultCollection/_api/_wit/teamProjects",
            "/DefaultCollection/*/_apis/work/boards#GET+PUT+PATCH",
            "/DefaultCollection/*/*/_apis/work/boards#GET+PUT+PATCH",
            "/DefaultCollection/*/*/_apis/work/iterations#GET+PATCH",
            "/DefaultCollection/*/_apis/work/teamsettings#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/*/*/_apis/work/teamsettings#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/*/_apis/work/processconfiguration#GET",
            "/DefaultCollection/*/_apis/work/backlogconfiguration#GET",
            "/DefaultCollection/*/*/_apis/work/backlogconfiguration#GET",
            "/DefaultCollection/*/*/_apis/work/backlogs#GET",
            "/DefaultCollection/*/*/_apis/work/workitemsorder#PATCH",
            "/DefaultCollection/*/*/_apis/work/taskboardColumns#GET+PUT",
            "/DefaultCollection/*/*/_apis/work/taskboardWorkItems#GET+PATCH",
            "/DefaultCollection/*/_apis/boards/boards#GET+POST+PATCH+DELETE",
            "/_apis/clt/testdrops#GET+POST",
            "/_apis/clt/testruns#GET+POST+PATCH",
            "/_apis/clt/testruns/*/errors",
            "/_apis/clt/testruns/*/messages",
            "/_apis/clt/testruns/*/results",
            "/_apis/clt/testruns/*/counterinstances",
            "/_apis/clt/testruns/*/countersamples",
            "/_apis/clt/apm",
            "/_apis/clt/configuration",
            "/_apis/clt/agentgroups#GET+POST+PATCH+DELETE",
            "/_apis/clt/agentgroups/*/agents#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/test#GET+POST+PATCH+DELETE",
            "/_apis/test/suites",
            "/_apis/Identities#GET+PUT+DELETE",
            "/_apis/IdentityBatch#POST",
            "/_apis/Groups#GET+DELETE+POST",
            "/_apis/Scopes#GET+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/discussion/threads#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/discussion/threadsBatch#POST",
            "/DefaultCollection/_apis/discussion/comments#PATCH",
            "/_apis/Commerce/*#GET",
            "/_apis/Commerce/Subscription/*/*#GET+PUT",
            "/_apis/gallery/*#GET",
            "/_apis/gallery/acquisitionrequests/*#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.base",
          title = "Base",
          availability = AuthorizationScopeAvailability.None,
          patterns = new string[24]
          {
            "/_apis#OPTIONS",
            "/DefaultCollection/_apis#OPTIONS",
            "/_apis/connectiondata#GET",
            "/_apis/servicelevel#GET",
            "/DefaultCollection/_apis/connectiondata#GET",
            "/_apis/ServiceDefinitions#GET",
            "/_apis/resourceareas#GET",
            "/DefaultCollection/_apis/resourceareas#GET",
            "/_apis/operations#GET",
            "/DefaultCollection/_apis/operations#GET",
            "/_apis/permissions#GET",
            "/_apis/SecurityNamespaces#GET",
            "/_apis/AccessControlLists#GET",
            "/_apis/security/permissionEvaluationBatch#POST",
            "/DefaultCollection/_apis/permissions#GET",
            "/DefaultCollection/_apis/SecurityNamespaces#GET",
            "/DefaultCollection/_apis/AccessControlLists#GET",
            "/DefaultCollection/_apis/security/permissionEvaluationBatch#POST",
            "/_apis/securityroles/scopes/*/roledefinitions#GET",
            "/_apis/securityroles/scopes/*/roleassignments/resources/*#GET",
            "/DefaultCollection/_apis/securityroles/scopes/*/roledefinitions#GET",
            "/DefaultCollection/_apis/securityroles/scopes/*/roleassignments/resources/*#GET",
            "/_apis/CustomerIntelligence#POST",
            "/_apis/FeatureFlags/*#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.profile",
          title = "User profile (read)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read your profile, accounts, collections, projects, teams, and other top-level organizational artifacts.",
          inheritsFrom = "vso.base",
          patterns = new string[19]
          {
            "/_apis/account#GET",
            "/_apis/account/regions#GET",
            "/_apis/accounts#GET",
            "/_apis/projectCollections#GET",
            "/_apis/profile/profiles#GET",
            "/_apis/profile/UserDefaults#GET",
            "/_apis/profile/regions#GET",
            "/_apis/profile/georegion#GET",
            "/_apis/profile/Locations#GET",
            "/_apis/profile/Settings#GET",
            "/_apis/profile/Attributes#GET+PATCH",
            "/_apis/ClientNotification/Subscriptions#GET",
            "/_apis/process/processes#GET",
            "/DefaultCollection/_apis/projects#GET",
            "/DefaultCollection/_apis#OPTIONS",
            "/_apis/Organization/Regions#GET",
            "/_apis/Organization/Organizations#GET",
            "/DefaultCollection/_apis/Organization/Collections#GET",
            "/DefaultCollection/_apis/Favorite/Favorites#GET"
          },
          groupingKey = "userProfile"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.profile_write",
          title = "User profile (write)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to write to your profile.",
          inheritsFrom = "vso.profile",
          patterns = new string[2]
          {
            "/_apis/profile/profiles#PATCH+PUT",
            "/DefaultCollection/_apis/Favorite/Favorites#POST+DELETE"
          },
          groupingKey = "userProfile"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.acquisition_write",
          title = "User acquisition workflow",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants the ability to update profile, create accounts",
          inheritsFrom = "vso.profile",
          patterns = new string[31]
          {
            "/*/_apis/ServiceDefinitions#GET",
            "/*/_apis/projectCollections#GET",
            "/*/_apis/projects#GET+POST",
            "/*/_apis/operations#GET",
            "/_apis/ServiceDefinitions#GET",
            "/_apis/projects#GET+POST",
            "/_apis/operations#GET",
            "/_apis/profile/avatar#GET+POST+DELETE+PUT",
            "/_apis/profile/profiles#GET+POST+PATCH",
            "/_apis/profile/UserDefaults#GET+PUT",
            "/_apis/accounts#GET+POST",
            "/api/account#GET+POST",
            "/_apis/delegatedauth/registration#GET",
            "/_apis/delegatedauth/registrationsecret#GET",
            "/_apis/delegatedauth/authorizations#GET",
            "/_apis/UserMapping/UserAccountMappings#GET",
            "/_apis/user/attributes#GET+PATCH+DELETE",
            "/_apis/user/avatar#GET+PUT+DELETE",
            "/_apis/user/avatarPreview#POST",
            "/_apis/user/contactWithOffers#GET+PUT",
            "/_apis/user/userDefaults#GET",
            "/_apis/user/user#GET+POST+PATCH",
            "/_apis/UserEntitlements/*#DELETE",
            "/_apis/ServicePrincipalEntitlements/*#DELETE",
            "/_apis/MemberEntitlements/*#DELETE",
            "/_apis/useraad/aadMembership#GET",
            "/_apis/Pipelines/Connections#POST",
            "/_apis/Contribution/dataProviders/query#POST",
            "/_apis/Contribution/nodes/query#POST",
            "/_apis/OrganizationPolicy/Policies#GET+PATCH",
            "/_apis/HostAcquisition/Regions#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.identity",
          title = "Identity (read)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read identities and groups.",
          inheritsFrom = "vso.base",
          patterns = new string[20]
          {
            "/*/_apis/identities#GET",
            "/*/_apis/groups#GET",
            "/*/_apis/scopes#GET",
            "/*/_apis/identitybatch#POST",
            "/_apis/identities#GET",
            "/_apis/groups#GET",
            "/_apis/scopes#GET",
            "/_apis/identitybatch#POST",
            "/DefaultCollection/_apis/identities#GET",
            "/DefaultCollection/_apis/groups#GET",
            "/DefaultCollection/_apis/scopes#GET",
            "/DefaultCollection/_apis/identitybatch#POST",
            "/DefaultCollection/_api/_identity/ReadGroupMembers#GET",
            "/DefaultCollection/*/_api/_identity/ReadGroupMembers#GET",
            "/DefaultCollection/_api/_identity/ReadScopedApplicationGroupsJson#GET",
            "/DefaultCollection/_api/*/_identity/ReadScopedApplicationGroupsJson#GET",
            "/DefaultCollection/_api/_security/ReadExplicitIdentitiesJson#GET",
            "/DefaultCollection/_api/*/_security/ReadExplicitIdentitiesJson#GET",
            "/DefaultCollection/_api/_common/identityImage#GET",
            "/DefaultCollection/_api/*/_common/identityImage#GET"
          },
          groupingKey = "identity"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.identity_manage",
          title = "Identity (manage)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read, write, and manage identities and groups.",
          inheritsFrom = "vso.identity",
          patterns = ((IEnumerable<string>) AuthorizationScopeDefinitions.ClientOmBasePatterns).Concat<string>((IEnumerable<string>) new string[14]
          {
            "/_apis/identities#GET+POST+PUT+DELETE",
            "/_apis/groups#GET+POST+PUT+DELETE",
            "/_apis/scopes#GET+POST+PUT+DELETE+PATCH",
            "/DefaultCollection/_apis/identities#GET+POST+PUT+DELETE",
            "/DefaultCollection/_apis/groups#GET+POST+PUT+DELETE",
            "/DefaultCollection/_apis/scopes#GET+POST+PUT+DELETE+PATCH",
            "/Services/*/IdentityManagementService.asmx#POST",
            "/*/Services/*/IdentityManagementService.asmx#POST",
            "/Services/*/IdentityManagementService2.asmx#POST",
            "/*/Services/*/IdentityManagementService2.asmx#POST",
            "/DefaultCollection/_api/_identity/ManageGroup#GET+POST",
            "/DefaultCollection/*/_api/_identity/ManageGroup#GET+POST",
            "/DefaultCollection/_api/_identity/AddIdentities#POST",
            "/DefaultCollection/*/_api/_identity/AddIdentities#POST"
          }).ToArray<string>(),
          groupingKey = "identity"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.hooks",
          title = "Service hooks (read)",
          description = "Grants the ability to read service hook subscriptions and metadata, including supported events, consumers, and actions.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.profile",
          publicScopeLookupFromPrivateScope = true,
          patterns = new string[16]
          {
            "/_apis/hooks/consumers#GET",
            "/_apis/hooks/inputValuesQuery#POST",
            "/_apis/hooks/notificationsQuery#POST",
            "/_apis/hooks/publishers#GET",
            "/_apis/hooks/publishersQuery#POST",
            "/_apis/hooks/subscriptions#GET",
            "/_apis/hooks/subscriptionsQuery#POST",
            "/_apis/hooks/testNotifications#POST",
            "/DefaultCollection/_apis/hooks/consumers#GET",
            "/DefaultCollection/_apis/hooks/inputValuesQuery#POST",
            "/DefaultCollection/_apis/hooks/notificationsQuery#POST",
            "/DefaultCollection/_apis/hooks/publishers#GET",
            "/DefaultCollection/_apis/hooks/publishersQuery#POST",
            "/DefaultCollection/_apis/hooks/subscriptions#GET",
            "/DefaultCollection/_apis/hooks/subscriptionsQuery#POST",
            "/DefaultCollection/_apis/hooks/testNotifications#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.hooks_write",
          title = "Service hooks (read and write)",
          description = "Grants the ability to create and update service hook subscriptions and read metadata, including supported events, consumers, and actions.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.hooks",
          publicScopeLookupFromPrivateScope = true,
          patterns = new string[2]
          {
            "/_apis/hooks/subscriptions#GET+POST+PUT+DELETE",
            "/DefaultCollection/_apis/hooks/subscriptions#GET+POST+PUT+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.hooks_interact",
          title = "Service hooks (interact)",
          description = "Grants the ability to interact and perform actions on events received via service hooks.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.profile",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/wit/workitems#PATCH",
            "/DefaultCollection/*/_apis/wit/workitemtypes/*/states#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.work",
          title = "Work items (read)",
          description = "Grants the ability to read work items, queries, boards, area and iterations paths, and other work item tracking related metadata. Also grants the ability to execute queries, search work items and to receive notifications about work item events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.hooks_write",
          patterns = new string[84]
          {
            "/_apis/tagging#GET",
            "/DefaultCollection/_apis/tagging#GET",
            "/DefaultCollection/_apis/wit/attachments#GET",
            "/DefaultCollection/_apis/wit/fields#GET",
            "/DefaultCollection/_apis/wit/workitemrelationtypes#GET",
            "/DefaultCollection/_apis/wit/queries#GET",
            "/DefaultCollection/_apis/wit/sendMail#POST",
            "/DefaultCollection/*/_apis/wit/sendMail#POST",
            "/DefaultCollection/_apis/wit/wiql#GET+POST",
            "/DefaultCollection/*/_apis/wit/wiql#GET+POST",
            "/DefaultCollection/*/*/_apis/wit/wiql#HEAD+GET+POST",
            "/DefaultCollection/_apis/wit/workitemsbatch#POST",
            "/DefaultCollection/*/_apis/wit/workitemsbatch#POST",
            "/DefaultCollection/_apis/wit/workitems#GET",
            "/DefaultCollection/*/_apis/wit/attachments#GET",
            "/DefaultCollection/*/_apis/wit/revisions#GET",
            "/DefaultCollection/*/_apis/wit/fields#GET",
            "/DefaultCollection/*/_apis/wit/classificationnodes#GET",
            "/DefaultCollection/*/_apis/wit/queries#GET",
            "/DefaultCollection/*/_apis/wit/queriesbatch#POST",
            "/DefaultCollection/*/_apis/wit/workitemtypecategories#GET",
            "/DefaultCollection/*/_apis/wit/workitemtypes#GET",
            "/DefaultCollection/*/_apis/wit/workitemtypetemplate#GET",
            "/DefaultCollection/*/_apis/wit/workitems#GET",
            "/DefaultCollection/*/*/_apis/wit/queries#GET",
            "/DefaultCollection/*/*/_apis/wit/templates#GET",
            "/DefaultCollection/_apis/wit/workitemicons#GET",
            "/DefaultCollection/_apis/wit/recyclebin#GET",
            "/DefaultCollection/*/_apis/wit/recyclebin#GET",
            "/DefaultCollection/_apis/wit/workitemtransitions#GET",
            "/DefaultCollection/_apis/wit/workitemtypetemplate#GET",
            "/DefaultCollection/_apis/wit/$ruleEngine#POST",
            "/DefaultCollection/*/_apis/work/boards#GET",
            "/DefaultCollection/*/*/_apis/work/boards#GET",
            "/DefaultCollection/*/_apis/work/iterations#GET",
            "/DefaultCollection/*/*/_apis/work/iterations#GET",
            "/DefaultCollection/*/_apis/work/teamsettings#GET",
            "/DefaultCollection/*/*/_apis/work/teamsettings#GET",
            "/DefaultCollection/*/_apis/work/processconfiguration#GET",
            "/DefaultCollection/*/_apis/work/backlogconfiguration#GET",
            "/DefaultCollection/*/*/_apis/work/backlogconfiguration#GET",
            "/DefaultCollection/*/*/_apis/work/backlogs#GET",
            "/DefaultCollection/_apis/work/boardcolumns#GET",
            "/DefaultCollection/*/_apis/work/boardcolumns#GET",
            "/DefaultCollection/_apis/work/boardrows#GET",
            "/DefaultCollection/*/_apis/work/boardrows#GET",
            "/DefaultCollection/_apis/work/accountMyWorkRecentActivity#GET",
            "/DefaultCollection/_apis/work/accountMyWork#GET",
            "/DefaultCollection/*/_apis/work/predefinedQueries#GET",
            "/DefaultCollection/*/_apis/boards/boards#GET",
            "/DefaultCollection/*/*/_apis/work/taskboardColumns#GET",
            "/DefaultCollection/*/*/_apis/work/taskboardWorkItems#GET",
            "/DefaultCollection/*/*/_apis/work/taskboard/cardrulesettings#GET",
            "/DefaultCollection/*/*/_apis/work/taskboard/cardsettings#GET",
            "/DefaultCollection/_apis/wit/reporting/workItemRevisions#GET+POST",
            "/DefaultCollection/*/_apis/wit/reporting/workItemRevisions#GET+POST",
            "/DefaultCollection/_apis/wit/reporting/workItemLinks#GET",
            "/DefaultCollection/*/_apis/wit/reporting/workItemLinks#GET",
            "/DefaultCollection/*/_apis/wit/reporting/workItemComments#GET",
            "/DefaultCollection/_apis/process/processes#GET",
            "/DefaultCollection/_apis/wit/artifactlinktypes#GET",
            "/DefaultCollection/_apis/wit/artifacturiquery#POST",
            "/DefaultCollection/*/_apis/wit/artifacturiquery#POST",
            "/DefaultCollection/_apis/work/processes#GET",
            "/DefaultCollection/_apis/work/processdefinitions#GET",
            "/DefaultCollection/_apis/work/processdefinitions/*/behaviors#GET",
            "/_apis/search/workItemSearchResults#POST",
            "/DefaultCollection/_apis/search/workItemSearchResults#POST",
            "/DefaultCollection/*/_apis/search/workItemSearchResults#POST",
            "/DefaultCollection/*/_apis/wit/tags#GET",
            "/DefaultCollection/*/_apis/build/*/workitems#GET",
            "/DefaultCollection/*/_apis/build/workitems#GET",
            "/v2/_odata/*#GET",
            "/v2/_odata/*/*#GET",
            "/_apis/compliance/requirement#GET",
            "/_apis/compliance/workItemConfig#GET",
            "/_apis/compliance/requirementsUIConfig#GET",
            "/DefaultCollection/*/_apis/work/plans#GET",
            "/DefaultCollection/_apis/work/processadmin#GET",
            "/DefaultCollection/_api/_wit/query#GET",
            "/DefaultCollection/*/*/_api/_teamChart/Burndown#GET",
            "/DefaultCollection/_api/_process/GetProcesses#GET",
            "/DefaultCollection/_api/_process/GetJobProgress#GET",
            "/DefaultCollection/_apis/properties#GET"
          },
          groupingKey = "workItems"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.work_write",
          title = "Work items (read and write)",
          description = "Grants the ability to read, create, and update work items and queries, update board metadata, read area and iterations paths other work item tracking related metadata, execute queries, and to receive notifications about work item events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.work",
          patterns = new string[49]
          {
            "/_apis/tagging#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/attachments#GET+POST+PUT",
            "/DefaultCollection/*/_apis/wit/attachments#GET+POST+PUT",
            "/DefaultCollection/*/_apis/wit/classificationnodes#GET+POST+DELETE+PATCH",
            "/DefaultCollection/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/*/_apis/wit/queries#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/tempQueries#POST",
            "/DefaultCollection/*/_apis/wit/tempQueries#POST",
            "/DefaultCollection/*/*/_apis/wit/tempQueries#POST",
            "/DefaultCollection/_apis/wit/workitems#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/recyclebin#GET+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/workItemsdelete#POST",
            "/DefaultCollection/*/_apis/wit/workItemsdelete#POST",
            "/DefaultCollection/*/_apis/wit/workItemFieldAllowedValues#GET",
            "/DefaultCollection/_apis/tagging#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/tagging#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/work/boards#GET+PUT+PATCH",
            "/DefaultCollection/*/*/_apis/work/boards#GET+PUT+PATCH",
            "/DefaultCollection/*/_apis/boards/boards#GET+PATCH+POST+DELETE",
            "/DefaultCollection/*/_apis/wit/workitems#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/recyclebin#GET+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/$batch#POST+PATCH",
            "/DefaultCollection/*/_apis/work/teamsettings#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/*/*/_apis/work/teamsettings#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/*/_apis/wit/workitemtypetemplate#GET+POST",
            "/DefaultCollection/_apis/wit/workitemtypetemplate#GET+POST",
            "/DefaultCollection/*/*/_apis/wit/templates#GET+PUT+POST+DELETE",
            "/DefaultCollection/_apis/work/processes#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/work/processdefinitions#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/wit/fields#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/fields#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/*/_apis/work/iterations#GET+PATCH",
            "/DefaultCollection/*/*/_apis/work/workitemsorder#PATCH",
            "/DefaultCollection/*/*/_apis/work/taskboardColumns#GET+PUT",
            "/DefaultCollection/*/*/_apis/work/taskboardWorkItems#GET+PATCH",
            "/DefaultCollection/*/*/_apis/work/taskboard/cardrulesettings#GET+PATCH",
            "/DefaultCollection/*/*/_apis/work/taskboard/cardsettings#GET+PUT",
            "/DefaultCollection/*/_apis/wit/tags#GET+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wit/projectProcessMigration#POST",
            "/DefaultCollection/*/_apis/work/plans#GET+POST+PUT+DELETE",
            "/DefaultCollection/*/*/_apis/work/automationrules#PATCH",
            "/DefaultCollection/_apis/work/processadmin#GET+POST",
            "/v2/_odata/*/*#GET+POST+PATCH+DELETE+PUT",
            "/_apis/compliance/requirement#GET+POST+PATCH+DELETE+PUT",
            "/_apis/compliance/workItemConfig#GET+POST+PATCH+DELETE+PUT",
            "/_apis/compliance/requirementsUIConfig#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/Core/_workitems#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/properties#GET+POST+PUT"
          },
          groupingKey = "workItems"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.work_full",
          title = "Work items (full)",
          description = "Grants full access to work items, queries, backlogs, plans, and work item tracking metadata. Also provides the ability to receive notifications about work item events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.work_write",
          patterns = ((IEnumerable<string>) AuthorizationScopeDefinitions.ClientOmBasePatterns).Concat<string>((IEnumerable<string>) new string[14]
          {
            "/WorkItemTracking/*/ClientService.asmx#POST",
            "/WorkItemTracking/*/ConfigurationSettingsService.asmx#POST",
            "/WorkItemTracking/*/Integration.asmx#POST",
            "/*/WorkItemTracking/*/ClientService.asmx#POST",
            "/*/WorkItemTracking/*/ConfigurationSettingsService.asmx#POST",
            "/*/WorkItemTracking/*/Integration.asmx#POST",
            "/Services/*/ProcessTemplate.asmx#POST",
            "/*/Services/*/ProcessTemplate.asmx#POST",
            "/Services/*/TeamConfigurationService.asmx#POST",
            "/*/Services/*/TeamConfigurationService.asmx#POST",
            "/WorkItemTracking/*/AttachFileHandler.ashx#GET",
            "/*/WorkItemTracking/*/AttachFileHandler.ashx#GET",
            "/WorkItemTracking/*/AttachFileHandler.ashx#POST",
            "/*/WorkItemTracking/*/AttachFileHandler.ashx#POST"
          }).ToArray<string>(),
          groupingKey = "workItems"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.build",
          title = "Build (read)",
          description = "Grants the ability to access build artifacts, including build results, definitions, and requests, and the ability to receive notifications about build events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.hooks_write",
          patterns = new string[54]
          {
            "/DefaultCollection/_apis/resources/Containers#GET",
            "/DefaultCollection/*/_apis/build/builds#GET",
            "/DefaultCollection/*/_apis/build/changes#GET",
            "/DefaultCollection/*/_apis/build/folders#GET",
            "/DefaultCollection/*/_apis/build/definitions#GET",
            "/DefaultCollection/*/_apis/build/metrics#GET",
            "/DefaultCollection/*/_apis/build/requests#GET",
            "/DefaultCollection/*/_apis/build/qualities#GET",
            "/DefaultCollection/*/_apis/build/tags#GET",
            "/DefaultCollection/*/_apis/build/options#GET",
            "/DefaultCollection/*/_apis/build/repos#GET",
            "/DefaultCollection/*/_apis/build/settings#GET",
            "/DefaultCollection/*/_apis/build/latest#GET",
            "/DefaultCollection/*/_apis/build/authorizedresources#GET",
            "/DefaultCollection/*/_apis/build/definitions/*/resources#GET",
            "/DefaultCollection/*/_apis/build/workitems#GET",
            "/DefaultCollection/*/_apis/build/status#GET",
            "/DefaultCollection/*/_apis/build/*/workitems#GET",
            "/DefaultCollection/_apis/build/builds#GET",
            "/DefaultCollection/_apis/build/queues#GET",
            "/DefaultCollection/_apis/build/options#GET",
            "/DefaultCollection/_apis/build/controllers#GET",
            "/DefaultCollection/*/_apis/distributedtask/hubs/build/plans#GET",
            "/DefaultCollection/_apis/public/build/definitions#GET",
            "/DefaultCollection/_apis/build/resourceusage#GET",
            "/DefaultCollection/*/_apis/build/retention#GET",
            "/DefaultCollection/_apis/build/retention/history#GET",
            "/DefaultCollection/*/_apis/build/retention/leases#GET",
            "/_apis/clienttools/*/release#GET",
            "/_apis/dedup/chunks/*#GET",
            "/_apis/dedup/nodes/*#GET",
            "/_apis/dedup/urls/*#POST",
            "/_apis/artifact/*/content#GET",
            "/*/_apis/clienttools/*/release#GET",
            "/*/_apis/dedup/chunks/*#GET",
            "/*/_apis/dedup/nodes/*#GET",
            "/*/_apis/dedup/urls/*#POST",
            "/*/_apis/artifact/*/content#GET",
            "/_apis/clienttools/*/settings#GET",
            "/_apis/pipelines#GET",
            "/*/_apis/pipelines#GET",
            "/_apis/pipelines/*/preview#POST",
            "/*/_apis/pipelines/*/preview#POST",
            "/DefaultCollection/*/_apis/pipelines#GET",
            "/DefaultCollection/*/_apis/pipelines/*/preview#POST",
            "/DefaultCollection/*/_apis/pipelines/*/runs#GET",
            "/DefaultCollection/*/_apis/pipelines/approvals#GET",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions#GET",
            "/DefaultCollection/*/_apis/pipelines/checks/configurations#GET",
            "/DefaultCollection/*/_apis/pipelines/checks/queryconfigurations#POST",
            "/DefaultCollection/*/_apis/pipelines/checks/runs#GET",
            "/DefaultCollection/*/_apis/sourceProviders#GET",
            "/Build/*/BuildService.asmx#POST",
            "/*/Build/*/BuildService.asmx#POST"
          },
          groupingKey = "build"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.build_execute",
          title = "Build (read and execute)",
          description = "Grants the ability to access build artifacts, including build results, definitions, and requests, and the ability to queue a build, update build properties, and the ability to receive notifications about build events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.build",
          patterns = new string[30]
          {
            "/DefaultCollection/*/_apis/build/builds#GET+PATCH+DELETE+PUT+POST",
            "/DefaultCollection/*/_apis/build/requests#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/build/qualities#GET+PUT+DELETE",
            "/DefaultCollection/*/_apis/build/folders#GET+POST+PUT+DELETE",
            "/DefaultCollection/*/_apis/build/definitions#GET+POST+PUT+DELETE+PATCH",
            "/DefaultCollection/*/_apis/build/authorizedresources#GET+PATCH",
            "/DefaultCollection/*/_apis/build/definitions/*/resources#GET+PATCH",
            "/DefaultCollection/_apis/build/builds#GET+PATCH+DELETE+PUT+POST",
            "/DefaultCollection/_apis/build/queues#GET+POST+DELETE",
            "/DefaultCollection/*/_apis/distributedtask/hubs/build/plans#GET+PUT+POST+PATCH",
            "/DefaultCollection/*/_apis/build/retention#GET+PATCH",
            "/DefaultCollection/*/_apis/build/retention/leases#GET+POST+DELETE",
            "/DefaultCollection/*/_apis/build/settings#GET+PATCH",
            "/DefaultCollection/*/_apis/build/tags#GET+DELETE",
            "/_apis/dedup/chunks/*#PUT+POST",
            "/_apis/dedup/nodes/*#PUT+POST",
            "/*/_apis/dedup/chunks/*#PUT+POST",
            "/*/_apis/dedup/nodes/*#PUT+POST",
            "/_apis/pipelines#GET+DELETE+PUT+POST",
            "/*/_apis/pipelines#GET+DELETE+PUT+POST",
            "/DefaultCollection/*/_apis/pipelines#GET+POST",
            "/DefaultCollection/*/_apis/pipelines/approvals#GET+PATCH",
            "/DefaultCollection/*/_apis/pipelines/checks/configurations#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/pipelines/checks/runs#GET+POST+PATCH",
            "/DefaultCollection/*/_apis/pipelines/*/runs#GET+POST",
            "/DefaultCollection/*/_apis/distributedtask/hubs/checks/plans/*/events#POST",
            "/DefaultCollection/*/_apis/distributedtask/hubs/checks/plans/*/logs#GET+POST",
            "/DefaultCollection/*/_apis/distributedtask/hubs/checks/plans/*/timelines/*/records#GET+PATCH",
            "/*/_apis/distributedtask/hubs/build/*/events#POST",
            "/DefaultCollection/*/_apis/sourceProviders#GET+POST"
          },
          groupingKey = "build"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.pipelines",
          title = "Pipelines",
          description = "Grants the ability for external services to run pipeline scenarios.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.build_execute",
          patterns = new string[11]
          {
            "/DefaultCollection/*/_apis/pipelines/*/runs#GET+POST",
            "/DefaultCollection/_apis/pipelines/*/runs#GET+POST",
            "/_apis/distributedtask/elasticpools#DELETE+GET+PATCH+POST",
            "/_apis/distributedtask/elasticpools/*/elasticpoollogs#GET",
            "/_apis/distributedtask/packages/agent#GET",
            "/_apis/distributedtask/packages/pipelines-agent#GET",
            "/_apis/distributedtask/pools#DELETE+GET+PATCH+POST",
            "/_apis/distributedtask/pools/*/agents#DELETE+GET+PATCH+POST+PUT",
            "/_apis/distributedtask/pools/*/jobrequests#GET",
            "/_apis/distributedtask/pools/*/admintoken#GET",
            "/_apis/distributedtask/pools/*/agents#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.code",
          title = "Code (read)",
          description = "Grants the ability to read source code and metadata about commits, changesets, branches, and other version control artifacts. Also grants the ability to search code and get notified about version control events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.hooks_write",
          patterns = new string[192]
          {
            "/DefaultCollection/_apis/tfvc/branches#GET",
            "/DefaultCollection/*/_apis/tfvc/branches#GET",
            "/DefaultCollection/_apis/tfvc/changesets#GET",
            "/DefaultCollection/*/_apis/tfvc/changesets#GET",
            "/DefaultCollection/_apis/tfvc/changesetsBatch#POST",
            "/DefaultCollection/_apis/tfvc/labels#GET",
            "/DefaultCollection/*/_apis/tfvc/labels#GET",
            "/DefaultCollection/_apis/tfvc/labelItems#GET",
            "/DefaultCollection/_apis/tfvc/shelvesets#GET",
            "/DefaultCollection/_apis/tfvc/items#GET",
            "/DefaultCollection/*/_apis/tfvc/items#GET",
            "/DefaultCollection/_apis/tfvc/itemBatch#POST",
            "/DefaultCollection/*/_apis/tfvc/itemBatch#POST",
            "/DefaultCollection/_apis/tfvc/*/projectinfo#GET",
            "/DefaultCollection/*/*/_git/*#GET",
            "/DefaultCollection/*/_git/*#GET",
            "/DefaultCollection/_git/*#GET",
            "/DefaultCollection/*/*/_git/_full/*#GET",
            "/DefaultCollection/*/_git/_full/*#GET",
            "/DefaultCollection/_git/_full/*#GET",
            "/DefaultCollection/*/*/_git/_optimized/*#GET",
            "/DefaultCollection/*/_git/_optimized/*#GET",
            "/DefaultCollection/_git/_optimized/*#GET",
            "/DefaultCollection/*/*/_git/*/git-upload-pack#POST",
            "/DefaultCollection/*/_git/*/git-upload-pack#POST",
            "/DefaultCollection/_git/*/git-upload-pack#POST",
            "/DefaultCollection/*/*/_git/_full/*/git-upload-pack#POST",
            "/DefaultCollection/*/_git/_full/*/git-upload-pack#POST",
            "/DefaultCollection/_git/_full/*/git-upload-pack#POST",
            "/DefaultCollection/*/*/_git/_optimized/*/git-upload-pack#POST",
            "/DefaultCollection/*/_git/_optimized/*/git-upload-pack#POST",
            "/DefaultCollection/_git/_optimized/*/git-upload-pack#POST",
            "/DefaultCollection/*/*/_git/*/info/lfs/objects#POST",
            "/DefaultCollection/*/_git/*/info/lfs/objects#POST",
            "/DefaultCollection/_git/*/info/lfs/objects#POST",
            "/DefaultCollection/*/*/_git/_full/*/info/lfs/objects#POST",
            "/DefaultCollection/*/_git/_full/*/info/lfs/objects#POST",
            "/DefaultCollection/_git/_full/*/info/lfs/objects#POST",
            "/DefaultCollection/*/*/_git/_optimized/*/info/lfs/objects#POST",
            "/DefaultCollection/*/_git/_optimized/*/info/lfs/objects#POST",
            "/DefaultCollection/_git/_optimized/*/info/lfs/objects#POST",
            "/DefaultCollection/*/*/_git/*/gvfs/*#POST",
            "/DefaultCollection/*/_git/*/gvfs/*#POST",
            "/DefaultCollection/_git/*/gvfs/*#POST",
            "/DefaultCollection/*/*/_git/_full/*/gvfs/*#POST",
            "/DefaultCollection/*/_git/_full/*/gvfs/*#POST",
            "/DefaultCollection/_git/_full/*/gvfs/*#POST",
            "/DefaultCollection/*/*/_git/_optimized/*/gvfs/*#POST",
            "/DefaultCollection/*/_git/_optimized/*/gvfs/*#POST",
            "/DefaultCollection/_git/_optimized/*/gvfs/*#POST",
            "/DefaultCollection/*/_apis/git/*/repositories#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/blobs#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/blobs#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/commits",
            "/DefaultCollection/*/_apis/git/*/repositories/*/commits/*/statuses#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/commitsBatch#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/diffs/commits#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/items#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/itemsBatch#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/filepaths#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/forks/*#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/forkSyncRequests#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/forkSyncRequests/*#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/pullrequests#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/pullrequestquery#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/pushes#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/refs#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/stats/branches#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/stats/branches#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/trees#GET",
            "/DefaultCollection/*/_apis/git/*/repositories/*/mergeBases#GET",
            "/DefaultCollection/*/_apis/git/deletedrepositories#GET",
            "/DefaultCollection/*/_apis/git/favorites/refs#GET",
            "/DefaultCollection/*/_apis/git/favorites/refs#POST",
            "/DefaultCollection/*/_apis/git/favorites/refs#PUT",
            "/DefaultCollection/*/_apis/git/favorites/refs#DELETE",
            "/DefaultCollection/*/_apis/git/favorites/refsForProject#GET",
            "/DefaultCollection/*/_apis/git/repositories#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/blobs#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/blobs#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/commits",
            "/DefaultCollection/*/_apis/git/repositories/*/commits/*/statuses#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/commitsBatch#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/diffs/commits#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/items#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/itemsBatch#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/fileDiffs#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/filepaths#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/forks/*#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/forkSyncRequests#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/forkSyncRequests/*#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequests#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequests/*/conflicts#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequestquery#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/pushes#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/refs#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/stats/branches#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/stats/branches#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/trees#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/limitedRefCriteria#GET",
            "/DefaultCollection/*/_apis/git/repositories/*/mergeBases#GET",
            "/DefaultCollection/_apis/git/*/repositories#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/blobs#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/blobs#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/commits",
            "/DefaultCollection/_apis/git/*/repositories/*/commits/*/statuses#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/commitsBatch#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/diffs/commits#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/items#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/itemsBatch#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/filepaths#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/forks/*#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/forkSyncRequests#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/forkSyncRequests/*#GET",
            "/DefaultCollection/_apis/git/repositories/*/merges/*#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/pullrequests#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/pullrequestquery#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/pushes#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/refs#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/stats/branches#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/stats/branches#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/trees#GET",
            "/DefaultCollection/_apis/git/*/repositories/*/mergeBases#GET",
            "/DefaultCollection/_apis/git/repositories#GET",
            "/DefaultCollection/_apis/git/repositories/*/blobs#GET",
            "/DefaultCollection/_apis/git/repositories/*/blobs#POST",
            "/DefaultCollection/_apis/git/repositories/*/commits",
            "/DefaultCollection/_apis/git/repositories/*/commits/*/statuses#GET",
            "/DefaultCollection/_apis/git/repositories/*/commitsBatch#POST",
            "/DefaultCollection/_apis/git/repositories/*/diffs/commits#GET",
            "/DefaultCollection/_apis/git/repositories/*/items#GET",
            "/DefaultCollection/_apis/git/repositories/*/itemsBatch#POST",
            "/DefaultCollection/_apis/git/repositories/*/filepaths#GET",
            "/DefaultCollection/_apis/git/repositories/*/forks/*#GET",
            "/DefaultCollection/_apis/git/repositories/*/forkSyncRequests#GET",
            "/DefaultCollection/_apis/git/repositories/*/forkSyncRequests/*#GET",
            "/DefaultCollection/_apis/git/repositories/*/pullrequests#GET",
            "/DefaultCollection/_apis/git/repositories/*/pullrequests/*/conflicts#GET",
            "/DefaultCollection/_apis/git/repositories/*/pullrequestquery#POST",
            "/DefaultCollection/_apis/git/repositories/*/pushes#GET",
            "/DefaultCollection/_apis/git/repositories/*/refs#GET",
            "/DefaultCollection/_apis/git/repositories/*/stats/branches#GET",
            "/DefaultCollection/_apis/git/repositories/*/stats/branches#POST",
            "/DefaultCollection/_apis/git/repositories/*/trees#GET",
            "/DefaultCollection/_apis/git/repositories/*/limitedRefCriteria#GET",
            "/DefaultCollection/_apis/git/repositories/*/mergeBases#GET",
            "/DefaultCollection/*/_apis/git/pullRequests#GET",
            "/DefaultCollection/_apis/git/pullRequests#GET",
            "/DefaultCollection/_apis/codereview/reviews#GET",
            "/DefaultCollection/*/_apis/codereview/reviews#GET",
            "/DefaultCollection/*/_apis/codereview/reviewsbatch#POST",
            "/DefaultCollection/*/_apis/codereview/settings#GET",
            "/DefaultCollection/_apis/visits/artifactVisitsBatch#POST",
            "/DefaultCollection/_apis/visits/artifactStatsBatch#POST",
            "/DefaultCollection/*/_apis/policy/Evaluations#GET",
            "/DefaultCollection/*/_apis/policy/configurations#GET",
            "/DefaultCollection/*/_apis/policy/types#GET",
            "/DefaultCollection/*/_apis/policy/configurations/*/revisions#GET",
            "/DefaultCollection/*/_apis/git/policy/configurations#GET",
            "/_apis/search/codeSearchResults#POST",
            "/DefaultCollection/_apis/search/codeSearchResults#POST",
            "/DefaultCollection/*/_apis/search/codeSearchResults#POST",
            "/_apis/search/advancedCodeSearchResults#POST",
            "/DefaultCollection/_apis/search/advancedCodeSearchResults#POST",
            "/DefaultCollection/*/_apis/search/advancedCodeSearchResults#POST",
            "/_apis/search/codeQueryResults#POST",
            "/DefaultCollection/_apis/search/codeQueryResults#POST",
            "/DefaultCollection/*/_apis/search/codeQueryResults#POST",
            "/_apis/search/codeAdvancedQueryResults#POST",
            "/DefaultCollection/_apis/search/codeAdvancedQueryResults#POST",
            "/DefaultCollection/*/_apis/search/codeAdvancedQueryResults#POST",
            "/_apis/search/codeSearchPaginatedResults#POST",
            "/DefaultCollection/_apis/search/codeSearchPaginatedResults#POST",
            "/DefaultCollection/*/_apis/search/codeSearchPaginatedResults#POST",
            "/_apis/search/customCode#GET",
            "/DefaultCollection/_apis/search/customCode#GET",
            "/DefaultCollection/*/_apis/search/customCode#GET",
            "/_apis/search/customRepository#GET",
            "/DefaultCollection/_apis/search/customRepository#GET",
            "/DefaultCollection/*/_apis/search/customRepository#GET",
            "/_apis/search/customProject#GET",
            "/DefaultCollection/_apis/search/customProject#GET",
            "/DefaultCollection/*/_apis/search/customProject#GET",
            "/_apis/search/customTenant#GET",
            "/DefaultCollection/_apis/search/customTenant#GET",
            "/DefaultCollection/*/_apis/search/customTenant#GET",
            "/_apis/search/status/repositories/*#GET",
            "/DefaultCollection/*/_apis/search/status/repositories/*#GET",
            "/_apis/search/status/tfvc#GET",
            "/DefaultCollection/*/_apis/search/status/tfvc#GET",
            "/DefaultCollection/*/_api/_versioncontrol/fileDiff#GET+POST",
            "/DefaultCollection/*/_api/_versioncontrol/itemContent#GET"
          },
          groupingKey = "code"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.code_write",
          title = "Code (read and write)",
          description = "Grants the ability to read, update, and delete source code, access metadata about commits, changesets, branches, and other version control artifacts. Also grants the ability to create and manage pull requests and code reviews and to receive notifications about version control events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.code",
          patterns = new string[56]
          {
            "/DefaultCollection/_apis/tfvc/changesets#GET+POST",
            "/DefaultCollection/*/_apis/tfvc/changesets#GET+POST",
            "/DefaultCollection/*/*/_git/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_git/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_git/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/*/_git/_full/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_git/_full/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_git/_full/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/*/_git/_optimized/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_git/_optimized/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_git/_optimized/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/*/repositories/*/pullrequests#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/*/repositories/*/pushes#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/refs#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/*/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/forkSyncRequests#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/forkSyncRequests/*#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequests#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequests/*/conflicts#PATCH",
            "/DefaultCollection/*/_apis/git/repositories/*/pushes#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/refs#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/*/_apis/git/repositories/*/forkSyncRequests#POST",
            "/DefaultCollection/*/_apis/git/repositories/*/forkSyncRequests/*#POST",
            "/DefaultCollection/*/_apis/git/*/repositories/*/objects#PUT",
            "/DefaultCollection/*/_apis/git/repositories/*/objects#PUT",
            "/DefaultCollection/_apis/git/*/repositories/*/pullrequests#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/git/*/repositories/*/pushes#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/refs#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/git/*/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/_apis/git/*/repositories/*/forkSyncRequests#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/forkSyncRequests/*#POST",
            "/DefaultCollection/_apis/git/repositories/*/pullrequests#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/git/repositories/*/pullrequests/*/conflicts#PATCH",
            "/DefaultCollection/_apis/git/repositories/*/pushes#POST",
            "/DefaultCollection/_apis/git/repositories/*/refs#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/git/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/_apis/git/repositories/*/forkSyncRequests#POST",
            "/DefaultCollection/_apis/git/repositories/*/forkSyncRequests/*#POST",
            "/DefaultCollection/_apis/git/repositories/*/merges#POST",
            "/DefaultCollection/_apis/git/*/repositories/*/objects#PUT",
            "/DefaultCollection/_apis/git/repositories/*/objects#PUT",
            "/DefaultCollection/*/_apis/codereview/reviews#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/codereview/settings#POST+PUT",
            "/DefaultCollection/_apis/visits/artifactVisits#PUT",
            "/DefaultCollection/*/_apis/policy/Evaluations#PATCH",
            "/DefaultCollection/*/_apis/policy/configurations#POST+PUT+DELETE",
            "/_apis/search/customCode#POST",
            "/DefaultCollection/_apis/search/customCode#POST",
            "/DefaultCollection/*/_apis/search/customCode#POST",
            "/_apis/search/customRepository#POST",
            "/DefaultCollection/_apis/search/customRepository#POST",
            "/DefaultCollection/*/_apis/search/customRepository#POST",
            "/_apis/search/customTenant#POST",
            "/DefaultCollection/_apis/search/customTenant#POST",
            "/DefaultCollection/*/_apis/search/customTenant#POST"
          },
          groupingKey = "code"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.code_manage",
          title = "Code (read, write, and manage)",
          description = "Grants the ability to read, update, and delete source code, access metadata about commits, changesets, branches, and other version control artifacts. Also grants the ability to create and manage code repositories, create and manage pull requests and code reviews, and to receive notifications about version control events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.code_write",
          patterns = new string[7]
          {
            "/DefaultCollection/*/_apis/git/repositories#POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/*/repositories#POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/repositories/*/limitedRefCriteria#PUT",
            "/DefaultCollection/_apis/git/repositories#POST+PATCH+DELETE",
            "/DefaultCollection/_apis/git/*/repositories#POST+PATCH+DELETE",
            "/DefaultCollection/_apis/git/repositories/*/limitedRefCriteria#PUT",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions/repository#GET+PATCH"
          },
          groupingKey = "code"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.code_full",
          title = "Code (full)",
          description = "Grants full access to source code, access metadata about commits, changesets, branches, and other version control artifacts. Also grants the ability to create and manage code repositories, create and manage pull requests and code reviews, and to receive notifications about version control events via service hooks.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.code_manage",
          patterns = ((IEnumerable<string>) AuthorizationScopeDefinitions.ClientOmBasePatterns).Concat<string>((IEnumerable<string>) new string[12]
          {
            "/Services/v4.0/item.ashx",
            "/Services/v4.0/FileHandlerService.asmx#POST",
            "/DefaultCollection/Services/v4.0/item.ashx",
            "/DefaultCollection/Services/v4.0/FileHandlerService.asmx#POST",
            "/VersionControl/*/integration.asmx#POST",
            "/VersionControl/*/repository.asmx#POST",
            "/*/VersionControl/*/integration.asmx#POST",
            "/*/VersionControl/*/repository.asmx#POST",
            "/VersionControl/*/item.ashx#POST",
            "/VersionControl/*/upload.ashx#POST",
            "/*/VersionControl/*/item.ashx#POST",
            "/*/VersionControl/*/upload.ashx#POST"
          }).ToArray<string>(),
          groupingKey = "code"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.code_status",
          title = "Code (status)",
          description = "Grants the ability to read and write commit and pull request status.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[6]
          {
            "/DefaultCollection/_apis/git/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/*/_apis/git/repositories/*/commits/*/statuses#GET+POST",
            "/DefaultCollection/_apis/git/repositories/*/pullrequests/*/statuses#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequests/*/statuses#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/git/repositories/*/pullrequests/*/iterations/*/statuses#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequests/*/iterations/*/statuses#GET+POST+PATCH+DELETE"
          },
          groupingKey = "code"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.threads_full",
          title = "PR threads",
          description = "Grants the ability to read and write to pull request comment threads.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/git/repositories/*/pullrequests/*/threads#GET+POST+PATCH+PUT+DELETE",
            "/DefaultCollection/*/_apis/git/repositories/*/pullrequests/*/threads#GET+POST+PATCH+PUT+DELETE"
          },
          groupingKey = "prThreads"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.chat_write",
          title = "Team rooms (read and write)",
          description = "Grants the ability to access rooms and view, post, and update messages. Also grants the ability to receive notifications about new messages via service hooks.",
          availability = AuthorizationScopeAvailability.Deprecated,
          inheritsFrom = "vso.hooks_write",
          patterns = new string[3]
          {
            "/DefaultCollection/_apis/chat/rooms#GET",
            "/DefaultCollection/_apis/chat/rooms/*/messages#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/chat/rooms/*/users#GET+PUT+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.chat_manage",
          title = "Team rooms (read, write, and manage)",
          description = "Grants the ability to access rooms and view, post, and update messages. Also grants the ability to manage rooms and users and to receive notifications about new messages via service hooks.",
          availability = AuthorizationScopeAvailability.Deprecated,
          inheritsFrom = "vso.chat_write",
          patterns = new string[1]
          {
            "/DefaultCollection/_apis/chat/rooms#GET+POST+PATCH+PUT+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.agentpools",
          title = "Agent Pools (read)",
          description = "Grants the ability to view tasks, pools, queues, agents, and currently running or recently completed jobs for agents",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[26]
          {
            "/_apis/distributedtask/agentclouds#GET",
            "/_apis/distributedtask/agentclouds/*/requests/*/job#GET",
            "/_apis/distributedtask/agentcloudtypes#GET",
            "/_apis/distributedtask/elasticpools#GET",
            "/_apis/distributedtask/elasticpools/*/elasticpoollogs#GET",
            "/_apis/distributedtask/packages/agent#GET",
            "/_apis/distributedtask/packages/pipelines-agent#GET",
            "/_apis/distributedtask/pools#GET",
            "/_apis/distributedtask/pools/*/agents#GET",
            "/_apis/distributedtask/pools/*/jobrequests#GET",
            "/_apis/distributedtask/queues#GET",
            "/_apis/distributedtask/tasks#GET",
            "/_apis/distributedtask/pools/*/permissions#GET",
            "/DefaultCollection/_apis/distributedtask/pools/*/permissions#GET",
            "/DefaultCollection/_apis/distributedtask/agentclouds#GET",
            "/DefaultCollection/_apis/distributedtask/agentcloudtypes#GET",
            "/DefaultCollection/_apis/distributedtask/elasticpools#GET",
            "/DefaultCollection/_apis/distributedtask/packages/agent#GET",
            "/DefaultCollection/_apis/distributedtask/packages/pipelines-agent#GET",
            "/DefaultCollection/_apis/distributedtask/pools#GET",
            "/DefaultCollection/_apis/distributedtask/pools/*/agents#GET",
            "/DefaultCollection/_apis/distributedtask/pools/*/jobrequests#GET",
            "/DefaultCollection/_apis/distributedtask/queues#GET",
            "/DefaultCollection/_apis/distributedtask/yamlschema#GET",
            "/DefaultCollection/*/_apis/distributedtask/queues#GET",
            "/DefaultCollection/_apis/distributedtask/tasks#GET"
          },
          groupingKey = "agentPools"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.agentpools_manage",
          title = "Agent Pools (read, manage)",
          description = "Grants the ability to manage pools, queues, and agents",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.agentpools",
          patterns = new string[24]
          {
            "/_apis/distributedtask/agentclouds#DELETE+GET+PATCH+POST",
            "/_apis/distributedtask/elasticpools#DELETE+GET+PATCH+POST",
            "/_apis/distributedtask/elasticpools/*/elasticpoollogs#GET",
            "/_apis/distributedtask/packages/agent#GET",
            "/_apis/distributedtask/packages/pipelines-agent#GET",
            "/_apis/distributedtask/pools#DELETE+GET+PATCH+POST",
            "/_apis/distributedtask/pools/*/agents#DELETE+GET+PATCH+POST+PUT",
            "/_apis/distributedtask/pools/*/jobrequests#GET",
            "/_apis/distributedtask/queues#DELETE+GET+PATCH+POST+PUT",
            "/_apis/distributedtask/tasks#DELETE+GET+PATCH+POST+PUT",
            "/_apis/distributedtask/pools/*/permissions#GET",
            "/DefaultCollection/_apis/distributedtask/pools/*/permissions#GET",
            "/DefaultCollection/_apis/distributedtask/agentclouds#DELETE+GET+PATCH+POST",
            "/DefaultCollection/_apis/distributedtask/elasticpools#DELETE+GET+PATCH+POST",
            "/DefaultCollection/_apis/distributedtask/packages/agent#GET",
            "/DefaultCollection/_apis/distributedtask/packages/pipelines-agent#GET",
            "/DefaultCollection/_apis/distributedtask/pools#DELETE+GET+PATCH+POST",
            "/DefaultCollection/_apis/distributedtask/pools/*/agents#DELETE+GET+PATCH+POST+PUT",
            "/DefaultCollection/_apis/distributedtask/pools/*/jobrequests#GET",
            "/DefaultCollection/_apis/distributedtask/queues#DELETE+GET+PATCH+POST",
            "/DefaultCollection/*/_apis/distributedtask/queues#DELETE+GET+PATCH+POST+PUT",
            "/DefaultCollection/_apis/distributedtask/tasks#DELETE+GET+PATCH+POST+PUT",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions/queue#GET+PATCH",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions/agentpool#GET+PATCH"
          },
          groupingKey = "agentPools"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.agentpools_listen",
          title = "Agent Pools (read, listen)",
          description = "Grants the ability to view pools, manage sessions, and listen for messages from the message queue",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.agentpools",
          patterns = new string[10]
          {
            "/_apis/distributedtask/pools/*/jobrequests#GET+PATCH+DELETE",
            "/_apis/distributedtask/pools/*/messages#GET+DELETE",
            "/_apis/distributedtask/pools/*/sessions#GET+POST+DELETE",
            "/_apis/distributedtask/pools/*/agents/*/updates#PUT",
            "/_apis/distributedtask/pools/*/permissions#GET",
            "/DefaultCollection/_apis/distributedtask/pools/*/permissions#GET",
            "/DefaultCollection/_apis/distributedtask/pools/*/jobrequests#GET+PATCH+DELETE",
            "/DefaultCollection/_apis/distributedtask/pools/*/messages#GET+DELETE",
            "/DefaultCollection/_apis/distributedtask/pools/*/sessions#GET+POST+DELETE",
            "/DefaultCollection/_apis/distributedtask/pools/*/agents/*/updates#PUT"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.governance.extension_write",
          title = "Governance extension (read and write)",
          description = "Grants ability for an extension to Governance to read and write data.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.base",
          patterns = new string[9]
          {
            "/_apis/governance/components#GET",
            "/_apis/governance/products#GET",
            "/_apis/governance/products/*/registrations/*/policyStatuses#PUT",
            "/*/_apis/ComponentGovernance/components#GET",
            "/*/_apis/ComponentGovernance/GovernedRepositories#GET",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/registrations/*/policyStatuses#PUT",
            "/_apis/ComponentGovernance/GovernedRepositories/*/registrations/*/policyStatuses#PUT",
            "/*/_apis/ComponentGovernance/componentknowledgequeries#POST",
            "/_apis/ComponentGovernance/componentknowledgequeries#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.governance",
          title = "Governance (read)",
          description = "Grants ability to read products and registrations.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.profile",
          patterns = new string[4]
          {
            "/_apis/governance/*#GET",
            "/_apis/governance/products/*/registrationcomponentQueries#POST",
            "/*/_apis/ComponentGovernance/*#GET",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/registrationcomponentQueries#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.governance_write",
          title = "Governance (read and write)",
          description = "Grants ability to read and write products and registrations.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.governance",
          patterns = new string[13]
          {
            "/_apis/governance/products/*/snapshots#PATCH",
            "/_apis/governance/products/*/registrationrequests#POST",
            "/_apis/governance/products/*/registrations#PATCH",
            "/_apis/governance/products/*/registrationbatch#POST+PATCH",
            "/_apis/governance/products/*/registrations/*/policystatuses/*/retryrequest#POST",
            "/_apis/governance/products/*/registrations/*/policystatuses#PATCH",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/snapshots#PATCH",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/registrationrequests#POST",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/registrations#PATCH",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/registrationbatch#POST+PATCH",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/registrations/*/policystatuses/*/retryrequest#POST",
            "/*/_apis/ComponentGovernance/GovernedRepositories/*/registrations/*/policystatuses#PATCH",
            "/_apis/ComponentGovernance/GovernedRepositories/*/registrations/*/policystatuses#PATCH"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.governance_manage",
          title = "Governance (read, write and manage)",
          description = "Grants ability to read, write, update and delete products and registrations.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.governance_write",
          patterns = new string[3]
          {
            "/_apis/governance/*#POST+PATCH+PUT+DELETE",
            "/*/_apis/ComponentGovernance/*#POST+PATCH+PUT+DELETE",
            "/_apis/ComponentGovernance/*#POST+PATCH+PUT+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.packaging",
          title = "Packaging (read)",
          description = "Grants the ability to read feeds and packages. Also grants the ability to search packages.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.profile",
          patterns = new string[23]
          {
            "/_apis/clienttools/artifacttool/release#GET",
            "/_apis/dedup/chunks/*#GET",
            "/_apis/dedup/nodes/*#GET",
            "/_apis/dedup/urls/*#POST",
            "/_apis/packaging#HEAD+GET",
            "/_apis/public/packaging#HEAD+GET",
            "/_packaging#HEAD+GET",
            "/*/_apis/clienttools/artifacttool/release#GET",
            "/*/_apis/dedup/chunks/*#GET",
            "/*/_apis/dedup/nodes/*#GET",
            "/*/_apis/dedup/urls/*#POST",
            "/*/_apis/packaging#HEAD+GET",
            "/*/_apis/public/packaging#HEAD+GET",
            "/*/_packaging#HEAD+GET",
            "/_apis/search/packageSearchResults#POST",
            "/DefaultCollection/_apis/search/packageSearchResults#POST",
            "/DefaultCollection/*/_apis/search/packageSearchResults#POST",
            "/_packaging/*/npm/registry/-/npm/v1/security/advisories/bulk#POST",
            "/_packaging/*/npm/registry/-/npm/v1/security/audits/quick#POST",
            "/_packaging/*/npm/registry/-/npm/v1/security/audits#POST",
            "/*/_packaging/*/npm/registry/-/npm/v1/security/advisories/bulk#POST",
            "/*/_packaging/*/npm/registry/-/npm/v1/security/audits/quick#POST",
            "/*/_packaging/*/npm/registry/-/npm/v1/security/audits#POST"
          },
          groupingKey = "packaging"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.packaging_write",
          title = "Packaging (read and write)",
          description = "Grants the ability to create and read feeds and packages.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.packaging",
          patterns = new string[18]
          {
            "/_apis/dedup/chunks/*#PUT+POST",
            "/_apis/dedup/nodes/*#PUT+POST",
            "/_apis/packaging#HEAD+GET+POST+PUT",
            "/_apis/provenance#HEAD+GET+POST+PUT",
            "/_packaging#HEAD+GET+POST+PUT",
            "/_packaging/*/nuget/v2#DELETE",
            "/_packaging/*/npm#DELETE",
            "/_apis/packaging/feeds/*/maven/groups/*/artifacts/*/versions#DELETE",
            "/_apis/packaging/*/packages/*/versions#DELETE",
            "/*/_apis/dedup/chunks/*#PUT+POST",
            "/*/_apis/dedup/nodes/*#PUT+POST",
            "/*/_apis/packaging#HEAD+GET+POST+PUT",
            "/*/_apis/provenance#HEAD+GET+POST+PUT",
            "/*/_packaging#HEAD+GET+POST+PUT",
            "/*/_packaging/*/nuget/v2#DELETE",
            "/*/_packaging/*/npm#DELETE",
            "/*/_apis/packaging/feeds/*/maven/groups/*/artifacts/*/versions#DELETE",
            "/*/_apis/packaging/*/packages/*/versions#DELETE"
          },
          groupingKey = "packaging"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.packaging_manage",
          title = "Packaging (read, write, and manage)",
          description = "Grants the ability to create, read, update, and delete feeds and packages.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.packaging_write",
          patterns = new string[4]
          {
            "/_apis/packaging#HEAD+GET+PATCH+POST+PUT+DELETE",
            "/_packaging#HEAD+GET+PATCH+POST+PUT+DELETE",
            "/*/_apis/packaging#HEAD+GET+PATCH+POST+PUT+DELETE",
            "/*/_packaging#HEAD+GET+PATCH+POST+PUT+DELETE"
          },
          groupingKey = "packaging"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.oss",
          title = "OSS (read)",
          description = "OSS service read",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.profile",
          patterns = new string[1]
          {
            "/DefaultCollection/_apis/oss/*#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.oss_write",
          title = "OSS (read and write)",
          description = "OSS service read and write",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.oss",
          patterns = new string[3]
          {
            "/DefaultCollection/_apis/oss/Requests#POST+PUT+PATCH",
            "/DefaultCollection/_apis/oss/Versions#PATCH",
            "/DefaultCollection/_apis/oss/Validation#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.oss_manage",
          title = "OSS (read, write and manage)",
          description = "OSS service read, write and manage",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.oss_write",
          patterns = new string[1]
          {
            "/DefaultCollection/_apis/oss/*#POST+PUT+PATCH+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.oss.extension_read",
          title = "OSS (read OSS extension data)",
          description = "Grants ability to read OSS extension data",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.profile",
          patterns = new string[1]
          {
            "/DefaultCollection/_apis/oss/ExtensionData/*#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.oss.extension_write",
          title = "OSS (read and write OSS extension data)",
          description = "Grants ability to read and write OSS extension data",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.oss.extension_read",
          patterns = new string[1]
          {
            "/DefaultCollection/_apis/oss/ExtensionData/*#POST+PUT+PATCH+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.test",
          title = "Test management (read)",
          description = "Grants the ability to read test plans, cases, results and other test management related artifacts.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.profile",
          patterns = new string[63]
          {
            "/DefaultCollection/_apis/test/suites#GET",
            "/DefaultCollection/_apis/testplan/suites#GET",
            "/DefaultCollection/*/_apis/test/plans#GET",
            "/DefaultCollection/*/_apis/test/runs#GET",
            "/DefaultCollection/*/_apis/test/extensionFields#GET",
            "/DefaultCollection/*/_apis/test/suites#GET",
            "/DefaultCollection/*/_apis/test/results#GET",
            "/DefaultCollection/*/_apis/test/testSettings#GET",
            "/DefaultCollection/*/_apis/test/codeCoverage#GET",
            "/DefaultCollection/*/_apis/test/configurations#GET",
            "/DefaultCollection/*/_apis/test/variables#GET",
            "/DefaultCollection/*/_apis/test/cloneOperation#GET",
            "/DefaultCollection/*/_apis/test/session#GET",
            "/DefaultCollection/*/_apis/test/suiteEntry#GET",
            "/DefaultCollection/*/_apis/test/ResultRetentionSettings#GET",
            "/DefaultCollection/*/_apis/test/ResultSummaryByRelease#GET",
            "/DefaultCollection/*/_apis/test/ResultSummaryByBuild#GET",
            "/DefaultCollection/*/_apis/test/ResultSummaryByRequirement#GET",
            "/DefaultCollection/*/_apis/test/ResultDetailsByRelease#GET",
            "/DefaultCollection/*/_apis/test/ResultDetailsByBuild#GET",
            "/DefaultCollection/*/_apis/test/TestMethods#GET",
            "/DefaultCollection/*/_apis/test/Points#GET",
            "/DefaultCollection/*/_apis/testresults/runs#GET",
            "/DefaultCollection/*/_apis/testresults/tags#GET",
            "/DefaultCollection/*/_apis/testresults/tagsummary#GET",
            "/DefaultCollection/*/_apis/testresults/testlog#GET",
            "/DefaultCollection/*/_apis/testresults/testlogstoreendpoint#GET",
            "/DefaultCollection/*/_apis/testresults/extensionFields#GET",
            "/DefaultCollection/*/_apis/testresults/results#GET",
            "/DefaultCollection/*/_apis/testresults/testSettings#GET",
            "/DefaultCollection/*/_apis/testresults/codeCoverage#GET",
            "/DefaultCollection/*/_apis/testresults/ResultRetentionSettings#GET",
            "/DefaultCollection/*/_apis/testresults/ResultSummaryByRelease#GET",
            "/DefaultCollection/*/_apis/testresults/ResultSummaryByBuild#GET",
            "/DefaultCollection/*/_apis/testresults/ResultSummaryByRequirement#GET",
            "/DefaultCollection/*/_apis/testresults/ResultDetailsByRelease#GET",
            "/DefaultCollection/*/_apis/testresults/ResultDetailsByBuild#GET",
            "/DefaultCollection/*/_apis/testresults/ResultsByPipeline#GET",
            "/DefaultCollection/*/_apis/testresults/ResultsByBuild#GET",
            "/DefaultCollection/*/_apis/testresults/ResultsByRelease#GET",
            "/DefaultCollection/*/_apis/testresults/TestMethods#GET",
            "/DefaultCollection/*/_apis/testresults/resultgroupsbybuild#GET",
            "/DefaultCollection/*/_apis/testresults/resultgroupsbyrelease#GET",
            "/DefaultCollection/*/_apis/testresults/metrics#GET",
            "/DefaultCollection/*/_apis/testresults/testsession#GET",
            "/DefaultCollection/*/_apis/testresults/testfailuretype#GET",
            "/DefaultCollection/*/_apis/testplan/plans#GET",
            "/DefaultCollection/*/_apis/testplan/suites#GET",
            "/DefaultCollection/*/_apis/testplan/configurations#GET",
            "/DefaultCollection/*/_apis/testplan/variables#GET",
            "/DefaultCollection/*/_apis/testplan/suiteEntry#GET",
            "/DefaultCollection/*/_apis/testplan/TestCases/CloneTestCaseOperation#GET",
            "/DefaultCollection/*/_apis/tcm/runs#GET",
            "/DefaultCollection/*/*/_apis/test/session#GET",
            "/DefaultCollection/_apis/FeatureFlags#GET",
            "/TestManagement/v1.0/AttachmentUpload.ashx#POST",
            "/*/TestManagement/v1.0/AttachmentUpload.ashx#POST",
            "/TestManagement/v2.0/TestManagementWebService.asmx#POST",
            "/*/TestManagement/v2.0/TestManagementWebService.asmx#POST",
            "/TestManagement/v2.0/TestManagementWebService3.asmx#POST",
            "/*/TestManagement/v2.0/TestManagementWebService3.asmx#POST",
            "/TestManagement/v2.0/TestManagementWebService4.asmx#POST",
            "/*/TestManagement/v2.0/TestManagementWebService4.asmx#POST"
          },
          groupingKey = "testManagement"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.test_write",
          title = "Test management (read and write)",
          description = "Grants the ability to read, create, and update test plans, cases, results and other test management related artifacts.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.test",
          patterns = new string[45]
          {
            "/DefaultCollection/*/_apis/test/plans#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/test/suites#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/test/runs#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/test/results#GET+POST",
            "/DefaultCollection/*/_apis/test/extensionFields#GET+POST",
            "/DefaultCollection/*/_apis/test/testSettings#GET+POST+DELETE",
            "/DefaultCollection/*/_apis/test/codeCoverage#GET+POST",
            "/DefaultCollection/*/_apis/test/configurations#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/test/variables#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/test/cloneOperation#GET+POST",
            "/DefaultCollection/*/_apis/test/session#GET+POST+PATCH",
            "/DefaultCollection/*/_apis/test/suiteEntry#GET+PATCH",
            "/DefaultCollection/*/_apis/test/ResultRetentionSettings#GET+PATCH",
            "/DefaultCollection/*/_apis/test/ResultSummaryByRelease#GET+POST",
            "/DefaultCollection/*/_apis/test/ResultSummaryByRequirement#GET+POST",
            "/DefaultCollection/*/_apis/test/testcases#DELETE",
            "/DefaultCollection/*/_apis/test/TestMethods#GET+POST+DELETE",
            "/DefaultCollection/*/_apis/test/Points#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/testresults/testsession#GET+POST",
            "/DefaultCollection/*/_apis/testresults/runs#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/testresults/uploadbuildattachments#POST",
            "/DefaultCollection/*/_apis/testresults/tags#GET+POST+PATCH",
            "/DefaultCollection/*/_apis/testresults/tagsummary#GET+POST+PATCH",
            "/DefaultCollection/*/_apis/testresults/testlog#GET+POST",
            "/DefaultCollection/*/_apis/testresults/testlogstoreendpoint#GET+POST",
            "/DefaultCollection/*/_apis/testresults/results#GET+POST+PATCH",
            "/DefaultCollection/*/_apis/testresults/extensionFields#GET+POST",
            "/DefaultCollection/*/_apis/testresults/testSettings#GET+POST+DELETE",
            "/DefaultCollection/*/_apis/testresults/codeCoverage#GET+POST",
            "/DefaultCollection/*/_apis/testresults/testfailuretype#GET+POST+DELETE",
            "/DefaultCollection/*/_apis/testresults/ResultRetentionSettings#GET+PATCH",
            "/DefaultCollection/*/_apis/testresults/ResultSummaryByRelease#GET+POST",
            "/DefaultCollection/*/_apis/testresults/ResultSummaryByRequirement#GET+POST",
            "/DefaultCollection/*/_apis/testresults/TestMethods#GET+POST+DELETE",
            "/DefaultCollection/*/_apis/testplan/plans#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/testplan/suites#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/testplan/configurations#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/testplan/variables#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/testplan/suiteEntry#GET+PATCH",
            "/DefaultCollection/*/_apis/testplan/testcases#DELETE",
            "/DefaultCollection/*/_apis/testplan/TestCases/CloneTestCaseOperation#GET+POST",
            "/DefaultCollection/*/_api/_testManagement/BulkMarkTestPoints#POST",
            "/DefaultCollection/*/_apis/testplan/TestCases/TestCaseFile#POST",
            "/DefaultCollection/*/*/_apis/test/session#GET+POST+PATCH",
            "/DefaultCollection/_apis/FeatureFlags#GET"
          },
          groupingKey = "testManagement"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.loadtest",
          title = "Load test (read)",
          availability = AuthorizationScopeAvailability.Deprecated,
          description = "Grants the ability to read your load test runs, test results and APM artifacts.",
          inheritsFrom = "vso.base",
          patterns = new string[11]
          {
            "/_apis/clt/testdrops",
            "/_apis/clt/testruns",
            "/_apis/clt/testruns/*/errors",
            "/_apis/clt/testruns/*/messages",
            "/_apis/clt/testruns/*/results",
            "/_apis/clt/testruns/*/counterinstances",
            "/_apis/clt/testruns/*/countersamples",
            "/_apis/clt/apm",
            "/_apis/clt/configuration",
            "/_apis/clt/agentgroups",
            "/_apis/clt/agentgroups/*/agents"
          },
          groupingKey = "loadTest"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.loadtest_write",
          title = "Load test (read and write)",
          availability = AuthorizationScopeAvailability.Deprecated,
          description = "Grants the ability to create and update load test runs, and read metadata including test results and APM artifacts.",
          inheritsFrom = "vso.loadtest",
          patterns = new string[4]
          {
            "/_apis/clt/testdrops#GET+POST",
            "/_apis/clt/testruns#GET+POST+PATCH",
            "/_apis/clt/agentgroups#GET+POST+PATCH+DELETE",
            "/_apis/clt/agentgroups/*/agents#GET+POST+PATCH+DELETE"
          },
          groupingKey = "loadTest"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.licensing",
          title = "Access to the VSTS licensing endpoints",
          description = "Provides read only access to VSTS licensing endpoint to get a user's client rights.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.profile",
          patterns = new string[4]
          {
            "/_apis/licensing/clientrights",
            "/_apis/licensing/ExtensionEntitlements/*/*#GET",
            "/_apis/licensing/Usage#GET",
            "/_apis/licensing/AccountAssignedExtensions#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.extension",
          title = "Extensions (read)",
          description = "Grants the ability to read installed extensions.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.profile",
          patterns = new string[2]
          {
            "/_apis/ExtensionManagement/InstalledExtensions#GET",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions#GET"
          },
          groupingKey = "extensions"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.extension_manage",
          title = "Extensions (read and manage)",
          description = "Grants the ability to install, uninstall, and perform other administrative actions on installed extensions.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.extension",
          patterns = new string[10]
          {
            "/_apis/ExtensionManagement/InstalledExtensions#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions#GET+POST+PATCH+DELETE+PUT",
            "/_apis/ExtensionManagement/InstalledExtensionsByName#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensionsByName#GET+POST+PATCH+DELETE+PUT",
            "/_apis/ExtensionManagement/RequestedExtensions#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/_apis/ExtensionManagement/RequestedExtensions#GET+POST+PATCH+DELETE+PUT",
            "/_apis/ExtensionManagement/AcquisitionOptions#GET",
            "/DefaultCollection/_apis/ExtensionManagement/AcquisitionOptions#GET",
            "/_apis/ExtensionManagement/AcquisitionRequests#POST",
            "/DefaultCollection/_apis/ExtensionManagement/AcquisitionRequests#POST"
          },
          groupingKey = "extensions"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.extension.data",
          title = "Extension data (read)",
          description = "Grants the ability to read data (settings and documents) stored by installed extensions.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.profile",
          patterns = new string[8]
          {
            "/_apis/ExtensionManagement/InstalledExtensions/*/Data#GET",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions/*/Data#GET",
            "/_apis/ExtensionManagement/InstalledExtensions/*/*/Data#GET",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions/*/*/Data#GET",
            "/_apis/Contribution/InstalledApps#GET",
            "/DefaultCollection/_apis/Contribution/InstalledApps#GET",
            "/_apis/ExtensionManagement/InstalledExtensions/*/*/ExtensionDataCollectionQuery#POST",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions/*/*/ExtensionDataCollectionQuery#POST"
          },
          groupingKey = "extensionData"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.extension.data_write",
          title = "Extension data (read and write)",
          description = "Grants the ability to read and write data (settings and documents) stored by installed extensions.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.extension.data",
          patterns = new string[4]
          {
            "/_apis/ExtensionManagement/InstalledExtensions/*/Data#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions/*/Data#GET+POST+PATCH+DELETE+PUT",
            "/_apis/ExtensionManagement/InstalledExtensions/*/*/Data#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions/*/*/Data#GET+POST+PATCH+DELETE+PUT"
          },
          groupingKey = "extensionData"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.extension.default",
          title = "Basic read access",
          description = "Grants read access to user, collection, and project information. Also grants the ability to store data.",
          availability = AuthorizationScopeAvailability.None,
          inheritsFrom = "vso.extension.data_write",
          patterns = new string[4]
          {
            "/_apis/Contribution/InstalledApps#GET",
            "/DefaultCollection/_apis/Contribution/InstalledApps#GET",
            "/_apis/Contribution/nodes/query#POST",
            "/DefaultCollection/_apis/Contribution/nodes/query#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.extension.preview",
          title = "Extension preview",
          description = "Grants a preview set of scopes for extensions.",
          availability = AuthorizationScopeAvailability.Deprecated,
          inheritsFrom = "preview_api_all",
          patterns = new string[2]
          {
            "/_apis/ExtensionManagement/InstalledExtensions/*/Data#GET+POST+PATCH+DELETE+PUT",
            "/DefaultCollection/_apis/ExtensionManagement/InstalledExtensions/*/Data#GET+POST+PATCH+DELETE+PUT"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.commerce.write",
          title = "Access Commerce Apis",
          description = "Access to read from all commerce apis, and link a subscription",
          inheritsFrom = "vso.base",
          availability = AuthorizationScopeAvailability.None,
          patterns = new string[15]
          {
            "/_apis/Commerce/*#GET+POST+PATCH+PUT",
            "/_apis/Commerce/Subscription/*/*#GET+PUT+POST",
            "/_apis/OfferMeter/*#GET+POST",
            "/_apis/OfferMeter/OfferMeter/*#GET+POST",
            "/_apis/OfferMeter/OfferMeterPrice/*#GET+PUT",
            "/_apis/Meters/*#GET+PATCH",
            "/_apis/Meters/Meters/*#GET+PATCH",
            "/_apis/OfferSubscription/*#GET+POST+PATCH+PUT",
            "/_apis/OfferSubscription/OfferSubscription/*#GET+POST+PATCH+PUT",
            "/_apis/Subscription/Subscription/*/*#GET+PUT+POST+PATCH+DELETE",
            "/_apis/PurchaseRequest/PurchaseRequest/*#PUT+PATCH",
            "/_apis/Subscription/AccountDetails/*#GET",
            "/_apis/OrganizationBilling/OrganizationBilling/*#GET",
            "/_apis/DefaultAccessLevel/DefaultAccessLevel/*#GET+POST",
            "/_apis/EnterpriseBilling/EnterpriseBilling/*#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.azcomm_write",
          title = "Access AzComm Apis",
          description = "Access to read and write the AzComm data",
          inheritsFrom = "vso.base",
          availability = AuthorizationScopeAvailability.None,
          patterns = new string[6]
          {
            "/*/_apis/AzComm/BillingSetup/*#GET+PATCH+PUT+DELETE",
            "/*/_apis/AzComm/MeterResource/*#GET+PATCH+POST",
            "/_apis/AzComm/MeterUsage/*#GET+POST",
            "/*/_apis/AzComm/MeterUsage2/*/*#GET+POST",
            "/_apis/AzComm/AzureSubscription/*#GET",
            "/*/_apis/AzComm/DefaultLicenseType/*#GET+PUT"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.gallery",
          title = "Marketplace",
          description = "Grants read access to public and private items and publishers.",
          inheritsFrom = "vso.profile",
          availability = AuthorizationScopeAvailability.Public,
          patterns = new string[2]
          {
            "/_apis/gallery/publishers#GET",
            "/_apis/gallery/extensions#GET"
          },
          groupingKey = "marketplace"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.gallery_acquire",
          title = "Marketplace (acquire)",
          description = "Grants read access and the ability to acquire items.",
          inheritsFrom = "vso.gallery",
          availability = AuthorizationScopeAvailability.Public,
          patterns = new string[2]
          {
            "/_apis/gallery/acquisitionrequests#POST",
            "/_apis/gallery/acquisitionoptions#GET+POST"
          },
          groupingKey = "marketplace"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.gallery_publish",
          title = "Marketplace (publish)",
          description = "Grants read access and the ability to upload, update, and share items.",
          inheritsFrom = "vso.gallery",
          availability = AuthorizationScopeAvailability.Public,
          patterns = new string[4]
          {
            "/_apis/gallery/publishers/*/extensions#POST+PUT+DELETE",
            "/_apis/gallery/publisher/*/extension/*/accountsbyname#POST+DELETE",
            "/_apis/gallery/extensions#POST+PUT+DELETE",
            "/_apis/gallery/extensions/*/accounts#POST+DELETE"
          },
          groupingKey = "marketplace"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.gallery_manage",
          title = "Marketplace (manage)",
          description = "Grants read access, the ability to publish and manage items and publishers.",
          inheritsFrom = "vso.gallery_publish",
          availability = AuthorizationScopeAvailability.Public,
          patterns = new string[1]
          {
            "/_apis/gallery/publishers#POST+PUT+DELETE"
          },
          groupingKey = "marketplace"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.entitlements",
          title = "Entitlements (Read)",
          description = "Provides read only access to VSTS licensing entitlements endpoint to get account entitlements.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[1]
          {
            "/_apis/licensing/entitlements#GET"
          },
          groupingKey = "entitlements"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "preview_msdn_licensing",
          title = "Access to MSDN subscriptions",
          description = "Provides this application the ability to access your MSDN subscription information including your level and expiration date and interact with Microsoft Developer Services on your behalf.",
          availability = AuthorizationScopeAvailability.None,
          patterns = new string[1]
          {
            "/_apis/licensing/msdn/me"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "user_impersonation",
          availability = AuthorizationScopeAvailability.System,
          patterns = new string[1]{ "/_signedin#GET+POST" }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "signout",
          availability = AuthorizationScopeAvailability.System,
          patterns = new string[1]{ "/_signout#GET+POST" }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "app_token",
          availability = AuthorizationScopeAvailability.System,
          patterns = new string[1]{ "/*#*" }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.release",
          title = "Release (read)",
          description = "Grants the ability to read release artifacts, including folders, releases, release definitions and release environment.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.profile",
          patterns = new string[17]
          {
            "/DefaultCollection/_apis/release/releases#GET",
            "/DefaultCollection/*/_apis/release/definitions#GET",
            "/DefaultCollection/*/_apis/release/definitions/*/revisions#GET",
            "/DefaultCollection/*/_apis/release/definitionEnvironments#GET",
            "/DefaultCollection/*/_apis/release/releases#GET",
            "/DefaultCollection/*/_apis/release/releases/*/workitems#GET",
            "/DefaultCollection/*/_apis/release/approvals#GET",
            "/DefaultCollection/*/_apis/release/releases/*/manualInterventions#GET",
            "/DefaultCollection/*/_apis/release/deployments#GET+POST",
            "/DefaultCollection/*/_apis/release/artifacts/versions#GET",
            "/DefaultCollection/*/_apis/release/environmenttemplates#GET",
            "/DefaultCollection/*/_apis/distributedtask/hubs/release/plans#GET",
            "/DefaultCollection/*/_apis/distributedtask/hubs/gates/plans#GET",
            "/DefaultCollection/*/_apis/release/artifacts/inputvaluesquery#POST",
            "/DefaultCollection/*/_apis/release/folders#GET",
            "/DefaultCollection/*/_apis/release/releasesettings#GET",
            "/DefaultCollection/*/_apis/DeploymentTracking/deploymentresources#GET"
          },
          groupingKey = "release"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.release_execute",
          title = "Release (read, write and execute)",
          description = "Grants the ability to read and update release artifacts, including folders, releases, release definitions and release environment, and the ability to queue a new release.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.release",
          patterns = new string[16]
          {
            "/DefaultCollection/*/_apis/release/definitions#GET+POST+PUT",
            "/DefaultCollection/*/_apis/release/definitions/*/revisions#GET",
            "/DefaultCollection/*/_apis/release/releases#GET+POST+PATCH+PUT",
            "/DefaultCollection/*/_apis/release/releases/*/workitems#GET",
            "/DefaultCollection/*/_apis/release/releases/*/environments#PATCH",
            "/DefaultCollection/*/_apis/release/approvals#GET",
            "/DefaultCollection/*/_apis/release/releases/*/manualInterventions#GET",
            "/DefaultCollection/*/_apis/release/environmenttemplates#GET+POST+PATCH",
            "/DefaultCollection/*/_apis/release/gates#PATCH",
            "/DefaultCollection/*/_apis/release/deployments#GET+POST",
            "/DefaultCollection/*/_apis/release/artifacts/versions#GET+POST",
            "/DefaultCollection/*/_apis/release/folders#GET+POST+PATCH",
            "/*/_apis/distributedtask/hubs/release/plans/*/events#POST",
            "/*/_apis/distributedtask/hubs/gates/plans/*/events#POST",
            "/DefaultCollection/*/_apis/distributedtask/hubs/release/plans#GET+POST+PATCH",
            "/DefaultCollection/*/_apis/distributedtask/hubs/gates/plans#GET+POST+PATCH"
          },
          groupingKey = "release"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.release_manage",
          title = "Release (read, write, execute and manage)",
          description = "Grants the ability to read, update and delete release artifacts, including folders, releases, release definitions and release environment and the ability to queue and approve a new release.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.release_execute",
          patterns = new string[10]
          {
            "/DefaultCollection/*/_apis/release/definitions#GET+POST+PUT+DELETE",
            "/DefaultCollection/*/_apis/release/definitions/*/revisions#GET",
            "/DefaultCollection/*/_apis/release/releases#GET+POST+PATCH+PUT+DELETE",
            "/DefaultCollection/*/_apis/release/releases/*/workitems#GET",
            "/DefaultCollection/*/_apis/release/releases/*/environments#PATCH",
            "/DefaultCollection/*/_apis/release/approvals#GET+PATCH",
            "/DefaultCollection/*/_apis/release/releases/*/manualInterventions#GET+PATCH",
            "/DefaultCollection/*/_apis/release/environmenttemplates#GET+POST+PATCH+DELETE",
            "/DefaultCollection/*/_apis/release/deployments#GET+POST",
            "/DefaultCollection/*/_apis/release/folders#GET+POST+PATCH+DELETE"
          },
          groupingKey = "release"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.release_logs",
          title = "Release Logs (view and download)",
          description = "Grants the ability to view and download logs.",
          availability = AuthorizationScopeAvailability.None,
          patterns = new string[4]
          {
            "/DefaultCollection/*/_apis/release/releases/*/logs#GET",
            "/DefaultCollection/*/_apis/release/releases/*/environments/*/tasks/*/logs#GET",
            "/DefaultCollection/*/_apis/release/releases/*/environments/*/gates/*/tasks/*/logs#GET",
            "/DefaultCollection/*/_apis/release/releases/*/environments/*/deployPhases/*/tasks/*/logs#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.securefiles_read",
          title = "Secure Files (read)",
          description = "Grants the ability to read secure files.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/distributedtask/securefiles#GET"
          },
          groupingKey = "secureFiles"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.securefiles_write",
          title = "Secure Files (read, create)",
          description = "Grants the ability to read and create secure files.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.securefiles_read",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/distributedtask/securefiles#POST"
          },
          groupingKey = "secureFiles"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.securefiles_manage",
          title = "Secure Files (read, create, and manage)",
          description = "Grants the ability to read, create, and manage secure files.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.securefiles_write",
          patterns = new string[2]
          {
            "/DefaultCollection/*/_apis/distributedtask/securefiles#GET+POST+PUT+DELETE",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions/securefile#GET+PATCH"
          },
          groupingKey = "secureFiles"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.machinegroup_manage",
          title = "Deployment group (read, manage)",
          description = "Provides ability to manage deployment group and agent pools",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.agentpools_manage",
          patterns = new string[2]
          {
            "/DefaultCollection/*/_apis/distributedtask/machinegroups/*#GET+POST+DELETE+PATCH",
            "/DefaultCollection/*/_apis/distributedtask/deploymentgroups/*#GET+POST+DELETE+PATCH+PUT"
          },
          groupingKey = "machineGroup"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.live_updates",
          title = "Live Updates",
          description = "Grants the ability to receive live updates using signalr.",
          availability = AuthorizationScopeAvailability.None,
          patterns = new string[2]
          {
            "/signalr#GET+POST",
            "/_apis/*/signalr#GET+POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.taskgroups",
          title = FrameworkResources.ScopeTskGrpsRead(),
          description = "Grants the ability to read task groups",
          availability = AuthorizationScopeAvailability.Deprecated,
          inheritsFrom = "vso.base",
          patterns = new string[2]
          {
            "/DefaultCollection/*/_apis/distributedtask/taskgroups#GET",
            "/DefaultCollection/*/_apis/distributedtask/taskgroups/*/revisions#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.taskgroups_add",
          title = FrameworkResources.ScopeTskGrpsReadCreate(),
          description = "Grants the ability to read and create task groups",
          availability = AuthorizationScopeAvailability.Deprecated,
          inheritsFrom = "vso.taskgroups",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/distributedtask/taskgroups#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.taskgroups_read",
          title = "Task Groups (read)",
          description = "Grants the ability to read task groups",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[2]
          {
            "/DefaultCollection/*/_apis/distributedtask/taskgroups#GET",
            "/DefaultCollection/*/_apis/distributedtask/taskgroups/*/revisions#GET"
          },
          groupingKey = "taskGroups"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.taskgroups_write",
          title = "Task Groups (read, create)",
          description = "Grants the ability to read and create task groups",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.taskgroups_read",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/distributedtask/taskgroups#POST"
          },
          groupingKey = "taskGroups"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.taskgroups_manage",
          title = "Task Groups (read, create and manage)",
          description = "Grants the ability to read, create and manage taskgroups.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.taskgroups_write",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/distributedtask/taskgroups#GET+POST+PUT+DELETE"
          },
          groupingKey = "taskGroups"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.variablegroups_read",
          title = "Variable Groups (read)",
          description = "Grants the ability to read variable groups",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/distributedtask/variablegroups#GET"
          },
          groupingKey = "variableGroups"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.variablegroups_write",
          title = "Variable Groups (read, create)",
          description = "Grants the ability to read and create variable groups",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.variablegroups_read",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/distributedtask/variablegroups#POST"
          },
          groupingKey = "variableGroups"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.variablegroups_manage",
          title = "Variable Groups (read, create and manage)",
          description = "Grants the ability to read, create and manage variablegroups.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.variablegroups_write",
          patterns = new string[3]
          {
            "/DefaultCollection/_apis/distributedtask/variablegroups#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/distributedtask/variablegroups#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions/variablegroup#GET+PATCH"
          },
          groupingKey = "variableGroups"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.serviceendpoint",
          title = "Service Endpoints (read)",
          description = "Grants the ability to read service endpoints.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.profile",
          patterns = new string[6]
          {
            "/DefaultCollection/*/_apis/distributedtask/serviceendpoints#GET",
            "/DefaultCollection/_apis/distributedtask/serviceendpointtypes#GET",
            "/DefaultCollection/*/_apis/serviceendpoint/endpoints#GET",
            "/DefaultCollection/_apis/serviceendpoint/endpoints#GET",
            "/DefaultCollection/_apis/serviceendpoint/types#GET",
            "/DefaultCollection/*/_apis/serviceendpoint/*/executionhistory#GET"
          },
          groupingKey = "serviceEndpoints"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.serviceendpoint_query",
          title = "Service Endpoints (read and query)",
          description = "Grants the ability to read and query service endpoints.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.serviceendpoint",
          patterns = new string[4]
          {
            "/DefaultCollection/*/_apis/distributedtask/serviceendpointproxy#POST",
            "/DefaultCollection/*/_apis/serviceendpoint/endpointproxy#POST",
            "/_apis/distributedtask/serviceendpointproxy#POST",
            "/_apis/serviceendpoint/endpointproxy#POST"
          },
          groupingKey = "serviceEndpoints"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.serviceendpoint_manage",
          title = "Service Endpoints (read, query and manage)",
          description = "Grants the ability to read, query and manage service endpoints.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.serviceendpoint_query",
          patterns = new string[6]
          {
            "/DefaultCollection/*/_apis/distributedtask/serviceendpoints#GET+POST+PUT+DELETE",
            "/DefaultCollection/*/_apis/serviceendpoint/endpoints#GET+POST+PUT+DELETE",
            "/DefaultCollection/_apis/serviceendpoint/endpoints#GET+POST+PUT+PATCH+DELETE",
            "/_apis/serviceendpoint/endpoints#GET+POST+PUT+DELETE",
            "/_apis/distributedtask/serviceendpoints#GET+POST+PUT+DELETE",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions/endpoint#GET+PATCH"
          },
          groupingKey = "serviceEndpoints"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.drop",
          title = "Build Drop (read)",
          description = "Grants the ability to list and download drops.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Drop",
          inheritsFrom = "vso.profile",
          patterns = new string[19]
          {
            "/DefaultCollection/_apis/drop/client#HEAD",
            "/DefaultCollection/_apis/blob/blobs/*#GET",
            "/DefaultCollection/_apis/blob/blobsbatch#POST",
            "/DefaultCollection/_apis/dedup/chunks/*#GET",
            "/DefaultCollection/_apis/dedup/nodes/*#GET",
            "/DefaultCollection/_apis/dedup/urls/*#POST",
            "/DefaultCollection/_apis/domains#GET",
            "/DefaultCollection/_apis/domains/*/blob/*#GET",
            "/DefaultCollection/_apis/domains/*/blob/blobs/*#GET",
            "/DefaultCollection/_apis/domains/*/blob/blobsbatch#POST",
            "/DefaultCollection/_apis/domains/*/dedup/chunks/*#GET",
            "/DefaultCollection/_apis/domains/*/dedup/nodes/*#GET",
            "/DefaultCollection/_apis/domains/*/dedup/urls/*#GET+POST",
            "/DefaultCollection/_apis/drop/drop#OPTIONS",
            "/DefaultCollection/_apis/drop/drop/*#GET",
            "/DefaultCollection/_apis/drop/fetch/*#GET+POST",
            "/DefaultCollection/_apis/drop/drops/*#OPTIONS+GET",
            "/DefaultCollection/_apis/drop/manifests/*#OPTIONS+GET",
            "/_apis/clienttools/*/settings#GET"
          },
          groupingKey = "drop"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.drop_write",
          title = "Build Drop (read and write)",
          description = "Grants the ability to list, download and upload drops.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Drop",
          inheritsFrom = "vso.drop",
          patterns = new string[11]
          {
            "/DefaultCollection/_apis/blob/blobs#OPTIONS",
            "/DefaultCollection/_apis/blob/blobs/*#POST",
            "/DefaultCollection/_apis/dedup/chunks/*#PUT+POST",
            "/DefaultCollection/_apis/dedup/nodes/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/blob/blobs#OPTIONS",
            "/DefaultCollection/_apis/domains/*/blob/blobs/*#POST",
            "/DefaultCollection/_apis/domains/*/dedup/chunks/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/dedup/nodes/*#PUT+POST",
            "/DefaultCollection/_apis/drop/drop/*#PUT+POST+PATCH",
            "/DefaultCollection/_apis/drop/drops/*#OPTIONS+PUT+POST+PATCH",
            "/DefaultCollection/_apis/drop/manifests/*#OPTIONS+POST"
          },
          groupingKey = "drop"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.drop_manage",
          title = "Build Drop (read, write, and manage)",
          description = "Grants the ability to list, download, upload and delete drops.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Drop",
          inheritsFrom = "vso.drop_write",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/drop/drop/*#DELETE",
            "/DefaultCollection/_apis/drop/drops/*#DELETE"
          },
          groupingKey = "drop"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.blobstore",
          title = "Build Blobstore (read)",
          description = "Grants the ability to download blobs.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Blobstore",
          inheritsFrom = "vso.profile",
          patterns = new string[15]
          {
            "/DefaultCollection/_apis/blob/blobs/*#GET",
            "/DefaultCollection/_apis/blob/blobsbatch#POST",
            "/DefaultCollection/_apis/dedup/chunks/*#GET",
            "/DefaultCollection/_apis/dedup/nodes/*#GET",
            "/DefaultCollection/_apis/dedup/urls/*#POST",
            "/DefaultCollection/_apis/domains#GET",
            "/DefaultCollection/_apis/domains/*/blob/*#GET",
            "/DefaultCollection/_apis/domains/*/blob/blobs/*#GET",
            "/DefaultCollection/_apis/domains/*/blob/blobsbatch#POST",
            "/DefaultCollection/_apis/domains/*/dedup/chunks/*#GET",
            "/DefaultCollection/_apis/domains/*/dedup/nodes/*#GET",
            "/DefaultCollection/_apis/domains/*/dedup/urls/*#GET+POST",
            "/DefaultCollection/_apis/domains/*/dedup/blobs/*#GET",
            "/DefaultCollection/_apis/domains/*/dedup/sessions/*#GET",
            "/DefaultCollection/_apis/domains/*/dedup/sessions/*/parts#GET"
          },
          groupingKey = "blobstore"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.blobstore_write",
          title = "Build Blobstore (read and write)",
          description = "Grants the ability to download and upload blobs.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Blobstore",
          inheritsFrom = "vso.blobstore",
          patterns = new string[12]
          {
            "/DefaultCollection/_apis/blob/blobs#OPTIONS",
            "/DefaultCollection/_apis/blob/blobs/*#POST",
            "/DefaultCollection/_apis/dedup/chunks/*#PUT+POST",
            "/DefaultCollection/_apis/dedup/nodes/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/blob/blobs#OPTIONS",
            "/DefaultCollection/_apis/domains/*/blob/blobs/*#POST",
            "/DefaultCollection/_apis/domains/*/dedup/chunks/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/dedup/nodes/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/dedup/blobs#POST",
            "/DefaultCollection/_apis/domains/*/dedup/sessions#POST",
            "/DefaultCollection/_apis/domains/*/dedup/sessions/*#PATCH",
            "/DefaultCollection/_apis/domains/*/dedup/sessions/*/parts#PUT"
          },
          groupingKey = "blobstore"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.blobstore_manage",
          title = "Build Blobstore (read,write, and manage)",
          description = "Grants the ability to download, upload, and delete blobs.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Blobstore",
          inheritsFrom = "vso.blobstore",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/domains/*/dedup/sessions/*#DELETE",
            "/DefaultCollection/_apis/domains#GET+POST"
          },
          groupingKey = "blobstore"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.buildcache",
          title = "Build Cache (read)",
          description = "Grants the ability to access build cache artifacts.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.BuildCache",
          inheritsFrom = "vso.profile",
          patterns = new string[19]
          {
            "/DefaultCollection/_apis/buildcache/fingerprintselector/*#GET",
            "/DefaultCollection/_apis/buildcache/contentbag#OPTIONS",
            "/DefaultCollection/_apis/buildcache/contentbag/*#GET",
            "/DefaultCollection/_apis/buildcache/cachedeterminismguid/*#GET",
            "/DefaultCollection/_apis/buildcache/selector/*#GET",
            "/DefaultCollection/_apis/buildcache/blobselector/*#GET",
            "/DefaultCollection/_apis/buildcache/contenthashlist/*#OPTIONS+GET",
            "/DefaultCollection/_apis/buildcache/blobcontenthashlist/*#OPTIONS+GET",
            "/DefaultCollection/_apis/blob/referencesbatch#POST",
            "/DefaultCollection/_apis/blob/blobs#OPTIONS",
            "/DefaultCollection/_apis/blob/blobs/*#GET",
            "/DefaultCollection/_apis/blob/blobsbatch#POST",
            "/DefaultCollection/_apis/domains#GET",
            "/DefaultCollection/_apis/domains/*/blob/*#GET",
            "/DefaultCollection/_apis/domains/*/blob/blobs/*#GET",
            "/DefaultCollection/_apis/domains/*/blob/blobsbatch#POST",
            "/DefaultCollection/_apis/dedup/chunks/*#GET",
            "/DefaultCollection/_apis/dedup/nodes/*#GET",
            "/DefaultCollection/_apis/dedup/urls/*#POST"
          },
          groupingKey = "buildCache"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.buildcache_write",
          title = "Build Cache (read and write)",
          description = "Grants the ability to access and update build cache artifacts.",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.BuildCache",
          inheritsFrom = "vso.buildcache",
          patterns = new string[10]
          {
            "/DefaultCollection/_apis/buildcache/contentbag/*#POST",
            "/DefaultCollection/_apis/buildcache/contenthashlist/*#POST",
            "/DefaultCollection/_apis/buildcache/blobcontenthashlist/*#POST",
            "/DefaultCollection/_apis/buildcache/incorporateStrongFingeprint/*#PUT",
            "/DefaultCollection/_apis/blob/blobs/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/blob/blobs/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/dedup/chunks/*#PUT+POST",
            "/DefaultCollection/_apis/domains/*/dedup/nodes/*#PUT+POST",
            "/DefaultCollection/_apis/dedup/chunks/*#PUT+POST",
            "/DefaultCollection/_apis/dedup/nodes/*#PUT+POST"
          },
          groupingKey = "buildCache"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.authorization_grant",
          title = "Authorization Grant",
          description = "Special scope issued to delegated authorization grants which are used in the OAuth2 access token exchange flow. This scope is used to differentiate authorization grants from PATs",
          availability = AuthorizationScopeAvailability.System,
          patterns = new string[0]
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.project",
          title = "Project and team (read)",
          description = "Grants the ability to read projects and teams.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[4]
          {
            "/DefaultCollection/_apis/projects#GET",
            "/DefaultCollection/_apis/projects/*/teams#GET",
            "/DefaultCollection/_apis/teams#GET",
            "/DefaultCollection/*/_apis/build/generalsettings#GET"
          },
          groupingKey = "projectAndTeam"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.project_write",
          title = "Project and team (read and write)",
          description = "Grants the ability to read and update projects and teams.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.project",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/projects#PATCH",
            "/DefaultCollection/*/_apis/build/generalsettings#PATCH"
          },
          groupingKey = "projectAndTeam"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.project_manage",
          title = "Project and team (read, write and manage)",
          description = "Grants the ability to create, read, update, and delete projects and teams.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.project_write",
          patterns = ((IEnumerable<string>) AuthorizationScopeDefinitions.ClientOmBasePatterns).Concat<string>((IEnumerable<string>) new string[7]
          {
            "/DefaultCollection/_apis/projects#POST+PUT+DELETE",
            "/Services/*/CommonStructureService.asmx#POST",
            "/*/Services/*/CommonStructureService.asmx#POST",
            "/TeamFoundation/Administration/*/CatalogService.asmx#POST",
            "/*/TeamFoundation/Administration/*/CatalogService.asmx#POST",
            "/*/*/TeamFoundation/Administration/*/CatalogService.asmx#POST",
            "/*/*/*/TeamFoundation/Administration/*/CatalogService.asmx#POST"
          }).ToArray<string>(),
          groupingKey = "projectAndTeam"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.symbols",
          title = "Symbols (read)",
          description = "Grants the ability to read symbols.",
          availability = AuthorizationScopeAvailability.Public,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Symbol",
          inheritsFrom = "vso.profile",
          patterns = new string[7]
          {
            "/DefaultCollection/_apis/symbol/availability#GET",
            "/DefaultCollection/_apis/symbol/client#HEAD+GET",
            "/DefaultCollection/_apis/symbol/symsrv#OPTIONS",
            "/DefaultCollection/_apis/symbol/symsrv/*#GET",
            "/DefaultCollection/_apis/symbol/debugentries#OPTIONS",
            "/DefaultCollection/_apis/symbol/debugentries/*#GET",
            "/DefaultCollection/_apis/symbol/requests#OPTIONS+GET"
          },
          groupingKey = "symbols"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.symbols_write",
          title = "Symbols (read and write)",
          description = "Grants the ability to read and write symbols.",
          availability = AuthorizationScopeAvailability.Public,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Symbol",
          inheritsFrom = "vso.symbols",
          patterns = new string[4]
          {
            "/DefaultCollection/_apis/symbol/requests#POST",
            "/DefaultCollection/_apis/symbol/requests/*#POST+PATCH",
            "/DefaultCollection/_apis/blob/blobs#OPTIONS",
            "/DefaultCollection/_apis/blob/blobs/*#POST"
          },
          groupingKey = "symbols"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.symbols_manage",
          title = "Symbols (read, write and manage)",
          description = "Grants the ability to read, write and manage symbols.",
          availability = AuthorizationScopeAvailability.Public,
          availabilityFeatureFlag = "Artifact.AuthorizationScope.Symbol",
          inheritsFrom = "vso.symbols_write",
          patterns = new string[1]
          {
            "/DefaultCollection/_apis/symbol/requests/*#DELETE"
          },
          groupingKey = "symbols"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.analytics",
          title = "Analytics (read)",
          description = "Grants the ability to query analytics.",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[6]
          {
            "/DefaultCollection/_odata/*#GET",
            "/DefaultCollection/*/_odata/*#GET",
            "/_odata/*/$batch#POST",
            "/_odata/$batch#POST",
            "/DefaultCollection/*/_apis/analytics/views#GET",
            "/DefaultCollection/*/_apis/projectanalysis/languagemetrics#GET"
          },
          groupingKey = "analytics"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.gh_analytics",
          title = "Github Analytics",
          description = "Grants the ability for github to run Analytics scenarios.",
          availability = AuthorizationScopeAvailability.Conditional,
          inheritsFrom = "vso.analytics",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/analytics/stage#GET+POST+PATCH+DELETE",
            "/DefaultCollection/_apis/analytics/stage/*/*/Invalid#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.identitypicker",
          title = "Identity Picker (read)",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants the ability to interact with federated identities.",
          inheritsFrom = "vso.base",
          patterns = new string[6]
          {
            "/_apis/IdentityPicker/identities#POST",
            "/_apis/IdentityPicker/identities/*/avatar#GET",
            "/_apis/IdentityPicker/identities#GET",
            "/DefaultCollection/_apis/IdentityPicker/identities#POST",
            "/DefaultCollection/_apis/IdentityPicker/identities/*/avatar#GET",
            "/DefaultCollection/_apis/IdentityPicker/identities#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.dashboards",
          title = "Team dashboards (read)",
          description = "Grants the ability to read team dashboard information",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[7]
          {
            "/DefaultCollection/*/*/_apis/dashboard/dashboards#HEAD+GET",
            "/DefaultCollection/*/*/_apis/dashboard/dashboards/*/widgets#HEAD+GET",
            "/DefaultCollection/*/*/_apis/dashboard/widgettypes#HEAD+GET",
            "/DefaultCollection/*/_apis/dashboard/dashboards#HEAD+GET",
            "/DefaultCollection/*/_apis/dashboard/dashboards/*/widgets#HEAD+GET",
            "/DefaultCollection/*/_apis/dashboard/widgettypes#HEAD+GET",
            "/DefaultCollection/*/_apis/reporting/transformquery#POST"
          },
          groupingKey = "teamDashboards"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.dashboards_manage",
          title = "Team dashboards (manage)",
          description = "Grants the ability to manage team dashboard information",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.dashboards",
          patterns = new string[4]
          {
            "/DefaultCollection/*/*/_apis/dashboard/dashboards#HEAD+GET+POST+PUT+DELETE",
            "/DefaultCollection/*/*/_apis/dashboard/dashboards/*/widgets#HEAD+GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/dashboard/dashboards#HEAD+GET+POST+PUT+DELETE",
            "/DefaultCollection/*/_apis/dashboard/dashboards/*/widgets#HEAD+GET+POST+PUT+PATCH+DELETE"
          },
          groupingKey = "teamDashboards"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.connected_server",
          title = "Connected Server",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to access endpoints needed from an onprem connected server",
          inheritsFrom = "vso.base",
          patterns = new string[2]
          {
            "/_apis/Commerce/CommercePackage#GET",
            "/_apis/Commerce/CommercePackage/*#GET"
          },
          groupingKey = "connectedServer"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.notification",
          title = "Notifications (read)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Provides read access to subscriptions and event metadata, including filterable field values.",
          inheritsFrom = "vso.profile",
          patterns = new string[19]
          {
            "/_apis/notification/EventTypes#GET",
            "/_apis/notification/EventTypes/*/fieldValuesQuery#POST",
            "/_apis/notification/Subscriptions#GET",
            "/_apis/notification/SubscriptionQuery#POST",
            "/_apis/notification/Follows#GET",
            "/_apis/notification/SubscriptionTemplates#GET",
            "/_apis/notification/StatisticsQuery#POST",
            "/_apis/notification/subscribers#GET",
            "/_apis/notification/EventTransforms#POST",
            "/DefaultCollection/_apis/notification/EventTypes#GET",
            "/DefaultCollection/_apis/notification/EventTypes/*/fieldValuesQuery#POST",
            "/DefaultCollection/_apis/notification/Subscriptions#GET",
            "/DefaultCollection/_apis/notification/SubscriptionQuery#POST",
            "/DefaultCollection/_apis/notification/Follows#GET",
            "/DefaultCollection/_apis/notification/SubscriptionTemplates#GET",
            "/DefaultCollection/_apis/notification/StatisticsQuery#POST",
            "/DefaultCollection/_apis/notification/subscribers#GET",
            "/DefaultCollection/_apis/notification/EventTransforms#POST",
            "/DefaultCollection/_apis/notification/settings#GET"
          },
          groupingKey = "notifications"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.notification_write",
          title = "Notifications (write)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Provides read/write access to subscriptions and read access to event metadata, including filterable field values.",
          inheritsFrom = "vso.notification",
          patterns = new string[9]
          {
            "/_apis/notification/Subscriptions#GET+PATCH+POST+DELETE+PUT",
            "/DefaultCollection/_apis/notification/Subscriptions#GET+PATCH+POST+DELETE+PUT",
            "/_apis/notification/SubscriptionEvaluationRequests#GET+POST",
            "/DefaultCollection/_apis/notification/SubscriptionEvaluationRequests#GET+POST",
            "/_apis/notification/Follows#GET+POST+DELETE",
            "/DefaultCollection/_apis/notification/Follows#GET+POST+DELETE",
            "/_apis/notification/subscribers#GET+PATCH",
            "/DefaultCollection/_apis/notification/subscribers#GET+PATCH",
            "/DefaultCollection/_apis/notification/settings#PATCH"
          },
          groupingKey = "notifications"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.notification_manage",
          title = "Notifications (manage)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Provides read, write, and management access to subscriptions and read access to event metadata, including filterable field values.",
          inheritsFrom = "vso.notification_write",
          patterns = new string[2]
          {
            "/_apis/notification/BatchNotificationOperations#POST",
            "/DefaultCollection/_apis/notification/BatchNotificationOperations#POST"
          },
          groupingKey = "notifications"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.notification_publish",
          title = "Notifications (publish events)",
          availability = AuthorizationScopeAvailability.None,
          description = "Provides ability to fire events",
          inheritsFrom = "vso.profile",
          patterns = new string[2]
          {
            "/_apis/notification/Events#POST",
            "/DefaultCollection/_apis/notification/Events#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.settings",
          title = "Settings (read)",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants read access to user and account, project, and other scoped settings.",
          inheritsFrom = "vso.profile",
          patterns = new string[3]
          {
            "/_apis/Settings/*#GET",
            "/DefaultCollection/_apis/Settings/*#GET",
            "/_api/_account/GetAccountSettings#GET"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.settings_write",
          title = "Settings (read and write)",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants read and write access to user and account, project, and other scoped settings.",
          inheritsFrom = "vso.profile",
          patterns = new string[2]
          {
            "/_apis/Settings/*#GET+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/Settings/*#GET+PUT+PATCH+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.features",
          title = "Features (read)",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants read access to obtain feature states.",
          inheritsFrom = "vso.profile",
          patterns = new string[4]
          {
            "/_apis/FeatureManagement/*#GET",
            "/DefaultCollection/_apis/FeatureManagement/*#GET",
            "/_apis/FeatureManagement/FeatureStatesQuery/*#POST",
            "/DefaultCollection/_apis/FeatureManagement/FeatureStatesQuery/*#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.features_write",
          title = "Features (read and write)",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants read and write access to manage feature states.",
          inheritsFrom = "vso.features",
          patterns = new string[2]
          {
            "/_apis/FeatureManagement/*#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/FeatureManagement/*#GET+POST+PUT+PATCH+DELETE"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.proxy",
          title = "Proxy Service scope",
          availability = AuthorizationScopeAvailability.None,
          description = "Provides access to the proxy service (SOAP).",
          inheritsFrom = "vso.base",
          patterns = new string[25]
          {
            "/Services/v4.0/item.ashx",
            "/Services/v4.0/FileHandlerService.asmx#POST",
            "/TeamFoundation/Administration/v3.0/LocationService.asmx#POST",
            "/Services/v3.0/LocationService.asmx#POST",
            "/DefaultCollection/Services/v4.0/item.ashx",
            "/DefaultCollection/Services/v4.0/FileHandlerService.asmx#POST",
            "/DefaultCollection/Services/v3.0/LocationService.asmx#POST",
            "/*/*/*/_git/*#GET",
            "/*/*/_git/*#GET",
            "/*/_git/*#GET",
            "/*/*/*/_git/_full/*#GET",
            "/*/*/_git/_full/*#GET",
            "/*/_git/_full/*#GET",
            "/*/*/*/_git/*/git-upload-pack#POST",
            "/*/*/_git/*/git-upload-pack#POST",
            "/*/_git/*/git-upload-pack#POST",
            "/*/*/*/_git/_full/*/git-upload-pack#POST",
            "/*/*/_git/_full/*/git-upload-pack#POST",
            "/*/_git/_full/*/git-upload-pack#POST",
            "/*/*/*/_git/*/gvfs/*#POST",
            "/*/*/_git/*/gvfs/*#POST",
            "/*/_git/*/gvfs/*#POST",
            "/*/*/*/_git/_full/*/gvfs/*#POST",
            "/*/*/_git/_full/*/gvfs/*#POST",
            "/*/_git/_full/*/gvfs/*#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.graph",
          title = "Graph (read)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read user, group, scope and group membership information",
          inheritsFrom = "vso.base",
          patterns = new string[28]
          {
            "/_apis/graph/descriptors/*#GET",
            "/_apis/graph/groups/*#GET",
            "/_apis/graph/requestaccess#POST",
            "/_apis/graph/subjectlookup#POST",
            "/_apis/graph/subjectquery#POST",
            "/_apis/graph/memberships/*#GET",
            "/_apis/graph/memberships/*/*#GET+HEAD",
            "/_apis/graph/membershipstates/*#GET",
            "/_apis/graph/storagekeys/*#GET",
            "/_apis/graph/scopes/*#GET",
            "/_apis/graph/subjects/*#GET",
            "/_apis/graph/users/*#GET",
            "/_apis/graph/serviceprincipals/*#GET",
            "/_apis/graph/members/*#GET",
            "/*/_apis/graph/descriptors/*#GET",
            "/*/_apis/graph/groups/*#GET",
            "/*/_apis/graph/requestaccess#POST",
            "/*/_apis/graph/subjectlookup#POST",
            "/*/_apis/graph/subjectquery#POST",
            "/*/_apis/graph/memberships/*#GET",
            "/*/_apis/graph/memberships/*/*#GET+HEAD",
            "/*/_apis/graph/membershipstates/*#GET",
            "/*/_apis/graph/storagekeys/*#GET",
            "/*/_apis/graph/scopes/*#GET",
            "/*/_apis/graph/subjects/*#GET",
            "/*/_apis/graph/users/*#GET",
            "/*/_apis/graph/serviceprincipals/*#GET",
            "/*/_apis/graph/members/*#GET"
          },
          groupingKey = "graph"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.graph_manage",
          title = "Graph (manage)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read user, group, scope and group membership information, and to add users, groups and manage group memberships",
          inheritsFrom = "vso.graph",
          patterns = new string[13]
          {
            "/_apis/graph/groups/*#POST+PATCH+DELETE",
            "/_apis/graph/memberships/*/*#PUT+DELETE",
            "/_apis/graph/scopes/*#POST+PATCH+DELETE",
            "/_apis/graph/subjects/*#PUT+DELETE",
            "/_apis/graph/users/*#POST+PATCH+DELETE",
            "/_apis/graph/serviceprincipals/*#POST+PATCH+DELETE",
            "/*/_apis/graph/groups/*#POST+PATCH+DELETE",
            "/*/_apis/graph/memberships/*/*#PUT+DELETE",
            "/*/_apis/graph/scopes/*#POST+PATCH+DELETE",
            "/*/_apis/graph/subjects/*#PUT+DELETE",
            "/*/_apis/graph/users/*#POST+PATCH+DELETE",
            "/*/_apis/graph/serviceprincipals/*#POST+PATCH+DELETE",
            "/*/_apis/graph/resolvedisconnectedusers/*#POST"
          },
          groupingKey = "graph"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.graph_write",
          title = "Graph (write)",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants the ability to read user, group, scope and group membership information, and to add users, groups and manage group memberships",
          inheritsFrom = "vso.graph",
          patterns = new string[9]
          {
            "/_apis/graph/groups/*#POST+PATCH+DELETE",
            "/_apis/graph/memberships/*/*#PUT+DELETE",
            "/_apis/graph/scopes/*#POST+PATCH+DELETE",
            "/_apis/graph/users/*#POST+DELETE",
            "/*/_apis/graph/groups/*#POST+PATCH+DELETE",
            "/*/_apis/graph/memberships/*/*#PUT+DELETE",
            "/*/_apis/graph/scopes/*#POST+PATCH+DELETE",
            "/*/_apis/graph/users/*#POST+DELETE",
            "/*/_apis/graph/resolvedisconnectedusers/*#POST"
          },
          groupingKey = "graph"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.security_manage",
          title = "Security (manage)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read, write, and manage security permissions.",
          inheritsFrom = "vso.base",
          patterns = ((IEnumerable<string>) AuthorizationScopeDefinitions.ClientOmBasePatterns).Concat<string>((IEnumerable<string>) new string[17]
          {
            "/_apis/SecurityNamespaces#POST+PATCH",
            "/_apis/AccessControlLists#POST+DELETE",
            "/_apis/AccessControlEntries#POST+DELETE",
            "/_apis/Permissions#DELETE",
            "/DefaultCollection/_apis/SecurityNamespaces#POST+PATCH",
            "/DefaultCollection/_apis/AccessControlLists#POST+DELETE",
            "/DefaultCollection/_apis/AccessControlEntries#POST+DELETE",
            "/DefaultCollection/_apis/Permissions#DELETE",
            "/DefaultCollection/_apis/permissionsreport#GET+POST",
            "/_apis/securityroles/scopes/*/roleassignments/resources/*#POST+PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/securityroles/scopes/*/roleassignments/resources/*#POST+PUT+PATCH+DELETE",
            "/Services/*/SecurityService.asmx#POST",
            "/*/Services/*/SecurityService.asmx#POST",
            "/Services/*/GroupSecurityService.asmx#POST",
            "/*/Services/*/GroupSecurityService.asmx#POST",
            "/DefaultCollection/_api/_security/DisplayPermissions#GET",
            "/DefaultCollection/*/_api/_security/DisplayPermissions#GET"
          }).ToArray<string>(),
          groupingKey = "security"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.memberentitlementmanagement",
          title = "MemberEntitlement Management (read)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read users, their licenses as well as projects and extensions they can access",
          inheritsFrom = "vso.base",
          patterns = new string[16]
          {
            "/_apis/MemberEntitlements/*#GET",
            "/_apis/MemberEntitlements#GET",
            "/_apis/GroupEntitlements/*#GET",
            "/_apis/GroupEntitlements#GET",
            "/_apis/UserEntitlements/*#GET",
            "/_apis/ServicePrincipalEntitlements/*#GET",
            "/_apis/UserEntitlements#GET",
            "/_apis/UserEntitlementSummary#GET",
            "/DefaultCollection/_apis/MemberEntitlements/*#GET",
            "/DefaultCollection/_apis/MemberEntitlements#GET",
            "/DefaultCollection/_apis/GroupEntitlements/*#GET",
            "/DefaultCollection/_apis/GroupEntitlements#GET",
            "/DefaultCollection/_apis/UserEntitlements/*#GET",
            "/DefaultCollection/_apis/ServicePrincipalEntitlements/*#GET",
            "/DefaultCollection/_apis/UserEntitlements#GET",
            "/DefaultCollection/_apis/UserEntitlementSummary#GET"
          },
          groupingKey = "memberEntitlementManagement"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.memberentitlementmanagement_write",
          title = "MemberEntitlement Management (write)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to manage users, their licenses as well as projects and extensions they can access",
          inheritsFrom = "vso.memberentitlementmanagement",
          patterns = new string[16]
          {
            "/_apis/MemberEntitlements/*#PATCH+DELETE",
            "/_apis/MemberEntitlements#PATCH+POST",
            "/_apis/GroupEntitlements/*#PUT+PATCH+DELETE",
            "/_apis/GroupEntitlements#POST",
            "/_apis/UserEntitlements/*#PATCH+DELETE",
            "/_apis/ServicePrincipalEntitlements/*#PATCH+DELETE",
            "/_apis/UserEntitlements#PATCH+POST",
            "/_apis/ServicePrincipalEntitlements#PATCH+POST",
            "/DefaultCollection/_apis/MemberEntitlements/*#PATCH+DELETE",
            "/DefaultCollection/_apis/MemberEntitlements#PATCH+POST",
            "/DefaultCollection/_apis/GroupEntitlements/*#PUT+PATCH+DELETE",
            "/DefaultCollection/_apis/GroupEntitlements#POST",
            "/DefaultCollection/_apis/UserEntitlements/*#PATCH+DELETE",
            "/DefaultCollection/_apis/ServicePrincipalEntitlements/*#PATCH+DELETE",
            "/DefaultCollection/_apis/UserEntitlements#PATCH+POST",
            "/DefaultCollection/_apis/ServicePrincipalEntitlements#PATCH+POST"
          },
          groupingKey = "memberEntitlementManagement"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.wiki",
          title = "Wiki (read)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read wikis, wiki pages and wiki attachments. Also grants the ability to search wiki pages.",
          inheritsFrom = "vso.base",
          patterns = new string[7]
          {
            "/DefaultCollection/_apis/wiki/wikis#GET",
            "/DefaultCollection/*/_apis/wiki/wikis#GET",
            "/DefaultCollection/_apis/wiki/wikis/*/pagesbatch#POST",
            "/DefaultCollection/*/_apis/wiki/wikis/*/pagesbatch#POST",
            "/_apis/search/wikiSearchResults#POST",
            "/DefaultCollection/_apis/search/wikiSearchResults#POST",
            "/DefaultCollection/*/_apis/search/wikiSearchResults#POST"
          },
          groupingKey = "wiki"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.wiki_write",
          title = "Wiki (read and write)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read, create and updates wikis, wiki pages and wiki attachments.",
          inheritsFrom = "vso.wiki",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/wiki/wikis#GET+POST+PUT+PATCH+DELETE",
            "/DefaultCollection/*/_apis/wiki/wikis#GET+POST+PUT+PATCH+DELETE"
          },
          groupingKey = "wiki"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.teams_integration",
          title = "Teams Integration Config (read)",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants the ability to access the configuration page for teams integration.",
          patterns = new string[2]
          {
            "/_teams/configure#GET",
            "/_teams/configure#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.notification_diagnostics",
          title = "Notifications (diagnostics)",
          availability = AuthorizationScopeAvailability.Public,
          description = "Provides access to notification-related diagnostic logs and provides the ability to enable diagnostics for individual subscriptions.",
          inheritsFrom = "vso.notification",
          patterns = new string[4]
          {
            "/_apis/notification/subscriptions/*/diagnostics#GET+PUT",
            "/DefaultCollection/_apis/notification/subscriptions/*/diagnostics#GET+PUT",
            "/_apis/notification/DiagnosticLogs#GET",
            "/DefaultCollection/_apis/notification/DiagnosticLogs#GET"
          },
          groupingKey = "notifications"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.workitemsearch",
          title = "Work item search (read)",
          availability = AuthorizationScopeAvailability.Deprecated,
          description = "Grants the ability to search work items they can access across all projects",
          inheritsFrom = "vso.base",
          patterns = new string[2]
          {
            "/_apis/search/workItemQueryResults#POST",
            "/DefaultCollection/_apis/search/workItemQueryResults#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.codesearch",
          title = "Code search (read)",
          availability = AuthorizationScopeAvailability.Deprecated,
          description = "Grants the ability to search code across all projects which they can access",
          inheritsFrom = "vso.base",
          patterns = new string[4]
          {
            "/_apis/search/codeQueryResults#POST",
            "/DefaultCollection/_apis/search/codeQueryResults#POST",
            "/_apis/search/codeAdvancedQueryResults#POST",
            "/DefaultCollection/_apis/search/codeAdvancedQueryResults#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.tokenadministration",
          title = "Token Administration",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to manage (view and revoke) existing tokens to organization administrators",
          inheritsFrom = "vso.base",
          patterns = new string[6]
          {
            "/DefaultCollection/_apis/tokenadmin/revocations#POST",
            "/DefaultCollection/_apis/tokenadmin/revocationrules#POST",
            "/DefaultCollection/_apis/tokenadmin/personalaccesstokens/*#GET",
            "/_apis/tokenadministration/tokenrevocations#POST",
            "/_apis/tokenadministration/tokenlistglobalidentities#POST",
            "/_apis/tokenadministration/tokenpersonalaccesstokens/*#POST"
          },
          groupingKey = "tokenadmin"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.tokens",
          title = "Delegated Authorization Tokens",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to manage delegated authorization tokens to users",
          inheritsFrom = "vso.base",
          patterns = new string[22]
          {
            "/_apis/token/AppTokenPairs#POST",
            "/_apis/token/AadUserTokens#POST",
            "/_apis/token/AccessTokens/*#POST+GET",
            "/_apis/token/AppSessionTokens#POST",
            "/_apis/token/SessionTokens#GET+POST+DELETE",
            "/_apis/token/AadAppTokens#GET",
            "/_apis/delegatedauth/Registration#GET+PUT+POST+DELETE",
            "/_apis/delegatedauth/RegistrationSecret/*#GET",
            "/_apis/delegatedauth/Authorizations#GET+POST",
            "/_apis/delegatedauth/HostAuthorization#GET+DELETE+POST",
            "/_apis/oauth2/token#POST",
            "/_apis/tokenissue/AppTokenPairs#POST",
            "/_apis/tokenissue/AadUserTokens#POST",
            "/_apis/tokenissue/AccessTokens/*#POST",
            "/_apis/tokenissue/AppSessionTokens#POST",
            "/_apis/tokenissue/SessionTokens#GET+PUT+POST+DELETE",
            "/_apis/tokenissue/AadAppTokens#GET",
            "/_apis/tokenauth/Registration#GET+PUT+POST+DELETE",
            "/_apis/tokenauth/RegistrationSecret/*#GET",
            "/_apis/tokenauth/Authorizations#GET+POST+DELETE",
            "/_apis/tokenauth/HostAuthorization#GET+DELETE+PATCH",
            "/_apis/tokenoauth2/token#POST"
          },
          groupingKey = "tokens"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.auditlog",
          title = "Audit Read Log",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to read the auditing log and audit streams to users",
          inheritsFrom = "vso.base",
          patterns = new string[5]
          {
            "/DefaultCollection/_apis/audit/actions#GET",
            "/_apis/audit/auditlog#GET",
            "/DefaultCollection/_apis/audit/downloadlog#GET",
            "/_apis/audit/streams#GET",
            "/DefaultCollection/_apis/audit/streams#GET"
          },
          groupingKey = "auditing"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.auditstreams_manage",
          title = "Audit Manage Streams",
          availability = AuthorizationScopeAvailability.Public,
          description = "Grants the ability to manage auditing streams to users",
          inheritsFrom = "vso.auditlog",
          patterns = new string[2]
          {
            "/_apis/audit/streams#GET+POST+PUT+DELETE",
            "/DefaultCollection/_apis/audit/streams#GET+POST+PUT+DELETE"
          },
          groupingKey = "auditing"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.auditstreams_delete",
          title = "Audit Delete Streams",
          availability = AuthorizationScopeAvailability.Deprecated,
          description = "Grants the ability to delete auditing streams to users",
          inheritsFrom = "vso.base",
          patterns = new string[1]
          {
            "/_apis/audit/streams#DELETE"
          },
          groupingKey = "auditing"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.chatops_teams",
          title = "Teams Integration",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants this application the ability to act on your behalf on Azure DevOps from within Microsoft Teams",
          patterns = new string[13]
          {
            "/_teams/configure#GET",
            "/_teams/configure#POST",
            "/_apis/teams/messages#POST",
            "/_teams/azpipelinesconfigure#GET",
            "/_teams/azpipelinesconfigure#POST",
            "/_apis/teams/azpipelinesmessages#POST",
            "/_teams/azboardsconfigure#GET",
            "/_teams/azboardsconfigure#POST",
            "/_apis/teams/azboardsmessages#POST",
            "/_teams/azreposconfigure#GET",
            "/_teams/azreposconfigure#POST",
            "/_apis/teams/azreposmessages#POST",
            "/_teams/signout#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.chatops_slack",
          title = "Slack integration",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants this application the ability to act on your behalf on Azure DevOps from within Slack",
          patterns = new string[12]
          {
            "/_apis/slack/slashcommands#POST",
            "/_apis/slack/menuoptions#POST",
            "/_apis/slack/interactivecomponents#POST",
            "/_apis/slack/slackeventmessages#POST",
            "/_apis/slack/boardsslashcommands#POST",
            "/_apis/slack/boardsmenuoptions#POST",
            "/_apis/slack/boardsinteractivecomponents#POST",
            "/_apis/slack/boardsslackeventmessages#POST",
            "/_apis/slack/reposslashcommands#POST",
            "/_apis/slack/reposinteractivecomponents#POST",
            "/_apis/slack/reposslackeventmessages#POST",
            "/_apis/slack/*#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.environment_manage",
          title = "Environment (read, manage)",
          description = "Provides ability to manage environment",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.agentpools_manage",
          patterns = new string[3]
          {
            "/DefaultCollection/*/_apis/pipelines/environments/*#GET+POST+DELETE+PATCH+PUT",
            "/DefaultCollection/*/_apis/distributedtask/environments/*#GET+POST+DELETE+PATCH+PUT",
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions/environment#GET+PATCH"
          },
          groupingKey = "environment"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.chatops_azure_teams",
          title = "Teams Integration",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants this application the ability to act on your behalf on Azure from within Microsoft Teams and access the Azure Management Service API thereby manage Azure resources (create, read, update and delete).",
          patterns = new string[2]
          {
            "/_apis/teams/azalertsmessages#POST",
            "/_teams/signout#POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.msdnsubadmin",
          title = "Access MsdnSubAdmin portal Apis",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants application permission to access the Apis for the MsdnSubAdmin service",
          inheritsFrom = "vso.base",
          patterns = new string[11]
          {
            "/_apis/AdminApi/*#GET+POST",
            "/_apis/Agreement/*#GET+POST",
            "/_apis/ManageGetHelp/*#GET+POST",
            "/_apis/Home/*#GET+POST",
            "/_apis/AadSearch/*#GET+POST",
            "/_apis/IdentityPicker/*#GET+POST",
            "/_apis/NoAccess/*#GET+POST",
            "/_apis/Onboarding/*#GET+POST",
            "/_apis/Privacy/*#GET+POST",
            "/_apis/ResendToken/*#GET+POST",
            "/_apis/AdminSupport/*#GET+POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.subscriptions",
          title = "Access Subscriptions portal Apis",
          availability = AuthorizationScopeAvailability.None,
          description = "Grants application permission to access the Apis for the MsdnSubAdmin service",
          inheritsFrom = "vso.base",
          patterns = new string[18]
          {
            "/_apis/Download/*#GET+POST",
            "/_apis/AzureSearch/*#GET+POST",
            "/_apis/Benefits/*#GET+POST",
            "/_apis/Content/*#GET+POST",
            "/_apis/ContentManagement/*#GET+POST",
            "/_apis/CouponCode/*#GET+POST",
            "/_apis/Errors/*#GET+POST",
            "/_apis/GetHelp/*#GET+POST",
            "/_apis/GetStarted/*#GET+POST",
            "/_apis/InternalTools/*#GET+POST",
            "/_apis/Key/*#GET+POST",
            "/_apis/MySubscriptions/*#GET+POST",
            "/_apis/ReleaseManagement/*#GET+POST",
            "/_apis/Retail/*#GET+POST",
            "/_apis/Search/*#GET+POST",
            "/_apis/AntiPriacy/*#GET+POST",
            "/_apis/Signout/*#GET+POST",
            "/_apis/Support/*#GET+POST"
          }
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.pipelineresources_use",
          title = "Pipeline Resources (use)",
          description = "Grants the ability to approve a pipeline's request to use a protected resource: agent pool, environment, queue, repository, secure files, service connection, and variable group",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.base",
          patterns = new string[1]
          {
            "/DefaultCollection/*/_apis/pipelines/approvals#PATCH"
          },
          groupingKey = "pipelineResources"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.pipelineresources_manage",
          title = "Pipeline Resources (use and manage)",
          description = "Grants the ability to manage a protected resource or a pipeline's request to use a protected resource: agent pool, environment, queue, repository, secure files, service connection, and variable group",
          availability = AuthorizationScopeAvailability.Public,
          inheritsFrom = "vso.pipelineresources_use",
          patterns = new string[4]
          {
            "/DefaultCollection/*/_apis/pipelines/pipelinepermissions#PATCH",
            "/DefaultCollection/*/_apis/build/authorizedresources#PATCH",
            "/DefaultCollection/*/_apis/build/definitions/*/resources#PATCH",
            "/DefaultCollection/*/_apis/pipelines/checks/configurations#POST+PATCH+DELETE"
          },
          groupingKey = "pipelineResources"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.advsec",
          title = "AdvancedSecurity (read)",
          description = "Grants the ability to read alerts, result instances, analysis result instances",
          availability = AuthorizationScopeAvailability.Public,
          availabilityFeatureFlag = "VisualStudio.Services.AdvancedSecurity.UserExperienceEnabled",
          inheritsFrom = "vso.base",
          patterns = new string[18]
          {
            "/DefaultCollection/_apis/advancedsecurity/alerts#GET",
            "/DefaultCollection/*/_apis/advancedsecurity/repositories/*/alerts#GET",
            "/DefaultCollection/*/_apis/advancedsecurity/repositories/*/filters#GET",
            "/DefaultCollection/*/_apis/alert/repositories/*/alerts#GET",
            "/DefaultCollection/*/_apis/alert/repositories/*/alerts/*#GET",
            "/DefaultCollection/_apis/advancedsecurity/alerts/*/instances#GET",
            "/DefaultCollection/*/_apis/alert/repositories/*/alerts/*/instances#GET",
            "/DefaultCollection/_apis/advancedsecurity/analyses#GET",
            "/DefaultCollection/*/_apis/advancedsecurity/repositories/*/Enablement#GET",
            "/DefaultCollection/_apis/management/enablement#GET",
            "/DefaultCollection/*/_apis/management/enablement#GET",
            "/DefaultCollection/*/_apis/management/repositories/*/enablement#GET",
            "/DefaultCollection/_apis/advancedsecurity/MeterUsage#GET",
            "/DefaultCollection/_apis/management/meterusage#GET",
            "/DefaultCollection/_apis/management/meterUsageEstimate#GET",
            "/DefaultCollection/*/_apis/management/meterUsageEstimate#GET",
            "/DefaultCollection/*/_apis/management/repositories/*/meterUsageEstimate#GET",
            "/DefaultCollection/_apis/advancedsecurity/billing#GET"
          },
          groupingKey = "advancedSecurity"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.advsec_write",
          title = "AdvancedSecurity (read and write)",
          description = "Grants the ability to upload analyses in sarif",
          availability = AuthorizationScopeAvailability.Public,
          availabilityFeatureFlag = "VisualStudio.Services.AdvancedSecurity.UserExperienceEnabled",
          inheritsFrom = "vso.advsec",
          patterns = new string[2]
          {
            "/DefaultCollection/_apis/advancedsecurity/alerts#PATCH",
            "/DefaultCollection/*/_apis/alert/repositories/*/alerts/*#PATCH"
          },
          groupingKey = "advancedSecurity"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.advsec_manage",
          title = "AdvancedSecurity (read, write, and manage)",
          description = "Grants the ability to access sarif upload information, delete analysis, and update alerts",
          availability = AuthorizationScopeAvailability.Public,
          availabilityFeatureFlag = "VisualStudio.Services.AdvancedSecurity.UserExperienceEnabled",
          inheritsFrom = "vso.advsec_write",
          patterns = new string[9]
          {
            "/DefaultCollection/_apis/advancedsecurity/analyses#DELETE",
            "/DefaultCollection/_apis/advancedsecurity/sarifs#POST",
            "/DefaultCollection/_apis/advancedsecurity/sarifs#GET",
            "/DefaultCollection/*/_apis/alert/repositories/*/sarifs#POST",
            "/DefaultCollection/_apis/alert/sarifs/*#GET",
            "/DefaultCollection/*/_apis/advancedsecurity/repositories/*/Enablement#PATCH",
            "/DefaultCollection/_apis/management/enablement#PATCH",
            "/DefaultCollection/*/_apis/management/enablement#PATCH",
            "/DefaultCollection/*/_apis/management/repositories/*/enablement#PATCH"
          },
          groupingKey = "advancedSecurity"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.githubconnections",
          title = "GitHub Connections (read)",
          description = "Grants the ability to read github connections and github repositories data",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "GitHubConnections.AuthorizationScope.Enable",
          inheritsFrom = "vso.base",
          patterns = new string[3]
          {
            "/DefaultCollection/*/_apis/githubconnections#GET",
            "/DefaultCollection/*/_apis/githubconnections/repos#GET",
            "/DefaultCollection/*/_apis/githubconnections/*/repos#GET"
          },
          groupingKey = "githubConnections"
        },
        new AuthorizationScopeDefinition()
        {
          scope = "vso.githubconnections_manage",
          title = "GitHub Connections (read and manage)",
          description = "Grants the ability to read and manage github connections and github repositories data",
          availability = AuthorizationScopeAvailability.Conditional,
          availabilityFeatureFlag = "GitHubConnections.AuthorizationScope.Enable",
          inheritsFrom = "vso.githubconnections",
          patterns = new string[2]
          {
            "/DefaultCollection/*/_apis/githubconnections/reposBatch#GET+POST",
            "/DefaultCollection/*/_apis/githubconnections/*/reposBatch#GET+POST"
          },
          groupingKey = "githubConnections"
        }
      };
      return scopeDefinitions;
    }

    public static bool TryParse(string json, out AuthorizationScopeDefinitions scopeDefinitions)
    {
      scopeDefinitions = (AuthorizationScopeDefinitions) null;
      if (string.IsNullOrWhiteSpace(json))
        return false;
      try
      {
        scopeDefinitions = JsonConvert.DeserializeObject<AuthorizationScopeDefinitions>(json);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public AuthorizationScopeDefinition[] scopes { get; set; }

    public IDictionary<string, AuthorizationScopeGroupings> groupings { get; set; }

    public IDictionary<string, string> scopeToAccessRights { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None);

    public string ToIndentedString() => JsonConvert.SerializeObject((object) this, Formatting.Indented);
  }
}
