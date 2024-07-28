// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ServerItemLocalVersionUpdate
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ServerItemLocalVersionUpdate : BaseLocalVersionUpdate
  {
    private ItemPathPair m_sourceItemPathPair;
    private int m_itemId;

    [XmlAttribute("sitem")]
    public string SourceServerItem
    {
      get => this.SourceItemPathPair.ProjectNamePath;
      set => this.SourceItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair SourceItemPathPair
    {
      get => this.m_sourceItemPathPair;
      set => this.m_sourceItemPathPair = value;
    }

    [XmlAttribute("itemid")]
    [DefaultValue(0)]
    [ClientProperty(Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int ItemId
    {
      get => this.m_itemId;
      set => this.m_itemId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "IID[{0}];{1} = {2}", (object) this.SourceServerItem, (object) this.LocalVersion, (object) this.TargetLocalItem);

    internal override void ValidateInternal(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      base.ValidateInternal(versionControlRequestContext, parameterName);
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      string sourceServerItem = this.SourceServerItem;
      versionControlRequestContext.Validation.checkServerItem(ref sourceServerItem, parameterName + ".ServerItem", false, false, true, false, serverPathLength);
      if (sourceServerItem == null)
        return;
      this.m_sourceItemPathPair = new ItemPathPair(sourceServerItem, this.SourceItemPathPair.ProjectGuidPath);
    }

    internal override void SetRecord(SqlDataRecord record)
    {
      record.SetDBNull(1);
      record.SetInt32(2, this.LocalVersion);
      record.SetString(3, DBPath.ServerToDatabasePath(this.SourceServerItem));
      if (string.IsNullOrEmpty(this.TargetLocalItem))
        record.SetDBNull(4);
      else
        record.SetString(4, DBPath.LocalToDatabasePath(this.TargetLocalItem));
    }

    internal override void SetRecord3(SqlDataRecord record)
    {
      string databasePath1 = DBPath.ServerToDatabasePath(this.SourceServerItem);
      record.SetDBNull(1);
      record.SetInt32(2, this.LocalVersion);
      record.SetString(3, databasePath1);
      if (string.IsNullOrEmpty(this.TargetLocalItem))
      {
        record.SetDBNull(4);
        record.SetDBNull(5);
        record.SetDBNull(6);
      }
      else
      {
        string databasePath2 = DBPath.LocalToDatabasePath(this.TargetLocalItem);
        int trailingPathLength = ServerItemLocalVersionUpdate.GetCommonTrailingPathLength(databasePath1, databasePath2);
        record.SetString(4, databasePath2);
        record.SetInt16(5, (short) (databasePath1.Length - trailingPathLength));
        record.SetInt16(6, (short) (databasePath2.Length - trailingPathLength));
      }
    }

    internal override void SetRecord4(SqlDataRecord record)
    {
      string databasePath1 = DBPath.ServerToDatabasePath(this.SourceServerItem);
      record.SetDBNull(2);
      record.SetInt32(3, this.LocalVersion);
      record.SetString(4, databasePath1);
      if (string.IsNullOrEmpty(this.TargetLocalItem))
      {
        record.SetDBNull(5);
        record.SetDBNull(6);
        record.SetDBNull(7);
      }
      else
      {
        string databasePath2 = DBPath.LocalToDatabasePath(this.TargetLocalItem);
        int trailingPathLength = ServerItemLocalVersionUpdate.GetCommonTrailingPathLength(databasePath1, databasePath2);
        record.SetString(5, databasePath2);
        record.SetInt16(6, (short) (databasePath1.Length - trailingPathLength));
        record.SetInt16(7, (short) (databasePath2.Length - trailingPathLength));
      }
    }

    private static int GetCommonTrailingPathLength(string serverPath, string localPath)
    {
      int index1 = serverPath.Length - 1;
      for (int index2 = localPath.Length - 1; index1 >= 0 && index2 >= 0; --index2)
      {
        int num = (int) serverPath[index1];
        if ((int) serverPath[index1] == (int) localPath[index2])
          --index1;
        else
          break;
      }
      return serverPath.Length - index1 - 1;
    }
  }
}
