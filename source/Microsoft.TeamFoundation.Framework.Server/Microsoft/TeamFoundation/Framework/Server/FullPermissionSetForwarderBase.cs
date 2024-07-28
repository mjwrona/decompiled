// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FullPermissionSetForwarderBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class FullPermissionSetForwarderBase
  {
    public virtual bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      IEnumerable<string> sourceTokens,
      bool recurse)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      List<string> tokens = new List<string>();
      foreach (string sourceToken in sourceTokens)
      {
        string tokenFromSourceToken = this.GetTargetTokenFromSourceToken(requestContext, sourceToken);
        if (tokenFromSourceToken != null)
          tokens.Add(tokenFromSourceToken);
      }
      using (IVssRequestContext targetRequestContext = this.GetTargetRequestContext(requestContext))
        this.GetTargetNamespace(targetRequestContext).RemoveAccessControlLists(targetRequestContext, (IEnumerable<string>) tokens, false);
      return true;
    }

    public virtual bool RemovePermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string sourceToken,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      string tokenFromSourceToken = this.GetTargetTokenFromSourceToken(requestContext, sourceToken);
      if (tokenFromSourceToken != null)
      {
        int sourcePermissions = this.GetTargetPermissionsFromSourcePermissions(requestContext, permissionsToRemove);
        if (sourcePermissions != 0)
        {
          using (IVssRequestContext targetRequestContext = this.GetTargetRequestContext(requestContext))
            this.GetTargetNamespace(targetRequestContext).RemovePermissions(targetRequestContext, tokenFromSourceToken, this.GetContextQualifiedDescriptor(requestContext, descriptor), sourcePermissions);
        }
      }
      return true;
    }

    public virtual bool RemovePermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string sourceToken,
      IEnumerable<IdentityDescriptor> identities)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      string tokenFromSourceToken = this.GetTargetTokenFromSourceToken(requestContext, sourceToken);
      if (tokenFromSourceToken != null)
      {
        List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>();
        foreach (IdentityDescriptor identity in identities)
          descriptors.Add(this.GetContextQualifiedDescriptor(requestContext, identity));
        using (IVssRequestContext targetRequestContext = this.GetTargetRequestContext(requestContext))
          this.GetTargetNamespace(targetRequestContext).RemoveAccessControlEntries(targetRequestContext, tokenFromSourceToken, (IEnumerable<IdentityDescriptor>) descriptors);
      }
      return true;
    }

    public virtual bool RenameToken(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string existingToken,
      string newToken,
      bool copy)
    {
      return true;
    }

    public virtual bool SetAccessControlLists(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      IEnumerable<IAccessControlList> sourceAccessControlLists,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      using (IVssRequestContext targetRequestContext = this.GetTargetRequestContext(requestContext))
      {
        List<IAccessControlList> accessControlListList = new List<IAccessControlList>();
        foreach (IAccessControlList accessControlList1 in sourceAccessControlLists)
        {
          string tokenFromSourceToken = this.GetTargetTokenFromSourceToken(requestContext, accessControlList1.Token);
          if (tokenFromSourceToken != null)
          {
            IAccessControlList accessControlList2 = this.GetTargetNamespace(targetRequestContext).QueryAccessControlList(targetRequestContext, tokenFromSourceToken, (IEnumerable<IdentityDescriptor>) null, false);
            AccessControlList acl = new AccessControlList(tokenFromSourceToken, accessControlList2.InheritPermissions);
            foreach (IAccessControlEntry accessControlEntry in accessControlList1.AccessControlEntries)
            {
              int sourcePermissions1 = this.GetTargetPermissionsFromSourcePermissions(requestContext, accessControlEntry.Allow);
              int sourcePermissions2 = this.GetTargetPermissionsFromSourcePermissions(requestContext, accessControlEntry.Deny);
              if (sourcePermissions1 != 0 || sourcePermissions2 != 0)
                acl.SetPermissions(this.GetContextQualifiedDescriptor(requestContext, accessControlEntry.Descriptor), sourcePermissions1, sourcePermissions2, true);
            }
            accessControlListList.Add((IAccessControlList) acl);
          }
        }
        this.GetTargetNamespace(targetRequestContext).SetAccessControlLists(targetRequestContext, (IEnumerable<IAccessControlList>) accessControlListList, throwOnInvalidIdentity, rootNewIdentities);
        return true;
      }
    }

    public virtual bool SetInheritFlag(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      bool inherit)
    {
      return true;
    }

    public virtual bool SetPermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string sourceToken,
      IEnumerable<IAccessControlEntry> sourceAces,
      bool merge,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities)
    {
      if (notificationType != NotificationType.DecisionPoint)
        return true;
      string tokenFromSourceToken = this.GetTargetTokenFromSourceToken(requestContext, sourceToken);
      if (tokenFromSourceToken != null)
      {
        List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>();
        foreach (IAccessControlEntry sourceAce in sourceAces)
        {
          int sourcePermissions1 = this.GetTargetPermissionsFromSourcePermissions(requestContext, sourceAce.Allow);
          int sourcePermissions2 = this.GetTargetPermissionsFromSourcePermissions(requestContext, sourceAce.Deny);
          accessControlEntryList.Add((IAccessControlEntry) new AccessControlEntry(this.GetContextQualifiedDescriptor(requestContext, sourceAce.Descriptor), sourcePermissions1, sourcePermissions2));
        }
        using (IVssRequestContext targetRequestContext = this.GetTargetRequestContext(requestContext))
          this.GetTargetNamespace(targetRequestContext).SetAccessControlEntries(targetRequestContext, tokenFromSourceToken, (IEnumerable<IAccessControlEntry>) accessControlEntryList, merge, throwOnInvalidIdentity, rootNewIdentities);
      }
      return true;
    }

    private IVssRequestContext GetTargetRequestContext(IVssRequestContext requestContext)
    {
      IVssServiceHost targetServiceHost = this.GetTargetServiceHost(requestContext);
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      RequestContextType requestContextType = !(targetServiceHost.InstanceId == requestContext.ServiceHost.InstanceId) ? (targetServiceHost.HostType != TeamFoundationHostType.Application || requestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection || targetServiceHost.Status != TeamFoundationServiceHostStatus.Stopped || requestContext.ServiceHost.Status != TeamFoundationServiceHostStatus.Stopped || !requestContext.IsServicingContext ? RequestContextType.SystemContext : RequestContextType.ServicingContext) : (!requestContext.IsServicingContext ? (!requestContext.IsSystemContext ? RequestContextType.UserContext : RequestContextType.SystemContext) : RequestContextType.ServicingContext);
      bool flag = requestContextType != 0;
      IVssRequestContext requestContext1 = requestContext;
      Guid instanceId = targetServiceHost.InstanceId;
      int contextType = (int) requestContextType;
      int num = flag ? 1 : 0;
      return service.BeginRequest(requestContext1, instanceId, (RequestContextType) contextType, true, num != 0);
    }

    private IVssSecurityNamespace GetTargetNamespace(IVssRequestContext targetRequestContext) => targetRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(targetRequestContext, this.GetTargetNamespaceId());

    protected abstract Guid GetTargetNamespaceId();

    protected abstract IVssServiceHost GetTargetServiceHost(IVssRequestContext requestContext);

    protected abstract string GetTargetTokenFromSourceToken(
      IVssRequestContext requestContext,
      string securityToken);

    protected abstract int GetTargetPermissionsFromSourcePermissions(
      IVssRequestContext requestContext,
      int sourcePermissions);

    private IdentityDescriptor GetContextQualifiedDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      if (!VssStringComparer.SID.StartsWith(descriptor.Identifier, SidIdentityHelper.WellKnownSidPrefix))
        return descriptor;
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null)[0].Descriptor;
    }
  }
}
