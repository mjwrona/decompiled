// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.WitProcessValidatorXMLHelpers
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  internal static class WitProcessValidatorXMLHelpers
  {
    public static IEnumerable<XElement> AllFieldsInWIT(this XDocument typeDocument) => typeDocument.Root.Element((XName) "WORKITEMTYPE").Element((XName) "FIELDS").Elements();

    public static IEnumerable<XElement> AllFieldsInWorkflow(this XDocument typeDocument) => typeDocument.Root.Element((XName) "WORKITEMTYPE").Element((XName) "WORKFLOW").Descendants((XName) "FIELD");

    public static string GetWITName(this XDocument typeDocument) => typeDocument.Root.Element((XName) "WORKITEMTYPE").Attribute((XName) "name").Value;

    public static string GetWITRefName(this XDocument typeDocument) => typeDocument.Root.Element((XName) "WORKITEMTYPE").Attribute((XName) "refname").Value;
  }
}
