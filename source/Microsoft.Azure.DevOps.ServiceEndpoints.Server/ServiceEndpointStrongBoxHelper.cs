// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointStrongBoxHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal class ServiceEndpointStrongBoxHelper
  {
    private const string c_layer = "ServiceEndpointStrongBoxHelper";

    public static bool TryGetEndpointDataFromStrongBox(
      IVssRequestContext requestContext,
      Guid endpointId,
      out Dictionary<string, string> strongBoxEndpointData)
    {
      strongBoxEndpointData = (Dictionary<string, string>) null;
      string strongBoxContent = ServiceEndpointStrongBoxHelper.GetStrongBoxContent(requestContext, endpointId, "EndpointData");
      if (string.IsNullOrEmpty(strongBoxContent))
        return false;
      strongBoxEndpointData = JsonUtility.FromString<Dictionary<string, string>>(strongBoxContent);
      return strongBoxEndpointData != null;
    }

    public static bool TryGetAuthorizationParametersFromStrongBox(
      IVssRequestContext requestContext,
      Guid endpointId,
      out IDictionary<string, string> strongBoxAuthorizationParameters)
    {
      strongBoxAuthorizationParameters = (IDictionary<string, string>) null;
      string strongBoxContent = ServiceEndpointStrongBoxHelper.GetStrongBoxContent(requestContext, endpointId, "Authorization");
      if (string.IsNullOrEmpty(strongBoxContent))
        return false;
      try
      {
        strongBoxAuthorizationParameters = (IDictionary<string, string>) JsonUtility.FromString<Dictionary<string, string>>(strongBoxContent);
      }
      catch (Exception ex1)
      {
        try
        {
          EndpointAuthorization endpointAuthorization = JsonUtility.FromString<EndpointAuthorization>(strongBoxContent);
          strongBoxAuthorizationParameters = endpointAuthorization?.Parameters;
        }
        catch (Exception ex2)
        {
          requestContext.TraceException(34000847, nameof (ServiceEndpointStrongBoxHelper), ex2);
          throw;
        }
      }
      return strongBoxAuthorizationParameters != null;
    }

    public static bool TryRecoverAuthorizationParametersFromStrongBox(
      IVssRequestContext requestContext,
      Guid endpointId,
      out IDictionary<string, string> strongBoxAuthorizationParameters)
    {
      strongBoxAuthorizationParameters = (IDictionary<string, string>) null;
      string strongBoxContent = ServiceEndpointStrongBoxHelper.GetStrongBoxContent(requestContext, endpointId, "RecoverableAuthParams");
      if (string.IsNullOrEmpty(strongBoxContent))
        return false;
      try
      {
        strongBoxAuthorizationParameters = (IDictionary<string, string>) JsonUtility.FromString<Dictionary<string, string>>(strongBoxContent);
      }
      catch (Exception ex1)
      {
        try
        {
          EndpointAuthorization endpointAuthorization = JsonUtility.FromString<EndpointAuthorization>(strongBoxContent);
          strongBoxAuthorizationParameters = endpointAuthorization?.Parameters;
        }
        catch (Exception ex2)
        {
          requestContext.TraceException(34000847, nameof (ServiceEndpointStrongBoxHelper), ex2);
          return false;
        }
      }
      return strongBoxAuthorizationParameters != null;
    }

    public static bool TryGetAccessToken(
      IVssRequestContext requestContext,
      Guid endpointId,
      out string accessToken)
    {
      accessToken = string.Empty;
      try
      {
        IDictionary<string, string> strongBoxAuthorizationParameters;
        if (ServiceEndpointStrongBoxHelper.TryGetAuthorizationParametersFromStrongBox(requestContext, endpointId, out strongBoxAuthorizationParameters))
        {
          if (strongBoxAuthorizationParameters.TryGetValue("AccessToken", out accessToken))
            return !string.IsNullOrEmpty(accessToken);
        }
      }
      catch (Exception ex)
      {
        string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to retrieve strong box item info for endpoint : {0} with exception: {1}", (object) endpointId, (object) ex.Message);
        requestContext.TraceError(34000821, nameof (ServiceEndpointStrongBoxHelper), format);
      }
      return false;
    }

    public static Guid GetOrCreateDrawer(
      IVssRequestContext requestContext,
      Guid endpointId,
      out bool isStrongBoxCreated)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string drawerNameForEndpoint = ServiceEndpointStrongBoxHelper.GetDrawerNameForEndpoint(endpointId);
      isStrongBoxCreated = false;
      Guid drawer = service.UnlockDrawer(requestContext1, drawerNameForEndpoint, false);
      if (drawer == Guid.Empty)
      {
        drawer = service.CreateDrawer(requestContext1, drawerNameForEndpoint);
        isStrongBoxCreated = true;
      }
      return drawer;
    }

    public static Guid GetDrawer(IVssRequestContext requestContext, Guid endpointId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string drawerNameForEndpoint = ServiceEndpointStrongBoxHelper.GetDrawerNameForEndpoint(endpointId);
      IVssRequestContext requestContext1 = vssRequestContext;
      string name = drawerNameForEndpoint;
      return service.UnlockDrawer(requestContext1, name, false);
    }

    public static void AddStrongBoxContent(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      string content)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      requestContext.GetService<ITeamFoundationStrongBoxService>().AddString(requestContext1, drawerId, lookupKey, content);
    }

    public static void DeleteStrongBoxDrawer(IVssRequestContext requestContext, Guid drawerId)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      requestContext.GetService<ITeamFoundationStrongBoxService>().DeleteDrawer(requestContext1, drawerId);
    }

    public static void DeleteStrongBoxDrawer(IVssRequestContext requestContext, string drawerName)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext1, drawerName, false);
      service.DeleteDrawer(requestContext1, drawerId);
    }

    public static string GetStrongBoxContent(
      IVssRequestContext requestContext,
      string strongBoxName,
      string lookUpKey)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo strongBoxInfo = ServiceEndpointStrongBoxHelper.GetStrongBoxInfo(requestContext, strongBoxName, lookUpKey);
      return strongBoxInfo != null ? service.GetString(requestContext1, strongBoxInfo.DrawerId, strongBoxInfo.LookupKey) : (string) null;
    }

    public static void DeleteStrongBoxContent(
      IVssRequestContext requestContext,
      string strongBoxName,
      string lookUpKey)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo strongBoxInfo = ServiceEndpointStrongBoxHelper.GetStrongBoxInfo(requestContext, strongBoxName, lookUpKey);
      if (strongBoxInfo == null)
        return;
      service.DeleteItem(requestContext1, strongBoxInfo.DrawerId, strongBoxInfo.LookupKey);
    }

    public static string GetStrongBoxContent(
      IVssRequestContext requestContext,
      StrongBoxItemInfo strongBoxItemInfo)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      return strongBoxItemInfo != null ? service.GetString(requestContext1, strongBoxItemInfo) : (string) null;
    }

    public static StrongBoxItemInfo GetStrongBoxInfo(
      IVssRequestContext requestContext,
      string strongBoxName,
      string lookUpKey)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      return requestContext.GetService<ITeamFoundationStrongBoxService>().GetItemInfo(requestContext1, strongBoxName, lookUpKey, false);
    }

    private static string GetStrongBoxContent(
      IVssRequestContext requestContext,
      Guid endpointId,
      string lookUpKey)
    {
      string drawerNameForEndpoint = ServiceEndpointStrongBoxHelper.GetDrawerNameForEndpoint(endpointId);
      return ServiceEndpointStrongBoxHelper.GetStrongBoxContent(requestContext, drawerNameForEndpoint, lookUpKey);
    }

    public static void DeleteStrongBoxContent(
      IVssRequestContext requestContext,
      Guid endpointId,
      string lookUpKey)
    {
      string drawerNameForEndpoint = ServiceEndpointStrongBoxHelper.GetDrawerNameForEndpoint(endpointId);
      ServiceEndpointStrongBoxHelper.DeleteStrongBoxContent(requestContext, drawerNameForEndpoint, lookUpKey);
    }

    private static string GetDrawerNameForEndpoint(Guid endpointId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.ServiceEndpointData", (object) endpointId);

    public static void UpdateStrongBoxDrawer(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      Guid drawer = ServiceEndpointStrongBoxHelper.GetOrCreateDrawer(requestContext, endpoint.Id, out bool _);
      ServiceEndpointStrongBoxHelper.AddStrongBoxContent(requestContext, drawer, "Authorization", JsonUtility.ToString((object) endpoint.Authorization.Parameters));
    }
  }
}
