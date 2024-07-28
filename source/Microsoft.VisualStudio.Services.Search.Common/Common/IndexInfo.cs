// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class IndexInfo : ICloneable
  {
    private int? m_version = new int?(-1);

    [DataMember(Order = 0)]
    public string IndexName { get; set; }

    [DataMember(Order = 1)]
    public int? Version
    {
      get
      {
        int? version = this.m_version;
        int num = 0;
        if (version.GetValueOrDefault() < num & version.HasValue)
          throw new InvalidOperationException("Version is invalid. Make sure IndexInfo object is created with correct (null or positive integer) value for Version property.");
        return this.m_version;
      }
      set => this.m_version = value;
    }

    [DataMember(Order = 2)]
    public string Routing { get; set; }

    [DataMember(Order = 3)]
    public string EntityName { get; set; }

    [DataMember(Order = 4)]
    public bool IsShadow { get; set; }

    [DataMember(Order = 5)]
    public DocumentContractType DocumentContractType { get; set; }

    public object Clone() => (object) new IndexInfo()
    {
      IndexName = this.IndexName,
      Version = this.Version,
      Routing = this.Routing,
      EntityName = this.EntityName,
      IsShadow = this.IsShadow,
      DocumentContractType = this.DocumentContractType
    };

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      IndexInfo indexInfo = (IndexInfo) obj;
      if (string.Equals(this.Routing, indexInfo.Routing, StringComparison.Ordinal) && string.Equals(this.EntityName, indexInfo.EntityName, StringComparison.Ordinal) && string.Equals(this.IndexName, indexInfo.IndexName, StringComparison.Ordinal))
      {
        int? version1 = this.Version;
        int? version2 = indexInfo.Version;
        if (version1.GetValueOrDefault() == version2.GetValueOrDefault() & version1.HasValue == version2.HasValue && this.IsShadow == indexInfo.IsShadow)
          return this.DocumentContractType == indexInfo.DocumentContractType;
      }
      return false;
    }

    public override int GetHashCode()
    {
      int num1 = 31 * 1;
      int? nullable1 = this.IndexName?.GetHashCode();
      int? nullable2 = nullable1.HasValue ? new int?(num1 + nullable1.GetValueOrDefault()) : new int?();
      int num2 = 31 * nullable2.GetValueOrDefault();
      nullable2 = this.Version;
      ref int? local = ref nullable2;
      nullable1 = local.HasValue ? new int?(local.GetValueOrDefault().GetHashCode()) : new int?();
      int? nullable3;
      if (!nullable1.HasValue)
      {
        nullable2 = new int?();
        nullable3 = nullable2;
      }
      else
        nullable3 = new int?(num2 + nullable1.GetValueOrDefault());
      nullable2 = nullable3;
      int num3 = 31 * nullable2.GetValueOrDefault();
      string routing = this.Routing;
      int? nullable4;
      if (routing == null)
      {
        nullable2 = new int?();
        nullable4 = nullable2;
      }
      else
        nullable4 = new int?(routing.GetHashCode());
      nullable1 = nullable4;
      int? nullable5;
      if (!nullable1.HasValue)
      {
        nullable2 = new int?();
        nullable5 = nullable2;
      }
      else
        nullable5 = new int?(num3 + nullable1.GetValueOrDefault());
      nullable2 = nullable5;
      int num4 = 31 * nullable2.GetValueOrDefault();
      string entityName = this.EntityName;
      int? nullable6;
      if (entityName == null)
      {
        nullable2 = new int?();
        nullable6 = nullable2;
      }
      else
        nullable6 = new int?(entityName.GetHashCode());
      nullable1 = nullable6;
      int? nullable7;
      if (!nullable1.HasValue)
      {
        nullable2 = new int?();
        nullable7 = nullable2;
      }
      else
        nullable7 = new int?(num4 + nullable1.GetValueOrDefault());
      nullable2 = nullable7;
      return 31 * (31 * nullable2.GetValueOrDefault() + this.IsShadow.GetHashCode()) + this.DocumentContractType.GetHashCode();
    }
  }
}
