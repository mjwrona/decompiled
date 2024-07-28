// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TenantSpecificFieldsInfo
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

namespace Microsoft.AzureAd.Icm.Types
{
  public class TenantSpecificFieldsInfo
  {
    public TenantSpecificFieldsInfo(char? textSeparator = null, char? valueSeparator = null)
    {
      this.TextSeparator = textSeparator;
      this.ValueSeparator = valueSeparator;
    }

    public char? TextSeparator { get; private set; }

    public char? ValueSeparator { get; private set; }
  }
}
