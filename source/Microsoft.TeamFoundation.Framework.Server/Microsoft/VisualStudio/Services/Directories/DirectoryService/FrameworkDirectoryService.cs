// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.FrameworkDirectoryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkDirectoryService : IDirectoryService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckHostedDeployment();
      this.ServiceHostId = systemRequestContext.ServiceHost.InstanceId;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public DirectoryEntityResult AddMember(
      IVssRequestContext requestContext,
      string member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      requestContext.TraceEnter(10026010, "DirectoryService", nameof (FrameworkDirectoryService), nameof (AddMember));
      try
      {
        requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
        this.Validator.ThrowIfInvalid(profile, license, localGroups);
        DirectoryEntityResult result = requestContext.GetClient<DirectoryHttpClient>().AddMemberAsync(member, profile, license, localGroups, propertiesToReturn, (object) requestContext).SyncResult<DirectoryEntityResult>();
        this.Validator.ThrowIfInvalid(result, member, profile, license, localGroups, propertiesToReturn);
        return result;
      }
      finally
      {
        requestContext.TraceLeave(10026019, "DirectoryService", nameof (FrameworkDirectoryService), nameof (AddMember));
      }
    }

    public DirectoryEntityResult AddMember(
      IVssRequestContext requestContext,
      IDirectoryEntityDescriptor member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      requestContext.TraceEnter(10026020, "DirectoryService", nameof (FrameworkDirectoryService), nameof (AddMember));
      try
      {
        requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
        this.Validator.ThrowIfInvalid(profile, license, localGroups);
        DirectoryEntityResult result = requestContext.GetClient<DirectoryHttpClient>().AddMemberAsync(member, profile, license, localGroups, propertiesToReturn, (object) requestContext).SyncResult<DirectoryEntityResult>();
        this.Validator.ThrowIfInvalid(result, member, profile, license, localGroups, propertiesToReturn);
        return result;
      }
      finally
      {
        requestContext.TraceLeave(10026029, "DirectoryService", nameof (FrameworkDirectoryService), nameof (AddMember));
      }
    }

    public IReadOnlyList<DirectoryEntityResult> AddMembers(
      IVssRequestContext requestContext,
      IReadOnlyList<IDirectoryEntityDescriptor> members,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      requestContext.TraceEnter(10026030, "DirectoryService", nameof (FrameworkDirectoryService), nameof (AddMembers));
      try
      {
        requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
        this.Validator.ThrowIfInvalid(profile, license, localGroups);
        IReadOnlyList<DirectoryEntityResult> results = requestContext.GetClient<DirectoryHttpClient>().AddMembersAsync(members, profile, license, localGroups, propertiesToReturn, (object) requestContext).SyncResult<IReadOnlyList<DirectoryEntityResult>>();
        this.Validator.ThrowIfInvalid(results, members, profile, license, localGroups, propertiesToReturn);
        return results;
      }
      finally
      {
        requestContext.TraceLeave(10026039, "DirectoryService", nameof (FrameworkDirectoryService), nameof (AddMembers));
      }
    }

    private Guid ServiceHostId { get; set; }

    private DirectoryAddMemberValidator Validator => DirectoryAddMemberValidator.Instance;
  }
}
