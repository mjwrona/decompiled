// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMAndWorkItemPath
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public struct TCMAndWorkItemPath : IEquatable<TCMAndWorkItemPath>
  {
    private readonly string m_tcmPath;
    private readonly string m_workItemPath;
    internal static readonly TCMAndWorkItemPath Empty;

    internal TCMAndWorkItemPath(string tcmPath, string workItemPath)
    {
      this.m_tcmPath = tcmPath;
      this.m_workItemPath = workItemPath;
    }

    internal string TCMPath => this.m_tcmPath;

    internal string WorkItemPath => this.m_workItemPath;

    public bool Equals(TCMAndWorkItemPath other) => this.TCMPath.Equals(other.TCMPath) && this.WorkItemPath.Equals(other.WorkItemPath);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(TCM:{0} WorkItem:{1})", (object) this.TCMPath, (object) this.WorkItemPath);
  }
}
