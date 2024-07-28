// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionIdentifier
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionIdentifier
  {
    public int PartitionIndex { get; set; }

    public int ServiceIndex { get; set; }

    public override bool Equals(object obj) => obj is PartitionIdentifier partitionIdentifier && partitionIdentifier.PartitionIndex == this.PartitionIndex && partitionIdentifier.ServiceIndex == this.ServiceIndex;

    public override int GetHashCode() => this.ToString().GetHashCode();

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", (object) this.PartitionIndex, (object) this.ServiceIndex);

    public static PartitionIdentifier FromPartitionInfo(PartitionInfo partitionInfo) => new PartitionIdentifier()
    {
      PartitionIndex = partitionInfo.PartitionIndex,
      ServiceIndex = partitionInfo.ServiceIndex
    };

    public static bool TryParse(
      string partitionIdentifierString,
      out PartitionIdentifier partitionIdentifier)
    {
      partitionIdentifier = (PartitionIdentifier) null;
      if (!string.IsNullOrEmpty(partitionIdentifierString))
      {
        string[] strArray = partitionIdentifierString.Split(new char[1]
        {
          '@'
        }, StringSplitOptions.RemoveEmptyEntries);
        int result1;
        int result2;
        if (strArray.Length == 2 && int.TryParse(strArray[0], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1) && int.TryParse(strArray[1], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
        {
          partitionIdentifier = new PartitionIdentifier()
          {
            PartitionIndex = result1,
            ServiceIndex = result2
          };
          return true;
        }
      }
      return false;
    }
  }
}
