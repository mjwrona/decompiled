// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQueryComputeUsageValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ODataQueryComputeUsageValidator : IODataValidator
  {
    public Action WarningCallback { get; set; }

    public string RequiredFeatureFlag => (string) null;

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
    }
  }
}
