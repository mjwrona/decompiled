// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ArtifactsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  [LogStorageExceptionFilter]
  public abstract class ArtifactsControllerBase : TfsApiController
  {
    private const string TraceMethodDetailsRegistryPath = "/Configuration/Artifacts/TraceMethodDetails";
    private static readonly string TraceMethodDetailsEnvironmentVarValue = Environment.GetEnvironmentVariable("TraceMethodDetails");

    protected override bool ExemptFromGlobalExceptionFormatting => true;

    protected bool ShouldTraceMethodDetail([CallerMemberName] string methodName = null)
    {
      string environmentVarValue = ArtifactsControllerBase.TraceMethodDetailsEnvironmentVarValue;
      if (string.IsNullOrEmpty(environmentVarValue))
        environmentVarValue = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue(this.TfsRequestContext, (RegistryQuery) "/Configuration/Artifacts/TraceMethodDetails", true, string.Empty);
      if (string.IsNullOrEmpty(environmentVarValue))
        return false;
      if (environmentVarValue == "*")
        return true;
      string str = this.GetType().Name.Replace("Controller", string.Empty) + "." + methodName;
      return environmentVarValue.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0;
    }
  }
}
