// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ArtifactPropertyKinds
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class ArtifactPropertyKinds
  {
    private static readonly string[] propertyNameFilter = new string[1]
    {
      "*"
    };

    internal static PropertiesCollection Convert(this IEnumerable<PropertyValue> value) => new PropertiesCollection((IDictionary<string, object>) value.ToDictionary<PropertyValue, string, object>((Func<PropertyValue, string>) (x => x.PropertyName), (Func<PropertyValue, object>) (x => x.Value)));

    internal static void MatchPropertiesOld<T>(
      TeamFoundationDataReader reader,
      IList<T> outputs,
      Func<T, string> getArtifactId,
      Action<T, PropertiesCollection> setProperties)
    {
      foreach (ArtifactPropertyValue current in reader.CurrentEnumerable<ArtifactPropertyValue>())
      {
        string moniker = current.Spec.Moniker;
        foreach (T output in (IEnumerable<T>) outputs)
        {
          string str = getArtifactId(output);
          if (moniker.Equals(str))
          {
            setProperties(output, current.PropertyValues.Convert());
            break;
          }
        }
      }
    }

    internal static void MatchProperties<T>(
      TeamFoundationDataReader reader,
      IList<T> outputs,
      Func<T, string> getArtifactId,
      Action<T, PropertiesCollection> setProperties)
    {
      Dictionary<string, T> dictionary = new Dictionary<string, T>(outputs.Count);
      foreach (T output in (IEnumerable<T>) outputs)
      {
        string key = getArtifactId(output);
        dictionary[key] = output;
      }
      foreach (ArtifactPropertyValue current in reader.CurrentEnumerable<ArtifactPropertyValue>())
      {
        T obj;
        if (dictionary.TryGetValue(current.Spec.Moniker, out obj))
          setProperties(obj, current.PropertyValues.Convert());
      }
    }

    internal static List<ArtifactSpec> MakeArtifactSpecs<T>(
      Guid artifactKind,
      IList<T> outputList,
      Func<T, string> getMoniker)
    {
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>(outputList.Count);
      artifactSpecList.AddRange(outputList.Where<T>((Func<T, bool>) (output => (object) output != null)).Select<T, ArtifactSpec>((Func<T, ArtifactSpec>) (output => ArtifactPropertyKinds.MakeArtifactSpec(artifactKind, getMoniker(output)))));
      return artifactSpecList;
    }

    internal static ArtifactSpec MakeArtifactSpec(Guid artifactKind, string moniker) => new ArtifactSpec(artifactKind, moniker, 0);

    public static void SaveReviewProperties(
      IVssRequestContext requestContext,
      PropertiesCollection reviewProperties,
      Guid projectId,
      int reviewId)
    {
      ArtifactSpec spec = ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.ReviewPropertyKind, ArtifactPropertyKinds.GetReviewMoniker(projectId, reviewId));
      ArtifactPropertyKinds.SaveProperties(requestContext, spec, reviewProperties);
    }

    public static void SaveIterationProperties(
      IVssRequestContext requestContext,
      PropertiesCollection iterationProperties,
      Guid projectId,
      int reviewId,
      int iterationId)
    {
      string iterationMoniker = ArtifactPropertyKinds.GetIterationMoniker(projectId, reviewId, new int?(iterationId));
      ArtifactPropertyKinds.SaveProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.IterationPropertyKind, iterationMoniker), iterationProperties);
    }

    internal static void SaveProperties(
      IVssRequestContext requestContext,
      ArtifactSpec spec,
      PropertiesCollection properties)
    {
      if (properties == null || properties.Count == 0)
        return;
      List<PropertyValue> propertyValueList = new List<PropertyValue>(properties.Count);
      propertyValueList.AddRange(properties.Select<KeyValuePair<string, object>, PropertyValue>((Func<KeyValuePair<string, object>, PropertyValue>) (prop => new PropertyValue(prop.Key, prop.Value))));
      requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, spec, (IEnumerable<PropertyValue>) propertyValueList);
    }

    internal static PropertiesCollection PatchProperties(
      IVssRequestContext requestContext,
      ArtifactSpec spec,
      PropertiesCollection propertiesToUpdate)
    {
      ArtifactPropertyKinds.SaveProperties(requestContext, spec, propertiesToUpdate);
      return ArtifactPropertyKinds.GetProperties(requestContext, spec);
    }

    internal static PropertiesCollection GetProperties(
      IVssRequestContext requestContext,
      ArtifactSpec spec)
    {
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, spec, (IEnumerable<string>) ArtifactPropertyKinds.propertyNameFilter))
      {
        ArtifactPropertyValue artifactPropertyValue = properties.Current<StreamingCollection<ArtifactPropertyValue>>().FirstOrDefault<ArtifactPropertyValue>();
        return artifactPropertyValue != null ? artifactPropertyValue.PropertyValues.Convert() : (PropertiesCollection) null;
      }
    }

    public static void FetchIterationExtendedProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      Iteration iteration,
      string customPropertyNameFilter)
    {
      ArtifactPropertyKinds.FetchIterationExtendedProperties(requestContext, projectId, (IList<Iteration>) new Iteration[1]
      {
        iteration
      }, new string[1]{ customPropertyNameFilter });
    }

    public static void FetchIterationExtendedProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Iteration> iterations,
      string customPropertyNameFilter)
    {
      ArtifactPropertyKinds.FetchIterationExtendedProperties(requestContext, projectId, iterations, new string[1]
      {
        customPropertyNameFilter
      });
    }

    public static void FetchIterationExtendedProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Iteration> iterations,
      string[] customPropertyNameFilter = null)
    {
      if (iterations == null || iterations.Count <= 0)
        return;
      if (ArtifactPropertyKinds.IsMonikerPerfFixEnabled(requestContext))
      {
        Dictionary<Iteration, string> iterationMonikerLookup = new Dictionary<Iteration, string>(iterations.Count, (IEqualityComparer<Iteration>) IterationDictionaryComparer.Instance);
        Func<Iteration, string> getIterationMonikerWithCache = (Func<Iteration, string>) (iteration =>
        {
          string iterationMoniker;
          if (!iterationMonikerLookup.TryGetValue(iteration, out iterationMoniker))
          {
            iterationMoniker = ArtifactPropertyKinds.GetIterationMoniker(projectId, iteration.ReviewId, iteration.Id);
            iterationMonikerLookup[iteration] = iterationMoniker;
          }
          return iterationMoniker;
        });
        List<ArtifactSpec> artifactSpecList = ArtifactPropertyKinds.MakeArtifactSpecs<Iteration>(ServerConstants.IterationPropertyKind, iterations, (Func<Iteration, string>) (x => getIterationMonikerWithCache(x)));
        using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) (customPropertyNameFilter ?? ArtifactPropertyKinds.propertyNameFilter)))
          ArtifactPropertyKinds.MatchProperties<Iteration>(properties, iterations, getIterationMonikerWithCache, (Action<Iteration, PropertiesCollection>) ((x, y) => x.Properties = y));
      }
      else
      {
        List<ArtifactSpec> artifactSpecList = ArtifactPropertyKinds.MakeArtifactSpecs<Iteration>(ServerConstants.IterationPropertyKind, iterations, (Func<Iteration, string>) (x => ArtifactPropertyKinds.GetIterationMoniker(projectId, x.ReviewId, x.Id)));
        using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) (customPropertyNameFilter ?? ArtifactPropertyKinds.propertyNameFilter)))
          ArtifactPropertyKinds.MatchPropertiesOld<Iteration>(properties, iterations, (Func<Iteration, string>) (x => ArtifactPropertyKinds.GetIterationMoniker(projectId, x.ReviewId, x.Id)), (Action<Iteration, PropertiesCollection>) ((x, y) => x.Properties = y));
      }
    }

    internal static void FetchReviewExtendedProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Review> reviews)
    {
      if (ArtifactPropertyKinds.IsMonikerPerfFixEnabled(requestContext))
      {
        Dictionary<int, string> reviewMonikerLookup = new Dictionary<int, string>(reviews.Count);
        Func<Review, string> getReviewMonikerWithCache = (Func<Review, string>) (review =>
        {
          string reviewMoniker;
          if (!reviewMonikerLookup.TryGetValue(review.Id, out reviewMoniker))
          {
            reviewMoniker = ArtifactPropertyKinds.GetReviewMoniker(projectId, review.Id);
            reviewMonikerLookup[review.Id] = reviewMoniker;
          }
          return reviewMoniker;
        });
        List<ArtifactSpec> artifactSpecList = ArtifactPropertyKinds.MakeArtifactSpecs<Review>(ServerConstants.ReviewPropertyKind, reviews, (Func<Review, string>) (x => getReviewMonikerWithCache(x)));
        using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) ArtifactPropertyKinds.propertyNameFilter))
          ArtifactPropertyKinds.MatchProperties<Review>(properties, reviews, getReviewMonikerWithCache, (Action<Review, PropertiesCollection>) ((x, y) => x.Properties = y));
      }
      else
      {
        List<ArtifactSpec> artifactSpecList = ArtifactPropertyKinds.MakeArtifactSpecs<Review>(ServerConstants.ReviewPropertyKind, reviews, (Func<Review, string>) (x => ArtifactPropertyKinds.GetReviewMoniker(projectId, x.Id)));
        using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) ArtifactPropertyKinds.propertyNameFilter))
          ArtifactPropertyKinds.MatchPropertiesOld<Review>(properties, reviews, (Func<Review, string>) (x => ArtifactPropertyKinds.GetReviewMoniker(projectId, x.Id)), (Action<Review, PropertiesCollection>) ((x, y) => x.Properties = y));
      }
    }

    internal static void FetchAttachmentExtendedProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Attachment> attachments)
    {
      if (attachments == null || !attachments.Any<Attachment>())
        return;
      if (ArtifactPropertyKinds.IsMonikerPerfFixEnabled(requestContext))
      {
        Dictionary<Attachment, string> attachmentMonikerLookup = new Dictionary<Attachment, string>(attachments.Count, (IEqualityComparer<Attachment>) AttachmentDictionaryComparer.Instance);
        Func<Attachment, string> getAttachmentMonikerWithCache = (Func<Attachment, string>) (attachment =>
        {
          string attachmentMoniker;
          if (!attachmentMonikerLookup.TryGetValue(attachment, out attachmentMoniker))
          {
            attachmentMoniker = ArtifactPropertyKinds.GetAttachmentMoniker(projectId, attachment);
            attachmentMonikerLookup[attachment] = attachmentMoniker;
          }
          return attachmentMoniker;
        });
        List<ArtifactSpec> artifactSpecList = ArtifactPropertyKinds.MakeArtifactSpecs<Attachment>(ServerConstants.AttachmentPropertyKind, (IList<Attachment>) attachments.ToList<Attachment>(), (Func<Attachment, string>) (x => getAttachmentMonikerWithCache(x)));
        using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) ArtifactPropertyKinds.propertyNameFilter))
          ArtifactPropertyKinds.MatchProperties<Attachment>(properties, (IList<Attachment>) attachments.ToList<Attachment>(), getAttachmentMonikerWithCache, (Action<Attachment, PropertiesCollection>) ((x, y) => x.Properties = y));
      }
      else
      {
        List<ArtifactSpec> artifactSpecList = ArtifactPropertyKinds.MakeArtifactSpecs<Attachment>(ServerConstants.AttachmentPropertyKind, (IList<Attachment>) attachments.ToList<Attachment>(), (Func<Attachment, string>) (x => ArtifactPropertyKinds.GetAttachmentMoniker(projectId, x)));
        using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) ArtifactPropertyKinds.propertyNameFilter))
          ArtifactPropertyKinds.MatchPropertiesOld<Attachment>(properties, (IList<Attachment>) attachments.ToList<Attachment>(), (Func<Attachment, string>) (x => ArtifactPropertyKinds.GetAttachmentMoniker(projectId, x)), (Action<Attachment, PropertiesCollection>) ((x, y) => x.Properties = y));
      }
    }

    internal static string GetReviewSettingMoniker(Guid projectId) => projectId.ToString();

    internal static string GetIterationMoniker(Guid projectId, int reviewId, int? iterationId) => string.Format("{0}-{1}-{2}", (object) projectId, (object) reviewId, (object) iterationId);

    internal static string GetReviewMoniker(Guid projectId, int reviewId) => string.Format("{0}-{1}", (object) projectId, (object) reviewId);

    internal static string GetAttachmentMoniker(Guid projectId, Attachment attachment) => string.Format("{0}-{1}-{2}", (object) projectId, (object) attachment.ReviewId, (object) attachment.Id);

    internal static string PreparePatchPropertiesInfo(PropertiesCollection properties)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (properties == null)
      {
        stringBuilder.Append("null");
      }
      else
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
          stringBuilder.Append(string.Format("'{0}':'{1}',", (object) property.Key, property.Value));
      }
      return stringBuilder.ToString();
    }

    internal static bool IsMonikerPerfFixEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("CodeReview.Moniker.PerfFixDisabled");
  }
}
