// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Extensions.PipelineWebHookExtension
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.DataAccess;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Azure.Pipelines.Deployment.Extensions
{
  public class PipelineWebHookExtension : IWebHookExtension
  {
    private const string c_layer = "PipelineWebHookExtension";

    public string ServiceName => "Pipeline";

    public void AddWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier)
    {
      throw new NotImplementedException();
    }

    public Guid CreateWebHookName(
      IVssRequestContext requestContext,
      Guid webHookId,
      string webHookName)
    {
      using (new MethodScope(requestContext, nameof (PipelineWebHookExtension), nameof (CreateWebHookName)))
      {
        using (PipelineWebHookSqlComponent component = requestContext.CreateComponent<PipelineWebHookSqlComponent>())
          return component.CreateWebHookName(webHookId, webHookName);
      }
    }

    public WebHook CreateWebHook(IVssRequestContext requestContext, WebHook webHook)
    {
      using (new MethodScope(requestContext, nameof (PipelineWebHookExtension), nameof (CreateWebHook)))
      {
        using (PipelineWebHookSqlComponent component = requestContext.CreateComponent<PipelineWebHookSqlComponent>())
          component.CreateWebHook(webHook);
      }
      return webHook;
    }

    public void DeleteWebHook(IVssRequestContext requestContext, WebHook webHook)
    {
      using (new MethodScope(requestContext, nameof (PipelineWebHookExtension), nameof (DeleteWebHook)))
      {
        using (PipelineWebHookSqlComponent component = requestContext.CreateComponent<PipelineWebHookSqlComponent>())
          component.DeleteWebHook(webHook);
      }
    }

    public void DeleteWebHookName(IVssRequestContext requestContext, WebHook webHook)
    {
      using (new MethodScope(requestContext, nameof (PipelineWebHookExtension), nameof (DeleteWebHookName)))
      {
        using (PipelineWebHookSqlComponent component = requestContext.CreateComponent<PipelineWebHookSqlComponent>())
          component.DeleteWebHookName(webHook);
      }
    }

    public void DeleteWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier)
    {
      throw new NotImplementedException();
    }

    public WebHook GetIncomingWebHook(
      IVssRequestContext requestContext,
      string webHookname,
      bool includeSubscriptions)
    {
      using (new MethodScope(requestContext, nameof (PipelineWebHookExtension), nameof (GetIncomingWebHook)))
      {
        using (PipelineWebHookSqlComponent component = requestContext.CreateComponent<PipelineWebHookSqlComponent>())
          return component.GetIncomingWebHook(webHookname, includeSubscriptions);
      }
    }

    public WebHook GetWebHook(
      IVssRequestContext requestContext,
      string uniqueArtifactIdentifier,
      bool includeSubscriptions)
    {
      using (new MethodScope(requestContext, nameof (PipelineWebHookExtension), nameof (GetWebHook)))
      {
        using (PipelineWebHookSqlComponent component = requestContext.CreateComponent<PipelineWebHookSqlComponent>())
          return component.GetWebHook(uniqueArtifactIdentifier, includeSubscriptions);
      }
    }

    public WebHook GetWebHook(
      IVssRequestContext requestContext,
      Guid webHookId,
      bool includeSubscriptions)
    {
      using (new MethodScope(requestContext, nameof (PipelineWebHookExtension), nameof (GetWebHook)))
      {
        using (PipelineWebHookSqlComponent component = requestContext.CreateComponent<PipelineWebHookSqlComponent>())
          return component.GetWebHook(webHookId, includeSubscriptions);
      }
    }

    public string GetWebHookPayloadUrl(IVssRequestContext requestContext, WebHook webHook)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebHook>(webHook, nameof (webHook));
      if (webHook.IsLocalWebHook())
        return (string) null;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "distributedtask", TaskResourceIds.WebHooksLocationId, Guid.Empty, (object) new
      {
        webHookId = webHook.WebHookId.ToString()
      }).AbsoluteUri + string.Format("?api-version={0}", (object) new ApiResourceVersion(VssRestApiVersion.v6_0.ToVersion())
      {
        IsPreview = true
      }.ToString());
    }

    public InputValues GetWebHookPublisherInputValues(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> inputValues)
    {
      InputValues publisherInputValues = new InputValues();
      IPipelineWebHookPublisherService service = requestContext.GetService<IPipelineWebHookPublisherService>();
      try
      {
        PipelineWebHookPublisher webHookPublisher1 = this.GetWebHookPublisher(inputValues);
        switch (inputId)
        {
          case "createwebhook":
            service.CreateWebHookPublisher(requestContext, webHookPublisher1);
            break;
          case "listwebhook":
            PipelineWebHookPublisher webHookPublisher2 = service.GetWebHookPublisher(requestContext, webHookPublisher1);
            if (webHookPublisher2 == null)
            {
              publisherInputValues.Error = new InputValuesError()
              {
                Message = DeploymentResources.CannotFindWebHookPublisher((object) webHookPublisher1.WebHookId)
              };
              break;
            }
            publisherInputValues.PossibleValues = (IList<InputValue>) new List<InputValue>();
            publisherInputValues.PossibleValues.Add(new InputValue()
            {
              Value = webHookPublisher2.WebHookId.ToString()
            });
            break;
          case "deletewebhook":
            service.DeleteWebHookPublisher(requestContext, webHookPublisher1);
            break;
        }
      }
      catch (Exception ex)
      {
        publisherInputValues.Error = new InputValuesError()
        {
          Message = ex.Message
        };
      }
      return publisherInputValues;
    }

    public void QueueJobToTriggerPipeline(
      IVssRequestContext requestContext,
      Guid webHookId,
      string eventPayload)
    {
      XmlNode jsonXmlNode = new PipelineWebHookEventData()
      {
        WebHookId = webHookId,
        EventData = eventPayload
      }.SerializeToJsonXmlNode();
      TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
      using (requestContext.AllowAnonymousWrites())
        service.QueueOneTimeJob(requestContext, typeof (TriggerPipelineForArtifactEventJob).Namespace, typeof (TriggerPipelineForArtifactEventJob).FullName, jsonXmlNode, JobPriorityLevel.Normal);
    }

    public void Dispose()
    {
    }

    private PipelineWebHookPublisher GetWebHookPublisher(IDictionary<string, string> inputValues)
    {
      PipelineWebHookPublisher webHookPublisher = new PipelineWebHookPublisher();
      if (inputValues == null)
        throw new WebHookPublisherException(DeploymentResources.CannotGetWebHookPublisher());
      string input1;
      Guid result1;
      if (!inputValues.TryGetValue(PipelineWebHookPublisherPropertyNames.WebHookId, out input1) || !Guid.TryParse(input1, out result1))
        throw new WebHookPublisherException(DeploymentResources.CannotGetWebHookPublisherRequiredProperty((object) "WebHookId"));
      webHookPublisher.WebHookId = result1;
      string str;
      if (inputValues.TryGetValue(PipelineWebHookPublisherPropertyNames.PayloadUrl, out str))
        webHookPublisher.PayloadUrl = str;
      string input2;
      Guid result2;
      if (!inputValues.TryGetValue(PipelineWebHookPublisherPropertyNames.ProjectId, out input2) || !Guid.TryParse(input2, out result2))
        throw new WebHookPublisherException(DeploymentResources.CannotGetWebHookPublisherRequiredProperty((object) "ProjectId"));
      webHookPublisher.Project = new ProjectInfo()
      {
        Id = result2
      };
      string s;
      int result3;
      if (!inputValues.TryGetValue(PipelineWebHookPublisherPropertyNames.PipelineDefinitionId, out s) || !int.TryParse(s, out result3))
        throw new WebHookPublisherException(DeploymentResources.CannotGetWebHookPublisherRequiredProperty((object) "PipelineDefinitionId"));
      webHookPublisher.PipelineDefinitionId = result3;
      return webHookPublisher;
    }
  }
}
