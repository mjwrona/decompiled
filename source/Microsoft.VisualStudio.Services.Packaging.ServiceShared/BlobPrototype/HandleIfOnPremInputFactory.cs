// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.HandleIfOnPremInputFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class HandleIfOnPremInputFactory
  {
    public static HandleIfOnPremInputFactory<TIn, TOut> Create<TIn, TOut>(
      IExecutionEnvironment executionEnvironment,
      IAsyncHandler<TIn, TOut> handler)
      where TIn : class
    {
      return new HandleIfOnPremInputFactory<TIn, TOut>(executionEnvironment, handler);
    }
  }
}
