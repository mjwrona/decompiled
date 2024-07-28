// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.TargetBlockExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class TargetBlockExtensions
  {
    private const string SendAsyncHangPatternExplanation = "Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.";

    public static void PostOrThrow<T>(
      this ITargetBlock<T> targetBlock,
      T input,
      CancellationToken token,
      int taskDelayInMS = 5000)
    {
      if (targetBlock.Post<T>(input))
        return;
      TargetBlockExtensions.ThrowError<T>(targetBlock, "post", token, taskDelayInMS);
    }

    public static Task SendOrThrowAsync<T, T2>(
      this ITargetBlock<T> targetBlock,
      ITargetBlock<T2> finalBlock,
      T input,
      CancellationToken token)
    {
      return targetBlock == finalBlock ? targetBlock.SendOrThrowAsync<T>(input, token) : finalBlock.CancelProducerIfDataflowFaulted<T2>((Func<TargetBlockExtensions.DataflowNetworkCancellationToken, Task>) (linkedToken => targetBlock.SendOrThrowAsync<T>(input, linkedToken.Token, 5000)), token);
    }

    public static Task SendOrThrowAsync<T>(
      this ITargetBlock<T> targetBlock,
      T input,
      TargetBlockExtensions.DataflowNetworkCancellationToken token)
    {
      return targetBlock.SendOrThrowAsync<T>(input, token.Token);
    }

    public static Task SendOrThrowSingleBlockNetworkAsync<T>(
      this ActionBlock<T> actionBlock,
      T input,
      CancellationToken token)
    {
      return actionBlock.SendOrThrowAsync<T>(input, token);
    }

    private static async Task SendOrThrowAsync<T>(
      this ITargetBlock<T> targetBlock,
      T input,
      CancellationToken token,
      int taskDelayInMS = 5000)
    {
      if (await targetBlock.SendUnsafeAsync<T>(input, token).ConfigureAwait(false))
        return;
      TargetBlockExtensions.ThrowError<T>(targetBlock, "send", token, taskDelayInMS);
    }

    public static Task<bool> SendUnsafeAsync<TInput>(this ITargetBlock<TInput> target, TInput item) => DataflowBlock.SendAsync<TInput>(target, item);

    public static Task<bool> SendUnsafeAsync<TInput>(
      this ITargetBlock<TInput> target,
      TInput item,
      CancellationToken cancellationToken)
    {
      return DataflowBlock.SendAsync<TInput>(target, item, cancellationToken);
    }

    public static Task PostAllToUnboundedAndCompleteAsync<T>(
      this ITargetBlock<T> targetBlock,
      IEnumerable<T> inputs,
      CancellationToken cancellationToken)
    {
      foreach (T input in inputs)
      {
        try
        {
          targetBlock.PostOrThrow<T>(input, cancellationToken);
        }
        catch (InvalidOperationException ex) when (targetBlock.Completion.IsFaulted)
        {
          break;
        }
      }
      targetBlock.Complete();
      return targetBlock.Completion;
    }

    internal static async Task CancelProducerIfDataflowFaulted<T>(
      this ITargetBlock<T> finalBlock,
      Func<TargetBlockExtensions.DataflowNetworkCancellationToken, Task> producerAsync,
      CancellationToken token)
    {
      using (CancellationTokenSource cts = new CancellationTokenSource())
      {
        using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, token))
        {
          TargetBlockExtensions.DataflowNetworkCancellationToken cancellationToken = new TargetBlockExtensions.DataflowNetworkCancellationToken(linkedCts.Token);
          Task finalBlockCompletion = finalBlock.Completion;
          Task producerTask = producerAsync(cancellationToken);
          if (await Task.WhenAny(finalBlockCompletion, producerTask).ConfigureAwait(false) == finalBlockCompletion)
          {
            cts.Cancel();
            await Task.WhenAll(producerTask, finalBlockCompletion).ConfigureAwait(false);
          }
          else
            await producerTask.ConfigureAwait(false);
          finalBlockCompletion = (Task) null;
          producerTask = (Task) null;
        }
      }
    }

    public static Task SendAllAndCompleteSingleBlockNetworkAsync<T1>(
      this ActionBlock<T1> actionBlock,
      IEnumerable<T1> inputs,
      CancellationToken token)
    {
      return actionBlock.SendAllAndCompleteAsync<T1>(inputs, token);
    }

    private static async Task SendAllAndCompleteAsync<T1>(
      this ITargetBlock<T1> targetBlock,
      IEnumerable<T1> inputs,
      CancellationToken token)
    {
      foreach (T1 input in inputs)
      {
        try
        {
          await targetBlock.SendOrThrowAsync<T1>(input, token, 5000).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex) when (targetBlock.Completion.IsFaulted)
        {
          break;
        }
      }
      targetBlock.Complete();
      await targetBlock.Completion.ConfigureAwait(false);
    }

    public static async Task SendAllAndCompleteAsync<T1, T2>(
      this ITargetBlock<T1> targetBlock,
      IEnumerable<T1> inputs,
      ITargetBlock<T2> finalBlock,
      CancellationToken token)
    {
      if (targetBlock == finalBlock)
      {
        await targetBlock.SendAllAndCompleteAsync<T1>(inputs, token);
      }
      else
      {
        ConfiguredTaskAwaitable configuredTaskAwaitable = finalBlock.CancelProducerIfDataflowFaulted<T2>((Func<TargetBlockExtensions.DataflowNetworkCancellationToken, Task>) (async linkedToken =>
        {
          foreach (T1 input in inputs)
          {
            try
            {
              await targetBlock.SendOrThrowAsync<T1>(input, linkedToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex) when (targetBlock.Completion.IsFaulted)
            {
              break;
            }
          }
        }), token).ConfigureAwait(false);
        await configuredTaskAwaitable;
        targetBlock.Complete();
        configuredTaskAwaitable = finalBlock.Completion.ConfigureAwait(false);
        await configuredTaskAwaitable;
      }
    }

    private static void ThrowError<T>(
      ITargetBlock<T> actionBlock,
      string verb,
      CancellationToken token,
      int taskDelayInMS = 5000)
    {
      Task completion = actionBlock.Completion;
      if (!completion.IsCanceled && !completion.IsFaulted && !completion.IsCompleted)
      {
        if (Task.WaitAny(completion, Task.Delay(TimeSpan.FromMilliseconds((double) taskDelayInMS))) == 1 && !completion.IsCanceled && !completion.IsFaulted)
          throw new IllegalTargetBlockStateException("Could not " + verb + " to ActionBlock, which nonetheless is not cancelled or faulted, and has not completed after waiting for 5 seconds.");
      }
      if (completion.IsCanceled)
        throw new TaskCanceledException("Could not " + verb + " to ActionBlock. The block is cancelled." + (token.IsCancellationRequested ? " The cancellation has been requested on the passed token." : ""));
      if (completion.IsFaulted)
        throw new InvalidOperationException("Could not " + verb + " to faulted ActionBlock. Error: " + completion.Exception.Message, (Exception) completion.Exception);
      if (completion.IsCompleted)
        throw new InvalidOperationException("Could not " + verb + " to ActionBlock. The block is completed.");
      string str = string.Equals("post", verb) ? " This might be caused by bounded capacity setting." : "";
      throw new IllegalTargetBlockStateException("Could not " + verb + " to ActionBlock, which nonetheless is not completed, cancelled, or faulted." + str);
    }

    [Obsolete("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.", true)]
    public static Task<bool> SendAsync<TInput>(
      this ITargetBlock<TInput> target,
      TInput item,
      CancellationToken cancellationToken,
      object thisIsUnsafe1 = null)
    {
      throw new InvalidOperationException("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.");
    }

    [Obsolete("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.", true)]
    public static Task<bool> SendAsync<TInput>(
      this ITargetBlock<TInput> target,
      TInput item,
      CancellationToken cancellationToken,
      string thisIsUnsafe2 = null)
    {
      throw new InvalidOperationException("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.");
    }

    [Obsolete("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.", true)]
    public static Task<bool> SendAsync<TInput>(
      this ITargetBlock<TInput> target,
      TInput item,
      object thisIsUnsafe = null)
    {
      throw new InvalidOperationException("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.");
    }

    [Obsolete("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.", true)]
    public static Task<bool> SendAsync<TInput>(
      this ITargetBlock<TInput> target,
      TInput item,
      string thisIsUnsafe2 = null)
    {
      throw new InvalidOperationException("Use of ITargetBlock<TInput>.SendAsync can lead to a hang when used in a Dataflow with BoundedCapacity and faults. Call SendUnsafeAsync if this was intended.");
    }

    public struct DataflowNetworkCancellationToken
    {
      public readonly CancellationToken Token;

      internal DataflowNetworkCancellationToken(CancellationToken cancellationToken) => this.Token = cancellationToken;
    }
  }
}
