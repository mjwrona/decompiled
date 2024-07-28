// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CodeCoverageFile
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class CodeCoverageFile
  {
    public string DownloadUrl { get; set; }

    public string FileName { get; set; }

    public int Id { get; set; }

    public int TestRunId { get; set; }

    public string BuildFlavor { get; set; }

    public string BuildPlatform { get; set; }

    public string SerializedInnerData { get; set; }

    public CoverageStorageType storageType { get; set; }
  }
}
