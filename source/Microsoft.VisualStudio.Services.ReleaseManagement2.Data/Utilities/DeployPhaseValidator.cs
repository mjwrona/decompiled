// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeployPhaseValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public abstract class DeployPhaseValidator : BasePhaseValidator
  {
    protected DeployPhaseValidator(string deployPhaseName)
      : base(deployPhaseName)
    {
    }

    protected override IDictionary<string, string> GetInvalidInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      Dictionary<string, string> invalidInputs = new Dictionary<string, string>();
      DeploymentInput deploymentInput1 = (DeploymentInput) deploymentInput;
      if (deploymentInput1.QueueId < 0)
        invalidInputs.Add("QueueId", deploymentInput1.QueueId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return (IDictionary<string, string>) invalidInputs;
    }

    public override void ValidateArtifactsDownloadInput(
      BaseDeploymentInput deploymentInput,
      IList<Artifact> linkedArtifacts)
    {
      DeploymentInput deploymentInput1 = (DeploymentInput) deploymentInput;
      if (deploymentInput1.ArtifactsDownloadInput.DownloadInputs.IsNullOrEmpty<ArtifactDownloadInputBase>())
        return;
      if (deploymentInput1.SkipArtifactsDownload)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidSkipArtifactsDownloadInDeploymentInput, (object) this.DeployPhaseName));
      Dictionary<string, string> linkedArtifactAliasToArtifactTypeMap = linkedArtifacts != null ? linkedArtifacts.ToDictionary<Artifact, string, string>((Func<Artifact, string>) (a => a.Alias), (Func<Artifact, string>) (a => a.Type), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ArtifactDownloadInputBase downloadInput in (IEnumerable<ArtifactDownloadInputBase>) deploymentInput1.ArtifactsDownloadInput.DownloadInputs)
        downloadInput.ValidateArtifactDownloadInput(this.DeployPhaseName, (IDictionary<string, string>) linkedArtifactAliasToArtifactTypeMap);
      IEnumerable<ArtifactDownloadInputBase> downloadInputBases = deploymentInput1.ArtifactsDownloadInput.DownloadInputs.Where<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (downloadInput => downloadInput.IsNonTaskifiedArtifact()));
      if (!downloadInputBases.IsNullOrEmpty<ArtifactDownloadInputBase>() && !downloadInputBases.All<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (downloadInput => downloadInput.ArtifactDownloadMode.Equals("Skip", StringComparison.OrdinalIgnoreCase))) && !downloadInputBases.All<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (downloadInput => downloadInput.ArtifactDownloadMode.Equals("All", StringComparison.OrdinalIgnoreCase))))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidArtifactsDownloadForNonTaskifiedArtifacts, (object) this.DeployPhaseName, (object) string.Join(", ", downloadInputBases.Select<ArtifactDownloadInputBase, string>((Func<ArtifactDownloadInputBase, string>) (downloadInput => downloadInput.Alias)))));
    }

    protected override IList<string> GetModifiedProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      DeploymentInput deploymentInput = webApiDeploymentInput != null ? (DeploymentInput) webApiDeploymentInput : throw new ArgumentNullException(nameof (webApiDeploymentInput));
      DeploymentInput deploymentInput2 = (DeploymentInput) serverDeploymentInput;
      List<string> modifiedProperties = new List<string>();
      if (!DeployPhaseValidator.AreDemandsSame(deploymentInput.Demands, deploymentInput2.Demands))
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<IList<Demand>>((Expression<Func<IList<Demand>>>) (() => deploymentInput2.Demands)));
      if (deploymentInput.QueueId != deploymentInput2.QueueId)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => deploymentInput2.QueueId)));
      if (deploymentInput.SkipArtifactsDownload != deploymentInput2.SkipArtifactsDownload)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<bool>((Expression<Func<bool>>) (() => deploymentInput2.SkipArtifactsDownload)));
      if (!deploymentInput.ArtifactsDownloadInput.Equals(deploymentInput2.ArtifactsDownloadInput))
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<ArtifactsDownloadInput>((Expression<Func<ArtifactsDownloadInput>>) (() => deploymentInput2.ArtifactsDownloadInput)));
      if (deploymentInput.EnableAccessToken != deploymentInput2.EnableAccessToken)
        modifiedProperties.Add(BasePhaseValidator.GetPropertyName<bool>((Expression<Func<bool>>) (() => deploymentInput2.EnableAccessToken)));
      return (IList<string>) modifiedProperties;
    }

    protected override IList<string> GetModifiedImmutableProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      if (webApiDeploymentInput == null)
        throw new ArgumentNullException(nameof (webApiDeploymentInput));
      return (IList<string>) new List<string>();
    }

    private static bool AreDemandsSame(IList<Demand> serverDemands, IList<Demand> webApiDemands)
    {
      if (serverDemands == null)
        return webApiDemands == null || webApiDemands.Count == 0;
      if (webApiDemands == null)
        return serverDemands.Count == 0;
      if (serverDemands.Select<Demand, string>((Func<Demand, string>) (i => i.Name)).Distinct<string>().Count<string>() != webApiDemands.Select<Demand, string>((Func<Demand, string>) (i => i.Name)).Distinct<string>().Count<string>())
        return false;
      foreach (Demand serverDemand1 in (IEnumerable<Demand>) serverDemands)
      {
        Demand serverDemand = serverDemand1;
        Demand demand = webApiDemands.FirstOrDefault<Demand>((Func<Demand, bool>) (d => d.Name == serverDemand.Name));
        if (demand == null || serverDemand.Value != demand.Value)
          return false;
      }
      return true;
    }
  }
}
