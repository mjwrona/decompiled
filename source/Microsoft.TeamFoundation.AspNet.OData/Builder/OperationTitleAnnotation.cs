// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.OperationTitleAnnotation
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

namespace Microsoft.AspNet.OData.Builder
{
  internal class OperationTitleAnnotation
  {
    public OperationTitleAnnotation(string title) => this.Title = title;

    public string Title { get; private set; }
  }
}
