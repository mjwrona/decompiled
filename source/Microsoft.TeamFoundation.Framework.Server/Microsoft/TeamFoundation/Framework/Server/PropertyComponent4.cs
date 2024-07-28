// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent4 : PropertyComponent3
  {
    protected override ObjectBinder<DbArtifactPropertyValue> CreatePropertyValueBinder() => (ObjectBinder<DbArtifactPropertyValue>) new PropertyComponent.DbArtifactPropertyValueColumns2((PropertyComponent) this);

    internal override bool SetPropertyValue(
      IEnumerable<ArtifactPropertyValue> artifactPropertyValues,
      ArtifactKind kind,
      DateTime? changedDate,
      Guid? changedBy)
    {
      this.PrepareStoredProcedure("prc_SetPropertyValue");
      using (ArtifactPropertyValueDbPagingManager valueDbPagingManager = new ArtifactPropertyValueDbPagingManager(this.RequestContext, (PropertyComponent) this))
      {
        valueDbPagingManager.EnqueueAll(artifactPropertyValues);
        if (valueDbPagingManager.TotalCount == 0)
          return false;
        this.BindGuid("@kind", valueDbPagingManager.PagedArtifactKind.Kind);
        this.BindGuid("@author", this.Author);
        if (valueDbPagingManager.IsPaged)
          this.BindInt("@lobParam", valueDbPagingManager.ParameterId);
        else
          this.BindString("@propertyValueList", valueDbPagingManager.ServerItemWriter.ToString(), -1, false, SqlDbType.NVarChar);
        if (changedDate.HasValue)
          this.BindDateTime("@changedDate", changedDate.Value);
        if (changedBy.HasValue)
          this.BindGuid("@changedBy", changedBy.Value);
        this.ExecuteNonQuery();
        return true;
      }
    }
  }
}
