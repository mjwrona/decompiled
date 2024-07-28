// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven.DataDrivenConsumerAction`1
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven
{
  public abstract class DataDrivenConsumerAction<TConsumer> : ConsumerActionImplementation where TConsumer : DataDrivenConsumer
  {
    private const int c_defaultHttpTimeoutSeconds = 30;
    private const string c_groupNameInputPlaceholder = "inputId";
    private const string c_registryPathFormatUrlPublishEvent = "/Service/ServiceHooks/{0}/{1}/UrlFormatPublishEvent";
    private const string c_registryPathFormatUrlQueryInputValues = "/Service/ServiceHooks/{0}/{1}/UrlFormatQueryInputValues";
    private string m_actionId;
    private DataDrivenConsumerActionConfig m_consumerActionDataConfig;
    private static readonly TimeSpan s_httpTimeout = TimeSpan.FromSeconds(30.0);
    private static readonly Regex s_registrFragmentNotAllowedCharsRegEx = new Regex("[\\$\\s<:>\\\\\\|#%_\\[\\]\\?\"'/]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private IDictionary<string, string> m_eventTransformTemplates = (IDictionary<string, string>) new Dictionary<string, string>();

    public DataDrivenConsumerAction(string actionId)
    {
      this.m_actionId = actionId;
      this.m_consumerActionDataConfig = this.BuildConsumerActionDataConfig();
      if (this.m_consumerActionDataConfig.PublishEvent.Transforms == null)
        return;
      foreach (PublishEventTransformConfig transform in this.m_consumerActionDataConfig.PublishEvent.Transforms)
      {
        if (transform.TemplateBody != null)
          this.TransformTemplates.Add(transform.EventType, transform.TemplateBody);
        else if (!string.IsNullOrEmpty(transform.TemplateAssetType))
        {
          string str = this.LoadTransformResource(typeof (TConsumer), transform.TemplateAssetType);
          if (str != null)
            this.TransformTemplates.Add(transform.EventType, str);
        }
      }
    }

    public DataDrivenConsumerAction(DataDrivenConsumerActionConfig actionConfig)
    {
      this.m_actionId = actionConfig.Id;
      this.m_consumerActionDataConfig = actionConfig;
    }

    protected virtual DataDrivenConsumerActionConfig BuildConsumerActionDataConfig() => DataDrivenConsumerActionConfig.CreateFromConsumerType(typeof (TConsumer), this.m_actionId);

    public override string ConsumerId => this.m_consumerActionDataConfig.ConsumerId;

    public override string Id => this.m_consumerActionDataConfig.Id;

    public override string Name => this.m_consumerActionDataConfig.Name;

    public override string Description => this.m_consumerActionDataConfig.Description;

    public override string[] SupportedEventTypes => this.m_consumerActionDataConfig.SupportedEventTypes;

    public override IDictionary<string, string[]> SupportedResourceVersions => this.m_consumerActionDataConfig.SupportedResourceVersions;

    public override bool AllowResourceVersionOverride => this.m_consumerActionDataConfig.AllowResourceVersionOverride;

    public override IList<InputDescriptor> InputDescriptors => this.m_consumerActionDataConfig.InputDescriptors == null ? (IList<InputDescriptor>) new List<InputDescriptor>() : (IList<InputDescriptor>) this.m_consumerActionDataConfig.InputDescriptors;

    public IDictionary<string, string> TransformTemplates => this.m_eventTransformTemplates;

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      InputValues inputValues = base.GetInputValues(requestContext, inputId, currentConsumerInputValues);
      DataDrivenHttpActionConfig httpActionConfig = this.GetQueryHttpActionConfig(inputId);
      if (httpActionConfig == null)
        return inputValues;
      if (!this.AreInputDependenciesValid(requestContext, inputId, currentConsumerInputValues))
        return inputValues;
      try
      {
        return this.GetInputValuesFromHttpQuery(requestContext, currentConsumerInputValues, httpActionConfig);
      }
      catch
      {
        return inputValues;
      }
    }

    public ActionTask MyHandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      HttpRequestMessage httpRequest = this.CreateHttpRequest(requestContext, HttpMethod.Post, (DataDrivenHttpActionConfig) this.m_consumerActionDataConfig.PublishEvent, eventArgs.Notification.Details.ConsumerInputs, "/Service/ServiceHooks/{0}/{1}/UrlFormatPublishEvent");
      IDictionary<string, string> consumerInputs = eventArgs.Notification.Details.ConsumerInputs;
      PublishEventHttpActionConfig publishEvent = this.m_consumerActionDataConfig.PublishEvent;
      string confidentialUrl = this.BuildRequestUrl(requestContext, (DataDrivenHttpActionConfig) publishEvent, consumerInputs, "/Service/ServiceHooks/{0}/{1}/UrlFormatPublishEvent", true);
      JObject jobject = EventTransformer.TransformEvent(raisedEvent, this.ExpandInputPlaceholders(publishEvent.ResourceDetailsToSend ?? string.Empty, consumerInputs).ToEventResourceDetailsValue(), this.ExpandInputPlaceholders(publishEvent.MessagesToSend ?? string.Empty, consumerInputs).ToEventMessagesValue(), this.ExpandInputPlaceholders(publishEvent.DetailedMessagesToSend ?? string.Empty, consumerInputs).ToEventMessagesValue(), eventArgs.Notification.Details is NotificationDetailsInternal details ? details.NotificationData : (IDictionary<string, string>) null);
      string template;
      string str;
      if (this.m_eventTransformTemplates.TryGetValue(raisedEvent.EventType, out template))
        str = TemplateEngineFactory.CreateDefault().ApplyTemplate(template, jobject, new Dictionary<string, object>()
        {
          ["IVssRequestContext"] = (object) requestContext
        });
      else
        str = jobject.GetStringRepresentation();
      httpRequest.Content = (HttpContent) new StringContent(str, Encoding.UTF8, JsonMediaTypeFormatter.DefaultMediaType.MediaType);
      return (ActionTask) new HttpActionTask(httpRequest, httpRequest.BuildHttpRequestStringRepresentation(str, confidentialUrl));
    }

    private InputValues GetInputValuesFromHttpQuery(
      IVssRequestContext requestContext,
      IDictionary<string, string> currentCInputValues,
      DataDrivenHttpActionConfig queryHttpActionConfig)
    {
      using (HttpClient httpClient = this.GetHttpClient(requestContext))
      {
        httpClient.Timeout = DataDrivenConsumerAction<TConsumer>.s_httpTimeout;
        HttpRequestMessage httpRequest = this.CreateHttpRequest(requestContext, HttpMethod.Get, queryHttpActionConfig, currentCInputValues, "/Service/ServiceHooks/{0}/{1}/UrlFormatQueryInputValues");
        return httpClient.PauseTimerAndSendResult(requestContext, httpRequest).Content.ReadAsAsync<InputValues>().Result;
      }
    }

    private DataDrivenHttpActionConfig GetQueryHttpActionConfig(string inputId)
    {
      if (this.m_consumerActionDataConfig.OverrideQueryInputValues != null)
      {
        DataDrivenConsumerActionConfig.QueryInputValuesConfig inputValuesConfig = ((IEnumerable<DataDrivenConsumerActionConfig.QueryInputValuesConfig>) this.m_consumerActionDataConfig.OverrideQueryInputValues).SingleOrDefault<DataDrivenConsumerActionConfig.QueryInputValuesConfig>((Func<DataDrivenConsumerActionConfig.QueryInputValuesConfig, bool>) (x => x.InputId == inputId));
        if (inputValuesConfig != null)
          return inputValuesConfig.Query;
      }
      return this.m_consumerActionDataConfig.QueryInputValues;
    }

    private bool AreInputDependenciesValid(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentValues)
    {
      IList<string> dependencyInputIds = this.m_consumerActionDataConfig.GetInputDescritor(inputId).DependencyInputIds;
      if (dependencyInputIds == null || dependencyInputIds.Count == 0)
        return true;
      foreach (string inputId1 in (IEnumerable<string>) dependencyInputIds)
      {
        InputDescriptor inputDescritor = this.m_consumerActionDataConfig.GetInputDescritor(inputId1);
        string inputValue;
        currentValues.TryGetValue(inputDescritor.Id, out inputValue);
        try
        {
          inputDescritor.ValidateInternal(requestContext, inputValue);
        }
        catch (SubscriptionInputException ex)
        {
          return false;
        }
      }
      return true;
    }

    private HttpRequestMessage CreateHttpRequest(
      IVssRequestContext requestContext,
      HttpMethod method,
      DataDrivenHttpActionConfig httpAction,
      IDictionary<string, string> inputFieldsValues,
      string registryPathFormatOverrideUrl)
    {
      string requestUri = this.BuildRequestUrl(requestContext, httpAction, inputFieldsValues, registryPathFormatOverrideUrl);
      HttpRequestMessage httpRequest = new HttpRequestMessage(method, requestUri);
      if (httpAction.Headers != null)
      {
        foreach (string header in httpAction.Headers)
        {
          char[] chArray = new char[1]{ ':' };
          string[] strArray = header.Split(chArray);
          httpRequest.Headers.Add(strArray[0], this.ExpandInputPlaceholders(strArray[1], inputFieldsValues));
        }
      }
      return httpRequest;
    }

    private string BuildRequestUrl(
      IVssRequestContext requestContext,
      DataDrivenHttpActionConfig httpAction,
      IDictionary<string, string> inputFieldsValues,
      string registryPathFormatOverrideUrl,
      bool maskConfidentialInputs = false)
    {
      return this.ExpandInputPlaceholders(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) this.BuildRegistryPath(registryPathFormatOverrideUrl), true, httpAction.Url), inputFieldsValues, maskConfidentialInputs);
    }

    private string BuildRegistryPath(string registryPathFormat) => string.Format(registryPathFormat, (object) this.GetSanitizedRegistryFragmentPath(this.m_consumerActionDataConfig.ConsumerId), (object) this.GetSanitizedRegistryFragmentPath(this.m_actionId));

    private string GetSanitizedRegistryFragmentPath(string fragmentPath) => DataDrivenConsumerAction<TConsumer>.s_registrFragmentNotAllowedCharsRegEx.Replace(fragmentPath, string.Empty);

    private string ExpandInputPlaceholders(
      string template,
      IDictionary<string, string> inputFieldsValues,
      bool maskConfidentialInputs = false)
    {
      JObject model1 = inputFieldsValues.Aggregate<KeyValuePair<string, string>, JObject>(new JObject(), (Func<JObject, KeyValuePair<string, string>, JObject>) ((model, input) =>
      {
        if (maskConfidentialInputs && this.IsInputConfidential(input.Key))
          model.Add(input.Key, (JToken) SecurityHelper.GetMaskedValue(input.Value));
        else
          model.Add(input.Key, (JToken) input.Value);
        return model;
      }));
      return TemplateEngineFactory.CreateDefault().ApplyTemplate(template, model1);
    }

    private bool IsInputConfidential(string inputId)
    {
      InputDescriptor inputDescritor = this.m_consumerActionDataConfig.GetInputDescritor(inputId);
      return inputDescritor != null && inputDescritor.IsConfidential;
    }

    private string LoadTransformResource(Type consumerType, string transformResourceName)
    {
      string name = consumerType.Namespace + "." + transformResourceName;
      using (StreamReader streamReader = new StreamReader(consumerType.Assembly.GetManifestResourceStream(name)))
        return streamReader.ReadToEnd();
    }
  }
}
