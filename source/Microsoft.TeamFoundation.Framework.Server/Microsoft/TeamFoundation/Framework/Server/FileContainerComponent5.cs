// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent5
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
  internal class FileContainerComponent5 : FileContainerComponent4
  {
    public override List<FileContainerItem> CopyFiles(
      long containerId,
      List<KeyValuePair<string, string>> sourcesAndTargets,
      bool deleteSources,
      bool ignoreMissingSources,
      bool overwriteTargets,
      Guid dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (CopyFiles));
      this.PrepareStoredProcedure("prc_CopyContainerItemFiles");
      this.BindLong("@containerId", containerId);
      this.BindKeyValuePairStringTable("@sourcesAndTargets", sourcesAndTargets.Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((System.Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (x => new KeyValuePair<string, string>(DBPath.UserToDatabasePath(x.Key, true, true), DBPath.UserToDatabasePath(x.Value, true, true)))));
      this.BindGuid("@createdBy", this.Author);
      this.BindBoolean("@deleteSources", deleteSources);
      this.BindBoolean("@ignoreMissingSources", ignoreMissingSources);
      this.BindBoolean("@overwriteTargets", overwriteTargets);
      this.BindDataspace(dataspaceIdentifier);
      List<FileContainerItem> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileContainerItem>((ObjectBinder<FileContainerItem>) this.GetFileContainerItemBinder());
        items = resultCollection.GetCurrent<FileContainerItem>().Items;
      }
      this.TraceLeave(0, nameof (CopyFiles));
      return items;
    }
  }
}
