// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class FieldSetting : Dictionary<string, string>, IComparable<FieldSetting>
  {
    public static string FIELD_IDENTIFIER = "fieldIdentifier";
    public static string ROW_NUMBER = "rowNumber";

    public int CompareTo(FieldSetting other) => this.RowNumber.CompareTo(other.RowNumber);

    private string GetPropertyValueSafe(string key)
    {
      string propertyValueSafe = (string) null;
      if (this.ContainsKey(key))
        propertyValueSafe = this[key];
      if (propertyValueSafe == null)
        propertyValueSafe = "";
      return propertyValueSafe;
    }

    public string FieldIdentifier
    {
      get => this.GetPropertyValueSafe(FieldSetting.FIELD_IDENTIFIER);
      set => this[FieldSetting.FIELD_IDENTIFIER] = value;
    }

    public int RowNumber
    {
      get => int.Parse(this.GetPropertyValueSafe(FieldSetting.ROW_NUMBER), (IFormatProvider) CultureInfo.InvariantCulture);
      set => this[FieldSetting.ROW_NUMBER] = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public FieldSetting()
      : base((IEqualityComparer<string>) VssStringComparer.PropertyName)
    {
    }

    public virtual bool isValid() => true;

    public FieldSetting(string identifier, List<KeyValuePair<string, string>> propertyValues)
      : this()
    {
      this.FieldIdentifier = identifier;
      foreach (KeyValuePair<string, string> propertyValue in propertyValues)
      {
        if (!this.ContainsKey(propertyValue.Key))
          this.Add(propertyValue.Key, propertyValue.Value);
      }
    }
  }
}
