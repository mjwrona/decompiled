// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.FeatureAvailabilityConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class FeatureAvailabilityConstants
  {
    public static readonly string FeatureAvailabilityAdminUsersGroupIdentifier = SidIdentityHelper.ConstructWellKnownSid(7U, 1U);
    public static readonly string FeatureAvailabilityAccountAdminUsersGroupIdentifier = SidIdentityHelper.ConstructWellKnownSid(7U, 2U);
    public static readonly string FeatureAvailabilityReadersUsersGroupIdentifier = SidIdentityHelper.ConstructWellKnownSid(7U, 3U);
  }
}
