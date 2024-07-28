// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.JobConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class JobConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<JobConverter>(1)
    });

    public Microsoft.Azure.Pipelines.WebApi.Job ToWebApiJob(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Job source,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Run run)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.Job) null;
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Server.ObjectModel.Run>(run, nameof (run));
      ISecuredObject securedObject = run.ToSecuredObject();
      Microsoft.Azure.Pipelines.WebApi.Job webApiJob = new Microsoft.Azure.Pipelines.WebApi.Job(source.Id);
      webApiJob.Name = source.Name;
      webApiJob.State = (Microsoft.Azure.Pipelines.WebApi.JobState) source.State.Value;
      Microsoft.Azure.Pipelines.Server.ObjectModel.JobResult? result = source.Result;
      webApiJob.Result = result.HasValue ? new Microsoft.Azure.Pipelines.WebApi.JobResult?((Microsoft.Azure.Pipelines.WebApi.JobResult) result.GetValueOrDefault()) : new Microsoft.Azure.Pipelines.WebApi.JobResult?();
      webApiJob.FinishTime = source.FinishTime;
      webApiJob.StartTime = source.StartTime;
      webApiJob.Attempt = source.Attempt;
      IPipelinesRouteService service = requestContext.GetService<IPipelinesRouteService>();
      webApiJob.Links.AddLink("web", securedObject, service.GetRunWebUrl(requestContext, run.ProjectId, run.Pipeline.Id, run.Id));
      webApiJob.Links.AddLink("pipeline.web", securedObject, service.GetPipelineWebUrl(requestContext, run.ProjectId, run.Pipeline.Id));
      return webApiJob;
    }
  }
}
