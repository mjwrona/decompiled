// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesHttpClient
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  public class KubernetesHttpClient
  {
    private readonly IVssRequestContext _requestContext;
    private readonly IServiceEndpointProxyService2 _endpointProxyService;
    private ServiceEndpoint _serviceEndpoint;
    private Guid _scopeIdentifier;

    public KubernetesHttpClient(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      Guid scopeIdentifier)
    {
      this._requestContext = requestContext;
      this._serviceEndpoint = serviceEndpoint;
      this._scopeIdentifier = scopeIdentifier;
      this._endpointProxyService = requestContext.GetService<IServiceEndpointProxyService2>();
    }

    public KubernetesResult<KubernetesData.V1_13.Namespace> CreateNamespace(
      string kubernetesNamespaceName)
    {
      ServiceEndpointRequest serviceEndpointRequest = this.CreateServiceEndpointRequest();
      serviceEndpointRequest.DataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = "KubernetesCreateNamespace",
        Parameters = {
          {
            "KubernetesNamespace",
            kubernetesNamespaceName
          }
        }
      };
      return this.ToSingleKubernetesResult<KubernetesData.V1_13.Namespace>(this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest));
    }

    public KubernetesResult<KubernetesData.V1_13.ServiceAccount> CreateServiceAccount(
      string kubernetesNamespace,
      string serviceAccountName)
    {
      ServiceEndpointRequest serviceEndpointRequest = this.CreateServiceEndpointRequest();
      serviceEndpointRequest.DataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = "KubernetesCreateServiceAccount",
        Parameters = {
          {
            "KubernetesNamespace",
            kubernetesNamespace
          },
          {
            "ServiceAccountName",
            serviceAccountName
          }
        }
      };
      return this.ToSingleKubernetesResult<KubernetesData.V1_13.ServiceAccount>(this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest));
    }

    public KubernetesResult<KubernetesData.V1_13.RoleBinding> CreateRoleBinding(
      string kubernetesNamespace,
      string serviceAccountName,
      string rolebindingName,
      string roleKind,
      string roleName)
    {
      ServiceEndpointRequest serviceEndpointRequest = this.CreateServiceEndpointRequest();
      serviceEndpointRequest.DataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = "KubernetesCreateRolebinding",
        Parameters = {
          {
            "KubernetesNamespace",
            kubernetesNamespace
          },
          {
            "RoleBindingName",
            rolebindingName
          },
          {
            "ServiceAccountName",
            serviceAccountName
          },
          {
            "ServiceAccountNamespace",
            kubernetesNamespace
          },
          {
            "RoleKind",
            roleKind
          },
          {
            "RoleName",
            roleName
          }
        }
      };
      return this.ToSingleKubernetesResult<KubernetesData.V1_13.RoleBinding>(this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest));
    }

    public KubernetesResult<KubernetesData.V1_13.ClusterRoleBinding> CreateClusterRoleBinding(
      string kubernetesNamespace,
      string serviceAccountName,
      string rolebindingName,
      string roleName)
    {
      ServiceEndpointRequest serviceEndpointRequest = this.CreateServiceEndpointRequest();
      serviceEndpointRequest.DataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = "KubernetesCreateClusterRolebinding",
        Parameters = {
          {
            "ClusterRoleBindingName",
            rolebindingName
          },
          {
            "ServiceAccountName",
            serviceAccountName
          },
          {
            "ServiceAccountNamespace",
            kubernetesNamespace
          },
          {
            "RoleName",
            roleName
          }
        }
      };
      return this.ToSingleKubernetesResult<KubernetesData.V1_13.ClusterRoleBinding>(this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest));
    }

    public KubernetesResult<KubernetesData.V1_13.ServiceAccount> GetServiceAccount(
      string kubernetesNamespace,
      string serviceAccountName)
    {
      ServiceEndpointRequest serviceEndpointRequest = this.CreateServiceEndpointRequest();
      serviceEndpointRequest.DataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = "KubernetesGetServiceAccount",
        Parameters = {
          {
            "KubernetesNamespace",
            kubernetesNamespace
          },
          {
            "ServiceAccountName",
            serviceAccountName
          }
        }
      };
      return this.ToSingleKubernetesResult<KubernetesData.V1_13.ServiceAccount>(this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest));
    }

    public KubernetesResult<KubernetesData.V1_13.Secret> CreateSecret(
      string kubernetesNamespace,
      string secretName,
      string serviceAccountName)
    {
      ServiceEndpointRequest serviceEndpointRequest = this.CreateServiceEndpointRequest();
      serviceEndpointRequest.DataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = "KubernetesCreateLongLivedSecret",
        Parameters = {
          {
            "KubernetesNamespace",
            kubernetesNamespace
          },
          {
            "SecretName",
            secretName
          },
          {
            "ServiceAccountName",
            serviceAccountName
          }
        }
      };
      return this.ToSingleKubernetesResult<KubernetesData.V1_13.Secret>(this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest));
    }

    public KubernetesResult<KubernetesData.V1_13.Secret> GetSecret(
      string kubernetesNamespace,
      string secretname)
    {
      ServiceEndpointRequest serviceEndpointRequest = this.CreateServiceEndpointRequest();
      serviceEndpointRequest.DataSourceDetails = new DataSourceDetails()
      {
        DataSourceName = "KubernetesGetSecret",
        Parameters = {
          {
            "KubernetesNamespace",
            kubernetesNamespace
          },
          {
            "SecretName",
            secretname
          }
        }
      };
      return this.ToSingleKubernetesResult<KubernetesData.V1_13.Secret>(this._endpointProxyService.ExecuteServiceEndpointRequest(this._requestContext, this._scopeIdentifier, Guid.Empty.ToString(), serviceEndpointRequest));
    }

    private KubernetesResult<T> ToSingleKubernetesResult<T>(
      ServiceEndpointRequestResult endpointResult)
    {
      if (!string.IsNullOrEmpty(endpointResult.ErrorMessage) || endpointResult.Result == null)
        return KubernetesResult<T>.Error(endpointResult.ErrorMessage, endpointResult.StatusCode);
      int count = ((JContainer) endpointResult.Result).Count;
      if (count != 1)
        return KubernetesResult<T>.Error(string.Format("Incorrect response from the Kubernetes Server. Expected objects: 1, Found: {0}", (object) count), endpointResult.StatusCode);
      try
      {
        return KubernetesResult<T>.Success(JsonConvert.DeserializeObject<T>(endpointResult.Result[(object) 0].ToString()), endpointResult.StatusCode);
      }
      catch (Exception ex)
      {
        this._requestContext.TraceException(nameof (KubernetesHttpClient), ex);
        return KubernetesResult<T>.Error(string.Format("Error deserializing Kubernetes API response: {0}\r\n --result value: {1}", !string.IsNullOrEmpty(ex.InnerException?.Message) ? (object) (ex.Message + " InnerException: " + ex.InnerException.Message) : (object) ex.ToString(), (object) endpointResult.Result), endpointResult.StatusCode);
      }
    }

    private ServiceEndpointRequest CreateServiceEndpointRequest() => new ServiceEndpointRequest()
    {
      ServiceEndpointDetails = new ServiceEndpointDetails()
      {
        Type = this._serviceEndpoint.Type,
        Url = this._serviceEndpoint.Url,
        Authorization = this._serviceEndpoint.Authorization,
        Data = this._serviceEndpoint.Data
      }
    };
  }
}
