// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.TagsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class TagsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per Vssf")]
    public IEnumerable<string> GetTags(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Func<TagsSqlComponent, IEnumerable<string>> action = (Func<TagsSqlComponent, IEnumerable<string>>) (component => component.GetTags(projectId));
      return requestContext.ExecuteWithinUsingWithComponent<TagsSqlComponent, IEnumerable<string>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per Vssf")]
    public IEnumerable<string> AddTags(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Func<TagsSqlComponent, IEnumerable<string>> action = (Func<TagsSqlComponent, IEnumerable<string>>) (component => (IEnumerable<string>) component.AddTags(projectId, releaseId, tags).ToList<string>());
      return requestContext.ExecuteWithinUsingWithComponent<TagsSqlComponent, IEnumerable<string>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per Vssf")]
    public IEnumerable<string> DeleteTags(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Func<TagsSqlComponent, IEnumerable<string>> action = (Func<TagsSqlComponent, IEnumerable<string>>) (component => (IEnumerable<string>) component.DeleteTags(projectId, releaseId, tags).ToList<string>());
      return requestContext.ExecuteWithinUsingWithComponent<TagsSqlComponent, IEnumerable<string>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per Vssf")]
    public IEnumerable<string> AddDefinitionTags(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Func<TagsSqlComponent, IEnumerable<string>> action = (Func<TagsSqlComponent, IEnumerable<string>>) (component => component.AddDefinitionTags(projectId, definitionId, tags));
      return requestContext.ExecuteWithinUsingWithComponent<TagsSqlComponent, IEnumerable<string>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per Vssf")]
    public IEnumerable<string> DeleteDefinitionTags(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Func<TagsSqlComponent, IEnumerable<string>> action = (Func<TagsSqlComponent, IEnumerable<string>>) (component => component.DeleteDefinitionTags(projectId, definitionId, tags));
      return requestContext.ExecuteWithinUsingWithComponent<TagsSqlComponent, IEnumerable<string>>(action);
    }
  }
}
