// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PolicyMigrator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PolicyMigrator
  {
    private readonly IVssRequestContext m_rc;

    public PolicyMigrator(IVssRequestContext rc) => this.m_rc = rc;

    public string Migrate(
      Guid projectId,
      Guid sourceRepo,
      Guid targetRepo,
      OdbId odbId,
      string[] migrationBranchList = null)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      ITeamFoundationPolicyService service = this.m_rc.GetService<ITeamFoundationPolicyService>();
      Dictionary<string, string> branchesToRescope = new Dictionary<string, string>();
      if (migrationBranchList != null)
      {
        foreach (string migrationBranch in migrationBranchList)
        {
          BranchMigrationData branchMigrationData = MigrationBranchParser.Parse(migrationBranch);
          if (branchMigrationData != null && !branchesToRescope.ContainsKey(branchMigrationData.SourceName))
            branchesToRescope.Add(branchMigrationData.SourceName, branchMigrationData.TargetName);
        }
      }
      foreach (PolicyConfigurationRecord configurationRecord in service.GetLatestPolicyConfigurationRecords(this.m_rc, projectId, int.MaxValue, 1, out int? _).Where<PolicyConfigurationRecord>((Func<PolicyConfigurationRecord, bool>) (policy => policy.IsEnabled && !policy.IsDeleted && !policy.IsEnterpriseManaged)))
      {
        JObject jobject = JObject.Parse(configurationRecord.Settings);
        JToken scopeArray = jobject["scope"];
        if (this.TryRetargetPolicy(sourceRepo, targetRepo, (IDictionary<string, string>) branchesToRescope, ref scopeArray))
        {
          try
          {
            jobject["scope"] = scopeArray;
            PolicyConfigurationRecord policyConfiguration = service.CreatePolicyConfiguration(this.m_rc, configurationRecord.TypeId, configurationRecord.ProjectId, configurationRecord.IsEnabled, configurationRecord.IsBlocking, false, jobject.ToString(Formatting.None));
            using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(odbId))
              gitOdbComponent.WritePolicyMigrationObject(this.m_rc.ServiceHost.CollectionServiceHost.InstanceId, sourceRepo, targetRepo, configurationRecord.ConfigurationId, configurationRecord.ConfigurationRevisionId, policyConfiguration.ConfigurationId);
            ++num1;
          }
          catch (Exception ex)
          {
            ++num3;
            this.m_rc.RequestTracer.TraceAlways(1013852, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (PolicyMigrator), string.Format("Error migrating policy {0}: {1}", (object) configurationRecord.ConfigurationId, (object) ex.Message));
          }
        }
        else
          ++num2;
      }
      return string.Format("Migrated {0} policies.\nSkipped {1} unneeded policies.", (object) num1, (object) num2) + string.Format("\n{0} could not be migrated due to errors.", (object) num3);
    }

    private bool TryRetargetPolicy(
      Guid sourceRepo,
      Guid targetRepo,
      IDictionary<string, string> branchesToRescope,
      ref JToken scopeArray)
    {
      bool flag = false;
      JArray jarray = new JArray();
      foreach (JToken jtoken1 in (IEnumerable<JToken>) scopeArray)
      {
        Guid result;
        if (Guid.TryParse(jtoken1.Value<string>((object) "repositoryId"), out result) && sourceRepo == result)
        {
          JToken jtoken2 = jtoken1.DeepClone();
          jtoken2[(object) "repositoryId"] = (JToken) targetRepo.ToString();
          if (jtoken1.Value<string>((object) "matchKind") == "Exact" && !string.IsNullOrEmpty(jtoken1.Value<string>((object) "refName")) && branchesToRescope.ContainsKey(jtoken1.Value<string>((object) "refName")))
            jtoken2[(object) "refName"] = (JToken) branchesToRescope[jtoken1.Value<string>((object) "refName")];
          jarray.Add(jtoken2);
          flag = true;
        }
      }
      scopeArray = (JToken) jarray;
      return flag;
    }
  }
}
