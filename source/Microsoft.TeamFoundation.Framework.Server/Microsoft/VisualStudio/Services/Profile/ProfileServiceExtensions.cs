// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.VisualStudio.Services.Profile
{
  public static class ProfileServiceExtensions
  {
    private const string TraceArea = "IdentityService";
    private const string TraceLayer = "ProfileServiceExtensions";

    public static bool CanReceiveMail(this IReadOnlyVssIdentity identity)
    {
      ArgumentUtility.CheckForNull<IReadOnlyVssIdentity>(identity, nameof (identity));
      return !identity.IsContainer || AadIdentityHelper.IsAadGroup(identity.Descriptor);
    }
  }
}
