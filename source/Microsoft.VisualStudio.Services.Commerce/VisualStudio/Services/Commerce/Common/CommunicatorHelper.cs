// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.CommunicatorHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce.Common
{
  public class CommunicatorHelper
  {
    public IList<Microsoft.VisualStudio.Services.Identity.Identity> GetProjectCollectionAdministratorIdentities(
      IVssRequestContext requestContext)
    {
      HashSet<Microsoft.VisualStudio.Services.Identity.Identity> identitySet = new HashSet<Microsoft.VisualStudio.Services.Identity.Identity>();
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity1 = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      }, QueryMembership.Direct, (IEnumerable<string>) null)[0];
      IdentityService dentityService1 = service;
      IVssRequestContext requestContext1 = requestContext;
      ICollection<IdentityDescriptor> members1 = readIdentity1.Members;
      List<IdentityDescriptor> list1 = members1 != null ? members1.ToList<IdentityDescriptor>() : (List<IdentityDescriptor>) null;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity2 in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) dentityService1.ReadIdentities(requestContext1, (IList<IdentityDescriptor>) list1, QueryMembership.Direct, (IEnumerable<string>) null))
      {
        string mailValue;
        if (readIdentity2.Properties.TryGetValue<string>("Mail", out mailValue) && !string.IsNullOrEmpty(mailValue))
        {
          identitySet.Add(readIdentity2);
        }
        else
        {
          IdentityService dentityService2 = service;
          IVssRequestContext requestContext2 = requestContext;
          ICollection<IdentityDescriptor> members2 = readIdentity2.Members;
          List<IdentityDescriptor> list2 = members2 != null ? members2.ToList<IdentityDescriptor>() : (List<IdentityDescriptor>) null;
          IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> values = dentityService2.ReadIdentities(requestContext2, (IList<IdentityDescriptor>) list2, QueryMembership.Direct, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) id) && id.Properties.TryGetValue<string>("Mail", out mailValue) && !string.IsNullOrEmpty(mailValue)));
          identitySet.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, HashSet<Microsoft.VisualStudio.Services.Identity.Identity>>(values);
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identitySet.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public IList<string> GetAdminCoAdminMailAddresses(
      IVssRequestContext collectionContext,
      Guid subscriptionId)
    {
      IVssRequestContext context = collectionContext.To(TeamFoundationHostType.Deployment);
      IAzureBillingService service1 = context.GetService<IAzureBillingService>();
      IAzureResourceHelper service2 = context.GetService<IAzureResourceHelper>();
      IVssRequestContext requestContext1 = context;
      Guid subscriptionId1 = subscriptionId;
      CommerceBillingContextInfo contextForSubscription = service1.GetBillingContextForSubscription(requestContext1, subscriptionId1);
      if (contextForSubscription != null)
      {
        Guid? tenantId1 = contextForSubscription.TenantId;
        if (tenantId1.HasValue)
        {
          IAzureResourceHelper azureResourceHelper = service2;
          IVssRequestContext requestContext2 = context;
          Guid subscriptionId2 = subscriptionId;
          tenantId1 = contextForSubscription.TenantId;
          Guid tenantId2 = tenantId1.Value;
          return (IList<string>) azureResourceHelper.GetEmailsOfAdminAndCoAdmins(requestContext2, subscriptionId2, tenantId2);
        }
      }
      return (IList<string>) null;
    }
  }
}
