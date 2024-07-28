// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.PermissionsTraceDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class PermissionsTraceDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.PermissionsTrace";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      TracePermissionModel trace;
      try
      {
        string namespaceId;
        string token;
        string permissionBit;
        PermissionsTraceDataProvider.GetPermissionsTraceParams(providerContext, out namespaceId, out token, out permissionBit);
        string subjectDescriptor = (string) null;
        Guid permissionSetId = new Guid();
        string permissionSetToken = (string) null;
        string projectName;
        PermissionsHelper.GetSecurityPermissionsParams(requestContext, providerContext, out subjectDescriptor, out permissionSetId, out permissionSetToken, out projectName);
        ArgumentUtility.CheckStringForNullOrEmpty(subjectDescriptor, "descriptor");
        ArgumentUtility.CheckStringForNullOrEmpty(namespaceId, "namespaceId");
        ArgumentUtility.CheckStringForNullOrEmpty(token, "token");
        ArgumentUtility.CheckStringForNullOrEmpty(permissionBit, "permissionBit");
        ArgumentUtility.CheckForEmptyGuid(permissionSetId, "permissionSetId");
        IdentityDescriptor identityDescriptor = SubjectDescriptor.FromString(subjectDescriptor).ToIdentityDescriptor(requestContext);
        ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, "identityDescriptor");
        PermissionUpdate permissionUpdate = new PermissionUpdate()
        {
          PermissionBit = int.Parse(permissionBit),
          NamespaceId = new Guid(namespaceId),
          Token = token
        };
        trace = SecurityNamespacePermissionsManagerFactory.CreateManager(requestContext, permissionSetId, permissionSetToken, projectName).GetTrace(requestContext, identityDescriptor, permissionUpdate);
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050066, TraceLevel.Error, "TracePermissions", "DataProvider", ex.Message);
        return (object) null;
      }
      return (object) trace.ToClientTracePermissionModel(requestContext);
    }

    private static void GetPermissionsTraceParams(
      DataProviderContext providerContext,
      out string namespaceId,
      out string token,
      out string permissionBit)
    {
      namespaceId = (string) null;
      token = (string) null;
      permissionBit = (string) null;
      if (providerContext.Properties.ContainsKey(nameof (namespaceId)) && providerContext.Properties[nameof (namespaceId)] != null)
        namespaceId = providerContext.Properties[nameof (namespaceId)].ToString();
      if (providerContext.Properties.ContainsKey(nameof (token)) && providerContext.Properties[nameof (token)] != null)
        token = providerContext.Properties[nameof (token)].ToString();
      if (!providerContext.Properties.ContainsKey(nameof (permissionBit)) || providerContext.Properties[nameof (permissionBit)] == null)
        return;
      permissionBit = providerContext.Properties[nameof (permissionBit)].ToString();
    }
  }
}
