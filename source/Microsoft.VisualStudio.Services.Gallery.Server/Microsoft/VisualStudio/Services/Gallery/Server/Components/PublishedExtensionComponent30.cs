// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent30
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent30 : PublishedExtensionComponent29
  {
    public virtual ExtensionsByInstallationTargetsResult QueryExtensionsForCacheBuilding(
      List<InstallationTarget> installationTargets,
      int pageNumber,
      int pageSize,
      ExtensionQueryFlags flags)
    {
      string str = "Gallery.prc_QueryExtensionsForCacheBuilding";
      this.PrepareStoredProcedure(str);
      this.BindExtensionInstallationTargetTable(nameof (installationTargets), (IEnumerable<InstallationTarget>) installationTargets);
      this.BindInt(nameof (pageNumber), pageNumber);
      this.BindInt(nameof (pageSize), pageSize);
      this.BindInt(nameof (flags), (int) flags);
      ExtensionsByInstallationTargetsResult installationTargetsResult = new ExtensionsByInstallationTargetsResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        Dictionary<Guid, PublishedExtension> dictionary = this.ProcessExtensionResult(resultCollection, flags);
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ResultCountBinder());
        resultCollection.NextResult();
        installationTargetsResult.TotalCount = resultCollection.GetCurrent<int>().Items[0];
        installationTargetsResult.Extensions = dictionary.Values.ToList<PublishedExtension>();
        return installationTargetsResult;
      }
    }
  }
}
