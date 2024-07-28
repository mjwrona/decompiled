// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.NonSwallowingTransformManyBlock
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class NonSwallowingTransformManyBlock
  {
    public static TransformManyBlock<TInput, TOutput> Create<TInput, TOutput>(
      Func<TInput, IEnumerable<TOutput>> transform,
      CancellationToken cancellationToken)
    {
      return new TransformManyBlock<TInput, TOutput>(NonSwallowingTransformBlockUtils.CreateNonSwallowingFunc<TInput, IEnumerable<TOutput>>(transform, typeof (TransformManyBlock<TInput, TOutput>), cancellationToken));
    }

    public static TransformManyBlock<TInput, TOutput> Create<TInput, TOutput>(
      Func<TInput, Task<IEnumerable<TOutput>>> transform,
      CancellationToken cancellationToken)
    {
      return new TransformManyBlock<TInput, TOutput>(NonSwallowingTransformBlockUtils.CreateNonSwallowingTaskFunc<TInput, IEnumerable<TOutput>>(transform, typeof (TransformManyBlock<TInput, TOutput>), cancellationToken));
    }

    public static TransformManyBlock<TInput, TOutput> Create<TInput, TOutput>(
      Func<TInput, IEnumerable<TOutput>> transform,
      ExecutionDataflowBlockOptions dataflowBlockOptions)
    {
      return new TransformManyBlock<TInput, TOutput>(NonSwallowingTransformBlockUtils.CreateNonSwallowingFunc<TInput, IEnumerable<TOutput>>(transform, typeof (TransformManyBlock<TInput, TOutput>), dataflowBlockOptions.CancellationToken), dataflowBlockOptions);
    }

    public static TransformManyBlock<TInput, TOutput> Create<TInput, TOutput>(
      Func<TInput, Task<IEnumerable<TOutput>>> transform,
      ExecutionDataflowBlockOptions dataflowBlockOptions)
    {
      return new TransformManyBlock<TInput, TOutput>(NonSwallowingTransformBlockUtils.CreateNonSwallowingTaskFunc<TInput, IEnumerable<TOutput>>(transform, typeof (TransformManyBlock<TInput, TOutput>), dataflowBlockOptions.CancellationToken), dataflowBlockOptions);
    }
  }
}
