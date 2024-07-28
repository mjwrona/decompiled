// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseConverter
  {
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Cannot avoid this as this is a Release object which is a core contract")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release ConvertModelToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IVssRequestContext context,
      Guid projectId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "Service", "ReleaseConverter.ConvertModelToContract", 1961073))
      {
        string str1 = release != null ? release.CreatedBy.ToString() : throw new ArgumentNullException(nameof (release));
        string str2 = release.CreatedFor.ToString();
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release()
        {
          Id = release.Id,
          ProjectReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ProjectReference()
          {
            Id = projectId
          },
          Name = release.Name,
          Status = release.Status.ToWebApi(),
          CreatedOn = release.CreatedOn,
          ModifiedOn = release.ModifiedOn,
          ModifiedBy = new IdentityRef()
          {
            Id = release.ModifiedBy.ToString()
          },
          CreatedBy = new IdentityRef() { Id = str1 },
          CreatedFor = new IdentityRef() { Id = str2 },
          Description = release.Description,
          ReleaseDefinitionReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
          {
            Name = release.ReleaseDefinitionName,
            Id = release.ReleaseDefinitionId,
            Path = release.ReleaseDefinitionPath
          },
          ReleaseDefinitionRevision = release.ReleaseDefinitionRevision,
          KeepForever = release.KeepForever,
          Reason = release.Reason.ToWebApi(),
          ReleaseNameFormat = release.ReleaseNameFormat,
          DefinitionSnapshotRevision = release.DefinitionSnapshotRevision,
          Tags = release.Tags
        };
        contract.TriggeringArtifactAlias = release.TriggeringArtifactAlias;
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) release.Environments)
          contract.Environments.Add(environment.ToWebApi(release, context, projectId));
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) release.VariableGroups)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup apiVariableGroup = VariableGroupConverter.ToWebApiVariableGroup(variableGroup);
          if (apiVariableGroup != null)
            contract.VariableGroups.Add(apiVariableGroup);
        }
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) release.Variables)
          contract.Variables[variable.Key] = variable.Value.ToWebApiConfigurationVariableValue();
        foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) release.LinkedArtifacts)
          contract.Artifacts.Add(linkedArtifact.ToWebApi(context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion")));
        contract.ReleaseDefinitionReference = ShallowReferencesHelper.CreateDefinitionShallowReference(context, projectId, release.ReleaseDefinitionId, release.ReleaseDefinitionName, release.ReleaseDefinitionPath);
        string releaseRestUrl = WebAccessUrlBuilder.GetReleaseRestUrl(context, projectId, release.Id);
        string releaseWebAccessUri = WebAccessUrlBuilder.GetReleaseWebAccessUri(context, projectId.ToString(), release.Id);
        string definitionRestUrl = WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(context, projectId, release.ReleaseDefinitionId);
        string definitionWebAccessUri = WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(context, projectId.ToString(), release.ReleaseDefinitionId);
        contract.Url = releaseRestUrl;
        contract.Links.AddLink("self", releaseRestUrl);
        contract.Links.AddLink("web", releaseWebAccessUri);
        contract.ReleaseDefinitionReference.Url = definitionRestUrl;
        contract.ReleaseDefinitionReference.Links.AddLink("self", definitionRestUrl);
        contract.ReleaseDefinitionReference.Links.AddLink("web", definitionWebAccessUri);
        contract.LogsContainerUrl = WebAccessUrlBuilder.GetReleaseLogsContainerUrl(context, projectId, release.Id);
        if (release.Properties != null && release.Properties.Any<PropertyValue>())
        {
          foreach (PropertyValue property in (IEnumerable<PropertyValue>) release.Properties)
            contract.Properties.TryAdd<string, object>(property.PropertyName, property.Value);
        }
        return contract;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be reviewed")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release ConvertContractToModel(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release releaseContract,
      bool isDefaultToLatestArtifactVersionEnabled)
    {
      if (releaseContract == null)
        throw new ArgumentNullException(nameof (releaseContract));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release model = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release()
      {
        Id = releaseContract.Id,
        Name = releaseContract.Name,
        Status = releaseContract.Status.FromWebApi(),
        CreatedOn = releaseContract.CreatedOn,
        ModifiedOn = releaseContract.ModifiedOn,
        Description = releaseContract.Description,
        KeepForever = releaseContract.KeepForever,
        Reason = releaseContract.Reason.FromWebApi(),
        DefinitionSnapshotRevision = releaseContract.DefinitionSnapshotRevision
      };
      model.TriggeringArtifactAlias = releaseContract.TriggeringArtifactAlias;
      Guid result1;
      if (releaseContract.CreatedBy != null && Guid.TryParse(releaseContract.CreatedBy.Id, out result1))
        model.CreatedBy = result1;
      Guid result2;
      if (releaseContract.CreatedFor != null && Guid.TryParse(releaseContract.CreatedFor.Id, out result2))
        model.CreatedFor = new Guid?(result2);
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> source = new List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup variableGroup in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>) releaseContract.VariableGroups)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup serverVariableGroup = VariableGroupConverter.ToServerVariableGroup(variableGroup);
        if (serverVariableGroup != null)
          source.Add(serverVariableGroup);
      }
      model.VariableGroups.AddRange<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>>((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroupUtility.CloneVariableGroups((IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) source));
      model.Tags.AddRange<string, IList<string>>((IEnumerable<string>) releaseContract.Tags);
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) releaseContract.Variables)
        model.Variables[variable.Key] = variable.Value.ToServerConfigurationVariableValue();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) releaseContract.Artifacts)
        model.LinkedArtifacts.Add(artifact.FromWebApi(isDefaultToLatestArtifactVersionEnabled));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) releaseContract.Environments)
        model.Environments.Add(environment.FromWebApi());
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) releaseContract.Properties)
        model.Properties.Add(new PropertyValue(property.Key, property.Value));
      return model;
    }
  }
}
