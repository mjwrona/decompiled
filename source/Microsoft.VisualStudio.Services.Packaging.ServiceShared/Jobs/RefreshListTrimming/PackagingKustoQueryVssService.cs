// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming.PackagingKustoQueryVssService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming
{
  public class PackagingKustoQueryVssService : 
    KustoQueryServiceBase,
    IPackagingKustoQueryVssService,
    IVssFrameworkService
  {
    protected override string Layer => nameof (PackagingKustoQueryVssService);

    public override void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<T> QueryPackagingTraces<T>(IVssRequestContext requestContext, string query) where T : class => this.Query<T>(requestContext, new string[1]
    {
      "PackagingTraces"
    }, query, KustoQueryConfig.DefaultConfig);

    protected override KustoQueryRestriction GetQueryRestrictions(
      IVssRequestContext requestContext,
      string[] requestedTables)
    {
      return (KustoQueryRestriction) new PackagingKustoQueryRestrictions(requestContext, requestedTables, KustoQueryConfig.DefaultConfig);
    }
  }
}
