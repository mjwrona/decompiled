// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Converters.CreatePipelineParametersExtensions
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Azure.Pipelines.WebServer.Converters
{
  public static class CreatePipelineParametersExtensions
  {
    public static Microsoft.Azure.Pipelines.Server.ObjectModel.CreatePipelineParameters ToCreatePipelineParameters(
      this Microsoft.Azure.Pipelines.WebApi.CreatePipelineParameters parameters)
    {
      if (parameters == null)
        return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreatePipelineParameters) null;
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.WebApi.CreatePipelineConfigurationParameters>(parameters.Configuration, "Configuration", "pipelines");
      return new Microsoft.Azure.Pipelines.Server.ObjectModel.CreatePipelineParameters()
      {
        Name = parameters.Name,
        Folder = parameters.Folder,
        Configuration = parameters.Configuration.ToCreatePipelineConfigurationParameters()
      };
    }
  }
}
