// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.Components.TaggingComponent6
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Tags.Server.Components
{
  internal class TaggingComponent6 : TaggingComponent5
  {
    internal override bool SupportsPermissions => true;

    internal override void DeleteTagDefinitions(IEnumerable<Guid> tagIds)
    {
      this.PrepareStoredProcedure("prc_DeleteTagDefinitions");
      this.BindGuidTable("@tagIds", tagIds);
      this.ExecuteNonQuery();
    }
  }
}
