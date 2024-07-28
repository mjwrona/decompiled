// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.AbstractAnalyzerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B7677EA-AF32-40D9-850C-DD66DED9A2C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI
{
  public abstract class AbstractAnalyzerFactory
  {
    public abstract HashSet<IAnalyzer> GetAnalyzers(Scenario scenario);
  }
}
