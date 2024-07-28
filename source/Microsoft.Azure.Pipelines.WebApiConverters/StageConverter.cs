// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.StageConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class StageConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<StageConverter>(1)
    });

    public Microsoft.Azure.Pipelines.WebApi.Stage ToWebApiStage(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Stage source,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Run run)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.Stage) null;
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Server.ObjectModel.Run>(run, nameof (run));
      ISecuredObject securedObject = run.ToSecuredObject();
      Microsoft.Azure.Pipelines.WebApi.Stage webApiStage = new Microsoft.Azure.Pipelines.WebApi.Stage(securedObject);
      webApiStage.Id = source.Id;
      webApiStage.Name = source.Name;
      webApiStage.DisplayName = source.DisplayName;
      webApiStage.State = (Microsoft.Azure.Pipelines.WebApi.StageState) source.State;
      Microsoft.Azure.Pipelines.Server.ObjectModel.StageResult? result = source.Result;
      webApiStage.Result = result.HasValue ? new Microsoft.Azure.Pipelines.WebApi.StageResult?((Microsoft.Azure.Pipelines.WebApi.StageResult) result.GetValueOrDefault()) : new Microsoft.Azure.Pipelines.WebApi.StageResult?();
      webApiStage.FinishTime = source.FinishTime;
      webApiStage.StartTime = source.StartTime;
      webApiStage.Attempt = source.Attempt;
      IPipelinesRouteService service = requestContext.GetService<IPipelinesRouteService>();
      webApiStage.Links.AddLink("web", securedObject, service.GetRunWebUrl(requestContext, source.ProjectId, run.Pipeline.Id, source.RunId));
      webApiStage.Links.AddLink("pipeline.web", securedObject, service.GetPipelineWebUrl(requestContext, source.ProjectId, run.Pipeline.Id));
      return webApiStage;
    }

    public Microsoft.Azure.Pipelines.WebApi.StageOld ToWebApiStageOld(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Server.ObjectModel.StageOld source,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Run run)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.StageOld) null;
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Server.ObjectModel.Run>(run, nameof (run));
      ISecuredObject securedObject = run.ToSecuredObject();
      Microsoft.Azure.Pipelines.WebApi.StageOld webApiStageOld = new Microsoft.Azure.Pipelines.WebApi.StageOld(securedObject);
      webApiStageOld.Id = source.Id;
      webApiStageOld.Name = source.Name;
      webApiStageOld.DisplayName = source.DisplayName;
      webApiStageOld.State = (Microsoft.Azure.Pipelines.WebApi.StageState) source.State;
      Microsoft.Azure.Pipelines.Server.ObjectModel.StageResult? result = source.Result;
      webApiStageOld.Result = result.HasValue ? new Microsoft.Azure.Pipelines.WebApi.StageResult?((Microsoft.Azure.Pipelines.WebApi.StageResult) result.GetValueOrDefault()) : new Microsoft.Azure.Pipelines.WebApi.StageResult?();
      IPipelinesRouteService service = requestContext.GetService<IPipelinesRouteService>();
      webApiStageOld.Links.AddLink("web", securedObject, service.GetRunWebUrl(requestContext, source.ProjectId, run.Pipeline.Id, source.RunId));
      webApiStageOld.Links.AddLink("pipeline.web", securedObject, service.GetPipelineWebUrl(requestContext, source.ProjectId, run.Pipeline.Id));
      return webApiStageOld;
    }
  }
}
