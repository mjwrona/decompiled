// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesResult`1
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  public class KubernetesResult<T>
  {
    public KubernetesResult(T result, string errorMessage, HttpStatusCode httpStatusCode)
    {
      this.Result = result;
      this.ErrorMessage = errorMessage;
      this.StatusCode = httpStatusCode;
    }

    public T Result { get; private set; }

    public string ErrorMessage { get; private set; }

    public HttpStatusCode StatusCode { get; private set; }

    public bool IsSuccessful => HttpStatusCode.OK <= this.StatusCode && this.StatusCode <= (HttpStatusCode) 299;

    public static KubernetesResult<T> Error(string errorMessage, HttpStatusCode httpStatusCode) => new KubernetesResult<T>(default (T), errorMessage, httpStatusCode);

    public static KubernetesResult<T> Success(T result, HttpStatusCode httpStatusCode) => new KubernetesResult<T>(result, (string) null, httpStatusCode);
  }
}
