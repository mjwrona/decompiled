// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.CustomerIntelligenceHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class CustomerIntelligenceHelper
  {
    private const string c_area = "TaskHub";
    private const int c_serverPoolId = -1;

    public static bool IsEndpointTargetingAzure(string endpointType) => new List<string>()
    {
      "azure",
      "servicefabric",
      "azurerm"
    }.Any<string>((Func<string, bool>) (azureEndpointType => string.Equals(azureEndpointType, endpointType, StringComparison.OrdinalIgnoreCase)));

    public static bool IsEndpointTargetingAzureNationalCloud(string endpointEnvironment) => new List<string>()
    {
      "AzureChinaCloud",
      "AzureUSGovernment",
      "AzureGermanCloud"
    }.Any<string>((Func<string, bool>) (azureEndpointEnvironment => string.Equals(azureEndpointEnvironment, endpointEnvironment, StringComparison.OrdinalIgnoreCase)));

    public static bool DoesEndpointContainsAzure(string endpointType) => endpointType.ToUpperInvariant().Contains("AZURE");

    public static bool DoesContainEndpointTargetingAzure(
      IEnumerable<string> endpointsUsedInEnvironment)
    {
      if (endpointsUsedInEnvironment == null)
        return false;
      return endpointsUsedInEnvironment.Any<string>((Func<string, bool>) (endpoint => CustomerIntelligenceHelper.DoesEndpointContainsAzure(endpoint))) || endpointsUsedInEnvironment.Any<string>((Func<string, bool>) (endpoint => CustomerIntelligenceHelper.IsEndpointTargetingAzure(endpoint)));
    }

    public static void PublishServiceEndpointCreatedOrUpdatedOrDeletedTelemetry(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      string featureName)
    {
      try
      {
        if (endpoint == null)
          return;
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("EndpointId", (object) endpoint.Id);
        intelligenceData.Add("EndpointType", endpoint.Type);
        intelligenceData.Add("ProjectId", (object) projectId);
        if (!string.IsNullOrWhiteSpace(endpoint.Type))
        {
          string scheme = endpoint.Authorization?.Scheme;
          if (!string.IsNullOrEmpty(scheme))
            intelligenceData.Add("AuthScheme", scheme);
          if (string.Equals(endpoint.Type, "kubernetes", StringComparison.OrdinalIgnoreCase))
            intelligenceData.Add("CloudProvider", CustomerIntelligenceHelper.GetKubernetesCloudProvider(endpoint.Url));
          else if (string.Equals(endpoint.Type, "AzureRM", StringComparison.OrdinalIgnoreCase))
          {
            string str1;
            if (endpoint.Data.TryGetValue("creationMode", out str1))
              intelligenceData.Add("creationMode", str1);
            string a;
            endpoint.Data.TryGetValue("scopeLevel", out a);
            intelligenceData.Add("scopeLevel", a);
            if (string.Equals(a, "ManagementGroup", StringComparison.OrdinalIgnoreCase))
            {
              string str2;
              endpoint.Data.TryGetValue("managementGroupId", out str2);
              intelligenceData.Add("managementGroupId", str2);
            }
            if (string.Equals(a, "AzureMLWorkspace", StringComparison.OrdinalIgnoreCase))
            {
              string str3;
              endpoint.Data.TryGetValue("mlWorkspaceId", out str3);
              intelligenceData.Add("mlWorkspaceId", str3);
            }
            string str4 = Guid.Empty.ToString();
            if (endpoint.Authorization != null)
              endpoint.Authorization.Parameters.TryGetValue("serviceprincipalid", out str4);
            intelligenceData.Add("serviceprincipalid", str4);
            string str5;
            endpoint.Data.TryGetValue("subscriptionId", out str5);
            intelligenceData.Add("subscriptionId", str5);
          }
        }
        IVssRequestContext requestContext1 = requestContext;
        string feature = featureName;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, "ServiceEndpoints", feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (CustomerIntelligenceHelper), ex);
      }
    }

    public static void PublishServiceEndpointProxyTelemetry(
      IVssRequestContext requestContext,
      HttpStatusCode httpStatusCode,
      string featureName,
      string url,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      TaskDefinitionEndpoint taskDefinitionEndpoint,
      string taskDefinitionEndpointType,
      string resultTemplate,
      string resultSelector,
      Exception exception)
    {
      try
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        int num = (int) httpStatusCode;
        if (num >= 200 && num < 300)
          intelligenceData.Add("RequestSucceeded", true.ToString());
        else
          intelligenceData.Add("RequestSucceeded", false.ToString());
        intelligenceData.Add("HttpStatusCode", (object) httpStatusCode);
        if (serviceEndpoint != null)
        {
          if (!string.IsNullOrEmpty(url))
            intelligenceData.Add("EndpointUrl", url);
          intelligenceData.Add("EndpointType", serviceEndpoint.Type);
          if (!string.IsNullOrEmpty(resultTemplate))
            intelligenceData.Add("ResultTemplate", resultTemplate);
          if (!string.IsNullOrEmpty(resultSelector))
            intelligenceData.Add("ResultSelector", resultSelector);
        }
        else if (taskDefinitionEndpoint != null)
        {
          if (!string.IsNullOrEmpty(taskDefinitionEndpoint.Selector))
            intelligenceData.Add("Selector", taskDefinitionEndpoint.Selector);
          if (!string.IsNullOrEmpty(taskDefinitionEndpoint.KeySelector))
            intelligenceData.Add("KeySelector", taskDefinitionEndpoint.KeySelector);
          intelligenceData.Add("EndpointType", taskDefinitionEndpointType);
        }
        IVssRequestContext requestContext1 = requestContext;
        string feature = featureName;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, "ServiceEndpointProxy", feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (CustomerIntelligenceHelper), ex);
      }
    }

    public static void PublishEndpointJobTelemetry(
      IVssRequestContext requestContext,
      string jobName,
      IDictionary<string, string> jobData)
    {
      try
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        if (jobData != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) jobData)
            properties.Add(keyValuePair.Key, keyValuePair.Value);
        }
        service.Publish(requestContext, "ServiceEndpoints", jobName, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (CustomerIntelligenceHelper), ex);
      }
    }

    public static void PublishServiceEndpointExecutionHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<ServiceEndpointExecutionRecord> executionRecords)
    {
      try
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        if (executionRecords.IsNullOrEmpty<ServiceEndpointExecutionRecord>())
          return;
        foreach (ServiceEndpointExecutionRecord executionRecord in (IEnumerable<ServiceEndpointExecutionRecord>) executionRecords)
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          Guid endpointId = executionRecord.EndpointId;
          properties.Add("connectionId", (object) endpointId);
          ServiceEndpointExecutionData data = executionRecord.Data;
          if (data != null)
          {
            if (data.PlanType.Equals("Build", StringComparison.OrdinalIgnoreCase))
              properties.Add("BuildIdentifier", new StringBuilder(projectId.ToString()).Append(":").Append(executionRecord.Data.Owner.Id).ToString());
            else
              properties.Add("ReleaseIdentifier", new StringBuilder(projectId.ToString()).Append(":").Append(executionRecord.Data.Owner.Id).ToString());
            if (!string.IsNullOrWhiteSpace(data.EndpointType))
            {
              properties.Add("connectionType", data.EndpointType);
              if ("azurerm".Equals(data.EndpointType, StringComparison.OrdinalIgnoreCase) || "azure".Equals(data.EndpointType, StringComparison.OrdinalIgnoreCase))
              {
                if (data.EndpointData != null)
                {
                  string str = (string) data.EndpointData["subscriptionId"];
                  if (!string.IsNullOrWhiteSpace(str))
                    properties.Add("subscriptionId", str);
                }
                if (data.EndpointAuthorizationParameters != null)
                {
                  string authorizationParameter = (string) data.EndpointAuthorizationParameters["serviceprincipalid"];
                  if (!string.IsNullOrWhiteSpace(authorizationParameter))
                    properties.Add("serviceprincipalid", authorizationParameter);
                }
              }
              else if (data.EndpointUrl != (Uri) null && string.Equals(data.EndpointType, "kubernetes", StringComparison.OrdinalIgnoreCase))
                properties.Add("CloudProvider", CustomerIntelligenceHelper.GetKubernetesCloudProvider(data.EndpointUrl));
            }
            properties.Add("JobStartTime", (object) data.StartTime);
            properties.Add("JobEndTime", (object) data.FinishTime);
            properties.Add("JobResult", data.Result.ToString());
          }
          service.Publish(requestContext, "ServiceEndpoints", "ServiceConnectionExecutionRequest", properties);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (CustomerIntelligenceHelper), ex);
      }
    }

    private static string GetKubernetesCloudProvider(Uri endpointUrl)
    {
      string kubernetesCloudProvider = "Others";
      if (endpointUrl.Host.ToUpperInvariant().Contains("AZMK8S.IO"))
        kubernetesCloudProvider = "Azure AKS";
      else if (endpointUrl.Host.ToUpperInvariant().Contains("EKS.AMAZONAWS.COM"))
        kubernetesCloudProvider = "Amazon AWS";
      return kubernetesCloudProvider;
    }

    private static string GetConditionType(string condition)
    {
      condition = condition?.Trim();
      if (string.IsNullOrEmpty(condition) || string.Equals(condition, "succeeded()", StringComparison.OrdinalIgnoreCase))
        return "succeeded";
      if (string.Equals(condition, "always()", StringComparison.OrdinalIgnoreCase))
        return "always";
      if (string.Equals(condition, "canceled()", StringComparison.OrdinalIgnoreCase))
        return "canceled";
      if (string.Equals(condition, "failed()", StringComparison.OrdinalIgnoreCase))
        return "failed";
      return string.Equals(condition, "succeededOrFailed()", StringComparison.OrdinalIgnoreCase) ? "succeededOrFailed" : "custom";
    }
  }
}
