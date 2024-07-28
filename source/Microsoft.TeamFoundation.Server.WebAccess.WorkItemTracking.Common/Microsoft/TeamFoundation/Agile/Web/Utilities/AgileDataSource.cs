// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.AgileDataSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class AgileDataSource
  {
    public static JsObject GetProcessSettings(
      IVssRequestContext tfsRequestContext,
      string projectName,
      string projectUri)
    {
      return tfsRequestContext.GetService<IProjectConfigurationService>().GetProcessSettings(tfsRequestContext, projectUri, true).ToJson(tfsRequestContext, projectName);
    }

    public static JsObject GetProcessSettingsProperty(
      IVssRequestContext tfsRequestContext,
      string projectUri,
      string propertyName)
    {
      Property[] properties = tfsRequestContext.GetService<IProjectConfigurationService>().GetProcessSettings(tfsRequestContext, projectUri, true).Properties;
      if (!((IEnumerable<string>) Enum.GetNames(typeof (ProjectPropertiesEnum))).Any<string>((Func<string, bool>) (vpn => VssStringComparer.PropertyName.Equals(vpn, propertyName))))
        throw new ArgumentException(Resources.PropertyName_Invalid);
      JsObject settingsProperty = new JsObject();
      Func<Property, bool> predicate = (Func<Property, bool>) (p => VssStringComparer.PropertyName.Equals(p.Name, propertyName));
      Property property = ((IEnumerable<Property>) properties).FirstOrDefault<Property>(predicate);
      if (property != null)
        settingsProperty["value"] = (object) property.Value;
      return settingsProperty;
    }
  }
}
