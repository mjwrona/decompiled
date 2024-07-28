// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ReservedGroupWellKnownNames
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation
{
  public static class ReservedGroupWellKnownNames
  {
    public static readonly SortedSet<string> WellKnownGroupNames = new SortedSet<string>()
    {
      "Agent Pool Administrators",
      "Agent Pool Service Accounts",
      "Agent Queue Administrators",
      "Agent Queue Creators",
      "Agent Queue Users",
      "Deployment Group Administrators",
      "Endpoint Administrators",
      "Endpoint Creators"
    };
  }
}
