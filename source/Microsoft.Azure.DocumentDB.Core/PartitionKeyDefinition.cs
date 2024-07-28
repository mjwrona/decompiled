// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionKeyDefinition
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  public sealed class PartitionKeyDefinition : JsonSerializable
  {
    private Collection<string> paths;
    private PartitionKind? kind;

    [JsonProperty(PropertyName = "paths")]
    public Collection<string> Paths
    {
      get
      {
        if (this.paths == null)
          this.paths = this.GetValue<Collection<string>>("paths") ?? new Collection<string>();
        return this.paths;
      }
      set
      {
        this.paths = value;
        this.SetValue("paths", (object) value);
      }
    }

    [JsonProperty(PropertyName = "kind")]
    [JsonConverter(typeof (StringEnumConverter))]
    internal PartitionKind Kind
    {
      get
      {
        if (!this.kind.HasValue)
          this.kind = new PartitionKind?(this.GetValue<PartitionKind>("kind", PartitionKind.Hash));
        return this.kind.Value;
      }
      set
      {
        this.kind = new PartitionKind?();
        this.SetValue("kind", (object) value.ToString());
      }
    }

    [JsonProperty(PropertyName = "version", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public PartitionKeyDefinitionVersion? Version
    {
      get
      {
        int? nullable = this.GetValue<int?>("version");
        return !nullable.HasValue ? new PartitionKeyDefinitionVersion?() : new PartitionKeyDefinitionVersion?((PartitionKeyDefinitionVersion) nullable.GetValueOrDefault());
      }
      set
      {
        PartitionKeyDefinitionVersion? nullable = value;
        this.SetValue("version", (object) (nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?()));
      }
    }

    [JsonProperty(PropertyName = "systemKey", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal bool? IsSystemKey
    {
      get => this.GetValue<bool?>("systemKey");
      set => this.SetValue("systemKey", (object) value);
    }

    internal override void OnSave()
    {
      if (this.paths != null)
        this.SetValue("paths", (object) this.paths);
      if (!this.kind.HasValue)
        return;
      this.SetValue("kind", (object) this.kind.ToString());
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<int?>("version");
      this.GetValue<Collection<string>>("paths");
    }

    internal static bool AreEquivalent(PartitionKeyDefinition pkd1, PartitionKeyDefinition pkd2)
    {
      if (pkd1.Kind != pkd2.Kind)
        return false;
      PartitionKeyDefinitionVersion? version1 = pkd1.Version;
      PartitionKeyDefinitionVersion? version2 = pkd2.Version;
      if (!(version1.GetValueOrDefault() == version2.GetValueOrDefault() & version1.HasValue == version2.HasValue) || !pkd1.Paths.OrderBy<string, string>((Func<string, string>) (i => i)).SequenceEqual<string>((IEnumerable<string>) pkd2.Paths.OrderBy<string, string>((Func<string, string>) (i => i))))
        return false;
      bool? isSystemKey1 = pkd1.IsSystemKey;
      bool? isSystemKey2 = pkd2.IsSystemKey;
      return isSystemKey1.GetValueOrDefault() == isSystemKey2.GetValueOrDefault() & isSystemKey1.HasValue == isSystemKey2.HasValue;
    }
  }
}
