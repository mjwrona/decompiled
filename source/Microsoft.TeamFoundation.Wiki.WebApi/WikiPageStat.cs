// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiPageStat
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.TeamFoundation.Wiki.WebApi.Shared;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [DataContract]
  public class WikiPageStat : WikiSecuredObject
  {
    public WikiPageStat(DateTime day, int count)
    {
      this.Day = day;
      this.Count = count;
    }

    [DataMember(Name = "day", EmitDefaultValue = false)]
    public DateTime Day { get; private set; }

    [DataMember(Name = "count", EmitDefaultValue = false)]
    public int Count { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject) => base.SetSecuredObject(securedObject);
  }
}
