// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.EnvironmentConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class EnvironmentConverter
  {
    public static EnvironmentInstance ToEnvironment(
      this EnvironmentCreateParameter environmentCreateParameter)
    {
      return new EnvironmentInstance()
      {
        Name = environmentCreateParameter.Name,
        Description = environmentCreateParameter.Description
      };
    }

    public static EnvironmentInstance ToEnvironment(
      this EnvironmentUpdateParameter environmentUpdateParameter)
    {
      return new EnvironmentInstance()
      {
        Name = environmentUpdateParameter.Name,
        Description = environmentUpdateParameter.Description
      };
    }

    public static EnvironmentInstance ToEnvironment(this EnvironmentObject environmentObject)
    {
      EnvironmentInstance environment = new EnvironmentInstance()
      {
        Id = environmentObject.Id,
        Name = environmentObject.Name,
        Description = environmentObject.Description,
        CreatedBy = environmentObject.CreatedBy,
        CreatedOn = environmentObject.CreatedOn,
        LastModifiedBy = environmentObject.LastModifiedBy,
        LastModifiedOn = environmentObject.LastModifiedOn,
        ReferencedResources = environmentObject.ReferencedResources
      };
      foreach (EnvironmentResourceReferenceObject resource in (IEnumerable<EnvironmentResourceReferenceObject>) environmentObject.Resources)
        environment.Resources.Add(resource.ToResourceReference());
      return environment;
    }

    public static IList<EnvironmentInstance> ToEnvironmentList(
      this IList<EnvironmentObject> environmentObjectList)
    {
      IList<EnvironmentInstance> environmentList = (IList<EnvironmentInstance>) new List<EnvironmentInstance>();
      if (environmentObjectList != null)
      {
        foreach (EnvironmentObject environmentObject in (IEnumerable<EnvironmentObject>) environmentObjectList)
        {
          EnvironmentInstance environmentInstance = new EnvironmentInstance()
          {
            Id = environmentObject.Id,
            Name = environmentObject.Name,
            Description = environmentObject.Description,
            CreatedBy = environmentObject.CreatedBy,
            CreatedOn = environmentObject.CreatedOn,
            LastModifiedBy = environmentObject.LastModifiedBy,
            LastModifiedOn = environmentObject.LastModifiedOn,
            ReferencedResources = environmentObject.ReferencedResources
          };
          foreach (EnvironmentResourceReferenceObject resource in (IEnumerable<EnvironmentResourceReferenceObject>) environmentObject.Resources)
            environmentInstance.Resources.Add(resource.ToResourceReference());
          environmentList.Add(environmentInstance);
        }
      }
      return environmentList;
    }
  }
}
