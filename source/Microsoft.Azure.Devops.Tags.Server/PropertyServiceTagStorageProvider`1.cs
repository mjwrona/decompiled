// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.PropertyServiceTagStorageProvider`1
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Tags.Server
{
  internal class PropertyServiceTagStorageProvider<T> : ITagStorageProvider<T>
  {
    public static readonly string[] PropertyServiceSqlTagFilter = new string[1]
    {
      "Microsoft.TeamFoundation.Tagging.TagDefinition.%"
    };
    public const string TagDefinitionPropertyNamePrefix = "Microsoft.TeamFoundation.Tagging.TagDefinition.";
    private Guid m_artifactKind;
    private static readonly string[] s_tagFilter = new string[1]
    {
      "Microsoft.TeamFoundation.Tagging.TagDefinition.*"
    };
    private const int s_dummyPropertyValue = 0;

    public PropertyServiceTagStorageProvider(Guid artifactKind) => this.m_artifactKind = artifactKind;

    public TagIdsHistoryEntry<T>[] GetTagsHistoryForArtifact(
      IVssRequestContext requestContext,
      TagArtifact<T> artifact)
    {
      ArtifactSpec unversionedArtifactSpec = PropertyServiceTagStorageProvider<T>.CreateUnversionedArtifactSpec(this.m_artifactKind, artifact);
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) new ArtifactSpec[1]
      {
        unversionedArtifactSpec
      }, (IEnumerable<string>) PropertyServiceTagStorageProvider<T>.s_tagFilter, PropertiesOptions.AllVersions))
      {
        List<ArtifactPropertyValue> list = properties.Cast<ArtifactPropertyValue>().ToList<ArtifactPropertyValue>();
        return list.Count <= 0 ? new TagIdsHistoryEntry<T>[0] : PropertyServiceTagStorageProvider<T>.ParseArtifactTagsHistory((IEnumerable<ArtifactPropertyValue>) list);
      }
    }

    public void DeleteTagHistoryForArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<TagArtifact<T>> artifacts)
    {
      IEnumerable<ArtifactSpec> artifactSpecs = artifacts.Select<TagArtifact<T>, ArtifactSpec>((Func<TagArtifact<T>, ArtifactSpec>) (a => PropertyServiceTagStorageProvider<T>.CreateUnversionedArtifactSpec(this.m_artifactKind, a)));
      requestContext.GetService<ITeamFoundationPropertyService>().DeleteArtifacts(requestContext, artifactSpecs, PropertiesOptions.AllVersions);
    }

    public IEnumerable<ArtifactTagIds<T>> GetTagsForArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<TagArtifact<T>> artifacts)
    {
      IEnumerable<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty> propertiesForArtifacts = this.GetTagPropertiesForArtifacts(requestContext, artifacts);
      List<ArtifactTagIds<T>> tagsForArtifacts = new List<ArtifactTagIds<T>>();
      foreach (IGrouping<T, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty> source in propertiesForArtifacts.GroupBy<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, T>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, T>) (tp => tp.Artifact.Id)))
      {
        Guid[] array = source.GroupBy<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>) (x => x.TagId)).Select<IGrouping<Guid, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>((Func<IGrouping<Guid, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>) (x => x.OrderByDescending<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, int>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, int>) (y => y.Artifact.Version)).FirstOrDefault<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>())).Where<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, bool>) (x => x != null && x.AssociationAction == 0)).Select<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>) (x => x.TagId)).ToArray<Guid>();
        VersionedTagArtifact<T> artifactId = new VersionedTagArtifact<T>(source.First<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>().Artifact.DataspaceIdentifier, source.Key, 0);
        tagsForArtifacts.Add(new ArtifactTagIds<T>(artifactId, (IEnumerable<Guid>) array));
      }
      return (IEnumerable<ArtifactTagIds<T>>) tagsForArtifacts;
    }

    public IEnumerable<ArtifactTagIds<T>> GetTagsForVersionedArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<VersionedTagArtifact<T>> versionedArtifacts)
    {
      VersionedTagArtifact<T>[] array = versionedArtifacts.GroupBy<VersionedTagArtifact<T>, Tuple<Guid, T>>((Func<VersionedTagArtifact<T>, Tuple<Guid, T>>) (a => new Tuple<Guid, T>(a.DataspaceIdentifier, a.Id))).Select<IGrouping<Tuple<Guid, T>, VersionedTagArtifact<T>>, VersionedTagArtifact<T>>((Func<IGrouping<Tuple<Guid, T>, VersionedTagArtifact<T>>, VersionedTagArtifact<T>>) (g => g.First<VersionedTagArtifact<T>>())).ToArray<VersionedTagArtifact<T>>();
      ILookup<Tuple<Guid, T>, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty> lookup = this.GetTagPropertiesForArtifacts(requestContext, (IEnumerable<TagArtifact<T>>) array).ToLookup<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Tuple<Guid, T>>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Tuple<Guid, T>>) (a => new Tuple<Guid, T>(a.Artifact.DataspaceIdentifier, a.Artifact.Id)));
      List<ArtifactTagIds<T>> versionedArtifacts1 = new List<ArtifactTagIds<T>>();
      foreach (VersionedTagArtifact<T> versionedArtifact in versionedArtifacts)
      {
        VersionedTagArtifact<T> requestedArtifact = versionedArtifact;
        IEnumerable<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty> source1 = lookup[new Tuple<Guid, T>(requestedArtifact.DataspaceIdentifier, requestedArtifact.Id)];
        if (source1.Any<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>())
        {
          IEnumerable<IGrouping<Guid, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>> source2 = source1.GroupBy<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>) (x => x.TagId));
          versionedArtifacts1.Add(new ArtifactTagIds<T>(requestedArtifact, (IEnumerable<Guid>) source2.Select<IGrouping<Guid, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>((Func<IGrouping<Guid, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>) (x => x.OrderByDescending<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, int>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, int>) (y => y.Artifact.Version)).FirstOrDefault<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, bool>) (z => requestedArtifact.Version >= z.Artifact.Version)))).Where<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, bool>) (x => x != null && x.AssociationAction == 0)).Select<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>((Func<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty, Guid>) (x => x.TagId)).ToArray<Guid>()));
        }
      }
      return (IEnumerable<ArtifactTagIds<T>>) versionedArtifacts1;
    }

    public void UpdateTagsForArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactTagUpdate<T>> tagUpdates,
      DateTime? changedDate,
      Guid? changedBy)
    {
      ArtifactPropertyValue[] array = tagUpdates.Select<ArtifactTagUpdate<T>, ArtifactPropertyValue>((Func<ArtifactTagUpdate<T>, ArtifactPropertyValue>) (tu =>
      {
        List<PropertyValue> propertyValueList = new List<PropertyValue>(4);
        if (tu.AddedTagIds != null)
          propertyValueList.AddRange(tu.AddedTagIds.Select<Guid, PropertyValue>((Func<Guid, PropertyValue>) (tagId => new PropertyValue(PropertyServiceTagStorageProvider<T>.TagIdToPropertyName(tagId), (object) 0))));
        if (tu.RemovedTagIds != null)
          propertyValueList.AddRange(tu.RemovedTagIds.Select<Guid, PropertyValue>((Func<Guid, PropertyValue>) (tagId => new PropertyValue(PropertyServiceTagStorageProvider<T>.TagIdToPropertyName(tagId), (object) 1))));
        return new ArtifactPropertyValue(PropertyServiceTagStorageProvider<T>.CreateArtifactSpec(this.m_artifactKind, tu.Artifact), (IEnumerable<PropertyValue>) propertyValueList);
      })).ToArray<ArtifactPropertyValue>();
      if (array.Length == 0)
        return;
      requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) array, changedDate, changedBy);
    }

    public IEnumerable<VersionedTagArtifact<T>> QueryArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<string> strings = tagIds.Select<Guid, string>(PropertyServiceTagStorageProvider<T>.\u003C\u003EO.\u003C0\u003E__TagIdToPropertyName ?? (PropertyServiceTagStorageProvider<T>.\u003C\u003EO.\u003C0\u003E__TagIdToPropertyName = new Func<Guid, string>(PropertyServiceTagStorageProvider<T>.TagIdToPropertyName)));
      IVssRequestContext requestContext1 = requestContext;
      Guid artifactKind = this.m_artifactKind;
      IEnumerable<string> propertyNameFilters = strings;
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactKind, propertyNameFilters))
      {
        List<ArtifactPropertyValue> list = properties.Cast<ArtifactPropertyValue>().ToList<ArtifactPropertyValue>();
        return !list.Any<ArtifactPropertyValue>() ? Enumerable.Empty<VersionedTagArtifact<T>>() : PropertyServiceTagStorageProvider<T>.ParseArtifactTags((IEnumerable<ArtifactPropertyValue>) list, (IEnumerable<VersionedTagArtifact<T>>) null).Where<ArtifactTagIds<T>>((Func<ArtifactTagIds<T>, bool>) (a => !tagIds.Except<Guid>(a.Tags).Any<Guid>())).Select<ArtifactTagIds<T>, VersionedTagArtifact<T>>((Func<ArtifactTagIds<T>, VersionedTagArtifact<T>>) (a => a.Artifact));
      }
    }

    public IEnumerable<Guid> DeleteHistoryForTags(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tagIds, nameof (tagIds));
      requestContext.GetService<ITeamFoundationPropertyService>().DeleteProperties(requestContext, this.m_artifactKind, tagIds.Select<Guid, string>((Func<Guid, string>) (tagId => PropertyServiceTagStorageProvider<T>.TagIdToPropertyName(tagId))));
      return Enumerable.Empty<Guid>();
    }

    public static IEnumerable<Guid> GetKnownArtifactKinds(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationPropertyService>().GetArtifactKinds(requestContext).Select<ArtifactKind, Guid>((Func<ArtifactKind, Guid>) (kind => kind.Kind));

    private IEnumerable<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty> GetTagPropertiesForArtifacts(
      IVssRequestContext requestContext,
      IEnumerable<TagArtifact<T>> artifacts)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      IEnumerable<ArtifactSpec> unversionedArtifactSpecs = PropertyServiceTagStorageProvider<T>.CreateUnversionedArtifactSpecs(this.m_artifactKind, artifacts);
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<ArtifactSpec> artifactSpecs = unversionedArtifactSpecs;
      string[] tagFilter = PropertyServiceTagStorageProvider<T>.s_tagFilter;
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext1, artifactSpecs, (IEnumerable<string>) tagFilter, PropertiesOptions.AllVersions))
        return (IEnumerable<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>) properties.Cast<ArtifactPropertyValue>().ToList<ArtifactPropertyValue>().SelectMany<ArtifactPropertyValue, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>((Func<ArtifactPropertyValue, IEnumerable<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>>) (apv =>
        {
          VersionedTagArtifact<T> artifact = PropertyServiceTagStorageProvider<T>.CreateArtifactId(apv.Spec);
          return apv.PropertyValues.Select<PropertyValue, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>((Func<PropertyValue, PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>) (pv => new PropertyServiceTagStorageProvider<T>.ArtifactTagProperty()
          {
            Artifact = artifact,
            TagId = PropertyServiceTagStorageProvider<T>.PropertyNameToTagId(pv.PropertyName),
            AssociationAction = (int) pv.Value,
            ChangedDate = pv.ChangedDate,
            ChangedBy = pv.ChangedBy
          }));
        })).ToArray<PropertyServiceTagStorageProvider<T>.ArtifactTagProperty>();
    }

    public static TagIdsHistoryEntry<T>[] ParseArtifactTagsHistory(
      IEnumerable<ArtifactPropertyValue> artifactPropertyList)
    {
      List<TagIdsHistoryEntry<T>> tagIdsHistoryEntryList = new List<TagIdsHistoryEntry<T>>();
      HashSet<Guid> guidSet = new HashSet<Guid>();
      foreach (ArtifactPropertyValue versionData in (IEnumerable<ArtifactPropertyValue>) artifactPropertyList.OrderBy<ArtifactPropertyValue, int>((Func<ArtifactPropertyValue, int>) (a => a.Spec.Version)))
      {
        DateTime changedDate;
        Guid changedBy;
        PropertyServiceTagStorageProvider<T>.ProcessTagsForVersion(guidSet, versionData, out changedDate, out changedBy);
        TagIdsHistoryEntry<T> tagIdsHistoryEntry = new TagIdsHistoryEntry<T>(PropertyServiceTagStorageProvider<T>.CreateArtifactId(versionData.Spec), (IEnumerable<Guid>) guidSet.ToArray<Guid>(), changedDate, changedBy);
        tagIdsHistoryEntryList.Add(tagIdsHistoryEntry);
      }
      return tagIdsHistoryEntryList.ToArray();
    }

    private static IEnumerable<ArtifactTagIds<T>> ParseArtifactTags(
      IEnumerable<ArtifactPropertyValue> rawValues,
      IEnumerable<VersionedTagArtifact<T>> requestedVersions)
    {
      IEnumerable<IGrouping<T, ArtifactPropertyValue>> groupings = rawValues.GroupBy<ArtifactPropertyValue, T>((Func<ArtifactPropertyValue, T>) (artifact => PropertyServiceTagStorageProvider<T>.CreateArtifactId(artifact.Spec).Id));
      Dictionary<T, int> dictionary = (Dictionary<T, int>) null;
      if (requestedVersions != null)
        dictionary = requestedVersions.ToDictionary<VersionedTagArtifact<T>, T, int>((Func<VersionedTagArtifact<T>, T>) (r => r.Id), (Func<VersionedTagArtifact<T>, int>) (r => r.Version));
      List<ArtifactTagIds<T>> artifactTags = new List<ArtifactTagIds<T>>();
      foreach (IGrouping<T, ArtifactPropertyValue> source1 in groupings)
      {
        HashSet<Guid> guidSet = new HashSet<Guid>();
        IOrderedEnumerable<ArtifactPropertyValue> source2 = source1.OrderBy<ArtifactPropertyValue, int>((Func<ArtifactPropertyValue, int>) (a => a.Spec.Version));
        int num = dictionary == null ? int.MaxValue : dictionary[source1.Key];
        foreach (ArtifactPropertyValue versionData in (IEnumerable<ArtifactPropertyValue>) source2)
        {
          if (versionData.Spec.Version <= num)
            PropertyServiceTagStorageProvider<T>.ProcessTagsForVersion(guidSet, versionData, out DateTime _, out Guid _);
          else
            break;
        }
        if (guidSet.Any<Guid>())
        {
          ArtifactTagIds<T> artifactTagIds = new ArtifactTagIds<T>(PropertyServiceTagStorageProvider<T>.CreateArtifactId(source2.First<ArtifactPropertyValue>().Spec), (IEnumerable<Guid>) guidSet);
          artifactTags.Add(artifactTagIds);
        }
      }
      return (IEnumerable<ArtifactTagIds<T>>) artifactTags;
    }

    private static void ProcessTagsForVersion(
      HashSet<Guid> currentTags,
      ArtifactPropertyValue versionData,
      out DateTime changedDate,
      out Guid changedBy)
    {
      changedDate = DateTime.MinValue;
      changedBy = Guid.Empty;
      foreach (PropertyValue propertyValue in versionData.PropertyValues)
      {
        Guid? changedBy1 = propertyValue.ChangedBy;
        if (changedBy1.HasValue)
        {
          ref Guid local = ref changedBy;
          changedBy1 = propertyValue.ChangedBy;
          Guid guid = changedBy1.Value;
          local = guid;
        }
        if (propertyValue.ChangedDate.HasValue)
          changedDate = propertyValue.ChangedDate.Value;
        if ((int) propertyValue.Value == 0)
          currentTags.Add(PropertyServiceTagStorageProvider<T>.PropertyNameToTagId(propertyValue.PropertyName));
        else
          currentTags.Remove(PropertyServiceTagStorageProvider<T>.PropertyNameToTagId(propertyValue.PropertyName));
      }
    }

    internal static string TagIdToPropertyName(Guid tagId) => "Microsoft.TeamFoundation.Tagging.TagDefinition." + tagId.ToString();

    private static Guid PropertyNameToTagId(string propertyName) => propertyName.StartsWith("Microsoft.TeamFoundation.Tagging.TagDefinition.", StringComparison.OrdinalIgnoreCase) ? new Guid(propertyName.Substring("Microsoft.TeamFoundation.Tagging.TagDefinition.".Length)) : throw new TagDefinitionNotFoundException();

    internal static ArtifactSpec CreateArtifactSpec(
      Guid artifactKind,
      VersionedTagArtifact<T> artifact)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      if (typeof (T) == typeof (int))
        return new ArtifactSpec(artifactKind, (int) Convert.ChangeType((object) artifact.Id, typeof (int)), artifact.Version, artifact.DataspaceIdentifier);
      if (typeof (T) == typeof (Guid))
        return new ArtifactSpec(artifactKind, ((Guid) Convert.ChangeType((object) artifact.Id, typeof (Guid))).ToByteArray(), artifact.Version, artifact.DataspaceIdentifier);
      throw new InvalidTagArtifactTypeException(typeof (T));
    }

    internal static ArtifactSpec CreateUnversionedArtifactSpec(
      Guid artifactKind,
      TagArtifact<T> artifact)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      if (typeof (T) == typeof (int))
        return new ArtifactSpec(artifactKind, (int) Convert.ChangeType((object) artifact.Id, typeof (int)), 0, artifact.DataspaceIdentifier);
      if (typeof (T) == typeof (Guid))
        return new ArtifactSpec(artifactKind, ((Guid) Convert.ChangeType((object) artifact.Id, typeof (Guid))).ToByteArray(), 0, artifact.DataspaceIdentifier);
      throw new InvalidTagArtifactTypeException(typeof (T));
    }

    private static IEnumerable<ArtifactSpec> CreateArtifactSpecs(
      Guid artifactKind,
      IEnumerable<VersionedTagArtifact<T>> artifactIds)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactIds, nameof (artifactIds));
      return artifactIds.Select<VersionedTagArtifact<T>, ArtifactSpec>((Func<VersionedTagArtifact<T>, ArtifactSpec>) (artifact => PropertyServiceTagStorageProvider<T>.CreateArtifactSpec(artifactKind, artifact)));
    }

    private static IEnumerable<ArtifactSpec> CreateUnversionedArtifactSpecs(
      Guid artifactKind,
      IEnumerable<TagArtifact<T>> artifactIds)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactIds, nameof (artifactIds));
      return artifactIds.Select<TagArtifact<T>, ArtifactSpec>((Func<TagArtifact<T>, ArtifactSpec>) (artifact => PropertyServiceTagStorageProvider<T>.CreateUnversionedArtifactSpec(artifactKind, artifact)));
    }

    private static VersionedTagArtifact<T> CreateArtifactId(ArtifactSpec artifactSpec)
    {
      T artifactId;
      if (typeof (T) == typeof (int))
      {
        artifactId = (T) Convert.ChangeType((object) ((int) artifactSpec.Id[3] + ((int) artifactSpec.Id[2] << 8) + ((int) artifactSpec.Id[1] << 16) + ((int) artifactSpec.Id[0] << 24)), typeof (T));
      }
      else
      {
        if (!(typeof (T) == typeof (Guid)))
          throw new InvalidTagArtifactTypeException(typeof (T));
        artifactId = (T) Convert.ChangeType((object) new Guid(artifactSpec.Id), typeof (Guid));
      }
      return new VersionedTagArtifact<T>(artifactSpec.DataspaceIdentifier, artifactId, artifactSpec.Version);
    }

    private class ArtifactTagProperty
    {
      public VersionedTagArtifact<T> Artifact { get; set; }

      public Guid TagId { get; set; }

      public int AssociationAction { get; set; }

      public DateTime? ChangedDate { get; set; }

      public Guid? ChangedBy { get; set; }
    }
  }
}
