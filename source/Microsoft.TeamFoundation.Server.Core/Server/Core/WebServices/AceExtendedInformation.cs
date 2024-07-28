// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.AceExtendedInformation
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  public class AceExtendedInformation
  {
    [XmlAttribute]
    public int InheritedAllow { get; set; }

    [XmlAttribute]
    public int InheritedDeny { get; set; }

    [XmlAttribute]
    public int EffectiveAllow { get; set; }

    [XmlAttribute]
    public int EffectiveDeny { get; set; }
  }
}
