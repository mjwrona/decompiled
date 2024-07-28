// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepEventArgs
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class ServicingStepEventArgs
  {
    public ServicingStepEventArgs(
      string operation,
      string group,
      string name,
      ServicingStepState state,
      DateTime timestamp)
    {
      this.Operation = operation;
      this.Group = group;
      this.Name = name;
      this.State = state;
      this.Timestamp = timestamp;
    }

    public string Operation { get; }

    public string Group { get; }

    public string Name { get; }

    public ServicingStepState State { get; }

    public DateTime Timestamp { get; }
  }
}
