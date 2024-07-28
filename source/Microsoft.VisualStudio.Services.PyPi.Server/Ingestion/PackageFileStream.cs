// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.PackageFileStream
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using System.IO;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion
{
  public record PackageFileStream(
    string FilePath,
    long Length,
    Stream? Stream,
    BlobIdentifier? ExistingBlobIdentifier)
  ;
}
