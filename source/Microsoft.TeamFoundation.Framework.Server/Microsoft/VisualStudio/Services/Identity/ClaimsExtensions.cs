// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ClaimsExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class ClaimsExtensions
  {
    public static object[] Traceable(this IEnumerable<Claim> claims) => claims == null ? (object[]) null : claims.Select<Claim, object>(ClaimsExtensions.\u003C\u003EO.\u003C0\u003E__Traceable ?? (ClaimsExtensions.\u003C\u003EO.\u003C0\u003E__Traceable = new Func<Claim, object>(ClaimsExtensions.Traceable))).ToArray<object>();

    public static object Traceable(this Claim claim) => claim != null ? (object) new
    {
      Type = claim.Type,
      Value = claim.Value,
      ValueType = claim.ValueType
    } : (object) null;
  }
}
