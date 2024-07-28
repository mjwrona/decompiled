// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlWellKnownGroups
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class VersionControlWellKnownGroups
  {
    public static readonly SecurityIdentifier ProxyServiceGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(5U, 1U));
  }
}
