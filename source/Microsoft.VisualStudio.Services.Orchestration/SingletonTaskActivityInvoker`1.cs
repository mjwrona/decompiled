// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.SingletonTaskActivityInvoker`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public sealed class SingletonTaskActivityInvoker<TActivity> : ITaskActivityInvoker
  {
    private readonly TActivity m_activities;

    public SingletonTaskActivityInvoker(TActivity activities) => this.m_activities = activities;

    public Task<object> Invoke(
      TaskContext context,
      string methodName,
      Func<object, Task<object>> invoke)
    {
      return invoke((object) this.m_activities);
    }
  }
}
