// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IQueryScopingCacheUpdater
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public interface IQueryScopingCacheUpdater
  {
    void UpdateQueryScopingCache(
      IVssRequestContext requestContext,
      QueryScopingCacheUpdateData changeData);
  }
}
