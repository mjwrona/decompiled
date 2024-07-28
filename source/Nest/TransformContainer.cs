// Decompiled with JetBrains decompiler
// Type: Nest.TransformContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TransformContainer : ITransformContainer, IDescriptor
  {
    internal TransformContainer()
    {
    }

    public TransformContainer(TransformBase transform)
    {
      transform.ThrowIfNull<TransformBase>(nameof (transform));
      transform.WrapInContainer((ITransformContainer) this);
    }

    IChainTransform ITransformContainer.Chain { get; set; }

    IScriptTransform ITransformContainer.Script { get; set; }

    ISearchTransform ITransformContainer.Search { get; set; }
  }
}
