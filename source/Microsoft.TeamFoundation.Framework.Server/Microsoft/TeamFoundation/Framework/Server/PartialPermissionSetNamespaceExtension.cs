// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PartialPermissionSetNamespaceExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class PartialPermissionSetNamespaceExtension : DefaultSecurityNamespaceExtension
  {
    public abstract IVssSecurityNamespace GetTargetNamespace(IVssRequestContext requestContext);

    public abstract int SourcePermissionsForwarded { get; }

    public abstract int DetermineTargetPermissions(
      IVssSecurityNamespace securityNamespace,
      int sourcePermissions);

    public abstract int DetermineSourcePermissions(
      IVssSecurityNamespace securityNamespace,
      int targetPermissions);

    public abstract string DetermineTargetToken(
      IVssSecurityNamespace securityNamespace,
      string sourceToken);

    public abstract string DetermineSourceToken(
      IVssSecurityNamespace securityNamespace,
      string targetToken);

    public override bool HasPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      int effectiveAllows,
      int effectiveDenys,
      bool preliminaryDecision)
    {
      string targetToken = this.DetermineTargetToken(securityNamespace, token);
      if (preliminaryDecision || targetToken == null)
        return preliminaryDecision;
      IVssSecurityNamespace targetNamespace = this.GetTargetNamespace(requestContext);
      if ((requestedPermissions & this.SourcePermissionsForwarded) != 0)
      {
        int targetPermissions = this.DetermineTargetPermissions(securityNamespace, requestedPermissions);
        if (!targetNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermission(requestContext, principal, targetToken, targetPermissions, false))
          return false;
        requestedPermissions &= ~this.SourcePermissionsForwarded;
      }
      return (requestedPermissions & effectiveAllows) == requestedPermissions;
    }

    public override IEnumerable<QueriedAccessControlList> QueryPermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse,
      IEnumerable<QueriedAccessControlList> preliminaryAcls)
    {
      string token1;
      if (token == null)
        token1 = (string) null;
      else if ((token1 = this.DetermineTargetToken(securityNamespace, token)) == null)
        return preliminaryAcls;
      IVssSecurityNamespace targetNamespace = this.GetTargetNamespace(requestContext);
      if (targetNamespace == null)
        return preliminaryAcls;
      Dictionary<string, QueriedAccessControlList> dictionary = targetNamespace.QueryAccessControlLists(requestContext, token1, evaluationPrincipals, includeExtendedInfo, recurse).ToDictionary<QueriedAccessControlList, string>((Func<QueriedAccessControlList, string>) (s => s.Token), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<QueriedAccessControlList> accessControlListList = new List<QueriedAccessControlList>();
      foreach (QueriedAccessControlList preliminaryAcl in preliminaryAcls)
      {
        string targetToken = this.DetermineTargetToken(securityNamespace, preliminaryAcl.Token);
        QueriedAccessControlList targetACL;
        if (targetToken != null && dictionary.TryGetValue(targetToken, out targetACL))
        {
          this.CombineSourceACLAndTargetACL(securityNamespace, preliminaryAcl, targetACL, includeExtendedInfo);
          dictionary.Remove(targetToken);
        }
        accessControlListList.Add(preliminaryAcl);
      }
      foreach (QueriedAccessControlList targetACL in dictionary.Values)
      {
        string sourceToken = this.DetermineSourceToken(securityNamespace, targetACL.Token);
        if (sourceToken != null)
        {
          QueriedAccessControlList sourceACL = new QueriedAccessControlList(sourceToken, targetACL.Inherit, (IList<QueriedAccessControlEntry>) new List<QueriedAccessControlEntry>());
          this.CombineSourceACLAndTargetACL(securityNamespace, sourceACL, targetACL, includeExtendedInfo);
          accessControlListList.Add(sourceACL);
        }
      }
      return (IEnumerable<QueriedAccessControlList>) accessControlListList;
    }

    public override int QueryEffectivePermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      EvaluationPrincipal principal,
      int preliminaryEffectivePermissions)
    {
      string targetToken = this.DetermineTargetToken(securityNamespace, token);
      if (targetToken != null)
      {
        IVssSecurityNamespace targetNamespace = this.GetTargetNamespace(requestContext);
        preliminaryEffectivePermissions |= this.DetermineSourcePermissions(securityNamespace, targetNamespace.QueryEffectivePermissions(requestContext, targetToken, principal));
      }
      return preliminaryEffectivePermissions;
    }

    private void CombineSourceACLAndTargetACL(
      IVssSecurityNamespace securityNamespace,
      QueriedAccessControlList sourceACL,
      QueriedAccessControlList targetACL,
      bool includeExtendedInfo)
    {
      Dictionary<IdentityDescriptor, QueriedAccessControlEntry> dictionary = targetACL.AccessControlEntries.ToDictionary<QueriedAccessControlEntry, IdentityDescriptor>((Func<QueriedAccessControlEntry, IdentityDescriptor>) (s => s.IdentityDescriptor), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      for (int index = 0; index < sourceACL.AccessControlEntries.Count; ++index)
      {
        QueriedAccessControlEntry accessControlEntry1 = sourceACL.AccessControlEntries[index];
        QueriedAccessControlEntry accessControlEntry2;
        if (!dictionary.TryGetValue(accessControlEntry1.IdentityDescriptor, out accessControlEntry2))
          accessControlEntry2 = new QueriedAccessControlEntry(accessControlEntry1.IdentityDescriptor, 0, 0);
        int allow = accessControlEntry1.Allow | this.DetermineSourcePermissions(securityNamespace, accessControlEntry2.Allow);
        int deny = accessControlEntry1.Deny | this.DetermineSourcePermissions(securityNamespace, accessControlEntry2.Deny);
        int inheritedAllow = 0;
        int inheritedDeny = 0;
        int effectiveAllow = 0;
        int effectiveDeny = 0;
        if (includeExtendedInfo)
        {
          inheritedAllow = accessControlEntry1.InheritedAllow | this.DetermineSourcePermissions(securityNamespace, accessControlEntry2.InheritedAllow);
          inheritedDeny = accessControlEntry1.InheritedDeny | this.DetermineSourcePermissions(securityNamespace, accessControlEntry2.InheritedDeny);
          effectiveAllow = accessControlEntry1.EffectiveAllow | this.DetermineSourcePermissions(securityNamespace, accessControlEntry2.EffectiveAllow);
          effectiveDeny = accessControlEntry1.EffectiveDeny | this.DetermineSourcePermissions(securityNamespace, accessControlEntry2.EffectiveDeny);
        }
        sourceACL.AccessControlEntries[index] = new QueriedAccessControlEntry(accessControlEntry1.IdentityDescriptor, allow, deny, inheritedAllow, inheritedDeny, effectiveAllow, effectiveDeny);
        dictionary.Remove(accessControlEntry2.IdentityDescriptor);
      }
      foreach (QueriedAccessControlEntry accessControlEntry in dictionary.Values)
      {
        int sourcePermissions1 = this.DetermineSourcePermissions(securityNamespace, accessControlEntry.Allow);
        int sourcePermissions2 = this.DetermineSourcePermissions(securityNamespace, accessControlEntry.Deny);
        if (sourcePermissions1 != 0 || sourcePermissions2 != 0)
        {
          int inheritedAllow = 0;
          int inheritedDeny = 0;
          int effectiveAllow = 0;
          int effectiveDeny = 0;
          if (includeExtendedInfo)
          {
            inheritedAllow = this.DetermineSourcePermissions(securityNamespace, accessControlEntry.InheritedAllow);
            inheritedDeny = this.DetermineSourcePermissions(securityNamespace, accessControlEntry.InheritedDeny);
            effectiveAllow = this.DetermineSourcePermissions(securityNamespace, accessControlEntry.EffectiveAllow);
            effectiveDeny = this.DetermineSourcePermissions(securityNamespace, accessControlEntry.EffectiveDeny);
          }
          sourceACL.AccessControlEntries.Add(new QueriedAccessControlEntry(accessControlEntry.IdentityDescriptor, sourcePermissions1, sourcePermissions2, inheritedAllow, inheritedDeny, effectiveAllow, effectiveDeny));
        }
      }
    }
  }
}
