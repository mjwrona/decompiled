// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexingPolicyOld
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  internal sealed class IndexingPolicyOld : JsonSerializable, ICloneable
  {
    private Collection<IndexingPath> included;
    private IList<string> excluded;

    public IndexingPolicyOld()
    {
      this.Automatic = true;
      this.IndexingMode = IndexingMode.Consistent;
    }

    public bool Automatic
    {
      get => this.GetValue<bool>("automatic");
      set => this.SetValue("automatic", (object) value);
    }

    [JsonConverter(typeof (StringEnumConverter))]
    public IndexingMode IndexingMode
    {
      get
      {
        IndexingMode result = IndexingMode.Lazy;
        string str = this.GetValue<string>("indexingMode");
        if (!string.IsNullOrEmpty(str))
          Enum.TryParse<IndexingMode>(str, true, out result);
        return result;
      }
      set => this.SetValue("indexingMode", (object) value.ToString());
    }

    public Collection<IndexingPath> IncludedPaths
    {
      get
      {
        if (this.included == null)
        {
          this.included = this.GetValue<Collection<IndexingPath>>("includedPaths");
          if (this.included == null)
            this.included = new Collection<IndexingPath>();
        }
        return this.included;
      }
    }

    public IList<string> ExcludedPaths
    {
      get
      {
        if (this.excluded == null)
        {
          this.excluded = this.GetValue<IList<string>>("excludedPaths");
          if (this.excluded == null)
            this.excluded = (IList<string>) new List<string>();
        }
        return this.excluded;
      }
    }

    internal override void OnSave()
    {
      if ((this.included == null || this.included.Count != 0 || this.excluded == null ? 0 : (this.excluded.Count == 0 ? 1 : 0)) != 0)
        return;
      if (this.included != null)
        this.SetObjectCollection<IndexingPath>("includedPaths", this.included);
      if (this.excluded == null)
        return;
      this.SetValue("excludedPaths", (object) this.excluded);
    }

    public object Clone()
    {
      IndexingPolicyOld indexingPolicyOld = new IndexingPolicyOld()
      {
        Automatic = this.Automatic,
        IndexingMode = this.IndexingMode
      };
      foreach (IndexingPath includedPath in this.IncludedPaths)
        indexingPolicyOld.IncludedPaths.Add((IndexingPath) includedPath.Clone());
      foreach (string excludedPath in (IEnumerable<string>) this.ExcludedPaths)
        indexingPolicyOld.ExcludedPaths.Add(excludedPath);
      return (object) indexingPolicyOld;
    }
  }
}
