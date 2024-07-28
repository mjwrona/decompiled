// Decompiled with JetBrains decompiler
// Type: Nest.TransformBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class TransformBase
  {
    public static implicit operator TransformContainer(TransformBase transform) => transform != null ? new TransformContainer(transform) : (TransformContainer) null;

    internal abstract void WrapInContainer(ITransformContainer container);
  }
}
