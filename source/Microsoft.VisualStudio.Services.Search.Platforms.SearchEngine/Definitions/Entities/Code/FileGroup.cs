// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.FileGroup
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class FileGroup
  {
    public FileGroup()
    {
      this.BranchName = new List<string>();
      this.BranchNameOriginal = new List<string>();
      this.ChangeId = new List<string>();
    }

    public List<string> BranchName { get; set; }

    public List<string> BranchNameOriginal { get; set; }

    public List<string> ChangeId { get; set; }
  }
}
