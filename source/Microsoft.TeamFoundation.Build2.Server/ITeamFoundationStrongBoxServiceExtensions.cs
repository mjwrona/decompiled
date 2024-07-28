// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ITeamFoundationStrongBoxServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ITeamFoundationStrongBoxServiceExtensions
  {
    public static bool TryGetStrongBoxValue(
      this ITeamFoundationStrongBoxService strongBoxService,
      IVssRequestContext requestContext,
      Guid drawerId,
      string key,
      out string value)
    {
      using (requestContext.TraceScope(nameof (ITeamFoundationStrongBoxServiceExtensions), nameof (TryGetStrongBoxValue)))
      {
        try
        {
          value = strongBoxService.GetString(requestContext, drawerId, key);
          return true;
        }
        catch (StrongBoxItemNotFoundException ex)
        {
          value = (string) null;
          return false;
        }
      }
    }
  }
}
