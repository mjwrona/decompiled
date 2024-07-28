// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PermissionLevel.PermissionLevelResource
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.PermissionLevel;

namespace Microsoft.TeamFoundation.Framework.Server.PermissionLevel
{
  internal class PermissionLevelResource
  {
    public PermissionLevelResource(PermissionLevelDefinitionScope scope, string resourceId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      ArgumentUtility.CheckForDefinedEnum<PermissionLevelDefinitionScope>(scope, nameof (scope));
      this.Scope = scope;
      this.ResourceId = resourceId;
    }

    public PermissionLevelResource Clone() => new PermissionLevelResource(this.Scope, this.ResourceId);

    public PermissionLevelDefinitionScope Scope { get; }

    public string ResourceId { get; }
  }
}
