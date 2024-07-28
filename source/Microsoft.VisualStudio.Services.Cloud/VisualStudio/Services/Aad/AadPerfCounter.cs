// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadPerfCounter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Aad
{
  internal class AadPerfCounter : IAadPerfCounter
  {
    private VssPerformanceCounter requests;
    private VssPerformanceCounter requestsPerSecond;
    private VssPerformanceCounter requestsSuccess;
    private VssPerformanceCounter requestsPerSecondSuccess;
    private VssPerformanceCounter requestsError;
    private VssPerformanceCounter requestsPerSecondError;
    private VssPerformanceCounter requestsFailure;
    private VssPerformanceCounter requestsPerSecondFailure;
    private readonly PerformanceCounterManagerWrapper _performanceCounterManagerWrapper;

    public AadPerfCounter(
      string area,
      string operationName,
      PerformanceCounterManagerWrapper wrapper)
    {
      this._performanceCounterManagerWrapper = wrapper;
      this.Initialize(area, operationName);
    }

    public static AadPerfCounter GetCounter(
      string area,
      string operationName,
      PerformanceCounterManagerWrapper factory = null)
    {
      return new AadPerfCounter(area, operationName, factory ?? new PerformanceCounterManagerWrapper());
    }

    public void IncrementEntryCounters()
    {
      this._performanceCounterManagerWrapper.IncrementCounter(this.requests);
      this._performanceCounterManagerWrapper.IncrementCounter(this.requestsPerSecond);
    }

    public void IncrementSuccessCounters()
    {
      this._performanceCounterManagerWrapper.IncrementCounter(this.requestsSuccess);
      this._performanceCounterManagerWrapper.IncrementCounter(this.requestsPerSecondSuccess);
    }

    public void IncrementErrorCounters()
    {
      this._performanceCounterManagerWrapper.IncrementCounter(this.requestsError);
      this._performanceCounterManagerWrapper.IncrementCounter(this.requestsPerSecondError);
    }

    public void IncrementFailureCounters()
    {
      this._performanceCounterManagerWrapper.IncrementCounter(this.requestsFailure);
      this._performanceCounterManagerWrapper.IncrementCounter(this.requestsPerSecondFailure);
    }

    private void Initialize(string operatioPrefix, string operationName)
    {
      string str = operatioPrefix + "." + operationName;
      this.requests = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".Requests");
      this.requestsPerSecond = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".RequestsPerSecond");
      this.requestsSuccess = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".Requests.Success");
      this.requestsPerSecondSuccess = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".RequestsPerSecond.Success");
      this.requestsError = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".Requests.Error");
      this.requestsPerSecondError = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".RequestsPerSecond.Error");
      this.requestsFailure = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".Requests.Failure");
      this.requestsPerSecondFailure = this._performanceCounterManagerWrapper.GetPerformanceCounter(str + ".RequestsPerSecond.Failure");
    }
  }
}
