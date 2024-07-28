// Decompiled with JetBrains decompiler
// Type: Nest.TransformDestinationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TransformDestinationDescriptor : 
    DescriptorBase<TransformDestinationDescriptor, ITransformDestination>,
    ITransformDestination
  {
    IndexName ITransformDestination.Index { get; set; }

    string ITransformDestination.Pipeline { get; set; }

    public TransformDestinationDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<ITransformDestination, IndexName>) ((a, v) => a.Index = v));

    public TransformDestinationDescriptor Index<T>() => this.Assign<Type>(typeof (T), (Action<ITransformDestination, Type>) ((a, v) => a.Index = (IndexName) v));

    public TransformDestinationDescriptor Pipeline(string pipeline) => this.Assign<string>(pipeline, (Action<ITransformDestination, string>) ((a, v) => a.Pipeline = v));
  }
}
