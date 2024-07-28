// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.FileMetadataRecord
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class FileMetadataRecord
  {
    public long Id { get; set; }

    public string FilePath { get; set; }

    public BranchMetadata BranchMetadata { get; set; }

    public FileMetadataRecord()
    {
    }

    public FileMetadataRecord(FileMetadataRecord record)
    {
      this.Id = record != null ? record.Id : throw new ArgumentNullException(nameof (record));
      this.FilePath = record.FilePath;
      this.BranchMetadata = (BranchMetadata) null;
      if (record.BranchMetadata?.Metadata == null)
        return;
      this.BranchMetadata = new BranchMetadata()
      {
        Metadata = new ContentHashBranchListDictionary()
      };
      foreach (KeyValuePair<string, Branches> keyValuePair in (Dictionary<string, Branches>) record.BranchMetadata.Metadata)
      {
        Branches branches = new Branches();
        branches.AddRange((IEnumerable<string>) keyValuePair.Value);
        this.BranchMetadata.Metadata[keyValuePair.Key] = branches;
      }
    }

    public override bool Equals(object obj) => obj is FileMetadataRecord fileMetadataRecord && fileMetadataRecord.FilePath == this.FilePath;

    public override int GetHashCode() => this.FilePath.GetHashCode();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ID: {0} ", (object) this.Id));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FilePath: {0} ", (object) this.FilePath));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BranchMetadata: {0} ", (object) Serializers.ToXmlString((object) this.BranchMetadata, typeof (BranchMetadata))));
      return stringBuilder.ToString();
    }
  }
}
