// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.IdentityComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  internal class IdentityComparer : IEqualityComparer<Microsoft.VisualStudio.Services.Identity.Identity>
  {
    public bool Equals(Microsoft.VisualStudio.Services.Identity.Identity x, Microsoft.VisualStudio.Services.Identity.Identity y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.Descriptor.Equals(y.Descriptor);
    }

    public int GetHashCode(Microsoft.VisualStudio.Services.Identity.Identity obj) => obj.Descriptor.GetHashCode();

    public static IdentityComparer Instance { get; } = new IdentityComparer();
  }
}
