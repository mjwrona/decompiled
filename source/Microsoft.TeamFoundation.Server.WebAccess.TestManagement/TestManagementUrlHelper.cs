// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementUrlHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestManagementUrlHelper
  {
    public static string GetTfsBaseUrl(IVssRequestContext requestContext)
    {
      string tfsBaseUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
      if (tfsBaseUrl == null)
      {
        string absoluteUri = requestContext.RequestUri().AbsoluteUri;
        tfsBaseUrl = absoluteUri.Substring(0, absoluteUri.IndexOf(HttpRouteCollectionExtensions.DefaultRoutePrefix));
      }
      return tfsBaseUrl;
    }
  }
}
