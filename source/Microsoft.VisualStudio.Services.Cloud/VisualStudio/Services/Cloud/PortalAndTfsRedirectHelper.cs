// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.PortalAndTfsRedirectHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.HostAuthentication;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class PortalAndTfsRedirectHelper
  {
    private const string c_area = "UrlRedirection";
    private const string c_layer = "PortalAndTfsRedirectHelper";
    private static readonly Guid s_aexInstanceType = new Guid("00000041-0000-8888-8000-000000000000");

    public static string GetRedirectUrl(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      string b = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, in ConfigurationConstants.DevOpsRootDomain);
      string redirectUrl;
      if (string.Equals(requestContext.RequestUri().Host, b, StringComparison.OrdinalIgnoreCase))
      {
        if (requestContext.IsAnonymous())
        {
          redirectUrl = "https://azure.microsoft.com/services/devops/?nav=min";
        }
        else
        {
          Microsoft.VisualStudio.Services.Account.Account organizationForNavigation = PortalAndTfsRedirectHelper.GetOrganizationForNavigation(requestContext);
          redirectUrl = organizationForNavigation == null ? PortalAndTfsRedirectHelper.GetRedirectUrl(requestContext, PortalAndTfsRedirectHelper.s_aexInstanceType, "profile/account") : Microsoft.TeamFoundation.Framework.Server.Organization.OrganizationHelper.GetOrganizationUrl(requestContext, organizationForNavigation, true);
        }
      }
      else
        redirectUrl = PortalAndTfsRedirectHelper.GetRedirectUrl(requestContext, ServiceInstanceTypes.SPS, "go/profile");
      return redirectUrl;
    }

    private static Microsoft.VisualStudio.Services.Account.Account GetOrganizationForNavigation(
      IVssRequestContext requestContext)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Account.Account> source1 = Microsoft.TeamFoundation.Framework.Server.Organization.OrganizationHelper.GetOrganizationsForRequestIdentity(requestContext);
      if (!source1.Any<Microsoft.VisualStudio.Services.Account.Account>())
        return (Microsoft.VisualStudio.Services.Account.Account) null;
      IUserService service = requestContext.GetService<IUserService>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      IList<AccessedHost> accessedHostList = (IList<AccessedHost>) Array.Empty<AccessedHost>();
      try
      {
        accessedHostList = service.GetMostRecentlyAccessedHosts(requestContext, userIdentity.SubjectDescriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(665544, "UrlRedirection", nameof (PortalAndTfsRedirectHelper), ex);
      }
      if (accessedHostList.Count > 0)
      {
        Dictionary<Guid, Microsoft.VisualStudio.Services.Account.Account> dictionary = source1.ToDictionary<Microsoft.VisualStudio.Services.Account.Account, Guid>((Func<Microsoft.VisualStudio.Services.Account.Account, Guid>) (o => o.AccountId));
        foreach (AccessedHost accessedHost in (IEnumerable<AccessedHost>) accessedHostList)
        {
          Microsoft.VisualStudio.Services.Account.Account organizationForNavigation;
          if (dictionary.TryGetValue(accessedHost.HostId, out organizationForNavigation))
            return organizationForNavigation;
        }
      }
      HostAuthenticationToken hostAuthToken = HostAuthenticationCookie.GetHostAuthenticationToken(requestContext);
      if (hostAuthToken != null)
      {
        IEnumerable<Microsoft.VisualStudio.Services.Account.Account> source2 = source1.Where<Microsoft.VisualStudio.Services.Account.Account>((Func<Microsoft.VisualStudio.Services.Account.Account, bool>) (x => hostAuthToken.IsHostAuthenticated(x.AccountId)));
        if (source2.Any<Microsoft.VisualStudio.Services.Account.Account>())
          source1 = source2;
      }
      return source1.OrderBy<Microsoft.VisualStudio.Services.Account.Account, string>((Func<Microsoft.VisualStudio.Services.Account.Account, string>) (o => o.AccountName), (IComparer<string>) StringComparer.OrdinalIgnoreCase).Last<Microsoft.VisualStudio.Services.Account.Account>();
    }

    private static string GetRedirectUrl(
      IVssRequestContext requestContext,
      Guid instanceType,
      string path)
    {
      return TFCommonUtil.CombinePaths(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, instanceType, AccessMappingConstants.PublicAccessMappingMoniker), path);
    }
  }
}
