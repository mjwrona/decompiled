// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetDirectoryRoleMembersRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetDirectoryRoleMembersRequest : AadServiceRequest
  {
    public GetDirectoryRoleMembersRequest()
    {
    }

    internal GetDirectoryRoleMembersRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public Guid DirectoryRoleObjectId { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      try
      {
        context.VssRequestContext.TraceEnter(8525600, "VisualStudio.Services.Aad", "Service", nameof (Execute));
        IAadGraphClient graphClient = context.GetGraphClient();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRoleMembersRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRoleMembersRequest();
        request.AccessToken = context.GetAccessToken();
        request.DirectoryRoleObjectId = this.DirectoryRoleObjectId;
        Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRoleMembersResponse directoryRoleMembers = graphClient.GetDirectoryRoleMembers(vssRequestContext, request);
        ISet<GraphObject> roleMembers = directoryRoleMembers.Exception == null ? directoryRoleMembers.Members : throw new AadGraphException(directoryRoleMembers.Exception.Message);
        context.VssRequestContext.TraceConditionally(8525605, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => roleMembers != null ? roleMembers.Serialize<ISet<GraphObject>>() : string.Empty));
        return (AadServiceResponse) new GetDirectoryRoleMembersResponse()
        {
          Members = (ISet<AadObject>) roleMembers.Select<GraphObject, AadObject>((Func<GraphObject, AadObject>) (m => AadGraphClient.ConvertObject(m, true))).ToHashSet<AadObject>()
        };
      }
      finally
      {
        context.VssRequestContext.TraceLeave(8525610, "VisualStudio.Services.Aad", "Service", nameof (Execute));
      }
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      try
      {
        context.VssRequestContext.TraceEnter(8525600, "VisualStudio.Services.Aad", "Service", nameof (ExecuteWithMicrosoftGraph));
        IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
        IVssRequestContext vssRequestContext = context.VssRequestContext;
        MsGraphGetDirectoryRoleMembersRequest request = new MsGraphGetDirectoryRoleMembersRequest();
        request.AccessToken = context.GetAccessToken(true);
        request.DirectoryRoleObjectId = this.DirectoryRoleObjectId;
        ISet<DirectoryObject> roleMembers = msGraphClient.GetDirectoryRoleMembers(vssRequestContext, request).Members;
        context.VssRequestContext.TraceConditionally(8525605, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Service", (Func<string>) (() => roleMembers != null ? roleMembers.Serialize<ISet<DirectoryObject>>() : string.Empty));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return (AadServiceResponse) new GetDirectoryRoleMembersResponse()
        {
          Members = (ISet<AadObject>) roleMembers.Select<DirectoryObject, AadObject>(GetDirectoryRoleMembersRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryObject ?? (GetDirectoryRoleMembersRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryObject = new Func<DirectoryObject, AadObject>(MicrosoftGraphConverters.ConvertDirectoryObject))).ToHashSet<AadObject>()
        };
      }
      finally
      {
        context.VssRequestContext.TraceLeave(8525610, "VisualStudio.Services.Aad", "Service", nameof (ExecuteWithMicrosoftGraph));
      }
    }
  }
}
