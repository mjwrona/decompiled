// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.WikiIndexProvisionerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class WikiIndexProvisionerFactory : IndexProvisionerFactory
  {
    protected override IEntityType EntityType => (IEntityType) WikiEntityType.GetInstance();

    public override IIndexProvisioner GetIndexProvisioner(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit)
    {
      ArgumentUtility.CheckForNull<IndexingUnit>(indexingUnit, nameof (indexingUnit));
      string indexingUnitType = indexingUnit.IndexingUnitType;
      if (indexingUnitType == "Collection" || indexingUnitType == "Project" || indexingUnitType == "Git_Repository" || indexingUnitType == "Organization")
        return this.GetDefaultIndexProvisioner(indexingExecutionContext);
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported IndexingUnit '{0}' for {1}", (object) indexingUnit, (object) typeof (WikiIndexProvisionerFactory))));
    }
  }
}
