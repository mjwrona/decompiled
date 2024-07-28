// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceCommons
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class CommerceCommons
  {
    public const string HttpHeaderRequestId = "x-ms-request-id";
    public const string HttpHeaderDate = "Date";

    internal static void SetRequestContextUniqueIdentifier(
      IVssRequestContext requestContext,
      string uniqueIdentifier)
    {
      Guid result;
      if (!Guid.TryParse(uniqueIdentifier, out result) || !(result != Guid.Empty))
        return;
      PropertyInfo property = requestContext.GetType().GetProperty("UniqueIdentifier");
      if ((object) property == null || !property.CanWrite)
        return;
      property.SetValue((object) requestContext, (object) result);
    }

    internal static string GetRdfeOperationId(HttpRequestMessage request)
    {
      IEnumerable<string> values;
      request.Headers.TryGetValues("x-ms-request-id", out values);
      return values == null ? (string) null : values.FirstOrDefault<string>();
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity GetPrimaryMsaIdentityHelper(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return !collectionContext.IsOrganizationAadBacked() ? collectionContext.GetService<IdentityService>().GetPrimaryMsaIdentity(collectionContext, (IReadOnlyVssIdentity) identity) : identity;
    }

    public static List<string> EscapeUriDataSet(params string[] values) => ((IEnumerable<string>) values).Select<string, string>((Func<string, string>) (v => Uri.EscapeDataString(v))).ToList<string>();
  }
}
