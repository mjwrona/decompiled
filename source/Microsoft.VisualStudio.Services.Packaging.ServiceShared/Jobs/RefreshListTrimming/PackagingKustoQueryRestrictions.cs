// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming.PackagingKustoQueryRestrictions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming
{
  internal class PackagingKustoQueryRestrictions : KustoQueryRestriction
  {
    public PackagingKustoQueryRestrictions(
      IVssRequestContext requestContext,
      string[] requestedTables,
      KustoQueryConfig kustoQueryConfig)
      : base(requestContext, requestedTables, kustoQueryConfig)
    {
    }

    public override string Apply(string query) => KustoQueryHelper.Concat(this.m_restrictingStatements, query);

    protected override string BuildRestrictingStatements(
      IVssRequestContext requestContext,
      string[] requestedTables)
    {
      if (requestedTables.Length != 1 || requestedTables[0] != "PackagingTraces")
        throw new InvalidOperationException("Cannot query outside PackagingTraces table");
      return KustoQueryHelper.Concat((string) (KustoStatement) new KustoLetStatement("PackagingTraces", string.Format("view() {{ {0} }}", (object) new KustoTabularExpressionStatement("PackagingTraces", new string[1]
      {
        string.Format("where HostId == '{0:d}'", (object) requestContext.ServiceHost.InstanceId)
      }))), (string) (KustoStatement) new KustoRestrictStatement(new string[1]
      {
        "PackagingTraces"
      }));
    }
  }
}
