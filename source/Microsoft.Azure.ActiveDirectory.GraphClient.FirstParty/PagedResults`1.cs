// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.PagedResults`1
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class PagedResults<T> where T : GraphObject
  {
    public PagedResults()
    {
      this.Results = (IList<T>) new List<T>();
      this.MixedResults = (IList<string>) new List<string>();
    }

    public IList<T> Results { get; private set; }

    public IList<string> MixedResults { get; private set; }

    public Uri RequestUri { get; internal set; }

    public string PageToken { get; set; }

    public string ODataMetadataType { get; set; }

    public bool IsLastPage => string.IsNullOrEmpty(this.PageToken);
  }
}
