// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ServerModelUtility
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class ServerModelUtility
  {
    private static readonly Lazy<JsonSerializerSettings> SerializerSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new VssJsonMediaTypeFormatter().SerializerSettings));
    private static readonly Dictionary<string, string> SourceDataKeyMap = new Dictionary<string, string>()
    {
      {
        "projectId",
        "project"
      },
      {
        "servicesId",
        "connection"
      },
      {
        "sourceId",
        "definition"
      }
    };

    public static string ToString(object value)
    {
      if (value == null)
        return (string) null;
      StringBuilder sb = new StringBuilder();
      using (StringWriter stringWriter = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
      {
        JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) stringWriter);
        JsonSerializer.Create(ServerModelUtility.SerializerSettings.Value).Serialize((JsonWriter) jsonTextWriter, value);
      }
      return sb.ToString();
    }

    public static T FromString<T>(string value)
    {
      if (string.IsNullOrEmpty(value))
        return default (T);
      using (StringReader reader1 = new StringReader(value))
      {
        JsonTextReader reader2 = new JsonTextReader((TextReader) reader1);
        return JsonSerializer.Create(ServerModelUtility.SerializerSettings.Value).Deserialize<T>((JsonReader) reader2);
      }
    }

    public static void FillSourceData(
      string inputSourceData,
      Dictionary<string, InputValue> targetSourceData)
    {
      if (targetSourceData == null)
        throw new ArgumentNullException(nameof (targetSourceData));
      Dictionary<string, InputValue> dictionary = ServerModelUtility.FromString<Dictionary<string, InputValue>>(inputSourceData);
      if (dictionary == null)
        return;
      foreach (KeyValuePair<string, InputValue> keyValuePair in dictionary)
        targetSourceData[ServerModelUtility.NormalizeSourceDataKey(keyValuePair.Key)] = keyValuePair.Value;
    }

    private static string NormalizeSourceDataKey(string dataKey) => !ServerModelUtility.SourceDataKeyMap.ContainsKey(dataKey) ? dataKey : ServerModelUtility.SourceDataKeyMap[dataKey];

    public static IList<string> ToStringList(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (IList<string>) null;
      string[] strArray = value.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      return strArray.Length != 0 ? (IList<string>) strArray : (IList<string>) null;
    }

    public static string GetRepositoryIdFromArtifactSource(
      Dictionary<string, InputValue> artifactSourceData)
    {
      if (artifactSourceData == null || !artifactSourceData.ContainsKey("version"))
        return string.Empty;
      IDictionary<string, object> data = artifactSourceData["version"].Data;
      return data == null || !data.ContainsKey("repositoryId") ? string.Empty : data["repositoryId"].ToString();
    }

    public static string GetRepositoryTypeFromArtifactSource(
      Dictionary<string, InputValue> artifactSourceData)
    {
      if (artifactSourceData == null || !artifactSourceData.ContainsKey("version"))
        return string.Empty;
      IDictionary<string, object> data = artifactSourceData["version"].Data;
      return data == null || !data.ContainsKey("repositoryType") ? string.Empty : data["repositoryType"].ToString();
    }
  }
}
