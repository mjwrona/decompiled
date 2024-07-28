// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IPerfTraceService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (PerfTraceService))]
  public interface IPerfTraceService : IVssFrameworkService
  {
    T MeasureTopLevelScenario<T>(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      int samplingRate,
      Func<T> action);

    void MeasureTopLevelScenario(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      int samplingRate,
      Action action);

    T Measure<T>(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      Func<T> action);

    void Measure(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      Action action);
  }
}
