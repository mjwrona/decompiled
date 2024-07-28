// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.CommonStructureServiceFaultCodes
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("The CommonStructureServiceFaultCodes class is obsolete.")]
  public sealed class CommonStructureServiceFaultCodes
  {
    private CommonStructureServiceFaultCodes()
    {
    }

    public static SoapFaultSubCode Unknown => new SoapFaultSubCode(new XmlQualifiedName(nameof (Unknown), "http://schemas.microsoft.com/TeamFoundation/2005/06/CommonStructureService/faultcodes/03"));

    public static SoapFaultSubCode Security => new SoapFaultSubCode(new XmlQualifiedName(nameof (Security), "http://schemas.microsoft.com/TeamFoundation/2005/06/CommonStructureService/faultcodes/03"));

    public static SoapFaultSubCode Service => new SoapFaultSubCode(new XmlQualifiedName(nameof (Service), "http://schemas.microsoft.com/TeamFoundation/2005/06/CommonStructureService/faultcodes/03"));

    public static SoapFaultSubCode Application => new SoapFaultSubCode(new XmlQualifiedName(nameof (Application), "http://schemas.microsoft.com/TeamFoundation/2005/06/CommonStructureService/faultcodes/03"));

    public static SoapFaultSubCode ArgumentException => new SoapFaultSubCode(new XmlQualifiedName(nameof (ArgumentException), "http://schemas.microsoft.com/TeamFoundation/2005/06/CommonStructureService/faultcodes/03"));

    public static SoapFaultSubCode ArgumentNullException => new SoapFaultSubCode(new XmlQualifiedName(nameof (ArgumentNullException), "http://schemas.microsoft.com/TeamFoundation/2005/06/CommonStructureService/faultcodes/03"));

    public static SoapFaultSubCode BadServerConfig => new SoapFaultSubCode(new XmlQualifiedName(nameof (BadServerConfig), "http://schemas.microsoft.com/TeamFoundation/2005/06/CommonStructureService/faultcodes/03"));
  }
}
