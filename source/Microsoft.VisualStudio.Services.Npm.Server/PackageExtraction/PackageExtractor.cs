// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageExtraction.PackageExtractor
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageExtraction
{
  public class PackageExtractor : IPackageExtractor
  {
    private const string PackageJsonFileName = "/package.json";
    private const string ServerJsFileName = "/server.js";
    private const string BindingGypFileName = "/binding.gyp";
    private const string ReadmeMdFileName = "/readme.md";
    private readonly Stream packageTarballDataStream;
    private readonly Lazy<PackageJson> packageJson;
    private byte[] packageJsonData;
    private byte[] readmeMdData;
    private bool? serverJsFileExists;
    private bool? bindingGypFileExists;
    private string readmeMdPath;
    private bool readmeMdPathInitialized;
    private bool readmeMdBytesInitialized;

    public PackageExtractor(
      Stream packageTarballStream,
      ITracerService tracerService,
      string packageNameForLogging,
      string packageVersionForLogging,
      string upstreamLocationForLogging,
      string callerSourceLocationContextForLogging)
    {
      PackageExtractor packageExtractor = this;
      if (packageTarballStream == null)
        throw new InvalidPackageTarballException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageTarball());
      packageTarballStream.Seek(0L, SeekOrigin.Begin);
      this.packageTarballDataStream = packageTarballStream;
      this.packageJsonData = (byte[]) null;
      this.readmeMdData = (byte[]) null;
      this.readmeMdPath = (string) null;
      this.packageJson = new Lazy<PackageJson>((Func<PackageJson>) (() =>
      {
        try
        {
          // ISSUE: explicit non-virtual call
          return PackageJsonUtils.DeserializeNpmJsonDocument<PackageJson>(DeflateCompressibleBytes.FromUncompressedBytes(__nonvirtual (packageExtractor.GetPackageJsonBytes())), tracerService, packageNameForLogging, packageVersionForLogging, upstreamLocationForLogging, callerSourceLocationContextForLogging, out IReadOnlyList<Exception> _);
        }
        catch (JsonException ex)
        {
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageJson(), (Exception) ex);
        }
      }));
    }

    public byte[] GetPackageJsonBytes()
    {
      if (this.packageJsonData == null)
        this.InitializeValuesFromTarball();
      return this.packageJsonData;
    }

    public PackageJson GetPackageJson() => this.packageJson.Value;

    public bool PackageHasServerJsFileAtRoot()
    {
      if (!this.serverJsFileExists.HasValue)
        this.InitializeValuesFromTarball();
      return this.serverJsFileExists.Value;
    }

    public bool PackageHasBindingGypFileAtRoot()
    {
      if (!this.bindingGypFileExists.HasValue)
        this.InitializeValuesFromTarball();
      return this.bindingGypFileExists.Value;
    }

    public byte[] GetReadmeMdBytes()
    {
      if (!this.readmeMdBytesInitialized)
        this.InitializeValuesFromTarball();
      return this.readmeMdData;
    }

    public string GetReadmeMdPath()
    {
      if (!this.readmeMdPathInitialized)
        this.InitializeValuesFromTarball();
      return this.readmeMdPath;
    }

    [MemberNotNull(new string[] {"bindingGypFileExists", "serverJsFileExists", "packageJsonData"})]
    private void InitializeValuesFromTarball()
    {
      bool flag = false;
      this.serverJsFileExists = new bool?(false);
      this.bindingGypFileExists = new bool?(false);
      this.readmeMdPathInitialized = false;
      this.readmeMdBytesInitialized = false;
      this.packageTarballDataStream.Seek(0L, SeekOrigin.Begin);
      Stream stream1 = (Stream) null;
      try
      {
        int num = this.IsGZipStream(this.packageTarballDataStream) ? 1 : 0;
        this.packageTarballDataStream.Seek(0L, SeekOrigin.Begin);
        if (num != 0)
        {
          stream1 = (Stream) new GZipInputStream(this.packageTarballDataStream);
          ((InflaterInputStream) stream1).IsStreamOwner = false;
        }
        using (TarInputStream stream2 = new TarInputStream(num != 0 ? stream1 : this.packageTarballDataStream, GZipConstants.Encoding))
        {
          stream2.IsStreamOwner = false;
          string empty = string.Empty;
          stream2.SetEntryFactory((TarInputStream.IEntryFactory) new FullPathTarEntryFactory());
          TarEntry nextEntry;
          while ((nextEntry = stream2.GetNextEntry()) != null)
          {
            if (!nextEntry.IsDirectory && nextEntry.Name.Count<char>((Func<char, bool>) (c => c == '/')) == 1)
            {
              if (nextEntry.Name.EndsWith("/package.json"))
              {
                this.packageJsonData = this.ExtractBytesFromEntry(stream2, nextEntry, out string _);
                flag = true;
              }
              else if (!this.serverJsFileExists.Value && nextEntry.Name.EndsWith("/server.js", StringComparison.InvariantCultureIgnoreCase))
                this.serverJsFileExists = new bool?(true);
              else if (!this.bindingGypFileExists.Value && nextEntry.Name.EndsWith("/binding.gyp", StringComparison.InvariantCultureIgnoreCase))
                this.bindingGypFileExists = new bool?(true);
              else if (nextEntry.Name.EndsWith("/readme.md", StringComparison.InvariantCultureIgnoreCase))
                this.readmeMdData = this.ExtractBytesFromEntry(stream2, nextEntry, out this.readmeMdPath);
            }
          }
          if (!flag)
            throw new InvalidPackageTarballException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_NoPackageJsonInTarball());
          this.readmeMdPathInitialized = true;
          this.readmeMdBytesInitialized = true;
        }
      }
      catch (SharpZipBaseException ex)
      {
        throw new InvalidPackageTarballException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageTarball(), (Exception) ex);
      }
      finally
      {
        stream1?.Dispose();
      }
    }

    private bool IsGZipStream(Stream stream)
    {
      byte[] buffer = new byte[2];
      stream.Read(buffer, 0, 2);
      return buffer[0] == (byte) 31 && buffer[1] == (byte) 139;
    }

    private byte[] ExtractBytesFromEntry(
      TarInputStream stream,
      TarEntry entry,
      out string entryPath)
    {
      using (MemoryStream outputStream = new MemoryStream())
      {
        outputStream.Position = 0L;
        outputStream.SetLength(0L);
        stream.CopyEntryContents((Stream) outputStream);
        entryPath = entry.Name;
        return outputStream.ToArray();
      }
    }
  }
}
