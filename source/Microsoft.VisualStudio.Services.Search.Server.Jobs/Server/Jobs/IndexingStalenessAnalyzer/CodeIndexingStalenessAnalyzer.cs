// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer.CodeIndexingStalenessAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer
{
  public abstract class CodeIndexingStalenessAnalyzer
  {
    protected abstract IEntityType EntityType { get; }

    protected abstract string IndexingUnitType { get; }

    protected IIndexingUnitDataAccess m_indexingUnitDataAccess { private set; get; }

    protected CodeIndexingStalenessAnalyzer(IIndexingUnitDataAccess indexingUnitDataAccess) => this.m_indexingUnitDataAccess = indexingUnitDataAccess ?? throw new ArgumentNullException(nameof (indexingUnitDataAccess));

    public abstract string AnalyzeCodeIndexingStaleness(ExecutionContext executionContext);

    public List<IndexingUnit> GetIndexingUnits(ExecutionContext executionContext) => this.m_indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, this.IndexingUnitType, this.EntityType, -1);
  }
}
