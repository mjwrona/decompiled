// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlAnnotations
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlAnnotations
  {
    private readonly List<CsdlAnnotation> annotations;
    private readonly string target;
    private readonly string qualifier;

    public CsdlAnnotations(
      IEnumerable<CsdlAnnotation> annotations,
      string target,
      string qualifier)
    {
      this.annotations = new List<CsdlAnnotation>(annotations);
      this.target = target;
      this.qualifier = qualifier;
    }

    public IEnumerable<CsdlAnnotation> Annotations => (IEnumerable<CsdlAnnotation>) this.annotations;

    public string Qualifier => this.qualifier;

    public string Target => this.target;
  }
}
