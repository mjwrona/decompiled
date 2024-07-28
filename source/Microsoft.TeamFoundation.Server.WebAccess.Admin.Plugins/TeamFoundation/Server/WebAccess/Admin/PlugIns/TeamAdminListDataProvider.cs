// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.TeamAdminListDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class TeamAdminListDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.TeamsAdminList";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      string subjectDescriptorString;
      providerContext.Properties.TryGetValue<string>("descriptor", out subjectDescriptorString);
      if (string.IsNullOrEmpty(subjectDescriptorString))
      {
        ArgumentException argumentException = new ArgumentException("Unable to resolve team");
        requestContext.TraceException(10050096, "TeamsPivot", "Service", (Exception) argumentException);
        throw argumentException;
      }
      TeamFoundationIdentity identity = ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, new IdentityDescriptor[1]
      {
        SubjectDescriptor.FromString(subjectDescriptorString).ToIdentityDescriptor(requestContext)
      })).First<TeamFoundationIdentity>();
      return (object) requestContext.GetService<ITeamService>().GetTeamAdmins(requestContext, IdentityUtil.Convert(identity)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) (admin => admin.ToIdentityRef(requestContext)));
    }
  }
}
