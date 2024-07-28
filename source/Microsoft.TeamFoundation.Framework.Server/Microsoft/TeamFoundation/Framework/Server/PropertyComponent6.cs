// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PropertyComponent6 : PropertyComponent5
  {
    internal override int DeleteProperties(
      Guid artifactKind,
      IEnumerable<string> propertyNames,
      int batchSize = 2000,
      int? maxPropertiesToDelete = null)
    {
      if (maxPropertiesToDelete.HasValue)
      {
        int? nullable = maxPropertiesToDelete;
        int num = 0;
        if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          throw new NotSupportedException();
      }
      this.PrepareStoredProcedure("prc_DeleteProperties");
      this.BindGuidTable("@artifactKinds", (IEnumerable<Guid>) new Guid[1]
      {
        artifactKind
      });
      this.BindStringTable("@propertyNames", propertyNames);
      this.ExecuteNonQuery();
      return -1;
    }
  }
}
