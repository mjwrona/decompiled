// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseDefinition
  {
    private const string StrongBoxDrawerFormat = "/Service/ReleaseManagement/{0}/Definitions/{1}";
    private const string StrongBoxSecretVariableKeyPrefixFormat = "{0}/Variables";
    private IList<PropertyValue> properties;

    public ReleaseDefinitionSource Source { get; set; }

    public int Id { get; set; }

    public int Revision { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsDisabled { get; set; }

    public ReleaseReference LastRelease { get; set; }

    public IList<DefinitionEnvironment> Environments { get; private set; }

    public virtual IList<ArtifactSource> LinkedArtifacts { get; set; }

    public IDictionary<string, ConfigurationVariableValue> Variables { get; private set; }

    public IList<int> VariableGroups { get; }

    public virtual IList<ReleaseTriggerBase> Triggers { get; private set; }

    public string ReleaseNameFormat { get; set; }

    public Guid ProjectId { get; set; }

    public string Path { get; set; }

    public IList<string> Tags { get; private set; }

    public PipelineProcessTypes PipelineProcessType { get; set; }

    public PipelineProcess PipelineProcess { get; set; }

    public string Comment { get; set; }

    public ReadOnlyCollection<ArtifactSourceTrigger> ArtifactSourceTriggers => this.Triggers.Where<ReleaseTriggerBase>((Func<ReleaseTriggerBase, bool>) (t => t.TriggerType == ReleaseTriggerType.ArtifactSource)).Select<ReleaseTriggerBase, ArtifactSourceTrigger>((Func<ReleaseTriggerBase, ArtifactSourceTrigger>) (t => (ArtifactSourceTrigger) t)).ToList<ArtifactSourceTrigger>().AsReadOnly();

    public ReadOnlyCollection<ScheduledReleaseTrigger> ScheduledTriggers => this.Triggers.Where<ReleaseTriggerBase>((Func<ReleaseTriggerBase, bool>) (t => t.TriggerType == ReleaseTriggerType.Schedule)).Select<ReleaseTriggerBase, ScheduledReleaseTrigger>((Func<ReleaseTriggerBase, ScheduledReleaseTrigger>) (t => (ScheduledReleaseTrigger) t)).ToList<ScheduledReleaseTrigger>().AsReadOnly();

    public ReadOnlyCollection<SourceRepoTrigger> SourceRepoTriggers => this.Triggers.Where<ReleaseTriggerBase>((Func<ReleaseTriggerBase, bool>) (t => t.TriggerType == ReleaseTriggerType.SourceRepo)).Select<ReleaseTriggerBase, SourceRepoTrigger>((Func<ReleaseTriggerBase, SourceRepoTrigger>) (t => (SourceRepoTrigger) t)).ToList<SourceRepoTrigger>().AsReadOnly();

    public ReadOnlyCollection<ContainerImageTrigger> ContainerImageTriggers => this.Triggers.Where<ReleaseTriggerBase>((Func<ReleaseTriggerBase, bool>) (t => t.TriggerType == ReleaseTriggerType.ContainerImage)).Select<ReleaseTriggerBase, ContainerImageTrigger>((Func<ReleaseTriggerBase, ContainerImageTrigger>) (t => (ContainerImageTrigger) t)).ToList<ContainerImageTrigger>().AsReadOnly();

    public ReadOnlyCollection<PackageTrigger> PackageTriggers => this.Triggers.Where<ReleaseTriggerBase>((Func<ReleaseTriggerBase, bool>) (t => t.TriggerType == ReleaseTriggerType.Package)).Select<ReleaseTriggerBase, PackageTrigger>((Func<ReleaseTriggerBase, PackageTrigger>) (t => (PackageTrigger) t)).ToList<PackageTrigger>().AsReadOnly();

    public ReadOnlyCollection<PullRequestTrigger> PullRequestTriggers => this.Triggers.Where<ReleaseTriggerBase>((Func<ReleaseTriggerBase, bool>) (t => t.TriggerType == ReleaseTriggerType.PullRequest)).Select<ReleaseTriggerBase, PullRequestTrigger>((Func<ReleaseTriggerBase, PullRequestTrigger>) (t => (PullRequestTrigger) t)).ToList<PullRequestTrigger>().AsReadOnly();

    public IList<PropertyValue> Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = (IList<PropertyValue>) new List<PropertyValue>();
        return this.properties;
      }
    }

    public ReleaseDefinition()
    {
      this.Environments = (IList<DefinitionEnvironment>) new List<DefinitionEnvironment>();
      this.LinkedArtifacts = (IList<ArtifactSource>) new List<ArtifactSource>();
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.VariableGroups = (IList<int>) new List<int>();
      this.Triggers = (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
      this.Tags = (IList<string>) new List<string>();
    }

    public ReleaseDefinition DeepClone()
    {
      ReleaseDefinition newDefinition = new ReleaseDefinition()
      {
        Id = this.Id,
        Revision = this.Revision,
        Name = this.Name,
        CreatedBy = this.CreatedBy,
        CreatedOn = this.CreatedOn,
        ModifiedBy = this.ModifiedBy,
        ModifiedOn = this.ModifiedOn,
        IsDeleted = this.IsDeleted,
        IsDisabled = this.IsDisabled
      };
      newDefinition.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) this.VariableGroups);
      this.Environments.ToList<DefinitionEnvironment>().ForEach((Action<DefinitionEnvironment>) (env => newDefinition.Environments.Add(env.DeepClone())));
      this.Triggers.ToList<ReleaseTriggerBase>().ForEach((Action<ReleaseTriggerBase>) (trigger => newDefinition.Triggers.Add(trigger.DeepClone())));
      this.Tags.ToList<string>().ForEach((Action<string>) (tag => newDefinition.Tags.Add(tag)));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.FillVariables(this.Variables, newDefinition.Variables);
      if (this.Properties != null && this.Properties.Any<PropertyValue>())
      {
        foreach (PropertyValue property in (IEnumerable<PropertyValue>) this.Properties)
          newDefinition.Properties.Add(property);
      }
      return newDefinition;
    }

    public void FillSecrets(ReleaseDefinition definitionWithSecrets)
    {
      if (definitionWithSecrets == null)
        throw new ArgumentNullException(nameof (definitionWithSecrets));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.FillSecrets(definitionWithSecrets.Variables, this.Variables);
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) this.Environments)
      {
        DefinitionEnvironment env = environment;
        DefinitionEnvironment environmentWithSecrets = definitionWithSecrets.Environments.SingleOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (x => x.Name == env.Name));
        if (environmentWithSecrets != null)
          env.FillSecrets(environmentWithSecrets);
      }
    }

    public virtual string GetDrawerName(Guid projectIdentifier) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/ReleaseManagement/{0}/Definitions/{1}", (object) projectIdentifier, (object) this.Id);

    public virtual string SecretVariableLookupPrefix => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/Variables", (object) this.Revision);

    public virtual bool HasSecretsWithValues() => this.Environments.Aggregate<DefinitionEnvironment, bool>(Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.HasSecretsWithValues(this.Variables), (Func<bool, DefinitionEnvironment, bool>) ((returnVal, env) => returnVal | env.HasSecretsWithValues()));

    public virtual bool HasSecrets() => this.Environments.Aggregate<DefinitionEnvironment, bool>(Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.HasSecrets(this.Variables), (Func<bool, DefinitionEnvironment, bool>) ((returnVal, env) => returnVal | env.HasSecrets()));

    public DefinitionEnvironment GetEnvironment(int definitionEnvironmentId) => this.Environments.SingleOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Id == definitionEnvironmentId));

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Computed properties corrupt serialized data")]
    public DefinitionEnvironment GetTargetedEnvironment() => !this.ArtifactSourceTriggers.Any<ArtifactSourceTrigger>() ? (DefinitionEnvironment) null : this.Environments.SingleOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Id == this.ArtifactSourceTriggers.First<ArtifactSourceTrigger>().TargetEnvironmentId));

    public bool TryGetPrimaryArtifactSource(
      IVssRequestContext requestContext,
      out int artifactSourceId)
    {
      artifactSourceId = -1;
      if (!this.LinkedArtifacts.Any<ArtifactSource>())
        return false;
      ArtifactSource artifactSource = this.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.IsPrimary));
      if (artifactSource != null)
      {
        artifactSourceId = artifactSource.Id;
      }
      else
      {
        artifactSourceId = this.LinkedArtifacts.Min<ArtifactSource>((Func<ArtifactSource, int>) (a => a.Id));
        requestContext.Trace(1960095, TraceLevel.Warning, "ReleaseManagementService", "Service", Resources.NoPrimaryArtifactInReleaseDefinition, (object) this.Id);
      }
      return true;
    }

    public ArtifactSource GetArtifact(int artifactSourceId) => this.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (a => a != null && a.Id == artifactSourceId));

    public bool IsYamlDefinition(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.Yaml.Preview") && this.PipelineProcessType == PipelineProcessTypes.Yaml;
  }
}
