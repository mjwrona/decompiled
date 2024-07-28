// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityDescriptorExtensions
  {
    private const int c_PuidLength = 16;
    private static readonly int MsaDescriptorIdentifierLength = 16 + "@Live.com".Length;

    public static SubjectDescriptor ToSubjectDescriptor(
      this IdentityDescriptor identityDescriptor,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (identityDescriptor == (IdentityDescriptor) null)
        return new SubjectDescriptor();
      return identityDescriptor.IsCuidBased() ? requestContext.GetService<IGraphIdentifierConversionService>().GetCuidBasedDescriptorByLegacyDescriptor(requestContext, identityDescriptor) : new SubjectDescriptor(identityDescriptor.GetSubjectTypeForNonCuidBasedIdentity(requestContext), identityDescriptor.Identifier);
    }

    internal static bool IsCuidBased(this IdentityDescriptor identityDescriptor)
    {
      if (!(identityDescriptor != (IdentityDescriptor) null))
        return false;
      return identityDescriptor.IsCspPartnerIdentityType() || identityDescriptor.IsMsaUserType() || identityDescriptor.IsAadUserType() || identityDescriptor.IsAadServicePrincipalType();
    }

    internal static bool IsMsaUserType(this IdentityDescriptor identityDescriptor) => identityDescriptor != (IdentityDescriptor) null && identityDescriptor.IsClaimsIdentityType() && !identityDescriptor.Identifier.Contains("\\") && identityDescriptor.Identifier.Length == IdentityDescriptorExtensions.MsaDescriptorIdentifierLength && identityDescriptor.Identifier.EndsWith("@Live.com", StringComparison.OrdinalIgnoreCase);

    internal static bool IsAadUserType(this IdentityDescriptor identityDescriptor) => identityDescriptor != (IdentityDescriptor) null && identityDescriptor.IsClaimsIdentityType() && identityDescriptor.Identifier.Contains("\\") && identityDescriptor.Identifier.Contains("@") && AadIdentityHelper.IsAadUser(identityDescriptor);
  }
}
