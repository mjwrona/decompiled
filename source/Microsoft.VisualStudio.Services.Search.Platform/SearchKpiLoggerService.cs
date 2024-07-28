// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.Common.SearchKpiLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5EF41D26-5C57-4D41-AC16-607E07E24DBC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Search.Platform.Common
{
  public class SearchKpiLoggerService : ISearchKpiLoggerService, IVssFrameworkService
  {
    private ISearchKpiLoggerService m_kpiLogger;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_kpiLogger = SearchPlatformHelper.GetExtension<ISearchKpiLoggerService>(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_kpiLogger == null)
        return;
      if (this.m_kpiLogger is IDisposable kpiLogger)
      {
        kpiLogger.Dispose();
        this.m_kpiLogger = (ISearchKpiLoggerService) null;
      }
      this.m_kpiLogger = (ISearchKpiLoggerService) null;
    }

    public void Publish(
      IVssRequestContext requestContext,
      string kpiArea,
      string scope,
      List<KpiMetric> kpiMetric)
    {
      this.m_kpiLogger.Publish(requestContext, kpiArea, scope, kpiMetric);
    }
  }
}
