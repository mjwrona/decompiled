// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Organization.OrganizationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Account.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Framework;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.Organization
{
  public static class OrganizationHelper
  {
    private const string c_accountUrlProperty = "Microsoft.VisualStudio.Services.Account.ServiceUrl.00025394-6065-48CA-87D9-7F5672854EF7";
    private static readonly string[] c_accountUrlProperties = new string[1]
    {
      "Microsoft.VisualStudio.Services.Account.ServiceUrl.00025394-6065-48CA-87D9-7F5672854EF7"
    };
    private static readonly string s_LogLegacyAndSubjectDescriptorDifferencesFlag = "MS.TF.Framework.Server.Organization.LogDescriptorDifferences";
    private static readonly string s_SubjectDescriptorStagedDeploymentFlag = "MS.TF.Framework.Server.Organization.ResolveAccountMappingsWithSubjectDescriptor";
    private static readonly string s_SubjectDescriptorStagedDeploymentRegistry = "AccountMappingsSubjectDescriptor";
    private static readonly string s_TraceArea = nameof (OrganizationHelper);

    public static IEnumerable<Microsoft.VisualStudio.Services.Account.Account> GetOrganizationsForRequestIdentity(
      IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext vssRequestContext = context.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity deploymentIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      bool flag1 = OrganizationHelper.IsLogDescriptorDifferencesFeatureEnabled(requestContext);
      bool flag2 = OrganizationHelper.IsResolveAccountMappingsWithSubjectDescriptorFeatureEnabled(requestContext);
      if (flag1)
      {
        try
        {
          IdentityDescriptor descriptor = userIdentity.Descriptor;
          deploymentIdentity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
          {
            descriptor
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          SubjectDescriptor subjectDescriptor = requestContext.UserSubjectDescriptor();
          Microsoft.VisualStudio.Services.Identity.Identity subjectDescriptorDeploymentIdentity = service.ReadIdentities(vssRequestContext, (IList<SubjectDescriptor>) new List<SubjectDescriptor>()
          {
            subjectDescriptor
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (deploymentIdentity == null || deploymentIdentity.Equals((object) subjectDescriptorDeploymentIdentity))
          {
            if (subjectDescriptorDeploymentIdentity != null)
            {
              if (subjectDescriptorDeploymentIdentity.Equals((object) deploymentIdentity))
                goto label_6;
            }
            else
              goto label_6;
          }
          requestContext.TraceAlways(54008, TraceLevel.Info, OrganizationHelper.s_TraceArea, nameof (GetOrganizationsForRequestIdentity), string.Format("Legacy descriptor {0} results do not match subject descriptor {1} results", (object) descriptor, (object) subjectDescriptor));
          requestContext.TraceConditionally(54009, TraceLevel.Info, OrganizationHelper.s_TraceArea, nameof (GetOrganizationsForRequestIdentity), (Func<string>) (() => OrganizationHelper.LogDifferenceInAccountsByMembership(deploymentIdentity, subjectDescriptorDeploymentIdentity, requestContext)));
        }
        catch (Exception ex)
        {
          requestContext.Trace(54010, TraceLevel.Error, OrganizationHelper.s_TraceArea, nameof (GetOrganizationsForRequestIdentity), string.Format("Unable to log differences between legacy and subject descriptor results. {0}", (object) ex));
        }
      }
label_6:
      if (flag2)
      {
        try
        {
          if (requestContext.GetService<IStagedDeploymentService>().AllowNewEntryThrough(requestContext, OrganizationHelper.s_SubjectDescriptorStagedDeploymentRegistry, requestContext.GetUserId()))
            deploymentIdentity = service.ReadIdentities(vssRequestContext, (IList<SubjectDescriptor>) new List<SubjectDescriptor>()
            {
              requestContext.UserSubjectDescriptor()
            }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          else
            deploymentIdentity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
            {
              userIdentity.Descriptor
            }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        catch (Exception ex)
        {
          requestContext.Trace(54011, TraceLevel.Error, OrganizationHelper.s_TraceArea, nameof (GetOrganizationsForRequestIdentity), string.Format("Unable to use subject descriptor to resolve account mappings. {0}", (object) ex));
          deploymentIdentity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
          {
            userIdentity.Descriptor
          }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
      }
      if (!flag1 && !flag2)
        deploymentIdentity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          userIdentity.Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return deploymentIdentity != null ? (IEnumerable<Microsoft.VisualStudio.Services.Account.Account>) context.GetClient<AccountHttpClient>(ServiceInstanceTypes.SPS).GetAccountsByMemberAsync(deploymentIdentity.Id, (IEnumerable<string>) OrganizationHelper.c_accountUrlProperties, (object) null, new CancellationToken()).SyncResult<List<Microsoft.VisualStudio.Services.Account.Account>>() : Enumerable.Empty<Microsoft.VisualStudio.Services.Account.Account>();
    }

    public static string GetOrganizationUrl(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Account.Account organization,
      bool forceDevOpsDomain = false)
    {
      string property = organization.GetProperty("Microsoft.VisualStudio.Services.Account.ServiceUrl.00025394-6065-48CA-87D9-7F5672854EF7") as string;
      if (string.IsNullOrEmpty(property) || !forceDevOpsDomain)
        return property;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string parentDomain = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in ConfigurationConstants.DevOpsRootDomain);
      if (UriUtility.IsSubdomainOf(new Uri(property).Host, parentDomain))
        return property;
      return new DevOpsCollectionHostUriData(organization.AccountName).BuildUri(vssRequestContext, new Guid(), true, true)?.AbsoluteUri;
    }

    public static bool IsLogDescriptorDifferencesFeatureEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(OrganizationHelper.s_LogLegacyAndSubjectDescriptorDifferencesFlag);

    public static bool IsResolveAccountMappingsWithSubjectDescriptorFeatureEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled(OrganizationHelper.s_SubjectDescriptorStagedDeploymentFlag);
    }

    public static string GetSubjectDescriptorStagedDeploymentRegistry() => OrganizationHelper.s_SubjectDescriptorStagedDeploymentRegistry;

    private static string LogDifferenceInAccountsByMembership(
      Microsoft.VisualStudio.Services.Identity.Identity legacyIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity subjectIdentity,
      IVssRequestContext requestContext)
    {
      AccountHttpClient client = requestContext.GetClient<AccountHttpClient>();
      List<Microsoft.VisualStudio.Services.Account.Account> result1 = client.GetAccountsByMemberAsync(legacyIdentity.Id, (IEnumerable<string>) null, (object) null, new CancellationToken()).Result;
      List<Microsoft.VisualStudio.Services.Account.Account> result2 = client.GetAccountsByMemberAsync(subjectIdentity.Id, (IEnumerable<string>) null, (object) null, new CancellationToken()).Result;
      int num1 = 0;
      int num2 = 0;
      if (result1 != null)
        num1 = result1.Count;
      if (result2 != null)
        num2 = result2.Count;
      return string.Format("Legacy descriptor identity maps to {0} accounts, subject descriptor identity maps to {1} accounts.", (object) num1, (object) num2);
    }
  }
}
