// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent7 : PropertyComponent6
  {
    internal override void CreateArtifactKind(ArtifactKind artifactKind)
    {
      this.PrepareStoredProcedure("prc_CreateArtifactKind");
      this.BindGuid("@kind", artifactKind.Kind);
      this.BindBoolean("@internal", artifactKind.IsInternalKind);
      this.BindString("@description", artifactKind.Description, 2000, true, SqlDbType.NVarChar);
      this.BindString("@dataspaceCategory", this.GetDataspaceCategory(artifactKind.DataspaceCategory), 256, false, SqlDbType.NVarChar);
      this.BindBoolean("@monikerBased", artifactKind.IsMonikerBased);
      this.BindInt("@flags", (int) artifactKind.Flags);
      this.ExecuteNonQuery();
    }

    protected override void BindDataspaces(Guid[] dataspaceIdentifiers)
    {
      if (dataspaceIdentifiers == null)
        dataspaceIdentifiers = Array.Empty<Guid>();
      this.BindUniqueInt32Table("@dataspaceIds", (IEnumerable<int>) this.GetDataspaceIds(dataspaceIdentifiers));
    }

    protected override ObjectBinder<DbArtifactPropertyValue> CreatePropertyValueBinder() => (ObjectBinder<DbArtifactPropertyValue>) new PropertyComponent.DbArtifactPropertyValueColumns3((PropertyComponent) this);

    protected override PropertyComponent.ArtifactSpecColumns BindColumns(Guid kind) => new PropertyComponent.ArtifactSpecColumns(kind, (PropertyComponent) this);

    protected override PropertyComponent.ArtifactKindColumns CreateArtifactKindColumns() => (PropertyComponent.ArtifactKindColumns) new PropertyComponent.ArtifactKindColumns3();
  }
}
