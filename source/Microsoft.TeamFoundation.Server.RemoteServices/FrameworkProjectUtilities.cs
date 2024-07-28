// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RemoteServices.FrameworkProjectUtilities
// Assembly: Microsoft.TeamFoundation.Server.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 836F660A-A756-49A7-82F0-68378533B43C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.RemoteServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.RemoteServices
{
  internal static class FrameworkProjectUtilities
  {
    public static string Serialize(ProjectInfo project) => project.Serialize<ProjectInfo>(true);

    public static bool TryDeserialize(string data, out ProjectInfo project) => JsonUtilities.TryDeserialize<ProjectInfo>(data, out project, true);
  }
}
