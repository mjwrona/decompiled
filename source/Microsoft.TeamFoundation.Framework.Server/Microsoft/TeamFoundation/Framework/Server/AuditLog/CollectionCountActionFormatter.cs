// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.CollectionCountActionFormatter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  [Export(typeof (IActionFormatter))]
  public class CollectionCountActionFormatter : IActionFormatter
  {
    private const string c_area = "AuditLog";
    private const string c_layer = "CollectionCountActionFormatter";

    public string Name => "CollectionCount";

    public bool TryFormatAction(
      ActionFormatterInput decorationInput,
      IDictionary<string, object> dataBag,
      bool traceOnError,
      out string formattedString)
    {
      formattedString = (string) null;
      try
      {
        string input = decorationInput?.Input;
        object obj;
        if (dataBag.TryGetValue(input, out obj))
        {
          ArgumentUtility.CheckType<IEnumerable>(obj, "value", "IEnumerable");
          formattedString = (obj as IEnumerable).Cast<object>().Count<object>().ToString();
          return true;
        }
        if (traceOnError)
          TeamFoundationTracingService.TraceRawAlwaysOn(1428532363, TraceLevel.Warning, "AuditLog", nameof (CollectionCountActionFormatter), string.Format("{0} could not be resolved", obj));
        return false;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1428532365, TraceLevel.Error, "AuditLog", nameof (CollectionCountActionFormatter), ex);
        return false;
      }
    }
  }
}
