// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks.WebHookService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks
{
  public class WebHookService : IWebHookService, IVssFrameworkService
  {
    private IDictionary<string, IWebHookExtension> m_ExtensionsByType;

    public void AddWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(webHookId, nameof (webHookId));
      this.GetWebHookExtension(requestContext)?.AddWebHookSubscription(requestContext, webHookId, uniqueArtifactIdentifier);
    }

    public void DeleteWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      WebHook webHook,
      IDictionary<string, string> parameters,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebHook>(webHook, nameof (webHook));
      IWebHookExtension webHookExtension = this.GetWebHookExtension(requestContext);
      if (artifactType.IsIncomingWebHookArtifactType())
      {
        webHookExtension?.DeleteWebHookName(requestContext, webHook);
        webHookExtension?.DeleteWebHook(requestContext, webHook);
      }
      else
      {
        webHookExtension?.DeleteWebHook(requestContext, webHook);
        try
        {
          this.UnRegisterWebHook(requestContext, projectId, webHook, parameters, artifactType);
        }
        catch (Exception ex)
        {
          resultMessage = ex.Message;
        }
      }
    }

    public void DeleteWebHookSubscription(
      IVssRequestContext requestContext,
      Guid webHookId,
      string uniqueArtifactIdentifier)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(webHookId, nameof (webHookId));
      ArgumentUtility.CheckStringForNullOrEmpty(uniqueArtifactIdentifier, nameof (uniqueArtifactIdentifier));
      this.GetWebHookExtension(requestContext)?.DeleteWebHookSubscription(requestContext, webHookId, uniqueArtifactIdentifier);
    }

    public WebHook EnsureIncomingWebHookExists(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      string uniqueArtifactIdentifier,
      IDictionary<string, string> parameters,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      Guid result = Guid.Empty;
      string str;
      parameters.TryGetValue(WebhookPropertyNames.WebhookName, out str);
      string input;
      if (parameters.TryGetValue("connection", out input))
        Guid.TryParse(input, out result);
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(parameters, nameof (parameters));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(str, "webhookName");
      ArgumentUtility.CheckForEmptyGuid(result, "connectionId");
      WebHook incomingWebHook = this.GetIncomingWebHook(requestContext, str, true);
      if (incomingWebHook == null)
      {
        WebHook webHook = new WebHook()
        {
          ArtifactType = artifactType.Name,
          WebHookId = Guid.NewGuid(),
          UniqueArtifactIdentifier = uniqueArtifactIdentifier,
          ConnectionId = result
        };
        incomingWebHook = this.CreateIncomingWebHook(requestContext, webHook, str);
      }
      return incomingWebHook;
    }

    public WebHook EnsureWebHookExists(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      string uniqueArtifactIdentifier,
      IDictionary<string, string> parameters,
      out string resultMessage)
    {
      resultMessage = string.Empty;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      ArgumentUtility.CheckStringForNullOrEmpty(uniqueArtifactIdentifier, nameof (uniqueArtifactIdentifier));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(parameters, nameof (parameters));
      WebHook webHook1 = this.GetWebHook(requestContext, uniqueArtifactIdentifier, true);
      if (webHook1 == null)
      {
        WebHook webHook2 = new WebHook()
        {
          ArtifactType = artifactType.Name,
          UniqueArtifactIdentifier = uniqueArtifactIdentifier,
          WebHookId = Guid.NewGuid()
        };
        Guid result;
        if (parameters.ContainsKey("connection") && Guid.TryParse(parameters["connection"], out result))
          webHook2.ConnectionId = result;
        try
        {
          this.RegisterWebHook(requestContext, projectId, webHook2, parameters, artifactType);
        }
        catch (Exception ex)
        {
          resultMessage = ex.Message;
          return (WebHook) null;
        }
        webHook1 = this.CreateWebHook(requestContext, webHook2);
      }
      else if (!this.IsWebHookRegistered(requestContext, projectId, webHook1, parameters, artifactType))
      {
        try
        {
          this.RegisterWebHook(requestContext, projectId, webHook1, parameters, artifactType);
        }
        catch (Exception ex)
        {
          resultMessage = ex.Message;
          return (WebHook) null;
        }
      }
      return webHook1;
    }

    public WebHook GetIncomingWebHook(
      IVssRequestContext requestContext,
      string webHookName,
      bool includeSubscriptions)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(webHookName, nameof (webHookName));
      return this.GetWebHookExtension(requestContext)?.GetIncomingWebHook(requestContext, webHookName, includeSubscriptions);
    }

    public WebHook GetWebHook(
      IVssRequestContext requestContext,
      Guid webHookId,
      bool includeSubscriptions)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(webHookId, nameof (webHookId));
      return this.GetWebHookExtension(requestContext)?.GetWebHook(requestContext, webHookId, includeSubscriptions);
    }

    public WebHook GetWebHook(
      IVssRequestContext requestContext,
      string uniqueArtifactIdentifier,
      bool includeSubscriptions = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return this.GetWebHookExtension(requestContext)?.GetWebHook(requestContext, uniqueArtifactIdentifier, includeSubscriptions);
    }

    public IWebHookExtension GetWebHookExtension(IVssRequestContext requestContext)
    {
      string serviceName = string.Equals(requestContext.ServiceName, "Pipeline", StringComparison.OrdinalIgnoreCase) || string.Equals(requestContext.ServiceName, "ReleaseManagement", StringComparison.OrdinalIgnoreCase) ? requestContext.ServiceName : "Pipeline";
      IWebHookExtension webHookExtension;
      if (!this.m_ExtensionsByType.TryGetValue(serviceName, out webHookExtension))
      {
        IDisposableReadOnlyList<IWebHookExtension> extensions = requestContext.GetExtensions<IWebHookExtension>();
        webHookExtension = extensions.FirstOrDefault<IWebHookExtension>((Func<IWebHookExtension, bool>) (x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase)));
        if (webHookExtension != null)
          this.m_ExtensionsByType[serviceName] = webHookExtension;
        if (!(webHookExtension is IDisposable))
          extensions.Dispose();
      }
      return webHookExtension;
    }

    public void LinkWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      WebHook webHook,
      IDictionary<string, string> parameters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(parameters, nameof (parameters));
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      ArgumentUtility.CheckForNull<WebHook>(webHook, nameof (webHook));
      Dictionary<string, string> dictionary = parameters.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Key), (Func<KeyValuePair<string, string>, string>) (pair => pair.Value));
      dictionary.Add("webHookName", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Webhook{0}", (object) webHook.WebHookId.ToString("N")));
      dictionary.Add("webHookId", webHook.WebHookId.ToString());
      ArtifactTaskInputMapper.ProcessYamlInputMapping(artifactType, (IDictionary<string, string>) dictionary);
      IArtifactType artifactType1 = artifactType;
      IVssRequestContext context = requestContext.Elevate();
      ProjectInfo projectInfo = new ProjectInfo();
      projectInfo.Id = projectId;
      Dictionary<string, string> currentInputValues = dictionary;
      InputValues inputValues = artifactType1.GetInputValues(context, projectInfo, "linkwebhook", (IDictionary<string, string>) currentInputValues);
      if (inputValues != null && inputValues.Error != null)
      {
        requestContext.TraceAlways(10016127, "WebHook", "Cannot link webHook. Artifacttype: {0}, ProjectId: {1}, Parameters:{2}, Error: {3}", (object) artifactType.Name, (object) projectId, (object) dictionary.ToDebugString(), (object) inputValues.Error.Message);
        throw new WebHookException(inputValues.Error.Message);
      }
    }

    public void UnlinkWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      WebHook webHook,
      IDictionary<string, string> parameters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(parameters, nameof (parameters));
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      ArgumentUtility.CheckForNull<WebHook>(webHook, nameof (webHook));
      Dictionary<string, string> dictionary = parameters.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Key), (Func<KeyValuePair<string, string>, string>) (pair => pair.Value));
      dictionary.Add("webHookName", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Webhook{0}", (object) webHook.WebHookId.ToString("N")));
      dictionary.Add("webHookId", webHook.WebHookId.ToString());
      ArtifactTaskInputMapper.ProcessYamlInputMapping(artifactType, (IDictionary<string, string>) dictionary);
      IArtifactType artifactType1 = artifactType;
      IVssRequestContext context = requestContext.Elevate();
      ProjectInfo projectInfo = new ProjectInfo();
      projectInfo.Id = projectId;
      Dictionary<string, string> currentInputValues = dictionary;
      InputValues inputValues = artifactType1.GetInputValues(context, projectInfo, "unlinkwebhook", (IDictionary<string, string>) currentInputValues);
      if (inputValues != null && inputValues.Error != null)
      {
        requestContext.TraceAlways(10016128, "WebHook", "Cannot list webHooks from external service. ArtifactType: {0}, ProjectId: {1}, Parameters:{2}, Error: {3}", (object) artifactType.Name, (object) projectId, (object) dictionary.ToDebugString(), (object) inputValues.Error.Message);
        throw new WebHookException(inputValues.Error.Message);
      }
    }

    public WebHook CreateIncomingWebHook(
      IVssRequestContext requestContext,
      WebHook webHook,
      string webHookName)
    {
      IWebHookExtension webHookExtension = this.GetWebHookExtension(requestContext);
      Guid guid = webHookExtension != null ? webHookExtension.CreateWebHookName(requestContext, webHook.WebHookId, webHookName) : Guid.Empty;
      if (webHook.WebHookId != Guid.Empty && webHook.WebHookId == guid && webHookExtension != null)
        webHookExtension.CreateWebHook(requestContext, webHook);
      return webHook;
    }

    private WebHook CreateWebHook(IVssRequestContext requestContext, WebHook webHook) => this.GetWebHookExtension(requestContext)?.CreateWebHook(requestContext, webHook);

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_ExtensionsByType = (IDictionary<string, IWebHookExtension>) systemRequestContext.GetExtensions<IWebHookExtension>().ToDictionary<IWebHookExtension, string>((Func<IWebHookExtension, string>) (ext => ext.ServiceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_ExtensionsByType = (IDictionary<string, IWebHookExtension>) null;

    private static InputValues GetInputValuesForRegisteredWebHooks(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> parameters,
      IArtifactType artifactType)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(parameters, nameof (parameters));
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      IArtifactType artifactType1 = artifactType;
      IVssRequestContext context = requestContext.Elevate();
      ProjectInfo projectInfo = new ProjectInfo();
      projectInfo.Id = projectId;
      IDictionary<string, string> currentInputValues = parameters;
      InputValues inputValues = artifactType1.GetInputValues(context, projectInfo, "listwebhook", currentInputValues);
      if (inputValues != null && inputValues.Error != null)
        requestContext.TraceAlways(10016125, "WebHook", "Cannot list webHooks from external service. Artifacttype: {0}, ProjectId: {1}, Parameters:{2}, Error: {3}", (object) artifactType.Name, (object) projectId, (object) parameters.ToDebugString(), (object) inputValues.Error.Message);
      return inputValues;
    }

    private static Dictionary<string, string> GetRegisteredWebHookPayload(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> parameters,
      IArtifactType artifactType,
      string payloadUrl)
    {
      Dictionary<string, string> registeredWebHookPayload = new Dictionary<string, string>();
      InputValues registeredWebHooks = WebHookService.GetInputValuesForRegisteredWebHooks(requestContext, projectId, parameters, artifactType);
      if (registeredWebHooks != null)
      {
        IDictionary<string, object> data = registeredWebHooks != null ? registeredWebHooks.PossibleValues.FirstOrDefault<InputValue>((Func<InputValue, bool>) (x => string.Equals(x.Value, payloadUrl, StringComparison.OrdinalIgnoreCase)))?.Data : (IDictionary<string, object>) null;
        if (data != null)
        {
          foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) data)
            registeredWebHookPayload.Add(keyValuePair.Key, keyValuePair.Value.ToString());
        }
      }
      return registeredWebHookPayload;
    }

    private bool IsWebHookRegistered(
      IVssRequestContext requestContext,
      Guid projectId,
      WebHook webHookToCompare,
      IDictionary<string, string> parameters,
      IArtifactType artifactType)
    {
      foreach (WebHook registeredWebHook in (IEnumerable<WebHook>) this.ListRegisteredWebHooks(requestContext, projectId, artifactType, (IDictionary<string, string>) new Dictionary<string, string>(parameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "webHookId",
          webHookToCompare.WebHookId.ToString()
        },
        {
          "webHookName",
          string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Webhook{0}", (object) webHookToCompare.WebHookId.ToString("N"))
        }
      }))
      {
        if (registeredWebHook.WebHookId.Equals(webHookToCompare.WebHookId))
        {
          if (artifactType?.ArtifactTriggerConfiguration?.IsWebhookSupportedAtServerLevel.GetValueOrDefault())
            this.LinkWebHook(requestContext, projectId, artifactType, registeredWebHook, parameters);
          return true;
        }
      }
      return false;
    }

    private IList<WebHook> ListRegisteredWebHooks(
      IVssRequestContext requestContext,
      Guid projectId,
      IArtifactType artifactType,
      IDictionary<string, string> parameters)
    {
      List<WebHook> webHookList = new List<WebHook>();
      InputValues registeredWebHooks = WebHookService.GetInputValuesForRegisteredWebHooks(requestContext, projectId, parameters, artifactType);
      if (registeredWebHooks != null)
      {
        foreach (InputValue possibleValue in (IEnumerable<InputValue>) registeredWebHooks.PossibleValues)
        {
          Guid idFromPayloadUrl = WebHookHelper.GetWebHookIdFromPayloadUrl(requestContext, possibleValue.Value);
          if (idFromPayloadUrl != Guid.Empty)
            webHookList.Add(new WebHook()
            {
              WebHookId = idFromPayloadUrl
            });
        }
      }
      return (IList<WebHook>) webHookList;
    }

    private void RegisterWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      WebHook webHook,
      IDictionary<string, string> parameters,
      IArtifactType artifactType)
    {
      IWebHookExtension webHookExtension = this.GetWebHookExtension(requestContext);
      if (webHookExtension == null)
        return;
      string webHookPayloadUrl = webHookExtension.GetWebHookPayloadUrl(requestContext, webHook);
      Dictionary<string, string> parameters1 = new Dictionary<string, string>(parameters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!string.IsNullOrEmpty(webHookPayloadUrl))
        parameters1.Add("payloadUrl", webHookPayloadUrl);
      string secret = string.Empty;
      if (!string.IsNullOrEmpty(artifactType.ArtifactTriggerConfiguration?.PayloadHashHeaderName))
      {
        secret = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        parameters1.Add("secret", secret);
      }
      parameters1.Add("webHookName", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Webhook{0}", (object) webHook.WebHookId.ToString("N")));
      parameters1.Add("webHookId", webHook.WebHookId.ToString());
      if (artifactType.Name == "AzureContainerRepository" && !parameters1.ContainsKey("location"))
      {
        IArtifactType artifactType1 = artifactType;
        IVssRequestContext context = requestContext.Elevate();
        ProjectInfo projectInfo = new ProjectInfo();
        projectInfo.Id = projectId;
        Dictionary<string, string> currentInputValues = parameters1;
        InputValues inputValues = artifactType1.GetInputValues(context, projectInfo, "locationbyregistryname", (IDictionary<string, string>) currentInputValues);
        parameters1["location"] = inputValues != null ? inputValues.PossibleValues.FirstOrDefault<InputValue>()?.Value : (string) null;
      }
      IArtifactType artifactType2 = artifactType;
      IVssRequestContext context1 = requestContext.Elevate();
      ProjectInfo projectInfo1 = new ProjectInfo();
      projectInfo1.Id = projectId;
      Dictionary<string, string> currentInputValues1 = parameters1;
      InputValues inputValues1 = artifactType2.GetInputValues(context1, projectInfo1, "createwebhook", (IDictionary<string, string>) currentInputValues1);
      if (inputValues1 != null && inputValues1.Error != null)
      {
        requestContext.TraceAlways(10016123, "WebHook", "Cannot register webHook with external service. ArtifactType: {0}, projectId: {1}, parameters: {2}, Error:{3}", (object) artifactType.Name, (object) projectId, (object) parameters1.ToDebugString(), (object) inputValues1.Error.Message);
        throw new WebHookException(inputValues1.Error.Message);
      }
      ArtifactTriggerConfiguration triggerConfiguration = artifactType.ArtifactTriggerConfiguration;
      if ((triggerConfiguration != null ? (triggerConfiguration.IsWebhookSupportedAtServerLevel ? 1 : 0) : 0) != 0 && !this.IsWebHookResourceTrigger(parameters))
        this.LinkWebHook(requestContext, projectId, artifactType, webHook, parameters);
      if (string.IsNullOrEmpty(artifactType.ArtifactTriggerConfiguration?.PayloadHashHeaderName) || artifactType.IsIncomingWebHookArtifactType())
        return;
      WebHookHelper.InitializeWebHookSecret(requestContext, "WebHookDrawer", webHook.WebHookId.ToString(), secret);
    }

    private bool IsWebHookResourceTrigger(IDictionary<string, string> properties)
    {
      bool flag = false;
      string str;
      PipelineTriggerType result;
      if (properties != null && properties.TryGetValue("pipelineTriggerType", out str) && !string.IsNullOrEmpty(str) && Enum.TryParse<PipelineTriggerType>(str, out result))
        flag = result == PipelineTriggerType.WebhookTriggeredEvent;
      return flag;
    }

    private void UnRegisterWebHook(
      IVssRequestContext requestContext,
      Guid projectId,
      WebHook webHook,
      IDictionary<string, string> parameters,
      IArtifactType artifactType)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebHook>(webHook, nameof (webHook));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(parameters, nameof (parameters));
      ArgumentUtility.CheckForNull<IArtifactType>(artifactType, nameof (artifactType));
      ArtifactTriggerConfiguration triggerConfiguration = artifactType.ArtifactTriggerConfiguration;
      if ((triggerConfiguration != null ? (triggerConfiguration.IsWebhookSupportedAtServerLevel ? 1 : 0) : 0) != 0 && !this.IsWebHookResourceTrigger(parameters))
        this.UnlinkWebHook(requestContext, projectId, artifactType, webHook, parameters);
      IWebHookExtension webHookExtension = this.GetWebHookExtension(requestContext);
      if (webHookExtension == null)
        return;
      Dictionary<string, string> dictionary = parameters.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Key), (Func<KeyValuePair<string, string>, string>) (pair => pair.Value));
      string webHookPayloadUrl = webHookExtension.GetWebHookPayloadUrl(requestContext, webHook);
      if (!string.IsNullOrEmpty(webHookPayloadUrl))
        dictionary.Add("payloadUrl", webHookPayloadUrl);
      dictionary.Add("webHookName", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Webhook{0}", (object) webHook.WebHookId.ToString("N")));
      dictionary.Add("webHookId", webHook.WebHookId.ToString());
      dictionary.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) WebHookService.GetRegisteredWebHookPayload(requestContext, projectId, (IDictionary<string, string>) dictionary, artifactType, webHookPayloadUrl));
      IArtifactType artifactType1 = artifactType;
      IVssRequestContext context = requestContext.Elevate();
      ProjectInfo projectInfo = new ProjectInfo();
      projectInfo.Id = projectId;
      Dictionary<string, string> currentInputValues = dictionary;
      InputValues inputValues = artifactType1.GetInputValues(context, projectInfo, "deletewebhook", (IDictionary<string, string>) currentInputValues);
      if (inputValues != null && inputValues.Error != null)
      {
        requestContext.TraceAlways(10016126, "WebHook", "Cannot unregister webhook with external service. ArtifactType: {0}, ProjectId: {1}, Parameters:{2}, Error: {3}", (object) artifactType.Name, (object) projectId, (object) dictionary.ToDebugString(), (object) inputValues.Error.Message);
        throw new WebHookException(inputValues.Error.Message);
      }
      if (artifactType.IsIncomingWebHookArtifactType())
        return;
      WebHookHelper.RemoveWebHookSecret(requestContext, "WebHookDrawer", webHook.WebHookId.ToString());
    }
  }
}
