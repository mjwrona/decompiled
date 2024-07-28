// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns.SkipAndReturnNullIfFeatureFlagOffHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns
{
  public class SkipAndReturnNullIfFeatureFlagOffHandler<TIn, TOut> : 
    IAsyncHandler<TIn, TOut>,
    IHaveInputType<TIn>,
    IHaveOutputType<TOut>
  {
    private readonly IFeatureFlagService featureFlagService;
    private readonly string flag;
    private readonly IAsyncHandler<TIn, TOut> handler;

    public SkipAndReturnNullIfFeatureFlagOffHandler(
      IFeatureFlagService featureFlagService,
      string flag,
      IAsyncHandler<TIn, TOut> handler)
    {
      this.featureFlagService = featureFlagService;
      this.flag = flag;
      this.handler = handler;
    }

    public async Task<TOut> Handle(TIn request) => this.featureFlagService.IsEnabled(this.flag) ? await this.handler.Handle(request) : default (TOut);
  }
}
