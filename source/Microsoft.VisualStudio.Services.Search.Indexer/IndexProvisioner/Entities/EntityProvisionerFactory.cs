// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities.EntityProvisionerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities
{
  public static class EntityProvisionerFactory
  {
    private static Dictionary<string, ProvisionerConfigAndConstantsProvider> s_entityIndexProvisioners = new Dictionary<string, ProvisionerConfigAndConstantsProvider>()
    {
      {
        "Code",
        (ProvisionerConfigAndConstantsProvider) new CodeEntityProvisionProvider()
      },
      {
        "ProjectRepo",
        (ProvisionerConfigAndConstantsProvider) new ProjectEntityProvisionProvider()
      },
      {
        "WorkItem",
        (ProvisionerConfigAndConstantsProvider) new WorkItemEntityProvisionProvider()
      },
      {
        "Wiki",
        (ProvisionerConfigAndConstantsProvider) new WikiEntityProvisionProvider()
      },
      {
        "Package",
        (ProvisionerConfigAndConstantsProvider) new PackageEntityProvisionProvider()
      },
      {
        "Board",
        (ProvisionerConfigAndConstantsProvider) new BoardEntityProvisionProvider()
      },
      {
        "Setting",
        (ProvisionerConfigAndConstantsProvider) new SettingEntityProvisionProvider()
      }
    };

    public static ProvisionerConfigAndConstantsProvider GetIndexProvisioner(IEntityType entityType)
    {
      ProvisionerConfigAndConstantsProvider indexProvisioner;
      if (EntityProvisionerFactory.s_entityIndexProvisioners.TryGetValue(entityType.Name, out indexProvisioner))
        return indexProvisioner;
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityProvisionerFactory:GetIndexProvisioner - EntityType '{0}' not supported.", (object) entityType.Name)));
    }

    public static IEnumerable<ProvisionerConfigAndConstantsProvider> GetIndexProvisioners(
      ExecutionContext executionContext)
    {
      return EntityProvisionerFactory.s_entityIndexProvisioners.Values.Where<ProvisionerConfigAndConstantsProvider>((Func<ProvisionerConfigAndConstantsProvider, bool>) (x => x.IsEnabled(executionContext.RequestContext)));
    }
  }
}
