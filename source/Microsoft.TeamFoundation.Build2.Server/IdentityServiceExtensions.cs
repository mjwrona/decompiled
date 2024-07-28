// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IdentityServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class IdentityServiceExtensions
  {
    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid identifier)
    {
      return identityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identifier
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      string identifier)
    {
      return identityService.GetIdentities(requestContext, identifier).OrderBy<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (identity => identity), (IComparer<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityServiceExtensions.IdentityComparer()).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static IList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      string identifier)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      using (requestContext.TraceScope(nameof (IdentityServiceExtensions), nameof (GetIdentities)))
      {
        Guid result;
        if (Guid.TryParse(identifier, out result))
          source = identityService.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
          {
            result
          }, QueryMembership.None, (IEnumerable<string>) null);
        if ((source != null ? source.SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() : (Microsoft.VisualStudio.Services.Identity.Identity) null) == null)
        {
          if (requestContext.ExecutionEnvironment.IsHostedDeployment && !UserNameUtil.IsComplete(identifier) && ArgumentUtility.IsValidEmailAddress(identifier))
          {
            string relative = "Windows Live ID";
            Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
            if (!organizationAadTenantId.Equals(Guid.Empty))
              relative = organizationAadTenantId.ToString();
            identifier = UserNameUtil.Complete(identifier, relative);
          }
          try
          {
            if (TFCommonUtil.IsLegalIdentity(identifier))
              source = identityService.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, identifier, QueryMembership.None, (IEnumerable<string>) null);
            if (source != null)
            {
              if (source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() != null)
                goto label_18;
            }
            source = identityService.ReadIdentities(requestContext, IdentitySearchFilter.General, identifier, QueryMembership.None, (IEnumerable<string>) null);
          }
          catch (AccessCheckException ex)
          {
            requestContext.TraceInfo(12030183, nameof (IdentityServiceExtensions), ex.Message);
          }
        }
      }
label_18:
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      IdentityDescriptor identifier)
    {
      return identityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identifier
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetGroups(
      this IdentityService identityService,
      IVssRequestContext requestContext,
      Guid scopeId,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> groups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ListGroups(requestContext, new Guid[1]
      {
        scopeId
      }, false, (IEnumerable<string>) null);
      foreach (IdentityDescriptor descriptor1 in descriptors)
      {
        IdentityDescriptor descriptor = descriptor1;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => IdentityHelper.IsWellKnownGroup(x.Descriptor, descriptor)));
        groups.Add(identity);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) groups;
    }

    internal class IdentityComparer : IComparer<Microsoft.VisualStudio.Services.Identity.Identity>
    {
      public int Compare(Microsoft.VisualStudio.Services.Identity.Identity x, Microsoft.VisualStudio.Services.Identity.Identity y)
      {
        if (x.IsActive && !y.IsActive)
          return -1;
        return !x.IsActive && y.IsActive ? 1 : x.Id.CompareTo(y.Id);
      }
    }
  }
}
