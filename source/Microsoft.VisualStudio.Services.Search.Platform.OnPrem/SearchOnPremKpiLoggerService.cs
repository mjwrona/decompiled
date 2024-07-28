// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Search.Platform.OnPrem.SearchOnPremKpiLoggerService
// Assembly: Microsoft.VisualStudio.Services.Search.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 34F866B2-C282-4F28-9C5F-A4E5E97C2DB9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platform.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Search.Platform.OnPrem
{
  [Export(typeof (ISearchKpiLoggerService))]
  public class SearchOnPremKpiLoggerService : ISearchKpiLoggerService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Publish(
      IVssRequestContext requestContext,
      string kpiArea,
      string scope,
      List<KpiMetric> kpiMetric)
    {
    }
  }
}
