// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.ITestHttpClient
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface ITestHttpClient
  {
    Task<TestAgent> CreateAgentAsync(TestAgent testAgent);

    Task<TestAgent> GetAgentAsync(int id);

    Task DeleteAgentAsync(int id);

    Task<TestAutomationRunSlice> GetSliceAsync(int testAgentId);

    Task UpdateSliceAsync(TestAutomationRunSlice sliceUpdatePackage);

    Task<TestExecutionServiceCommand> GetCommandAsync(
      int testAgentId,
      long? lastCommandId = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<DistributedTestRun> UpdateTestRunAsync(
      DistributedTestRun distributedTestRun,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRunExecutionConfiguration> GetRerunConfigurationAsync(
      TestRunExecutionConfiguration testRunExecutionConfiguration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestExecutionControlOptions> GetTestExecutionControlOptionsAsync(
      string envUrl,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    HttpClient HttpClient { get; }
  }
}
