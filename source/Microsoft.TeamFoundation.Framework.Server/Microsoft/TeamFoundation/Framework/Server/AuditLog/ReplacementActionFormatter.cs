// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ReplacementActionFormatter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  [Export(typeof (IActionFormatter))]
  public class ReplacementActionFormatter : IActionFormatter
  {
    private const string c_area = "AuditLog";
    private const string c_layer = "ReplacementActionFormatter";

    public string Name => "Replacement";

    public bool TryFormatAction(
      ActionFormatterInput decorationInput,
      IDictionary<string, object> dataBag,
      bool traceOnError,
      out string formattedString)
    {
      formattedString = (string) null;
      string input = decorationInput?.Input;
      object obj;
      if (dataBag.TryGetValue(input, out obj))
      {
        formattedString = !(obj is JArray jarray) ? obj as string : string.Join(", ", jarray.ToObject<string[]>());
        return formattedString != null;
      }
      if (traceOnError)
        TeamFoundationTracingService.TraceRawAlwaysOn(1428532360, TraceLevel.Warning, "AuditLog", nameof (ReplacementActionFormatter), string.Format("{0} could not be resolved", obj));
      return false;
    }
  }
}
