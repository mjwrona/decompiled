// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataEdmPropertyAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Camel-casing in class name.")]
  public sealed class ODataEdmPropertyAnnotation
  {
    public ODataNullValueBehaviorKind NullValueReadBehaviorKind { get; set; }
  }
}
