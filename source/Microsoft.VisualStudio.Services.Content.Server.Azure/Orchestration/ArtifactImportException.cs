// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration.ArtifactImportException
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration
{
  public class ArtifactImportException : Exception
  {
    internal static ArtifactImportException Create(
      string expName,
      string message,
      bool stopRetrying,
      Exception cause = null)
    {
      ArtifactImportException ex = new ArtifactImportException("[" + expName + "] " + message, cause);
      if (stopRetrying)
        ex.AsFatalServicingOrchestrationException<ArtifactImportException>();
      return ex;
    }

    private ArtifactImportException(string message, Exception cause = null)
      : base(message, cause)
    {
    }
  }
}
