// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ServerSettings
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ServerSettings
  {
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public byte DefaultWorkspaceLocation
    {
      get => (byte) this.DefaultWorkspaceLocationEnum;
      set => this.DefaultWorkspaceLocationEnum = (WorkspaceLocation) value;
    }

    internal WorkspaceLocation DefaultWorkspaceLocationEnum { get; set; }

    [ClientProperty(ClientVisibility.Internal)]
    public LocalItemExclusionSet DefaultLocalItemExclusionSet { get; set; }

    public bool AllowAsynchronousCheckoutInServerWorkspaces { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public int MaxAllowedServerPathLength { get; set; }

    [ClientProperty(ClientVisibility.Internal)]
    public string StableHashString { get; set; }
  }
}
