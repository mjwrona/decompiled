// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationRuntimeFactory
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationRuntimeFactory
  {
    public static OrchestrationRuntimeInstance CreateRuntime(
      IVssRequestContext requestContext,
      OrchestrationHubDescription hubDescription)
    {
      OrchestrationRuntime runtime = (OrchestrationRuntime) null;
      using (IDisposableReadOnlyList<IOrchestrationHubExtension> extensions = requestContext.GetExtensions<IOrchestrationHubExtension>((Func<IOrchestrationHubExtension, bool>) (x => x.HubType == hubDescription.HubType)))
      {
        if (extensions.Count > 1)
        {
          requestContext.Trace(0, TraceLevel.Error, "Orchestration", "Service", "Found {0} hub extensions", (object) extensions.Count);
          return (OrchestrationRuntimeInstance) null;
        }
        if (extensions.Count == 1)
          requestContext.Trace(0, TraceLevel.Verbose, "Orchestration", "Dispatcher", "Applying behaviors for hub extension '{0}'", (object) extensions[0].GetType().FullName);
        runtime = new OrchestrationRuntime(new OrchestrationSerializer(extensions.Count > 0 ? extensions[0].GetSerializerSettings(requestContext) : (JsonSerializerSettings) null));
      }
      bool flag = false;
      IDisposableReadOnlyList<IOrchestrationRuntimeExtension> extensions1 = (IDisposableReadOnlyList<IOrchestrationRuntimeExtension>) null;
      List<IOrchestrationRuntimeExtension> runtimeExtensionList = new List<IOrchestrationRuntimeExtension>();
      try
      {
        extensions1 = requestContext.GetExtensions<IOrchestrationRuntimeExtension>((Func<IOrchestrationRuntimeExtension, bool>) (x => x.HubType == hubDescription.HubType));
        if (extensions1.Count == 0)
        {
          requestContext.Trace(0, TraceLevel.Error, "Orchestration", "Dispatcher", "Found {0} runtime extensions", (object) extensions1.Count);
          return (OrchestrationRuntimeInstance) null;
        }
        requestContext.Trace(0, TraceLevel.Verbose, "Orchestration", "Dispatcher", "Found {0} runtime extensions", (object) extensions1.Count);
        foreach (IOrchestrationRuntimeExtension runtimeExtension in (IEnumerable<IOrchestrationRuntimeExtension>) extensions1)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Orchestration", "Dispatcher", "Applying behaviors for runtime extension {0}", (object) runtimeExtension.GetType().FullName);
          runtimeExtension.OnStart(requestContext, hubDescription, runtime);
          runtimeExtensionList.Add(runtimeExtension);
        }
        flag = true;
      }
      finally
      {
        if (!flag && extensions1 != null)
        {
          foreach (IOrchestrationRuntimeExtension runtimeExtension in runtimeExtensionList)
          {
            try
            {
              runtimeExtension.OnStop(requestContext, hubDescription, runtime);
            }
            catch (Exception ex)
            {
              requestContext.Trace(0, TraceLevel.Error, "Orchestration", "Dispatcher", "Error encountered will stopping extension {0}: {1}", (object) runtimeExtension.GetType().FullName, (object) ex.ToReadableStackTrace());
            }
          }
          extensions1.Dispose();
        }
      }
      return new OrchestrationRuntimeInstance(requestContext, hubDescription, runtime, extensions1);
    }
  }
}
