// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.Demand
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class Demand
  {
    private const char c_demandPartSeparator = '/';

    public string Id { get; set; }

    public string DemandType { get; set; }

    public string DemandValue { get; set; }

    public static bool TryParse(string id, out Demand demand)
    {
      demand = (Demand) null;
      if (string.IsNullOrEmpty(id))
        return false;
      id = id.Trim();
      int length = id.IndexOf('/');
      if (length <= 0 || length == id.Length - 1)
        return false;
      demand = new Demand() { Id = id };
      demand.DemandType = id.Substring(0, length);
      demand.DemandValue = id.Substring(length + 1);
      return true;
    }

    public static Demand Parse(string id)
    {
      Demand demand;
      return !Demand.TryParse(id, out demand) ? (Demand) null : demand;
    }
  }
}
