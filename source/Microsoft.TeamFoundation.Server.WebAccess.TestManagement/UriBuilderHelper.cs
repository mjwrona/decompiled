// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.UriBuilderHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class UriBuilderHelper : TestHelperBase
  {
    private const string c_mtmScheme = "mtm";
    private const string c_mtmSecureScheme = "mtms";
    private const string c_testPlanUriFormat = "{0}/p:{1}/Testing/testplan/connect";
    private const string c_planQueryString = "id={0}";

    internal UriBuilderHelper(TestManagerRequestContext testContext)
      : base(testContext)
    {
    }

    public string GetPlanUri(int planId) => this.GetPlanUri(this.InitializeUriBuilder(), planId);

    public UriBuilder InitializeUriBuilder()
    {
      string collectionUrl = UriBuilderHelper.GetCollectionUrl(this.TestContext.TfsRequestContext);
      string projectName = this.TestContext.ProjectName;
      UriBuilder uriBuilder = new UriBuilder(collectionUrl);
      uriBuilder.Scheme = !string.Equals(uriBuilder.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? "mtm" : "mtms";
      uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/p:{1}/Testing/testplan/connect", (object) uriBuilder.Path, (object) projectName);
      return uriBuilder;
    }

    public string GetPlanUri(UriBuilder builder, int planId)
    {
      builder.Query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "id={0}", (object) planId);
      return Uri.UnescapeDataString(builder.ToString());
    }

    public static string GetCollectionUrl(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
  }
}
