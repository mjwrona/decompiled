// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.FallbackPipelineFragmentProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class FallbackPipelineFragmentProvider : PipelineFragmentProviderBase
  {
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> s_buildFrameworkMapping = (IReadOnlyDictionary<string, IReadOnlyList<string>>) new Dictionary<string, IReadOnlyList<string>>()
    {
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ant,
        (IReadOnlyList<string>) new string[1]
        {
          TemplateIds.Ant
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Go,
        (IReadOnlyList<string>) new string[1]
        {
          TemplateIds.Go
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Gradle,
        (IReadOnlyList<string>) new string[2]
        {
          TemplateIds.Gradle,
          TemplateIds.Android
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Jekyll,
        (IReadOnlyList<string>) new string[1]
        {
          TemplateIds.Pipelines.JekyllContainer
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Make,
        (IReadOnlyList<string>) new string[1]
        {
          TemplateIds.Pipelines.Gcc
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Maven,
        (IReadOnlyList<string>) new string[2]
        {
          TemplateIds.Maven,
          TemplateIds.Pipelines.MavenWebAppToLinuxOnAzure
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ruby,
        (IReadOnlyList<string>) new string[1]
        {
          TemplateIds.Ruby
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Static,
        (IReadOnlyList<string>) new string[1]
        {
          TemplateIds.Html
        }
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.XCode,
        (IReadOnlyList<string>) new string[1]
        {
          TemplateIds.Xcode
        }
      }
    };

    protected override IEnumerable<Template> GetTemplatesInternal(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      FallbackPipelineFragmentProvider fragmentProvider = this;
      IReadOnlyList<string> stringList;
      if (FallbackPipelineFragmentProvider.s_buildFrameworkMapping.TryGetValue(detectedBuildFramework.Id, out stringList))
      {
        foreach (string templateId in (IEnumerable<string>) stringList)
        {
          Template template;
          try
          {
            template = fragmentProvider.GetTemplate(requestContext, templateId);
          }
          catch (TemplateNotFoundException ex)
          {
            continue;
          }
          yield return template;
        }
      }
    }

    protected override bool IsSupportedFramework(DetectedBuildFramework detectedBuildFramework) => FallbackBuildFrameworkDetector.IsFallbackBuildFrameworkDetectorId(detectedBuildFramework.Id);
  }
}
