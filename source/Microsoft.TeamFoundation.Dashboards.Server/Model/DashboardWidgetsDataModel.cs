// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Model.DashboardWidgetsDataModel
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Model
{
  public class DashboardWidgetsDataModel
  {
    public IEnumerable<WidgetDataModel> Widgets;
    public string ETag;

    public Guid DashboardId { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }
  }
}
