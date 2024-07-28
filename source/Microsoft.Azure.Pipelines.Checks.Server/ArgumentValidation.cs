// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.ArgumentValidation
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal static class ArgumentValidation
  {
    private const int SettingsMaxLength = 8000;

    public static void ValidateCheckConfigurationParameters(
      CheckConfiguration checkConfigurationParameters)
    {
      ArgumentUtility.CheckForNull<CheckConfiguration>(checkConfigurationParameters, nameof (checkConfigurationParameters), "Pipeline.Checks");
      ArgumentUtility.CheckForNull<CheckType>(checkConfigurationParameters.Type, "checkConfigurationType", "Pipeline.Checks");
      if (checkConfigurationParameters.Type.Id == Guid.Empty)
        ArgumentUtility.CheckStringForNullOrEmpty(checkConfigurationParameters.Type.Name, "checkConfigurationTypeName", "Pipeline.Checks");
      ArgumentValidation.ValidateResource(checkConfigurationParameters.Resource);
      ArgumentValidation.CheckSettingsLength(checkConfigurationParameters.GetCheckConfigurationSettings());
    }

    public static void ValidateCheckConfigurationParametersOnUpdate(
      int id,
      CheckConfiguration checkConfigurationParameters)
    {
      ArgumentUtility.CheckForNonPositiveInt(id, nameof (id), "Pipeline.Checks");
      ArgumentValidation.ValidateCheckConfigurationParameters(checkConfigurationParameters);
    }

    public static void ValidateResource(Resource resource)
    {
      ArgumentUtility.CheckForNull<Resource>(resource, nameof (resource), "Pipeline.Checks");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(resource.Type, "resource.Type", "Pipeline.Checks");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(resource.Id, "resource.Id", "Pipeline.Checks");
    }

    public static void CheckSettingsLength(object settings)
    {
      if (settings == null)
        return;
      ArgumentUtility.CheckStringLength(JsonUtility.ToString(settings), "checkConfigurationSettings", 8000, expectedServiceArea: "Pipeline.Checks");
    }

    public static void ValidateCheckRunFilter(CheckRunFilter checkRunFilter)
    {
      ArgumentUtility.CheckForEmptyGuid(checkRunFilter.Type, "Type", "Pipeline.Checks");
      ArgumentValidation.ValidateResource(checkRunFilter.Resource);
    }
  }
}
