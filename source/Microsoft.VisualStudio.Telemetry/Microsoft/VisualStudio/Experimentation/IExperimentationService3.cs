// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.IExperimentationService3
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  public interface IExperimentationService3
  {
    IDictionary<string, object> GetCachedTreatmentVariables(string configId);

    Task<IDictionary<string, object>> GetTreatmentVariablesAsync(
      string configId,
      CancellationToken token);

    Task<string> GetStringTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token);

    Task<int?> GetIntTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token);

    Task<bool?> GetBoolTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token);

    Task<double?> GetDoubleTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token);
  }
}
