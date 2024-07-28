// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.PublicProjectRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Types
{
  public class PublicProjectRequestRestrictionsAttribute : 
    PublicBaseRequestRestrictionsAttribute,
    IPublicRequestRestrictionsAttribute
  {
    public PublicProjectRequestRestrictionsAttribute()
      : this(false, true, "project", (string) null)
    {
    }

    public PublicProjectRequestRestrictionsAttribute(string minApiVersion)
      : this(false, true, "project", minApiVersion)
    {
    }

    public PublicProjectRequestRestrictionsAttribute(
      bool s2sOnly,
      bool enforceDataspaceRestrictionsForMembers = true,
      string minApiVersion = null)
      : this(s2sOnly, enforceDataspaceRestrictionsForMembers, "project", minApiVersion)
    {
    }

    public PublicProjectRequestRestrictionsAttribute(
      bool s2sOnly,
      bool enforceDataspaceRestrictionsForMembers,
      string projectParameter,
      string minApiVersion = null)
      : base(s2sOnly, enforceDataspaceRestrictionsForMembers, minApiVersion)
    {
      this.ProjectParameter = projectParameter;
    }

    public PublicProjectRequestRestrictionsAttribute(
      bool s2sOnly,
      bool enforceDataspaceRestrictionsForMembers,
      string minApiVersion,
      RequiredAuthentication requiredAuthentication,
      bool allowNonSsl,
      bool allowCors,
      AuthenticationMechanisms mechanisms,
      string description,
      UserAgentFilterType agentFilterType,
      string agentFilter,
      string projectParameter = "project")
      : base(s2sOnly, enforceDataspaceRestrictionsForMembers, minApiVersion, requiredAuthentication, allowNonSsl, allowCors, mechanisms, description, agentFilterType, agentFilter, TeamFoundationHostType.ProjectCollection)
    {
      this.ProjectParameter = projectParameter;
    }

    public string ProjectParameter { get; private set; }

    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      bool allowPublicAccess = false;
      Guid dataspaceIdentifier = Guid.Empty;
      string projFilter;
      if (routeValues == null || !routeValues.TryGetValue<string>(this.ProjectParameter, out projFilter))
        return new AllowPublicAccessResult(false, false, Guid.Empty);
      if (!string.IsNullOrEmpty(projFilter))
      {
        ProjectInfo projectInfo = this.GetProjectInfo(requestContext, projFilter);
        if (projectInfo != null)
        {
          dataspaceIdentifier = projectInfo.Id;
          if (projectInfo.Visibility.HasFlag((Enum) ProjectVisibility.Public))
            allowPublicAccess = true;
        }
      }
      return new AllowPublicAccessResult(allowPublicAccess, true, dataspaceIdentifier);
    }

    private ProjectInfo GetProjectInfo(IVssRequestContext requestContext, string projFilter)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      ProjectInfo projectInfo = (ProjectInfo) null;
      Guid result;
      if (Guid.TryParse(projFilter, out result))
        service.TryGetProject(requestContext.Elevate(), result, out projectInfo);
      else
        service.TryGetProject(requestContext.Elevate(), projFilter, out projectInfo);
      return projectInfo;
    }
  }
}
