// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CsmControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [CaptureExternalOperationIdHeader("x-ms-correlation-request-id")]
  [SetCsmV2ResponseHeaders]
  public abstract class CsmControllerBase : CommerceControllerBase
  {
    protected const string ResourceProviderNamespace = "Microsoft.VisualStudio";
    private const string TraceLayer = "CsmControllerBase";

    protected CsmHttpClient SpsCsmResourceProviderClient => this.TfsRequestContext.GetClient<CsmHttpClient>();

    protected CsmResourceProviderHttpClient CommerceCsmResourceProviderClient => this.TfsRequestContext.GetClient<CsmResourceProviderHttpClient>();

    protected HttpResponseMessage CreateHttpResponse(object responseObject) => new HttpResponseMessage()
    {
      Content = (HttpContent) new StringContent(JsonConvert.SerializeObject(responseObject), Encoding.UTF8, "application/json")
    };

    protected T ExecuteWithForwarding<T, R>(
      IVssRequestContext requestContext,
      Func<T> crossServiceRequest,
      Func<T> localHandler,
      Func<T, T, bool> compare,
      Func<bool> shouldCallRemote,
      Func<T, R> extractValue,
      int enterTracePoint,
      int mismatchTracePoint,
      int exceptionTracePoint)
    {
      bool flag = shouldCallRemote();
      if (flag && CommerceDeploymentHelper.IsCsmCommerceMasterEnabled(requestContext))
      {
        if (requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection || requestContext.ServiceHost.HostType == TeamFoundationHostType.Application)
          CollectionHelper.FaultInCollection(requestContext);
        return crossServiceRequest();
      }
      DualWritesHelper.SetDualWriteMode(requestContext, !flag);
      T obj1 = localHandler();
      if (flag)
      {
        if (requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection || requestContext.ServiceHost.HostType == TeamFoundationHostType.Application)
          CollectionHelper.FaultInCollection(requestContext);
        T obj2 = default (T);
        try
        {
          requestContext.Trace(enterTracePoint, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Forwarding call to commerce");
          obj2 = crossServiceRequest();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(exceptionTracePoint, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, ex);
        }
        if (compare != null && !compare(obj1, obj2))
        {
          JsonSerializerSettings settings = new JsonSerializerSettings()
          {
            NullValueHandling = NullValueHandling.Include
          };
          string str1 = JsonConvert.SerializeObject((object) extractValue(obj1), settings);
          string str2 = JsonConvert.SerializeObject((object) extractValue(obj2), settings);
          requestContext.TraceAlways(mismatchTracePoint, TraceLevel.Info, this.TraceArea, this.ControllerContext.ControllerDescriptor.ControllerName, "Data mismatch between services." + Environment.NewLine + "Local response: " + str1 + Environment.NewLine + "Remote response: " + str2);
        }
      }
      return obj1;
    }

    public override string TraceArea => "Commerce";
  }
}
