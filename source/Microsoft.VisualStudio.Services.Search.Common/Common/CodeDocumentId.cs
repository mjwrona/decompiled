// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CodeDocumentId
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class CodeDocumentId : DocumentId
  {
    private readonly string m_documentId;

    public CodeDocumentId(
      DocumentContractType contractType,
      Guid repositoryId,
      string contentId,
      FileAttributes fileAttributes,
      List<string> branches)
      : base(CodeDocumentId.GetNormalizedFilePath(fileAttributes))
    {
      this.m_documentId = CodeDocumentId.GetDocumentId(contractType, repositoryId, contentId, fileAttributes, branches);
    }

    public override int GetHashCode() => this.m_documentId.GetHashCode();

    public override bool Equals(object obj) => obj is CodeDocumentId codeDocumentId && this.m_documentId == codeDocumentId.m_documentId;

    public override string ToString() => this.m_documentId;

    private static string GetNormalizedFilePath(FileAttributes fileAttributes) => fileAttributes != null ? fileAttributes.NormalizedFilePath : throw new ArgumentNullException(nameof (fileAttributes));

    private static string GetDocumentId(
      DocumentContractType contractType,
      Guid repositoryId,
      string contentId,
      FileAttributes fileAttributes,
      List<string> branches)
    {
      switch (contractType)
      {
        case DocumentContractType.SourceNoDedupeFileContractV3:
          if (branches.Count > 1)
            throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} does not support multi branch indexing.", (object) contractType)));
          return repositoryId.ToString() + "@" + CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", branches[0]) + "@" + fileAttributes.NormalizedFilePath;
        case DocumentContractType.DedupeFileContractV3:
          return repositoryId.ToString() + "@" + contentId.NormalizeString() + "@" + fileAttributes.NormalizedFilePath.ToLowerInvariant();
        case DocumentContractType.SourceNoDedupeFileContractV4:
        case DocumentContractType.SourceNoDedupeFileContractV5:
          if (branches.Count > 1)
            throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} does not support multi branch indexing.", (object) contractType)));
          string pathToComputeHash1 = CodeDocumentId.GetFilePathToComputeHash(fileAttributes);
          return repositoryId.ToString().ToLowerInvariant() + "_" + CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", branches[0]) + "_" + FileAttributes.GetSHA2Hash(pathToComputeHash1).HexHash.ToLowerInvariant();
        case DocumentContractType.DedupeFileContractV4:
        case DocumentContractType.DedupeFileContractV5:
          string pathToComputeHash2 = CodeDocumentId.GetFilePathToComputeHash(fileAttributes);
          return repositoryId.ToString().ToLowerInvariant() + "_" + contentId.ToLowerInvariant() + "_" + FileAttributes.GetSHA2Hash(pathToComputeHash2).HexHash.ToLowerInvariant();
        default:
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is not a valid contract for {1} entity.", (object) contractType, (object) CodeEntityType.GetInstance().Name)));
      }
    }

    public static string GetFilePathToComputeHash(FileAttributes fileAttributes)
    {
      if (fileAttributes == null)
        throw new ArgumentNullException(nameof (fileAttributes));
      if (!(fileAttributes.IndexingUnitType == "TFVC_Repository"))
        return fileAttributes.OriginalFilePath;
      int count = fileAttributes.OriginalFilePath.IndexOf(CommonConstants.DirectorySeparatorCharacter, "$/".Length);
      if (count > 0 && count != "$/".Length && count != fileAttributes.OriginalFilePath.Length - 1)
        return fileAttributes.OriginalFilePath.Remove(0, count);
      throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("{0} doesn't have a valid File Path.", (object) fileAttributes.OriginalFilePath)));
    }
  }
}
