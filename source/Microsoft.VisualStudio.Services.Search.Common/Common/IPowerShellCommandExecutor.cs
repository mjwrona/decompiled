// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IPowerShellCommandExecutor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public interface IPowerShellCommandExecutor : IDisposable
  {
    bool ImportModulesFromPath(string path);

    void AddCommadToPowershellInstance(Command command);

    Collection<PSObject> ExecutePowershellCommand();
  }
}
