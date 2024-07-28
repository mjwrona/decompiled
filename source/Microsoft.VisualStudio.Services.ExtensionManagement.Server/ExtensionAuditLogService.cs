// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.ExtensionAuditLogService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Components;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement
{
  public class ExtensionAuditLogService : 
    VssBaseService,
    IExtensionAuditLogService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ExtensionAuditLog GetAuditLog(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      ExtensionAuditLog auditLog = (ExtensionAuditLog) null;
      using (ExtensionAuditLogComponent component = requestContext.CreateComponent<ExtensionAuditLogComponent>())
        auditLog = component.GetExtensionAuditLog(publisherName, extensionName);
      if (auditLog != null)
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        HashSet<string> source1 = new HashSet<string>();
        foreach (ExtensionAuditLogEntry entry in auditLog.Entries)
        {
          if (!source1.Contains(entry.UpdatedBy.Id))
            source1.Add(entry.UpdatedBy.Id);
        }
        source1.Remove(Guid.Empty.ToString());
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source2 = service.ReadIdentities(requestContext, (IList<Guid>) source1.Select<string, Guid>((Func<string, Guid>) (i => new Guid(i))).ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
        if (source2 != null)
        {
          Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = source2.ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (i => i.Id.ToString().ToLower()));
          foreach (ExtensionAuditLogEntry entry in auditLog.Entries)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (dictionary.TryGetValue(entry.UpdatedBy.Id.ToLower(), out identity))
              entry.UpdatedBy.DisplayName = identity?.DisplayName;
          }
        }
      }
      return auditLog;
    }
  }
}
