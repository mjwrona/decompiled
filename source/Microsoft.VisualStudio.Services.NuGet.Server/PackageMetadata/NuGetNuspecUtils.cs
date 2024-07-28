// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata.NuGetNuspecUtils
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata
{
  public static class NuGetNuspecUtils
  {
    public static byte[] GetNuspecBytes(Stream nupkgStream)
    {
      ArgumentUtility.CheckForNull<Stream>(nupkgStream, nameof (nupkgStream));
      try
      {
        if (nupkgStream.Length < 22L)
          throw InvalidPackageExceptionHelper.PackageTooSmall();
        using (ZipArchive zipArchive = new ZipArchive(nupkgStream, ZipArchiveMode.Read, true))
        {
          ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (x => x.Name.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase) && x.FullName.IndexOf('/') == -1));
          if (zipArchiveEntry == null)
            throw InvalidPackageExceptionHelper.CouldntFindNuspec();
          using (Stream stream = zipArchiveEntry.Open())
          {
            byte[] buffer = new byte[zipArchiveEntry.Length];
            int offset = 0;
            while (offset < buffer.Length)
              offset += stream.Read(buffer, offset, buffer.Length - offset);
            return buffer;
          }
        }
      }
      catch (InvalidDataException ex)
      {
        if (ex.InnerException is IOException)
          throw ex.InnerException;
        throw InvalidPackageExceptionHelper.PackageDataIsCorrupt(ex.Message, ex.InnerException);
      }
      finally
      {
        nupkgStream.Seek(0L, SeekOrigin.Begin);
      }
    }

    public static XDocument LoadXDocumentFromBytes(byte[] bytes)
    {
      using (MemoryStream memoryStream = new MemoryStream(bytes, false))
      {
        try
        {
          return XDocument.Load((Stream) memoryStream);
        }
        catch (XmlException ex)
        {
          throw InvalidPackageExceptionHelper.UnreadableNuspec((Exception) ex);
        }
      }
    }
  }
}
