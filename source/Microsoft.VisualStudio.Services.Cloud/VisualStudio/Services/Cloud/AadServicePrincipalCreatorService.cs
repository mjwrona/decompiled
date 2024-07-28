// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AadServicePrincipalCreatorService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AadServicePrincipalCreatorService : 
    IAadServicePrincipalCreatorService,
    IVssFrameworkService
  {
    private static readonly Guid s_mmsProvisionerAuthServicePrincipalGuid = Guid.Parse("00000048-0000-8888-8000-000000000000");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> CreateAADServicePrincipalIdentities(
      IVssRequestContext targetRequestContext,
      IServicingContext servicingContext,
      ServicePrincipalIdentity[] spIdentities)
    {
      CachedRegistryService service = targetRequestContext.GetService<CachedRegistryService>();
      RegistryEntry registryEntry = service.ReadEntriesFallThru(targetRequestContext, (RegistryQuery) (OAuth2RegistryConstants.S2SRoot + "/*")).FirstOrDefault<RegistryEntry>((Func<RegistryEntry, bool>) (x => x.Name.Equals("AuthEnabled", StringComparison.OrdinalIgnoreCase)));
      bool flag;
      if (registryEntry != null)
      {
        flag = registryEntry.GetValue<bool>(false);
      }
      else
      {
        string query = OAuth2RegistryConstants.Root + "/Enabled";
        servicingContext.LogInfo("Upgrading from a Pre-M59 service. Inspecing existing {0} registry path for S2SAuth enablement", (object) query);
        flag = service.GetValue<bool>(targetRequestContext, (RegistryQuery) query, false);
      }
      if (flag)
      {
        string s2StenantId = this.GetS2STenantId(targetRequestContext, (IVssRegistryService) service);
        string servicePrincipalName = OAuth2RegistryConstants.S2SWellKnownServicePrincipalName;
        return this.CreateAADServicePrincipalIdentities(targetRequestContext, servicingContext, spIdentities, s2StenantId, servicePrincipalName);
      }
      servicingContext.Warn("S2SOAuth is *not* enabled, skipping creating identities, further errors may occur.");
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[0];
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> CreateAADServicePrincipalIdentities(
      IVssRequestContext targetRequestContext,
      IServicingContext servicingContext,
      ServicePrincipalIdentity[] spIdentities,
      string tenantId,
      string aadId)
    {
      targetRequestContext.GetService<IdentityService>();
      servicingContext.LogInfo("Configuring {0} service principal identities", (object) spIdentities.Length);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      for (int index = 0; index < spIdentities.Length; ++index)
      {
        ServicePrincipalIdentity spIdentity = spIdentities[index];
        servicingContext.LogInfo("Congfiguring identity for service principal {0}.", (object) spIdentity.ServicePrincipal);
        string identifier = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", (object) spIdentity.ServicePrincipal, (object) tenantId);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", (object) aadId, (object) tenantId);
        Guid servicePrincipal = new Guid(spIdentity.ServicePrincipal);
        if (!ServicePrincipals.IsInternalServicePrincipalId(servicePrincipal) && servicePrincipal != FirstPartyS2SAuthTokenValidator.s_firstPartyIntAadApplicationId && servicePrincipal != FirstPartyS2SAuthTokenValidator.s_firstPartyProdAadApplicationId)
        {
          servicingContext.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The service principal guid was not in the correct format. ServicePrincipal: {0}", (object) servicePrincipal));
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities;
        }
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
        identity1.Id = servicePrincipal;
        identity1.Descriptor = new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", identifier);
        identity1.ProviderDisplayName = identifier;
        identity1.IsActive = true;
        identity1.UniqueUserId = 0;
        identity1.IsContainer = false;
        identity1.Members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
        identity1.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
        identity1.CustomDisplayName = spIdentity.DisplayName;
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
        identity2.SetProperty("Domain", (object) str);
        identity2.SetProperty("Account", (object) identifier);
        identity2.SetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", (object) identifier);
        identity2.SetProperty("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", (object) str);
        identities.Add(identity2);
      }
      if (identities.Count > 0)
      {
        servicingContext.LogInfo("Creating {0} identities for service principals", (object) identities.Count);
        this.UpdateIdentities(targetRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities);
      }
      else
        servicingContext.LogInfo("No service principal identities to create.");
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities;
    }

    protected virtual string GetS2STenantId(
      IVssRequestContext targetRequestContext,
      IVssRegistryService registry)
    {
      return registry.GetValue(targetRequestContext, (RegistryQuery) OAuth2RegistryConstants.S2STenantId, false, (string) null);
    }

    protected virtual void UpdateIdentities(
      IVssRequestContext targetRequestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      targetRequestContext.GetService<IdentityService>().UpdateIdentities(targetRequestContext, identities);
    }
  }
}
