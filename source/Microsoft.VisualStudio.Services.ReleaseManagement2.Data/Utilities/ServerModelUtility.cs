// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ServerModelUtility
  {
    private const string OldBuildArtifactTypeId = "buildArtifact";
    private static readonly Lazy<JsonSerializerSettings> SerializerSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new VssJsonMediaTypeFormatter().SerializerSettings));

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

    public static string GetNormalizedArtifactTypeIdAfterDbRead(string artifactTypeId)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "buildArtifact",
          "Build"
        },
        {
          "jenkinsArtifact",
          "Jenkins"
        },
        {
          "tfsOnPremArtifact",
          "Team Build (external)"
        }
      };
      return !dictionary.ContainsKey(artifactTypeId) ? artifactTypeId : dictionary[artifactTypeId];
    }

    public static string[] GetNormalizedArtifactTypeIdBeforeDbRead(string artifactTypeId) => !ServerModelUtility.IsBuildArtifact(artifactTypeId) ? new string[1]
    {
      artifactTypeId
    } : new string[2]{ "buildArtifact", "Build" };

    public static IEnumerable<int> ToIntList(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (IEnumerable<int>) new List<int>();
      List<int> intList = new List<int>();
      string str = value;
      char[] separator = new char[1]{ ',' };
      foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        int result;
        if (!int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidIntegerValue, (object) s, (object) value));
        intList.Add(result);
      }
      return (IEnumerable<int>) intList;
    }

    public static bool IsBuildArtifact(string artifactTypeId) => string.Equals(artifactTypeId, "buildArtifact", StringComparison.OrdinalIgnoreCase) || string.Equals(artifactTypeId, "Build", StringComparison.OrdinalIgnoreCase);

    public static void FillSourceData(
      string inputSourceData,
      Dictionary<string, InputValue> targetSourceData)
    {
      if (targetSourceData == null)
        throw new ArgumentNullException(nameof (targetSourceData));
      ArtifactSourceDataUtility.DecompressSourceData(inputSourceData, (IDictionary<string, InputValue>) targetSourceData);
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "By design")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions GetWebApiEnvironmentOptions(
      string runOptions)
    {
      if (runOptions.IsNullOrEmpty<char>())
        return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions();
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) "EmailNotificationType");
      return runOptions.ToUpperInvariant().Contains(str.ToUpperInvariant()) ? ServerModelUtility.FromString<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentOptions>(runOptions) : ServerModelUtility.FromString<IDictionary<string, string>>(runOptions).ToEnvironmentOptions();
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "By design")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions GetServerEnvironmentOptions(
      string runOptions)
    {
      if (runOptions.IsNullOrEmpty<char>())
        return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions();
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) "EmailNotificationType");
      if (runOptions.ToUpperInvariant().Contains(str.ToUpperInvariant()))
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions environmentOptions = ServerModelUtility.FromString<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions>(runOptions);
        if (environmentOptions.EmailNotificationType == "Never" && environmentOptions.EmailRecipients != null)
          environmentOptions.EmailRecipients = (string) null;
        return environmentOptions;
      }
      IDictionary<string, string> dictionary = ServerModelUtility.FromString<IDictionary<string, string>>(runOptions);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions environmentOptions1 = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentOptions();
      if (dictionary.ContainsKey("EnvironmentOwnerEmailNotificationType"))
        environmentOptions1.EmailNotificationType = dictionary["EnvironmentOwnerEmailNotificationType"];
      return environmentOptions1;
    }

    public static string GetSourceIdInLegacyFormat(string sourceId)
    {
      string idInLegacyFormat = sourceId;
      if (sourceId != null && sourceId.Contains(":"))
        idInLegacyFormat = ((IEnumerable<string>) sourceId.Split(':')).LastOrDefault<string>();
      return idInLegacyFormat;
    }

    public static string GetDefaultPhaseName(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes phaseType)
    {
      switch (phaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment:
          return Resources.RunOnAgentPhaseName;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer:
          return Resources.RunOnServerPhaseName;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment:
          return Resources.RunOnMachineGroupPhaseName;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates:
          return Resources.DeploymentGatesPhaseName;
        default:
          return Resources.RunOnAgentPhaseName;
      }
    }

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
