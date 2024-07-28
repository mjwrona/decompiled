// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcChange
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcChange : Change<TfvcItem>
  {
    public TfvcChange()
    {
    }

    public TfvcChange(TfvcItem item, VersionControlChangeType changeType)
    {
      this.Item = item;
      this.ChangeType = changeType;
    }

    public TfvcChange(TfvcItem item, VersionControlChangeType changeType, string sourceServerItem)
      : this(item, changeType)
    {
      this.SourceServerItem = sourceServerItem;
    }

    [DataMember(EmitDefaultValue = false)]
    public int PendingVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<TfvcMergeSource> MergeSources { get; set; }
  }
}
