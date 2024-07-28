// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UniqueKeyPolicy
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class UniqueKeyPolicy : JsonSerializable
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

    public override bool Equals(object obj)
    {
      if (!(obj is UniqueKeyPolicy uniqueKeyPolicy) || this.UniqueKeys.Count != uniqueKeyPolicy.UniqueKeys.Count)
        return false;
      foreach (UniqueKey uniqueKey in this.uniqueKeys)
      {
        if (!uniqueKeyPolicy.UniqueKeys.Contains(uniqueKey))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      foreach (UniqueKey uniqueKey in this.uniqueKeys)
        hashCode ^= uniqueKey.GetHashCode();
      return hashCode;
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
