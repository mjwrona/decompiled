// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.IAgileSettingsExtensions
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public static class IAgileSettingsExtensions
  {
    private const string ClosedDateKey = "ClosedDateReferenceName";
    private const string VSTSClosedDateField = "Microsoft.VSTS.Common.ClosedDate";

    public static string GetClosedDateFieldReferenceName(
      this IAgileSettings settings,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      string empty = string.Empty;
      object obj;
      string fieldReferenceName;
      if (!requestContext.Items.TryGetValue("ClosedDateReferenceName", out obj))
      {
        TypeField closedDateField = settings.Process.ClosedDateField;
        fieldReferenceName = closedDateField == null ? (!settings.Process.RequirementBacklog.IsFieldOnAllWorkItemTypes(requestContext, settings.ProjectName, "Microsoft.VSTS.Common.ClosedDate") ? CoreFieldReferenceNames.Id : "Microsoft.VSTS.Common.ClosedDate") : closedDateField.Name;
        requestContext.Items["ClosedDateReferenceName"] = (object) fieldReferenceName;
      }
      else
        fieldReferenceName = Convert.ToString(obj);
      return fieldReferenceName;
    }
  }
}
