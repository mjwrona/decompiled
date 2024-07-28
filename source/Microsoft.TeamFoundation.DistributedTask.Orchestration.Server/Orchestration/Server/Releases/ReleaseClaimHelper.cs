// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Releases.ReleaseClaimHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Releases
{
  public static class ReleaseClaimHelper
  {
    public const string ReleaseIdClaimName = "ReleaseId";
    private const char ReleaseIdClaimSeperator = ':';
    private const string ReleaseIdCustomClaimValueFormat = "{0}:{1}";

    public static KeyValuePair<string, string> GetReleaseIdClaim(Guid projectId, int releaseId) => new KeyValuePair<string, string>("ReleaseId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseId));

    public static bool TryGetRelease(
      this IEnumerable<Claim> claims,
      out Guid projectId,
      out int releaseId)
    {
      projectId = Guid.Empty;
      releaseId = 0;
      Claim claim = claims != null ? claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "ReleaseId")) : (Claim) null;
      if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
        return false;
      string[] strArray = claim.Value.Split(':');
      return strArray.Length == 2 && Guid.TryParse(strArray[0], out projectId) && int.TryParse(strArray[1], out releaseId);
    }
  }
}
