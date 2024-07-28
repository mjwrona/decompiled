// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.VersionControlProvider
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Diff;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public abstract class VersionControlProvider
  {
    public const int c_maxHistoryEntries = 10000;
    public const int c_maxHistoryItemCount = 10000;
    public const int c_partialDiffLinePadding = 3;
    private const int c_defaultMaxLineDiffOperationMs = 20000;
    private const int c_defaultMaxWordDiffOperationMs = 10000;

    protected VersionControlProvider(IVssRequestContext requestContext, string repositoryName)
    {
      this.RequestContext = requestContext;
      this.RepositoryName = repositoryName;
    }

    public string RepositoryName { get; protected set; }

    internal IVssRequestContext RequestContext { get; private set; }

    public abstract Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      string path,
      string version,
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions details);

    public abstract Stream GetFileContentStream(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file);

    public abstract StoredFile GetFileContentStreamWithMetadata(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file);

    public abstract byte[] GetFileHashValueNoContent(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file);

    public abstract HistoryQueryResults QueryHistory(ChangeListSearchCriteria searchCriteria);

    public abstract ChangeList GetChangeList(string version, int maxNumberOfChanges);

    public abstract ChangeQueryResults GetChangeListChanges(
      string version,
      int maxNumberOfChanges,
      int skipCount);

    public abstract IEnumerable<int> GetLinkedWorkItemIds(string[] versions);

    public abstract IEnumerable<TeamIdentityReference> GetAuthors();

    protected abstract void SetSecuredObject(VersionControlSecuredObject securableObject);

    public Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      string path,
      string version)
    {
      return this.GetItem(path, version, false);
    }

    public Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      string path,
      string version,
      bool includeContentMetadata)
    {
      return this.GetItem(path, version, includeContentMetadata, 0L);
    }

    public Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      string path,
      string version,
      bool includeContentMetadata,
      long scanBytesForEncoding)
    {
      return this.GetItem(path, version, new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions()
      {
        IncludeContentMetadata = includeContentMetadata,
        ScanBytesForEncoding = scanBytesForEncoding
      });
    }

    public Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileContentMetadata GetFileContentMetadata(
      string path,
      bool isFolder,
      int encoding = 0)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileContentMetadata fileContentMetadata = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileContentMetadata();
      fileContentMetadata.FileName = this.GetFileNameFromPath(path);
      if (!isFolder)
      {
        fileContentMetadata.Extension = this.GetFileExtensionFromPath(path);
        fileContentMetadata.ContentType = MimeMapper.GetContentType(fileContentMetadata.Extension);
        if (string.IsNullOrEmpty(fileContentMetadata.ContentType))
          fileContentMetadata.ContentType = "application/octet-stream";
        switch (MimeMapper.GetContentViewerType(fileContentMetadata.Extension, fileContentMetadata.ContentType))
        {
          case ContentViewerType.Text:
            fileContentMetadata.Encoding = Math.Max(encoding, 0);
            break;
          case ContentViewerType.Image:
            fileContentMetadata.IsImage = true;
            fileContentMetadata.IsBinary = true;
            fileContentMetadata.Encoding = -1;
            break;
          default:
            fileContentMetadata.Encoding = encoding;
            fileContentMetadata.IsBinary = encoding < 0;
            break;
        }
      }
      return fileContentMetadata;
    }

    public int TryDetectFileEncoding(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file, int defaultEncoding) => this.TryDetectFileEncoding(file, defaultEncoding, 0L);

    public int TryDetectFileEncoding(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file, int defaultEncoding, long scanBytes)
    {
      Encoding encoding1;
      try
      {
        encoding1 = Encoding.GetEncoding(defaultEncoding);
      }
      catch (Exception ex)
      {
        encoding1 = Encoding.Default;
      }
      using (Stream fileContentStream = this.GetFileContentStream(file))
      {
        Encoding encoding2 = FileTypeUtil.DetermineEncoding(fileContentStream, true, encoding1, scanBytes, out bool _);
        return encoding2 == null ? -1 : encoding2.CodePage;
      }
    }

    public MemoryStream GetFileContentStream(
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file,
      long maxLength,
      bool forceLoad,
      out bool truncated)
    {
      using (Stream fileContentStream = this.GetFileContentStream(file))
        return this.GetFileContentStream(fileContentStream, maxLength, forceLoad, out truncated);
    }

    public MemoryStream GetFileContentStream(
      Stream fileStream,
      long maxLength,
      bool forceLoad,
      out bool truncated)
    {
      truncated = false;
      MemoryStream fileContentStream = new MemoryStream();
      int count1 = 65536;
      byte[] buffer = new byte[count1];
      int val1 = 0;
      int num = 0;
      int count2 = 0;
      while ((long) num < maxLength | forceLoad && (val1 = fileStream.Read(buffer, 0, count1)) > 0)
      {
        count2 = forceLoad ? val1 : (int) Math.Min((long) val1, maxLength - (long) num);
        num += count2;
        if (count2 > 0)
          fileContentStream.Write(buffer, 0, count2);
      }
      if (!forceLoad)
      {
        if (val1 > count2)
          truncated = true;
        else if (val1 > 0)
          truncated = fileStream.Read(buffer, 0, 1) > 0;
      }
      fileContentStream.Seek(0L, SeekOrigin.Begin);
      return fileContentStream;
    }

    public IEnumerable<ZipFileStreamEntry> GetZipFileEntries(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel rootItem)
    {
      if (!rootItem.IsFolder)
        return (IEnumerable<ZipFileStreamEntry>) new ZipFileStreamEntry[1]
        {
          new ZipFileStreamEntry(this.GetFileNameFromPath(rootItem.ServerItem), this.GetFileContentStream(rootItem))
        };
      List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel> itemModelList = new List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>();
      this.FlattenChildItems(rootItem, itemModelList);
      string rootFolderEntry = this.GetFileNameFromPath(rootItem.ServerItem);
      if (string.IsNullOrEmpty(rootFolderEntry))
        rootFolderEntry = FileSpec.RemoveInvalidFileNameChars(this.RepositoryName);
      return itemModelList.Select<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel, ZipFileStreamEntry>((Func<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel, ZipFileStreamEntry>) (item =>
      {
        string str = rootFolderEntry + "/" + item.ServerItem.Substring(rootItem.ServerItem.Length).Trim('/');
        return !item.IsFolder ? new ZipFileStreamEntry(str, this.GetFileContentStream(item)) : new ZipFileStreamEntry(str);
      }));
    }

    private void FlattenChildItems(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel item, List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel> allItems)
    {
      allItems.Add(item);
      if (item.ChildItems == null)
        return;
      foreach (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel childItem in (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>) item.ChildItems)
        this.FlattenChildItems(childItem, allItems);
    }

    public virtual string GetFileNameFromPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return string.Empty;
      int num = path.LastIndexOfAny(new char[2]{ '/', '\\' });
      return num >= 0 ? path.Substring(num + 1) : path;
    }

    public virtual string GetFileExtensionFromPath(string path)
    {
      if (!string.IsNullOrEmpty(path))
      {
        int num = path.LastIndexOf('.');
        if (num >= 0)
          return path.Substring(num + 1);
      }
      return string.Empty;
    }

    public Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff GetFileDiffModel(
      FileDiffParameters diffParameters,
      long? maxFileDiffBytes = null)
    {
      long num = 524288;
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff model = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff();
      model.Blocks = new List<FileDiffBlock>();
      Encoding modifiedEncoding = (Encoding) null;
      if (!string.IsNullOrEmpty(diffParameters.ModifiedPath))
      {
        model.ModifiedFile = this.GetItem(diffParameters.ModifiedPath, diffParameters.ModifiedVersion, new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions()
        {
          IncludeContentMetadata = true,
          IncludeVersionDescription = true,
          ScanBytesForEncoding = num
        });
        if (diffParameters.ModifiedContent != null && model.ModifiedFile != null)
        {
          modifiedEncoding = Encoding.GetEncoding(model.ModifiedFile.ContentMetadata.Encoding);
          if (modifiedEncoding.IsSingleByte && diffParameters.ModifiedContent.Any<char>((Func<char, bool>) (c => c > '\u007F')))
            modifiedEncoding = Encoding.UTF8;
        }
      }
      if (!string.IsNullOrEmpty(diffParameters.OriginalPath))
        model.OriginalFile = this.GetItem(diffParameters.OriginalPath, diffParameters.OriginalVersion, new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions()
        {
          IncludeContentMetadata = true,
          IncludeVersionDescription = true,
          ScanBytesForEncoding = num
        });
      if (modifiedEncoding == null && model.ModifiedFile != null)
        modifiedEncoding = model.ModifiedFile.ContentMetadata.Encoding == -1 ? Encoding.UTF8 : Encoding.GetEncoding(model.ModifiedFile.ContentMetadata.Encoding);
      if (model.OriginalFile == null && model.ModifiedFile == null)
        model.IdenticalContent = true;
      else if (model.OriginalFile == null || model.OriginalFile.IsFolder && model.ModifiedFile != null && !model.ModifiedFile.IsFolder)
      {
        model.BinaryContent = model.ModifiedFile.ContentMetadata.IsBinary;
        model.ImageComparison = model.ModifiedFile.ContentMetadata.IsImage;
        this.PopulateFileDiffModelBlocks(model, diffParameters, (Func<StoredFile>) (() => (StoredFile) null), (Func<StoredFile>) (() => this.GetDiffContentStream(model.ModifiedFile, diffParameters.ModifiedContent, modifiedEncoding)), Encoding.Default.CodePage, modifiedEncoding.CodePage, maxFileDiffBytes);
      }
      else if (model.ModifiedFile == null)
      {
        model.BinaryContent = model.OriginalFile.ContentMetadata.IsBinary;
        model.ImageComparison = model.OriginalFile.ContentMetadata.IsImage;
        this.PopulateFileDiffModelBlocks(model, diffParameters, (Func<StoredFile>) (() => this.GetDiffContentStream(model.OriginalFile, diffParameters.OriginalContent, Encoding.GetEncoding(model.OriginalFile.ContentMetadata.Encoding))), (Func<StoredFile>) (() => (StoredFile) null), model.OriginalFile.ContentMetadata.Encoding, Encoding.Default.CodePage, maxFileDiffBytes);
      }
      else
      {
        model.BinaryContent = model.OriginalFile.ContentMetadata.IsBinary || model.ModifiedFile.ContentMetadata.IsBinary;
        model.ImageComparison = model.OriginalFile.ContentMetadata.IsImage && model.ModifiedFile.ContentMetadata.IsImage;
        model.ModifiedFileEncoding = modifiedEncoding.WebName;
        model.OriginalFileEncoding = model.OriginalFile.ContentMetadata.Encoding == -1 ? Encoding.UTF8.WebName : Encoding.GetEncoding(model.OriginalFile.ContentMetadata.Encoding).WebName;
        this.PopulateFileDiffModelBlocks(model, diffParameters, (Func<StoredFile>) (() => this.GetDiffContentStream(model.OriginalFile, diffParameters.OriginalContent, Encoding.GetEncoding(model.OriginalFile.ContentMetadata.Encoding))), (Func<StoredFile>) (() => this.GetDiffContentStream(model.ModifiedFile, diffParameters.ModifiedContent, modifiedEncoding)), model.OriginalFile.ContentMetadata.Encoding, modifiedEncoding.CodePage, maxFileDiffBytes);
      }
      this.SetSecuredObject((VersionControlSecuredObject) model);
      return model;
    }

    private StoredFile GetDiffContentStream(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel model, string content, Encoding encoding) => content != null ? new StoredFile((Stream) new MemoryStream(encoding.GetBytes(content)), (byte[]) null) : this.GetFileContentStreamWithMetadata(model);

    protected void PopulateFileDiffModelBlocks(
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff model,
      FileDiffParameters diffParameters,
      Func<StoredFile> getOriginalStream,
      Func<StoredFile> getModifiedStream,
      int originalFileEncoding,
      int modifiedFileEncoding,
      long? maxFileDiffBytes = null)
    {
      if (model.BinaryContent)
      {
        this.PopulateFileDiffModelBlocksForBinaryContent(model);
      }
      else
      {
        long maxLength = VersionControlSettings.ReadMaxFileSize(this.RequestContext.Elevate(), maxFileDiffBytes);
        using (StoredFile storedFile1 = getOriginalStream())
        {
          using (StoredFile storedFile2 = getModifiedStream())
          {
            if (storedFile1 == null && storedFile2 == null)
            {
              model.IdenticalContent = true;
              model.Blocks = new List<FileDiffBlock>();
            }
            else if (storedFile1 == null)
            {
              model.IdenticalContent = false;
              bool truncated;
              using (Stream fileContentStream = (Stream) this.GetFileContentStream(storedFile2.Stream, maxLength, diffParameters.ForceLoad, out truncated))
              {
                FileDiffBlock fileDiffBlock = new FileDiffBlock()
                {
                  ChangeType = FileDiffBlockChangeType.Add,
                  ModifiedLineNumberStart = 1
                };
                if (diffParameters.LineNumbersOnly)
                {
                  fileDiffBlock.ModifiedLinesCount = VersionControlFileReader.CountFileLines(fileContentStream, modifiedFileEncoding);
                }
                else
                {
                  fileDiffBlock.ModifiedLines = VersionControlFileReader.ReadFileLines(fileContentStream, modifiedFileEncoding);
                  fileDiffBlock.ModifiedLinesCount = fileDiffBlock.ModifiedLines.Count;
                }
                model.EmptyContent = fileDiffBlock.ModifiedLinesCount == 0;
                model.ModifiedFileTruncated = truncated;
                model.Blocks = new List<FileDiffBlock>()
                {
                  fileDiffBlock
                };
                model.LineCharBlocks = new List<FileCharDiffBlock>()
                {
                  new FileCharDiffBlock() { LineChange = fileDiffBlock }
                };
              }
            }
            else if (storedFile2 == null)
            {
              model.IdenticalContent = false;
              bool truncated;
              using (Stream fileContentStream = (Stream) this.GetFileContentStream(storedFile1.Stream, maxLength, diffParameters.ForceLoad, out truncated))
              {
                FileDiffBlock fileDiffBlock = new FileDiffBlock()
                {
                  ChangeType = FileDiffBlockChangeType.Delete,
                  OriginalLineNumberStart = 1
                };
                if (diffParameters.LineNumbersOnly)
                {
                  fileDiffBlock.OriginalLinesCount = VersionControlFileReader.CountFileLines(fileContentStream, originalFileEncoding);
                }
                else
                {
                  fileDiffBlock.OriginalLines = VersionControlFileReader.ReadFileLines(fileContentStream, originalFileEncoding);
                  fileDiffBlock.OriginalLinesCount = fileDiffBlock.OriginalLines.Count;
                }
                model.EmptyContent = fileDiffBlock.OriginalLinesCount == 0;
                model.OriginalFileTruncated = truncated;
                model.Blocks = new List<FileDiffBlock>()
                {
                  fileDiffBlock
                };
                model.LineCharBlocks = new List<FileCharDiffBlock>()
                {
                  new FileCharDiffBlock() { LineChange = fileDiffBlock }
                };
              }
            }
            else
            {
              bool truncated1;
              using (Stream fileContentStream1 = (Stream) this.GetFileContentStream(storedFile1.Stream, maxLength, diffParameters.ForceLoad, out truncated1))
              {
                bool truncated2;
                using (Stream fileContentStream2 = (Stream) this.GetFileContentStream(storedFile2.Stream, maxLength, diffParameters.ForceLoad, out truncated2))
                {
                  model.OriginalFileTruncated = truncated1;
                  model.ModifiedFileTruncated = truncated2;
                  bool flag = storedFile1.HashValue != null && storedFile2.HashValue != null;
                  if ((!flag ? 0 : (((IEnumerable<byte>) storedFile1.HashValue).SequenceEqual<byte>((IEnumerable<byte>) storedFile2.HashValue) ? 1 : 0)) != 0)
                  {
                    model.IdenticalContent = true;
                    model.Blocks = new List<FileDiffBlock>();
                    model.LineCharBlocks = new List<FileCharDiffBlock>();
                    if (diffParameters.PartialDiff)
                      return;
                    FileDiffBlock fileDiffBlock = new FileDiffBlock()
                    {
                      ChangeType = FileDiffBlockChangeType.None,
                      OriginalLineNumberStart = 1,
                      ModifiedLineNumberStart = 1
                    };
                    if (diffParameters.LineNumbersOnly)
                    {
                      fileDiffBlock.OriginalLinesCount = VersionControlFileReader.CountFileLines(fileContentStream2, modifiedFileEncoding);
                      fileDiffBlock.ModifiedLinesCount = fileDiffBlock.OriginalLinesCount;
                    }
                    else
                    {
                      fileDiffBlock.OriginalLines = VersionControlFileReader.ReadFileLines(fileContentStream2, modifiedFileEncoding);
                      fileDiffBlock.OriginalLinesCount = fileDiffBlock.OriginalLines.Count;
                      fileDiffBlock.ModifiedLines = fileDiffBlock.OriginalLines;
                      fileDiffBlock.ModifiedLinesCount = fileDiffBlock.OriginalLinesCount;
                    }
                    model.Blocks.Add(fileDiffBlock);
                    model.LineCharBlocks.Add(new FileCharDiffBlock()
                    {
                      LineChange = fileDiffBlock
                    });
                  }
                  else
                  {
                    IVssRegistryService service = this.RequestContext.GetService<IVssRegistryService>();
                    int lineDiffTimeoutMs = service.ReadWebSetting<int>(this.RequestContext, "/VersionControl/MaxLineDiffOperationMs", 20000);
                    int wordDiffTimeoutMs = diffParameters.IncludeCharDiffs ? service.ReadWebSetting<int>(this.RequestContext, "/VersionControl/MaxWordDiffOperationMs", 10000) : 0;
                    model.LineCharBlocks = DiffGenerator.ComputeDifference(this.RequestContext.RequestTracer, fileContentStream1, fileContentStream2, originalFileEncoding, modifiedFileEncoding, diffParameters.IgnoreTrimmedWhitespace.GetValueOrDefault(true), diffParameters.LineNumbersOnly, diffParameters.IncludeCharDiffs, lineDiffTimeoutMs, wordDiffTimeoutMs);
                    model.Blocks = model.LineCharBlocks.Select<FileCharDiffBlock, FileDiffBlock>((Func<FileCharDiffBlock, FileDiffBlock>) (diff => diff.LineChange)).ToList<FileDiffBlock>();
                    if (flag)
                      model.WhitespaceChangesOnly = model.Blocks.Count == 0 || model.Blocks.Count == 1 && model.Blocks[0].ChangeType == FileDiffBlockChangeType.None;
                    else
                      model.IdenticalContent = model.Blocks.Count == 0 || model.Blocks.Count == 1 && model.Blocks[0].ChangeType == FileDiffBlockChangeType.None;
                    if (!diffParameters.PartialDiff)
                      return;
                    this.UpdateModelForPartialDiff(model, 3);
                  }
                }
              }
            }
          }
        }
      }
    }

    private void PopulateFileDiffModelBlocksForBinaryContent(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff model)
    {
      model.IdenticalContent = false;
      if (model.OriginalFile == null && model.ModifiedFile == null)
      {
        model.IdenticalContent = true;
      }
      else
      {
        if (model.OriginalFile == null || model.ModifiedFile == null)
          return;
        byte[] hashValueNoContent1 = this.GetFileHashValueNoContent(model.OriginalFile);
        byte[] hashValueNoContent2 = this.GetFileHashValueNoContent(model.ModifiedFile);
        if (hashValueNoContent1 == null || hashValueNoContent2 == null)
          return;
        model.IdenticalContent = ((IEnumerable<byte>) hashValueNoContent1).SequenceEqual<byte>((IEnumerable<byte>) hashValueNoContent2);
      }
    }

    private void UpdateModelForPartialDiff(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff model, int partialDiffLinePadding)
    {
      if (model.IdenticalContent)
      {
        model.Blocks = new List<FileDiffBlock>();
        model.LineCharBlocks = new List<FileCharDiffBlock>();
      }
      else
      {
        List<FileDiffBlock> fileDiffBlockList = new List<FileDiffBlock>();
        List<FileCharDiffBlock> fileCharDiffBlockList = new List<FileCharDiffBlock>();
        for (int index1 = 0; index1 < model.Blocks.Count; ++index1)
        {
          FileDiffBlock block = model.Blocks[index1];
          FileCharDiffBlock lineCharBlock = model.LineCharBlocks[index1];
          if (block.ChangeType == FileDiffBlockChangeType.None)
          {
            if (index1 == 0)
            {
              if (block.ModifiedLines.Count > partialDiffLinePadding)
              {
                block.ModifiedLineNumberStart += block.ModifiedLines.Count - partialDiffLinePadding;
                block.OriginalLineNumberStart += block.ModifiedLines.Count - partialDiffLinePadding;
                block.TruncatedBefore = true;
                List<string> stringList1 = new List<string>();
                List<string> stringList2 = new List<string>();
                for (int index2 = 0; index2 < partialDiffLinePadding; ++index2)
                {
                  stringList1.Add(block.OriginalLines[block.OriginalLines.Count - partialDiffLinePadding + index2]);
                  stringList2.Add(block.ModifiedLines[block.ModifiedLines.Count - partialDiffLinePadding + index2]);
                }
                block.OriginalLines = stringList1;
                block.ModifiedLines = stringList2;
              }
              fileDiffBlockList.Add(block);
              lineCharBlock.LineChange = block;
              fileCharDiffBlockList.Add(lineCharBlock);
            }
            else if (index1 == model.Blocks.Count - 1)
            {
              if (block.ModifiedLines.Count > partialDiffLinePadding)
              {
                block.ModifiedLines = block.ModifiedLines.Take<string>(partialDiffLinePadding).ToList<string>();
                block.OriginalLines = block.OriginalLines.Take<string>(partialDiffLinePadding).ToList<string>();
                block.TruncatedAfter = true;
              }
              fileDiffBlockList.Add(block);
              lineCharBlock.LineChange = block;
              fileCharDiffBlockList.Add(lineCharBlock);
            }
            else if (block.ModifiedLines.Count > 2 * partialDiffLinePadding)
            {
              List<string> modifiedLines = block.ModifiedLines;
              List<string> originalLines = block.OriginalLines;
              block.OriginalLines = originalLines.Take<string>(partialDiffLinePadding).ToList<string>();
              block.ModifiedLines = modifiedLines.Take<string>(partialDiffLinePadding).ToList<string>();
              block.TruncatedAfter = true;
              fileDiffBlockList.Add(block);
              lineCharBlock.LineChange = block;
              fileCharDiffBlockList.Add(lineCharBlock);
              FileDiffBlock fileDiffBlock = new FileDiffBlock()
              {
                ChangeType = FileDiffBlockChangeType.None,
                ModifiedLineNumberStart = block.ModifiedLineNumberStart + modifiedLines.Count - partialDiffLinePadding,
                OriginalLineNumberStart = block.OriginalLineNumberStart + modifiedLines.Count - 3,
                TruncatedBefore = true
              };
              fileDiffBlock.OriginalLines = new List<string>();
              fileDiffBlock.ModifiedLines = new List<string>();
              for (int index3 = 0; index3 < partialDiffLinePadding; ++index3)
              {
                fileDiffBlock.OriginalLines.Add(originalLines[originalLines.Count - partialDiffLinePadding + index3]);
                fileDiffBlock.ModifiedLines.Add(modifiedLines[modifiedLines.Count - partialDiffLinePadding + index3]);
              }
              fileDiffBlock.ModifiedLinesCount = fileDiffBlock.ModifiedLines.Count;
              fileDiffBlock.OriginalLinesCount = fileDiffBlock.ModifiedLinesCount;
              fileDiffBlockList.Add(fileDiffBlock);
              fileCharDiffBlockList.Add(new FileCharDiffBlock()
              {
                LineChange = fileDiffBlock
              });
            }
            else
            {
              fileDiffBlockList.Add(block);
              lineCharBlock.LineChange = block;
              fileCharDiffBlockList.Add(lineCharBlock);
            }
          }
          else
          {
            fileDiffBlockList.Add(block);
            lineCharBlock.LineChange = block;
            fileCharDiffBlockList.Add(lineCharBlock);
          }
        }
        model.Blocks = fileDiffBlockList;
        model.LineCharBlocks = fileCharDiffBlockList;
      }
    }

    protected IEnumerable<int> GetLinkedWorkItemIdsFromArtifactUri(string artifactUri)
    {
      try
      {
        this.RequestContext.TraceEnter(513180, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetLinkedWorkItemIdsFromArtifactUri));
        this.RequestContext.GetService<TeamFoundationLinkingService>();
        return this.RequestContext.GetService<IWorkItemArtifactUriQueryRemotableService>().QueryWorkItemsForArtifactUris(this.RequestContext, new ArtifactUriQuery()
        {
          ArtifactUris = (IEnumerable<string>) new string[1]
          {
            artifactUri
          }
        }).ArtifactUrisQueryResult.Values.SelectMany<IEnumerable<WorkItemReference>, int>((Func<IEnumerable<WorkItemReference>, IEnumerable<int>>) (wie => wie.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (wi => wi.Id))));
      }
      finally
      {
        this.RequestContext.TraceLeave(513185, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "GetLinkedWorkItemIds");
      }
    }
  }
}
