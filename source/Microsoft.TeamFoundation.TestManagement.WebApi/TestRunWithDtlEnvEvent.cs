// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestRunWithDtlEnvEvent
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class TestRunWithDtlEnvEvent : TestRunEvent
  {
    [DataMember(IsRequired = true)]
    public string MappedTestRunEventType { get; private set; }

    [DataMember(IsRequired = true)]
    public List<int> ConfigurationIds { get; private set; }

    [DataMember(IsRequired = true)]
    public TimeSpan RunTimeout { get; private set; }

    [DataMember(IsRequired = true)]
    public string TestConfigurationsMapping { get; private set; }

    public TestRunWithDtlEnvEvent(
      TestRun testRun,
      string mappedTestRunEventType,
      List<int> configIds,
      TimeSpan runTimeout,
      string testConfigurationsMapping)
      : base(testRun)
    {
      this.MappedTestRunEventType = mappedTestRunEventType;
      this.ConfigurationIds = configIds;
      this.RunTimeout = runTimeout;
      this.TestConfigurationsMapping = testConfigurationsMapping;
    }
  }
}
