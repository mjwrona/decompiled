// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ConnectedServiceDeletedEvent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class ConnectedServiceDeletedEvent
  {
    public ConnectedServiceDeletedEvent(string name, string teamProject)
    {
      this.Name = name;
      this.TeamProject = teamProject;
    }

    public string Name { get; private set; }

    public string TeamProject { get; private set; }
  }
}
