// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.CodeContractField
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public class CodeContractField : EntitySearchField
  {
    private const string DocumentIdField = "documentId";
    private const string IndexingVersionField = "indexingVersion";
    public const string FileNameField = "fileName";
    private const string FilePathField = "filePath";
    private const string FilePathRawField = "filePathRaw";
    public const string SortableFilePathField = "filePath.filePathRaw";
    private const string FileExtensionField = "fileExtension";
    public const string ContentField = "content";
    public const string OriginalContentField = "originalContent";
    public const string OriginalContentReverseField = "originalContent.reverse";
    private const string CommitIdField = "commitId";
    public const string ChangeIdField = "changeId";
    public const string RawSuffix = "raw";
    public const string TrigramSuffix = "trigram";
    public const string NGramSuffix = "ngram";
    public const string ContentIdField = "contentId";
    private const string AccountNameField = "accountName";
    private const string ProjectVisibilityField = "projectVisibility";
    public const string ProjectIdField = "projectId";
    private const string ProjectInfoField = "projectInfo";
    public const string RepoNameField = "repoName";
    public const string RepoIdField = "repoId";
    private const string IsDefaultBranchField = "isDefaultBranch";
    private const string FileExtensionIdField = "fileExtensionId";
    public const string VersionControlTypeField = "vcType";
    public const string DefaultTypeName = "content";
    public const string IndexedTimeStamp = "indexedTimeStamp";
    private const string PathsField = "paths";
    public const string PathsBranchNameField = "paths.branchName";
    public const string PathsChangeIdField = "paths.changeId";
    public const string FilePathOriginalField = "filePathOriginal";
    private const string AccountNameOriginalField = "accountNameOriginal";
    public const string RepoNameOriginalField = "repoNameOriginal";
    public const string RepositoryIdField = "repositoryId";
    private const string LastChangeUtcTimeField = "lastCommitUtcTime";
    private const string OrganizationNameRawField = "organizationName.raw";
    private const string CollectionNameRawField = "collectionName.raw";
    private const string ProjectNameRawField = "projectName.raw";
    private const string RepoNameRawField = "repoName.raw";
    public const string IndexedTimeStampRawField = "indexedTimeStamp.raw";
    public const string OriginalContentTrigramField = "originalContent.trigram";
    public const string OriginalContentNGramField = "originalContent.ngram";

    public CodeContractField.CodeSearchFieldDesc StoredToFieldDesc { get; }

    public CodeContractField.CodeSearchFieldDesc StoredForFieldDesc { get; }

    public CodeContractField(
      string elasticsearchFieldName,
      bool isStoredField = false,
      CodeContractField.CodeSearchFieldDesc storedForFieldDesc = CodeContractField.CodeSearchFieldDesc.None,
      CodeContractField.CodeSearchFieldDesc storedToFieldDesc = CodeContractField.CodeSearchFieldDesc.None,
      bool supportsFacetFilter = false,
      string facetFilterName = null)
      : base(elasticsearchFieldName, isStoredField, supportsFacetFilter, facetFilterName)
    {
      this.StoredForFieldDesc = storedForFieldDesc;
      this.StoredToFieldDesc = storedToFieldDesc;
    }

    internal CodeContractField(CodeContractField.CodeSearchFieldDesc field)
      : base(field.ElasticsearchFieldName(), field.IsStoredField(), field.FacetFilterName() != null, field.FacetFilterName())
    {
      this.StoredToFieldDesc = field.StoredToField();
      this.StoredForFieldDesc = field.StoredForField();
    }

    public enum CodeSearchFieldDesc
    {
      None,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("documentId"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.DocumentId)] DocumentId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("indexingVersion"), CodeSearchFieldExtensions.IsStoredField] IndexingVersion,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("indexedTimeStamp")] IndexedTimeStamp,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("indexedTimeStamp.raw")] IndexedTimeStampRaw,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("organizationId")] OrganizationId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("organizationNameOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.OrganizationName)] OrganizationNameOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("organizationName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.OrganizationNameOriginal)] OrganizationName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("organizationName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.OrganizationNameRaw)] OrganizationNameInNoPayloadMappings,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("organizationName.raw"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.OrganizationNameInNoPayloadMappings)] OrganizationNameRaw,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("accountNameOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.AccountName)] AccountNameOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("accountName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.AccountNameOriginal), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.Account)] AccountName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("collectionNameOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.CollectionName)] CollectionNameOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("collectionName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.CollectionNameOriginal), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.CollectionName)] CollectionName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("collectionName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.CollectionNameRaw), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.CollectionName)] CollectionNameInNoPayloadMappings,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("collectionName.raw"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.CollectionNameInNoPayloadMappings)] CollectionNameRaw,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("collectionId"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.CollectionId)] CollectionId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("projectNameOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.ProjectName)] ProjectNameOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("projectName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.ProjectNameOriginal), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.ProjectName)] ProjectName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("projectName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.ProjectNameRaw), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.ProjectName)] ProjectNameInNoPayloadMappings,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("projectName.raw"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.CollectionNameInNoPayloadMappings)] ProjectNameRaw,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("projectId"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.ProjectId)] ProjectId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("projectInfo")] ProjectInfo,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("projectVisibility")] ProjectVisibility,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("repoNameOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.RepoName)] RepoNameOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("repoName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.RepoNameOriginal), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.RepoName)] RepoName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("repoName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.RepoNameRaw), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.RepoName)] RepoNameInNoPayloadMappings,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("repoName.raw"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.RepoNameInNoPayloadMappings)] RepoNameRaw,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("repositoryId"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.RepositoryId)] RepositoryId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("paths")] Paths,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("paths.changeId"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.ChangeId)] PathsChangeId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("isDefaultBranch"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.IsDefaultBranch)] IsDefaultBranch,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("branchNameOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.BranchName)] BranchNameOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("branchName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.BranchNameOriginal), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.BranchName)] BranchName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("paths.branchName"), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.PathsBranchNameOriginal)] PathsBranchName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("paths.branchNameOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.PathsBranchName)] PathsBranchNameOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("fileName"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.FileName)] FileName,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("filePath"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.FilePath), CodeSearchFieldExtensions.StoredToField(CodeContractField.CodeSearchFieldDesc.FilePathOriginal)] FilePath,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("filePathOriginal"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.StoredForField(CodeContractField.CodeSearchFieldDesc.FilePath)] FilePathOriginal,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("filePathRaw")] FilePathRaw,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("filePath.filePathRaw"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.SortableFilePath)] SortableFilePath,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("fileExtension"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.FileExtension)] FileExtension,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("fileExtensionId")] FileExtensionId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("content"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.Content)] Content,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("originalContent")] OriginalContent,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("originalContent.reverse")] OriginalContentReverse,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("originalContent.trigram")] OriginalContentTrigram,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("originalContent.ngram")] OriginalContentNGram,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("commitId"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.CommitId)] CommitId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("changeId"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.ChangeId)] ChangeId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("contentId"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.ContentId)] ContentId,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("vcType"), CodeSearchFieldExtensions.IsStoredField, CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.VersionControlType)] VersionControlType,
      [CodeSearchFieldExtensions.ElasticsearchFieldName("lastCommitUtcTime"), CodeSearchFieldExtensions.CodeContractQueryableElement(CodeFileContract.CodeContractQueryableElement.LastChangeUtcTime)] LastChangeUtcTime,
    }
  }
}
