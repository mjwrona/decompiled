// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ChartConfigurationBundle
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Charting.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ChartConfigurationBundle
  {
    public ChartConfigurationBundle(
      ChartConfiguration chartConfiguration,
      string uri,
      ChartPinningState chartPinningState)
    {
      this.ChartConfiguration = chartConfiguration;
      this.Uri = uri;
      this.chartPinningState = chartPinningState;
    }

    public ChartConfiguration ChartConfiguration { get; private set; }

    public string Uri { get; private set; }

    public ChartPinningState chartPinningState { get; private set; }
  }
}
