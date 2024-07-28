// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.V1.VocabularyModelProvider
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Microsoft.OData.Edm.Vocabularies.V1
{
  internal static class VocabularyModelProvider
  {
    public static readonly IEdmModel CoreModel;
    public static readonly IEdmModel CapabilitiesModel;
    public static readonly IEdmModel AlternateKeyModel;
    public static readonly IEdmModel CommunityModel;
    public static readonly IEdmModel ValidationModel;
    public static readonly IEdmModel AuthorizationModel;
    public static readonly IEnumerable<IEdmModel> VocabularyModels;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static VocabularyModelProvider()
    {
      Assembly assembly = typeof (VocabularyModelProvider).GetAssembly();
      string[] manifestResourceNames = assembly.GetManifestResourceNames();
      string vocabularyName1 = ((IEnumerable<string>) manifestResourceNames).FirstOrDefault<string>((Func<string, bool>) (x => x.Contains("CoreVocabularies.xml")));
      VocabularyModelProvider.CoreModel = VocabularyModelProvider.LoadSchemaEdmModel(assembly, vocabularyName1, (IEnumerable<IEdmModel>) new IEdmModel[0]);
      string vocabularyName2 = ((IEnumerable<string>) manifestResourceNames).FirstOrDefault<string>((Func<string, bool>) (x => x.Contains("AuthorizationVocabularies.xml")));
      VocabularyModelProvider.AuthorizationModel = VocabularyModelProvider.LoadCsdlEdmModel(assembly, vocabularyName2, (IEnumerable<IEdmModel>) new IEdmModel[1]
      {
        VocabularyModelProvider.CoreModel
      });
      string vocabularyName3 = ((IEnumerable<string>) manifestResourceNames).Where<string>((Func<string, bool>) (x => x.Contains("ValidationVocabularies.xml"))).FirstOrDefault<string>();
      VocabularyModelProvider.ValidationModel = VocabularyModelProvider.LoadSchemaEdmModel(assembly, vocabularyName3, (IEnumerable<IEdmModel>) new IEdmModel[1]
      {
        VocabularyModelProvider.CoreModel
      });
      string vocabularyName4 = ((IEnumerable<string>) manifestResourceNames).FirstOrDefault<string>((Func<string, bool>) (x => x.Contains("CapabilitiesVocabularies.xml")));
      VocabularyModelProvider.CapabilitiesModel = VocabularyModelProvider.LoadCsdlEdmModel(assembly, vocabularyName4, (IEnumerable<IEdmModel>) new IEdmModel[3]
      {
        VocabularyModelProvider.CoreModel,
        VocabularyModelProvider.AuthorizationModel,
        VocabularyModelProvider.ValidationModel
      });
      string vocabularyName5 = ((IEnumerable<string>) manifestResourceNames).Where<string>((Func<string, bool>) (x => x.Contains("AlternateKeysVocabularies.xml"))).FirstOrDefault<string>();
      VocabularyModelProvider.AlternateKeyModel = VocabularyModelProvider.LoadSchemaEdmModel(assembly, vocabularyName5, (IEnumerable<IEdmModel>) new IEdmModel[1]
      {
        VocabularyModelProvider.CoreModel
      });
      string vocabularyName6 = ((IEnumerable<string>) manifestResourceNames).Where<string>((Func<string, bool>) (x => x.Contains("CommunityVocabularies.xml"))).FirstOrDefault<string>();
      VocabularyModelProvider.CommunityModel = VocabularyModelProvider.LoadCsdlEdmModel(assembly, vocabularyName6, (IEnumerable<IEdmModel>) new IEdmModel[1]
      {
        VocabularyModelProvider.CoreModel
      });
      VocabularyModelProvider.VocabularyModels = (IEnumerable<IEdmModel>) new List<IEdmModel>()
      {
        VocabularyModelProvider.CoreModel,
        VocabularyModelProvider.CapabilitiesModel,
        VocabularyModelProvider.AlternateKeyModel,
        VocabularyModelProvider.CommunityModel,
        VocabularyModelProvider.ValidationModel,
        VocabularyModelProvider.AuthorizationModel
      };
    }

    private static IEdmModel LoadCsdlEdmModel(
      Assembly assembly,
      string vocabularyName,
      IEnumerable<IEdmModel> referenceModels)
    {
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(vocabularyName))
      {
        IEdmModel model;
        CsdlReader.TryParse(XmlReader.Create(manifestResourceStream), referenceModels, false, out model, out IEnumerable<EdmError> _);
        return model;
      }
    }

    private static IEdmModel LoadSchemaEdmModel(
      Assembly assembly,
      string vocabularyName,
      IEnumerable<IEdmModel> referenceModels)
    {
      using (Stream manifestResourceStream = assembly.GetManifestResourceStream(vocabularyName))
      {
        IEdmModel model;
        SchemaReader.TryParse((IEnumerable<XmlReader>) new XmlReader[1]
        {
          XmlReader.Create(manifestResourceStream)
        }, referenceModels, false, out model, out IEnumerable<EdmError> _);
        return model;
      }
    }
  }
}
