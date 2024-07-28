// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceIdentityHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceIdentityHelper
  {
    private static readonly Guid CommerceContext = new Guid("B124DC54-4263-4A99-9C2D-E1B1D5AAA2F5");

    public static Guid? GetRequestAuthenticatedIdentityId(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      return authenticatedIdentity != null ? new Guid?(authenticatedIdentity.MasterId != Guid.Empty ? authenticatedIdentity.MasterId : authenticatedIdentity.Id) : new Guid?();
    }

    public static Guid UserIdentityIdOrCommerceContext(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity == null ? CommerceIdentityHelper.CommerceContext : userIdentity.MasterId;
    }

    public static Guid UserIdentityIdOrCommerceContext(
      IVssRequestContext requestContext,
      out Guid userCUID)
    {
      userCUID = CommerceIdentityHelper.CommerceContext;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null)
        return CommerceIdentityHelper.CommerceContext;
      userCUID = userIdentity.Cuid();
      return userIdentity.MasterId;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      if (authenticatedIdentity == null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (userIdentity != null && !userIdentity.Id.Equals(Guid.Empty))
          return userIdentity;
      }
      return authenticatedIdentity;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ReadCspIdentity(
      IVssRequestContext requestContext,
      Guid domainId,
      string puid)
    {
      SubjectDescriptor partnerByProviderInfo = requestContext.GetService<IGraphIdentifierConversionService>().GetDescriptorForCspPartnerByProviderInfo(requestContext, domainId.ToString(), puid);
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        partnerByProviderInfo
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static string GetEmailFromUpn(string upn)
    {
      if (string.IsNullOrEmpty(upn) || !upn.Contains<char>('#'))
        return upn;
      int num = upn.IndexOf('#');
      return num < 0 ? upn : upn.Substring(num + 1);
    }
  }
}
