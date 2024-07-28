// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseSecretsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseSecretsService : ReleaseManagement2ServiceBase
  {
    private readonly SecretsHelper secretsHelper;
    private readonly Func<IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>> variablesMerger;
    private readonly Func<IVssRequestContext, Guid, IList<VariableGroup>, IDictionary<string, MergedConfigurationVariableValue>> variableGroupsMerger;

    public ReleaseSecretsService()
    {
      this.secretsHelper = new SecretsHelper();
      this.variableGroupsMerger = (Func<IVssRequestContext, Guid, IList<VariableGroup>, IDictionary<string, MergedConfigurationVariableValue>>) ((context, projectId, groups) => VariableGroupsMerger.MergeVariableGroups((IList<VariableGroup>) new List<VariableGroup>((IEnumerable<VariableGroup>) groups)));
      this.variablesMerger = (Func<IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>, IDictionary<string, ConfigurationVariableValue>>) ((dominant, recessive, recessive1, recessive2, recessive3) => DictionaryMerger.MergeDictionaries<string, ConfigurationVariableValue>((IEnumerable<IDictionary<string, ConfigurationVariableValue>>) new IDictionary<string, ConfigurationVariableValue>[5]
      {
        dominant,
        recessive,
        recessive1,
        recessive2,
        recessive3
      }));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design.")]
    public virtual IDictionary<string, string> GetSecretVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int deployPhaseId,
      bool includeExternalVariables)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      Func<ReleaseSqlComponent, Release> action = (Func<ReleaseSqlComponent, Release>) (component => component.GetRelease(projectId, releaseId));
      Release release = requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, Release>(action);
      return this.GetSecretVariables(requestContext, projectId, release, environmentId, deployPhaseId, includeExternalVariables);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design.")]
    public virtual IDictionary<string, string> GetSecretVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      int environmentId,
      int deployPhaseId,
      bool includeExternalVariables)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      ReleaseEnvironment environment = release.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (x => x.Id == environmentId));
      if (environment == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NoEnvironmentFoundInRelease, (object) environmentId, (object) release.Id));
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue> issueList = this.secretsHelper.ReadSecrets(requestContext, projectId, release, environment, includeExternalVariables);
      IDictionary<string, ConfigurationVariableValue> dictionary1 = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>();
      ReleaseDeployPhase releaseDeployPhase = environment.ReleaseDeployPhases.SingleOrDefault<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (phase => phase.Id == deployPhaseId));
      if (releaseDeployPhase != null)
      {
        Deployment deployment = environment.DeploymentAttempts.Single<Deployment>((Func<Deployment, bool>) (attempt => attempt.Attempt == releaseDeployPhase.Attempt));
        ReleaseEnvironmentSnapshotDelta deploymentSnapshotDelta = new DataAccessLayer(requestContext, projectId).GetDeploymentSnapshotDelta(release.Id, deployment.Id);
        if (deploymentSnapshotDelta != null && !deploymentSnapshotDelta.Variables.IsNullOrEmpty<KeyValuePair<string, ConfigurationVariableValue>>())
        {
          this.secretsHelper.ReadSecrets(requestContext, projectId, deploymentSnapshotDelta);
          dictionary1 = deploymentSnapshotDelta.Variables;
        }
      }
      Dictionary<string, string> secretVariables = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, ConfigurationVariableValue> dictionary2 = this.variableGroupsMerger(requestContext, projectId, release.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, ConfigurationVariableValue>) (p => new ConfigurationVariableValue()
      {
        Value = p.Value.Value,
        IsSecret = p.Value.IsSecret
      }));
      Dictionary<string, ConfigurationVariableValue> dictionary3 = this.variableGroupsMerger(requestContext, projectId, environment.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, ConfigurationVariableValue>) (p => new ConfigurationVariableValue()
      {
        Value = p.Value.Value,
        IsSecret = p.Value.IsSecret
      }));
      foreach (KeyValuePair<string, ConfigurationVariableValue> keyValuePair in this.variablesMerger(dictionary1, environment.Variables, (IDictionary<string, ConfigurationVariableValue>) dictionary3, release.Variables, (IDictionary<string, ConfigurationVariableValue>) dictionary2).Where<KeyValuePair<string, ConfigurationVariableValue>>((Func<KeyValuePair<string, ConfigurationVariableValue>, bool>) (kvp => kvp.Value != null && kvp.Value.IsSecret)))
        secretVariables[keyValuePair.Key] = keyValuePair.Value.Value ?? string.Empty;
      if (!issueList.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue>())
      {
        Deployment latestDeployment = environment.GetLatestDeployment();
        requestContext.GetService<DeploymentsService>().AddDeploymentIssues(requestContext, projectId, release.Id, latestDeployment.Id, issueList);
      }
      return (IDictionary<string, string>) secretVariables;
    }
  }
}
