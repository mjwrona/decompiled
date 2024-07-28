// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMapComponent3
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMapComponent3 : IdentityMapComponent2
  {
    internal override void UpdateIdentityMappings2(IEnumerable<KeyValuePair<Guid, Guid>> mappings)
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.PrepareStoredProcedure("prc_UpdateIdentityMap");
      this.BindKeyValuePairGuidGuidTable("@mappings", mappings);
      this.ExecuteNonQuery();
    }
  }
}
