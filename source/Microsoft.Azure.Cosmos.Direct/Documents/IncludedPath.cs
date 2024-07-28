// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IncludedPath
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class IncludedPath : JsonSerializable, ICloneable
  {
    private Collection<Index> indexes;

    [JsonProperty(PropertyName = "path")]
    public string Path
    {
      get => this.GetValue<string>("path");
      set => this.SetValue("path", (object) value);
    }

    [JsonProperty(PropertyName = "indexes")]
    public Collection<Index> Indexes
    {
      get
      {
        if (this.indexes == null)
        {
          this.indexes = this.GetValue<Collection<Index>>("indexes");
          if (this.indexes == null)
            this.indexes = new Collection<Index>();
        }
        return this.indexes;
      }
      set
      {
        this.indexes = value != null ? value : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.PropertyCannotBeNull, (object) nameof (Indexes)));
        this.SetValue("indexes", (object) value);
      }
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<string>("path");
      foreach (JsonSerializable index in this.Indexes)
        index.Validate();
    }

    internal override void OnSave()
    {
      if (this.indexes == null)
        return;
      foreach (JsonSerializable index in this.indexes)
        index.OnSave();
      this.SetValue("indexes", (object) this.indexes);
    }

    public object Clone()
    {
      IncludedPath includedPath = new IncludedPath()
      {
        Path = this.Path
      };
      foreach (Index index in this.Indexes)
        includedPath.Indexes.Add(index);
      return (object) includedPath;
    }
  }
}
