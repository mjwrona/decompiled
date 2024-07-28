// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ChangedPermissionAuditEventFormatter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  [Export(typeof (IAuditEventDataFormatter))]
  public class ChangedPermissionAuditEventFormatter : AuditEventDataFormatterBase<ChangedPermission>
  {
    protected const string c_area = "AuditLog";
    private const string c_layer = "ChangedPermissionAuditEventFormatter";

    public override bool TryDecorateData(
      DecorationIdentityMap identitiesMap,
      IDictionary<string, object> data)
    {
      try
      {
        object obj;
        if (data.TryGetValue("EventSummary", out obj))
        {
          IEnumerable<ChangedPermission> source = JsonConvert.DeserializeObject<IEnumerable<ChangedPermission>>(obj.ToString());
          IEnumerable<IdentityDescriptor> identityDescriptors = source.Select<ChangedPermission, IdentityDescriptor>((Func<ChangedPermission, IdentityDescriptor>) (p => p.SubjectDescriptor));
          identityDescriptors.ForEach<IdentityDescriptor>((Action<IdentityDescriptor>) (x => identitiesMap.AddDescriptor(x)));
          if (!identitiesMap.AnyDescriptors())
            return false;
          foreach (ChangedPermission changedPermission in source)
          {
            ResolvedIdentityRef val;
            if (identitiesMap.TryGetIdentity(changedPermission.SubjectDescriptor, out val))
              changedPermission.SubjectDisplayName = val.Identity.DisplayName;
            else
              TeamFoundationTracingService.TraceRawAlwaysOn(1428532393, TraceLevel.Error, "AuditLog", nameof (TryDecorateData), string.Format("Failed to find identity for {0}", (object) changedPermission.SubjectDescriptor));
          }
          data["EventSummary"] = (object) source;
          if (source.Count<ChangedPermission>() == 1)
          {
            ChangedPermission changedPermission = source.First<ChangedPermission>();
            data["SubjectDescriptor"] = (object) changedPermission.SubjectDescriptor;
            data["ChangedPermission"] = (object) changedPermission.PermissionNames;
            data["PermissionModifiedTo"] = (object) changedPermission.Change;
            data["SubjectDisplayName"] = (object) changedPermission.SubjectDisplayName;
          }
          else
            data["SubjectDisplayName"] = (object) this.GetSubjectDisplayNameForMultiplePermissions(identityDescriptors.Distinct<IdentityDescriptor>().Select<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x =>
            {
              ResolvedIdentityRef val;
              identitiesMap.TryGetIdentity(x, out val);
              return val.Identity;
            })));
        }
        return true;
      }
      catch (InvalidOperationException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1428532391, TraceLevel.Error, "AuditLog", nameof (TryDecorateData), (Exception) ex);
        return false;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1428532392, TraceLevel.Error, "AuditLog", nameof (TryDecorateData), ex);
        return false;
      }
    }

    private string GetSubjectDisplayNameForMultiplePermissions(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      int count = identities.Count<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (count <= 0)
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(1428532390, TraceLevel.Warning, "AuditLog", nameof (ChangedPermissionAuditEventFormatter), "No identities were found");
        return string.Empty;
      }
      if (count == 1)
        return identities.First<Microsoft.VisualStudio.Services.Identity.Identity>().DisplayName;
      if (count == 2)
        return FrameworkResources.AuditSecurityAnd((object) identities.First<Microsoft.VisualStudio.Services.Identity.Identity>().DisplayName, (object) identities.Last<Microsoft.VisualStudio.Services.Identity.Identity>().DisplayName);
      if (count > 4)
        return FrameworkResources.AuditSecurityAndOtherIdentities((object) string.Join(", ", identities.Take<Microsoft.VisualStudio.Services.Identity.Identity>(3).Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.DisplayName)).ToArray<string>()), (object) (count - 3));
      string[] array = identities.Take<Microsoft.VisualStudio.Services.Identity.Identity>(count).Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.DisplayName)).ToArray<string>();
      return FrameworkResources.AuditSecurityAnd((object) string.Join(", ", ((IEnumerable<string>) array).Take<string>(count - 1)), (object) array[count - 1]);
    }
  }
}
