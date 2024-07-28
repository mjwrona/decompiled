// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.ChangeListSearchCriteria
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class ChangeListSearchCriteria
  {
    private DateTime? m_fromDate;
    private DateTime? m_toDate;

    [DataMember(Name = "itemPath", EmitDefaultValue = false)]
    public string ItemPath { get; set; }

    [DataMember(Name = "itemPaths", EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<string> ItemPaths { get; set; }

    [DataMember(Name = "itemVersion", EmitDefaultValue = false)]
    public string ItemVersion { get; set; }

    [DataMember(Name = "user", EmitDefaultValue = false)]
    public string User { get; set; }

    [DataMember(Name = "fromDate", EmitDefaultValue = false)]
    public string FromDate { get; set; }

    [DataMember(Name = "toDate", EmitDefaultValue = false)]
    public string ToDate { get; set; }

    [DataMember(Name = "fromVersion", EmitDefaultValue = false)]
    public string FromVersion { get; set; }

    [DataMember(Name = "toVersion", EmitDefaultValue = false)]
    public string ToVersion { get; set; }

    [DataMember(Name = "compareVersion", EmitDefaultValue = false)]
    public string CompareVersion { get; set; }

    [DataMember(Name = "followRenames", EmitDefaultValue = false)]
    public bool FollowRenames { get; set; }

    [DataMember(Name = "excludeDeletes", EmitDefaultValue = false)]
    public bool ExcludeDeletes { get; set; }

    [DataMember(Name = "skip", EmitDefaultValue = false)]
    public int? Skip { get; set; }

    [DataMember(Name = "top", EmitDefaultValue = false)]
    public int? Top { get; set; }

    public void SetFromDate(DateTime fromDate)
    {
      this.m_fromDate = new DateTime?(fromDate);
      this.FromDate = fromDate.ToString();
    }

    public void SetToDate(DateTime toDate)
    {
      this.m_toDate = new DateTime?(toDate);
      this.ToDate = toDate.ToString();
    }
  }
}
