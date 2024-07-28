// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceIntrinsicSettings
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [XmlRoot(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.VisualStudio.Services.Commerce")]
  public class CommerceIntrinsicSettings
  {
    public List<KeyValuePair> Items { get; set; }

    private KeyValuePair GetItemByKey(string Key) => this.Items.First<KeyValuePair>((Func<KeyValuePair, bool>) (x => x.Key.Equals(Key, StringComparison.OrdinalIgnoreCase)));

    private string GetValueByKey(string Key)
    {
      try
      {
        return this.GetItemByKey(Key).Value;
      }
      catch (InvalidOperationException ex)
      {
        return (string) null;
      }
    }

    private void SetValueByKey(string Key, string Value)
    {
      try
      {
        this.GetItemByKey(Key).Value = Value;
      }
      catch (InvalidOperationException ex)
      {
        this.Items.Add(new KeyValuePair(Key, Value));
      }
    }

    [XmlIgnore]
    public string Email
    {
      get => this.GetValueByKey("EMail");
      set => this.SetValueByKey("EMail", value);
    }

    [XmlIgnore]
    public string Puid
    {
      get => this.GetValueByKey(nameof (Puid));
      set => this.SetValueByKey(nameof (Puid), value);
    }

    [XmlIgnore]
    public string AccountName
    {
      get => this.GetValueByKey(nameof (AccountName));
      set => this.SetValueByKey(nameof (AccountName), value);
    }

    [XmlIgnore]
    public string OperationType
    {
      get => this.GetValueByKey("operationType");
      set => this.SetValueByKey("operationType", value);
    }

    [XmlIgnore]
    public string SubscriptionSource
    {
      get => this.GetValueByKey(nameof (SubscriptionSource));
      set => this.SetValueByKey(nameof (SubscriptionSource), value);
    }

    [XmlIgnore]
    public string IdentityDomain
    {
      get => this.GetValueByKey(nameof (IdentityDomain));
      set => this.SetValueByKey(nameof (IdentityDomain), value);
    }

    [XmlIgnore]
    public string TfsRegion
    {
      get => this.GetValueByKey(nameof (TfsRegion));
      set => this.SetValueByKey(nameof (TfsRegion), value);
    }

    public CommerceIntrinsicSettings() => this.Items = new List<KeyValuePair>();

    public CommerceIntrinsicSettings(List<KeyValuePair> items) => this.Items = items ?? new List<KeyValuePair>();
  }
}
