// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ScopeResources
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ScopeResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ScopeResources), typeof (ScopeResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ScopeResources.s_resMgr;

    private static string Get(string resourceName) => ScopeResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ScopeResources.Get(resourceName) : ScopeResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ScopeResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ScopeResources.GetInt(resourceName) : (int) ScopeResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ScopeResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ScopeResources.GetBool(resourceName) : (bool) ScopeResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ScopeResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ScopeResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string Acquire() => ScopeResources.Get(nameof (Acquire));

    public static string Acquire(CultureInfo culture) => ScopeResources.Get(nameof (Acquire), culture);

    public static string AgentPools() => ScopeResources.Get(nameof (AgentPools));

    public static string AgentPools(CultureInfo culture) => ScopeResources.Get(nameof (AgentPools), culture);

    public static string AgentPoolsDescription() => ScopeResources.Get(nameof (AgentPoolsDescription));

    public static string AgentPoolsDescription(CultureInfo culture) => ScopeResources.Get(nameof (AgentPoolsDescription), culture);

    public static string Auditing() => ScopeResources.Get(nameof (Auditing));

    public static string Auditing(CultureInfo culture) => ScopeResources.Get(nameof (Auditing), culture);

    public static string AuditingDescription() => ScopeResources.Get(nameof (AuditingDescription));

    public static string AuditingDescription(CultureInfo culture) => ScopeResources.Get(nameof (AuditingDescription), culture);

    public static string Build() => ScopeResources.Get(nameof (Build));

    public static string Build(CultureInfo culture) => ScopeResources.Get(nameof (Build), culture);

    public static string BuildDescription() => ScopeResources.Get(nameof (BuildDescription));

    public static string BuildDescription(CultureInfo culture) => ScopeResources.Get(nameof (BuildDescription), culture);

    public static string Code() => ScopeResources.Get(nameof (Code));

    public static string Code(CultureInfo culture) => ScopeResources.Get(nameof (Code), culture);

    public static string CodeDescription() => ScopeResources.Get(nameof (CodeDescription));

    public static string CodeDescription(CultureInfo culture) => ScopeResources.Get(nameof (CodeDescription), culture);

    public static string CodeSearch() => ScopeResources.Get(nameof (CodeSearch));

    public static string CodeSearch(CultureInfo culture) => ScopeResources.Get(nameof (CodeSearch), culture);

    public static string CodeSearchDescription() => ScopeResources.Get(nameof (CodeSearchDescription));

    public static string CodeSearchDescription(CultureInfo culture) => ScopeResources.Get(nameof (CodeSearchDescription), culture);

    public static string ConnectedServer() => ScopeResources.Get(nameof (ConnectedServer));

    public static string ConnectedServer(CultureInfo culture) => ScopeResources.Get(nameof (ConnectedServer), culture);

    public static string ConnectedServerDescription() => ScopeResources.Get(nameof (ConnectedServerDescription));

    public static string ConnectedServerDescription(CultureInfo culture) => ScopeResources.Get(nameof (ConnectedServerDescription), culture);

    public static string ReadAuditLog() => ScopeResources.Get(nameof (ReadAuditLog));

    public static string ReadAuditLog(CultureInfo culture) => ScopeResources.Get(nameof (ReadAuditLog), culture);

    public static string ManageStreams() => ScopeResources.Get(nameof (ManageStreams));

    public static string ManageStreams(CultureInfo culture) => ScopeResources.Get(nameof (ManageStreams), culture);

    public static string DeleteStreams() => ScopeResources.Get(nameof (DeleteStreams));

    public static string DeleteStreams(CultureInfo culture) => ScopeResources.Get(nameof (DeleteStreams), culture);

    public static string Diagnostics() => ScopeResources.Get(nameof (Diagnostics));

    public static string Diagnostics(CultureInfo culture) => ScopeResources.Get(nameof (Diagnostics), culture);

    public static string Entitlements() => ScopeResources.Get(nameof (Entitlements));

    public static string Entitlements(CultureInfo culture) => ScopeResources.Get(nameof (Entitlements), culture);

    public static string EntitlementsDescription() => ScopeResources.Get(nameof (EntitlementsDescription));

    public static string EntitlementsDescription(CultureInfo culture) => ScopeResources.Get(nameof (EntitlementsDescription), culture);

    public static string ExpiresInMonths(object arg0) => ScopeResources.Format(nameof (ExpiresInMonths), arg0);

    public static string ExpiresInMonths(object arg0, CultureInfo culture) => ScopeResources.Format(nameof (ExpiresInMonths), culture, arg0);

    public static string ExpiresInOneYear() => ScopeResources.Get(nameof (ExpiresInOneYear));

    public static string ExpiresInOneYear(CultureInfo culture) => ScopeResources.Get(nameof (ExpiresInOneYear), culture);

    public static string ExtensionData() => ScopeResources.Get(nameof (ExtensionData));

    public static string ExtensionData(CultureInfo culture) => ScopeResources.Get(nameof (ExtensionData), culture);

    public static string ExtensionDataDescription() => ScopeResources.Get(nameof (ExtensionDataDescription));

    public static string ExtensionDataDescription(CultureInfo culture) => ScopeResources.Get(nameof (ExtensionDataDescription), culture);

    public static string Extensions() => ScopeResources.Get(nameof (Extensions));

    public static string Extensions(CultureInfo culture) => ScopeResources.Get(nameof (Extensions), culture);

    public static string ExtensionsDescription() => ScopeResources.Get(nameof (ExtensionsDescription));

    public static string ExtensionsDescription(CultureInfo culture) => ScopeResources.Get(nameof (ExtensionsDescription), culture);

    public static string Graph() => ScopeResources.Get(nameof (Graph));

    public static string Graph(CultureInfo culture) => ScopeResources.Get(nameof (Graph), culture);

    public static string GraphDescription() => ScopeResources.Get(nameof (GraphDescription));

    public static string GraphDescription(CultureInfo culture) => ScopeResources.Get(nameof (GraphDescription), culture);

    public static string Identity() => ScopeResources.Get(nameof (Identity));

    public static string Identity(CultureInfo culture) => ScopeResources.Get(nameof (Identity), culture);

    public static string IdentityDescription() => ScopeResources.Get(nameof (IdentityDescription));

    public static string IdentityDescription(CultureInfo culture) => ScopeResources.Get(nameof (IdentityDescription), culture);

    public static string LoadTest() => ScopeResources.Get(nameof (LoadTest));

    public static string LoadTest(CultureInfo culture) => ScopeResources.Get(nameof (LoadTest), culture);

    public static string LoadTestDescription() => ScopeResources.Get(nameof (LoadTestDescription));

    public static string LoadTestDescription(CultureInfo culture) => ScopeResources.Get(nameof (LoadTestDescription), culture);

    public static string MachineGroup() => ScopeResources.Get(nameof (MachineGroup));

    public static string MachineGroup(CultureInfo culture) => ScopeResources.Get(nameof (MachineGroup), culture);

    public static string MachineGroupDescription() => ScopeResources.Get(nameof (MachineGroupDescription));

    public static string MachineGroupDescription(CultureInfo culture) => ScopeResources.Get(nameof (MachineGroupDescription), culture);

    public static string Manage() => ScopeResources.Get(nameof (Manage));

    public static string Manage(CultureInfo culture) => ScopeResources.Get(nameof (Manage), culture);

    public static string Marketplace() => ScopeResources.Get(nameof (Marketplace));

    public static string Marketplace(CultureInfo culture) => ScopeResources.Get(nameof (Marketplace), culture);

    public static string MarketplaceDescription() => ScopeResources.Get(nameof (MarketplaceDescription));

    public static string MarketplaceDescription(CultureInfo culture) => ScopeResources.Get(nameof (MarketplaceDescription), culture);

    public static string MemberEntitlementManagement() => ScopeResources.Get(nameof (MemberEntitlementManagement));

    public static string MemberEntitlementManagement(CultureInfo culture) => ScopeResources.Get(nameof (MemberEntitlementManagement), culture);

    public static string MemberEntitlementManagementDescription() => ScopeResources.Get(nameof (MemberEntitlementManagementDescription));

    public static string MemberEntitlementManagementDescription(CultureInfo culture) => ScopeResources.Get(nameof (MemberEntitlementManagementDescription), culture);

    public static string Miscellaneous() => ScopeResources.Get(nameof (Miscellaneous));

    public static string Miscellaneous(CultureInfo culture) => ScopeResources.Get(nameof (Miscellaneous), culture);

    public static string MiscellaneousDescription() => ScopeResources.Get(nameof (MiscellaneousDescription));

    public static string MiscellaneousDescription(CultureInfo culture) => ScopeResources.Get(nameof (MiscellaneousDescription), culture);

    public static string Never() => ScopeResources.Get(nameof (Never));

    public static string Never(CultureInfo culture) => ScopeResources.Get(nameof (Never), culture);

    public static string Notifications() => ScopeResources.Get(nameof (Notifications));

    public static string Notifications(CultureInfo culture) => ScopeResources.Get(nameof (Notifications), culture);

    public static string NotificationsDescription() => ScopeResources.Get(nameof (NotificationsDescription));

    public static string NotificationsDescription(CultureInfo culture) => ScopeResources.Get(nameof (NotificationsDescription), culture);

    public static string Packaging() => ScopeResources.Get(nameof (Packaging));

    public static string Packaging(CultureInfo culture) => ScopeResources.Get(nameof (Packaging), culture);

    public static string PackagingDescription() => ScopeResources.Get(nameof (PackagingDescription));

    public static string PackagingDescription(CultureInfo culture) => ScopeResources.Get(nameof (PackagingDescription), culture);

    public static string ProjectAndTeam() => ScopeResources.Get(nameof (ProjectAndTeam));

    public static string ProjectAndTeam(CultureInfo culture) => ScopeResources.Get(nameof (ProjectAndTeam), culture);

    public static string ProjectAndTeamDescription() => ScopeResources.Get(nameof (ProjectAndTeamDescription));

    public static string ProjectAndTeamDescription(CultureInfo culture) => ScopeResources.Get(nameof (ProjectAndTeamDescription), culture);

    public static string PrThreads() => ScopeResources.Get(nameof (PrThreads));

    public static string PrThreads(CultureInfo culture) => ScopeResources.Get(nameof (PrThreads), culture);

    public static string PrThreadsDescription() => ScopeResources.Get(nameof (PrThreadsDescription));

    public static string PrThreadsDescription(CultureInfo culture) => ScopeResources.Get(nameof (PrThreadsDescription), culture);

    public static string Publish() => ScopeResources.Get(nameof (Publish));

    public static string Publish(CultureInfo culture) => ScopeResources.Get(nameof (Publish), culture);

    public static string Read() => ScopeResources.Get(nameof (Read));

    public static string Read(CultureInfo culture) => ScopeResources.Get(nameof (Read), culture);

    public static string ReadAndCreate() => ScopeResources.Get(nameof (ReadAndCreate));

    public static string ReadAndCreate(CultureInfo culture) => ScopeResources.Get(nameof (ReadAndCreate), culture);

    public static string ReadAndExecute() => ScopeResources.Get(nameof (ReadAndExecute));

    public static string ReadAndExecute(CultureInfo culture) => ScopeResources.Get(nameof (ReadAndExecute), culture);

    public static string ReadAndManage() => ScopeResources.Get(nameof (ReadAndManage));

    public static string ReadAndManage(CultureInfo culture) => ScopeResources.Get(nameof (ReadAndManage), culture);

    public static string ReadAndQuery() => ScopeResources.Get(nameof (ReadAndQuery));

    public static string ReadAndQuery(CultureInfo culture) => ScopeResources.Get(nameof (ReadAndQuery), culture);

    public static string ReadAndWrite() => ScopeResources.Get(nameof (ReadAndWrite));

    public static string ReadAndWrite(CultureInfo culture) => ScopeResources.Get(nameof (ReadAndWrite), culture);

    public static string ReadCreateAndManage() => ScopeResources.Get(nameof (ReadCreateAndManage));

    public static string ReadCreateAndManage(CultureInfo culture) => ScopeResources.Get(nameof (ReadCreateAndManage), culture);

    public static string ReadQueryAndManage() => ScopeResources.Get(nameof (ReadQueryAndManage));

    public static string ReadQueryAndManage(CultureInfo culture) => ScopeResources.Get(nameof (ReadQueryAndManage), culture);

    public static string ReadWriteAndExecute() => ScopeResources.Get(nameof (ReadWriteAndExecute));

    public static string ReadWriteAndExecute(CultureInfo culture) => ScopeResources.Get(nameof (ReadWriteAndExecute), culture);

    public static string ReadWriteAndManage() => ScopeResources.Get(nameof (ReadWriteAndManage));

    public static string ReadWriteAndManage(CultureInfo culture) => ScopeResources.Get(nameof (ReadWriteAndManage), culture);

    public static string ReadWriteExecuteAndManage() => ScopeResources.Get(nameof (ReadWriteExecuteAndManage));

    public static string ReadWriteExecuteAndManage(CultureInfo culture) => ScopeResources.Get(nameof (ReadWriteExecuteAndManage), culture);

    public static string Release() => ScopeResources.Get(nameof (Release));

    public static string Release(CultureInfo culture) => ScopeResources.Get(nameof (Release), culture);

    public static string ReleaseDescription() => ScopeResources.Get(nameof (ReleaseDescription));

    public static string ReleaseDescription(CultureInfo culture) => ScopeResources.Get(nameof (ReleaseDescription), culture);

    public static string Security() => ScopeResources.Get(nameof (Security));

    public static string Security(CultureInfo culture) => ScopeResources.Get(nameof (Security), culture);

    public static string SecurityDescription() => ScopeResources.Get(nameof (SecurityDescription));

    public static string SecurityDescription(CultureInfo culture) => ScopeResources.Get(nameof (SecurityDescription), culture);

    public static string ServiceEndpoints() => ScopeResources.Get(nameof (ServiceEndpoints));

    public static string ServiceEndpoints(CultureInfo culture) => ScopeResources.Get(nameof (ServiceEndpoints), culture);

    public static string ServiceEndpointsDescription() => ScopeResources.Get(nameof (ServiceEndpointsDescription));

    public static string ServiceEndpointsDescription(CultureInfo culture) => ScopeResources.Get(nameof (ServiceEndpointsDescription), culture);

    public static string SpecificDate() => ScopeResources.Get(nameof (SpecificDate));

    public static string SpecificDate(CultureInfo culture) => ScopeResources.Get(nameof (SpecificDate), culture);

    public static string Status() => ScopeResources.Get(nameof (Status));

    public static string Status(CultureInfo culture) => ScopeResources.Get(nameof (Status), culture);

    public static string Symbols() => ScopeResources.Get(nameof (Symbols));

    public static string Symbols(CultureInfo culture) => ScopeResources.Get(nameof (Symbols), culture);

    public static string SymbolsDescription() => ScopeResources.Get(nameof (SymbolsDescription));

    public static string SymbolsDescription(CultureInfo culture) => ScopeResources.Get(nameof (SymbolsDescription), culture);

    public static string TaskGroups() => ScopeResources.Get(nameof (TaskGroups));

    public static string TaskGroups(CultureInfo culture) => ScopeResources.Get(nameof (TaskGroups), culture);

    public static string TaskGroupsDescription() => ScopeResources.Get(nameof (TaskGroupsDescription));

    public static string TaskGroupsDescription(CultureInfo culture) => ScopeResources.Get(nameof (TaskGroupsDescription), culture);

    public static string TeamDashboard() => ScopeResources.Get(nameof (TeamDashboard));

    public static string TeamDashboard(CultureInfo culture) => ScopeResources.Get(nameof (TeamDashboard), culture);

    public static string TeamDashboardDescription() => ScopeResources.Get(nameof (TeamDashboardDescription));

    public static string TeamDashboardDescription(CultureInfo culture) => ScopeResources.Get(nameof (TeamDashboardDescription), culture);

    public static string TestManagement() => ScopeResources.Get(nameof (TestManagement));

    public static string TestManagement(CultureInfo culture) => ScopeResources.Get(nameof (TestManagement), culture);

    public static string TestManagementDescription() => ScopeResources.Get(nameof (TestManagementDescription));

    public static string TestManagementDescription(CultureInfo culture) => ScopeResources.Get(nameof (TestManagementDescription), culture);

    public static string UserProfile() => ScopeResources.Get(nameof (UserProfile));

    public static string UserProfile(CultureInfo culture) => ScopeResources.Get(nameof (UserProfile), culture);

    public static string UserProfileDescription() => ScopeResources.Get(nameof (UserProfileDescription));

    public static string UserProfileDescription(CultureInfo culture) => ScopeResources.Get(nameof (UserProfileDescription), culture);

    public static string VariableGroups() => ScopeResources.Get(nameof (VariableGroups));

    public static string VariableGroups(CultureInfo culture) => ScopeResources.Get(nameof (VariableGroups), culture);

    public static string VariableGroupsDescription() => ScopeResources.Get(nameof (VariableGroupsDescription));

    public static string VariableGroupsDescription(CultureInfo culture) => ScopeResources.Get(nameof (VariableGroupsDescription), culture);

    public static string Wiki() => ScopeResources.Get(nameof (Wiki));

    public static string Wiki(CultureInfo culture) => ScopeResources.Get(nameof (Wiki), culture);

    public static string WikiDescription() => ScopeResources.Get(nameof (WikiDescription));

    public static string WikiDescription(CultureInfo culture) => ScopeResources.Get(nameof (WikiDescription), culture);

    public static string WorkItems() => ScopeResources.Get(nameof (WorkItems));

    public static string WorkItems(CultureInfo culture) => ScopeResources.Get(nameof (WorkItems), culture);

    public static string WorkItemsDescription() => ScopeResources.Get(nameof (WorkItemsDescription));

    public static string WorkItemsDescription(CultureInfo culture) => ScopeResources.Get(nameof (WorkItemsDescription), culture);

    public static string WorkItemSearch() => ScopeResources.Get(nameof (WorkItemSearch));

    public static string WorkItemSearch(CultureInfo culture) => ScopeResources.Get(nameof (WorkItemSearch), culture);

    public static string WorkItemSearchDescription() => ScopeResources.Get(nameof (WorkItemSearchDescription));

    public static string WorkItemSearchDescription(CultureInfo culture) => ScopeResources.Get(nameof (WorkItemSearchDescription), culture);

    public static string Write() => ScopeResources.Get(nameof (Write));

    public static string Write(CultureInfo culture) => ScopeResources.Get(nameof (Write), culture);

    public static string Full() => ScopeResources.Get(nameof (Full));

    public static string Full(CultureInfo culture) => ScopeResources.Get(nameof (Full), culture);

    public static string Tokens() => ScopeResources.Get(nameof (Tokens));

    public static string Tokens(CultureInfo culture) => ScopeResources.Get(nameof (Tokens), culture);

    public static string TokensDescription() => ScopeResources.Get(nameof (TokensDescription));

    public static string TokensDescription(CultureInfo culture) => ScopeResources.Get(nameof (TokensDescription), culture);

    public static string TokenAdministration() => ScopeResources.Get(nameof (TokenAdministration));

    public static string TokenAdministration(CultureInfo culture) => ScopeResources.Get(nameof (TokenAdministration), culture);

    public static string TokenAdministrationDescription() => ScopeResources.Get(nameof (TokenAdministrationDescription));

    public static string TokenAdministrationDescription(CultureInfo culture) => ScopeResources.Get(nameof (TokenAdministrationDescription), culture);

    public static string Analytics() => ScopeResources.Get(nameof (Analytics));

    public static string Analytics(CultureInfo culture) => ScopeResources.Get(nameof (Analytics), culture);

    public static string AnalyticsDescription() => ScopeResources.Get(nameof (AnalyticsDescription));

    public static string AnalyticsDescription(CultureInfo culture) => ScopeResources.Get(nameof (AnalyticsDescription), culture);

    public static string BuildCache() => ScopeResources.Get(nameof (BuildCache));

    public static string BuildCache(CultureInfo culture) => ScopeResources.Get(nameof (BuildCache), culture);

    public static string BuildCacheDescription() => ScopeResources.Get(nameof (BuildCacheDescription));

    public static string BuildCacheDescription(CultureInfo culture) => ScopeResources.Get(nameof (BuildCacheDescription), culture);

    public static string Drop() => ScopeResources.Get(nameof (Drop));

    public static string Drop(CultureInfo culture) => ScopeResources.Get(nameof (Drop), culture);

    public static string DropDescription() => ScopeResources.Get(nameof (DropDescription));

    public static string DropDescription(CultureInfo culture) => ScopeResources.Get(nameof (DropDescription), culture);

    public static string SecureFiles() => ScopeResources.Get(nameof (SecureFiles));

    public static string SecureFiles(CultureInfo culture) => ScopeResources.Get(nameof (SecureFiles), culture);

    public static string SecureFilesDescription() => ScopeResources.Get(nameof (SecureFilesDescription));

    public static string SecureFilesDescription(CultureInfo culture) => ScopeResources.Get(nameof (SecureFilesDescription), culture);

    public static string Environment() => ScopeResources.Get(nameof (Environment));

    public static string Environment(CultureInfo culture) => ScopeResources.Get(nameof (Environment), culture);

    public static string EnvironmentDescription() => ScopeResources.Get(nameof (EnvironmentDescription));

    public static string EnvironmentDescription(CultureInfo culture) => ScopeResources.Get(nameof (EnvironmentDescription), culture);

    public static string PipelineResources() => ScopeResources.Get(nameof (PipelineResources));

    public static string PipelineResources(CultureInfo culture) => ScopeResources.Get(nameof (PipelineResources), culture);

    public static string PipelineResourcesDescription() => ScopeResources.Get(nameof (PipelineResourcesDescription));

    public static string PipelineResourcesDescription(CultureInfo culture) => ScopeResources.Get(nameof (PipelineResourcesDescription), culture);

    public static string GitHubConnections() => ScopeResources.Get(nameof (GitHubConnections));

    public static string GitHubConnections(CultureInfo culture) => ScopeResources.Get(nameof (GitHubConnections), culture);

    public static string GitHubConnectionsDescription() => ScopeResources.Get(nameof (GitHubConnectionsDescription));

    public static string GitHubConnectionsDescription(CultureInfo culture) => ScopeResources.Get(nameof (GitHubConnectionsDescription), culture);

    public static string Use() => ScopeResources.Get(nameof (Use));

    public static string Use(CultureInfo culture) => ScopeResources.Get(nameof (Use), culture);

    public static string UseAndManage() => ScopeResources.Get(nameof (UseAndManage));

    public static string UseAndManage(CultureInfo culture) => ScopeResources.Get(nameof (UseAndManage), culture);

    public static string AdvancedSecurity() => ScopeResources.Get(nameof (AdvancedSecurity));

    public static string AdvancedSecurity(CultureInfo culture) => ScopeResources.Get(nameof (AdvancedSecurity), culture);

    public static string AdvancedSecurityDescription() => ScopeResources.Get(nameof (AdvancedSecurityDescription));

    public static string AdvancedSecurityDescription(CultureInfo culture) => ScopeResources.Get(nameof (AdvancedSecurityDescription), culture);
  }
}
