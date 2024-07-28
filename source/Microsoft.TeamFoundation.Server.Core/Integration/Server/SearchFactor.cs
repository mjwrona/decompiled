// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.SearchFactor
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03")]
  [Serializable]
  public enum SearchFactor
  {
    None,
    Sid,
    AccountName,
    DistinguishedName,
    AdministrativeApplicationGroup,
    ServiceApplicationGroup,
    EveryoneApplicationGroup,
  }
}
