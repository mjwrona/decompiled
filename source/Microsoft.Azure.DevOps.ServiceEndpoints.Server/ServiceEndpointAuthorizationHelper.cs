// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointAuthorizationHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal class ServiceEndpointAuthorizationHelper
  {
    public static void StoreSecretsInStrongBox(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      ServiceEndpointType endpointType,
      bool isUpdate)
    {
      using (new MethodScope(requestContext, "PlatformServiceEndpointService", nameof (StoreSecretsInStrongBox)))
      {
        bool isStrongBoxCreated;
        Guid drawer = ServiceEndpointStrongBoxHelper.GetOrCreateDrawer(requestContext, endpoint.Id, out isStrongBoxCreated);
        if (isUpdate)
        {
          ServiceEndpointAuthorizationHelper.PatchEndpointData(requestContext, endpoint);
          ServiceEndpointAuthorizationHelper.PatchEndpointAuthorizationParameters(requestContext, endpoint);
        }
        try
        {
          ServiceEndpointStrongBoxHelper.AddStrongBoxContent(requestContext, drawer, "EndpointData", JsonUtility.ToString((object) endpoint.GetFilteredEndpointData(endpointType, true)));
          string content = JsonUtility.ToString((object) endpoint.GetFilteredAuthorizationParameters(endpointType, true));
          ServiceEndpointStrongBoxHelper.AddStrongBoxContent(requestContext, drawer, "Authorization", content);
        }
        catch (Exception ex)
        {
          if (isStrongBoxCreated)
            ServiceEndpointStrongBoxHelper.DeleteStrongBoxDrawer(requestContext, drawer);
          throw;
        }
      }
    }

    public static void StoreAuthorizationSchemeInputsForRecovery(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      List<string> inputsToStore)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(endpoint, nameof (endpoint));
      ArgumentUtility.CheckForNull<List<string>>(inputsToStore, nameof (inputsToStore));
      using (new MethodScope(requestContext, "PlatformServiceEndpointService", nameof (StoreAuthorizationSchemeInputsForRecovery)))
      {
        Guid drawer = ServiceEndpointStrongBoxHelper.GetDrawer(requestContext, endpoint.Id);
        ServiceEndpointAuthorizationHelper.PatchEndpointAuthorizationParameters(requestContext, endpoint);
        IDictionary<string, string> authorizationParameters = endpoint.GetAuthorizationParameters(inputsToStore);
        ServiceEndpointStrongBoxHelper.AddStrongBoxContent(requestContext, drawer, "RecoverableAuthParams", JsonUtility.ToString((object) authorizationParameters));
      }
    }

    public static bool TryRecoverOldAuthorizationSchemeParameters(
      IVssRequestContext requestContext,
      ServiceEndpoint endpointToRecover)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(endpointToRecover, nameof (endpointToRecover));
      using (new MethodScope(requestContext, "PlatformServiceEndpointService", nameof (TryRecoverOldAuthorizationSchemeParameters)))
      {
        IDictionary<string, string> strongBoxAuthorizationParameters;
        if (!ServiceEndpointStrongBoxHelper.TryRecoverAuthorizationParametersFromStrongBox(requestContext, endpointToRecover.Id, out strongBoxAuthorizationParameters))
          return false;
        bool? nullable;
        if (endpointToRecover == null)
        {
          nullable = new bool?();
        }
        else
        {
          EndpointAuthorization authorization = endpointToRecover.Authorization;
          nullable = authorization != null ? new bool?(authorization.Parameters.TryAddRange<string, string, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) strongBoxAuthorizationParameters)) : new bool?();
        }
        return nullable.GetValueOrDefault();
      }
    }

    public static void ClearAuthorizationSchemeInputsForRecovery(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(endpoint, nameof (endpoint));
      using (new MethodScope(requestContext, "PlatformServiceEndpointService", nameof (ClearAuthorizationSchemeInputsForRecovery)))
        ServiceEndpointStrongBoxHelper.DeleteStrongBoxContent(requestContext, endpoint.Id, "RecoverableAuthParams");
    }

    private static void PatchEndpointData(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      Dictionary<string, string> strongBoxEndpointData;
      if (!ServiceEndpointStrongBoxHelper.TryGetEndpointDataFromStrongBox(requestContext, endpoint.Id, out strongBoxEndpointData))
        return;
      ServiceEndpointHelper.PatchEndpointSecrets(endpoint?.Data, (IDictionary<string, string>) strongBoxEndpointData);
    }

    private static void PatchEndpointAuthorizationParameters(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      IDictionary<string, string> strongBoxAuthorizationParameters;
      if (!ServiceEndpointStrongBoxHelper.TryGetAuthorizationParametersFromStrongBox(requestContext, endpoint.Id, out strongBoxAuthorizationParameters))
        return;
      ServiceEndpointHelper.PatchEndpointSecrets(endpoint?.Authorization?.Parameters, strongBoxAuthorizationParameters);
    }
  }
}
