// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IKpiService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (KpiService))]
  public interface IKpiService : IVssFrameworkService
  {
    void DeleteKpiState(IVssRequestContext requestContext, KpiDefinition kpiDefinition);

    void DeleteKpiStates(
      IVssRequestContext requestContext,
      IEnumerable<KpiDefinition> kpiDefinitions);

    void EnsureKpiIsRegistered(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope,
      string displayName,
      string description = null);

    KpiDefinition GetKpiDefinition(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope);

    void Publish(IVssRequestContext requestContext, string area, string name, double value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string scope,
      string name,
      double value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string name,
      double value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string scope,
      string name,
      double value);

    void Publish(IVssRequestContext requestContext, string area, KpiMetric metric);

    void Publish(IVssRequestContext requestContext, string area, string scope, KpiMetric metric);

    void Publish(IVssRequestContext requestContext, string area, Guid hostId, KpiMetric metric);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string scope,
      KpiMetric metric);

    void Publish(IVssRequestContext requestContext, string area, List<KpiMetric> metrics);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string scope,
      List<KpiMetric> metrics);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      List<KpiMetric> metrics);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      Guid hostId,
      string scope,
      List<KpiMetric> metrics);

    void SaveKpi(IVssRequestContext requestContext, KpiDefinition kpiDefinition);

    void SaveKpis(IVssRequestContext requestContext, IEnumerable<KpiDefinition> kpiDefinitions);

    void SetStates(
      IVssRequestContext requestContext,
      string area,
      string name,
      List<KpiStateDefinition> states);

    void SetStates(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope,
      List<KpiStateDefinition> states);
  }
}
