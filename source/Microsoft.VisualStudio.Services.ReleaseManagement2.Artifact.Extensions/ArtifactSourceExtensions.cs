// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.ArtifactSourceExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public static class ArtifactSourceExtensions
  {
    public static bool HasDefinitions(this IDictionary<string, string> sourceInputValues)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      return sourceInputValues.GetDefinitionIdStrings().Count > 0;
    }

    public static IList<string> GetDefinitionIdStrings(
      this IDictionary<string, string> sourceInputValues)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      if (sourceInputValues.IsMultiDefinitionType())
      {
        string enumerable;
        if (sourceInputValues.TryGetValue("definitions", out enumerable) && !enumerable.IsNullOrEmpty<char>())
          return (IList<string>) enumerable.Split(",".ToCharArray());
      }
      else
      {
        string enumerable;
        if (sourceInputValues.TryGetValue("definition", out enumerable) && !enumerable.IsNullOrEmpty<char>())
          return (IList<string>) new List<string>()
          {
            enumerable
          };
      }
      return (IList<string>) new List<string>();
    }

    public static IList<string> GetDefinitionIdStrings(
      this IDictionary<string, InputValue> sourceData)
    {
      if (sourceData == null)
        throw new ArgumentNullException(nameof (sourceData));
      if (sourceData.IsMultiDefinitionType())
      {
        InputValue inputValue;
        if (sourceData.TryGetValue("definitions", out inputValue) && !inputValue.Value.IsNullOrEmpty<char>())
          return (IList<string>) inputValue.Value.Split(",".ToCharArray());
      }
      else
      {
        InputValue inputValue;
        if (sourceData.TryGetValue("definition", out inputValue) && !inputValue.Value.IsNullOrEmpty<char>())
          return (IList<string>) new List<string>()
          {
            inputValue.Value
          };
      }
      return (IList<string>) new List<string>();
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Guid.TryParse(System.String,System.Guid@)", Justification = "opened a bug")]
    public static Guid GetProjectId(this ArtifactSource artifactSource)
    {
      Guid g = artifactSource != null ? artifactSource.TeamProjectId : throw new ArgumentNullException(nameof (artifactSource));
      return !Guid.Empty.Equals(g) ? g : throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.MissingRequiredArtifactSourceDataInArtifact, (object) "project", (object) artifactSource.Id));
    }

    public static bool IsMultiDefinitionType(this IDictionary<string, InputValue> sourceData)
    {
      if (sourceData == null)
        throw new ArgumentNullException(nameof (sourceData));
      bool result = false;
      InputValue inputValue;
      if (sourceData.TryGetValue(nameof (IsMultiDefinitionType), out inputValue) && !inputValue.Value.IsNullOrEmpty<char>())
        bool.TryParse(inputValue.Value, out result);
      return result;
    }

    public static bool IsMultiDefinitionType(this IDictionary<string, string> sourceInputValues)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      bool result = false;
      string str;
      if (sourceInputValues.TryGetValue(nameof (IsMultiDefinitionType), out str))
        bool.TryParse(str, out result);
      return result;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Variable name is meaningful.")]
    public static string CreateArtifactSourceId(
      this ArtifactSource artifactSource,
      string uniqueSourceIdentifier,
      bool addMultipleSourceIdsForMultiBuildArtifact)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      MustacheTemplateParser mustacheTemplateParser = new MustacheTemplateParser();
      Dictionary<string, string> dictionary = artifactSource.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, string>((Func<KeyValuePair<string, InputValue>, string>) (pair => pair.Key), (Func<KeyValuePair<string, InputValue>, string>) (pair => pair.Value.Value));
      if (artifactSource.IsMultiDefinitionType & addMultipleSourceIdsForMultiBuildArtifact)
      {
        List<string> values = new List<string>();
        foreach (string definitionIdString in (IEnumerable<string>) artifactSource.SourceData.GetDefinitionIdStrings())
        {
          dictionary["definition"] = definitionIdString;
          JToken replacementContext = JToken.FromObject((object) dictionary);
          string str = mustacheTemplateParser.ReplaceValues(uniqueSourceIdentifier, (object) replacementContext);
          values.Add(str);
        }
        return string.Join("$", (IEnumerable<string>) values);
      }
      JToken replacementContext1 = JToken.FromObject((object) dictionary);
      return mustacheTemplateParser.ReplaceValues(uniqueSourceIdentifier, (object) replacementContext1);
    }

    public static bool HasMatchingDefinitionId(this ArtifactSource artifactSource, int definitionId)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      return artifactSource.HasMatchingDefinitionId(definitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static bool HasMatchingDefinitionId(
      this ArtifactSource artifactSource,
      string definitionId)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      return artifactSource.SourceData.GetDefinitionIdStrings().Contains<string>(definitionId, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "By design")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design")]
    [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "By design")]
    public static bool IsInternalArtifact(
      this PipelineArtifactSource artifactSource,
      IVssRequestContext requestContext,
      Guid projectId,
      out object artifactContext)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      artifactContext = (object) null;
      string artifactTypeId = artifactSource.ArtifactTypeId;
      bool flag = string.Equals(artifactTypeId, "Git") || string.Equals(artifactTypeId, "PackageManagement") || string.Equals(artifactTypeId, "TFVC");
      InputValue inputValue;
      int result1;
      if (string.Equals(artifactTypeId, "Build") && artifactSource.SourceData != null && artifactSource.SourceData.TryGetValue("definition", out inputValue) && !string.IsNullOrEmpty(inputValue?.Value) && int.TryParse(inputValue.Value, out result1))
      {
        BuildDefinition result2 = requestContext.GetClient<BuildHttpClient>().GetDefinitionAsync(projectId, result1, new int?(), new DateTime?(), (IEnumerable<string>) null, new bool?(), (object) null, new CancellationToken()).GetResult<BuildDefinition>(requestContext.CancellationToken);
        artifactContext = (object) result2;
        string type = result2?.Repository?.Type;
        if (string.Equals(type, "TfsGit", StringComparison.OrdinalIgnoreCase) || string.Equals(type, "TfsVersionControl", StringComparison.OrdinalIgnoreCase))
          flag = true;
      }
      return flag;
    }
  }
}
