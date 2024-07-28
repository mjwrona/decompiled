// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.UriBuilderHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class UriBuilderHelper : RestApiHelper
  {
    private const string c_mtmScheme = "mtm";
    private const string c_mtmSecureScheme = "mtms";
    private const string c_testPlanUriFormat = "{0}p:{1}/Testing/testplan/connect";
    private const string c_planQueryString = "id={0}";

    public UriBuilderHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public string GetPlanUri(int planId, string projectName)
    {
      UriBuilder uriBuilder = new UriBuilder(this.GetCollectionUrl());
      uriBuilder.Scheme = !string.Equals(uriBuilder.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? "mtm" : "mtms";
      uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}p:{1}/Testing/testplan/connect", (object) uriBuilder.Path, (object) projectName);
      uriBuilder.Query = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "id={0}", (object) planId);
      return uriBuilder.ToString();
    }

    private string GetCollectionUrl() => this.RequestContext.GetService<ILocationService>().GetLocationServiceUrl(this.RequestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.ClientAccessMappingMoniker);
  }
}
