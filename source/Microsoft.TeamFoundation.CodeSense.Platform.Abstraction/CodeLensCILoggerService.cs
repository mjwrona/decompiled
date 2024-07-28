// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.CodeLensCILoggerService
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public class CodeLensCILoggerService : 
    ICodeLensCILoggerService,
    IVssFrameworkService,
    ICodeLensLayerCakeSettings
  {
    private ExportLifetimeContext<ICodeLensCILoggerService> _codeLensCILogger;
    private LayerCakeCISettings settings;

    public LayerCakeCISettings Settings => this.settings;

    public void PublishCI(
      IVssRequestContext requestContext,
      CodeLensCILevel metricLevel,
      string area,
      string feature,
      Dictionary<string, object> value)
    {
      this._codeLensCILogger.Value.PublishCI(requestContext, metricLevel, area, feature, value);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this._codeLensCILogger == null)
        return;
      this._codeLensCILogger.Dispose();
      this._codeLensCILogger = (ExportLifetimeContext<ICodeLensCILoggerService>) null;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this._codeLensCILogger = CodeLensExtension.CreateLifeTimeExport<ICodeLensCILoggerService>(systemRequestContext);
      this.SetLayerCakeCISettings(systemRequestContext);
    }

    internal void SetLayerCakeCISettings(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      bool flag = service.IsLayerCakeSamplingFeatureEnabled(requestContext);
      TimeSpan timeSpan = TimeSpan.FromMilliseconds(service.GetTimeElapsedThresholdForLayerCakeCIInMilliseconds(requestContext));
      int probabilityInverse = service.GetLayerCakeSamplingProbabilityInverse(requestContext);
      this.settings = new LayerCakeCISettings()
      {
        IsLayerCakeSamplingEnabled = flag,
        TimeElapsedThresholdForLayerCakeCI = timeSpan,
        ProbabilityInverse = probabilityInverse
      };
    }
  }
}
