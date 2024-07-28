// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation.FormattingFieldDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation
{
  internal class FormattingFieldDefinition
  {
    public string Name { get; private set; }

    public string ReferenceName { get; private set; }

    public InternalFieldType FieldType { get; private set; }

    public FormattingFieldDefinition(
      string name,
      string referenceName,
      InternalFieldType fieldType)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckForNull<string>(referenceName, nameof (referenceName));
      this.Name = name;
      this.ReferenceName = referenceName;
      this.FieldType = fieldType;
    }
  }
}
