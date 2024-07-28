// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Feature`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class Feature<T> : IFeature<T>
  {
    private readonly IFeature<T> _parent1;
    private readonly IFeature<T> _parent2;
    private FeatureState _value;

    public Feature(IFeature<T> parent1, IFeature<T> parent2, FeatureState initialValue)
    {
      this._parent1 = parent1;
      this._parent2 = parent2;
      this._value = initialValue;
    }

    public Feature(IFeature<T> parent1, FeatureState initialValue)
      : this(parent1, (IFeature<T>) null, initialValue)
    {
    }

    public Feature(FeatureState initialValue)
      : this((IFeature<T>) null, (IFeature<T>) null, initialValue)
    {
    }

    public virtual FeatureState GetFeatureState(T context)
    {
      if (this._parent1 == null)
        return this._value;
      return this._parent2 == null ? Feature<T>.Max(this._value, this._parent1.GetFeatureState(context)) : Feature<T>.Max(this._value, this._parent1.GetFeatureState(context), this._parent2.GetFeatureState(context));
    }

    internal void SetValue(FeatureState newValue) => this._value = newValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static FeatureState Max(FeatureState state1, FeatureState state2) => state1 <= state2 ? state2 : state1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static FeatureState Max(FeatureState state1, FeatureState state2, FeatureState state3) => Feature<T>.Max(state1, Feature<T>.Max(state2, state3));
  }
}
