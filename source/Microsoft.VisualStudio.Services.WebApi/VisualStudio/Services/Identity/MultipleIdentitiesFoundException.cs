// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MultipleIdentitiesFoundException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ExceptionMapping("0.0", "3.0", "MultipleIdentitiesFoundException", "Microsoft.VisualStudio.Services.Identity.MultipleIdentitiesFoundException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class MultipleIdentitiesFoundException : IdentityServiceException
  {
    public MultipleIdentitiesFoundException(
      string identityName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> matchingIdentities)
      : base(MultipleIdentitiesFoundException.BuildExceptionMessage(identityName, (IEnumerable<IReadOnlyVssIdentity>) matchingIdentities))
    {
    }

    public MultipleIdentitiesFoundException(
      string identityName,
      IEnumerable<IReadOnlyVssIdentity> matchingIdentities)
      : base(MultipleIdentitiesFoundException.BuildExceptionMessage(identityName, matchingIdentities))
    {
    }

    public MultipleIdentitiesFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private static string BuildExceptionMessage(
      string identityName,
      IEnumerable<IReadOnlyVssIdentity> matchingIdentities)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (IReadOnlyVssIdentity matchingIdentity in matchingIdentities)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, "- {0} ({1})", (object) matchingIdentity.ProviderDisplayName, (object) matchingIdentity.CustomDisplayName);
      return IdentityResources.MultipleIdentitiesFoundError((object) identityName, (object) stringBuilder.ToString());
    }
  }
}
