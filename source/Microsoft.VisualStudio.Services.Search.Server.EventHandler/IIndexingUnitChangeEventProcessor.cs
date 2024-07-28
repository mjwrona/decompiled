// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.IIndexingUnitChangeEventProcessor
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  [DefaultServiceImplementation(typeof (IndexingUnitChangeEventProcessor))]
  public interface IIndexingUnitChangeEventProcessor : IVssFrameworkService
  {
    void Process(ExecutionContext executionContext);

    void Process(ExecutionContext executionContext, int indexingUnitId);

    void Process(ExecutionContext executionContext, IEntityType entityType);
  }
}
