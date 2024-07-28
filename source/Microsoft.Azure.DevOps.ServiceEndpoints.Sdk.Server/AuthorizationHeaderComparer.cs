// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AuthorizationHeaderComparer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal sealed class AuthorizationHeaderComparer : IEqualityComparer<AuthorizationHeader>
  {
    public bool Equals(AuthorizationHeader x, AuthorizationHeader y)
    {
      if (x != null && y != null)
        return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
      return x == null && y == null;
    }

    public int GetHashCode(AuthorizationHeader obj) => obj?.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0;
  }
}
