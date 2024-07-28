// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TempTeamToProjectMappingComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class TempTeamToProjectMappingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<TempTeamToProjectMappingComponent>(1, true),
      (IComponentCreator) new ComponentCreator<TempTeamToProjectMappingComponent2>(2)
    }, "TempTeamToProject");

    public TempTeamToProjectMappingComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void SetTeamProjectMapping(IEnumerable<KeyValuePair<Guid, Guid>> mappings)
    {
    }
  }
}
