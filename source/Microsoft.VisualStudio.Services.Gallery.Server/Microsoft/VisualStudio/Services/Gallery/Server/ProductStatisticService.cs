// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ProductStatisticService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ProductStatisticService : IProductStatisticService, IVssFrameworkService
  {
    private const string s_area = "gallery";
    private const string s_layer = "productstatisticsservice";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateProductStatistic(
      IVssRequestContext requestContext,
      string productType,
      string statistic,
      double statisticValue)
    {
      try
      {
        using (ProductStatisticComponent component = requestContext.CreateComponent<ProductStatisticComponent>())
        {
          if (!(component is ProductStatisticComponent1 statisticComponent1))
            return;
          statisticComponent1.UpdateProductStatistic(productType, statistic, statisticValue);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061080, "gallery", "productstatisticsservice", ex);
        throw;
      }
    }

    public double GetProductStatistic(
      IVssRequestContext requestContext,
      string productType,
      string statistic)
    {
      try
      {
        using (ProductStatisticComponent component = requestContext.CreateComponent<ProductStatisticComponent>())
        {
          if (component is ProductStatisticComponent1 statisticComponent1)
            return statisticComponent1.GetProductStatistic(productType, statistic);
        }
        return 0.0;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061080, "gallery", "productstatisticsservice", ex);
        throw;
      }
    }
  }
}
