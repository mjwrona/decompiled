// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationSession
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public sealed class OrchestrationSession
  {
    public OrchestrationSession()
    {
    }

    public OrchestrationSession(string sessionId, CompressionType compressionType, byte[] state)
    {
      this.SessionId = sessionId;
      this.CompressionType = compressionType;
      this.State = state;
    }

    public string SessionId { get; set; }

    public CompressionType CompressionType { get; set; }

    public byte[] State { get; set; }

    public TimeSpan NextRun { get; set; }

    public bool IsComplete { get; set; }
  }
}
