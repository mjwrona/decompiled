// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public class EdmReference : IEdmReference, IEdmElement
  {
    private Uri uri;
    private List<IEdmInclude> includes = new List<IEdmInclude>();
    private List<IEdmIncludeAnnotations> includeAnnotations = new List<IEdmIncludeAnnotations>();

    public EdmReference(Uri uri) => this.uri = uri;

    public Uri Uri => this.uri;

    public IEnumerable<IEdmInclude> Includes => (IEnumerable<IEdmInclude>) this.includes;

    public IEnumerable<IEdmIncludeAnnotations> IncludeAnnotations => (IEnumerable<IEdmIncludeAnnotations>) this.includeAnnotations;

    public void AddInclude(IEdmInclude edmInclude) => this.includes.Add(edmInclude);

    public void AddIncludeAnnotations(IEdmIncludeAnnotations edmIncludeAnnotations) => this.includeAnnotations.Add(edmIncludeAnnotations);
  }
}
