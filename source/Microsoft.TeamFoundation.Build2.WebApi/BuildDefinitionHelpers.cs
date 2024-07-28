// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionHelpers
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  public static class BuildDefinitionHelpers
  {
    public static BuildDefinition Deserialize(string definitionString)
    {
      BuildDefinition buildDefinition = JsonUtility.FromString<BuildDefinition>(definitionString);
      if (buildDefinition?.Process == null)
        buildDefinition = JsonConvert.DeserializeObject<BuildDefinition3_2>(definitionString).ToBuildDefinition();
      return buildDefinition;
    }

    public static BuildDefinitionTemplate GetTemplateFromStream(Stream stream)
    {
      string end;
      using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
        end = streamReader.ReadToEnd();
      BuildDefinitionTemplate templateFromStream = JsonConvert.DeserializeObject<BuildDefinitionTemplate>(end);
      if (templateFromStream?.Template?.Process == null)
        templateFromStream = JsonConvert.DeserializeObject<BuildDefinitionTemplate3_2>(end).ToBuildDefinitionTemplate();
      return templateFromStream;
    }
  }
}
