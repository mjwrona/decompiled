// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.ProviderAlreadyPresentException
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public class ProviderAlreadyPresentException : VssServiceException
  {
    public ProviderAlreadyPresentException(
      IEnumerable<string> providerFullNames,
      string providerType)
      : base(ProviderAlreadyPresentException.GetErrorMessage(providerFullNames, providerType))
    {
    }

    public ProviderAlreadyPresentException(IEnumerable<string> providerFullNames, Guid providerId)
      : base(ProviderAlreadyPresentException.GetErrorMessage(providerFullNames, providerId.ToString()))
    {
    }

    private static string GetErrorMessage(IEnumerable<string> names, string provider)
    {
      string str = string.Join(",", names.ToArray<string>());
      return string.Format("ActivityType-" + provider + " has multiple providers namely: " + str);
    }
  }
}
