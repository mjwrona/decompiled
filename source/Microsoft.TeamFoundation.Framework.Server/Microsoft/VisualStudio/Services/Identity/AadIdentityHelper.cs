// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AadIdentityHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class AadIdentityHelper
  {
    private static readonly char[] DomainSeparator = new char[1]
    {
      '\\'
    };

    public static bool IsAadUser(IdentityDescriptor descriptor) => AadIdentityHelper.IsAadUser(descriptor, out Guid _);

    internal static bool IsAadUser(IdentityDescriptor descriptor, out Guid tenantId)
    {
      tenantId = AadIdentityHelper.GetIdentityTenantId(descriptor);
      return AadIdentityHelper.IsAadTenant(tenantId);
    }

    public static bool IsAadUser(IReadOnlyVssIdentity identity) => AadIdentityHelper.IsAadTenant(AadIdentityHelper.ExtractTenantId(identity));

    public static bool IsAadTenant(Guid tenantId) => tenantId != Guid.Empty;

    public static Guid GetIdentityTenantId(IdentityDescriptor descriptor)
    {
      if ((object) descriptor == null || descriptor.Identifier == null)
        return Guid.Empty;
      string[] strArray = (IdentityHelper.IsValidBindPendingDescriptor(descriptor) ? descriptor.Identifier.Substring("upn:".Length) : descriptor.Identifier).Split(AadIdentityHelper.DomainSeparator);
      if (strArray.Length != 2)
        return Guid.Empty;
      Guid result;
      Guid.TryParse(strArray[0], out result);
      return result;
    }

    public static string GetIdentityUpn(IdentityDescriptor descriptor)
    {
      if ((object) descriptor == null || descriptor.Identifier == null)
        return (string) null;
      string[] strArray = (IdentityHelper.IsValidBindPendingDescriptor(descriptor) ? descriptor.Identifier.Substring("upn:".Length) : descriptor.Identifier).Split(AadIdentityHelper.DomainSeparator);
      return strArray.Length != 2 ? (string) null : strArray[1];
    }

    internal static bool IsTfsGroup(IdentityDescriptor descriptor) => string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) && descriptor.Identifier.StartsWith(SidIdentityHelper.TeamFoundationSidPrefix, StringComparison.OrdinalIgnoreCase);

    public static bool IsAadGroup(IdentityDescriptor descriptor) => (object) descriptor != null && descriptor.Identifier != null && AadIdentityHelper.IsTfsGroup(descriptor) && SidIdentityHelper.IsAadGroupSid(new SecurityIdentifierInfo(new SecurityIdentifier(descriptor.Identifier)).GetBinaryForm());

    public static bool IsAadGroupNotDeleted(IReadOnlyVssIdentity identity) => identity != null && AadIdentityHelper.IsAadGroup(identity.Descriptor) && !identity.GetProperty<bool>("IsGroupDeleted", false);

    public static bool IsAadDirectoryRole(IReadOnlyVssIdentity identity) => identity != null && AadIdentityHelper.IsAadGroup(identity.Descriptor) && identity.GetProperty<SpecialGroupType>("SpecialType", SpecialGroupType.Generic) == SpecialGroupType.AzureActiveDirectoryRole;

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetAadGroups(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      return identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (g => AadIdentityHelper.IsAadGroup(g?.Descriptor)));
    }

    public static SubjectDescriptor GetAadUserSubjectDescriptor(Guid tenantId, Guid objectId)
    {
      ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForEmptyGuid(objectId, nameof (objectId));
      return new SubjectDescriptor("aad", IdentityCuidHelper.GenerateCuidByTenantIdOid(tenantId, objectId).ToString());
    }

    public static SubjectDescriptor GetAadGroupSubjectDescriptor(Guid objectId)
    {
      ArgumentUtility.CheckForEmptyGuid(objectId, nameof (objectId));
      return new SubjectDescriptor("aadgp", SidIdentityHelper.ConstructAadGroupSid(objectId).ToString());
    }

    internal static Guid ExtractAadGroupId(IdentityDescriptor aadGroupDescriptor) => SidIdentityHelper.DecodeAadSid(new SecurityIdentifierInfo(new SecurityIdentifier(aadGroupDescriptor.Identifier)));

    internal static bool TryExtractObjectId(IReadOnlyVssIdentity aadIdentity, out Guid objectId)
    {
      objectId = AadIdentityHelper.ExtractObjectId(aadIdentity);
      return !objectId.Equals(Guid.Empty);
    }

    internal static Guid ExtractObjectId(IReadOnlyVssIdentity aadIdentity)
    {
      switch (aadIdentity.GetProperty<object>("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) null))
      {
        case null:
          return Guid.Empty;
        case Guid objectId:
          return objectId;
        case string input:
          Guid result;
          return !Guid.TryParse(input, out result) ? Guid.Empty : result;
        default:
          return Guid.Empty;
      }
    }

    internal static Guid ExtractTenantId(IReadOnlyVssIdentity userIdentity)
    {
      Guid result;
      return userIdentity == null || !Guid.TryParse(userIdentity.GetProperty<string>("Domain", (string) null), out result) ? Guid.Empty : result;
    }

    internal static bool IsInvitationPending(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity identity)
    {
      IAadInvitationCheckHandler extension = requestContext.GetExtension<IAadInvitationCheckHandler>(ExtensionLifetime.Service);
      if (extension == null)
        return false;
      Guid objectId = AadIdentityHelper.ExtractObjectId(identity);
      return !(objectId == Guid.Empty) && extension.IsInvitationPending(requestContext, objectId);
    }

    internal static string GetTenantName(IVssRequestContext requestContext) => requestContext.GetExtension<IAadTenantDetailProvider>(ExtensionLifetime.Service)?.GetDisplayName(requestContext);
  }
}
