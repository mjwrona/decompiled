// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.FaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public abstract class FaultMapper
  {
    protected FaultMapper(string name, IndexerFaultSource source)
    {
      this.Name = name;
      this.Source = source;
    }

    public string Name { get; }

    public IndexerFaultSource Source { get; }

    public IndexerFaultSeverity Severity { get; set; }

    public bool Enabled { get; set; }

    public bool Retriable { get; set; }

    public bool LogFault { get; set; } = true;

    public abstract bool IsMatch(Exception ex);
  }
}
