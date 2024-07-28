// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ArtifactTypeDefinitionConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ArtifactTypeDefinitionConverter
  {
    private static ArtifactTypeDefinition ToWebApi(ArtifactTypeBase serverDefinition)
    {
      if (serverDefinition == null)
        throw new ArgumentNullException(nameof (serverDefinition));
      return new ArtifactTypeDefinition()
      {
        Name = serverDefinition.Name,
        EndpointTypeId = serverDefinition.EndpointTypeId,
        DisplayName = serverDefinition.DisplayName,
        UniqueSourceIdentifier = serverDefinition.UniqueSourceIdentifier,
        InputDescriptors = serverDefinition.InputDescriptors,
        ArtifactType = serverDefinition.ArtifactType,
        ArtifactTriggerConfiguration = ArtifactTypeDefinitionConverter.ShouldAddTriggerConfiguration(serverDefinition.Name) ? ArtifactTypeDefinitionConverter.ToArtifactTriggerConfiguration(serverDefinition.ArtifactTriggerConfiguration) : (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ArtifactTriggerConfiguration) null,
        IsCommitsTraceabilitySupported = serverDefinition.IsCommitsTraceabilitySupported,
        IsWorkitemsTraceabilitySupported = serverDefinition.IsWorkitemsTraceabilitySupported
      };
    }

    private static bool ShouldAddTriggerConfiguration(string artifactType) => !new List<string>()
    {
      "AzureContainerRepository"
    }.Any<string>((Func<string, bool>) (x => x.Equals(artifactType)));

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ArtifactTriggerConfiguration ToArtifactTriggerConfiguration(
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactTriggerConfiguration configuration)
    {
      if (configuration == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ArtifactTriggerConfiguration) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ArtifactTriggerConfiguration()
      {
        IsTriggerSupported = configuration.IsTriggerSupported,
        IsTriggerSupportedOnlyInHosted = configuration.IsTriggerSupportedOnlyInHosted,
        IsWebhookSupportedAtServerLevel = configuration.IsWebhookSupportedAtServerLevel,
        PayloadHashHeaderName = configuration.PayloadHashHeaderName,
        Resources = configuration.Resources,
        WebhookPayloadMapping = configuration.WebhookPayloadMapping
      };
    }

    public static IEnumerable<ArtifactTypeDefinition> ConvertToContractList(
      IEnumerable<ArtifactTypeBase> serverDefinitionList)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return serverDefinitionList.Select<ArtifactTypeBase, ArtifactTypeDefinition>(ArtifactTypeDefinitionConverter.\u003C\u003EO.\u003C0\u003E__ToWebApi ?? (ArtifactTypeDefinitionConverter.\u003C\u003EO.\u003C0\u003E__ToWebApi = new Func<ArtifactTypeBase, ArtifactTypeDefinition>(ArtifactTypeDefinitionConverter.ToWebApi)));
    }
  }
}
