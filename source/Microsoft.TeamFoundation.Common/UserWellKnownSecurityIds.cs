// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.UserWellKnownSecurityIds
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Security.Principal;

namespace Microsoft.TeamFoundation
{
  [Obsolete("The anonymous principal has been moved to the system store.")]
  public static class UserWellKnownSecurityIds
  {
    public static readonly SecurityIdentifier AnonymousPrincipal = new SecurityIdentifier(UserWellKnownSidConstants.AnonymousPrincipalSid);
  }
}
