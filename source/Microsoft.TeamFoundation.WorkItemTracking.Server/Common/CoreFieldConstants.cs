// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.CoreFieldConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class CoreFieldConstants
  {
    public static readonly string[] CoreIdentityFields = new string[4]
    {
      "System.CreatedBy",
      "System.ChangedBy",
      "System.AuthorizedAs",
      "System.AssignedTo"
    };
    public static readonly int[] RequiredCoreFieldIds = new int[17]
    {
      -3,
      8,
      -2,
      7,
      25,
      1,
      -7,
      2,
      22,
      24,
      9,
      -4,
      33,
      32,
      -1,
      -105,
      3
    };
    public static readonly int[] AlertableCoreFields = ((IEnumerable<int>) CoreFieldConstants.RequiredCoreFieldIds).Union<int>((IEnumerable<int>) new int[11]
    {
      -104,
      -31,
      -32,
      -57,
      75,
      -5,
      -404,
      -42,
      90,
      92,
      91
    }).ToArray<int>();
  }
}
