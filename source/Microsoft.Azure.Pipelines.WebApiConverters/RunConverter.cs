// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.RunConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class RunConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<RunConverter>(1)
    });
    private const int ResourceVersion = 1;

    public virtual Microsoft.Azure.Pipelines.WebApi.Run ToWebApiRun(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Run source)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.Run) null;
      ISecuredObject securedObject = source.ToSecuredObject();
      Microsoft.Azure.Pipelines.WebApi.Run run = new Microsoft.Azure.Pipelines.WebApi.Run(securedObject);
      run.Id = source.Id;
      run.Name = source.Name;
      run.State = (Microsoft.Azure.Pipelines.WebApi.RunState) source.State;
      Microsoft.Azure.Pipelines.Server.ObjectModel.RunResult? result = source.Result;
      run.Result = result.HasValue ? new Microsoft.Azure.Pipelines.WebApi.RunResult?((Microsoft.Azure.Pipelines.WebApi.RunResult) result.GetValueOrDefault()) : new Microsoft.Azure.Pipelines.WebApi.RunResult?();
      run.CreatedDate = source.CreatedDate;
      run.FinishedDate = source.FinishedDate;
      run.FinalYaml = source.FinalYaml;
      Microsoft.Azure.Pipelines.WebApi.Run webApiRun = run;
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.Server.ObjectModel.Variable> variable in source.Variables)
        webApiRun.Variables.Add(variable.Key, new Microsoft.Azure.Pipelines.WebApi.Variable(securedObject)
        {
          IsSecret = variable.Value.IsSecret,
          Value = variable.Value.Value
        });
      if (source.Resources != null)
      {
        RunResourcesConverter versionedObject = requestContext.CreateVersionedObject<RunResourcesConverter>(1);
        webApiRun.Resources = versionedObject.ToWebApiRunResources(requestContext, source.Resources, source);
      }
      IPipelinesRouteService service = requestContext.GetService<IPipelinesRouteService>();
      webApiRun.Links.AddLink("self", securedObject, service.GetRunRestUrl(requestContext, source.ProjectId, source.Pipeline.Id, source.Id));
      webApiRun.Links.AddLink("web", securedObject, service.GetRunWebUrl(requestContext, source.ProjectId, source.Pipeline.Id, source.Id));
      webApiRun.Links.AddLink("pipeline.web", securedObject, service.GetPipelineWebUrl(requestContext, source.ProjectId, source.Pipeline.Id));
      PipelineConverter versionedObject1 = requestContext.CreateVersionedObject<PipelineConverter>(1);
      webApiRun.Pipeline = versionedObject1.ToWebApiPipelineReference(requestContext, source.Pipeline, source.ToSecuredObject());
      webApiRun.Links.AddLink("pipeline", securedObject, webApiRun.Pipeline.Url);
      if (requestContext.IsFeatureEnabled("Build2.GetRunRestAPIConsumedResources"))
        webApiRun.TemplateParameters = source.TemplateParameters;
      return webApiRun;
    }

    public RunReference ToWebApiRunReference(IVssRequestContext requestContext, Microsoft.Azure.Pipelines.Server.ObjectModel.Run source)
    {
      if (source == null)
        return (RunReference) null;
      return new RunReference(source.ToSecuredObject())
      {
        Id = source.Id,
        Name = source.Name
      };
    }
  }
}
