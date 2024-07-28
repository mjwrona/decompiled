// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmIncludeAnnotations
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmIncludeAnnotations : IEdmIncludeAnnotations
  {
    private readonly string termNamespace;
    private readonly string qualifier;
    private readonly string targetNamespace;

    public EdmIncludeAnnotations(string termNamespace, string qualifier, string targetNamespace)
    {
      this.termNamespace = termNamespace;
      this.qualifier = qualifier;
      this.targetNamespace = targetNamespace;
    }

    public string TermNamespace => this.termNamespace;

    public string Qualifier => this.qualifier;

    public string TargetNamespace => this.targetNamespace;
  }
}
