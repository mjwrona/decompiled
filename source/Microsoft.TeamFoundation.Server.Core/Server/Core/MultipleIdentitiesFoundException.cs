// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Server.Core
{
  [Serializable]
  public class MultipleIdentitiesFoundException : IdentityServiceException
  {
    public MultipleIdentitiesFoundException(
      string factorValue,
      TeamFoundationIdentity[] matchingIdentities)
      : base(MultipleIdentitiesFoundException.BuildExceptionMessage(factorValue, matchingIdentities))
    {
      this.FactorValue = factorValue;
      this.MatchingIdentities = matchingIdentities;
    }

    private static string BuildExceptionMessage(
      string factorValue,
      TeamFoundationIdentity[] matchingIdentities)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (TeamFoundationIdentity matchingIdentity in matchingIdentities)
        stringBuilder.AppendLine(FrameworkResources.MultipleIdentitiesFoundRow((object) matchingIdentity.DisplayName, (object) matchingIdentity.UniqueName));
      return TFCommonResources.MultipleIdentitiesFoundMessage((object) factorValue, (object) stringBuilder.ToString());
    }

    public string FactorValue { get; private set; }

    public TeamFoundationIdentity[] MatchingIdentities { get; private set; }
  }
}
