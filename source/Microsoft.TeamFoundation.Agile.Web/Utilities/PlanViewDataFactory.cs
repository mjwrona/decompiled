// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanViewDataFactory
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using System;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public static class PlanViewDataFactory
  {
    public static PlanViewData GetViewData(
      IVssRequestContext requestContext,
      Guid id,
      PlanType type,
      PlanViewFilter filter)
    {
      if (type == PlanType.DeliveryTimelineView)
        return DeliveryTimelineDataProvider.GetPlanViewData(requestContext, id, filter);
      throw new ViewTypeDoesNotExistException();
    }

    public static PlanPropertyCollection GetPlanProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      ScaledAgileView view,
      bool includeCardSettings)
    {
      if (view.Type == PlanType.DeliveryTimelineView)
        return DeliveryTimelineDataProvider.GetPlanProperties(requestContext, projectId, view, includeCardSettings);
      throw new ViewTypeDoesNotExistException();
    }
  }
}
