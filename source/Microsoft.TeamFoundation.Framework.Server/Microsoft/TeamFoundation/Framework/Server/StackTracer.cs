// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StackTracer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StackTracer
  {
    private StackTrace m_stackTrace;

    public StackTracer() => this.m_stackTrace = new StackTrace(1);

    public override string ToString() => this.m_stackTrace.ToString();
  }
}
