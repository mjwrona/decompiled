// Decompiled with JetBrains decompiler
// Type: Nest.TransformSyncContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class TransformSyncContainer : ITransformSyncContainer
  {
    public TransformSyncContainer()
    {
    }

    public TransformSyncContainer(TransformSyncBase transform)
    {
      transform.ThrowIfNull<TransformSyncBase>(nameof (transform));
      transform.WrapInContainer((ITransformSyncContainer) this);
    }

    public ITransformTimeSync Time { get; set; }

    public static implicit operator TransformSyncContainer(TransformSyncBase transform) => transform != null ? new TransformSyncContainer(transform) : (TransformSyncContainer) null;
  }
}
