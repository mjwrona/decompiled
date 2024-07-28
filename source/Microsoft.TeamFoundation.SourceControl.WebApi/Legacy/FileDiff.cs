// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class FileDiff : VersionControlSecuredObject
  {
    [DataMember(Name = "originalFile", EmitDefaultValue = false)]
    public ItemModel OriginalFile { get; set; }

    [DataMember(Name = "modifiedFile", EmitDefaultValue = false)]
    public ItemModel ModifiedFile { get; set; }

    [DataMember(Name = "blocks", EmitDefaultValue = false)]
    public List<FileDiffBlock> Blocks { get; set; }

    [DataMember(Name = "lineCharBlocks", EmitDefaultValue = false)]
    public List<FileCharDiffBlock> LineCharBlocks { get; set; }

    [DataMember(Name = "identicalContent", EmitDefaultValue = false)]
    public bool IdenticalContent { get; set; }

    [DataMember(Name = "whitespaceChangesOnly", EmitDefaultValue = false)]
    public bool WhitespaceChangesOnly { get; set; }

    [DataMember(Name = "binaryContent", EmitDefaultValue = false)]
    public bool BinaryContent { get; set; }

    [DataMember(Name = "imageComparison", EmitDefaultValue = false)]
    public bool ImageComparison { get; set; }

    [DataMember(Name = "originalFileTruncated", EmitDefaultValue = false)]
    public bool OriginalFileTruncated { get; set; }

    [DataMember(Name = "modifiedFileTruncated", EmitDefaultValue = false)]
    public bool ModifiedFileTruncated { get; set; }

    [DataMember(Name = "emptyContent", EmitDefaultValue = false)]
    public bool EmptyContent { get; set; }

    [DataMember(Name = "originalFileEncoding", EmitDefaultValue = false)]
    public string OriginalFileEncoding { get; set; }

    [DataMember(Name = "modifiedFileEncoding", EmitDefaultValue = false)]
    public string ModifiedFileEncoding { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.OriginalFile?.SetSecuredObject(securedObject);
      this.ModifiedFile?.SetSecuredObject(securedObject);
      List<FileDiffBlock> blocks = this.Blocks;
      if (blocks != null)
        blocks.SetSecuredObject<FileDiffBlock>(securedObject);
      List<FileCharDiffBlock> lineCharBlocks = this.LineCharBlocks;
      if (lineCharBlocks == null)
        return;
      lineCharBlocks.SetSecuredObject<FileCharDiffBlock>(securedObject);
    }
  }
}
