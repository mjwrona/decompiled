// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebPageDataProviderPageSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class WebPageDataProviderPageSource
  {
    private Uri m_uri;
    private NameValueCollection m_queryParameters;

    [DataMember]
    public NavigationContext Navigation { get; set; }

    [DataMember]
    public ContextIdentifier Project { get; set; }

    [DataMember]
    public ContextIdentifier Team { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public DiagnosticsContext Diagnostics { get; set; }

    [DataMember]
    public WebPageGlobalizationContext Globalization { get; set; }

    public Uri Uri
    {
      get
      {
        if (this.m_uri == (Uri) null && !string.IsNullOrEmpty(this.Url))
          this.m_uri = new Uri(this.Url);
        return this.m_uri;
      }
    }

    [DataMember]
    public IList<string> ContributionPaths { get; set; }

    [DataMember]
    public string SelectedHubGroupId { get; set; }

    [DataMember]
    public string SelectedHubId { get; set; }

    public NameValueCollection QueryParameters
    {
      get
      {
        if (this.m_queryParameters == null)
          this.m_queryParameters = !(this.Uri == (Uri) null) ? HttpUtility.ParseQueryString(this.Uri.Query) : new NameValueCollection();
        return this.m_queryParameters;
      }
    }
  }
}
