// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent3 : PropertyComponent2
  {
    internal override void CreateArtifactKind(ArtifactKind artifactKind)
    {
      this.PrepareStoredProcedure("prc_CreateArtifactKind");
      this.BindGuid("@kind", artifactKind.Kind);
      this.BindBoolean("@internal", artifactKind.IsInternalKind);
      this.BindString("@description", artifactKind.Description, 2000, true, SqlDbType.NVarChar);
      this.BindString("@databaseCategory", artifactKind.DataspaceCategory, 256, false, SqlDbType.NVarChar);
      this.BindBoolean("@monikerBased", artifactKind.IsMonikerBased);
      this.BindInt("@flags", (int) artifactKind.Flags);
      this.ExecuteNonQuery();
    }

    internal override List<ArtifactKind> GetPropertyKinds()
    {
      this.PrepareStoredProcedure("prc_GetPropertyKinds");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetPropertyKinds", this.RequestContext);
      resultCollection.AddBinder<ArtifactKind>((ObjectBinder<ArtifactKind>) this.CreateArtifactKindColumns());
      return resultCollection.GetCurrent<ArtifactKind>().Items;
    }

    internal override void SetArtifactKindFlags(
      Guid artifactKindId,
      ArtifactKindFlags onFlags,
      ArtifactKindFlags offFlags)
    {
      this.PrepareStoredProcedure("prc_SetArtifactKindFlags");
      this.BindGuid("@kind", artifactKindId);
      this.BindInt("@onFlags", (int) onFlags);
      this.BindInt("@offFlags", (int) offFlags);
      this.ExecuteNonQuery();
    }

    protected override PropertyComponent.ArtifactKindColumns CreateArtifactKindColumns() => (PropertyComponent.ArtifactKindColumns) new PropertyComponent.ArtifactKindColumns2();
  }
}
