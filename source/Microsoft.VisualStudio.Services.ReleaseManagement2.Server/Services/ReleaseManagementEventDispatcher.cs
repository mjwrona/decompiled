// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseManagementEventDispatcher
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  internal class ReleaseManagementEventDispatcher : VssTaskService
  {
    protected override int DefaultThreadCount => 16;

    protected override TimeSpan DefaultTaskTimeout => TimeSpan.FromSeconds(200.0);
  }
}
