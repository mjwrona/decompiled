// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ProjectRepoDocumentsUpdateDataStorage
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ProjectRepoDocumentsUpdateDataStorage
  {
    public int TotalProjectDocsUpdated { get; set; }

    public int TotalRepoDocsUpdated { get; set; }

    public IDictionary<string, int> ProjectDocFieldsChangeCount { get; set; }

    public IDictionary<string, int> RepoDocFieldsChangeCount { get; set; }

    public HashSet<string> ProjectsWithChange { get; set; }

    public HashSet<string> RepositoriesWithChange { get; set; }

    public long TimeTakenToCalculateTheDiffInMilliSeconds { get; set; }

    public ProjectRepoDocumentsUpdateDataStorage()
    {
      this.TotalProjectDocsUpdated = 0;
      this.TotalRepoDocsUpdated = 0;
      this.ProjectDocFieldsChangeCount = (IDictionary<string, int>) new Dictionary<string, int>();
      this.RepoDocFieldsChangeCount = (IDictionary<string, int>) new Dictionary<string, int>();
      this.ProjectsWithChange = new HashSet<string>();
      this.RepositoriesWithChange = new HashSet<string>();
    }

    public void CompareAndUpdateTheChangeCount(
      ProjectContract newConract,
      ProjectContract oldConract)
    {
      this.IncrementChangedFieldCountForProject<string>(newConract.DocumentId, newConract.Name, oldConract?.Name, "name");
      this.IncrementChangedFieldCountForProject<string>(newConract.DocumentId, newConract.Description, oldConract?.Description, "description");
      this.IncrementChangedFieldCountForProject<string>(newConract.DocumentId, newConract.Visibility, oldConract?.Visibility, "visibility");
      this.IncrementChangedFieldCountForProject<string>(newConract.DocumentId, newConract.CollectionName, oldConract?.CollectionName, "collectionName");
      this.IncrementChangedFieldCountForProject<string>(newConract.DocumentId, newConract.Url, oldConract?.Url, "url");
      this.IncrementChangedFieldCountForProject<string>(newConract.DocumentId, newConract.ImageUrl, oldConract?.ImageUrl, "imageUrl");
      this.IncrementChangedFieldCount<int?>(newConract.DocumentId, newConract.LikesCount, (int?) oldConract?.LikesCount, "likesCount", this.ProjectDocFieldsChangeCount, this.ProjectsWithChange);
      this.IncrementChangedFieldCountForProject<int?>(newConract.DocumentId, newConract.ActivityCount1day, (int?) oldConract?.ActivityCount1day, "activityCount1day");
      this.IncrementChangedFieldCountForProject<int?>(newConract.DocumentId, newConract.ActivityCount7days, (int?) oldConract?.ActivityCount7days, "activityCount7day");
      this.IncrementChangedFieldCountForProject<int?>(newConract.DocumentId, newConract.ActivityCount30days, (int?) oldConract?.ActivityCount30days, "activityCount30days");
      this.IncrementChangedFieldCountForProject<int?>(newConract.DocumentId, newConract.TrendFactor1Day, (int?) oldConract?.TrendFactor1Day, "TrendFactor1Day");
      this.IncrementChangedFieldCountForProject<int?>(newConract.DocumentId, newConract.TrendFactor7Days, (int?) oldConract?.TrendFactor7Days, "TrendFactor7Days");
      this.IncrementChangedFieldCountForProject<int?>(newConract.DocumentId, newConract.TrendFactor30Days, (int?) oldConract?.TrendFactor30Days, "TrendFactor30Days");
      this.IncrementChangedFieldCount(newConract.DocumentId, newConract.Tags, oldConract?.Tags, "tags", this.ProjectDocFieldsChangeCount, this.ProjectsWithChange);
      this.IncrementChangedFieldCount(newConract.DocumentId, newConract.ProjectLanguages, oldConract?.ProjectLanguages, "languages", this.ProjectDocFieldsChangeCount, this.ProjectsWithChange);
      bool flag1 = oldConract == null || oldConract.AggregatedActivity == null || oldConract.AggregatedActivity.Length == 0;
      bool flag2 = newConract.AggregatedActivity == null || newConract.AggregatedActivity.Length == 0;
      if (flag1 & flag2 || !(flag1 ^ flag2) && newConract.AggregatedActivity.Length.Equals(oldConract.AggregatedActivity.Length))
        return;
      this.UpdateCount("activityStats", this.ProjectDocFieldsChangeCount);
      this.ProjectsWithChange.Add(newConract.DocumentId);
    }

    public void CompareAndUpdateTheChangeCount(
      RepositoryContract newConract,
      RepositoryContract oldConract)
    {
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.Name, oldConract?.Name, "name");
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.Readme, oldConract?.Readme, "readme");
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.ReadmeFilePath, oldConract?.ReadmeFilePath, "readmeFilePath");
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.ReadmeMetadata, oldConract?.ReadmeMetadata, "readmeMetadata");
      this.IncrementChangedFieldCount(newConract.DocumentId, newConract.ReadmeLinks, oldConract?.ReadmeLinks, "readmeLinks", this.RepoDocFieldsChangeCount, this.RepositoriesWithChange);
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.Url, oldConract?.Url, "url");
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.CollectionName, oldConract?.CollectionName, "collectionName");
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.Visibility, oldConract?.Visibility, "visibility");
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.ProjectName, oldConract?.ProjectName, "projectName");
      this.IncrementChangedFieldCountForRepository<int?>(newConract.DocumentId, newConract.ActivityCount1day, (int?) oldConract?.ActivityCount1day, "activityCount1day");
      this.IncrementChangedFieldCountForRepository<int?>(newConract.DocumentId, newConract.ActivityCount7days, (int?) oldConract?.ActivityCount7days, "activityCount7day");
      this.IncrementChangedFieldCountForRepository<int?>(newConract.DocumentId, newConract.ActivityCount30days, (int?) oldConract?.ActivityCount30days, "activityCount30days");
      this.IncrementChangedFieldCountForRepository<string>(newConract.DocumentId, newConract.VersionControl, oldConract?.VersionControl, "versionControl");
      this.IncrementChangedFieldCountForRepository<int?>(newConract.DocumentId, newConract.ForksCount, (int?) oldConract?.ForksCount, "forks");
      this.IncrementChangedFieldCount(newConract.DocumentId, newConract.Languages, oldConract?.Languages, "languages", this.RepoDocFieldsChangeCount, this.RepositoriesWithChange);
    }

    public void PublishCiData()
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("NumberOfProjectDocsUpdated", (double) this.TotalProjectDocsUpdated);
      intelligenceData.Add("NumberOfRepositoryDocsUpdated", (double) this.TotalRepoDocsUpdated);
      intelligenceData.Add("TotalNumberOfDocsUpdated", (double) (this.TotalProjectDocsUpdated + this.TotalRepoDocsUpdated));
      intelligenceData.Add("NumberOfProjectDocsWithChange", (double) this.ProjectsWithChange.Count);
      intelligenceData.Add("NumberOfRepositoryDocsWithChange", (double) this.RepositoriesWithChange.Count);
      intelligenceData.Add("TotalNumberOfDocsWithChange", (double) (this.ProjectsWithChange.Count + this.RepositoriesWithChange.Count));
      intelligenceData.Add("TimeTakenToCalculateTheDiffInMilliSec", (double) this.TimeTakenToCalculateTheDiffInMilliSeconds);
      this.FlattenDictionaryToCiData(this.ProjectDocFieldsChangeCount, "ProjectContract", intelligenceData);
      this.FlattenDictionaryToCiData(this.RepoDocFieldsChangeCount, "RepositoryContract", intelligenceData);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Indexing Pipeline", "Feed", intelligenceData);
    }

    private void FlattenDictionaryToCiData(
      IDictionary<string, int> docFieldsChangeCount,
      string contractType,
      CustomerIntelligenceData ciResponseData)
    {
      string str = contractType + "_";
      foreach (string key in (IEnumerable<string>) docFieldsChangeCount.Keys)
        ciResponseData.Add(str + key, (double) docFieldsChangeCount[key]);
    }

    private void UpdateCount(string fieldName, IDictionary<string, int> docFieldsChangeCount)
    {
      if (!docFieldsChangeCount.ContainsKey(fieldName))
        docFieldsChangeCount[fieldName] = 1;
      else
        docFieldsChangeCount[fieldName]++;
    }

    private void IncrementChangedFieldCount<T>(
      string documentId,
      T newField,
      T oldField,
      string fieldName,
      IDictionary<string, int> docFieldsChangeCount,
      HashSet<string> changedDocs)
    {
      if ((object) newField == null && (object) oldField == null || ((object) newField != null || (object) oldField == null) && ((object) newField == null || (object) oldField != null) && newField.Equals((object) oldField))
        return;
      this.UpdateCount(fieldName, docFieldsChangeCount);
      changedDocs.Add(documentId);
    }

    private void IncrementChangedFieldCountForProject<T>(
      string documentId,
      T newField,
      T oldField,
      string fieldName)
    {
      this.IncrementChangedFieldCount<T>(documentId, newField, oldField, fieldName, this.ProjectDocFieldsChangeCount, this.ProjectsWithChange);
    }

    private void IncrementChangedFieldCountForRepository<T>(
      string documentId,
      T newField,
      T oldField,
      string fieldName)
    {
      this.IncrementChangedFieldCount<T>(documentId, newField, oldField, fieldName, this.RepoDocFieldsChangeCount, this.RepositoriesWithChange);
    }

    private void IncrementChangedFieldCount(
      string documentId,
      string[] newField,
      string[] oldField,
      string fieldName,
      IDictionary<string, int> docFieldsChangeCount,
      HashSet<string> changedDocs)
    {
      bool flag1 = oldField == null || oldField.Length == 0;
      bool flag2 = newField == null || newField.Length == 0;
      if (flag1 & flag2 || !(flag1 ^ flag2) && newField.Length == oldField.Length && ((IEnumerable<string>) newField).Intersect<string>((IEnumerable<string>) oldField).Count<string>() == newField.Length)
        return;
      this.UpdateCount(fieldName, docFieldsChangeCount);
      changedDocs.Add(documentId);
    }
  }
}
