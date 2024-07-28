// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.IChartConfigurationService
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  [DefaultServiceImplementation(typeof (ChartConfigurationService))]
  public interface IChartConfigurationService : IVssFrameworkService
  {
    ChartConfiguration SaveChartConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      ChartConfiguration chartConfiguration);

    void DeleteChartConfiguration(IVssRequestContext requestContext, Guid projectId, Guid id);

    void ValidateChartConfiguration(
      IVssRequestContext requestContext,
      ChartConfiguration chartConfiguration);

    IEnumerable<ChartConfiguration> GetChartConfigurationGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      string scope,
      string groupKey);

    ChartConfiguration GetChartConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid id);

    void DeleteChartConfigurationsByGroups(
      IVssRequestContext requestContext,
      Guid projectI,
      string scope,
      IEnumerable<string> groupKeysd);

    IEnumerable<Guid> GetChartConfigurationIdsByGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys);
  }
}
