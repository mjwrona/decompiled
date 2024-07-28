// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.HistoryEntry`1
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [KnownType(typeof (TfvcHistoryEntry))]
  public class HistoryEntry<T>
  {
    [DataMember(EmitDefaultValue = false)]
    public Microsoft.TeamFoundation.SourceControl.WebApi.ChangeList<T> ChangeList { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public VersionControlChangeType ItemChangeType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ServerItem { get; set; }
  }
}
