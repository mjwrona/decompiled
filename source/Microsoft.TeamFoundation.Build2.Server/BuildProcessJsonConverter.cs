// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildProcessJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildProcessJsonConverter : TypePropertyJsonConverter<BuildProcess>
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

    protected override BuildProcess GetInstance(int targetType, JObject value)
    {
      switch (targetType)
      {
        case 2:
          JToken jtoken;
          return value.TryGetValue("version", StringComparison.OrdinalIgnoreCase, out jtoken) && jtoken.Type == JTokenType.Integer && (int) jtoken < 3 ? (BuildProcess) new YamlCompatProcess() : (BuildProcess) new YamlProcess();
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
