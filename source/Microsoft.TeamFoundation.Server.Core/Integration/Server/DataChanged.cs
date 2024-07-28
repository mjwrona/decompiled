// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.DataChanged
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [Serializable]
  public class DataChanged
  {
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string DataType;
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public DateTime LastModified;

    public DataChanged(string dataType, DateTime dateTime)
    {
      this.DataType = dataType;
      this.LastModified = dateTime;
    }

    public DataChanged()
    {
    }
  }
}
