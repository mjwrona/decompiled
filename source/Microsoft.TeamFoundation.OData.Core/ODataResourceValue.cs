// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataResourceValue
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
  public sealed class ODataResourceValue : ODataValue
  {
    public string TypeName { get; set; }

    public IEnumerable<ODataProperty> Properties { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
    public ICollection<ODataInstanceAnnotation> InstanceAnnotations
    {
      get => this.GetInstanceAnnotations();
      set => this.SetInstanceAnnotations(value);
    }
  }
}
