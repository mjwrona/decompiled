// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent41
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent41 : PublishedExtensionComponent40
  {
    public override void CreateBackConsolidationMapping(
      Guid sourceExtensionId,
      string sourceExtensionVsixId,
      Guid targetExtensionId,
      string targetExtensionVsixId)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateBackConsolidationMapping");
      this.BindGuid(nameof (sourceExtensionId), sourceExtensionId);
      this.BindString(nameof (sourceExtensionVsixId), sourceExtensionVsixId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindGuid(nameof (targetExtensionId), targetExtensionId);
      this.BindString(nameof (targetExtensionVsixId), targetExtensionVsixId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreateBackConsolidationMapping", this.RequestContext))
        resultCollection.AddBinder<BackConsolidationMappingEntry>((ObjectBinder<BackConsolidationMappingEntry>) new BackConsolidationBinder());
    }

    public override IReadOnlyDictionary<string, BackConsolidationMappingEntry> GetBackConsolidationMapping(
      IVssRequestContext requestContext)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetBackConsolidationMapping");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetBackConsolidationMapping", requestContext))
      {
        resultCollection.AddBinder<BackConsolidationMappingEntry>((ObjectBinder<BackConsolidationMappingEntry>) new BackConsolidationBinder());
        List<BackConsolidationMappingEntry> items = resultCollection.GetCurrent<BackConsolidationMappingEntry>().Items;
        IDictionary<string, BackConsolidationMappingEntry> consolidationMapping = (IDictionary<string, BackConsolidationMappingEntry>) new Dictionary<string, BackConsolidationMappingEntry>();
        foreach (BackConsolidationMappingEntry consolidationMappingEntry in items)
          consolidationMapping.Add(consolidationMappingEntry.SourceExtensionVsixId, consolidationMappingEntry);
        return (IReadOnlyDictionary<string, BackConsolidationMappingEntry>) consolidationMapping;
      }
    }
  }
}
