// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PythonPipelineFragmentProvider
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
  public class PythonPipelineFragmentProvider : PipelineFragmentProviderBase
  {
    private const string c_workingDirectory = "$(System.DefaultWorkingDirectory)";

    protected override IEnumerable<Template> GetTemplatesInternal(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      PythonPipelineFragmentProvider fragmentProvider = this;
      string a;
      detectedBuildFramework.Settings.TryGetValue(PythonBuildFrameworkDetector.Settings.WebFramework, out a);
      if (string.Equals(a, PythonBuildFrameworkDetector.WebFrameworks.Django, StringComparison.OrdinalIgnoreCase))
        yield return fragmentProvider.GetTemplate(requestContext, TemplateIds.PythonDjango);
      else
        yield return fragmentProvider.GetTemplate(requestContext, TemplateIds.PythonPackage);
      if (detectedBuildFramework.BuildTargets.Any<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, PythonBuildFrameworkDetector.WellKnownTypes.AzureFunctionApp, StringComparison.OrdinalIgnoreCase))))
      {
        List<string> list = detectedBuildFramework.BuildTargets.Where<DetectedBuildTarget>((Func<DetectedBuildTarget, bool>) (t => string.Equals(t.Type, PythonBuildFrameworkDetector.WellKnownTypes.AzureFunctionApp, StringComparison.OrdinalIgnoreCase))).Select<DetectedBuildTarget, string>((Func<DetectedBuildTarget, string>) (t => "$(System.DefaultWorkingDirectory)" + new FilePath(t.Path).Folder.ToString())).ToList<string>();
        list.Sort();
        Template template = fragmentProvider.GetTemplate(requestContext, TemplateIds.Pipelines.PythonFunctionAppToLinuxOnAzure);
        PythonPipelineFragmentProvider.AssignPropertyPossibleValues(template, PythonBuildFrameworkDetector.Settings.WorkingDirectory, (IList<string>) list);
        yield return template;
      }
      yield return fragmentProvider.GetTemplate(requestContext, TemplateIds.Pipelines.PythonToLinuxWebAppOnAzure);
    }

    private static void AssignPropertyPossibleValues(
      Template template,
      string name,
      IList<string> values)
    {
      TemplateParameterDefinition parameterDefinition = template.Parameters.FirstOrDefault<TemplateParameterDefinition>((Func<TemplateParameterDefinition, bool>) (p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)));
      if (parameterDefinition == null || values == null || values.Count <= 0)
        return;
      parameterDefinition.PossibleValues = values;
      parameterDefinition.DefaultValue = values.First<string>();
    }

    protected override bool IsSupportedFramework(DetectedBuildFramework detectedBuildFramework) => string.Equals(detectedBuildFramework.Id, PythonBuildFrameworkDetector.Id, StringComparison.OrdinalIgnoreCase);
  }
}
