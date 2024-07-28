// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.FavoriteMetadataHelper
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.Favorites
{
  public class FavoriteMetadataHelper
  {
    public static string GetLinkBaseUrl(IVssRequestContext requestContext)
    {
      string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
      return requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, accessMappingMoniker);
    }

    public static void PublishFeatureMetadataFailure(
      IVssRequestContext requestContext,
      string providerName,
      Exception e)
    {
      requestContext.TraceException(30000000, "Tfs Favorites", providerName, e);
    }
  }
}
