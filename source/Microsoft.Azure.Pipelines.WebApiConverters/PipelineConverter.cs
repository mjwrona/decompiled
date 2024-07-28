// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.PipelineConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class PipelineConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<PipelineConverter>(1)
    });

    public virtual Microsoft.Azure.Pipelines.WebApi.Pipeline ToWebApiPipeline(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Pipeline source,
      bool includeConfiguration)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.Pipeline) null;
      ISecuredObject securedObject = source.ToSecuredObject();
      Microsoft.Azure.Pipelines.WebApi.Pipeline pipeline = new Microsoft.Azure.Pipelines.WebApi.Pipeline(securedObject);
      pipeline.Id = source.Id;
      pipeline.Revision = source.Revision;
      pipeline.Folder = source.Folder;
      pipeline.Name = source.Name;
      Microsoft.Azure.Pipelines.WebApi.Pipeline webApiPipeline = pipeline;
      if (includeConfiguration)
        webApiPipeline.Configuration = source.GetConfiguration(requestContext).ToWebApiConfiguration(requestContext, securedObject);
      IPipelinesRouteService service = requestContext.GetService<IPipelinesRouteService>();
      webApiPipeline.Links.AddLink("self", securedObject, service.GetPipelineRestUrl(requestContext, source.ProjectId, source.Id, new int?(source.Revision)));
      webApiPipeline.Links.AddLink("web", securedObject, service.GetPipelineWebUrl(requestContext, source.ProjectId, source.Id));
      return webApiPipeline;
    }

    public Microsoft.Azure.Pipelines.WebApi.PipelineReference ToWebApiPipelineReference(
      IVssRequestContext requestContext,
      Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineReference source,
      ISecuredObject securingObject)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.PipelineReference) null;
      IPipelinesRouteService service = requestContext.GetService<IPipelinesRouteService>();
      Microsoft.Azure.Pipelines.WebApi.PipelineReference pipelineReference = new Microsoft.Azure.Pipelines.WebApi.PipelineReference(securingObject);
      pipelineReference.Id = source.Id;
      pipelineReference.Revision = source.Revision;
      pipelineReference.Folder = source.Folder;
      pipelineReference.Name = source.Name;
      pipelineReference.Url = service.GetPipelineRestUrl(requestContext, source.ProjectId, source.Id, new int?(source.Revision));
      return pipelineReference;
    }
  }
}
