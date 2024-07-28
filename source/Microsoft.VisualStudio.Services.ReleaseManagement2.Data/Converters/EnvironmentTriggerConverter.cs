// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.EnvironmentTriggerConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class EnvironmentTriggerConverter
  {
    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger> ToServerEnvironmentTriggers(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger> environmentTriggers)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger> environmentTriggers1 = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger>();
      if (environmentTriggers != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger environmentTrigger in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger>) environmentTriggers)
          environmentTriggers1.Add(EnvironmentTriggerConverter.ToServerEnvironmentTrigger(environmentTrigger));
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger>) environmentTriggers1;
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger> ToWebApiEnvironmentTriggers(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger> environmentTriggers)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger> environmentTriggers1 = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger>();
      if (environmentTriggers != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger environmentTrigger in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger>) environmentTriggers)
          environmentTriggers1.Add(EnvironmentTriggerConverter.ToWebApiEnvironmentTrigger(environmentTrigger));
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger>) environmentTriggers1;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger ToServerEnvironmentTrigger(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger environmentTrigger)
    {
      if (environmentTrigger == null)
        throw new ArgumentNullException(nameof (environmentTrigger));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger()
      {
        ReleaseDefinitionId = environmentTrigger.ReleaseDefinitionId,
        EnvironmentId = environmentTrigger.DefinitionEnvironmentId,
        TriggerContent = environmentTrigger.TriggerContent,
        TriggerType = (byte) environmentTrigger.TriggerType
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger ToWebApiEnvironmentTrigger(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger environmentTrigger)
    {
      if (environmentTrigger == null)
        throw new ArgumentNullException(nameof (environmentTrigger));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger()
      {
        ReleaseDefinitionId = environmentTrigger.ReleaseDefinitionId,
        DefinitionEnvironmentId = environmentTrigger.EnvironmentId,
        TriggerContent = environmentTrigger.TriggerContent,
        TriggerType = (EnvironmentTriggerType) environmentTrigger.TriggerType
      };
    }
  }
}
