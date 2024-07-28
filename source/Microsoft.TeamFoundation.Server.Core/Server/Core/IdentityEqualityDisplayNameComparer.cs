// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityEqualityDisplayNameComparer
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class IdentityEqualityDisplayNameComparer : IEqualityComparer<Microsoft.VisualStudio.Services.Identity.Identity>
  {
    public bool Equals(Microsoft.VisualStudio.Services.Identity.Identity x, Microsoft.VisualStudio.Services.Identity.Identity y) => new IdentityDisplayNameComparer().Compare(x, y) == 0;

    public int GetHashCode(Microsoft.VisualStudio.Services.Identity.Identity obj) => obj.DisplayName.GetHashCode();
  }
}
