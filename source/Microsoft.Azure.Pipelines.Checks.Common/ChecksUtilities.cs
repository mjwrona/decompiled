// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.ChecksUtilities
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  public static class ChecksUtilities
  {
    private const string TraceLayer = "ChecksUtilities";

    public static List<Resource> GetDefaultAuthorizedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Resource> resources,
      int definitionId = 0)
    {
      return ChecksUtilities.GetFilteredResources(requestContext, resources, new Func<IResourcePlugin, List<Resource>, IList<Resource>>(getResourceDelegate));

      IList<Resource> getResourceDelegate(IResourcePlugin resourcePlugin, List<Resource> r) => resourcePlugin.GetDefaultAuthorizedResources(requestContext, projectId, definitionId);
    }

    public static List<Resource> GetResourcesWithPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Resource> resources,
      ResourcePermission permission = ResourcePermission.View)
    {
      try
      {
        return ChecksUtilities.GetFilteredResources(requestContext, resources, new Func<IResourcePlugin, List<Resource>, IList<Resource>>(getResourceDelegate));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34002102, nameof (ChecksUtilities), ex);
        throw;
      }

      IList<Resource> getResourceDelegate(IResourcePlugin resourcePlugin, List<Resource> r) => resourcePlugin.GetResourcesWithPermission(requestContext, projectId, (IList<Resource>) r, permission);
    }

    public static Dictionary<ResourceType, List<Resource>> GetResourceTypeToResourcesMap(
      List<Resource> resources)
    {
      Dictionary<ResourceType, List<Resource>> map = new Dictionary<ResourceType, List<Resource>>();
      resources?.ForEach((Action<Resource>) (resource =>
      {
        ResourceType result;
        ResourceTypeNames.TryParse(resource.Type, out result);
        if (!map.ContainsKey(result))
          map[result] = new List<Resource>();
        map[result].Add(resource);
      }));
      return map;
    }

    public static int CompareResources(Resource r1, Resource r2)
    {
      int num = StringComparer.OrdinalIgnoreCase.Compare(r1?.Type, r2?.Type);
      if (num == 0)
        num = StringComparer.OrdinalIgnoreCase.Compare(r1?.Id, r2?.Id);
      return num;
    }

    private static List<Resource> GetFilteredResources(
      IVssRequestContext requestContext,
      List<Resource> resources,
      Func<IResourcePlugin, List<Resource>, IList<Resource>> getResourceDelegate)
    {
      if (requestContext.IsFeatureEnabled("Pipelines.Policy.ResourceListOptimizations"))
        ChecksUtilities.RemoveDuplicateResources(resources);
      else
        resources = resources.Distinct<Resource>().ToList<Resource>();
      Dictionary<ResourceType, List<Resource>> typeToResourcesMap = ChecksUtilities.GetResourceTypeToResourcesMap(resources);
      requestContext.TraceInfo(34002101, nameof (ChecksUtilities), "ChecksUtilities::GetFilteredResources for {0} resource types", (object) typeToResourcesMap.Count<KeyValuePair<ResourceType, List<Resource>>>());
      List<Resource> filteredResources = new List<Resource>();
      IDisposableReadOnlyList<IResourcePlugin> extensions = requestContext.GetExtensions<IResourcePlugin>(ExtensionLifetime.Service, throwOnError: true);
      Dictionary<ResourceType, IResourcePlugin> dictionary = extensions != null ? extensions.ToDictionary<IResourcePlugin, ResourceType>((Func<IResourcePlugin, ResourceType>) (rt => rt.Type)) : (Dictionary<ResourceType, IResourcePlugin>) null;
      foreach (KeyValuePair<ResourceType, List<Resource>> keyValuePair in typeToResourcesMap)
      {
        IResourcePlugin resourcePlugin = (IResourcePlugin) null;
        dictionary?.TryGetValue(keyValuePair.Key, out resourcePlugin);
        if (resourcePlugin == null)
        {
          requestContext.TraceError(34002100, nameof (ChecksUtilities), "Plugin not found for resource type {0}", (object) keyValuePair.Key);
          throw new ArgumentException(PipelineChecksCommonResources.InvalidResourceType((object) keyValuePair.Key), "resource.Type");
        }
        requestContext.TraceInfo(34002101, nameof (ChecksUtilities), "Fetching permissible resources for type {0} containing {1} resources", (object) keyValuePair.Key.ToString(), (object) keyValuePair.Value.Count<Resource>());
        filteredResources.AddRangeIfRangeNotNull<Resource, List<Resource>>((IEnumerable<Resource>) getResourceDelegate(resourcePlugin, keyValuePair.Value));
      }
      requestContext.TraceInfo(34002101, nameof (ChecksUtilities), "Returning {0} of {1} resources as permissible resources", (object) (filteredResources != null ? filteredResources.Count<Resource>() : 0), (object) resources.Count<Resource>());
      return filteredResources;
    }

    public static void RemoveDuplicateResources(List<Resource> resources)
    {
      if (resources.IsNullOrEmpty<Resource>())
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      resources.Sort(ChecksUtilities.\u003C\u003EO.\u003C0\u003E__CompareResources ?? (ChecksUtilities.\u003C\u003EO.\u003C0\u003E__CompareResources = new Comparison<Resource>(ChecksUtilities.CompareResources)));
      int index1 = 0;
      Resource other = resources[index1];
      for (int index2 = 1; index2 < resources.Count; ++index2)
      {
        Resource resource = resources[index2];
        if (!resource.Equals(other))
        {
          resources[++index1] = resource;
          other = resource;
        }
      }
      int index3 = index1 + 1;
      if (index3 == resources.Count)
        return;
      resources.RemoveRange(index3, resources.Count - index3);
    }

    public static bool HasProjectLevelAdminPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      ResourceType type)
    {
      requestContext.TraceInfo(34002101, nameof (ChecksUtilities), "ChecksUtilities::HasProjectLevelAdminPermission for resource types {0}", (object) type.ToString());
      IDisposableReadOnlyList<IResourcePlugin> extensions = requestContext.GetExtensions<IResourcePlugin>(ExtensionLifetime.Service, throwOnError: true);
      Dictionary<ResourceType, IResourcePlugin> dictionary = extensions != null ? extensions.ToDictionary<IResourcePlugin, ResourceType>((Func<IResourcePlugin, ResourceType>) (rt => rt.Type)) : (Dictionary<ResourceType, IResourcePlugin>) null;
      IResourcePlugin resourcePlugin = (IResourcePlugin) null;
      dictionary?.TryGetValue(type, out resourcePlugin);
      if (resourcePlugin == null)
      {
        requestContext.TraceError(34002100, nameof (ChecksUtilities), "Plugin not found for resource type {0}", (object) type);
        throw new ArgumentException(PipelineChecksCommonResources.InvalidResourceType((object) type), "resource.Type");
      }
      return resourcePlugin.HasProjectLevelAdminPermission(requestContext, projectId);
    }
  }
}
