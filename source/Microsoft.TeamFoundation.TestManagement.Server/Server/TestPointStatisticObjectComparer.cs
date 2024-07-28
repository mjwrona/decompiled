// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointStatisticObjectComparer
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPointStatisticObjectComparer : IEqualityComparer<TestPointStatistic>
  {
    public bool Equals(TestPointStatistic x, TestPointStatistic y) => x != null && y != null && (int) x.TestPointState == (int) y.TestPointState && (int) x.ResultState == (int) y.ResultState && (int) x.ResultOutcome == (int) y.ResultOutcome && (int) x.FailureType == (int) y.FailureType && x.ResolutionStateId == y.ResolutionStateId;

    public int GetHashCode(TestPointStatistic obj)
    {
      int[] numArray = new int[5]
      {
        923521,
        29791,
        961,
        31,
        1
      };
      return (int) obj.TestPointState * numArray[0] + (int) obj.ResultState * numArray[1] + (int) obj.ResultOutcome * numArray[2] + (int) obj.FailureType * numArray[3] + obj.ResolutionStateId * numArray[4];
    }
  }
}
