// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ServicingResourcesHelper
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class ServicingResourcesHelper
  {
    public static IServicingResourceProvider CreateServicingResourceProvider(
      string servicingFilesPath,
      ServicingOperationTarget target,
      bool isSqlAzure)
    {
      return ServicingResourcesHelper.CreateServicingResourceProvider(servicingFilesPath, (IEnumerable<ServicingOperationTarget>) new ServicingOperationTarget[1]
      {
        target
      }, (isSqlAzure ? 1 : 0) != 0);
    }

    public static IServicingResourceProvider CreateServicingResourceProvider(
      string servicingFilesPath,
      IEnumerable<ServicingOperationTarget> targets,
      bool isSqlAzure)
    {
      IServicingResourceProvider fallbackResourceProvider = (IServicingResourceProvider) new AssemblyServicingResourceProvider(servicingFilesPath, isSqlAzure);
      foreach (ServicingOperationTarget target in targets)
      {
        string servicingResourcesPath = ServicingResourcesHelper.GetServicingResourcesPath(servicingFilesPath, target);
        if (Directory.Exists(servicingResourcesPath))
          fallbackResourceProvider = (IServicingResourceProvider) new FileSystemResourceProvider(servicingResourcesPath, fallbackResourceProvider);
      }
      return fallbackResourceProvider;
    }

    public static string GetOperationTargetDirectory(ServicingOperationTarget target)
    {
      switch (target)
      {
        case ServicingOperationTarget.PartitionDatabase:
          return "Database.Partition";
        case ServicingOperationTarget.ConfigurationDatabase:
          return "Database.Configuration";
        case ServicingOperationTarget.DeploymentHost:
          return "Host.Deployment";
        case ServicingOperationTarget.OrganizationHost:
          return "Host.Organization";
        case ServicingOperationTarget.CollectionHost:
          return "Host.Collection";
        default:
          throw new ArgumentException((string) null, nameof (target));
      }
    }

    public static string GetServicingOperationsPath(
      string servicingFilesPath,
      ServicingOperationTarget target)
    {
      return Path.Combine(ServicingResourcesHelper.GetServicingOperationTargetBasePath(servicingFilesPath, target), "Operations");
    }

    public static string GetServicingGroupsPath(
      string servicingFilesPath,
      ServicingOperationTarget target)
    {
      return Path.Combine(ServicingResourcesHelper.GetServicingOperationTargetBasePath(servicingFilesPath, target), "Groups");
    }

    public static string GetServicingResourcesPath(
      string servicingFilesPath,
      ServicingOperationTarget target)
    {
      return Path.Combine(ServicingResourcesHelper.GetServicingOperationTargetBasePath(servicingFilesPath, target), "ServicingFiles");
    }

    private static string GetServicingOperationTargetBasePath(
      string servicingFilesPath,
      ServicingOperationTarget target)
    {
      return Path.Combine(servicingFilesPath, ServicingResourcesHelper.GetOperationTargetDirectory(target));
    }
  }
}
