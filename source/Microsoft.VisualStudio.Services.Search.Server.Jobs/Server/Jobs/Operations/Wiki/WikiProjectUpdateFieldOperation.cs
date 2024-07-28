// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.WikiProjectUpdateFieldOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class WikiProjectUpdateFieldOperation : ProjectUpdateFieldOperation
  {
    public WikiProjectUpdateFieldOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    protected override bool UpdatePropertiesIfModified(IndexingExecutionContext executionContext)
    {
      TeamProject teamProjectFromTfs = this.GetTeamProjectFromTfs(executionContext);
      string projectName = (this.IndexingUnit.TFSEntityAttributes as ProjectCodeTFSAttributes).ProjectName;
      ProjectVisibility projectVisibility1 = (this.IndexingUnit.TFSEntityAttributes as ProjectCodeTFSAttributes).ProjectVisibility;
      ProjectVisibility projectVisibility2 = (this.IndexingUnit.Properties as ProjectCodeIndexingProperties).ProjectVisibility;
      bool flag = !teamProjectFromTfs.Name.Equals(projectName, StringComparison.Ordinal) || !projectName.Equals(this.IndexingUnit.Properties.Name, StringComparison.Ordinal);
      int num = (!teamProjectFromTfs.Visibility.Equals((object) projectVisibility1) ? 1 : (!projectVisibility1.Equals((object) projectVisibility2) ? 1 : 0)) | (flag ? 1 : 0);
      if (num == 0)
        return num != 0;
      this.IndexingUnit.Properties.Name = teamProjectFromTfs.Name;
      (this.IndexingUnit.Properties as ProjectCodeIndexingProperties).ProjectVisibility = teamProjectFromTfs.Visibility;
      return num != 0;
    }
  }
}
