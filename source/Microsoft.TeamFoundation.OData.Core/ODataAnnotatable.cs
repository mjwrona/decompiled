// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataAnnotatable
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.OData
{
  public abstract class ODataAnnotatable
  {
    private ICollection<ODataInstanceAnnotation> instanceAnnotations = (ICollection<ODataInstanceAnnotation>) new Collection<ODataInstanceAnnotation>();

    public ODataTypeAnnotation TypeAnnotation { get; set; }

    internal ICollection<ODataInstanceAnnotation> GetInstanceAnnotations() => this.instanceAnnotations;

    internal void SetInstanceAnnotations(ICollection<ODataInstanceAnnotation> value)
    {
      ExceptionUtils.CheckArgumentNotNull<ICollection<ODataInstanceAnnotation>>(value, nameof (value));
      this.instanceAnnotations = value;
    }
  }
}
