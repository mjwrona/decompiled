// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.YamlSchemaController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "yamlschema")]
  public class YamlSchemaController : DistributedTaskApiController
  {
    [HttpGet]
    public virtual object GetYamlSchema(bool validateTaskNames = true)
    {
      IList<TaskDefinition> taskDefinitions = this.TaskService.GetTaskDefinitions(this.TfsRequestContext);
      IList<TaskDefinition> allTasks = this.TfsRequestContext.IsFeatureEnabled("DistributedTask.YamlShowAllTasks") ? this.TaskService.GetTaskDefinitions(this.TfsRequestContext, allVersions: true) : (IList<TaskDefinition>) new List<TaskDefinition>();
      if (!this.TfsRequestContext.IsFeatureEnabled("DistributedTask.YamlDynamicIntellisense"))
        return this.TfsRequestContext.GetService<IYamlSchemaService>().GetYamlSchema(this.TfsRequestContext, taskDefinitions);
      YamlPipelineLoaderService.PipelineTraceWriter writer = new YamlPipelineLoaderService.PipelineTraceWriter(this.TfsRequestContext);
      return PipelineParser.GetPipelineSchema(taskDefinitions, (ITraceWriter) writer, ParseOptionsFactory.CreateParseOptions(this.TfsRequestContext, RetrieveOptions.Default), validateTaskNames, allTasks);
    }
  }
}
