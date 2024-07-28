// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.WorkItemTypeDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemTypeDefinition
  {
    public string TypeName { get; set; }

    public string TypeRefName { get; set; }

    public XmlElement Description { get; set; }

    public XmlElement FieldDefinitions { get; set; }

    public XmlElement Workflow { get; set; }

    public XmlElement Form { get; set; }
  }
}
