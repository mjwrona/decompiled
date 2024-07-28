// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQueryWarningsValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataQueryWarningsValidator : IODataValidator
  {
    public string RequiredFeatureFlag => (string) null;

    public Action WarningCallback { get; set; }

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      ODataQueryClassifier odataQueryClassifier = new ODataQueryClassifier(queryOptions);
      if (odataQueryClassifier.Warnings == ODataQueryWarnings.None)
        return;
      request.ODataWarnings().AddRange((IEnumerable<string>) odataQueryClassifier.WarningsAsStrings);
      Action warningCallback = this.WarningCallback;
      if (warningCallback == null)
        return;
      warningCallback();
    }
  }
}
