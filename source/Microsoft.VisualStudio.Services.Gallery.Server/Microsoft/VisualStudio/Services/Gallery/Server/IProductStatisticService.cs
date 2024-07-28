// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IProductStatisticService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (ProductStatisticService))]
  public interface IProductStatisticService : IVssFrameworkService
  {
    void UpdateProductStatistic(
      IVssRequestContext requestContext,
      string productType,
      string statistic,
      double statisticValue);

    double GetProductStatistic(
      IVssRequestContext requestContext,
      string productType,
      string statistic);
  }
}
