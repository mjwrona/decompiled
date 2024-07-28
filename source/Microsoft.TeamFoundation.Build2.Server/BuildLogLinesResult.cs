// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildLogLinesResult
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildLogLinesResult : IDisposable
  {
    private TeamFoundationDataReader m_reader;
    private bool m_disposed;

    public BuildLogLinesResult(TeamFoundationDataReader reader) => this.m_reader = reader;

    public IEnumerable<string> Lines
    {
      get
      {
        if (this.m_disposed)
          throw new ObjectDisposedException(nameof (BuildLogLinesResult));
        return this.m_reader.CurrentEnumerable<string>();
      }
    }

    public long StartLine { get; set; }

    public long EndLine { get; set; }

    public long TotalLines { get; set; }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      if (disposing && this.m_reader != null)
      {
        this.m_reader.Dispose();
        this.m_reader = (TeamFoundationDataReader) null;
      }
      this.m_disposed = true;
    }

    public void Dispose() => this.Dispose(true);
  }
}
