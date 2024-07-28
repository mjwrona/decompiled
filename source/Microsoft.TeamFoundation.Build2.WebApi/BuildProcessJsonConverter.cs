// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildProcessJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal sealed class BuildProcessJsonConverter : TypePropertyJsonConverter<BuildProcess>
  {
    protected override BuildProcess GetInstance(Type objectType)
    {
      if (objectType == typeof (DesignerProcess))
        return (BuildProcess) new DesignerProcess();
      if (objectType == typeof (YamlProcess))
        return (BuildProcess) new YamlProcess();
      if (objectType == typeof (DockerProcess))
        return (BuildProcess) new DockerProcess();
      return objectType == typeof (JustInTimeProcess) ? (BuildProcess) new JustInTimeProcess() : base.GetInstance(objectType);
    }

    protected override BuildProcess GetInstance(int targetType)
    {
      switch (targetType)
      {
        case 2:
          return (BuildProcess) new YamlProcess();
        case 3:
          return (BuildProcess) new DockerProcess();
        case 4:
          return (BuildProcess) new JustInTimeProcess();
        default:
          return (BuildProcess) new DesignerProcess();
      }
    }

    protected override bool TryInferType(JObject value, out int type)
    {
      if (value.TryGetValue("yamlFilename", StringComparison.OrdinalIgnoreCase, out JToken _))
      {
        type = 2;
        return true;
      }
      type = 1;
      return true;
    }
  }
}
