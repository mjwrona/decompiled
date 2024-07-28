// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityAuthenticationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityAuthenticationHelper
  {
    private const string BindPendingEmailAddressPrefix = "email:";

    public static Microsoft.VisualStudio.Services.Identity.Identity BuildTemporaryIdentityFromEmailAddress(
      string emailAddress)
    {
      return IdentityAuthenticationHelper.BuildTemporaryIdentityFromEmailAddress(emailAddress, emailAddress);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity BuildTemporaryIdentityFromEmailAddress(
      string emailAddress,
      string fullName)
    {
      IdentityDescriptor identityDescriptor = IdentityAuthenticationHelper.BuildTemporaryDescriptorFromEmailAddress(emailAddress);
      Microsoft.VisualStudio.Services.Identity.Identity identity = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity.Descriptor = identityDescriptor;
      identity.ProviderDisplayName = string.IsNullOrEmpty(fullName) ? emailAddress : fullName;
      identity.IsActive = true;
      identity.Members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity.SetProperty("Mail", (object) emailAddress);
      identity.SetProperty("Account", (object) emailAddress);
      return identity;
    }

    public static IdentityDescriptor BuildTemporaryDescriptorFromEmailAddress(string emailAddress)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(emailAddress, nameof (emailAddress));
      ArgumentUtility.CheckStringForAnyWhiteSpace(emailAddress, nameof (emailAddress));
      return new IdentityDescriptor("Microsoft.TeamFoundation.BindPendingIdentity", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) "email:", (object) emailAddress.ToLowerInvariant()));
    }

    public static bool IsTemporaryDescriptor(IdentityDescriptor descriptor) => !string.IsNullOrEmpty(descriptor?.Identifier) && descriptor.IsBindPendingType() && descriptor.Identifier.StartsWith("email:", StringComparison.OrdinalIgnoreCase);

    public static string RetrieveEmailAddressFromTemporaryDescriptor(IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(descriptor, nameof (descriptor));
      ArgumentUtility.CheckStringForNullOrEmpty(descriptor.Identifier, "descriptor.Identifier");
      ArgumentUtility.CheckStringForAnyWhiteSpace(descriptor.Identifier, "descriptor.Identifier");
      if (!VssStringComparer.IdentityDescriptor.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.BindPendingIdentity"))
        return (string) null;
      if (!VssStringComparer.IdentityDescriptor.StartsWith(descriptor.Identifier, "email:") || descriptor.Identifier.Length <= "email:".Length)
        throw new InvalidBindPendingIdentityDescriptorException(FrameworkResources.InvalidBindPendingIdentityDescriptorError((object) descriptor.Identifier));
      string emailAddress = descriptor.Identifier.Substring("email:".Length);
      return ArgumentUtility.IsValidEmailAddress(emailAddress) ? emailAddress : throw new InvalidBindPendingIdentityDescriptorException(FrameworkResources.InvalidBindPendingIdentityEmailAddressError((object) descriptor.Identifier));
    }
  }
}
