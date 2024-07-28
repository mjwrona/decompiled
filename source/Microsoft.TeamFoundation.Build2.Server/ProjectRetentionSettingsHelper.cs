// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ProjectRetentionSettingsHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class ProjectRetentionSettingsHelper
  {
    private ProjectRetentionSettings m_projectRetentionSettings;
    private const string c_defaultSettingsOverridePath = "/Configuration/Pipelines/Retention/ProjectSettingsDefaults/";
    private static readonly RegistryQuery s_projectSettingsDefaultOverrides = (RegistryQuery) "/Configuration/Pipelines/Retention/ProjectSettingsDefaults/**";

    public ProjectRetentionSettingsHelper(
      IVssRequestContext requestContext,
      Guid projectId,
      bool ignoreViewPermissions)
      : this(requestContext, (IBuildSecurityProvider) new BuildSecurityProvider(), projectId, ignoreViewPermissions)
    {
    }

    internal ProjectRetentionSettingsHelper(
      IVssRequestContext requestContext,
      IBuildSecurityProvider securityProvider,
      Guid projectId,
      bool ignoreViewPermissions)
    {
      this.SecurityProvider = securityProvider;
      this.HasEditPermission = securityProvider.HasProjectPermission(requestContext, projectId, BuildPermissions.AdministerBuildPermissions, true);
      if (!ignoreViewPermissions)
        this.SecurityProvider.CheckProjectPermission(requestContext, projectId, AdministrationPermissions.ViewBuildResources, true);
      ProjectRetentionSettings defaultSettings = ProjectRetentionSettingsHelper.LoadDefaults(requestContext);
      ProjectRetentionSettings retentionSettings = ProjectRetentionSettingsHelper.LoadSettings(requestContext, projectId, defaultSettings);
      retentionSettings.PurgeArtifacts.Min = defaultSettings.PurgeArtifacts.Min;
      retentionSettings.PurgeArtifacts.Max = defaultSettings.PurgeArtifacts.Max;
      retentionSettings.PurgeRuns.Min = defaultSettings.PurgeRuns.Min;
      retentionSettings.PurgeRuns.Max = defaultSettings.PurgeRuns.Max;
      retentionSettings.PurgePullRequestRuns.Min = defaultSettings.PurgePullRequestRuns.Min;
      retentionSettings.PurgePullRequestRuns.Max = defaultSettings.PurgePullRequestRuns.Max;
      retentionSettings.RunsToRetainPerProtectedBranch.Min = defaultSettings.RunsToRetainPerProtectedBranch.Min;
      retentionSettings.RunsToRetainPerProtectedBranch.Max = defaultSettings.RunsToRetainPerProtectedBranch.Max;
      this.m_projectRetentionSettings = retentionSettings;
    }

    public bool UpdateSettings(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int? artifactsRetention,
      int? runRetention,
      int? pullRequestRunRetention,
      int? retainRunsPerProtectedBranch)
    {
      if (this.HasEditPermission)
      {
        (bool, string, int, int)[] source = new (bool, string, int, int)[4]
        {
          ProjectRetentionSettingsHelper.CheckUpdateSetting(ref this.m_projectRetentionSettings.PurgeRuns, runRetention, "PurgeRuns"),
          ProjectRetentionSettingsHelper.CheckUpdateSetting(ref this.m_projectRetentionSettings.PurgePullRequestRuns, pullRequestRunRetention, "PurgePullRequestRuns"),
          ProjectRetentionSettingsHelper.CheckUpdateSetting(ref this.m_projectRetentionSettings.PurgeArtifacts, artifactsRetention, "PurgeArtifacts"),
          ProjectRetentionSettingsHelper.CheckUpdateSetting(ref this.m_projectRetentionSettings.RunsToRetainPerProtectedBranch, retainRunsPerProtectedBranch, "RunsToRetainPerProtectedBranch")
        };
        if (source[3].Item1)
        {
          int? nullable = retainRunsPerProtectedBranch;
          int num = 0;
          if (nullable.GetValueOrDefault() == num & nullable.HasValue)
          {
            using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
              requestContext.RunSynchronously<IReadOnlyList<RetentionLease>>((Func<Task<IReadOnlyList<RetentionLease>>>) (() => bc.DeleteRetentionLeasesByOwnerPrefix(projectInfo.Id, "Branch")));
          }
        }
        if (((IEnumerable<(bool, string, int, int)>) source).Any<(bool, string, int, int)>((Func<(bool, string, int, int), bool>) (x => x.Updated)))
        {
          ISettingsService service = requestContext.GetService<ISettingsService>();
          IVssRequestContext requestContext1 = requestContext;
          SettingsUserScope allUsers = SettingsUserScope.AllUsers;
          Guid id = projectInfo.Id;
          string settingScopeValue = id.ToString();
          // ISSUE: variable of a boxed type
          __Boxed<ProjectRetentionSettings> retentionSettings = (ValueType) this.ProjectRetentionSettings;
          service.SetValue(requestContext1, allUsers, "Project", settingScopeValue, "Pipelines/Retention/Settings", (object) retentionSettings);
          foreach ((bool _, string str, int num1, int num2) in ((IEnumerable<(bool, string, int, int)>) source).Where<(bool, string, int, int)>((Func<(bool, string, int, int), bool>) (x => x.Updated)))
          {
            IVssRequestContext requestContext2 = requestContext;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["SettingName"] = (object) str;
            data["OldValue"] = (object) num1;
            data["NewValue"] = (object) num2;
            data["ProjectName"] = (object) projectInfo.Name;
            id = projectInfo.Id;
            Guid targetHostId = new Guid();
            Guid projectId = id;
            requestContext2.LogAuditEvent("Pipelines.PipelineRetentionSettingChanged", data, targetHostId, projectId);
          }
          return true;
        }
      }
      return false;
    }

    private static (bool Updated, string SettingName, int OldValue, int NewValue) CheckUpdateSetting(
      ref RetentionSetting setting,
      int? newVal,
      string settingName)
    {
      if (newVal.HasValue)
      {
        int? nullable = newVal;
        int min = setting.Min;
        if (nullable.GetValueOrDefault() >= min & nullable.HasValue)
        {
          nullable = newVal;
          int max = setting.Max;
          if (nullable.GetValueOrDefault() <= max & nullable.HasValue)
          {
            nullable = newVal;
            int num1 = setting.Value;
            if (!(nullable.GetValueOrDefault() == num1 & nullable.HasValue))
            {
              int num2 = setting.Value;
              setting.Value = newVal.Value;
              return (true, settingName, num2, newVal.Value);
            }
          }
        }
      }
      return (false, (string) null, 0, 0);
    }

    private static ProjectRetentionSettings LoadDefaults(IVssRequestContext requestContext)
    {
      ProjectRetentionSettings retentionSettings = requestContext.ExecutionEnvironment.IsHostedDeployment ? ProjectRetentionSettings.HostedDefault : ProjectRetentionSettings.OnPremDefault;
      RegistryEntryCollection projectSettingsOverrides = requestContext.GetService<ICachedRegistryService>().ReadEntries(requestContext, ProjectRetentionSettingsHelper.s_projectSettingsDefaultOverrides);
      ApplyOverrides("/Configuration/Pipelines/Retention/ProjectSettingsDefaults/PurgeArtifacts/", ref retentionSettings.PurgeArtifacts);
      ApplyOverrides("/Configuration/Pipelines/Retention/ProjectSettingsDefaults/PurgeRuns/", ref retentionSettings.PurgeRuns);
      ApplyOverrides("/Configuration/Pipelines/Retention/ProjectSettingsDefaults/PurgePullRequests/", ref retentionSettings.PurgePullRequestRuns);
      ApplyOverrides("/Configuration/Pipelines/Retention/ProjectSettingsDefaults/RetainRunsPerProtectedBranch/", ref retentionSettings.RunsToRetainPerProtectedBranch);
      return retentionSettings;

      void ApplyOverrides(string registryPath, ref RetentionSetting s)
      {
        s.Min = projectSettingsOverrides.GetValueFromPath<int>(registryPath + "Min", s.Min);
        s.Max = projectSettingsOverrides.GetValueFromPath<int>(registryPath + "Max", s.Max);
        s.Value = projectSettingsOverrides.GetValueFromPath<int>(registryPath + "Default", s.Value);
      }
    }

    private static ProjectRetentionSettings LoadSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      ProjectRetentionSettings defaultSettings)
    {
      return requestContext.GetService<ISettingsService>().GetValue<ProjectRetentionSettings>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "Pipelines/Retention/Settings", defaultSettings);
    }

    public bool HasEditPermission { get; }

    public RetentionSetting PurgeRuns => this.ProjectRetentionSettings.PurgeRuns;

    public RetentionSetting PurgePullRequestRuns => this.ProjectRetentionSettings.PurgePullRequestRuns;

    public RetentionSetting PurgeArtifacts => this.ProjectRetentionSettings.PurgeArtifacts;

    public RetentionSetting RetainRunsPerProtectedBranch => this.ProjectRetentionSettings.RunsToRetainPerProtectedBranch;

    public ProjectRetentionSettings ProjectRetentionSettings => this.m_projectRetentionSettings;

    private IBuildSecurityProvider SecurityProvider { get; }
  }
}
