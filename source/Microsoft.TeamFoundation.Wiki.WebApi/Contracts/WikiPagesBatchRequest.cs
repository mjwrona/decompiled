// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.Contracts.WikiPagesBatchRequest
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi.Contracts
{
  [DataContract]
  public class WikiPagesBatchRequest
  {
    [DataMember(Name = "pageViewsForDays", EmitDefaultValue = false)]
    public int? PageViewsForDays { get; set; }

    [DataMember(Name = "continuationToken", EmitDefaultValue = false)]
    public string ContinuationToken { get; set; }

    [DataMember(Name = "top", EmitDefaultValue = false)]
    public int? Top { get; set; }
  }
}
