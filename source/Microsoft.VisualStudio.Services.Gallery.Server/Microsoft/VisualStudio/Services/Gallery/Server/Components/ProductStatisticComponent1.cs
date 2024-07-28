// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ProductStatisticComponent1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ProductStatisticComponent1 : ProductStatisticComponent
  {
    public virtual void UpdateProductStatistic(
      string product,
      string statistic,
      double statisticValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(product, nameof (product));
      ArgumentUtility.CheckStringForNullOrEmpty(statistic, nameof (statistic));
      this.PrepareStoredProcedure("Gallery.prc_UpdateProductStatistic");
      this.BindString(nameof (product), product, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (statistic), statistic, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDouble(nameof (statisticValue), statisticValue);
      this.ExecuteNonQuery();
    }

    public virtual double GetProductStatistic(string product, string statistic)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(product, nameof (product));
      ArgumentUtility.CheckStringForNullOrEmpty(statistic, nameof (statistic));
      this.PrepareStoredProcedure("Gallery.prc_GetProductStatistic");
      this.BindString(nameof (product), product, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (statistic), statistic, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetProductStatistic", this.RequestContext))
      {
        resultCollection.AddBinder<double>((ObjectBinder<double>) new ProductStatisticBinder());
        List<double> items = resultCollection.GetCurrent<double>().Items;
        // ISSUE: explicit non-virtual call
        return items != null && __nonvirtual (items.Count) > 0 ? items[0] : 0.0;
      }
    }
  }
}
