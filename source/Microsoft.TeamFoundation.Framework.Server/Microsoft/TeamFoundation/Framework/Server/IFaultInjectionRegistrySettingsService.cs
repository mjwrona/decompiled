// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IFaultInjectionRegistrySettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (FaultInjectionRegistrySettingsService))]
  internal interface IFaultInjectionRegistrySettingsService : IVssFrameworkService
  {
    FaultDefinition[] GetFaults(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType);

    string AddFaultDefinition(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType,
      FaultFilter filter,
      Dictionary<string, object> faultSettings);

    string AddFaultDefinition<T>(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType,
      FaultFilter filter,
      T faultSettings)
      where T : class, IFaultSettings;

    void ClearAllFaultDefinitions(IVssRequestContext requestContext);

    void DeleteSingleFaultDefinition(IVssRequestContext requestContext, string faultId);

    string[] GetAllRegisteredFaultDefinitionIds(IVssRequestContext requestContext);
  }
}
