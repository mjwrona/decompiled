// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.MsBuildPipelineFragmentProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class MsBuildPipelineFragmentProvider : PipelineFragmentProviderBase
  {
    protected override IEnumerable<Template> GetTemplatesInternal(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      List<string> source = new List<string>();
      if (detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.WebSite, StringComparison.OrdinalIgnoreCase))))
        source.Add(TemplateIds.Aspnet);
      if (detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.AspNetCore, StringComparison.OrdinalIgnoreCase))))
        source.Add(TemplateIds.AspnetCoreNetFramework);
      if (detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.Exe, StringComparison.OrdinalIgnoreCase))))
        source.Add(TemplateIds.NetDesktop);
      if (detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.UniversalWindowsPlatform, StringComparison.OrdinalIgnoreCase))))
        source.Add(TemplateIds.UniversalWindowsPlatform);
      int num = detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.XamarinAndroid, StringComparison.OrdinalIgnoreCase))) ? 1 : 0;
      bool flag = detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.XamarinIos, StringComparison.OrdinalIgnoreCase)));
      if (num != 0)
        source.Add(TemplateIds.Pipelines.XamarinAndroid);
      if (flag)
        source.Add(TemplateIds.Pipelines.XamarinIos);
      if (detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.SolutionFile, StringComparison.OrdinalIgnoreCase))))
      {
        if (!source.Contains<string>(TemplateIds.Aspnet, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TemplateIds.Aspnet);
        if (!source.Contains<string>(TemplateIds.AspnetCoreNetFramework, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TemplateIds.AspnetCoreNetFramework);
        if (!source.Contains<string>(TemplateIds.NetDesktop, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TemplateIds.NetDesktop);
        if (!source.Contains<string>(TemplateIds.UniversalWindowsPlatform, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TemplateIds.UniversalWindowsPlatform);
        if (!source.Contains<string>(TemplateIds.Pipelines.XamarinAndroid, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TemplateIds.Pipelines.XamarinAndroid);
        if (!source.Contains<string>(TemplateIds.Pipelines.XamarinIos, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source.Add(TemplateIds.Pipelines.XamarinIos);
      }
      return source.Select<string, Template>((Func<string, Template>) (id => this.GetTemplate(requestContext, id)));
    }

    protected override bool IsSupportedFramework(DetectedBuildFramework detectedBuildFramework) => string.Equals(detectedBuildFramework.Id, MsBuildBuildFrameworkDetector.Id, StringComparison.OrdinalIgnoreCase);
  }
}
