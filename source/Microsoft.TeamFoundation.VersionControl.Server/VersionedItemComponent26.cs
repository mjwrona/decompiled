// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemComponent26
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionedItemComponent26 : VersionedItemComponent25
  {
    public override ResultCollection FixCorruption()
    {
      this.PrepareStoredProcedure("prc_FixTfvcCorruption", 0);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<CorruptionResult>((ObjectBinder<CorruptionResult>) new CorruptionResultColumns());
      resultCollection.AddBinder<CorruptionResult>((ObjectBinder<CorruptionResult>) new CorruptionResultColumns());
      return resultCollection;
    }

    public override ResultCollection QueryItemsPaged(
      ItemSpec scopePath,
      int changesetId,
      ItemSpec lastItem,
      int top,
      int options)
    {
      return this.QueryItemsPaged(scopePath, changesetId, lastItem, options);
    }
  }
}
