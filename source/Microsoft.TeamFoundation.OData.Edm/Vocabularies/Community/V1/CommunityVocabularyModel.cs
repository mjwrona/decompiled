// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.Community.V1.CommunityVocabularyModel
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies.V1;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm.Vocabularies.Community.V1
{
  public static class CommunityVocabularyModel
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
    public static readonly IEdmModel Instance = VocabularyModelProvider.CommunityModel;
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm UrlEscapeFunctionTerm = VocabularyModelProvider.CommunityModel.FindDeclaredTerm("Org.OData.Community.V1.UrlEscapeFunction");
  }
}
