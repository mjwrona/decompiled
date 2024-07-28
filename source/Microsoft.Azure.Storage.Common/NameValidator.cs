// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.NameValidator
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Storage
{
  public static class NameValidator
  {
    private const int BlobFileDirectoryMinLength = 1;
    private const int ContainerShareQueueTableMinLength = 3;
    private const int ContainerShareQueueTableMaxLength = 63;
    private const int FileDirectoryMaxLength = 255;
    private const int BlobMaxLength = 1024;
    private static readonly string[] ReservedFileNames = new string[25]
    {
      ".",
      "..",
      "LPT1",
      "LPT2",
      "LPT3",
      "LPT4",
      "LPT5",
      "LPT6",
      "LPT7",
      "LPT8",
      "LPT9",
      "COM1",
      "COM2",
      "COM3",
      "COM4",
      "COM5",
      "COM6",
      "COM7",
      "COM8",
      "COM9",
      "PRN",
      "AUX",
      "NUL",
      "CON",
      "CLOCK$"
    };
    private static readonly RegexOptions RegexOptions = RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.CultureInvariant;
    private static readonly Regex FileDirectoryRegex = new Regex("^[^\"\\\\/:|<>*?]*\\/{0,1}$", NameValidator.RegexOptions);
    private static readonly Regex ShareContainerQueueRegex = new Regex("^[a-z0-9]+(-[a-z0-9]+)*$", NameValidator.RegexOptions);
    private static readonly Regex TableRegex = new Regex("^[A-Za-z][A-Za-z0-9]*$", NameValidator.RegexOptions);
    private static readonly Regex MetricsTableRegex = new Regex("^\\$Metrics(HourPrimary|MinutePrimary|HourSecondary|MinuteSecondary)?(Transactions)(Blob|Queue|Table)$", NameValidator.RegexOptions);

    public static void ValidateContainerName(string containerName)
    {
      if ("$root".Equals(containerName, StringComparison.Ordinal) || "$logs".Equals(containerName, StringComparison.Ordinal))
        return;
      NameValidator.ValidateShareContainerQueueHelper(containerName, "container");
    }

    public static void ValidateQueueName(string queueName) => NameValidator.ValidateShareContainerQueueHelper(queueName, "queue");

    public static void ValidateShareName(string shareName) => NameValidator.ValidateShareContainerQueueHelper(shareName, "share");

    private static void ValidateShareContainerQueueHelper(string resourceName, string resourceType)
    {
      if (string.IsNullOrWhiteSpace(resourceName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", (object) resourceType));
      if (resourceName.Length < 3 || resourceName.Length > 63)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", (object) resourceType, (object) 3, (object) 63));
      if (!NameValidator.ShareContainerQueueRegex.IsMatch(resourceName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", (object) resourceType));
    }

    public static void ValidateBlobName(string blobName)
    {
      if (string.IsNullOrWhiteSpace(blobName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", (object) "blob"));
      if (blobName.Length < 1 || blobName.Length > 1024)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", (object) "blob", (object) 1, (object) 1024));
      int num = 0;
      foreach (char ch in blobName)
      {
        if (ch == '/')
          ++num;
      }
      if (num >= 254)
        throw new ArgumentException("The count of URL path segments (strings between '/' characters) as part of the blob name cannot exceed 254.");
    }

    public static void ValidateFileName(string fileName)
    {
      NameValidator.ValidateFileDirectoryHelper(fileName, "file");
      if (fileName.EndsWith("/", StringComparison.Ordinal))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", (object) "file"));
      foreach (string reservedFileName in NameValidator.ReservedFileNames)
      {
        if (reservedFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. This {0} name is reserved.", (object) "file"));
      }
    }

    public static void ValidateDirectoryName(string directoryName) => NameValidator.ValidateFileDirectoryHelper(directoryName, "directory");

    private static void ValidateFileDirectoryHelper(string resourceName, string resourceType)
    {
      if (string.IsNullOrWhiteSpace(resourceName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", (object) resourceType));
      if (resourceName.Length < 1 || resourceName.Length > (int) byte.MaxValue)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", (object) resourceType, (object) 1, (object) (int) byte.MaxValue));
      if (!NameValidator.FileDirectoryRegex.IsMatch(resourceName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", (object) resourceType));
    }

    public static void ValidateTableName(string tableName)
    {
      if (string.IsNullOrWhiteSpace(tableName))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", (object) "table"));
      if (tableName.Length < 3 || tableName.Length > 63)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", (object) "table", (object) 3, (object) 63));
      if (!NameValidator.TableRegex.IsMatch(tableName) && !NameValidator.MetricsTableRegex.IsMatch(tableName) && !tableName.Equals("$MetricsCapacityBlob", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", (object) "table"));
    }
  }
}
