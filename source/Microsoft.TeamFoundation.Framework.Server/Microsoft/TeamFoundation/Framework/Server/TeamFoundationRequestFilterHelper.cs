// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationRequestFilterHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class TeamFoundationRequestFilterHelper
  {
    public static void PostAuthorizeRequest(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.ServiceHostInternal().RequestFilters == null)
        return;
      foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) requestContext.ServiceHost.ServiceHostInternal().RequestFilters)
      {
        requestContext.Trace(41523740, TraceLevel.Verbose, nameof (TeamFoundationRequestFilterHelper), nameof (PostAuthorizeRequest), "Calling PostAuthorize of the request filter {0}.", (object) requestFilter);
        try
        {
          requestFilter.PostAuthorizeRequest(requestContext);
        }
        catch (RequestFilterException ex)
        {
          requestContext.TraceException(42753800, nameof (TeamFoundationRequestFilterHelper), nameof (PostAuthorizeRequest), (Exception) ex);
          throw;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(27815684, nameof (TeamFoundationRequestFilterHelper), nameof (PostAuthorizeRequest), ex);
        }
      }
    }
  }
}
