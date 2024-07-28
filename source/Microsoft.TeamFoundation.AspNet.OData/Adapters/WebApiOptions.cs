// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiOptions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiOptions : IWebApiOptions
  {
    public WebApiOptions(HttpConfiguration configuration)
    {
      this.NullDynamicPropertyIsEnabled = configuration != null ? configuration.HasEnabledNullDynamicProperty() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      this.UrlKeyDelimiter = configuration.GetUrlKeyDelimiter();
    }

    public ODataUrlKeyDelimiter UrlKeyDelimiter { get; private set; }

    public bool NullDynamicPropertyIsEnabled { get; private set; }
  }
}
