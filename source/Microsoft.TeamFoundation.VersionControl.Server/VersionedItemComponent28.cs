// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemComponent28
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionedItemComponent28 : VersionedItemComponent27
  {
    public override List<Changeset> QueryChangesetRange(
      IEnumerable<Mapping> mappings,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      bool includeFiles,
      int maxChangesets)
    {
      this.PrepareStoredProcedure("prc_QueryChangesetRange", 3600);
      List<ItemSpec> rows1 = new List<ItemSpec>();
      List<ItemSpec> rows2 = new List<ItemSpec>();
      foreach (Mapping mapping in mappings)
      {
        if (mapping.Type == WorkingFolderType.Cloak)
          rows2.Add(new ItemSpec(mapping.ServerItem, (RecursionType) mapping.Depth));
        else
          rows1.Add(new ItemSpec(mapping.ServerItem, (RecursionType) mapping.Depth));
      }
      this.BindServiceDataspaceId("@rootItemDataspaceId");
      this.BindItemSpecTable("@includePaths", (IEnumerable<ItemSpec>) rows1);
      this.BindItemSpecTable("@excludePaths", (IEnumerable<ItemSpec>) rows2);
      this.BindVersionSpec("@versionSpecFrom", versionFrom, false);
      this.BindVersionSpec("@versionSpecTo", versionTo, false);
      this.BindBoolean("@includeFiles", includeFiles);
      this.BindInt("@maxChangeSets", maxChangesets);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      return resultCollection.GetCurrent<Changeset>().Items;
    }
  }
}
