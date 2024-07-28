// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Utility.InstrumentBlock
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Utility
{
  public class InstrumentBlock : IDisposable
  {
    private Stopwatch m_stopwatch;
    private string m_blockName;
    private Dictionary<string, double> m_blockExecutionTimeMap;

    public InstrumentBlock(string blockName, Dictionary<string, double> blockExecutionTimeMap)
    {
      if (blockExecutionTimeMap == null)
        return;
      this.m_stopwatch = Stopwatch.StartNew();
      this.m_blockName = blockName;
      this.m_blockExecutionTimeMap = blockExecutionTimeMap;
    }

    public void Dispose()
    {
      if (this.m_blockExecutionTimeMap == null)
        return;
      this.m_stopwatch.Stop();
      this.m_blockExecutionTimeMap[this.m_blockName] = (double) this.m_stopwatch.ElapsedMilliseconds;
    }
  }
}
