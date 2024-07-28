// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.TaskEventConfigExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public static class TaskEventConfigExtension
  {
    public static bool IsEnabled(this TaskEventConfig taskEventConfig)
    {
      if (taskEventConfig == null)
        throw new ArgumentNullException(nameof (taskEventConfig));
      if (taskEventConfig.Enabled.Type == JTokenType.Boolean)
        return taskEventConfig.Enabled.Value<bool>();
      if (taskEventConfig.Enabled.Type != JTokenType.String)
        return false;
      string str = taskEventConfig.Enabled.Value<string>();
      bool flag = false;
      ref bool local = ref flag;
      bool.TryParse(str, out local);
      return flag;
    }
  }
}
