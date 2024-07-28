// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemComponent27
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionedItemComponent27 : VersionedItemComponent26
  {
    public override ResultCollection QueryItemsPaged(
      ItemSpec scopePath,
      int changesetId,
      ItemSpec lastItem,
      int top,
      int options)
    {
      this.PrepareStoredProcedure("Tfvc.prc_QueryItemsPaged", 3600);
      this.BindServerItem("@scopeServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(scopePath.ItemPathPair), false);
      this.BindInt("@changeSetId", changesetId);
      if (lastItem != null)
        this.BindServerItem("@lastServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(lastItem.ItemPathPair), true);
      this.BindInt("@top", top);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Item>((ObjectBinder<Item>) this.CreateQueryItemsColumns());
      return resultCollection;
    }

    public override ResultCollection QueryItemsByChangesetPaged(
      ItemSpec scopePath,
      int baseChangesetId,
      int targetChangesetId,
      ItemSpec lastItem,
      int top)
    {
      this.PrepareStoredProcedure("Tfvc.prc_QueryItemsByChangesetPaged", 3600);
      this.BindServerItem("@scopeServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(scopePath.ItemPathPair), false);
      this.BindInt("@baseChangeSetId", baseChangesetId);
      this.BindInt("@targetChangeSetId", targetChangesetId);
      if (lastItem != null)
        this.BindServerItem("@lastServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(lastItem.ItemPathPair), true);
      this.BindInt("@top", top);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<PreviousHashItem>((ObjectBinder<PreviousHashItem>) this.CreateQueryItemsByChangesetColumns());
      return resultCollection;
    }
  }
}
