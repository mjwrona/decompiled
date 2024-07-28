// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseFileProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseFileProperties
  {
    public int FileId { get; set; }

    public DatabaseFileType FileType { get; set; }

    public int DataSpaceId { get; set; }

    public string LogicalName { get; set; }

    public string PhysicalName { get; set; }

    public DatabaseFileState State { get; set; }

    public int SizePages { get; set; }

    public int MaxSizePages { get; set; }

    public int Growth { get; set; }

    public bool IsPercentGrowth { get; set; }

    public bool IsMediaReadOnly { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsSparse { get; set; }

    public string FileGroupName { get; set; }

    public bool InDefaultFileGroup { get; set; }

    public int SizeMB => (int) ((long) this.SizePages * 8L / 1024L);

    public int MaxSizeMB => this.MaxSizePages == -1 ? -1 : (int) ((long) this.MaxSizePages * 8L / 1024L);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File Id: {0}, File Type: {1}, DataSpace Id: {2}, Logical Name: {3}, Physical Name: {4}, State: {5}, Size: {6} MB, Max Size: {7} MB, Growth: {8}, Media Read Only: {9}, Read Only: {10}, Sparse: {11}, Filegroup Name: {12}, In Default Filegroup: {13}", (object) this.FileId, (object) this.FileType, (object) this.DataSpaceId, (object) this.LogicalName, (object) this.PhysicalName, (object) this.State, (object) this.SizeMB, (object) this.MaxSizeMB, (object) (this.IsPercentGrowth ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}%", (object) this.Growth) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} MB", (object) ((double) (this.Growth * 8) / 1024.0))), (object) this.IsMediaReadOnly, (object) this.IsReadOnly, (object) this.IsSparse, (object) (this.FileGroupName ?? "N/A"), this.FileGroupName != null ? (object) this.InDefaultFileGroup : (object) "N/A");
  }
}
