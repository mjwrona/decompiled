// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.RegistryWildcardListOrgLevelPackagingSetting`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class RegistryWildcardListOrgLevelPackagingSetting<TValue> : 
    IOrgLevelPackagingSetting<IReadOnlyCollection<TValue>>
  {
    public RegistryWildcardListOrgLevelPackagingSetting(
      RegistryWildcardListOrgLevelPackagingSettingDefinition<TValue> definition,
      IRegistryService registryService,
      IDeploymentLevelRegistryService deploymentRegistryService)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003Cdefinition\u003EP = definition;
      // ISSUE: reference to a compiler-generated field
      this.\u003CregistryService\u003EP = registryService;
      // ISSUE: reference to a compiler-generated field
      this.\u003CdeploymentRegistryService\u003EP = deploymentRegistryService;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public IReadOnlyCollection<TValue> Get()
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<RegistryItem> second = this.\u003CregistryService\u003EP.Read(this.\u003Cdefinition\u003EP.DeploymentOrOrgRegistryQuery);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IReadOnlyCollection<TValue>) this.\u003CdeploymentRegistryService\u003EP.Read(this.\u003Cdefinition\u003EP.DeploymentOrOrgRegistryQuery).Concat<RegistryItem>(second).Select<RegistryItem, TValue>((Func<RegistryItem, TValue>) (x => RegistryUtility.FromString<TValue>(x.Value))).ToImmutableList<TValue>();
    }
  }
}
