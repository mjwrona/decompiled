// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactTypeService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  internal sealed class ArtifactTypeService : IArtifactService, IVssFrameworkService
  {
    public static readonly string ArtifactsContributionId = "ms.vss-distributed-task.pipeline-artifact";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<IArtifactType> GetArtifactTypes(IVssRequestContext requestContext) => (IEnumerable<IArtifactType>) this.LoadArtifactTypeDefinition(requestContext).Values;

    public IArtifactType GetArtifactType(IVssRequestContext requestContext, string typeId)
    {
      IArtifactType artifactType = (IArtifactType) null;
      if (!this.LoadArtifactTypeDefinition(requestContext).TryGetValue(typeId, out artifactType))
        throw new NotSupportedException(TaskResources.ArtifactTypeNotFound((object) typeId));
      return artifactType;
    }

    private IDictionary<string, IArtifactType> LoadArtifactTypeDefinition(
      IVssRequestContext systemRequestContext)
    {
      List<IArtifactType> source = new List<IArtifactType>()
      {
        (IArtifactType) new PipelineArtifact(),
        (IArtifactType) new TfsGitArtifact(),
        (IArtifactType) new GithubArtifact(),
        (IArtifactType) new BitBucketArtifact()
      };
      if (systemRequestContext.IsFeatureEnabled("DistributedTask.EnableSupportForGitHubEnterpriseArtifactType"))
        source.Add((IArtifactType) new GitHubEnterpriseArtifact());
      source.AddRange(ArtifactTypeService.GetExtensionArtifacts(systemRequestContext));
      List<IGrouping<string, IArtifactType>> list = source.GroupBy<IArtifactType, string>((Func<IArtifactType, string>) (c => c.Type)).Where<IGrouping<string, IArtifactType>>((Func<IGrouping<string, IArtifactType>, bool>) (g => g.Count<IArtifactType>() > 1)).ToList<IGrouping<string, IArtifactType>>();
      if (list.Any<IGrouping<string, IArtifactType>>())
        ArtifactTypeService.ThrowDuplicateIdentifierException((IEnumerable<IGrouping<string, IArtifactType>>) list);
      Dictionary<string, IArtifactType> dictionary = new Dictionary<string, IArtifactType>(source.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IArtifactType artifactType in source)
        dictionary[artifactType.Name] = artifactType;
      return (IDictionary<string, IArtifactType>) dictionary;
    }

    private static IEnumerable<IArtifactType> GetExtensionArtifacts(
      IVssRequestContext requestContext)
    {
      List<IArtifactType> extensionArtifacts = new List<IArtifactType>();
      foreach (Contribution contribution in requestContext.GetService<IContributionService>().QueryContributionsForTarget(requestContext, ArtifactTypeService.ArtifactsContributionId))
      {
        if (contribution.IsFeatureAvailable(requestContext))
        {
          try
          {
            CustomArtifact customArtifact = new CustomArtifact(contribution.Id, contribution.Properties.ToObject<ArtifactContributionDefinition>());
            extensionArtifacts.Add((IArtifactType) customArtifact);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10016109, TraceLevel.Error, "DistributedTask", "Artifacts", ex);
          }
        }
      }
      return (IEnumerable<IArtifactType>) extensionArtifacts;
    }

    private static void ThrowDuplicateIdentifierException(
      IEnumerable<IGrouping<string, IArtifactType>> typeDefinitions)
    {
      string str = string.Empty;
      foreach (IGrouping<string, IArtifactType> typeDefinition in typeDefinitions)
        str = !string.IsNullOrWhiteSpace(str) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0},{1}", (object) str, (object) typeDefinition.Key) : typeDefinition.Key;
      throw new ResourceValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.ArtifactTypeDuplicateIdentifier((object) str)));
    }
  }
}
