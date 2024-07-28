// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.IdentityScopeInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  internal class IdentityScopeInfo
  {
    public Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> InScopeIdentitiesMap { get; set; }

    public HashSet<Guid> NotInScopeIds { get; set; }

    public IdentityScopeInfo()
    {
      this.InScopeIdentitiesMap = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      this.NotInScopeIds = new HashSet<Guid>();
    }

    public static IdentityScopeInfo Merge(
      IdentityScopeInfo identityInfo1,
      IdentityScopeInfo identityInfo2)
    {
      return new IdentityScopeInfo()
      {
        NotInScopeIds = new HashSet<Guid>(identityInfo1.NotInScopeIds.Union<Guid>((IEnumerable<Guid>) identityInfo2.NotInScopeIds)),
        InScopeIdentitiesMap = identityInfo1.InScopeIdentitiesMap.Union<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>) identityInfo2.InScopeIdentitiesMap).ToDictionary<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Guid>) (k => k.Key), (Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (v => v.Value))
      };
    }
  }
}
