// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LocalVersionUpdate
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class LocalVersionUpdate : BaseLocalVersionUpdate
  {
    private int m_itemId;

    [XmlAttribute("itemid")]
    [DefaultValue(0)]
    public int ItemId
    {
      get => this.m_itemId;
      set => this.m_itemId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "IID[{0}];{1} = {2}", (object) this.ItemId, (object) this.LocalVersion, (object) this.TargetLocalItem);

    internal override void SetRecord(SqlDataRecord record)
    {
      record.SetInt32(1, this.ItemId);
      record.SetInt32(2, this.LocalVersion);
      record.SetDBNull(3);
      if (string.IsNullOrEmpty(this.TargetLocalItem))
        record.SetDBNull(4);
      else
        record.SetString(4, DBPath.LocalToDatabasePath(this.TargetLocalItem));
    }

    internal override void SetRecord3(SqlDataRecord record)
    {
      this.SetRecord(record);
      record.SetDBNull(5);
      record.SetDBNull(6);
    }

    internal override void SetRecord4(SqlDataRecord record)
    {
      record.SetDBNull(1);
      record.SetInt32(2, this.ItemId);
      record.SetInt32(3, this.LocalVersion);
      record.SetDBNull(4);
      if (string.IsNullOrEmpty(this.TargetLocalItem))
        record.SetDBNull(5);
      else
        record.SetString(5, DBPath.LocalToDatabasePath(this.TargetLocalItem));
      record.SetDBNull(6);
      record.SetDBNull(7);
    }
  }
}
