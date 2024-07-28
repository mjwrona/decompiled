// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.SystemPermissionsHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class SystemPermissionsHelper
  {
    public static IAccessControlEntry MergePermissions(
      IAccessControlEntry userAce,
      int systemAllow,
      int systemDeny)
    {
      return systemAllow != 0 || systemDeny != 0 ? (IAccessControlEntry) new AccessControlEntry(userAce.Descriptor, systemAllow | userAce.Allow & ~systemDeny, systemDeny | userAce.Deny & ~systemAllow, userAce.InheritedAllow & ~systemDeny, userAce.InheritedDeny & ~systemAllow, systemAllow | userAce.EffectiveAllow & ~systemDeny, systemDeny | userAce.EffectiveDeny & ~systemAllow) : userAce;
    }
  }
}
