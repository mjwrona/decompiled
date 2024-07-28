// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceHooksHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  public static class ServiceHooksHelper
  {
    public static object GetSampleBuildCompletedPayload()
    {
      Microsoft.TeamFoundation.Build.WebApi.Build completedPayload = new Microsoft.TeamFoundation.Build.WebApi.Build();
      completedPayload.Uri = "vstfs:///Build/Build/2";
      completedPayload.Id = 2;
      completedPayload.BuildNumber = "ConsumerAddressModule_20150407.1";
      completedPayload.Url = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/71777fbc-1cf2-4bd1-9540-128c1c71f766/_apis/build/Builds/2";
      completedPayload.StartTime = DateTime.Parse("2015-04-07T18:04:06.83Z").ToUniversalTime();
      completedPayload.FinishTime = DateTime.Parse("2015-04-07T18:06:10.69Z").ToUniversalTime();
      completedPayload.Reason = Microsoft.TeamFoundation.Build.WebApi.BuildReason.Manual;
      completedPayload.Status = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Succeeded;
      completedPayload.DropLocation = "#/3/drop";
      completedPayload.Drop = new DropLocationReference()
      {
        Location = "#/3/drop",
        Type = DropLocationReferenceType.Container,
        Url = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/resources/Containers/3/drop",
        DownloadUrl = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/resources/Containers/3/drop?api-version=1.0&$format=zip&downloadFileName=ConsumerAddressModule_20150407.1_drop"
      };
      completedPayload.Log = new LogLocationReference()
      {
        Type = DropLocationReferenceType.Container,
        Url = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/resources/Containers/3/logs",
        DownloadUrl = "https://fabrikam-fiber-inc.visualstudio.com/_apis/resources/Containers/3/logs?api-version=1.0&$format=zip&downloadFileName=ConsumerAddressModule_20150407.1_logs"
      };
      completedPayload.SourceGetVersion = "LG:refs/heads/master:600c52d2d5b655caa111abfd863e5a9bd304bb0e";
      completedPayload.LastChangedBy = new IdentityRef()
      {
        Id = "d6245f20-2af8-44f4-9451-8107cb2767db",
        DisplayName = "Normal Paulk",
        UniqueName = "fabrikamfiber16@hotmail.com",
        Url = "https://fabrikam-fiber-inc.visualstudio.com/_apis/Identities/d6245f20-2af8-44f4-9451-8107cb2767db",
        ImageUrl = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=d6245f20-2af8-44f4-9451-8107cb2767db"
      };
      completedPayload.RetainIndefinitely = new bool?(false);
      completedPayload.HasDiagnostics = true;
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinition buildDefinition = new Microsoft.TeamFoundation.Build.WebApi.BuildDefinition();
      buildDefinition.DefinitionType = DefinitionType.Xaml;
      buildDefinition.Id = 2;
      buildDefinition.Name = "ConsumerAddressModule";
      buildDefinition.Url = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/71777fbc-1cf2-4bd1-9540-128c1c71f766/_apis/build/Definitions/2";
      completedPayload.Definition = (ShallowReference) buildDefinition;
      QueueReference queueReference = new QueueReference();
      queueReference.QueueType = QueueType.BuildController;
      queueReference.Id = 4;
      queueReference.Name = "Hosted Build Controller";
      queueReference.Url = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/build/Queues/4";
      completedPayload.Queue = queueReference;
      completedPayload.Requests = new List<RequestReference>()
      {
        new RequestReference()
        {
          Id = 1,
          Url = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/71777fbc-1cf2-4bd1-9540-128c1c71f766/_apis/build/Requests/1",
          RequestedFor = new IdentityRef()
          {
            Id = "d6245f20-2af8-44f4-9451-8107cb2767db",
            DisplayName = "Normal Paulk",
            UniqueName = "fabrikamfiber16@hotmail.com",
            Url = "https://fabrikam-fiber-inc.visualstudio.com/_apis/Identities/d6245f20-2af8-44f4-9451-8107cb2767db",
            ImageUrl = "https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_api/_common/identityImage?id=d6245f20-2af8-44f4-9451-8107cb2767db"
          }
        }
      };
      return (object) completedPayload;
    }
  }
}
