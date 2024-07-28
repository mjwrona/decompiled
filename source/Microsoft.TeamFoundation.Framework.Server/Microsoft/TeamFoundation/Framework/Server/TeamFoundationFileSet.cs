// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationFileSet
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationFileSet : IDisposable
  {
    private ResultCollection m_result;

    public TeamFoundationFileSet(ResultCollection result)
    {
      ObjectBinder<TeamFoundationFile> current1 = result.GetCurrent<TeamFoundationFile>();
      if (!current1.MoveNext())
        return;
      this.m_result = result;
      this.Metadata = current1.Current;
      if (!this.m_result.TryNextResult())
        return;
      ObjectBinder<TeamFoundationFile> current2 = this.m_result.GetCurrent<TeamFoundationFile>();
      if (current2.MoveNext())
      {
        this.FullVersion = current2.Current;
        this.DeltaChain = current2;
      }
      else if (result != null && result.RequestContext != null)
        result.RequestContext.Trace(14293, TraceLevel.Error, "FileService", nameof (TeamFoundationFileSet), "prc_RetrieveFile didn't return two rows for ResourceId: {0}", (object) this.Metadata.Metadata.ResourceId);
      else
        TeamFoundationTracingService.TraceRaw(14293, TraceLevel.Error, "FileService", nameof (TeamFoundationFileSet), "prc_RetrieveFile didn't return two rows for ResourceId: {0}", (object) this.Metadata.Metadata.ResourceId);
    }

    public void Dispose()
    {
      if (this.m_result == null)
        return;
      this.m_result.Dispose();
      this.m_result = (ResultCollection) null;
    }

    public TeamFoundationFile Metadata { get; private set; }

    public TeamFoundationFile FullVersion { get; private set; }

    public ObjectBinder<TeamFoundationFile> DeltaChain { get; private set; }
  }
}
