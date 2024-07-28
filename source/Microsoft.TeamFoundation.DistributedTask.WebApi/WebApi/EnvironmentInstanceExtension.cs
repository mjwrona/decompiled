// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EnvironmentInstanceExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public static class EnvironmentInstanceExtension
  {
    public static PipelineResources GetLinkedResources(
      this EnvironmentInstance environment,
      int resourceId)
    {
      PipelineResources linkedResources = new PipelineResources();
      EnvironmentResourceReference resourceReference = environment.Resources.FirstOrDefault<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r => r.Id == resourceId));
      if (resourceReference != null)
      {
        foreach (EnvironmentLinkedResourceReference linkedResource in (IEnumerable<EnvironmentLinkedResourceReference>) resourceReference.LinkedResources)
        {
          switch (linkedResource.TypeName)
          {
            case "Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference":
              linkedResources.AddEndpointReference(new ServiceEndpointReference()
              {
                Id = new Guid(linkedResource.Id)
              });
              continue;
            case "Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentPoolReference":
              linkedResources.AddAgentPoolReference(new AgentPoolReference()
              {
                Id = int.Parse(linkedResource.Id)
              });
              continue;
            default:
              continue;
          }
        }
      }
      return linkedResources;
    }

    public static PipelineResources GetLinkedResources(this EnvironmentInstance environment)
    {
      PipelineResources linkedResources = new PipelineResources();
      foreach (EnvironmentResourceReference resource in (IEnumerable<EnvironmentResourceReference>) environment.Resources)
      {
        if (resource != null)
        {
          foreach (EnvironmentLinkedResourceReference linkedResource in (IEnumerable<EnvironmentLinkedResourceReference>) resource.LinkedResources)
          {
            if (linkedResource.TypeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference")
              linkedResources.AddEndpointReference(new ServiceEndpointReference()
              {
                Id = new Guid(linkedResource.Id)
              });
          }
        }
      }
      return linkedResources;
    }

    public static IList<EnvironmentResourceReference> FilterEnvironmentResourceReferences(
      this EnvironmentInstance environment,
      EnvironmentResourceFilter filter)
    {
      IList<EnvironmentResourceReference> resourceReferenceList = (IList<EnvironmentResourceReference>) null;
      if (environment.Resources.IsNullOrEmpty<EnvironmentResourceReference>() || filter == null)
        return (IList<EnvironmentResourceReference>) null;
      IList<EnvironmentResourceReference> resources = environment.Resources;
      if (filter.Type.HasValue)
        resourceReferenceList = (IList<EnvironmentResourceReference>) resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r =>
        {
          int type1 = (int) r.Type;
          EnvironmentResourceType? type2 = filter.Type;
          int valueOrDefault = (int) type2.GetValueOrDefault();
          return type1 == valueOrDefault & type2.HasValue;
        })).ToList<EnvironmentResourceReference>();
      if (filter.Id.HasValue)
        resourceReferenceList = (IList<EnvironmentResourceReference>) resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r => r.Id == filter.Id.Value)).ToList<EnvironmentResourceReference>();
      if (!string.IsNullOrEmpty(filter.Name))
        resourceReferenceList = (IList<EnvironmentResourceReference>) resources.Where<EnvironmentResourceReference>((Func<EnvironmentResourceReference, bool>) (r => r.Name.ToLowerInvariant() == filter.Name.ToLowerInvariant())).ToList<EnvironmentResourceReference>();
      if (!filter.Tags.IsNullOrEmpty<string>())
        resourceReferenceList = (IList<EnvironmentResourceReference>) EnvironmentInstanceExtension.MatchTagFilters(resources, filter.Tags);
      return resourceReferenceList;
    }

    private static List<EnvironmentResourceReference> MatchTagFilters(
      IList<EnvironmentResourceReference> resources,
      IList<string> tagFilters)
    {
      List<EnvironmentResourceReference> resourceReferenceList = new List<EnvironmentResourceReference>();
      foreach (EnvironmentResourceReference resource in (IEnumerable<EnvironmentResourceReference>) resources)
      {
        bool flag = true;
        foreach (string tagFilter in (IEnumerable<string>) tagFilters)
        {
          if (!resource.Tags.Contains<string>(tagFilter, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          resourceReferenceList.Add(resource);
      }
      return resourceReferenceList;
    }
  }
}
