// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.PipelineClaimHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  public static class PipelineClaimHelper
  {
    private const char SubTokenSeperator = ';';

    public static Claim GetBuildIdClaim(Guid projectId, int buildId) => new Claim("BuildId", projectId.ToString() + ";" + buildId.ToString());

    public static bool TryGetBuildId(
      this IEnumerable<Claim> claims,
      out Guid projectId,
      out int buildId)
    {
      projectId = Guid.Empty;
      buildId = 0;
      Claim claim = claims != null ? claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "BuildId")) : (Claim) null;
      if (claim != null)
      {
        if (!string.IsNullOrWhiteSpace(claim.Value))
        {
          try
          {
            string[] strArray = claim.Value.Split(';');
            return strArray.Length == 2 && Guid.TryParse(strArray[0], out projectId) && int.TryParse(strArray[1], out buildId);
          }
          catch
          {
            return false;
          }
        }
      }
      return false;
    }
  }
}
