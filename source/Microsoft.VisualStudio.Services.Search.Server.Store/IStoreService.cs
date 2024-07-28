// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.IStoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.VisualStudio.Services.Search.Indexer;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  public interface IStoreService
  {
    void Run(IndexingExecutionContext indexingExecutionContext);
  }
}
