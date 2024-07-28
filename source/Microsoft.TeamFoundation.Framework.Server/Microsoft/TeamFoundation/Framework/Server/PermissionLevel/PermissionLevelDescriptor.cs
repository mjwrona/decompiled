// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PermissionLevel.PermissionLevelDescriptor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.PermissionLevel
{
  internal class PermissionLevelDescriptor
  {
    public PermissionLevelDescriptor(Guid definitionId, string resourceId)
    {
      ArgumentUtility.CheckForEmptyGuid(definitionId, nameof (definitionId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      this.DefinitionId = definitionId;
      this.ResourceId = resourceId;
    }

    public Guid DefinitionId { get; }

    public string ResourceId { get; }
  }
}
