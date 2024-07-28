// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.AuditDecorationProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public class AuditDecorationProvider
  {
    public bool TryGetDecoratedDetails(
      IEnumerable<IActionFormatter> actionFormatters,
      string detailsFormat,
      IDictionary<string, object> data,
      DecorationIdentityMap identityMap,
      bool traceOnError,
      out string decoratedDetails)
    {
      decoratedDetails = (string) null;
      try
      {
        if (string.IsNullOrEmpty(detailsFormat) || data == null || data.Count == 0)
          return false;
        StringBuilder stringBuilder = new StringBuilder();
        int startIndex = -1;
        int separatorIndex = -1;
        for (int index = 0; index < detailsFormat.Length; ++index)
        {
          char ch = detailsFormat[index];
          switch (ch)
          {
            case ':':
              separatorIndex = index;
              break;
            case '{':
              startIndex = index;
              break;
            case '}':
              ActionFormatterInput input = separatorIndex > 0 ? this.GetActionInfo(identityMap, detailsFormat, startIndex, index, separatorIndex) : this.GetReplacementActionInputs(detailsFormat, startIndex, index);
              IActionFormatter actionFormatter = actionFormatters.FirstOrDefault<IActionFormatter>((Func<IActionFormatter, bool>) (x => x.Name == input.FormatterName));
              if (actionFormatter == null)
              {
                TeamFoundationTracingService.TraceRawAlwaysOn(1428532300, TraceLevel.Error, "AuditLog", "IActionFormatter", "Could not find an actionFormatter for " + input.Input + ". Looking for the " + input.FormatterName + " formatter.");
                return false;
              }
              string formattedString;
              actionFormatter.TryFormatAction(input, data, traceOnError, out formattedString);
              if (formattedString == null)
                return false;
              stringBuilder.Append(formattedString);
              separatorIndex = -1;
              startIndex = -1;
              break;
            default:
              if (startIndex < 0)
              {
                stringBuilder.Append(ch);
                break;
              }
              break;
          }
        }
        decoratedDetails = stringBuilder.ToString();
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1428532301, "AuditLog", "IActionFormatter", ex);
        return false;
      }
    }

    private ActionFormatterInput GetReplacementActionInputs(
      string detailsFormat,
      int startIndex,
      int endIndex)
    {
      return new ActionFormatterInput()
      {
        FormatterName = "Replacement",
        Input = detailsFormat.Substring(startIndex + 1, endIndex - startIndex - 1)
      };
    }

    private ActionFormatterInput GetActionInfo(
      DecorationIdentityMap identityMap,
      string detailsFormat,
      int startIndex,
      int endIndex,
      int separatorIndex)
    {
      string str1 = detailsFormat.Substring(startIndex + 1, endIndex - startIndex - 1);
      string str2 = str1.Substring(0, separatorIndex - startIndex - 1);
      string str3 = str1.Substring(separatorIndex - startIndex, endIndex - separatorIndex - 1);
      return new ActionFormatterInput()
      {
        FormatterName = str2,
        Input = str3,
        IdentityMap = str2 == "ResolveIdentity" ? identityMap : (DecorationIdentityMap) null
      };
    }
  }
}
