// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.CheckExecutionOptions
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  public static class CheckExecutionOptions
  {
    public static string GetSerializedExecutionOptions(CheckConfiguration checkConfiguration)
    {
      if (!checkConfiguration.Timeout.HasValue)
        return (string) null;
      return new CheckExecutionOptions.ExecutionOptions()
      {
        Timeout = checkConfiguration.Timeout
      }.Serialize<CheckExecutionOptions.ExecutionOptions>();
    }

    public static void PopulateCheckConfiguration(
      CheckConfiguration checkConfiguration,
      string serializedConfigParams)
    {
      CheckExecutionOptions.ExecutionOptions executionOptions;
      JsonUtilities.TryDeserialize<CheckExecutionOptions.ExecutionOptions>(serializedConfigParams, out executionOptions);
      if (executionOptions == null)
        return;
      checkConfiguration.Timeout = executionOptions.Timeout;
    }

    internal class ExecutionOptions
    {
      public int? Timeout;
    }
  }
}
