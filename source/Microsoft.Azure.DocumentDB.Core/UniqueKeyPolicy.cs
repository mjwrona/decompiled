// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UniqueKeyPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  public sealed class UniqueKeyPolicy : JsonSerializable
  {
    private Collection<UniqueKey> uniqueKeys;

    [JsonProperty(PropertyName = "uniqueKeys")]
    public Collection<UniqueKey> UniqueKeys
    {
      get
      {
        if (this.uniqueKeys == null)
        {
          this.uniqueKeys = this.GetValue<Collection<UniqueKey>>("uniqueKeys");
          if (this.uniqueKeys == null)
            this.uniqueKeys = new Collection<UniqueKey>();
        }
        return this.uniqueKeys;
      }
      set
      {
        this.uniqueKeys = value != null ? value : throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.PropertyCannotBeNull, (object) nameof (UniqueKeys)));
        this.SetValue("uniqueKeys", (object) this.uniqueKeys);
      }
    }

    internal override void OnSave()
    {
      if (this.uniqueKeys == null)
        return;
      foreach (JsonSerializable uniqueKey in this.uniqueKeys)
        uniqueKey.OnSave();
      this.SetValue("uniqueKeys", (object) this.uniqueKeys);
    }

    internal override void Validate()
    {
      base.Validate();
      foreach (JsonSerializable uniqueKey in this.UniqueKeys)
        uniqueKey.Validate();
    }
  }
}
