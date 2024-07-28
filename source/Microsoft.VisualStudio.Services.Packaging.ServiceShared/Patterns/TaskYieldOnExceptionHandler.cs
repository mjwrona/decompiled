// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.TaskYieldOnExceptionHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns
{
  public static class TaskYieldOnExceptionHandler
  {
    public static IAsyncHandler<TRequest, TResponse> TaskYieldOnException<TRequest, TResponse>(
      this IAsyncHandler<TRequest, TResponse> innerHandler)
    {
      return TaskYieldOnExceptionHandler.Create<TRequest, TResponse>(innerHandler);
    }

    public static IAsyncHandler<TRequest, TResponse> Create<TRequest, TResponse>(
      IAsyncHandler<TRequest, TResponse> innerHandler)
    {
      return (IAsyncHandler<TRequest, TResponse>) new TaskYieldOnExceptionHandler.Impl<TRequest, TResponse>(innerHandler);
    }

    private class Impl<TRequest, TResponse> : 
      IAsyncHandler<TRequest, TResponse>,
      IHaveInputType<TRequest>,
      IHaveOutputType<TResponse>
    {
      private readonly IAsyncHandler<TRequest, TResponse> innerHandler;

      public Impl(IAsyncHandler<TRequest, TResponse> innerHandler) => this.innerHandler = innerHandler;

      public async Task<TResponse> Handle(TRequest request)
      {
        TResponse response;
        try
        {
          response = await this.innerHandler.Handle(request);
        }
        catch (object ex)
        {
          await Task.Yield();
          throw;
        }
        TResponse response1 = response;
        response = default (TResponse);
        return response1;
      }
    }
  }
}
