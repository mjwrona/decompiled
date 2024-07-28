// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  public static class UserHelper
  {
    private const string LegacyClaimsType = "lclm";
    private const string LegacyCspType = "lcsp";

    internal static bool IsPseudoSubjectDescriptor(this SubjectDescriptor subjectDescriptor) => StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "lclm") || StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "lcsp");

    internal static bool IsSupportedUserServiceType(this SubjectDescriptor subjectDescriptor) => subjectDescriptor.IsClaimsUserType() || subjectDescriptor.IsCspPartnerIdentityType();

    internal static SubjectDescriptor CreatePseudoSubjectDescriptor(
      IdentityDescriptor identityDescriptor)
    {
      if (identityDescriptor.IsClaimsIdentityType() || identityDescriptor.IsAadServicePrincipalType())
        return new SubjectDescriptor("lclm", identityDescriptor.Identifier);
      return identityDescriptor.IsCspPartnerIdentityType() ? new SubjectDescriptor("lcsp", identityDescriptor.Identifier) : throw new NotSupportedException("Unsupported identity type: " + identityDescriptor.IdentityType);
    }

    internal static IdentityDescriptor ParsePseudoSubjectDescriptor(
      SubjectDescriptor subjectDescriptor)
    {
      if (StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "lclm"))
        return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", subjectDescriptor.Identifier);
      if (StringComparer.OrdinalIgnoreCase.Equals(subjectDescriptor.SubjectType, "lcsp"))
        return new IdentityDescriptor("Microsoft.TeamFoundation.Claims.CspPartnerIdentity", subjectDescriptor.Identifier);
      throw new NotSupportedException("Unsupported subject type: " + subjectDescriptor.SubjectType);
    }

    internal static SubjectDescriptor CreateDescriptorForIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (requestContext.ServiceInstanceType() == InstanceManagementHelper.UserSvcPrincipal)
      {
        if (identity.Id == Guid.Empty)
          throw new ArgumentException("The identity was not correctly authenticated.");
        if (identity.GetProperty<Guid>("CUID", Guid.Empty) == Guid.Empty)
          throw new ArgumentException("The identity was not properly authenticated.");
      }
      if (!identity.SubjectDescriptor.IsClaimsUserType() && !identity.SubjectDescriptor.IsCspPartnerIdentityType())
        requestContext.TraceAlways(124042, TraceLevel.Warning, "User", nameof (UserHelper), "Unexpected SubjectDescriptor: {0}. IdentityDescriptor: {1}", (object) identity.SubjectDescriptor, (object) identity.Descriptor);
      SubjectDescriptor subjectDescriptor = identity.Descriptor.ToSubjectDescriptor(requestContext);
      return subjectDescriptor.IsClaimsUserType() || subjectDescriptor.IsCspPartnerIdentityType() ? subjectDescriptor : throw new ArgumentException("The identity is not a supported identity type.");
    }

    public static bool TranslateUserId(
      IVssRequestContext requestContext,
      ref Guid userId,
      out bool isOtherAccountUser)
    {
      isOtherAccountUser = false;
      bool flag = false;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        Guid userId1 = requestContext.GetUserId();
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          userId
        }, QueryMembership.None, (IEnumerable<string>) new string[1]
        {
          "UserId"
        })[0];
        if (readIdentity != null)
        {
          if (userId1 != userId)
            isOtherAccountUser = true;
          Guid property = readIdentity.GetProperty<Guid>("UserId", Guid.Empty);
          if (property != Guid.Empty)
          {
            if (userId == userId1 && property != userId)
              flag = true;
            userId = property;
          }
        }
      }
      return flag;
    }

    public static bool IsOtherAccountUser(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.GetUserIdentity().SubjectDescriptor != descriptor)
      {
        if (requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
        {
          descriptor
        }, QueryMembership.None, (IEnumerable<string>) null)[0] != null)
          return true;
      }
      return false;
    }

    public static IDisposable GetTemporaryUseDelegatedS2STokens(IVssRequestContext requestContext) => (IDisposable) new UserHelper.TemporaryUseDelegatedS2STokens(requestContext);

    private class TemporaryUseDelegatedS2STokens : IDisposable
    {
      private IVssRequestContext m_requestContext;

      internal TemporaryUseDelegatedS2STokens(IVssRequestContext requestContext)
      {
        this.m_requestContext = requestContext.RootContext;
        this.m_requestContext.Items[RequestContextItemsKeys.UseDelegatedS2STokens] = (object) true;
      }

      public void Dispose() => this.m_requestContext.Items.Remove(RequestContextItemsKeys.UseDelegatedS2STokens);
    }
  }
}
