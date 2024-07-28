// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class PullRequestTrigger : ReleaseTriggerBase
  {
    public PullRequestTrigger()
    {
      this.TriggerType = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseTriggerType.PullRequest;
      this.TriggerConditions = (IList<PullRequestFilter>) new List<PullRequestFilter>();
      this.PullRequestConfiguration = new PullRequestConfiguration();
    }

    public string ArtifactAlias { get; set; }

    public IList<PullRequestFilter> TriggerConditions { get; set; }

    public PullRequestConfiguration PullRequestConfiguration { get; set; }

    public string StatusPolicyName { get; set; }

    public IDictionary<string, InputValue> GetPullRequestDictionaryObject(
      ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      IDictionary<string, InputValue> dictionaryObject = (IDictionary<string, InputValue>) new Dictionary<string, InputValue>();
      if (!string.IsNullOrEmpty(this.StatusPolicyName))
      {
        InputValue inputValue = new InputValue()
        {
          Value = this.StatusPolicyName,
          DisplayValue = this.StatusPolicyName
        };
        dictionaryObject.Add(WellKnownPullRequestVariables.PullRequestStatusPolicyName, inputValue);
      }
      ArtifactSource artifactSource = releaseDefinition.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == this.ArtifactAlias)).SingleOrDefault<ArtifactSource>();
      if (this.PullRequestConfiguration != null)
      {
        string str;
        if (!this.PullRequestConfiguration.UseArtifactReference)
        {
          str = this.PullRequestConfiguration.CodeRepositoryReference.SystemType.ToString();
          foreach (KeyValuePair<string, ReleaseManagementInputValue> keyValuePair in (IEnumerable<KeyValuePair<string, ReleaseManagementInputValue>>) this.PullRequestConfiguration.CodeRepositoryReference.RepositoryReference)
          {
            InputValue inputValue = new InputValue()
            {
              Value = keyValuePair.Value.Value,
              DisplayValue = keyValuePair.Value.DisplayValue
            };
            dictionaryObject.Add(keyValuePair.Key, inputValue);
          }
        }
        else
        {
          str = "None";
          InputValue inputValue1 = new InputValue()
          {
            Value = artifactSource.SourceData["definition"].Value,
            DisplayValue = artifactSource.SourceData["definition"].DisplayValue
          };
          if (artifactSource.IsGitArtifact)
          {
            str = "TfsGit";
            InputValue inputValue2 = new InputValue()
            {
              Value = artifactSource.SourceData["project"].Value,
              DisplayValue = artifactSource.SourceData["project"].DisplayValue
            };
            dictionaryObject.Add("pullRequestProjectId", inputValue2);
            dictionaryObject.Add("pullRequestRepositoryId", inputValue1);
          }
          else if (artifactSource.IsGitHubArtifact)
          {
            str = "GitHub";
            InputValue inputValue3 = new InputValue()
            {
              Value = artifactSource.SourceData["connection"].Value,
              DisplayValue = artifactSource.SourceData["connection"].DisplayValue
            };
            dictionaryObject.Add("pullRequestSystemConnectionId", inputValue3);
            dictionaryObject.Add("pullRequestRepositoryName", inputValue1);
          }
        }
        InputValue inputValue4 = new InputValue()
        {
          Value = str,
          DisplayValue = str
        };
        dictionaryObject.Add(WellKnownPullRequestVariables.PullRequestSystemType, inputValue4);
      }
      return dictionaryObject;
    }
  }
}
