// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestRunDefinitionJob
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Test.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public class TestRunDefinitionJob : ITestExecutionJob
  {
    public TestRunDefinitionJob(
      int testRunId,
      List<int> configIds,
      TimeSpan runTimeout,
      TeamProjectReference projectReference,
      string testConfigurationsMapping,
      string environmentUrl,
      Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter filter,
      RunProperties runProperties)
    {
      ArgumentValidator.CheckNull((object) projectReference, nameof (projectReference));
      this.TestRunId = testRunId;
      this.ConfigurationIds = configIds;
      this.ProjectReference = projectReference;
      this.Type = JobType.TestRunDefinition;
      this.RunTimeout = runTimeout;
      this.TestConfigurationsMapping = testConfigurationsMapping;
      this.EnvironmentUrl = environmentUrl;
      this.Filter = filter;
      this.RunProperties = runProperties;
    }

    public JobType Type { get; set; }

    public int TestRunId { get; private set; }

    public List<int> ConfigurationIds { get; private set; }

    public TeamProjectReference ProjectReference { get; private set; }

    public TimeSpan RunTimeout { get; private set; }

    public string TestConfigurationsMapping { get; private set; }

    public string EnvironmentUrl { get; private set; }

    public Microsoft.TeamFoundation.TestManagement.WebApi.RunFilter Filter { get; private set; }

    public RunProperties RunProperties { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestRunId: {0}, ConfigurationIds: {1}, RunTimeout: {2}, TestConfigurationsMapping: {3}", (object) this.TestRunId, (object) string.Join<int>(",", (IEnumerable<int>) this.ConfigurationIds), (object) this.RunTimeout, (object) this.TestConfigurationsMapping);
  }
}
