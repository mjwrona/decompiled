// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.ComplexConfig`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal sealed class ComplexConfig<T> : IConfigQueryable<T>
  {
    private readonly T _default;
    private readonly List<IExperiment<T>> _experiments;

    public ComplexConfig(T defaultValue, List<IExperiment<T>> experiments)
    {
      this._default = defaultValue;
      this._experiments = experiments;
    }

    public T Query(IVssRequestContext context, in Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Query query)
    {
      foreach (IExperiment<T> experiment in this._experiments)
      {
        T output;
        if (experiment.TryMatch(in query, out output))
          return output;
      }
      return this._default;
    }

    T IConfigQueryable<T>.Query(IVssRequestContext context, in Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Query query) => this.Query(context, in query);
  }
}
