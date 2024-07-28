// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.IdentityRefExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class IdentityRefExtensions
  {
    public static Guid? ToIdentityGuid(this IdentityRef identityRef)
    {
      Guid? identityGuid = new Guid?();
      Guid result;
      if (identityRef != null && !string.IsNullOrEmpty(identityRef.Id) && Guid.TryParse(identityRef.Id, out result))
        identityGuid = new Guid?(result);
      return identityGuid;
    }

    public static Guid ToIdentityGuid(
      IdentityRef identityRef,
      IVssRequestContext requestContext,
      IdentityMap identityMap)
    {
      Guid result = Guid.Empty;
      if (identityRef != null && !string.IsNullOrEmpty(identityRef.Id) && !Guid.TryParse(identityRef.Id, out result))
      {
        IdentityRef identityRef1 = identityMap.GetIdentityRef(requestContext, identityRef.Id);
        if (identityRef1 != null)
          result = Guid.Parse(identityRef1.Id);
      }
      return result;
    }
  }
}
