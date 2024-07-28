// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class Release
  {
    private const string StrongBoxSecretVariableKeyPrefixFormat = "Variables";
    private const string StrongBoxSecretVariableGroupPrefixFormat = "VariableGroups/{0}/Variables";
    private IList<PropertyValue> properties;

    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? CreatedFor { get; set; }

    public Guid ModifiedBy { get; set; }

    public ReleaseStatus Status { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public string ReleaseDefinitionName { get; set; }

    public string ReleaseDefinitionPath { get; set; }

    public int ReleaseDefinitionRevision { get; set; }

    public IDictionary<string, ConfigurationVariableValue> Variables { get; private set; }

    public IList<VariableGroup> VariableGroups { get; }

    public int TargetEnvironmentId { get; set; }

    public IList<ReleaseEnvironment> Environments { get; private set; }

    public bool IsDeferred { get; set; }

    public DateTime? DeferredDateTime { get; set; }

    public bool KeepForever { get; set; }

    public bool IsDeleted { get; set; }

    public IList<ArtifactSource> LinkedArtifacts { get; private set; }

    public ReleaseDefinitionEnvironmentsSnapshot DefinitionSnapshot { get; set; }

    public ReleaseReason Reason { get; set; }

    public string ReleaseNameFormat { get; set; }

    public string PartiallyExpandedReleaseNameFormat { get; set; }

    public int DefinitionSnapshotRevision { get; set; }

    public IList<string> Tags { get; private set; }

    public string TriggeringArtifactAlias { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public IList<PropertyValue> Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = (IList<PropertyValue>) new List<PropertyValue>();
        return this.properties;
      }
    }

    public Release()
    {
      this.Environments = (IList<ReleaseEnvironment>) new List<ReleaseEnvironment>();
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.LinkedArtifacts = (IList<ArtifactSource>) new List<ArtifactSource>();
      this.VariableGroups = (IList<VariableGroup>) new List<VariableGroup>();
      this.Tags = (IList<string>) new List<string>();
    }

    public virtual string GetDrawerName(Guid projectIdentifier) => ReleaseDataModelUtility.GetDrawerName(projectIdentifier, this.Id);

    public virtual string SecretVariableLookupPrefix => "Variables";

    public virtual string VariableGroupSecretsKeyPrefix(int groupId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VariableGroups/{0}/Variables", (object) groupId);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupt serialized data")]
    public ReleaseEnvironment GetLastModifiedEnvironment()
    {
      ReleaseEnvironmentStep currentEnvironmentStep = this.Environments.SelectMany<ReleaseEnvironment, ReleaseEnvironmentStep>((Func<ReleaseEnvironment, IEnumerable<ReleaseEnvironmentStep>>) (env => (IEnumerable<ReleaseEnvironmentStep>) env.GetStepsForTests)).OrderBy<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (step => step.CreatedOn)).LastOrDefault<ReleaseEnvironmentStep>();
      return currentEnvironmentStep == null ? this.Environments.Single<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Rank == 1)) : this.Environments.Single<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Id == currentEnvironmentStep.ReleaseEnvironmentId));
    }

    public ReleaseEnvironment GetEnvironmentByName(string environmentName) => this.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => string.Equals(env.Name, environmentName, StringComparison.OrdinalIgnoreCase)));

    public ArtifactSource GetArtifactByAlias(string artifactAlias) => this.LinkedArtifacts.SingleOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (artifact => string.Equals(artifact.Alias, artifactAlias, StringComparison.OrdinalIgnoreCase)));

    public int GetDefinitionEnvironmentId(int releaseEnvironmentId) => this.GetEnvironment(releaseEnvironmentId).DefinitionEnvironmentId;

    public ReleaseEnvironment GetEnvironment(int releaseEnvironmentId) => this.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Id == releaseEnvironmentId));

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupt serialized data")]
    public IList<int> GetEnvironmentIdList() => (IList<int>) this.Environments.Select<ReleaseEnvironment, int>((Func<ReleaseEnvironment, int>) (env => env.Id)).ToList<int>();

    public DefinitionEnvironmentData GetDefinitionEnvironmentData(string environmentName) => this.DefinitionSnapshot.Environments.SingleOrDefault<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (env => string.Equals(env.Name, environmentName, StringComparison.OrdinalIgnoreCase)));

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupting serialized data.")]
    public IEnumerable<DefinitionEnvironmentStep> GetHigherRankApprovalSteps(
      string environmentName,
      EnvironmentStepType stepType,
      int rank)
    {
      return this.GetDefinitionEnvironmentData(environmentName).GetDefinitionEnvironmentSteps(stepType).Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (x => x.Rank > rank)).Select<DefinitionEnvironmentStepData, DefinitionEnvironmentStep>((Func<DefinitionEnvironmentStepData, DefinitionEnvironmentStep>) (y => y.ToDefinitionEnvironmentStep()));
    }

    public ReleaseEnvironmentStep GetStep(int releaseEnvironmentStepId) => this.Environments.SelectMany<ReleaseEnvironment, ReleaseEnvironmentStep>((Func<ReleaseEnvironment, IEnumerable<ReleaseEnvironmentStep>>) (environment => (IEnumerable<ReleaseEnvironmentStep>) environment.GetStepsForTests)).Single<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.Id == releaseEnvironmentStepId));

    public Guid GetRunPlanId(int releaseEnvironmentId, int attempt)
    {
      Guid runPlanId = Guid.Empty;
      ReleaseEnvironment environment = this.GetEnvironment(releaseEnvironmentId);
      if (environment != null)
      {
        ReleaseDeployPhase releaseDeployPhase = environment.ReleaseDeployPhases.FirstOrDefault<ReleaseDeployPhase>((Func<ReleaseDeployPhase, bool>) (rdp => rdp.Attempt == attempt));
        runPlanId = releaseDeployPhase == null || !releaseDeployPhase.RunPlanId.HasValue ? Guid.Empty : releaseDeployPhase.RunPlanId.Value;
      }
      return runPlanId;
    }

    public void LinkArtifacts(IEnumerable<ArtifactSource> artifacts) => ((List<ArtifactSource>) this.LinkedArtifacts).AddRange(artifacts);

    public virtual void FillSecrets(Release releaseWithSecrets)
    {
      if (releaseWithSecrets == null)
        throw new ArgumentNullException(nameof (releaseWithSecrets));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.FillSecrets(releaseWithSecrets.Variables, this.Variables);
      VariableGroupUtility.FillSecrets(releaseWithSecrets.VariableGroups, this.VariableGroups);
      foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) this.Environments)
      {
        ReleaseEnvironment env = environment;
        ReleaseEnvironment environmentWithSecrets = releaseWithSecrets.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (x => x.Name == env.Name));
        if (environmentWithSecrets != null)
          env.FillSecrets(environmentWithSecrets);
      }
    }

    public virtual Release DeepClone()
    {
      Release release = new Release()
      {
        CreatedBy = this.CreatedBy,
        CreatedFor = this.CreatedFor,
        CreatedOn = this.CreatedOn,
        DeferredDateTime = this.DeferredDateTime,
        Description = this.Description,
        Id = this.Id,
        IsDeferred = this.IsDeferred,
        KeepForever = this.KeepForever,
        ModifiedBy = this.ModifiedBy,
        ModifiedOn = this.ModifiedOn,
        Name = this.Name,
        ReleaseDefinitionId = this.ReleaseDefinitionId,
        ReleaseDefinitionName = this.ReleaseDefinitionName,
        ReleaseDefinitionPath = this.ReleaseDefinitionPath,
        ReleaseDefinitionRevision = this.ReleaseDefinitionRevision,
        Status = this.Status,
        DefinitionSnapshot = this.DefinitionSnapshot == null ? (ReleaseDefinitionEnvironmentsSnapshot) null : this.DefinitionSnapshot.DeepClone(),
        Reason = this.Reason,
        DefinitionSnapshotRevision = this.DefinitionSnapshotRevision
      };
      foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) this.Environments)
        release.Environments.Add(environment.DeepClone());
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.FillVariables(this.Variables, release.Variables);
      release.VariableGroups.AddRange<VariableGroup, IList<VariableGroup>>((IEnumerable<VariableGroup>) VariableGroupUtility.CloneVariableGroups(this.VariableGroups));
      foreach (string tag in (IEnumerable<string>) this.Tags)
        release.Tags.Add(tag);
      foreach (PropertyValue property in (IEnumerable<PropertyValue>) this.Properties)
        release.Properties.Add(property);
      return release;
    }

    public virtual bool HasSecretsWithValues() => this.Environments.Aggregate<ReleaseEnvironment, bool>(Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.HasSecretsWithValues(this.Variables) || VariableGroupUtility.HasSecret(this.VariableGroups), (Func<bool, ReleaseEnvironment, bool>) ((returnVal, env) => returnVal | env.HasSecretsWithValues()));

    public virtual bool HasSecrets() => this.Environments.Aggregate<ReleaseEnvironment, bool>(Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.HasSecrets(this.Variables) || VariableGroupUtility.HasSecret(this.VariableGroups), (Func<bool, ReleaseEnvironment, bool>) ((returnVal, env) => returnVal | env.HasSecrets()));

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupt serialized data")]
    public TimeSpan GetRunDuration()
    {
      DateTime dateTime = this.Environments.Min<ReleaseEnvironment, DateTime>((Func<ReleaseEnvironment, DateTime>) (e => e.GetStepsForTests.Min<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (s => s.CreatedOn))));
      return this.Environments.Max<ReleaseEnvironment, DateTime>((Func<ReleaseEnvironment, DateTime>) (e => e.GetStepsForTests.Max<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (s => s.ModifiedOn)))) - dateTime;
    }

    public ArtifactSource GetArtifact(int artifactSourceId) => this.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (a => a != null && a.Id == artifactSourceId));

    public bool TryGetPrimaryArtifactSource(out int artifactSourceId)
    {
      ArtifactSource artifactSource = this.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (a => a != null && a.IsPrimary));
      if (artifactSource != null)
      {
        artifactSourceId = artifactSource.Id;
        return true;
      }
      artifactSourceId = -1;
      return false;
    }

    public bool TryGetPrimaryArtifactSourceVersion(out InputValue artifactSourceVersion)
    {
      artifactSourceVersion = (InputValue) null;
      int artifactSourceId;
      if (this.TryGetPrimaryArtifactSource(out artifactSourceId))
      {
        ArtifactSource artifact = this.GetArtifact(artifactSourceId);
        artifactSourceVersion = artifact == null || !artifact.SourceData.ContainsKey("version") ? (InputValue) null : artifact.SourceData["version"];
        if (artifactSourceVersion != null)
          return true;
      }
      return false;
    }

    public int NumberOfApprovalsWithSameRank(int definitionEnvironmentId, int rank)
    {
      DefinitionEnvironmentData definitionEnvironmentData = this.DefinitionSnapshot.Environments.FirstOrDefault<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (s => s.Id == definitionEnvironmentId));
      return definitionEnvironmentData == null ? 0 : definitionEnvironmentData.Steps.Count<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (s => s.Rank == rank));
    }

    public bool HasParallelApprovals(int definitionEnvironmentId, int rank) => this.NumberOfApprovalsWithSameRank(definitionEnvironmentId, rank) > 1;

    public DefinitionEnvironmentData GetDefinitionEnvironmentData(int definitionEnvironmentId) => this.DefinitionSnapshot.Environments.SingleOrDefault<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (env => env.Id == definitionEnvironmentId));
  }
}
