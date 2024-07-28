// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration.AggregationAccessorFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration
{
  public class AggregationAccessorFactoryBootstrapper : 
    IBootstrapper<IFactory<IAggregation, IAggregationAccessor>>
  {
    private readonly IVssRequestContext requestContext;

    public AggregationAccessorFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<IAggregation, IAggregationAccessor> Bootstrap() => (IFactory<IAggregation, IAggregationAccessor>) new ByFuncInputFactory<IAggregation, IAggregationAccessor>((Func<IAggregation, IAggregationAccessor>) (aggregation => aggregation.Bootstrap(this.requestContext)));
  }
}
