// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexMetadataStateAnalyserFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class IndexMetadataStateAnalyserFactory
  {
    public virtual IndexMetadataStateAnalyser GetIndexMetadataStateAnalyser(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return (IndexMetadataStateAnalyser) new CodeIndexMetadataStateAnalyser(dataAccessFactory, indexingUnitChangeEventHandler);
        case "WorkItem":
          return (IndexMetadataStateAnalyser) new WorkItemIndexMetadataStateAnalyser(dataAccessFactory, indexingUnitChangeEventHandler);
        case "Wiki":
          return (IndexMetadataStateAnalyser) new WikiIndexMetadataStateAnalyser(dataAccessFactory, indexingUnitChangeEventHandler);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType {0} is not supported", (object) entityType.Name)));
      }
    }
  }
}
