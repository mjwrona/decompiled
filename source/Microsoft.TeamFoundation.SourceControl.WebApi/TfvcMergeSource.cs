// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcMergeSource
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcMergeSource
  {
    public TfvcMergeSource()
    {
    }

    public TfvcMergeSource(string serverItem, int versionFrom, int versionTo, bool isRename)
    {
      this.ServerItem = serverItem;
      this.VersionFrom = versionFrom;
      this.VersionTo = versionTo;
      this.IsRename = isRename;
    }

    [DataMember(EmitDefaultValue = false)]
    public string ServerItem { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int VersionFrom { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int VersionTo { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsRename { get; set; }
  }
}
