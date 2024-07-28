// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.FieldValueHelpers
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class FieldValueHelpers
  {
    private static readonly Regex s_invalidCharactersCheck = new Regex("[\0-\b\v\f\u000E-\u001F\u007F-\u009F]", RegexOptions.Compiled);

    public static object GetInternalValue(object value, InternalFieldType fieldType)
    {
      object internalValue;
      int num = (int) FieldValueHelpers.TryConvertValueToInternal(value, fieldType, out internalValue);
      return internalValue;
    }

    public static FieldStatusFlags TryConvertValueToInternal(
      object value,
      InternalFieldType fieldType,
      out object internalValue)
    {
      FieldStatusFlags fieldStatusFlags = FieldStatusFlags.None;
      internalValue = value;
      switch (value)
      {
        case null:
label_35:
          return fieldStatusFlags;
        case string str when string.IsNullOrWhiteSpace(str):
          internalValue = (object) null;
          goto case null;
        case ServerDefaultFieldValue _:
        case TrendDataValue _:
        case WorkItemIdentity _:
          internalValue = value;
          goto case null;
        default:
          try
          {
            switch (fieldType)
            {
              case InternalFieldType.String:
              case InternalFieldType.PlainText:
              case InternalFieldType.Html:
              case InternalFieldType.TreePath:
              case InternalFieldType.History:
                if (value is UnknownIdentity || value is AmbiguousIdentity)
                  return FieldStatusFlags.InvalidIdentityField;
                if (str == null)
                  str = !(value is WorkItemCommentUpdate itemCommentUpdate) ? Convert.ToString(value) : itemCommentUpdate.Text;
                internalValue = (object) str;
                if (str != null)
                {
                  if (ArgumentUtility.HasMismatchedSurrogates(str))
                    return FieldStatusFlags.InvalidSpecialChars;
                  if (fieldType == InternalFieldType.String)
                  {
                    if (str.Length > 256)
                      return FieldStatusFlags.InvalidTooLong;
                    if (FieldValueHelpers.s_invalidCharactersCheck.IsMatch(str))
                      return FieldStatusFlags.InvalidSpecialChars;
                    goto label_35;
                  }
                  else
                    goto label_35;
                }
                else
                  goto label_35;
              case InternalFieldType.Integer:
                internalValue = (object) Convert.ToInt32(value);
                goto label_35;
              case InternalFieldType.DateTime:
                switch (value)
                {
                  case DateTime result1:
label_24:
                    DateTime universalTime = result1.ToUniversalTime();
                    if (universalTime < SqlDateTime.MinValue.Value)
                      return FieldStatusFlags.InvalidDate;
                    internalValue = (object) universalTime;
                    goto label_35;
                  case DateTimeOffset dateTimeOffset:
                    result1 = dateTimeOffset.DateTime;
                    goto label_24;
                  case string _:
                    string str1 = (string) value;
                    if (!DateTime.TryParse(str1, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out result1))
                    {
                      DateTimeOffset result;
                      if (!DateTimeOffset.TryParse(str1, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out result))
                        return FieldStatusFlags.InvalidType;
                      result1 = result.DateTime;
                      goto label_24;
                    }
                    else
                      goto label_24;
                  default:
                    return FieldStatusFlags.InvalidType;
                }
              case InternalFieldType.Double:
                internalValue = (object) Convert.ToDouble(value);
                goto label_35;
              case InternalFieldType.Guid:
                switch (value)
                {
                  case string _:
                    Guid result2;
                    if (string.IsNullOrWhiteSpace(str) || !Guid.TryParse(str, out result2))
                      return FieldStatusFlags.InvalidType;
                    internalValue = (object) result2;
                    goto label_35;
                  case Guid guid:
                    internalValue = (object) guid;
                    goto label_35;
                  default:
                    goto label_35;
                }
              case InternalFieldType.Boolean:
                internalValue = !(value is string str2) || !(str2 == "1") ? (str2 == null || !(str2 == "0") ? (object) Convert.ToBoolean(value) : (object) false) : (object) true;
                goto label_35;
              default:
                goto label_35;
            }
          }
          catch
          {
            return FieldStatusFlags.InvalidType;
          }
      }
    }
  }
}
