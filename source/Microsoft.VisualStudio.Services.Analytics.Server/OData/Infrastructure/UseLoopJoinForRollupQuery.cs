// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.UseLoopJoinForRollupQuery
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class UseLoopJoinForRollupQuery : IHintStrategy
  {
    public SqlOptions GetOptions(
      Type entitySetType,
      QueryType queryType,
      IQueryable query,
      ODataQueryOptions odataQueryOptions)
    {
      return SqlOptions.LoopJoinForRollupHint;
    }
  }
}
