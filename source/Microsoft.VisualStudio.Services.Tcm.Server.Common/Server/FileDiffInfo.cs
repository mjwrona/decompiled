// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FileDiffInfo
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class FileDiffInfo
  {
    public FileDiffInfo()
      : this(string.Empty)
    {
    }

    public FileDiffInfo(string filePath)
    {
      this.FilePath = filePath;
      this.DiffBlocks = new List<KeyValuePair<LineRange, LineRangeStatus>>();
    }

    public List<KeyValuePair<LineRange, LineRangeStatus>> DiffBlocks { get; set; }

    public string FilePath { get; set; }
  }
}
