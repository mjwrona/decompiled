// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitQueryCommitsCriteria
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitQueryCommitsCriteria
  {
    [DataMember(Name = "ids", EmitDefaultValue = false)]
    public List<string> Ids { get; set; }

    [DataMember(Name = "fromDate", EmitDefaultValue = false)]
    public string FromDate { get; set; }

    [DataMember(Name = "toDate", EmitDefaultValue = false)]
    public string ToDate { get; set; }

    [DataMember(Name = "itemVersion", EmitDefaultValue = false)]
    public GitVersionDescriptor ItemVersion { get; set; }

    [DataMember(Name = "compareVersion", EmitDefaultValue = false)]
    public GitVersionDescriptor CompareVersion { get; set; }

    [DataMember(Name = "fromCommitId", EmitDefaultValue = false)]
    public string FromCommitId { get; set; }

    [DataMember(Name = "toCommitId", EmitDefaultValue = false)]
    public string ToCommitId { get; set; }

    [DataMember(Name = "user", EmitDefaultValue = false)]
    public string Committer { get; set; }

    [DataMember(Name = "author", EmitDefaultValue = false)]
    public string Author { get; set; }

    [DataMember(Name = "itemPath", EmitDefaultValue = false)]
    public string ItemPath { get; set; }

    [DataMember(Name = "excludeDeletes", EmitDefaultValue = false)]
    public bool ExcludeDeletes { get; set; }

    [DataMember(Name = "$skip", EmitDefaultValue = false)]
    public int? Skip { get; set; }

    [DataMember(Name = "$top", EmitDefaultValue = false)]
    public int? Top { get; set; }

    [DataMember(Name = "includeLinks", EmitDefaultValue = false)]
    public bool IncludeLinks { get; set; }

    [DataMember(Name = "includeWorkItems", EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IncludeWorkItems { get; set; }

    [DataMember(Name = "includeUserImageUrl", EmitDefaultValue = false)]
    public bool IncludeUserImageUrl { get; set; }

    [DataMember(Name = "includePushData", EmitDefaultValue = false)]
    public bool IncludePushData { get; set; }

    [DataMember(Name = "historyMode", EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public GitHistoryMode HistoryMode { get; set; }

    [DataMember(Name = "showOldestCommitsFirst", EmitDefaultValue = false)]
    public bool ShowOldestCommitsFirst { get; set; }
  }
}
