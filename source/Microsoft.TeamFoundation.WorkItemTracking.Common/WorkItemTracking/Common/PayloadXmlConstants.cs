// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.PayloadXmlConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct PayloadXmlConstants
  {
    public const string Namespace = "http://schemas.microsoft.com/Currituck/2005/01/mtservices/payload";
    public const string TableTag = "table";
    public const string RowsTag = "rows";
    public const string RowTag = "r";
    public const string FieldTag = "f";
    public const string ColumnsTag = "columns";
    public const string ColumnTag = "c";
    public const string ColumnNameTag = "n";
    public const string ColumnTypeTag = "t";
    public const string TableNameTag = "name";
    public const string IndexTag = "k";
  }
}
