// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent11 : PropertyComponent10
  {
    private static readonly SqlMetaData[] typ_ArtifactPropertySpecCopyTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("ArtifactId", SqlDbType.VarBinary, 64L),
      new SqlMetaData("TargetArtifactId", SqlDbType.VarBinary, 64L),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("TargetDataspaceId", SqlDbType.Int)
    };

    internal override void CopyPropertyValues(
      IEnumerable<KeyValuePair<ArtifactSpec, ArtifactSpec>> artifactSpecs,
      ArtifactKind kind)
    {
      this.PrepareStoredProcedure("prc_pCopyPropertyValues2");
      this.BindGuid("@kind", kind.Kind);
      System.Func<KeyValuePair<ArtifactSpec, ArtifactSpec>, SqlDataRecord> selector = (System.Func<KeyValuePair<ArtifactSpec, ArtifactSpec>, SqlDataRecord>) (copyValue =>
      {
        ArtifactSpec key = copyValue.Key;
        ArtifactSpec artifactSpec = copyValue.Value;
        this.ValidateArtifactSpec(key, PropertiesOptions.None, kind);
        if (key.Kind != artifactSpec.Kind)
          throw new InvalidOperationException("Cannot change kinds during copy operation.");
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PropertyComponent11.typ_ArtifactPropertySpecCopyTable2);
        if (!string.IsNullOrEmpty(key.Moniker))
          throw new InvalidOperationException("Do not support copying using source spec moniker.");
        sqlDataRecord.SetBytes(0, 0L, key.Id, 0, key.Id.Length);
        if (!string.IsNullOrEmpty(artifactSpec.Moniker))
          throw new InvalidOperationException("Do not support copying using target spec moniker.");
        sqlDataRecord.SetBytes(1, 0L, artifactSpec.Id, 0, artifactSpec.Id.Length);
        sqlDataRecord.SetInt32(2, this.GetDataspaceId(key.DataspaceIdentifier));
        sqlDataRecord.SetInt32(3, this.GetDataspaceId(artifactSpec.DataspaceIdentifier));
        return sqlDataRecord;
      });
      this.BindTable("@propertySpecs", "typ_ArtifactPropertySpecCopyTable2", artifactSpecs.Select<KeyValuePair<ArtifactSpec, ArtifactSpec>, SqlDataRecord>(selector));
      this.ExecuteNonQuery();
    }
  }
}
