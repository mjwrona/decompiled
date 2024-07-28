// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PublicAuthUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class PublicAuthUtils
  {
    public static void AddAuthHeadersToResponse(
      IVssRequestContext requestContext,
      HttpResponseMessage response)
    {
      ITeamFoundationAuthenticationService service = requestContext.GetService<ITeamFoundationAuthenticationService>();
      service.AddTenantInfoResponseHeader(requestContext, response);
      service.AddFederatedAuthHeaders(requestContext, response);
    }
  }
}
