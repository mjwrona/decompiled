// Decompiled with JetBrains decompiler
// Type: MS.VS.Services.CodeLens.Platform.Onprem.CodeLensOnpremKpiLoggerService
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B5DEEFA-3C5E-4BFB-92E2-3ADDA47952C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.OnPrem.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace MS.VS.Services.CodeLens.Platform.Onprem
{
  [Export(typeof (ICodeLensKpiLoggerService))]
  public class CodeLensOnpremKpiLoggerService : ICodeLensKpiLoggerService, IVssFrameworkService
  {
    public void LogKPI(
      IVssRequestContext requestContext,
      Guid hostId,
      string kpiArea,
      string kpiName)
    {
    }

    public void PublishEvents(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
