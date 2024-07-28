// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FileAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class FileAttributes
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1083062, "Indexing Pipeline", "IndexingOperation");
    private string m_normalizedFilePath;
    private Hash m_filePathHash;
    private readonly DocumentContractType m_contractType;

    public FileAttributes(
      string filePath,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      this.OriginalFilePath = !string.IsNullOrWhiteSpace(filePath) ? filePath : throw new ArgumentNullException(nameof (filePath));
      this.IndexingUnitType = indexingUnitType;
      this.m_contractType = contractType;
    }

    public string OriginalFilePath { get; private set; }

    public string NormalizedFilePath
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.m_normalizedFilePath))
        {
          if (this.m_contractType.IsNoPayloadContract())
          {
            this.m_normalizedFilePath = FileAttributes.GetNormalizedFilePathForNoPayloadContract(this.OriginalFilePath);
          }
          else
          {
            this.m_normalizedFilePath = this.OriginalFilePath.NormalizePathWithoutTrimmingSlashesWithoutChangingCase();
            this.m_normalizedFilePath = this.m_normalizedFilePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace("//", "/");
          }
        }
        return this.m_normalizedFilePath;
      }
    }

    public Hash SHA2Hash
    {
      get
      {
        if (this.m_filePathHash == (Hash) null)
          this.m_filePathHash = FileAttributes.GetSHA2Hash(this.OriginalFilePath);
        return this.m_filePathHash;
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Looks like the parameter was used earlier and is just commented out. To avoid making more changes when the parameter is used again, supressing this rule.")]
    public string GetFilePathForMergingDocMetadata(bool isLargeRepo)
    {
      if (this.OriginalFilePath != this.NormalizedFilePath)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(FileAttributes.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Mismatch in OriginalFilePath: {0} and NormalizedFilePath: {1}.", (object) this.OriginalFilePath, (object) this.NormalizedFilePath)));
      return this.NormalizedFilePath;
    }

    public bool CheckFilePathStartsWith(string prefix)
    {
      string str = this.NormalizedFilePath.Trim('/');
      return this.IndexingUnitType == "TFVC_Repository" ? str.StartsWith(prefix, FileAttributes.GetTfvcFilePathStringComparer(this.m_contractType)) : str.StartsWith(prefix, this.GetFilePathComparisonBasedOnContractType());
    }

    public override bool Equals(object obj)
    {
      if (obj is FileAttributes fileAttributes2)
      {
        if (this == fileAttributes2)
          return true;
        if (FileAttributes.ValidateParameterMatch(this, fileAttributes2))
          return this.IndexingUnitType == "TFVC_Repository" ? string.Equals(this.NormalizedFilePath, fileAttributes2.NormalizedFilePath, FileAttributes.GetTfvcFilePathStringComparer(this.m_contractType)) : string.Equals(this.NormalizedFilePath, fileAttributes2.NormalizedFilePath, this.GetFilePathComparisonBasedOnContractType());
      }
      return false;
    }

    public int Compare(FileAttributes fileAttributes)
    {
      if (fileAttributes == null)
        throw new ArgumentException("fileAttributes cannot be null");
      if (!FileAttributes.ValidateParameterMatch(this, fileAttributes))
        throw new SearchServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "fileAttributes parameters doesn't match, check for {0} and {1} mismatch", (object) "IndexingUnitType", (object) "m_contractType"));
      return this.IndexingUnitType == "TFVC_Repository" ? string.Compare(this.NormalizedFilePath, fileAttributes.NormalizedFilePath, FileAttributes.GetTfvcFilePathStringComparer(this.m_contractType)) : string.Compare(this.NormalizedFilePath, fileAttributes.NormalizedFilePath, this.GetFilePathComparisonBasedOnContractType());
    }

    public int CompareOrdinal(FileAttributes fileAttributes)
    {
      if (fileAttributes == null)
        throw new ArgumentException("fileAttributes cannot be null");
      return FileAttributes.ValidateParameterMatch(this, fileAttributes) ? string.Compare(this.NormalizedFilePath, fileAttributes.NormalizedFilePath, StringComparison.Ordinal) : throw new SearchServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "fileAttributes parameters doesn't match, check for {0} and {1} mismatch", (object) "IndexingUnitType", (object) "m_contractType"));
    }

    public override int GetHashCode()
    {
      if (this.IndexingUnitType == "TFVC_Repository")
        return (this.NormalizedFilePath.ToLower(CultureInfo.CurrentCulture) + this.IndexingUnitType + (object) this.m_contractType).GetHashCode();
      return this.m_contractType.GetDocumentContractTypeCategory() == "DedupeFileContract" ? (this.NormalizedFilePath.ToLowerInvariant() + this.IndexingUnitType + (object) this.m_contractType).GetHashCode() : (this.NormalizedFilePath + this.IndexingUnitType + (object) this.m_contractType).GetHashCode();
    }

    public static Hash GetSHA2Hash(string filePath)
    {
      using (SHA256 shA256 = SHA256.Create())
        return new Hash(shA256.ComputeHash(Encoding.UTF8.GetBytes(filePath)));
    }

    public static StringComparer GetTfvcMetadataSinkComparer(DocumentContractType contractType) => !contractType.IsNoPayloadContract() ? StringComparer.InvariantCultureIgnoreCase : StringComparer.Ordinal;

    public static StringComparison GetTfvcFilePathStringComparer(DocumentContractType contractType) => !contractType.IsNoPayloadContract() ? StringComparison.CurrentCultureIgnoreCase : StringComparison.Ordinal;

    public static string GetNormalizedFilePathForNoPayloadContract(string filePath)
    {
      if (filePath == null)
        throw new ArgumentNullException(nameof (filePath));
      return filePath.Replace(Path.DirectorySeparatorChar, CommonConstants.DirectorySeparatorCharacter);
    }

    private StringComparison GetFilePathComparisonBasedOnContractType() => this.m_contractType == DocumentContractType.DedupeFileContractV3 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    private static bool ValidateParameterMatch(
      FileAttributes fileAttributes1,
      FileAttributes fileAttributes2)
    {
      if (fileAttributes1 == null || fileAttributes2 == null)
        throw new ArgumentException("fileAttributes cannot be null");
      return fileAttributes1.IndexingUnitType == fileAttributes2.IndexingUnitType && fileAttributes1.m_contractType == fileAttributes2.m_contractType;
    }

    public string GetFilePathOrHash() => this.m_contractType == DocumentContractType.DedupeFileContractV4 || this.m_contractType == DocumentContractType.DedupeFileContractV5 ? this.SHA2Hash.HexHash : this.NormalizedFilePath;

    public string IndexingUnitType { get; }

    public DocumentContractType DocumentContractType => this.m_contractType;
  }
}
