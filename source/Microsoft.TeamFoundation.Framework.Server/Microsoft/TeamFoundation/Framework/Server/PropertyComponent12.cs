// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent12
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent12 : PropertyComponent11
  {
    internal override int DeleteProperties(
      Guid artifactKind,
      IEnumerable<string> propertyNames,
      int batchSize = 2000,
      int? maxPropertiesToDelete = null)
    {
      this.PrepareStoredProcedure("prc_DeleteProperties");
      this.BindGuid("@kindId", artifactKind);
      this.BindStringTable("@propertyNames", propertyNames);
      this.BindInt("@batchSize", batchSize);
      if (maxPropertiesToDelete.HasValue)
      {
        int? nullable = maxPropertiesToDelete;
        int num = 0;
        if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          this.BindInt("@maxPropertiesToDelete", maxPropertiesToDelete.Value);
      }
      return (int) this.ExecuteScalar();
    }
  }
}
