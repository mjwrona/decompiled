// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.OAuthConfigurationSecretsHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal class OAuthConfigurationSecretsHelper : IOAuthConfigurationSecretsHelper
  {
    private const string c_layer = "OAuthConfigurationSecretsHelper";

    public void StoreSecrets(
      IVssRequestContext requestContext,
      Guid configurationId,
      string clientId,
      string clientSecret)
    {
      IVssRequestContext context = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = context.GetService<ITeamFoundationStrongBoxService>();
      string configurationDetails = this.GetDrawerNameForConfigurationDetails(configurationId);
      Guid drawer = this.GetOrCreateDrawer(requestContext, configurationDetails);
      List<Tuple<StrongBoxItemInfo, string>> tupleList = new List<Tuple<StrongBoxItemInfo, string>>();
      StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo()
      {
        LookupKey = clientId,
        DrawerId = drawer,
        ItemKind = StrongBoxItemKind.String
      };
      tupleList.Add(new Tuple<StrongBoxItemInfo, string>(strongBoxItemInfo, clientSecret));
      IVssRequestContext requestContext1 = context;
      List<Tuple<StrongBoxItemInfo, string>> items = tupleList;
      service.AddStrings(requestContext1, items);
    }

    public string ReadSecrets(
      IVssRequestContext requestContext,
      Guid configurationId,
      string clientId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      string configurationDetails = this.GetDrawerNameForConfigurationDetails(configurationId);
      try
      {
        Guid drawerId = service.UnlockDrawer(vssRequestContext, configurationDetails, false);
        return service.GetString(vssRequestContext, drawerId, clientId);
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        requestContext.TraceError(34000823, "ServiceEndpoints", string.Format("StrongBox drawer not found for Configuration. ConfigurationId: {0}. DrawerName: {1} Exception: {2}", (object) configurationId, (object) configurationDetails, (object) ex.Message));
        throw new OAuthConfigurationNotFoundException(ServiceEndpointResources.OAuthConfigurationSecretsNotFound((object) configurationId));
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        requestContext.TraceError(34000824, "ServiceEndpoints", string.Format("ClientSecret not found for ClientId. ConfigurationId: {0} DrawerName: {1} ClientId: {2}. Exception: {3}", (object) configurationId, (object) configurationDetails, (object) clientId, (object) ex.Message));
        throw new OAuthConfigurationNotFoundException(ServiceEndpointResources.OAuthConfigurationSecretsNotFound((object) configurationId));
      }
    }

    public void DeleteSecrets(IVssRequestContext requestContext, Guid configurationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      string configurationDetails = this.GetDrawerNameForConfigurationDetails(configurationId);
      Guid drawerId = service.UnlockDrawer(vssRequestContext, configurationDetails, false);
      if (drawerId == Guid.Empty)
        requestContext.TraceInfo(nameof (OAuthConfigurationSecretsHelper), "No drawer id exists for conifgurationID: {0}", (object) configurationId);
      else
        service.DeleteDrawer(vssRequestContext, drawerId);
    }

    private Guid GetOrCreateDrawer(IVssRequestContext requestContext, string drawerName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawer = service.UnlockDrawer(vssRequestContext, drawerName, false);
      if (drawer != Guid.Empty)
        return drawer;
      try
      {
        return service.CreateDrawer(vssRequestContext, drawerName);
      }
      catch (StrongBoxDrawerExistsException ex)
      {
        return service.UnlockDrawer(vssRequestContext, drawerName, true);
      }
    }

    private string GetDrawerNameForConfigurationDetails(Guid configurationId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/DistributedTask/OAuthConfiguration/{0}", (object) configurationId);
  }
}
