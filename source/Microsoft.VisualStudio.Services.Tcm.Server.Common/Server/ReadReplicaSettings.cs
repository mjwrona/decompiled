// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ReadReplicaSettings
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ReadReplicaSettings
  {
    internal List<string> EnabledCommands { get; } = new List<string>();

    internal ReadReplicaSettings(IVssRequestContext requestcontext)
    {
      try
      {
        this.EnabledCommands = requestcontext.GetService<IVssRegistryService>().ReadEntries(requestcontext, (RegistryQuery) "/Service/TestManagement/Settings/ReadReplicaEnabledCommands/*").Where<RegistryEntry>((Func<RegistryEntry, bool>) (entry => entry.GetValue<bool>())).Select<RegistryEntry, string>((Func<RegistryEntry, string>) (entry => entry.Name)).ToList<string>();
      }
      catch (Exception ex)
      {
        VssRequestContextExtensions.Trace(requestcontext, 1015600, TraceLevel.Error, "TestManagement", "Exceptions", "Error while fetching read replica enabled commands: {0}", new object[1]
        {
          (object) ex
        });
      }
    }
  }
}
