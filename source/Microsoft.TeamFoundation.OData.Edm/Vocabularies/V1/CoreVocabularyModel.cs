// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.V1.CoreVocabularyModel
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm.Vocabularies.V1
{
  public static class CoreVocabularyModel
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmModel is immutable")]
    public static readonly IEdmModel Instance = VocabularyModelProvider.CoreModel;
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm ConcurrencyTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.OptimisticConcurrency");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm DescriptionTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.Description");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm LongDescriptionTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.LongDescription");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm IsLanguageDependentTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.IsLanguageDependent");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm RequiresTypeTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.RequiresType");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm ResourcePathTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.ResourcePath");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm DereferenceableIDsTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.DereferenceableIDs");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm ConventionalIDsTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.ConventionalIDs");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm ImmutableTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.Immutable");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm ComputedTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.Computed");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm OptionalParameterTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.OptionalParameter");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm IsURLTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.IsURL");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm AcceptableMediaTypesTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.AcceptableMediaTypes");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm MediaTypeTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.MediaType");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm IsMediaTypeTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.IsMediaType");
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "EdmTerm is immutable")]
    public static readonly IEdmTerm PermissionsTerm = VocabularyModelProvider.CoreModel.FindDeclaredTerm("Org.OData.Core.V1.Permissions");
  }
}
