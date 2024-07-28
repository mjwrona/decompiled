// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiffBlock
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class FileDiffBlock : VersionControlSecuredObject
  {
    [DataMember(Name = "changeType")]
    public FileDiffBlockChangeType ChangeType { get; set; }

    [DataMember(Name = "oLine")]
    public int OriginalLineNumberStart { get; set; }

    [DataMember(Name = "oLinesCount")]
    public int OriginalLinesCount { get; set; }

    [DataMember(Name = "mLine")]
    public int ModifiedLineNumberStart { get; set; }

    [DataMember(Name = "mLinesCount")]
    public int ModifiedLinesCount { get; set; }

    [DataMember(Name = "oLines", EmitDefaultValue = false)]
    public List<string> OriginalLines { get; set; }

    [DataMember(Name = "mLines", EmitDefaultValue = false)]
    public List<string> ModifiedLines { get; set; }

    [DataMember(Name = "truncatedBefore", EmitDefaultValue = false)]
    public bool TruncatedBefore { get; set; }

    [DataMember(Name = "truncatedAfter", EmitDefaultValue = false)]
    public bool TruncatedAfter { get; set; }
  }
}
