// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitTreeEntryRef
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitTreeEntryRef : VersionControlSecuredObject
  {
    public GitTreeEntryRef()
    {
    }

    public GitTreeEntryRef(
      string ObjectId,
      string RelativePath,
      int rwxBits,
      int GitObjectTypeCode)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public string ObjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RelativePath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Mode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitObjectType GitObjectType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public long Size { get; set; }
  }
}
