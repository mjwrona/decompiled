// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitPolicyScopeSerializer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  internal abstract class GitPolicyScopeSerializer : VssSecureJsonConverter
  {
    private JsonReaderException MakeException(JsonReader reader, string message) => !(reader is JsonTextReader jsonTextReader) ? new JsonReaderException(Resources.Format("PolicyJsonReaderExceptionMessage", (object) message, (object) reader.Path)) : new JsonReaderException(Resources.Format("PolicyJsonReaderExceptionWithLineMessage", (object) message, (object) reader.Path, (object) jsonTextReader.LineNumber, (object) jsonTextReader.LinePosition));

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartArray)
        throw this.MakeException(reader, Resources.Format("PolicyScopeParsingError", (object) "["));
      List<GitPolicyRepositoryScopeItem> scopeItems = new List<GitPolicyRepositoryScopeItem>();
      while (reader.Read())
      {
        if (reader.TokenType != JsonToken.EndArray)
        {
          JObject jobject = JObject.Load(reader);
          GitPolicyRepositoryScopeItem repositoryScopeItem;
          if (jobject["matchKind"] != null)
          {
            if (!this.CanTakeRefNameScopes)
              throw this.MakeException(reader, Resources.Format("PolicyScopeRepositoriesOnly"));
            GitPolicyRefScopeItem policyRefScopeItem = jobject.ToObject<GitPolicyRefScopeItem>(serializer);
            if (policyRefScopeItem.IsDefaultBranchScope() && !policyRefScopeItem.IsRefNameNull())
              throw this.MakeException(reader, Resources.Format("PolicyScopeDefaultBranchKindRefMustBeEmpty"));
            repositoryScopeItem = policyRefScopeItem.IsDefaultBranchScope() || !policyRefScopeItem.IsRefNameNull() ? (GitPolicyRepositoryScopeItem) policyRefScopeItem : throw this.MakeException(reader, Resources.Format("PolicyScopeRefMustNotBeEmpty"));
          }
          else
            repositoryScopeItem = jobject.ToObject<GitPolicyRepositoryScopeItem>(serializer);
          scopeItems.Add(repositoryScopeItem);
        }
        else
          return scopeItems.Count != 0 ? this.CreateParsedObject((IReadOnlyList<GitPolicyRepositoryScopeItem>) scopeItems) : throw this.MakeException(reader, Resources.Format("PolicyScopeCannotBeEmpty"));
      }
      throw this.MakeException(reader, Resources.Format("PolicyScopeParsingError", (object) "]"));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      IReadOnlyList<GitPolicyRepositoryScopeItem> scopeItems = this.GetScopeItems(value);
      writer.WriteStartArray();
      foreach (GitPolicyRepositoryScopeItem repositoryScopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) scopeItems)
        serializer.Serialize(writer, (object) repositoryScopeItem);
      writer.WriteEndArray();
    }

    protected abstract object CreateParsedObject(
      IReadOnlyList<GitPolicyRepositoryScopeItem> scopeItems);

    protected abstract IReadOnlyList<GitPolicyRepositoryScopeItem> GetScopeItems(object value);

    protected abstract bool CanTakeRefNameScopes { get; }
  }
}
