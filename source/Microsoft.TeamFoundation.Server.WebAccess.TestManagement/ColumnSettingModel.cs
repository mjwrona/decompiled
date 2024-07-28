// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ColumnSettingModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class ColumnSettingModel
  {
    [DataMember(Name = "refName")]
    public string RefName { get; set; }

    [DataMember(Name = "width")]
    public int Width { get; set; }

    public override string ToString() => string.Format("{0};{1}", (object) this.RefName, (object) this.Width.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));

    public static ColumnSettingModel FromString(string columnInfo)
    {
      if (string.IsNullOrWhiteSpace(columnInfo))
        return ColumnSettingModel.Default;
      string[] strArray = columnInfo.Split(';');
      int result = 0;
      if (strArray.Length != 2 || !int.TryParse(strArray[1], out result))
        return ColumnSettingModel.Default;
      return new ColumnSettingModel()
      {
        RefName = strArray[0],
        Width = result
      };
    }

    public static ColumnSettingModel Default => new ColumnSettingModel()
    {
      RefName = string.Empty,
      Width = 0
    };
  }
}
