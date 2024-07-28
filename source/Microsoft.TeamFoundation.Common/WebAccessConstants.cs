// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WebAccessConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.ComponentModel;
using System.Security.Principal;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WebAccessConstants
  {
    public const string CommonDataRelativePath = "Microsoft\\DevOps\\Web Access";
    public const string CacheRelativePath = "Cache_v";
    public static readonly string WorkItemOnlyViewUsersGroupIdentifier = SidIdentityHelper.ConstructWellKnownSid(3U, 1U);
    public static readonly SecurityIdentifier WorkItemOnlyViewUsersGroup = new SecurityIdentifier(WebAccessConstants.WorkItemOnlyViewUsersGroupIdentifier);
  }
}
