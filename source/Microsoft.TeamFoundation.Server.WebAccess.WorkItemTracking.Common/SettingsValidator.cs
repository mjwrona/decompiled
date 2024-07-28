// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SettingsValidator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public class SettingsValidator
  {
    private List<string> m_errors;

    public SettingsValidator() => this.Reset();

    public void AddError(NodeDescription node, string errorMessage, params object[] values)
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, errorMessage, values);
      this.m_errors.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Validation_ElementError, (object) node.GetXPathString(), (object) str));
    }

    public void AddMissingElementError(NodeDescription node) => this.AddError(node, Resources.Validation_MissingElement);

    public void AddMissingAttributeError(NodeDescription node, string attributeName) => this.AddError(node, Resources.Validation_MissingAttributeValue, (object) attributeName);

    public void Reset() => this.m_errors = new List<string>();

    public bool HasErrors => this.m_errors.Any<string>();

    public IList<string> Errors => (IList<string>) this.m_errors;
  }
}
