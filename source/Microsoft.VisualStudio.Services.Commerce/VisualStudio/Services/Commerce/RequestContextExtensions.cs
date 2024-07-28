// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.RequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class RequestContextExtensions
  {
    public static void UsingSqlComponent<TComponent>(
      this IVssRequestContext requestContext,
      Action<TComponent> action)
      where TComponent : class, ISqlResourceComponent, new()
    {
      TComponent component = requestContext.CreateComponent<TComponent>();
      try
      {
        action(component);
      }
      finally
      {
        if ((object) component != null)
          component.Dispose();
      }
    }

    public static TResult UsingSqlComponent<TComponent, TResult>(
      this IVssRequestContext requestContext,
      Func<TComponent, TResult> func)
      where TComponent : class, ISqlResourceComponent, new()
    {
      TComponent component = requestContext.CreateComponent<TComponent>();
      try
      {
        return func(component);
      }
      finally
      {
        if ((object) component != null)
          component.Dispose();
      }
    }

    public static bool IsFeatureEnabled(
      this IVssRequestContext requestContext,
      Guid hostId,
      string featureName)
    {
      return requestContext.IsFeatureEnabled(hostId, new string[1]
      {
        featureName
      })[featureName];
    }

    public static IDictionary<string, bool> IsFeatureEnabled(
      this IVssRequestContext requestContext,
      Guid hostId,
      params string[] featureNames)
    {
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
      if (requestContext.ServiceHost.InstanceId == hostId)
      {
        foreach (string featureName in featureNames)
          dictionary[featureName] = requestContext.IsFeatureEnabled(featureName);
      }
      else
      {
        using (IVssRequestContext requestContext1 = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, hostId, RequestContextType.SystemContext))
        {
          foreach (string featureName in featureNames)
            dictionary[featureName] = requestContext1.IsFeatureEnabled(featureName);
        }
      }
      return (IDictionary<string, bool>) dictionary;
    }

    public static bool IsCommerceService(this IVssRequestContext requestContext) => requestContext.ServiceInstanceType() == CommerceConstants.CommerceServiceGuid;

    public static bool IsSpsService(this IVssRequestContext requestContext) => requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS;

    public static bool IsCallerCommerceServicePrincipal(this IVssRequestContext requestContext)
    {
      Guid spGuid;
      return ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && ServicePrincipals.TryParse(requestContext.UserContext, out spGuid, out Guid _) && spGuid == CommerceConstants.CommerceServiceGuid;
    }

    public static bool IsCallerSpsServicePrincipal(this IVssRequestContext requestContext)
    {
      Guid spGuid;
      return ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && ServicePrincipals.TryParse(requestContext.UserContext, out spGuid, out Guid _) && spGuid == CommerceConstants.SpsMasterId;
    }

    public static void CheckCommerceService(this IVssRequestContext requestContext)
    {
      if (!requestContext.IsCommerceService())
        throw new UnexpectedServiceException(requestContext.ServiceInstanceType(), CommerceServiceInstanceTypes.Commerce);
    }
  }
}
