// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.ClientPermissionLevelDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  public static class ClientPermissionLevelDefinitionExtensions
  {
    public static Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition ToServer(
      this Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition x)
    {
      if (x == null)
        return (Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition) null;
      return new Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition(x.Id)
      {
        Name = x.Name,
        Description = x.Description,
        Type = x.Type,
        Scope = x.Scope
      };
    }
  }
}
