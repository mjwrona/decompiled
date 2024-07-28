// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ChartPinningState
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ChartPinningState
  {
    public ChartPinningState(bool canEditPin, bool canPinChart)
    {
      this.canEditPin = canEditPin;
      this.canPinChart = canPinChart;
      this.isChartPinned = true;
    }

    public bool canEditPin { get; private set; }

    public bool canPinChart { get; private set; }

    public bool isChartPinned { get; private set; }
  }
}
