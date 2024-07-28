// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeployPhaseTypeExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class DeployPhaseTypeExtensions
  {
    public static string ToRunsOnValue(this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes deployPhaseType)
    {
      switch (deployPhaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment:
          return "Agent";
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer:
          return "Server";
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment:
          return "DeploymentGroup";
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates:
          return "Server";
        default:
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidDeployPhaseType, (object) deployPhaseType));
      }
    }

    public static string ToRunsOnValue(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes deployPhaseType)
    {
      switch (deployPhaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment:
          return "Agent";
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.RunOnServer:
          return "Server";
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment:
          return "DeploymentGroup";
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.DeploymentGates:
          return "Server";
        default:
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidDeployPhaseType, (object) deployPhaseType));
      }
    }
  }
}
