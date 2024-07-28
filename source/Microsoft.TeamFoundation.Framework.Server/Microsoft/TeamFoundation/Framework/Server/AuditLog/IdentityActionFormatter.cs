// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.IdentityActionFormatter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  [Export(typeof (IActionFormatter))]
  public class IdentityActionFormatter : IActionFormatter
  {
    private const string c_area = "AuditLog";
    private const string c_layer = "IdentityActionFormatter";

    public string Name => "ResolveIdentity";

    public bool TryFormatAction(
      ActionFormatterInput decorationInput,
      IDictionary<string, object> dataBag,
      bool traceOnError,
      out string formattedString)
    {
      formattedString = (string) null;
      string input1 = decorationInput?.Input;
      object input2;
      if (dataBag.TryGetValue(input1, out input2))
      {
        try
        {
          DecorationIdentityMap identityMap = decorationInput.IdentityMap;
          string str = input2 as string;
          if (input2 is JArray jarray && jarray.Count == 1)
            str = ((IEnumerable<string>) jarray.ToObject<string[]>()).First<string>();
          if (input2 is Guid result || Guid.TryParse(str, out result))
            return this.TryResolveGuid(result, identityMap, out formattedString);
          IdentityDescriptor descriptor;
          if (this.TryGetDescriptor(input2, str, out descriptor))
            return this.TryResolveDescriptor(descriptor, identityMap, out formattedString);
          throw new Exception(input1 + " could not be parsed by the expected values: Guid, IdentityDescriptor");
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1428532361, "AuditLog", nameof (IdentityActionFormatter), ex);
          formattedString = (string) null;
          return false;
        }
      }
      else
      {
        if (traceOnError)
          TeamFoundationTracingService.TraceRawAlwaysOn(1428532362, TraceLevel.Warning, "AuditLog", nameof (IdentityActionFormatter), string.Format("{0} was not found in the data bag", input2));
        return false;
      }
    }

    private bool TryResolveGuid(
      Guid identityGuid,
      DecorationIdentityMap map,
      out string displayName)
    {
      displayName = (string) null;
      ResolvedIdentityRef val;
      if (!map.TryGetIdentity(identityGuid, out val))
      {
        map.AddGuid(identityGuid);
        return false;
      }
      displayName = val.Identity.DisplayName;
      return true;
    }

    private bool TryGetDescriptor(
      object input,
      string inputString,
      out IdentityDescriptor descriptor)
    {
      ref IdentityDescriptor local = ref descriptor;
      IdentityDescriptor identityDescriptor = input as IdentityDescriptor;
      if ((object) identityDescriptor == null)
        identityDescriptor = IdentityDescriptor.FromString(inputString);
      local = identityDescriptor;
      return !descriptor.IsUnknownIdentityType();
    }

    private bool TryResolveDescriptor(
      IdentityDescriptor descriptor,
      DecorationIdentityMap map,
      out string displayName)
    {
      displayName = (string) null;
      ResolvedIdentityRef val;
      if (!map.TryGetIdentity(descriptor, out val))
      {
        map.AddDescriptor(descriptor);
        return false;
      }
      displayName = val.Identity.DisplayName;
      return true;
    }
  }
}
