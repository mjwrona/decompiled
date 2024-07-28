// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Jenkins.JenkinsConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Jenkins
{
  [Export(typeof (ConsumerImplementation))]
  public sealed class JenkinsConsumer : ConsumerImplementation
  {
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=393616";
    private const string c_logoUrl = "http://jenkins-ci.org/sites/default/files/images/headshot.png";
    private const string c_urlFormatGetTfsPlugin = "{0}/pluginManager/api/xml?depth=1&xpath=*/plugin[shortName=\"tfs\"]/version";
    private const string c_urlFormatCrumbRequestUrl = "{0}/crumbIssuer/api/json";
    private const string c_versionPrefix = "<version>";
    private const string c_versionSuffix = "</version>";
    private const string c_previewPrefix = "-";
    private const int c_defaultRequestTimeoutSeconds = 30;
    public const string ConsumerId = "jenkins";
    public const string ServerBaseUrlInputId = "serverBaseUrl";
    public const string UsernameInputId = "username";
    public const string PasswordInputId = "password";
    public const string IntegrationLevelInputId = "useTfsPlugin";
    public const string RegistryPathUrlFormatGetTfsPlugin = "/Service/ServiceHooks/JenkinsConsumer/UrlFormatGetTfsPlugin";
    public const string RegistryPathUrlFormatCrumbRequest = "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatCrumbRequest";
    private static readonly TimeSpan s_defaultRequestTimeout = TimeSpan.FromSeconds(30.0);
    private static readonly Version s_minTfsPluginVersion = new Version(5, 0, 0);

    public override string Id => "jenkins";

    public override string Name => JenkinsConsumerResources.ConsumerName;

    public override string Description => JenkinsConsumerResources.ConsumerDescription;

    public override string ImageUrl => "http://jenkins-ci.org/sites/default/files/images/headshot.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=393616";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = JenkinsConsumerResources.JenkinsConsumer_ServerBaseUrlInputName,
        Description = JenkinsConsumerResources.JenkinsConsumer_ServerBaseUrlInputDescription,
        InputMode = InputMode.TextBox,
        Id = "serverBaseUrl",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.Uri
        }
      },
      new InputDescriptor()
      {
        Name = JenkinsConsumerResources.JenkinsConsumer_UsernameInputName,
        Description = JenkinsConsumerResources.JenkinsConsumer_UsernameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "username",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        }
      },
      new InputDescriptor()
      {
        Name = JenkinsConsumerResources.JenkinsConsumer_PasswordInputName,
        Description = JenkinsConsumerResources.JenkinsConsumer_PasswordInputDescription,
        InputMode = InputMode.PasswordBox,
        Id = "password",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "serverBaseUrl"
        }
      }
    };

    public static bool TryGetConsumerInputs(
      IDictionary<string, string> inputValues,
      out string serverBaseUrl,
      out string username,
      out string password)
    {
      serverBaseUrl = (string) null;
      username = (string) null;
      password = (string) null;
      bool consumerInputs = inputValues.TryGetValue(nameof (serverBaseUrl), out serverBaseUrl) && inputValues.TryGetValue(nameof (username), out username) && inputValues.TryGetValue(nameof (password), out password);
      if (consumerInputs)
        consumerInputs = !string.IsNullOrEmpty(serverBaseUrl) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
      return consumerInputs;
    }

    public static InputValues GetInputValuesForIntegrationLevel(
      IVssRequestContext requestContext,
      IDictionary<string, string> currentConsumerInputValues,
      Func<IVssRequestContext, HttpClient> GetHttpClient)
    {
      InputValues integrationLevelCombo = JenkinsConsumer.GetDefaultsForIntegrationLevelCombo();
      string serverBaseUrl1;
      string username;
      string password;
      if (!JenkinsConsumer.TryGetConsumerInputs(currentConsumerInputValues, out serverBaseUrl1, out username, out password))
        return integrationLevelCombo;
      string serverBaseUrl2 = JenkinsConsumer.NormalizeUrl(serverBaseUrl1);
      try
      {
        string tfsPluginUrl = JenkinsConsumer.BuildGetTfsPluginUrl(requestContext, serverBaseUrl2);
        using (HttpClient httpClient = JenkinsConsumer.CreateHttpClient(requestContext, GetHttpClient, username, password))
        {
          HttpResponseMessage result = httpClient.PauseTimerAndGetResult(requestContext, tfsPluginUrl);
          if (result.IsSuccessStatusCode)
          {
            if (JenkinsConsumer.s_minTfsPluginVersion <= JenkinsConsumer.ReadTfsPuginVersion(result))
            {
              integrationLevelCombo.PossibleValues.Add(new InputValue()
              {
                DisplayValue = JenkinsConsumerResources.JenkinsConsumer_TfsPliginIntegrationLevel,
                Value = "tfs-plugin"
              });
              integrationLevelCombo.DefaultValue = "tfs-plugin";
            }
          }
          else if (result.StatusCode == HttpStatusCode.Unauthorized)
            integrationLevelCombo.Error = new InputValuesError()
            {
              Message = JenkinsConsumerResources.TriggerGenericBuildAction_QueryError_SuppliedCredentialsNotAuthorized
            };
        }
        return integrationLevelCombo;
      }
      catch (Exception ex)
      {
        Exception exception = ex is AggregateException ? ex.InnerException : ex;
        integrationLevelCombo.Error = new InputValuesError()
        {
          Message = string.Format(JenkinsConsumerResources.TriggerGenericBuildAction_QueryError_ExceptionFormat, (object) exception.GetBaseException().Message)
        };
        return integrationLevelCombo;
      }
    }

    private static Version ReadTfsPuginVersion(HttpResponseMessage response)
    {
      HttpContent content = response.Content;
      if (content != null)
      {
        string result1 = content.ReadAsStringAsync().Result;
        if (result1.StartsWith("<version>", StringComparison.OrdinalIgnoreCase) && result1.EndsWith("</version>", StringComparison.OrdinalIgnoreCase))
        {
          string input = result1.Substring(0, result1.Length - "</version>".Length).Substring("<version>".Length);
          int length = input.IndexOf("-");
          if (length > -1)
            input = input.Substring(0, length);
          Version result2;
          if (Version.TryParse(input, out result2))
            return result2;
        }
      }
      return new Version(0, 0);
    }

    public static InputValues GetDefaultsForIntegrationLevelCombo() => new InputValues()
    {
      InputId = "useTfsPlugin",
      IsLimitedToPossibleValues = true,
      PossibleValues = (IList<InputValue>) new List<InputValue>()
      {
        new InputValue()
        {
          DisplayValue = JenkinsConsumerResources.JenkinsConsumer_BuiltInIntegrationLevel,
          Value = "built-in"
        }
      },
      DefaultValue = "built-in"
    };

    public static InputDescriptor GetIntegrationLeveInputDescriptor() => new InputDescriptor()
    {
      Name = JenkinsConsumerResources.JenkinsConsumer_IntegrationLevelInputName,
      Description = JenkinsConsumerResources.JenkinsConsumer_IntegrationLevelInputDescription,
      InputMode = InputMode.Combo,
      Id = "useTfsPlugin",
      IsConfidential = false,
      Validation = new InputValidation()
      {
        IsRequired = false,
        DataType = InputDataType.String
      },
      HasDynamicValueInformation = true,
      DependencyInputIds = (IList<string>) new List<string>()
      {
        "serverBaseUrl",
        "username",
        "password"
      },
      Values = JenkinsConsumer.GetDefaultsForIntegrationLevelCombo()
    };

    public static string NormalizeUrl(string url)
    {
      url = url.Trim();
      url = url.EndsWith("/") ? url.Remove(url.Length - 1, 1) : url;
      return url;
    }

    private static string BuildGetTfsPluginUrl(
      IVssRequestContext requestContext,
      string serverBaseUrl)
    {
      return string.Format(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/UrlFormatGetTfsPlugin", true, "{0}/pluginManager/api/xml?depth=1&xpath=*/plugin[shortName=\"tfs\"]/version"), (object) serverBaseUrl);
    }

    public static bool AddCrumbHeaderIfNeeded(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      string serverBaseUrl,
      string username,
      string password,
      Func<IVssRequestContext, HttpClient> GetHttpClient,
      out string errorMessage)
    {
      try
      {
        string requestUri = JenkinsConsumer.BuildCrumbRequestUrl(requestContext, serverBaseUrl);
        IUrlAddressIpValidatorService service = requestContext.GetService<IUrlAddressIpValidatorService>();
        using (HttpClient httpClient = JenkinsConsumer.CreateHttpClient(requestContext, GetHttpClient, username, password))
        {
          HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, requestUri);
          service.ApplyIPAddressAllowedRangeOnHttpRequest(requestContext, message);
          HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
          if (result.IsSuccessStatusCode)
          {
            JObject jobject = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            string name = jobject["crumbRequestField"].ToString();
            string str = jobject["crumb"].ToString();
            requestMessage.Headers.Add(name, str);
          }
          else if (result.StatusCode != HttpStatusCode.NotFound)
          {
            errorMessage = string.Format("{0}: {1}", (object) result.StatusCode, (object) result.ReasonPhrase);
            return false;
          }
        }
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
        return false;
      }
      errorMessage = (string) null;
      return true;
    }

    private static string BuildCrumbRequestUrl(
      IVssRequestContext requestContext,
      string serverBaseUrl)
    {
      return string.Format(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ServiceHooks/JenkinsConsumer/TriggerGenericBuildAction/UrlFormatCrumbRequest", true, "{0}/crumbIssuer/api/json"), (object) serverBaseUrl);
    }

    public static HttpClient CreateHttpClient(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, HttpClient> GetHttpClient,
      string username,
      string password)
    {
      HttpClient httpClient = GetHttpClient(requestContext);
      httpClient.Timeout = JenkinsConsumer.s_defaultRequestTimeout;
      httpClient.SetBasicAuthentication(username, password);
      return httpClient;
    }
  }
}
