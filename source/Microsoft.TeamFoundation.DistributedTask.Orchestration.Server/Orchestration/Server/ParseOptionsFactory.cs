// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ParseOptionsFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class ParseOptionsFactory
  {
    public static ParseOptions CreateParseOptions(
      IVssRequestContext requestContext,
      RetrieveOptions retrieveOptions)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return new ParseOptions()
      {
        EnableDynamicVariables = requestContext.IsFeatureEnabled("DistributedTask.YamlDynamicVariables"),
        EnableEachExpressions = requestContext.IsFeatureEnabled("DistributedTask.YamlEachExpression"),
        EnableParameterReferenceErrors = requestContext.IsFeatureEnabled("DistributedTask.YamlParameterReferenceErrors"),
        EnableStages = requestContext.IsFeatureEnabled("DistributedTask.EnableStages"),
        EnableTelemetry = requestContext.IsFeatureEnabled("DistributedTask.YamlTelemetry"),
        MaxFiles = service.GetValue<int>(requestContext, in RegistryKeys.PipelineParserMaxFiles, false, 100),
        MaxResultSize = service.GetValue<int>(requestContext, in RegistryKeys.PipelineParserMaxResultSize, false, 20971520),
        MaxFileSize = service.GetValue<int>(requestContext, in RegistryKeys.PipelineParserMaxFileSize, false, 1048576),
        MaxParseEvents = service.GetValue<int>(requestContext, in RegistryKeys.PipelineParserMaxEvents, false, 10000000),
        MaxDepth = service.GetValue<int>(requestContext, in RegistryKeys.PipelineParserMaxDepth, false, 100),
        RetrieveOptions = retrieveOptions,
        RunJobsWithDemandsOnSingleHostedPool = requestContext.IsFeatureEnabled("DistributedTask.RunJobsWithDemandsOnSingleHostedPool") && !service.GetValue<bool>(requestContext, (RegistryQuery) RegistryKeys.DemandsOnSingleHostedPoolBlockedRegistrySettingsPath, false),
        EvaluateAfterAddingToVariablesMap = requestContext.IsFeatureEnabled("DistributedTask.EvaluateAfterAddingToVariablesMap"),
        AllowTemplateExpressionsInRef = requestContext.IsFeatureEnabled("DistributedTask.AllowTemplateExpressionsInRepositoryResourcesRef")
      };
    }
  }
}
