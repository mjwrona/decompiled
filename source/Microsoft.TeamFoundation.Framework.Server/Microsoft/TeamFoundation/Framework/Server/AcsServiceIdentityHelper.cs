// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AcsServiceIdentityHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class AcsServiceIdentityHelper
  {
    private const string c_defaultIdentityName = "Account Service";

    internal static bool IsServiceIdentity(IReadOnlyVssIdentity identity)
    {
      if (identity == null)
        return false;
      bool flag = false;
      if (identity.Descriptor.IdentityType == "Microsoft.TeamFoundation.ServiceIdentity")
        flag = true;
      else if (!identity.IsContainer && identity.Descriptor.IdentityType == "Microsoft.IdentityModel.Claims.ClaimsIdentity")
      {
        string property = identity.GetProperty<string>("Domain", (string) null);
        if (property != null && (VssStringComparer.DomainName.Equals(property, "LOCAL AUTHORITY") || VssStringComparer.DomainName.Equals(property, "ClaimsProvider")))
          flag = true;
      }
      return flag;
    }

    internal static string GetDefaultServiceIdentityCollectionName() => "Account Service";

    internal static string GetDefaultServiceIdentityInstanceName() => "Account Service";
  }
}
