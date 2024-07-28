// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.ProjectProcessConfigurationComponent6
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.Tvps;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class ProjectProcessConfigurationComponent6 : ProjectProcessConfigurationComponent5
  {
    internal override IReadOnlyCollection<ProjectGuidWatermarkPair> GetChangedStateProjectsSinceWatermark(
      int watermark)
    {
      this.PrepareStoredProcedure("prc_GetChangedStatesProjectIds");
      this.BindInt("@watermark", watermark);
      List<DataspaceIdWatermarkPairTypeRow> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DataspaceIdWatermarkPairTypeRow>((ObjectBinder<DataspaceIdWatermarkPairTypeRow>) new DataspaceIdWatermarkPairTypeRowBinder());
        items = resultCollection.GetCurrent<DataspaceIdWatermarkPairTypeRow>().Items;
      }
      List<ProjectGuidWatermarkPair> projectsSinceWatermark = new List<ProjectGuidWatermarkPair>(items.Count);
      foreach (DataspaceIdWatermarkPairTypeRow watermarkPairTypeRow in items)
        projectsSinceWatermark.Add(new ProjectGuidWatermarkPair()
        {
          ProjectGuid = this.GetDataspaceIdentifier(watermarkPairTypeRow.DataspaceId),
          MaxWatermark = watermarkPairTypeRow.Watermark
        });
      return (IReadOnlyCollection<ProjectGuidWatermarkPair>) projectsSinceWatermark;
    }
  }
}
