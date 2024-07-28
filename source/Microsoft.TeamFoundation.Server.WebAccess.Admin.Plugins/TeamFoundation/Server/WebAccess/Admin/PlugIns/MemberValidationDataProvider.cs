// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.MemberValidationDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins
{
  internal class MemberValidationDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.MemberValidation";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      return providerContext.Properties.ContainsKey("isMaterializedMember") && providerContext.Properties["isMaterializedMember"] != null && bool.Parse(providerContext.Properties["isMaterializedMember"].ToString()) ? (object) this.isMaterialized(requestContext, providerContext) : (object) false;
    }

    private bool isMaterialized(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      if (providerContext.Properties.ContainsKey("descriptor") && providerContext.Properties["descriptor"] != null)
      {
        string subjectDescriptorString = providerContext.Properties["descriptor"].ToString();
        try
        {
          IdentityDescriptor identityDescriptor = SubjectDescriptor.FromString(subjectDescriptorString).ToIdentityDescriptor(requestContext);
          if (requestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(requestContext, identityDescriptor, MembershipQuery.None, ReadIdentityOptions.None) != null)
            return true;
        }
        catch (Exception ex)
        {
          requestContext.Trace(10050097, TraceLevel.Info, "MembersPivot", "DataProvider", ex.Message);
        }
      }
      return false;
    }
  }
}
