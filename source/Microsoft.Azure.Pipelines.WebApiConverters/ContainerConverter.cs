// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.ContainerConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public static class ContainerConverter
  {
    public static Microsoft.Azure.Pipelines.WebApi.Container ToWebApiContainer(
      this Microsoft.Azure.Pipelines.Server.ObjectModel.Container container,
      ISecuredObject securedObject)
    {
      return new Microsoft.Azure.Pipelines.WebApi.Container()
      {
        Environment = container.Environment,
        MapDockerSocket = container.MapDockerSocket,
        Image = container.Image,
        Options = container.Options,
        Volumes = container.Volumes,
        Ports = container.Ports
      };
    }
  }
}
