// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostMigration.HostMigrationInjectionUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.HostMigration
{
  public static class HostMigrationInjectionUtil
  {
    public static bool CheckInjection(IVssRequestContext requestContext, string path) => HostMigrationInjectionUtil.CheckInjection<string>(requestContext, path, "").IsInjectionEnabled;

    public static (bool IsInjectionEnabled, T InjectionValue) CheckInjection<T>(
      IVssRequestContext requestContext,
      string path,
      T defaultValue)
    {
      if (requestContext.ServiceHost.IsProduction)
        return (false, defaultValue);
      if (!path.StartsWith(FrameworkServerConstants.HostMigrationTestInjectionRoot))
        throw new ArgumentException("Query path does not live in host migration injection");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      IEnumerable<RegistryItem> registryItems = service.Read(requestContext, (RegistryQuery) path);
      if (!registryItems.Any<RegistryItem>())
        return (false, defaultValue);
      T singleValue = registryItems.GetSingleValue<T>(defaultValue);
      requestContext.TraceAlways(HostMigrationTrace.HostMigrationFaultInjectionTracepoint, TraceLevel.Warning, "HostMigration", nameof (HostMigrationInjectionUtil), "Host migration fault injection detected: " + path.Substring(FrameworkServerConstants.HostMigrationTestInjectionRoot.Length) + " = " + singleValue.ToString());
      string str = ((IEnumerable<string>) path.Split('/')).Last<string>();
      service.SetValue<bool>(requestContext, FrameworkServerConstants.HostMigrationTestInjectionValidationRoot + "/" + str, true);
      return (true, singleValue);
    }
  }
}
