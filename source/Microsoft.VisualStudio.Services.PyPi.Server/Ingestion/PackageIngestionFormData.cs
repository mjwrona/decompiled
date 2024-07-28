// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.PackageIngestionFormData
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion
{
  public class PackageIngestionFormData
  {
    public PackageIngestionFormData(
      IReadOnlyDictionary<string, string[]> rawMetadata,
      PackageFileStream packageFileStream,
      PackageFileStream? gpgSignatureFileStream)
    {
      this.RawMetadata = rawMetadata;
      this.PackageFileStream = packageFileStream;
      this.GpgSignatureFileStream = gpgSignatureFileStream;
    }

    public IReadOnlyDictionary<string, string[]> RawMetadata { get; }

    public PackageFileStream PackageFileStream { get; }

    public PackageFileStream? GpgSignatureFileStream { get; }
  }
}
