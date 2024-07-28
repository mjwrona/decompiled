// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.NonSwallowingTransformBlockUtils
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class NonSwallowingTransformBlockUtils
  {
    internal static Func<TInput, TTransformOutput> CreateNonSwallowingFunc<TInput, TTransformOutput>(
      Func<TInput, TTransformOutput> transform,
      Type blockType,
      CancellationToken cancellationToken)
    {
      return (Func<TInput, TTransformOutput>) (input =>
      {
        try
        {
          return transform(input);
        }
        catch (OperationCanceledException ex)
        {
          Type blockType1 = blockType;
          CancellationToken cancellationToken1 = cancellationToken;
          throw NonSwallowingTransformBlockUtils.CreateTimeoutException(ex, blockType1, cancellationToken1);
        }
      });
    }

    internal static Func<TInput, Task<TTransformOutput>> CreateNonSwallowingTaskFunc<TInput, TTransformOutput>(
      Func<TInput, Task<TTransformOutput>> transform,
      Type blockType,
      CancellationToken cancellationToken)
    {
      return (Func<TInput, Task<TTransformOutput>>) (async input =>
      {
        TTransformOutput swallowingTaskFunc;
        try
        {
          swallowingTaskFunc = await transform(input);
        }
        catch (OperationCanceledException ex)
        {
          Type blockType1 = blockType;
          CancellationToken cancellationToken1 = cancellationToken;
          throw NonSwallowingTransformBlockUtils.CreateTimeoutException(ex, blockType1, cancellationToken1);
        }
        return swallowingTaskFunc;
      });
    }

    public static TimeoutException CreateTimeoutException(
      OperationCanceledException oce,
      Type blockType,
      CancellationToken cancellationToken)
    {
      return NonSwallowingTransformBlockUtils.CreateTimeoutException(oce, blockType.Name, cancellationToken);
    }

    public static TimeoutException CreateTimeoutException(
      OperationCanceledException oce,
      string cancelledObject,
      CancellationToken cancellationToken)
    {
      return new TimeoutException(!(cancellationToken == CancellationToken.None) ? (!(oce.CancellationToken != cancellationToken) ? Resources.CancellationCaller((object) cancelledObject) : Resources.CancellationNotCaller((object) cancelledObject)) : Resources.CancellationUnknown((object) cancelledObject), (Exception) oce);
    }
  }
}
