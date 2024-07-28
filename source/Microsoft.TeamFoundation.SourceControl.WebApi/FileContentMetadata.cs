// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.FileContentMetadata
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class FileContentMetadata : VersionControlSecuredObject
  {
    [DataMember(Name = "encoding", EmitDefaultValue = false)]
    public int Encoding { get; set; }

    [IgnoreDataMember]
    public bool EncodingWithBom { get; set; }

    [DataMember(Name = "contentType", EmitDefaultValue = false)]
    public string ContentType { get; set; }

    [DataMember(Name = "fileName", EmitDefaultValue = false)]
    public string FileName { get; set; }

    [DataMember(Name = "extension", EmitDefaultValue = false)]
    public string Extension { get; set; }

    [DataMember(Name = "isBinary", EmitDefaultValue = false)]
    public bool IsBinary { get; set; }

    [DataMember(Name = "isImage", EmitDefaultValue = false)]
    public bool IsImage { get; set; }

    [DataMember(Name = "vsLink", EmitDefaultValue = false)]
    public string VisualStudioWebLink { get; set; }
  }
}
