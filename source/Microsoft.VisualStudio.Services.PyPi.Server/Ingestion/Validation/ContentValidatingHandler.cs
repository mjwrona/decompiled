// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.ContentValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class ContentValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, 
    #nullable disable
    NullResult>,
    IHaveInputType<
    #nullable enable
    IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<
    #nullable disable
    NullResult>
  {
    private static readonly 
    #nullable enable
    IDictionary<string, string> ExtensionToMetadataFileMap = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        ".zip",
        "PKG-INFO"
      },
      {
        ".egg",
        "PKG-INFO"
      },
      {
        ".whl",
        "WHEEL"
      }
    };

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      Stream stream = request.ProtocolSpecificInfo.PackageFileStream.Stream;
      if (stream == null || !stream.CanSeek)
        return NullResult.NullTask;
      string key = Path.GetExtension(request.ProtocolSpecificInfo.PackageFileStream.FilePath) ?? string.Empty;
      if (!ContentValidatingHandler.ExtensionToMetadataFileMap.ContainsKey(key))
        return NullResult.NullTask;
      stream.Seek(0L, SeekOrigin.Begin);
      string extensionToMetadataFile = ContentValidatingHandler.ExtensionToMetadataFileMap[key];
      if (stream.Length < 22L)
        throw new InvalidPackageException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageTooSmall());
      ZipUtils.ValidateZipEndOfCentralDirectoryIsInRange(stream);
      try
      {
        using (ZipFile zipFile = new ZipFile(stream))
        {
          zipFile.IsStreamOwner = false;
          bool flag = false;
          foreach (ZipEntry zipEntry in zipFile)
          {
            if (zipEntry.CompressionMethod != CompressionMethod.Deflated && zipEntry.CompressionMethod != CompressionMethod.Stored)
              throw new InvalidPackageException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidZipCompressionMethod());
            if (!flag)
            {
              string[] strArray = zipEntry.Name.Split('/');
              if (strArray.Length == 2 && strArray[1].Equals(extensionToMetadataFile, StringComparison.Ordinal))
                flag = true;
            }
          }
          if (flag)
            return NullResult.NullTask;
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_MissingMetadataFile((object) key, (object) extensionToMetadataFile));
        }
      }
      catch (ZipException ex)
      {
        throw new InvalidPackageException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidZipArchive(), (Exception) ex);
      }
      catch (NotSupportedException ex)
      {
        if (ex.Message.ToLowerInvariant().Contains("compression method not supported"))
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidZipCompressionMethod());
        throw;
      }
      finally
      {
        stream.Seek(0L, SeekOrigin.Begin);
      }
    }
  }
}
