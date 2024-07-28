// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.CodeLensKpiLoggerService
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public class CodeLensKpiLoggerService : ICodeLensKpiLoggerService, IVssFrameworkService
  {
    private ExportLifetimeContext<ICodeLensKpiLoggerService> _codeLensKpiLogger;

    public void LogKPI(
      IVssRequestContext requestContext,
      Guid hostId,
      string kpiArea,
      string kpiName)
    {
      this._codeLensKpiLogger.Value.LogKPI(requestContext, hostId, kpiArea, kpiName);
    }

    public void PublishEvents(IVssRequestContext requestContext) => this._codeLensKpiLogger.Value.PublishEvents(requestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.PublishEvents(systemRequestContext);
      if (this._codeLensKpiLogger == null)
        return;
      this._codeLensKpiLogger.Dispose();
      this._codeLensKpiLogger = (ExportLifetimeContext<ICodeLensKpiLoggerService>) null;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this._codeLensKpiLogger = CodeLensExtension.CreateLifeTimeExport<ICodeLensKpiLoggerService>(systemRequestContext);
  }
}
