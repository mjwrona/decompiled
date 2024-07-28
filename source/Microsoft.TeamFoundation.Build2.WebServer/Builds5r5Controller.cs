// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds5r5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 5)]
  [CheckWellFormedProject(Required = true)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Collection)]
  public class Builds5r5Controller : Builds5Controller
  {
  }
}
