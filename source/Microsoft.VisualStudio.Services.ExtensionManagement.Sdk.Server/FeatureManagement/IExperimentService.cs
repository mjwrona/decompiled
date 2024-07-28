// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.IExperimentService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  [DefaultServiceImplementation(typeof (ExperimentService))]
  public interface IExperimentService : IVssFrameworkService
  {
    T RunPerformanceExperiment<T>(
      IVssRequestContext requestContext,
      string contributionId,
      Func<T> controlAlgorithm,
      Func<T> experimentalAlgorithm,
      bool runBothAlgorithms = false);

    void RunPerformanceExperiment(
      IVssRequestContext requestContext,
      string contributionId,
      Action controlAlgorithm,
      Action experimentalAlgorithm,
      bool runBothAlgorithms = false);

    ExperimentFeature GetExperiment(IVssRequestContext requestContext, string contributionId);
  }
}
