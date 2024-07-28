// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.Utilities
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal static class Utilities
  {
    internal static Resource GetPermissibleResourceForConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfigurationRef checkConfigurationRef,
      ResourcePermission permission = ResourcePermission.View)
    {
      List<Resource> source = new List<Resource>();
      if (checkConfigurationRef != null && checkConfigurationRef.Resource != null)
      {
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        List<Resource> resources = new List<Resource>();
        resources.Add(checkConfigurationRef.Resource);
        int permission1 = (int) permission;
        source = ChecksUtilities.GetResourcesWithPermission(requestContext1, projectId1, resources, (ResourcePermission) permission1);
      }
      if (source.Count != 1)
      {
        requestContext.TraceError(34001905, "Pipeline.Checks", "Access Denied to the resource {0} - Resource Id: {1}", (object) checkConfigurationRef.Resource.Type, (object) checkConfigurationRef.Resource.Id);
        throw new AccessDeniedException(PipelineChecksResources.ResourceAccessDenied());
      }
      return source.FirstOrDefault<Resource>();
    }

    internal static List<Resource> GetPermissibleResourcesForConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      List<CheckConfigurationRef> checkConfigurationRefs,
      ResourcePermission permission = ResourcePermission.View)
    {
      if (checkConfigurationRefs == null || checkConfigurationRefs.Count <= 0)
        return new List<Resource>();
      List<Resource> resources = new List<Resource>();
      foreach (CheckConfigurationRef configurationRef in checkConfigurationRefs)
      {
        if (configurationRef != null && configurationRef.Resource != null)
          resources.Add(configurationRef.Resource);
      }
      return ChecksUtilities.GetResourcesWithPermission(requestContext, projectId, resources, permission);
    }
  }
}
