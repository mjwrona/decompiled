// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.MetaID
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MetaID : IComparable<MetaID>, IEquatable<MetaID>
  {
    private int m_ID;
    private bool m_IsTemp;

    private MetaID(int id, bool isTemp)
    {
      this.m_ID = id;
      this.m_IsTemp = isTemp;
    }

    public bool IsTemporary => this.m_IsTemp;

    public int Value => this.m_ID;

    public static MetaID CreateTempMetaID(int tempId) => new MetaID(tempId, true);

    public static MetaID CreateMetaID(int id) => new MetaID(id, false);

    public override string ToString() => XmlConvert.ToString(this.m_ID);

    public override int GetHashCode() => this.m_ID.GetHashCode();

    int IComparable<MetaID>.CompareTo(MetaID other)
    {
      if (this.m_IsTemp == other.m_IsTemp)
        return this.m_ID - other.m_ID;
      return !this.m_IsTemp ? -1 : 1;
    }

    bool IEquatable<MetaID>.Equals(MetaID other) => other != null && this.m_IsTemp == other.m_IsTemp && this.m_ID == other.m_ID;

    public override bool Equals(object o)
    {
      if (o == null)
        return false;
      MetaID metaId = (MetaID) o;
      return this.m_ID == metaId.m_ID && this.m_IsTemp == metaId.m_IsTemp;
    }

    public static implicit operator MetaID(int id) => new MetaID(id, false);
  }
}
