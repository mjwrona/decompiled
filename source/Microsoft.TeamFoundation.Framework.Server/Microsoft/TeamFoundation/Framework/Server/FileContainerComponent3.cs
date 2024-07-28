// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent3 : FileContainerComponent2
  {
    public FileContainerComponent3() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override List<FileContainerItem> QuerySpecificItems(
      long containerId,
      IEnumerable<string> paths,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (QuerySpecificItems));
      this.PrepareStoredProcedure("prc_QueryContainerItems2");
      this.BindLong("@containerId", containerId);
      this.BindStringTable("@pathTable", paths.Select<string, string>((System.Func<string, string>) (path => DBPath.UserToDatabasePath(path, true, true))));
      this.BindDataspace(dataspaceIdentifier);
      List<FileContainerItem> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileContainerItem>((ObjectBinder<FileContainerItem>) this.GetFileContainerItemBinder());
        items = resultCollection.GetCurrent<FileContainerItem>().Items;
      }
      this.TraceLeave(0, nameof (QuerySpecificItems));
      return items;
    }

    public override List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> FilterContainers(
      string artifactUriFilter,
      Guid? dataspaceIdentifier)
    {
      return this.FilterContainers((ObjectBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer>) this.GetFileContainerBinder(), artifactUriFilter, dataspaceIdentifier);
    }

    public override List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> FilterContainers(
      ObjectBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer> binder,
      string artifactUriFilter,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (FilterContainers));
      this.PrepareStoredProcedure("prc_QueryContainersByUri");
      this.BindString("@artifactUriFilter", artifactUriFilter, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDataspace(dataspaceIdentifier);
      List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer>(binder);
        items = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.FileContainer.FileContainer>().Items;
      }
      this.TraceLeave(0, nameof (FilterContainers));
      return items;
    }
  }
}
