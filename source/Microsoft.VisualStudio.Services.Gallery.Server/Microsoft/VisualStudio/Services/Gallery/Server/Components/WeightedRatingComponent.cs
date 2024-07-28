// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.WeightedRatingComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class WeightedRatingComponent : TeamFoundationSqlResourceComponent
  {
    private const string s_area = "WeightedRatingComponent";
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<WeightedRatingComponent>(1)
    }, "WeightedRating");

    protected override string TraceArea => nameof (WeightedRatingComponent);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) WeightedRatingComponent.SqlExceptionFactories;

    public virtual void UpdateWeightedRating(
      string product,
      IEnumerable<string> installationTargets)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(product, nameof (product));
      if (installationTargets.IsNullOrEmpty<string>())
        throw new ArgumentException("Installation target should be specified");
      this.PrepareStoredProcedure("Gallery.prc_UpdateWeightedRating");
      this.BindString(nameof (product), product, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindStringTable(nameof (installationTargets), installationTargets, true);
      this.ExecuteNonQuery();
    }
  }
}
