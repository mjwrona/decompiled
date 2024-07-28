// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.DashboardDefaultViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  public class DashboardDefaultViewModel
  {
    public string DefaultDashboardId { get; set; }

    public Dashboard DefaultDashboardWidgets { get; set; }

    public int MaxWidgetsPerDashboard { get; set; }

    public int MaxDashboardPerGroup { get; set; }

    public bool IsStakeholder { get; set; }
  }
}
