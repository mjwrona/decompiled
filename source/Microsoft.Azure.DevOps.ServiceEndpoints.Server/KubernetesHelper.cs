// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.KubernetesHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class KubernetesHelper
  {
    private const string c_layer = "KubernetesHelper";
    private const string SArea = "DistributedTask";
    private const string SLayer = "KubernetesEndpoint";

    internal static void QueueKubernetesEndpointJob(
      IVssRequestContext requestContext,
      KubernetesEndpointJobData jobData)
    {
      using (new MethodScope(requestContext, nameof (KubernetesHelper), nameof (QueueKubernetesEndpointJob)))
      {
        try
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) jobData);
          requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, "KubernetesEndpointJob", "Microsoft.Azure.DevOps.ServiceEndpoints.Server.Extensions.KubernetesEndpointJob", xml, JobPriorityLevel.Highest, new TimeSpan(0, 0, 0));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, TraceLevel.Error, "DistributedTask", "KubernetesEndpoint", ex);
        }
      }
    }

    public static void SetupServiceAccount(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      Guid scopeIdentifier,
      IDictionary<string, string> telemetryData)
    {
      string kubernetesNamespace;
      if (!endpoint.Data.TryGetValue("namespace", out kubernetesNamespace) || string.IsNullOrEmpty(kubernetesNamespace))
        kubernetesNamespace = "default";
      string str1;
      bool result1;
      bool useClusterAdmin = ((!endpoint.Data.TryGetValue("clusterAdmin", out str1) ? 0 : (bool.TryParse(str1, out result1) ? 1 : 0)) & (result1 ? 1 : 0)) != 0;
      bool isRbacEnabled = false;
      KubernetesHelper.InvokeActionWithRetry(5, (Action) (() => isRbacEnabled = KubernetesHelper.IsRbacEnabledInAksCluster(requestContext, endpoint, scopeIdentifier)));
      telemetryData.Add("isRbacEnabled", isRbacEnabled.ToString());
      string serviceAccountName = "azdev-sa-" + Guid.NewGuid().ToString().Substring(0, 6);
      KubernetesHttpClient kubernetesHttpClient = new KubernetesHttpClient(requestContext, endpoint, scopeIdentifier);
      KubernetesHelper.InvokeActionWithRetry(5, (Action) (() =>
      {
        KubernetesResult<KubernetesData.V1_13.ServiceAccount> serviceAccount1 = kubernetesHttpClient.CreateServiceAccount(kubernetesNamespace, serviceAccountName);
        if (!string.IsNullOrEmpty(serviceAccount1.ErrorMessage))
          throw new ServiceEndpointException(ServiceEndpointResources.KubernetesServiceAccountCreationError((object) serviceAccount1.ErrorMessage));
      }));
      endpoint.Authorization.Parameters["serviceAccountName"] = serviceAccountName;
      telemetryData.Add("serviceAccountName", serviceAccountName);
      if (isRbacEnabled)
      {
        string rolebindingName = "azdev-rb-" + serviceAccountName + "-admin-on-" + kubernetesNamespace;
        KubernetesHelper.InvokeActionWithRetry(5, (Action) (() =>
        {
          string str2 = !useClusterAdmin ? kubernetesHttpClient.CreateRoleBinding(kubernetesNamespace, serviceAccountName, rolebindingName, "ClusterRole", "cluster-admin").ErrorMessage : kubernetesHttpClient.CreateClusterRoleBinding(kubernetesNamespace, serviceAccountName, rolebindingName, "cluster-admin").ErrorMessage;
          if (!string.IsNullOrEmpty(str2))
            throw new ServiceEndpointException(ServiceEndpointResources.KubernetesRoleBindingCreationError((object) str2));
        }));
        endpoint.Authorization.Parameters["roleBindingName"] = rolebindingName;
        telemetryData.Add("roleBindingName", rolebindingName);
      }
      string secretName = string.Empty;
      KubernetesResult<KubernetesData.V1_13.ServiceAccount> serviceAccount = (KubernetesResult<KubernetesData.V1_13.ServiceAccount>) null;
      KubernetesHelper.InvokeActionWithRetry(5, (Action) (() =>
      {
        serviceAccount = kubernetesHttpClient.GetServiceAccount(kubernetesNamespace, serviceAccountName);
        if (!string.IsNullOrEmpty(serviceAccount.ErrorMessage))
          throw new ServiceEndpointException(ServiceEndpointResources.KubernetesGetServiceAccountError((object) serviceAccount.ErrorMessage));
      }));
      KubernetesData.V1_13.ServiceAccount result2 = serviceAccount.Result;
      KubernetesData.V1_13.ObjectReference objectReference1;
      if (result2 == null)
      {
        objectReference1 = (KubernetesData.V1_13.ObjectReference) null;
      }
      else
      {
        List<KubernetesData.V1_13.ObjectReference> secrets = result2.Secrets;
        objectReference1 = secrets != null ? secrets.FirstOrDefault<KubernetesData.V1_13.ObjectReference>() : (KubernetesData.V1_13.ObjectReference) null;
      }
      KubernetesData.V1_13.ObjectReference objectReference2 = objectReference1;
      if (objectReference2 != null)
      {
        secretName = objectReference2.Name;
      }
      else
      {
        secretName = serviceAccountName + "-secret";
        KubernetesHelper.InvokeActionWithRetry(5, (Action) (() =>
        {
          KubernetesResult<KubernetesData.V1_13.Secret> secret = kubernetesHttpClient.CreateSecret(kubernetesNamespace, secretName, serviceAccountName);
          if (!string.IsNullOrEmpty(secret.ErrorMessage))
            throw new ServiceEndpointException(ServiceEndpointResources.KubernetesGetServiceAccountError((object) secret.ErrorMessage));
        }));
      }
      endpoint.Authorization.Parameters["secretName"] = secretName;
      telemetryData.Add("secretName", secretName);
      if (serviceAccount == null)
        throw new ServiceEndpointException(ServiceEndpointResources.KubernetesGetServiceAccountError((object) string.Empty));
      KubernetesHelper.InvokeActionWithRetry(5, (Action) (() =>
      {
        KubernetesResult<KubernetesData.V1_13.Secret> secret = kubernetesHttpClient.GetSecret(kubernetesNamespace, secretName);
        if (!string.IsNullOrEmpty(secret.ErrorMessage) || secret.Result?.Data == null)
          throw new ServiceEndpointException(ServiceEndpointResources.KubernetesGetSecretError((object) secret.ErrorMessage));
        endpoint.Authorization.Parameters["ApiToken"] = secret.Result.Data.Token;
        endpoint.Authorization.Parameters["serviceAccountCertificate"] = secret.Result.Data.Ca_Crt;
      }));
    }

    public static void CreateNamespace(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      Guid scopeIdentifier,
      IDictionary<string, string> telemetryData,
      bool reuse = false)
    {
      string namespaceToCreate;
      if (!endpoint.Data.TryGetValue("namespace", out namespaceToCreate) && string.IsNullOrEmpty(namespaceToCreate))
        throw new ServiceEndpointException(ServiceEndpointResources.KubernetesCreateNamespaceJobNamespaceNotFound((object) "namespace"));
      KubernetesHttpClient kubernetesHttpClient = new KubernetesHttpClient(requestContext, endpoint, scopeIdentifier);
      KubernetesHelper.InvokeActionWithRetry(5, (Action) (() =>
      {
        KubernetesResult<KubernetesData.V1_13.Namespace> kubernetesResult = kubernetesHttpClient.CreateNamespace(namespaceToCreate);
        if (!string.IsNullOrEmpty(kubernetesResult.ErrorMessage) && (!reuse || kubernetesResult.StatusCode != HttpStatusCode.Conflict))
          throw new ServiceEndpointException(ServiceEndpointResources.KubernetesNamespaceCreationError((object) kubernetesResult.ErrorMessage));
      }));
      telemetryData.Add("creatingNamespace", "true");
    }

    private static void InvokeActionWithRetry(int count, Action action) => new RetryManager(count, TimeSpan.FromSeconds(1.0)).Invoke(action);

    private static bool IsRbacEnabledInAksCluster(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      Guid scopeIdentifier)
    {
      Dictionary<string, string> dictionary = endpoint.Data.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) endpoint.Authorization.Parameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesHelper.FetchCloudUrl(requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, endpoint.Type, endpoint.Authorization.Scheme).FirstOrDefault<ServiceEndpointType>(), dictionary);
      string str1;
      ServiceEndpointDetails serviceEndpointDetails = dictionary.TryGetValue("clusterId", out str1) && !string.IsNullOrWhiteSpace(str1) ? Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesHelper.GetAzureRmEndpointDetails(dictionary) : throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesCannotFetchClusterCredentialsMissingClusterId());
      ServiceEndpointRequest serviceEndpointRequest = new ServiceEndpointRequest()
      {
        ServiceEndpointDetails = serviceEndpointDetails,
        DataSourceDetails = new DataSourceDetails()
        {
          DataSourceName = "AzureKubernetesClusterRbacEnabled",
          Parameters = {
            {
              "clusterId",
              str1
            }
          }
        }
      };
      ServiceEndpointRequestResult endpointRequestResult = requestContext.GetService<IServiceEndpointProxyService2>().ExecuteServiceEndpointRequest(requestContext, scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest);
      JArray source = string.IsNullOrEmpty(endpointRequestResult.ErrorMessage) ? (JArray) endpointRequestResult.Result : throw new InvalidOperationException(ServiceEndpointResources.KubernetesErrorFetchingCluster((object) endpointRequestResult.ErrorMessage));
      string str2 = source != null && source.Count<JToken>() == 1 ? source[0].ToString() : throw new InvalidOperationException(ServiceEndpointResources.KubernetesErrorFetchingClusterInvalidResponse());
      bool result;
      if (string.IsNullOrWhiteSpace(str2) || !bool.TryParse(str2, out result))
        throw new InvalidOperationException(ServiceEndpointResources.KubernetesInvalidValueForPropertyEnableRbac((object) str2));
      return result;
    }
  }
}
