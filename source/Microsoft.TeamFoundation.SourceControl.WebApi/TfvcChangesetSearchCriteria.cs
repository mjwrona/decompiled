// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcChangesetSearchCriteria
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcChangesetSearchCriteria
  {
    [DataMember(Name = "mappings", EmitDefaultValue = false)]
    private List<TfvcMappingFilter> m_mappings;

    [DataMember(Name = "itemPath", EmitDefaultValue = false)]
    public string ItemPath { get; set; }

    public IList<TfvcMappingFilter> Mappings
    {
      get
      {
        if (this.m_mappings == null)
          this.m_mappings = new List<TfvcMappingFilter>();
        return (IList<TfvcMappingFilter>) this.m_mappings;
      }
    }

    public TfvcVersionDescriptor versionDescriptor { get; set; }

    [DataMember(Name = "author", EmitDefaultValue = false)]
    public string Author { get; set; }

    [DataMember(Name = "fromDate", EmitDefaultValue = false)]
    public string FromDate { get; set; }

    [DataMember(Name = "toDate", EmitDefaultValue = false)]
    public string ToDate { get; set; }

    [DataMember(Name = "fromId", EmitDefaultValue = false)]
    public int FromId { get; set; }

    [DataMember(Name = "toId", EmitDefaultValue = false)]
    public int ToId { get; set; }

    [DataMember(Name = "followRenames", EmitDefaultValue = false)]
    public bool FollowRenames { get; set; }

    [DataMember(Name = "includeLinks", EmitDefaultValue = false)]
    public bool IncludeLinks { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<TfvcMappingFilter> mappings = this.m_mappings;
      // ISSUE: explicit non-virtual call
      if ((mappings != null ? (__nonvirtual (mappings.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_mappings = (List<TfvcMappingFilter>) null;
    }
  }
}
