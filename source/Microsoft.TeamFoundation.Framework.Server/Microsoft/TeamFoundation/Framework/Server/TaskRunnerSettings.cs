// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TaskRunnerSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TaskRunnerSettings
  {
    private const string c_registrySettingsRootPath = "/Configuration/TaskRunner/Settings";
    private const string c_registrySettingsPathWarningThreshold = "/Configuration/TaskRunner/Settings/WaitedOnQueueWarningThreshold";
    private const string c_registrySettingsPathErrorThreshold = "/Configuration/TaskRunner/Settings/WaitedOnQueueErrorThreshold";
    private const string c_registrySettingsPathErrorQueueLimitThreshold = "/Configuration/TaskRunner/Settings/QueueLimitThreshold";
    private const int c_defaultWaitedOnQueueWarningThreshold = 10;
    private const int c_defaultWaitedOnQueueErrorThreshold = 40;
    private const int c_defaultQueueLimitThreshold = 10000;

    public TaskRunnerSettings(IVssRequestContext systemRequestContext)
    {
      RegistryEntryCollection registryEntryCollection = new RegistryEntryCollection("/Configuration/TaskRunner/Settings", RegistryHelpers.GetDeploymentValuesRaw(systemRequestContext.FrameworkConnectionInfo, "/Configuration/TaskRunner/Settings"));
      this.WaitedOnQueueWarningThreshold = TimeSpan.FromSeconds((double) registryEntryCollection.GetValueFromPath<int>("/Configuration/TaskRunner/Settings/WaitedOnQueueWarningThreshold", 10));
      this.WaitedOnQueueErrorThreshold = TimeSpan.FromSeconds((double) registryEntryCollection.GetValueFromPath<int>("/Configuration/TaskRunner/Settings/WaitedOnQueueErrorThreshold", 40));
      this.QueueLimitThreshold = registryEntryCollection.GetValueFromPath<int>("/Configuration/TaskRunner/Settings/QueueLimitThreshold", 10000);
    }

    public TimeSpan WaitedOnQueueWarningThreshold { get; }

    public TimeSpan WaitedOnQueueErrorThreshold { get; }

    public int QueueLimitThreshold { get; }
  }
}
