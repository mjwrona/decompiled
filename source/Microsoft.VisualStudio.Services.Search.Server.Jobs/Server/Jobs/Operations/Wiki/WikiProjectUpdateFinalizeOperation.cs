// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.WikiProjectUpdateFinalizeOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class WikiProjectUpdateFinalizeOperation : ProjectUpdateFinalizeOperation
  {
    public WikiProjectUpdateFinalizeOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    internal override void FinalizeTfsAttributes()
    {
      ((ProjectCodeTFSAttributes) this.IndexingUnit.TFSEntityAttributes).ProjectName = this.IndexingUnit.Properties.Name;
      ((ProjectCodeTFSAttributes) this.IndexingUnit.TFSEntityAttributes).ProjectVisibility = (this.IndexingUnit.Properties as ProjectCodeIndexingProperties).ProjectVisibility;
    }
  }
}
