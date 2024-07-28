// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.EnvironmentObjectConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class EnvironmentObjectConverter
  {
    public static EnvironmentObject ToEnvironmentObject(this EnvironmentInstance environmentInstance)
    {
      EnvironmentObject environmentObject = new EnvironmentObject()
      {
        Id = environmentInstance.Id,
        Name = environmentInstance.Name,
        Description = environmentInstance.Description,
        CreatedBy = environmentInstance.CreatedBy,
        CreatedOn = environmentInstance.CreatedOn,
        LastModifiedBy = environmentInstance.LastModifiedBy,
        LastModifiedOn = environmentInstance.LastModifiedOn,
        ReferencedResources = environmentInstance.ReferencedResources
      };
      foreach (EnvironmentResourceReference resource in (IEnumerable<EnvironmentResourceReference>) environmentInstance.Resources)
        environmentObject.Resources.Add(resource.ToResourceReferenceObject());
      return environmentObject;
    }

    public static IList<EnvironmentObject> ToEnvironmentObjectList(
      this IList<EnvironmentInstance> environmentInstanceList)
    {
      IList<EnvironmentObject> environmentObjectList = (IList<EnvironmentObject>) new List<EnvironmentObject>();
      if (environmentInstanceList != null)
      {
        foreach (EnvironmentInstance environmentInstance in (IEnumerable<EnvironmentInstance>) environmentInstanceList)
        {
          EnvironmentObject environmentObject = new EnvironmentObject()
          {
            Id = environmentInstance.Id,
            Name = environmentInstance.Name,
            Description = environmentInstance.Description,
            CreatedBy = environmentInstance.CreatedBy,
            CreatedOn = environmentInstance.CreatedOn,
            LastModifiedBy = environmentInstance.LastModifiedBy,
            LastModifiedOn = environmentInstance.LastModifiedOn,
            ReferencedResources = environmentInstance.ReferencedResources
          };
          foreach (EnvironmentResourceReference resource in (IEnumerable<EnvironmentResourceReference>) environmentInstance.Resources)
            environmentObject.Resources.Add(resource.ToResourceReferenceObject());
          environmentObjectList.Add(environmentObject);
        }
      }
      return environmentObjectList;
    }
  }
}
