// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.S2SUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class S2SUtility
  {
    public static void AddOAuthS2SAudiences(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      ITFLogger logger,
      string audiences)
    {
      if (string.IsNullOrEmpty(audiences))
        return;
      List<string> list = ((IEnumerable<string>) registryService.GetValue(requestContext, (RegistryQuery) OAuth2RegistryConstants.AllowedAudiences, string.Empty).Split(new char[1]
      {
        '|'
      }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      string str1 = audiences;
      char[] separator = new char[1]{ '|' };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        string audience = str2;
        if (!list.Any<string>((Func<string, bool>) (x => x.Equals(audience, StringComparison.OrdinalIgnoreCase))))
          list.Add(audience);
      }
      string str3 = string.Join("|", (IEnumerable<string>) list);
      logger?.Info("Setting registry path {0} with value {1}", (object) OAuth2RegistryConstants.AllowedAudiences, (object) str3);
      registryService.SetValue<string>(requestContext, OAuth2RegistryConstants.AllowedAudiences, str3);
    }
  }
}
