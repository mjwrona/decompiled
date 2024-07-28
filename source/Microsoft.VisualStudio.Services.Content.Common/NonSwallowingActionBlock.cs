// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.NonSwallowingActionBlock
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class NonSwallowingActionBlock
  {
    public static ActionBlock<T> Create<T>(Action<T> action, CancellationToken cancellationToken) => new ActionBlock<T>(NonSwallowingActionBlock.CreateNonSwallowingAction<T>(action, cancellationToken));

    public static ActionBlock<T> Create<T>(
      Func<T, Task> action,
      CancellationToken cancellationToken)
    {
      return new ActionBlock<T>(NonSwallowingActionBlock.CreateNonSwallowingFunc<T>(action, cancellationToken));
    }

    public static ActionBlock<T> Create<T>(
      Action<T> action,
      ExecutionDataflowBlockOptions dataflowBlockOptions)
    {
      return new ActionBlock<T>(NonSwallowingActionBlock.CreateNonSwallowingAction<T>(action, dataflowBlockOptions.CancellationToken), dataflowBlockOptions);
    }

    public static ActionBlock<T> Create<T>(
      Func<T, Task> action,
      ExecutionDataflowBlockOptions dataflowBlockOptions)
    {
      return new ActionBlock<T>(NonSwallowingActionBlock.CreateNonSwallowingFunc<T>(action, dataflowBlockOptions.CancellationToken), dataflowBlockOptions);
    }

    private static Action<T> CreateNonSwallowingAction<T>(
      Action<T> action,
      CancellationToken cancellationToken)
    {
      return (Action<T>) (input =>
      {
        try
        {
          action(input);
        }
        catch (OperationCanceledException ex)
        {
          Type blockType = typeof (ActionBlock<T>);
          CancellationToken cancellationToken1 = cancellationToken;
          throw NonSwallowingTransformBlockUtils.CreateTimeoutException(ex, blockType, cancellationToken1);
        }
      });
    }

    private static Func<T, Task> CreateNonSwallowingFunc<T>(
      Func<T, Task> action,
      CancellationToken cancellationToken)
    {
      return (Func<T, Task>) (async input =>
      {
        try
        {
          await action(input).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
          Type blockType = typeof (ActionBlock<T>);
          CancellationToken cancellationToken1 = cancellationToken;
          throw NonSwallowingTransformBlockUtils.CreateTimeoutException(ex, blockType, cancellationToken1);
        }
      });
    }
  }
}
