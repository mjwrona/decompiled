// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointHistoryWatermark
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPointHistoryWatermark
  {
    public DateTime WatermarkDate { get; set; }

    public int PointId { get; set; }

    public int ChangeNumber { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is TestPointHistoryWatermark))
        return base.Equals(obj);
      TestPointHistoryWatermark historyWatermark = (TestPointHistoryWatermark) obj;
      return this.WatermarkDate.Equals(historyWatermark.WatermarkDate) && this.PointId.Equals(historyWatermark.PointId) && this.ChangeNumber.Equals(historyWatermark.ChangeNumber);
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
