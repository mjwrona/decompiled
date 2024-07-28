// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.TempFileMetadataRecord
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Utils;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class TempFileMetadataRecord
  {
    public long Id { get; set; }

    public long? FilePathId { get; set; }

    public FileAttributes FileAttributes { get; set; }

    public TemporaryBranchMetadata TemporaryBranchMetadata { get; set; }

    public byte AttemptCount { get; set; } = 1;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Id: {0} ", (object) this.Id));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FilePath: {0} ", (object) this.FileAttributes.NormalizedFilePath));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TemporaryBranchMetadata: {0} ", (object) Serializers.ToXmlString((object) this.TemporaryBranchMetadata, typeof (TemporaryBranchMetadata))));
      return stringBuilder.ToString();
    }
  }
}
