// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IdentityConstantsNormalizer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal static class IdentityConstantsNormalizer
  {
    private const string c_DomainTbd = "TBD";

    internal static bool CanSyncIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string identityType = identity.Descriptor.IdentityType;
      return VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.TeamFoundation.Identity") || VssStringComparer.IdentityDescriptor.Equals(identityType, "System.Security.Principal.WindowsIdentity") && !string.IsNullOrWhiteSpace(identity.GetProperty<string>("Domain", string.Empty)) || VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.IdentityModel.Claims.ClaimsIdentity") || VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.TeamFoundation.BindPendingIdentity") || VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.VisualStudio.Services.Identity.ServerTestIdentity") || VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.TeamFoundation.ServiceIdentity") || VssStringComparer.IdentityDescriptor.Equals(identityType, "System:ServicePrincipal") || VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal");
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity NormalizeIdentity(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string instanceId,
      string collectionId)
    {
      string str = IdentityConstantsNormalizer.IdentifySubstituteDomain(identity, instanceId, collectionId);
      if (str != null)
        identity.SetProperty("Domain", (object) str);
      if (string.IsNullOrWhiteSpace(identity.GetProperty<string>("Account", string.Empty)))
        identity.SetProperty("Account", (object) identity.DisplayName);
      return identity;
    }

    private static string IdentifySubstituteDomain(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string instanceId,
      string collectionId)
    {
      string property = identity.GetProperty<string>("Domain", string.Empty);
      string str = (string) null;
      if (VssStringComparer.IdentityDescriptor.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.Identity"))
      {
        if (string.IsNullOrWhiteSpace(property))
        {
          str = "488bb442-0beb-4c1e-98b6-4eddc604bd9e";
        }
        else
        {
          Uri result;
          if (!Guid.TryParse(property, out Guid _) && Uri.TryCreate(property, UriKind.RelativeOrAbsolute, out result) && result.IsAbsoluteUri)
          {
            string toolSpecificId = LinkingUtilities.DecodeUri(property.Trim()).ToolSpecificId;
            str = !VssStringComparer.DomainName.Equals(toolSpecificId, instanceId) ? (!VssStringComparer.DomainName.Equals(toolSpecificId, collectionId) ? toolSpecificId.ToString() : "488bb442-0beb-4c1e-98b6-4eddc604bd9e") : "b36ad70a-0d79-49c8-a165-30b643926fef";
          }
        }
      }
      else if (IdentityConstantsNormalizer.GetBisIdentityType((IVssIdentity) identity) == IdentityType.Extensible && string.IsNullOrWhiteSpace(property))
        str = "TBD";
      return str;
    }

    internal static IdentityType GetBisIdentityType(IVssIdentity identity)
    {
      string identityType = identity.Descriptor.IdentityType;
      if (VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.TeamFoundation.Identity"))
        return IdentityType.ApplicationGroup;
      if (VssStringComparer.IdentityDescriptor.Equals(identityType, "System.Security.Principal.WindowsIdentity"))
        return !identity.IsContainer ? IdentityType.WindowsUser : IdentityType.WindowsGroup;
      if (!VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.IdentityModel.Claims.ClaimsIdentity") && !VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.TeamFoundation.BindPendingIdentity") && !VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.VisualStudio.Services.Identity.ServerTestIdentity") && !VssStringComparer.IdentityDescriptor.Equals(identityType, "Microsoft.TeamFoundation.ServiceIdentity"))
        VssStringComparer.IdentityDescriptor.Equals(identityType, "System:ServicePrincipal");
      return IdentityType.Extensible;
    }
  }
}
