// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent26
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent26 : PublishedExtensionComponent25
  {
    protected override void ProcessExtensionLcids(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeLcids))
        return;
      resultCollection.AddBinder<ExtensionLcidRow>((ObjectBinder<ExtensionLcidRow>) new ExtensionLcidBinder());
      resultCollection.NextResult();
      foreach (ExtensionLcidRow extensionLcidRow in resultCollection.GetCurrent<ExtensionLcidRow>().Items)
      {
        PublishedExtension publishedExtension;
        if (extensionMap.TryGetValue(extensionLcidRow.ExtensionId, out publishedExtension))
        {
          if (publishedExtension.Lcids == null)
            publishedExtension.Lcids = new List<int>();
          publishedExtension.Lcids.Add(extensionLcidRow.Lcid);
        }
        else
          TeamFoundationTracingService.TraceRaw(12062071, TraceLevel.Error, "Gallery", nameof (ProcessExtensionLcids), string.Format("No key found for Extension : {0} while processing extension lcids", (object) extensionLcidRow.ExtensionId));
      }
    }
  }
}
