// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CodeCoverageFileEqualityComparer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class CodeCoverageFileEqualityComparer : IEqualityComparer<CodeCoverageFile>
  {
    public string GetIdentifier(CodeCoverageFile obj) => string.Format("{0}_{1}_{2}_{3}_{4}", (object) obj.TestRunId, (object) obj.Id, (object) obj.FileName, (object) obj.BuildFlavor, (object) obj.BuildPlatform);

    public bool Equals(CodeCoverageFile x, CodeCoverageFile y) => string.Equals(this.GetIdentifier(x), this.GetIdentifier(y), StringComparison.OrdinalIgnoreCase);

    public int GetHashCode(CodeCoverageFile obj) => this.GetIdentifier(obj).GetStableHashCode();
  }
}
