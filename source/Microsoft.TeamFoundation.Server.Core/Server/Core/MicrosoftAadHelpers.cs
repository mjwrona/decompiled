// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.MicrosoftAadHelpers
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class MicrosoftAadHelpers
  {
    public static bool IsMicrosoftTenant(IVssRequestContext requestContext)
    {
      if (!MicrosoftAadHelpers.IsRequestingIdentityExternal(requestContext))
        return false;
      Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
      if (organizationAadTenantId == Guid.Empty)
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string g = vssRequestContext.GetService<CachedRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/MsTenantId", false, (string) null);
      return g != null && organizationAadTenantId == new Guid(g);
    }

    public static void AddMicrosoftToProjectReadersGroup(
      IVssRequestContext tfsRequestContext,
      string projectName,
      out List<string> errors)
    {
      tfsRequestContext.CheckProjectCollectionRequestContext();
      errors = new List<string>();
      try
      {
        TeamFoundationIdentity readersGroupIdentity = MicrosoftAadHelpers.GetProjectReadersGroupIdentity(tfsRequestContext, projectName);
        GroupHelpers.CheckManageGroupMembershipPermission(tfsRequestContext, readersGroupIdentity);
        TeamFoundationIdentity foundationIdentity = MicrosoftAadHelpers.EnsureMicrosoftIdentityExists(tfsRequestContext.Elevate());
        if (readersGroupIdentity == null || foundationIdentity == null)
          return;
        tfsRequestContext.GetService<TeamFoundationIdentityService>().AddMemberToApplicationGroup(tfsRequestContext, readersGroupIdentity.Descriptor, foundationIdentity.Descriptor);
      }
      catch (AccessCheckException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        errors.Add(Resources.AddAADGroupError());
      }
    }

    public static void RemoveMicrosoftFromProjectReadersGroup(
      IVssRequestContext tfsRequestContext,
      string projectName,
      out List<string> errors)
    {
      tfsRequestContext.CheckProjectCollectionRequestContext();
      errors = new List<string>();
      try
      {
        TeamFoundationIdentity readersGroupIdentity = MicrosoftAadHelpers.GetProjectReadersGroupIdentity(tfsRequestContext, projectName);
        if (readersGroupIdentity == null)
          return;
        GroupHelpers.CheckManageGroupMembershipPermission(tfsRequestContext, readersGroupIdentity);
        TeamFoundationIdentityService service = tfsRequestContext.GetService<TeamFoundationIdentityService>();
        IdentityDescriptor microsoftGroupDescriptor1 = MicrosoftAadHelpers.GetMicrosoftGroupDescriptor(tfsRequestContext);
        if (service.IsMember(tfsRequestContext, readersGroupIdentity.Descriptor, microsoftGroupDescriptor1))
          service.RemoveMemberFromApplicationGroup(tfsRequestContext, readersGroupIdentity.Descriptor, microsoftGroupDescriptor1);
        IdentityDescriptor microsoftGroupDescriptor2 = MicrosoftAadHelpers.GetLegacyMicrosoftGroupDescriptor(tfsRequestContext);
        if (!(microsoftGroupDescriptor2 != (IdentityDescriptor) null) || IdentityDescriptorComparer.Instance.Equals(microsoftGroupDescriptor2, microsoftGroupDescriptor1) || !service.IsMember(tfsRequestContext, readersGroupIdentity.Descriptor, microsoftGroupDescriptor2))
          return;
        service.RemoveMemberFromApplicationGroup(tfsRequestContext, readersGroupIdentity.Descriptor, microsoftGroupDescriptor2);
      }
      catch (AccessCheckException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        errors.Add(Resources.RemoveAADGroupError());
      }
    }

    public static bool IsMicrosoftInReadersGroup(
      IVssRequestContext tfsRequestContext,
      string projectName)
    {
      tfsRequestContext.CheckProjectCollectionRequestContext();
      TeamFoundationIdentity microsoftGroup = MicrosoftAadHelpers.GetMicrosoftGroup(tfsRequestContext);
      if (microsoftGroup == null)
        return false;
      TeamFoundationIdentity readersGroupIdentity = MicrosoftAadHelpers.GetProjectReadersGroupIdentity(tfsRequestContext, projectName);
      return readersGroupIdentity != null && tfsRequestContext.GetService<TeamFoundationIdentityService>().IsMember(tfsRequestContext, readersGroupIdentity.Descriptor, microsoftGroup.Descriptor);
    }

    internal static TeamFoundationIdentity GetMicrosoftGroup(IVssRequestContext tfsRequestContext)
    {
      Guid msAadGroupId = MicrosoftAadHelpers.GetMsAadGroupId(tfsRequestContext);
      IVssRequestContext context = tfsRequestContext.To(TeamFoundationHostType.Application);
      TeamFoundationIdentityService service = context.GetService<TeamFoundationIdentityService>();
      IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(msAadGroupId));
      IVssRequestContext requestContext = context;
      IdentityDescriptor[] descriptors = new IdentityDescriptor[1]
      {
        descriptorFromSid
      };
      return ((IEnumerable<TeamFoundationIdentity>) service.ReadIdentities(requestContext, descriptors)).FirstOrDefault<TeamFoundationIdentity>();
    }

    internal static IdentityDescriptor GetMicrosoftGroupDescriptor(
      IVssRequestContext tfsRequestContext)
    {
      return IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(MicrosoftAadHelpers.GetMsAadGroupId(tfsRequestContext)));
    }

    internal static IdentityDescriptor GetLegacyMicrosoftGroupDescriptor(
      IVssRequestContext tfsRequestContext)
    {
      Guid? legacyMsAadGroupId = MicrosoftAadHelpers.GetLegacyMsAadGroupId(tfsRequestContext);
      return !legacyMsAadGroupId.HasValue ? (IdentityDescriptor) null : IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(legacyMsAadGroupId.Value));
    }

    internal static TeamFoundationIdentity GetProjectReadersGroupIdentity(
      IVssRequestContext tfsRequestContext,
      string projectName)
    {
      tfsRequestContext.CheckProjectCollectionRequestContext();
      TeamFoundationIdentityService service = tfsRequestContext.GetService<TeamFoundationIdentityService>();
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]\\{1}", (object) projectName, (object) "Readers");
      IVssRequestContext requestContext = tfsRequestContext;
      string factorValue = str;
      return service.ReadIdentity(requestContext, IdentitySearchFactor.DisplayName, factorValue);
    }

    private static TeamFoundationIdentity EnsureMicrosoftIdentityExists(
      IVssRequestContext tfsRequestContext)
    {
      Guid[] aadGroupObjectIds = new Guid[1]
      {
        MicrosoftAadHelpers.GetMsAadGroupId(tfsRequestContext)
      };
      List<string> failureMessages = (List<string>) null;
      List<TeamFoundationIdentity> aadGroupsWithCreationFailure = (List<TeamFoundationIdentity>) null;
      IList<Guid> aadGroupsInIms = tfsRequestContext.GetExtension<IAadGroupHelper>(ExtensionLifetime.Service).CreateAadGroupsInIms(tfsRequestContext, aadGroupObjectIds, out aadGroupsWithCreationFailure, out failureMessages);
      if (aadGroupsInIms == null || !aadGroupsInIms.Any<Guid>())
        return (TeamFoundationIdentity) null;
      IVssRequestContext vssRequestContext = tfsRequestContext.To(TeamFoundationHostType.Application);
      return ((IEnumerable<TeamFoundationIdentity>) vssRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(vssRequestContext, aadGroupsInIms.ToArray<Guid>())).FirstOrDefault<TeamFoundationIdentity>();
    }

    private static Guid GetMsAadGroupId(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new Guid(vssRequestContext.GetService<CachedRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/MsAadGroupId", false, (string) null));
    }

    private static Guid? GetLegacyMsAadGroupId(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string g = vssRequestContext.GetService<CachedRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/LegacyMsAadGroupId", false, (string) null);
      return !string.IsNullOrEmpty(g) ? new Guid?(new Guid(g)) : new Guid?();
    }

    private static bool IsRequestingIdentityExternal(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity != null && userIdentity.IsExternalUser;
    }
  }
}
