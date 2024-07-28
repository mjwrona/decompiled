// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry.AuthenticateOptionsParser
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry
{
  internal class AuthenticateOptionsParser : IAuthenticateOptionsParser
  {
    public IDictionary<string, string> Parse(string wwwAuthenticateHeader)
    {
      if (string.IsNullOrEmpty(wwwAuthenticateHeader))
        return (IDictionary<string, string>) new Dictionary<string, string>();
      return (IDictionary<string, string>) ((IEnumerable<string>) wwwAuthenticateHeader.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (p => p.Trim())).Select<string, string[]>((Func<string, string[]>) (p => p.Split(new char[1]
      {
        '='
      }, StringSplitOptions.RemoveEmptyEntries))).Where<string[]>((Func<string[], bool>) (p => p.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (p => p[0]), (Func<string[], string>) (p => p[1].Replace("\"", string.Empty)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
