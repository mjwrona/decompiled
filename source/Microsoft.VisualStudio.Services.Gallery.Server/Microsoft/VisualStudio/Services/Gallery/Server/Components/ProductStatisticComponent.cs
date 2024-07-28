// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ProductStatisticComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ProductStatisticComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "ProductStatisticComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ProductStatisticComponent>(0, true),
      (IComponentCreator) new ComponentCreator<ProductStatisticComponent1>(1)
    }, "ProductStatistic");

    static ProductStatisticComponent() => ProductStatisticComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ProductStatisticComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (ProductStatisticComponent);
  }
}
