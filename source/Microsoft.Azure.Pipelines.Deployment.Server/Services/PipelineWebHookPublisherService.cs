// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PipelineWebHookPublisherService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.DataAccess;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public class PipelineWebHookPublisherService : 
    IPipelineWebHookPublisherService,
    IVssFrameworkService
  {
    private const string c_layer = "PipelineWebHookPublisherService";

    public void CreateWebHookPublisher(
      IVssRequestContext requestContext,
      PipelineWebHookPublisher publisher)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<PipelineWebHookPublisher>(publisher, nameof (publisher));
      using (new MethodScope(requestContext, nameof (PipelineWebHookPublisherService), nameof (CreateWebHookPublisher)))
      {
        using (PipelineWebHookPublisherSqlComponent component = requestContext.CreateComponent<PipelineWebHookPublisherSqlComponent>())
          component.CreateWebHookPublisher(requestContext, publisher);
      }
    }

    public void DeleteWebHookPublisher(
      IVssRequestContext requestContext,
      PipelineWebHookPublisher publisher)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (PipelineWebHookPublisherService), nameof (DeleteWebHookPublisher)))
      {
        using (PipelineWebHookPublisherSqlComponent component = requestContext.CreateComponent<PipelineWebHookPublisherSqlComponent>())
          component.DeleteWebHookPublisher(requestContext, publisher);
      }
    }

    public PipelineWebHookPublisher GetWebHookPublisher(
      IVssRequestContext requestContext,
      PipelineWebHookPublisher publisher)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (PipelineWebHookPublisherService), nameof (GetWebHookPublisher)))
      {
        using (PipelineWebHookPublisherSqlComponent component = requestContext.CreateComponent<PipelineWebHookPublisherSqlComponent>())
          return component.GetWebHookPublisher(requestContext, publisher);
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
