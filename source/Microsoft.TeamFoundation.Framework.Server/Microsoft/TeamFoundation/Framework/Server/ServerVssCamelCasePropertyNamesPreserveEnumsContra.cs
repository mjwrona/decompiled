// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServerVssCamelCasePropertyNamesPreserveEnumsContractResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Serialization;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServerVssCamelCasePropertyNamesPreserveEnumsContractResolver : 
    VssCamelCasePropertyNamesPreserveEnumsContractResolver
  {
    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
      JsonObjectContract objectContract = base.CreateObjectContract(objectType);
      ServerJsonSerializationHelper.AddCallback((JsonContract) objectContract, objectType);
      return objectContract;
    }

    protected override JsonISerializableContract CreateISerializableContract(Type objectType)
    {
      JsonISerializableContract iserializableContract = base.CreateISerializableContract(objectType);
      ServerJsonSerializationHelper.AddCallback((JsonContract) iserializableContract, objectType);
      return iserializableContract;
    }

    protected override JsonDynamicContract CreateDynamicContract(Type objectType)
    {
      JsonDynamicContract dynamicContract = base.CreateDynamicContract(objectType);
      ServerJsonSerializationHelper.AddCallback((JsonContract) dynamicContract, objectType);
      return dynamicContract;
    }
  }
}
