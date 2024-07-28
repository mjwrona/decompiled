// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IVssIdentitySystemUserServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IVssIdentitySystemUserServiceExtensions
  {
    private const string Area = "Identity";
    private const string Layer = "IdentitySystemUserService";

    public static Microsoft.VisualStudio.Services.Identity.Identity ChangeAcsServiceIdentityIdentifier(
      this IVssIdentitySystemUserService identityServiceUserService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity acsdServiceIdentity,
      string newIdentifier)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      acsdServiceIdentity.Descriptor = new IdentityDescriptor(acsdServiceIdentity.Descriptor)
      {
        Identifier = newIdentifier
      };
      requestContext.GetService<IdentityService>().UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        acsdServiceIdentity
      });
      return acsdServiceIdentity;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity CreateAcsServiceIdentity(
      this IVssIdentitySystemUserService identityServiceUserService,
      IVssRequestContext requestContext,
      string role,
      string identifier,
      string name)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity.Descriptor = new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", identifier);
      identity.ProviderDisplayName = name;
      identity.IsActive = true;
      identity.Members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      Microsoft.VisualStudio.Services.Identity.Identity acsServiceIdentity1 = identity;
      acsServiceIdentity1.SetProperty("Domain", (object) role);
      acsServiceIdentity1.SetProperty("Account", (object) name);
      try
      {
        service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
        {
          acsServiceIdentity1
        });
        return acsServiceIdentity1;
      }
      catch (IdentityAccountNameCollisionRepairUnsafeException ex)
      {
        requestContext.Trace(82900, TraceLevel.Warning, "Identity", "IdentitySystemUserService", string.Format("Identity account name [{0}] already in use.  Repairing.  Exception: {1}", (object) name, (object) ex));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity acsServiceIdentity2 = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, IdentitySearchFilter.AccountName, role + "\\" + name, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (acsServiceIdentity2 != null)
          return acsServiceIdentity2;
        requestContext.Trace(82901, TraceLevel.Error, "Identity", "IdentitySystemUserService", string.Format("Expected identity [{0}] to exist at deployment, given the recieved {1}.", (object) acsServiceIdentity1, (object) "IdentityAccountNameCollisionRepairUnsafeException"));
        throw;
      }
    }
  }
}
