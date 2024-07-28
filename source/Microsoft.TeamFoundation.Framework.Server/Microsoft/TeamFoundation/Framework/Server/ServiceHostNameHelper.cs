// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostNameHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServiceHostNameHelper
  {
    public static string GeneratePGuidName(Guid? id = null)
    {
      if (!id.HasValue)
        id = new Guid?(Guid.NewGuid());
      return "P" + id.Value.ToString("D");
    }

    public static bool IsPGuid(string name) => ServiceHostNameHelper.TryParsePGuid(name, out Guid _);

    public static bool IsDGuid(string name) => ServiceHostNameHelper.TryParseCharGuid(name, 'D', 'd', out Guid _);

    public static bool IsAGuid(string name) => ServiceHostNameHelper.TryParseCharGuid(name, 'A', 'a', out Guid _);

    public static bool IsDataImportOrganization(string name) => ServiceHostNameHelper.TryParseRequestGuid(name, "DataImport-", out Guid _);

    public static bool IsOrgLeaveOrganization(string name) => ServiceHostNameHelper.TryParseRequestGuid(name, "OrgLeave-", out Guid _);

    public static bool TryParsePGuid(string name, out Guid id) => ServiceHostNameHelper.TryParseCharGuid(name, 'P', 'p', out id);

    private static bool TryParseCharGuid(
      string name,
      char upperCaseChar,
      char lowerCaseChar,
      out Guid id)
    {
      id = Guid.Empty;
      if (name != null && name.Length == 37)
      {
        char ch = name[0];
        if (!ch.Equals(upperCaseChar))
        {
          ch = name[0];
          if (!ch.Equals(lowerCaseChar))
            goto label_4;
        }
        return Guid.TryParse(name.Substring(1), out id);
      }
label_4:
      return false;
    }

    private static bool TryParseRequestGuid(string name, string prefix, out Guid requestId)
    {
      requestId = Guid.Empty;
      return name != null && name.Length == prefix.Length + 36 && name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && Guid.TryParse(name.Substring(prefix.Length), out requestId);
    }
  }
}
