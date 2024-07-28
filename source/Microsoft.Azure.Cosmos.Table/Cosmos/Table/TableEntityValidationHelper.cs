// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableEntityValidationHelper
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Cosmos.Table
{
  internal class TableEntityValidationHelper
  {
    private const string ValidPropertyNameFirstCharacterRegex = "_|[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}]";
    private const string ValidPropertyNameRemainingCharacterRegex = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}-]";
    private static Regex ValidPropertyNameRegex = new Regex("^(_|[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}])([\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}-])*$", RegexOptions.Compiled);
    private static readonly List<string> UnsupportedPropertyNames = new List<string>()
    {
      "id",
      "ttl"
    };
    private static Regex PartitionKeyRegex = new Regex("^[^\\\\/#?\\u0000-\\u001F\\u007F-\\u009F]+$", RegexOptions.Compiled);
    private static Regex RowKeyRegex = new Regex("^[^\\\\/#?\\u0000-\\u001F\\u007F-\\u009F]*[^ \\\\/#?\\u0000-\\u001F\\u007F-\\u009F]+$", RegexOptions.Compiled);

    public static bool IsValidPropertyName(string propertyName) => TableEntityValidationHelper.ValidPropertyNameRegex.IsMatch(propertyName);

    public static bool IsUnsupportedPropertyName(string propertyName) => TableEntityValidationHelper.UnsupportedPropertyNames.Contains<string>(propertyName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public static void ValidatePartitionKey(string partitionKey)
    {
      if (string.IsNullOrWhiteSpace(partitionKey))
        throw new TableInvalidInputException(TableErrorCodeStrings.PropertiesNeedValue, TableResources.PartitionKeyOrRowKeyEmpty);
      if (partitionKey.Length > 1024)
      {
        string errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TableResources.PartitionKeyTooLarge, (object) 1024);
        throw new TableInvalidInputException(TableErrorCodeStrings.KeyValueTooLarge, errorMessage);
      }
      if (!TableEntityValidationHelper.PartitionKeyRegex.IsMatch(partitionKey))
      {
        string errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TableResources.OutOfRangeInput, (object) "PartitionKey", (object) partitionKey);
        throw new TableInvalidInputException(TableErrorCodeStrings.OutOfRangeInput, errorMessage);
      }
    }

    public static void ValidateRowKey(string rowKey)
    {
      if (string.IsNullOrWhiteSpace(rowKey))
        throw new TableInvalidInputException(TableErrorCodeStrings.PropertiesNeedValue, TableResources.PartitionKeyOrRowKeyEmpty);
      if (rowKey.Length > 254)
      {
        string errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TableResources.RowKeyTooLarge, (object) 254);
        throw new TableInvalidInputException(TableErrorCodeStrings.KeyValueTooLarge, errorMessage);
      }
      if (!TableEntityValidationHelper.RowKeyRegex.IsMatch(rowKey))
      {
        string errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TableResources.OutOfRangeInput, (object) "RowKey", (object) rowKey);
        throw new TableInvalidInputException(TableErrorCodeStrings.OutOfRangeInput, errorMessage);
      }
    }
  }
}
