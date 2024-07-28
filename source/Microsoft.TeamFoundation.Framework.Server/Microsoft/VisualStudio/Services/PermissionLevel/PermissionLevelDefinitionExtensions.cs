// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  public static class PermissionLevelDefinitionExtensions
  {
    public static Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition ToClient(
      this Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition) null;
      return new Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition()
      {
        Id = x.Id,
        Name = x.Name,
        Description = x.Description,
        Type = x.Type,
        Scope = x.Scope
      };
    }

    public static IList<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition> ToClient(
      this IEnumerable<Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition> definitions)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return definitions == null ? (IList<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>) null : (IList<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>) definitions.Select<Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>(PermissionLevelDefinitionExtensions.\u003C\u003EO.\u003C0\u003E__ToClient ?? (PermissionLevelDefinitionExtensions.\u003C\u003EO.\u003C0\u003E__ToClient = new Func<Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>(PermissionLevelDefinitionExtensions.ToClient))).Where<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>((Func<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>();
    }
  }
}
