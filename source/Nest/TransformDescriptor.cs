// Decompiled with JetBrains decompiler
// Type: Nest.TransformDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TransformDescriptor : TransformContainer
  {
    private TransformDescriptor Assign<TValue>(
      TValue value,
      Action<ITransformContainer, TValue> assigner)
    {
      return Fluent.Assign<TransformDescriptor, ITransformContainer, TValue>(this, value, assigner);
    }

    public TransformDescriptor Search(
      Func<SearchTransformDescriptor, ISearchTransform> selector)
    {
      return this.Assign<Func<SearchTransformDescriptor, ISearchTransform>>(selector, (Action<ITransformContainer, Func<SearchTransformDescriptor, ISearchTransform>>) ((a, v) => a.Search = v != null ? v.InvokeOrDefault<SearchTransformDescriptor, ISearchTransform>(new SearchTransformDescriptor()) : (ISearchTransform) null));
    }

    public TransformDescriptor Script(
      Func<ScriptTransformDescriptor, IScriptTransform> selector)
    {
      return this.Assign<Func<ScriptTransformDescriptor, IScriptTransform>>(selector, (Action<ITransformContainer, Func<ScriptTransformDescriptor, IScriptTransform>>) ((a, v) => a.Script = v != null ? v(new ScriptTransformDescriptor()) : (IScriptTransform) null));
    }

    public TransformDescriptor Chain(
      Func<ChainTransformDescriptor, IChainTransform> selector)
    {
      return this.Assign<Func<ChainTransformDescriptor, IChainTransform>>(selector, (Action<ITransformContainer, Func<ChainTransformDescriptor, IChainTransform>>) ((a, v) => a.Chain = v != null ? v(new ChainTransformDescriptor()) : (IChainTransform) null));
    }
  }
}
