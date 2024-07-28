// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.IdentityHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class IdentityHelpers
  {
    public static bool AddMemberToGroup(
      IVssRequestContext requestContext,
      TeamFoundationIdentity group,
      TeamFoundationIdentity member,
      TeamFoundationIdentity addedIdentity,
      MembershipModel model)
    {
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(group, nameof (group));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(member, nameof (member));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(addedIdentity, nameof (addedIdentity));
      ArgumentUtility.CheckForNull<MembershipModel>(model, nameof (model));
      try
      {
        if (IdentityHelper.IsEveryoneGroup(member.Descriptor))
          throw new IdentityServiceException(string.Format(AdminServerResources.MembershipForEveryoneGroup, (object) member.DisplayName));
        requestContext.GetService<TeamFoundationIdentityService>().AddMemberToApplicationGroup(requestContext, group.Descriptor, member);
        model.AddedIdentities.Add(IdentityManagementHelpers.GetIdentityViewModel(addedIdentity));
        return true;
      }
      catch (AddMemberIdentityAlreadyMemberException ex)
      {
        requestContext.TraceException(599999, nameof (IdentityHelpers), nameof (AddMemberToGroup), (Exception) ex);
      }
      catch (IdentityServiceException ex)
      {
        IdentityViewModelBase identityViewModel = IdentityManagementHelpers.GetIdentityViewModel(addedIdentity);
        identityViewModel.Errors.Add(ex.Message);
        model.FailedAddedIdentities.Add(identityViewModel);
        requestContext.TraceException(599999, nameof (IdentityHelpers), nameof (AddMemberToGroup), (Exception) ex);
      }
      return false;
    }

    public static void RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      TeamFoundationIdentity group,
      TeamFoundationIdentity member,
      TeamFoundationIdentity removedIdentity,
      MembershipModel model)
    {
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(group, nameof (group));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(member, nameof (member));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(removedIdentity, nameof (removedIdentity));
      ArgumentUtility.CheckForNull<MembershipModel>(model, nameof (model));
      try
      {
        requestContext.GetService<TeamFoundationIdentityService>().RemoveMemberFromApplicationGroup(requestContext, group.Descriptor, member.Descriptor);
        model.DeletedIdentities.Add(IdentityManagementHelpers.GetIdentityViewModel(removedIdentity));
      }
      catch (IdentityDomainMismatchException ex)
      {
        GroupIdentityViewModel identityViewModel = IdentityManagementHelpers.GetIdentityViewModel<GroupIdentityViewModel>(group);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          identityViewModel.Errors.Add(string.Format(AdminServerResources.IdentityDomainMismatch, (object) identityViewModel.FriendlyDisplayName, (object) identityViewModel.SubHeader));
        else
          identityViewModel.Errors.Add(AdminServerResources.RemoveUserFromGroupDomainMismatch);
        model.FailedDeletedIdentities.Add((IdentityViewModelBase) identityViewModel);
        requestContext.TraceException(599999, nameof (IdentityHelpers), nameof (RemoveMemberFromGroup), (Exception) ex);
      }
      catch (IdentityServiceException ex)
      {
        IdentityViewModelBase identityViewModel = IdentityManagementHelpers.GetIdentityViewModel(removedIdentity);
        identityViewModel.Errors.Add(ex.Message);
        model.FailedDeletedIdentities.Add(identityViewModel);
        requestContext.TraceException(599999, nameof (IdentityHelpers), nameof (RemoveMemberFromGroup), (Exception) ex);
      }
    }

    public static bool IsEndPointGroupDescriptor(
      IVssRequestContext requestContext,
      Guid scopeId,
      IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      IdentityDescriptor x = IdentityDomain.MapFromWellKnownIdentifier(scopeId, descriptor);
      IdentityDescriptor y1 = IdentityDomain.MapFromWellKnownIdentifier(scopeId, TaskWellKnownIdentityDescriptors.EndpointAdministratorsDescriptor);
      IdentityDescriptor y2 = IdentityDomain.MapFromWellKnownIdentifier(scopeId, TaskWellKnownIdentityDescriptors.EndpointCreatorsDescriptors);
      return IdentityDescriptorComparer.Instance.Equals(x, y1) || IdentityDescriptorComparer.Instance.Equals(x, y2);
    }
  }
}
