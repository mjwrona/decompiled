// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Services.IScaledAgileViewService
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Web.Services
{
  [DefaultServiceImplementation(typeof (ScaledAgileViewService))]
  public interface IScaledAgileViewService : IVssFrameworkService
  {
    Plan GetView(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid viewId,
      bool includeCardSettings = true);

    IEnumerable<Plan> GetViewDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid ownerId = default (Guid));

    IList<Plan> GetViewDefinitions(
      IVssRequestContext requestContext,
      List<Tuple<Guid, Guid>> planIdsWithProjectId);

    Plan CreateView(IVssRequestContext requestContext, Guid projectId, CreatePlan plan);

    void UpdateView(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      UpdatePlan updatedPlan);

    void DeleteView(IVssRequestContext requestContext, Guid projectId, int daysOld);

    void SoftDeleteView(IVssRequestContext requestContext, Guid projectId, Guid viewId);

    PlanViewData GetViewData(
      IVssRequestContext requestContext,
      Guid viewId,
      PlanType viewType,
      PlanViewFilter viewFilter);

    PlanViewData GetNewViewData(
      IVssRequestContext requestContext,
      Guid viewId,
      PlanViewFilter viewFilter);

    void UpdateLastAccessed(IVssRequestContext requestContext, Guid projectId, Guid planId);

    (int plansCount, int maxPlans) GetPlansCount(IVssRequestContext requestContext, Guid projectId);
  }
}
