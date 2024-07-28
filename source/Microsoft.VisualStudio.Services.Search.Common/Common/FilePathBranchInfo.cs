// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FilePathBranchInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class FilePathBranchInfo
  {
    public string FilePath { get; set; }

    public string Branch { get; set; }

    public override int GetHashCode() => (this.FilePath + this.Branch).GetHashCode();

    public override bool Equals(object obj) => obj is FilePathBranchInfo filePathBranchInfo && this.FilePath == filePathBranchInfo.FilePath && this.Branch == filePathBranchInfo.Branch;
  }
}
