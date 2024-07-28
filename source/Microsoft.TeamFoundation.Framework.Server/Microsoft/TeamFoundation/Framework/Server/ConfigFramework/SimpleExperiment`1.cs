// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.SimpleExperiment`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal sealed class SimpleExperiment<T> : IExperiment<T>
  {
    private readonly T _value;

    public SimpleExperiment(T value) => this._value = value;

    public bool TryMatch(in Query q, out T output)
    {
      output = this._value;
      return true;
    }

    bool IExperiment<T>.TryMatch(in Query q, out T output) => this.TryMatch(in q, out output);
  }
}
