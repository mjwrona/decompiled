// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IQueryExperimentService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (QueryExperimentService))]
  public interface IQueryExperimentService : IVssFrameworkService
  {
    T GetValueForTargetExperiment<T>(
      IVssRequestContext requestContext,
      QueryExperiment targetExperiment,
      T inExperimentValue,
      T inControlValue);

    T RunPerformanceExperiment<T>(
      IVssRequestContext requestContext,
      Func<T> controlAlgorithm,
      Func<T> experimentalAlgorithm,
      bool runBothAlgorithms = false);

    QueryExperiment GetCurrentExperiment();

    QueryExperiment GetCurrentExperimentState(IVssRequestContext requestContext);
  }
}
