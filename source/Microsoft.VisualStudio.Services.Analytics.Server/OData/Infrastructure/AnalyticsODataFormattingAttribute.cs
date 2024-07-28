// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataFormattingAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnalyticsODataFormattingAttribute : ODataFormattingAttribute
  {
    public override IList<ODataMediaTypeFormatter> CreateODataFormatters()
    {
      IList<ODataMediaTypeFormatter> odataFormatters = base.CreateODataFormatters();
      foreach (ODataMediaTypeFormatter mediaTypeFormatter in (IEnumerable<ODataMediaTypeFormatter>) odataFormatters)
        mediaTypeFormatter.BaseAddressFactory = (Func<HttpRequestMessage, Uri>) (request =>
        {
          string odataLink = request.GetUrlHelper().CreateODataLink();
          return odataLink[odataLink.Length - 1] == '/' ? new Uri(odataLink) : new Uri(odataLink + "/");
        });
      return odataFormatters;
    }
  }
}
