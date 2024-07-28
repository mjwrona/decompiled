// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IdentityExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class IdentityExtensions
  {
    public static bool IsRequestor(this Microsoft.VisualStudio.Services.Identity.Identity identity, BuildData build)
    {
      if (identity == null || build == null)
        return false;
      bool flag = false;
      if (build.RequestedBy != Guid.Empty)
        flag = flag || object.Equals((object) build.RequestedBy, (object) identity.Id);
      if (!flag && build.RequestedFor != Guid.Empty)
        flag = flag || object.Equals((object) build.RequestedFor, (object) identity.Id);
      return flag;
    }
  }
}
