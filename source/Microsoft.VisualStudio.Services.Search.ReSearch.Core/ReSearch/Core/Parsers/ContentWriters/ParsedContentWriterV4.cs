// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters.ParsedContentWriterV4
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters
{
  internal class ParsedContentWriterV4 : ParsedContentWriterV3
  {
    private const uint s_initialBufferSize = 512;
    [StaticSafe]
    private static readonly List<CodeTokenKind> s_codeTokenKindsToBeFurtherTokenized = new List<CodeTokenKind>()
    {
      CodeTokenKind.Comment,
      CodeTokenKind.CSUsing,
      CodeTokenKind.CSNamespace,
      CodeTokenKind.CSComment,
      CodeTokenKind.CSStringLiteral,
      CodeTokenKind.CSNumericLiteral,
      CodeTokenKind.JavaLiteral,
      CodeTokenKind.JavaComment,
      CodeTokenKind.JavaPackageDeclaration,
      CodeTokenKind.JavaImportDeclaration,
      CodeTokenKind.VBComment,
      CodeTokenKind.VBNamespaceStatement,
      CodeTokenKind.VBStringLiteral
    };

    public override ParsedData WriteParsedContent(IParsedContent parsedContent)
    {
      CodeParsedContent parsedContent1 = parsedContent as CodeParsedContent;
      this.ParseCodeSymbols(parsedContent1);
      Dictionary<string, List<CodeSymbol>> symbolsInToFields = ParsedContentWriterV4.ParseCodeSymbolsAsFields.ParseCodeSymbolsInToFields(parsedContent1);
      ParsedData parsedData = new ParsedData()
      {
        Fields = new Dictionary<string, byte[]>()
      };
      foreach (KeyValuePair<string, List<CodeSymbol>> keyValuePair in symbolsInToFields)
      {
        string key = keyValuePair.Key;
        List<CodeSymbol> tokens = this.Sort(keyValuePair.Value);
        using (DynamicByteArray buffer = new DynamicByteArray(512U))
        {
          buffer.Position = 0U;
          this.WriteCodeSymbols(buffer, (IEnumerable<CodeSymbol>) tokens);
          parsedData.Fields.Add(key, new byte[(int) buffer.Position]);
          Array.Copy((Array) buffer.GetArray(), (Array) parsedData.Fields[key], parsedData.Fields[key].Length);
        }
      }
      return parsedData;
    }

    private void WriteCodeSymbols(DynamicByteArray buffer, IEnumerable<CodeSymbol> tokens)
    {
      if (tokens == null)
        return;
      foreach (CodeSymbol token in tokens)
      {
        byte[] bytes = Encoding.UTF8.GetBytes(token.Value);
        buffer.WriteVUInt32(token.CharacterOffset);
        buffer.WriteVUInt32(token.TokenOffset);
        buffer.WriteVUInt32((uint) bytes.Length);
        buffer.WriteBytes(bytes);
      }
    }

    protected override List<CodeSymbol> TokenizeCodeSymbols(List<CodeSymbol> symbols)
    {
      RegexTextTokenizer regexTextTokenizer = new RegexTextTokenizer();
      foreach (CodeSymbol codeSymbol1 in symbols.Where<CodeSymbol>((Func<CodeSymbol, bool>) (y => ParsedContentWriterV4.s_codeTokenKindsToBeFurtherTokenized.Contains(y.SymbolType))).ToList<CodeSymbol>())
      {
        regexTextTokenizer.CharacterOffsetBase = codeSymbol1.CharacterOffset;
        symbols.Remove(codeSymbol1);
        foreach (TextToken textToken in regexTextTokenizer.Tokenize(codeSymbol1.Value))
        {
          CodeSymbol codeSymbol2 = new CodeSymbol();
          codeSymbol2.Value = textToken.Value;
          codeSymbol2.SymbolType = codeSymbol1.SymbolType;
          codeSymbol2.CharacterOffset = textToken.CharacterOffset;
          CodeSymbol codeSymbol3 = codeSymbol2;
          symbols.Add(codeSymbol3);
        }
      }
      return symbols;
    }

    internal static class ParseCodeSymbolsAsFields
    {
      public const string ClassField = "Class";
      public const string DefinitionField = "Definition";
      public const string ReferenceField = "Reference";
      public const string MethodField = "Method";
      public const string StringLiteralField = "StringLiteral";
      public const string EnumField = "Enum";
      public const string DeclarationField = "Declaration";
      public const string BaseTypeField = "BaseType";
      public const string NamespaceField = "Namespace";
      public const string TypeField = "Type";
      public const string InterfaceField = "Interface";
      public const string CommentField = "Comment";
      public const string MacroField = "Macro";
      public const string FieldField = "Field";
      internal static readonly Dictionary<string, HashSet<CodeTokenKind>> FieldToAssociatedCodeTokenKindsMap = new Dictionary<string, HashSet<CodeTokenKind>>()
      {
        {
          "Class",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToClass()
        },
        {
          "Definition",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToDefinition()
        },
        {
          "Reference",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToReference()
        },
        {
          "Method",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToMethod()
        },
        {
          "StringLiteral",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToStringLiteral()
        },
        {
          "Enum",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToEnum()
        },
        {
          "Declaration",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToDeclaration()
        },
        {
          "BaseType",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToBaseType()
        },
        {
          "Namespace",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToNamespace()
        },
        {
          "Type",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToType()
        },
        {
          "Interface",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToInterface()
        },
        {
          "Comment",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToComment()
        },
        {
          "Field",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToField()
        },
        {
          "Macro",
          ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToMacro()
        }
      };
      private static readonly Dictionary<CodeTokenKind, HashSet<string>> s_codeTokenKindToFieldsMap = ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindToAssociatedFieldsMap();

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToClass() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.ClassDeclaration,
        CodeTokenKind.ClassDefinition,
        CodeTokenKind.ClassOrStructOrNamespaceDeclaration,
        CodeTokenKind.CSClassDefinition,
        CodeTokenKind.VBClassStatement,
        CodeTokenKind.VBModuleStatement,
        CodeTokenKind.JavaClassDefinition
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToDefinition() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.ClassDefinition,
        CodeTokenKind.ConstantFieldDeclaration,
        CodeTokenKind.ConstructorDefinition,
        CodeTokenKind.DestructorDefinition,
        CodeTokenKind.EnumeratorDefinition,
        CodeTokenKind.EnumeratorItemDeclaration,
        CodeTokenKind.FieldDeclaration,
        CodeTokenKind.FriendFunctionDefinition,
        CodeTokenKind.FunctionDefinition,
        CodeTokenKind.FunctionOrMethodDefinition,
        CodeTokenKind.GlobalConstantDefinition,
        CodeTokenKind.InterfaceDefinition,
        CodeTokenKind.MacroDefinition,
        CodeTokenKind.MethodDefinition,
        CodeTokenKind.OperatorDefinition,
        CodeTokenKind.StructDefinition,
        CodeTokenKind.TypeDefinition,
        CodeTokenKind.UnionDefinition,
        CodeTokenKind.UnknownKindDefinition,
        CodeTokenKind.CSFieldDeclaration,
        CodeTokenKind.CSFieldDeclarationAndAssignment,
        CodeTokenKind.CSMethodDefinition,
        CodeTokenKind.CSParameterName,
        CodeTokenKind.CSPropertyDeclarationName,
        CodeTokenKind.CSVariableDeclarationAndAssignment,
        CodeTokenKind.CSClassDefinition,
        CodeTokenKind.CSAlias,
        CodeTokenKind.CSConversionOperatorTypeDefinition,
        CodeTokenKind.CSOperatorDefinition,
        CodeTokenKind.CSDestructorDefinition,
        CodeTokenKind.CSConstructorDefinition,
        CodeTokenKind.CSEnumDefinition,
        CodeTokenKind.CSEnumMemberDeclaration,
        CodeTokenKind.CSInterfaceDefinition,
        CodeTokenKind.VBClassStatement,
        CodeTokenKind.VBInterfaceStatement,
        CodeTokenKind.VBEnumStatement,
        CodeTokenKind.VBEnumMemberDeclaration,
        CodeTokenKind.VBDelegateSubStatement,
        CodeTokenKind.VBSubStatement,
        CodeTokenKind.VBModuleStatement,
        CodeTokenKind.VBFuctionStatement,
        CodeTokenKind.VBVariableDeclarator,
        CodeTokenKind.VBVariableDeclarationAndAssignment,
        CodeTokenKind.VBFieldDeclaration,
        CodeTokenKind.VBFieldDeclarationAndAssignment,
        CodeTokenKind.VBPropertyStatement,
        CodeTokenKind.JavaClassDefinition,
        CodeTokenKind.JavaMethodDefinition,
        CodeTokenKind.JavaEnumDefinition,
        CodeTokenKind.JavaConstructorDefinition,
        CodeTokenKind.JavaInterfaceDefinition
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToReference() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.FriendClassOrStructDeclaration,
        CodeTokenKind.FriendFunctionDeclaration,
        CodeTokenKind.FriendFunctionDefinition,
        CodeTokenKind.IdentifierReference,
        CodeTokenKind.HeaderReference,
        CodeTokenKind.SystemHeaderReference,
        CodeTokenKind.MacroReference,
        CodeTokenKind.MacroUndefinition,
        CodeTokenKind.NamespaceReference,
        CodeTokenKind.TypeReference,
        CodeTokenKind.CSReturnTypeDeclaration,
        CodeTokenKind.CSParameterType,
        CodeTokenKind.CSPropertyDeclarationType,
        CodeTokenKind.CSVariableDeclarationType,
        CodeTokenKind.CSIdentifier,
        CodeTokenKind.CSArgument,
        CodeTokenKind.CSMemberAccess,
        CodeTokenKind.CSInvocation,
        CodeTokenKind.CSAssignment,
        CodeTokenKind.CSCast,
        CodeTokenKind.CSCatchDeclarationType,
        CodeTokenKind.CSFieldDeclarationType,
        CodeTokenKind.CSObjectCreationType,
        CodeTokenKind.VBAssignment,
        CodeTokenKind.VBIdentifier,
        CodeTokenKind.VBParameter,
        CodeTokenKind.VBInvocationExpression,
        CodeTokenKind.JavaIdentifier,
        CodeTokenKind.JavaFormalParameter,
        CodeTokenKind.JavaCatchClauseIdentifier,
        CodeTokenKind.JavaReturnTypeDeclaration,
        CodeTokenKind.JavaTypeArgument,
        CodeTokenKind.JavaParameterType,
        CodeTokenKind.JavaFieldDeclarationType,
        CodeTokenKind.JavaType,
        CodeTokenKind.JavaLocalVariableDeclarationType,
        CodeTokenKind.JavaExceptionType,
        CodeTokenKind.JavaCatchType
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToComment() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.Comment,
        CodeTokenKind.CSComment,
        CodeTokenKind.VBComment,
        CodeTokenKind.JavaComment
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToDeclaration() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.ArgumentDeclaration,
        CodeTokenKind.BaseTypeDeclaration,
        CodeTokenKind.ClassDeclaration,
        CodeTokenKind.ClassOrStructOrNamespaceDeclaration,
        CodeTokenKind.ConstantFieldDeclaration,
        CodeTokenKind.ConstructorDeclaration,
        CodeTokenKind.DestructorDeclaration,
        CodeTokenKind.EnumeratorDeclaration,
        CodeTokenKind.EnumeratorItemDeclaration,
        CodeTokenKind.ExternVariableDeclaration,
        CodeTokenKind.ExternFunctionDeclaration,
        CodeTokenKind.FieldDeclaration,
        CodeTokenKind.FriendClassOrStructDeclaration,
        CodeTokenKind.FriendFunctionDeclaration,
        CodeTokenKind.FunctionDeclaration,
        CodeTokenKind.GlobalConstantDeclaration,
        CodeTokenKind.GlobalVariableDeclaration,
        CodeTokenKind.InterfaceDeclaration,
        CodeTokenKind.MethodDeclaration,
        CodeTokenKind.NamespaceDeclaration,
        CodeTokenKind.OperatorDeclaration,
        CodeTokenKind.ReturnTypeDeclaration,
        CodeTokenKind.StructDeclaration,
        CodeTokenKind.TemplateArgumentDeclaration,
        CodeTokenKind.TemplateSpecificationDeclaration,
        CodeTokenKind.UnionDeclaration,
        CodeTokenKind.UnknownKindDeclaration,
        CodeTokenKind.CSConversionOperatorTypeDeclaration,
        CodeTokenKind.CSFieldDeclaration,
        CodeTokenKind.CSFieldDeclarationAndAssignment,
        CodeTokenKind.CSPropertyDeclarationName,
        CodeTokenKind.CSVariableDeclaration,
        CodeTokenKind.CSVariableDeclarationAndAssignment,
        CodeTokenKind.CSCatchDeclarationIdentifier,
        CodeTokenKind.CSConstructorDeclaration,
        CodeTokenKind.CSDelegateDeclaration,
        CodeTokenKind.CSDestructorDeclaration,
        CodeTokenKind.CSEnumMemberDeclaration,
        CodeTokenKind.CSOperatorDeclaration,
        CodeTokenKind.CSParameterName,
        CodeTokenKind.CSAttributeParameterName,
        CodeTokenKind.CSInterfaceDefinition,
        CodeTokenKind.VBEventStatement,
        CodeTokenKind.VBEnumStatement,
        CodeTokenKind.VBEnumMemberDeclaration,
        CodeTokenKind.VBParameter,
        CodeTokenKind.VBVariableDeclarator,
        CodeTokenKind.VBVariableDeclarationAndAssignment,
        CodeTokenKind.VBFieldDeclaration,
        CodeTokenKind.VBFieldDeclarationAndAssignment,
        CodeTokenKind.VBInterfaceMethodDeclaration,
        CodeTokenKind.VBInterfaceStatement,
        CodeTokenKind.VBPropertyStatement,
        CodeTokenKind.JavaFormalParameter,
        CodeTokenKind.JavaFieldDeclaration,
        CodeTokenKind.JavaLocalVariableDeclaration,
        CodeTokenKind.JavaVariableDeclarator
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToNamespace() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.ClassOrStructOrNamespaceDeclaration,
        CodeTokenKind.NamespaceDeclaration,
        CodeTokenKind.NamespaceReference,
        CodeTokenKind.CSNamespace,
        CodeTokenKind.VBNamespaceStatement,
        CodeTokenKind.JavaPackageDeclaration
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToEnum() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.EnumeratorDeclaration,
        CodeTokenKind.EnumeratorDefinition,
        CodeTokenKind.EnumeratorItemDeclaration,
        CodeTokenKind.CSEnumMemberDeclaration,
        CodeTokenKind.CSEnumDefinition,
        CodeTokenKind.VBEnumStatement,
        CodeTokenKind.VBEnumMemberDeclaration,
        CodeTokenKind.JavaEnumDefinition,
        CodeTokenKind.JavaEnumConstant
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToStringLiteral() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.CSStringLiteral,
        CodeTokenKind.VBStringLiteral,
        CodeTokenKind.JavaLiteral
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToMethod() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.ConstructorDeclaration,
        CodeTokenKind.ConstructorDefinition,
        CodeTokenKind.DestructorDeclaration,
        CodeTokenKind.DestructorDefinition,
        CodeTokenKind.FunctionDeclaration,
        CodeTokenKind.FunctionDefinition,
        CodeTokenKind.FunctionOrMethodDefinition,
        CodeTokenKind.MethodDeclaration,
        CodeTokenKind.MethodDefinition,
        CodeTokenKind.CSConstructorDeclaration,
        CodeTokenKind.CSConstructorDefinition,
        CodeTokenKind.CSDestructorDeclaration,
        CodeTokenKind.CSDestructorDefinition,
        CodeTokenKind.CSMethodDefinition,
        CodeTokenKind.CSMethodDeclaration,
        CodeTokenKind.VBFuctionStatement,
        CodeTokenKind.VBSubNewStatement,
        CodeTokenKind.VBSubStatement,
        CodeTokenKind.VBInterfaceMethodDeclaration,
        CodeTokenKind.JavaMethodDefinition,
        CodeTokenKind.JavaConstructorDefinition
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToType()
      {
        HashSet<CodeTokenKind> kindsRelatedToType = new HashSet<CodeTokenKind>()
        {
          CodeTokenKind.StructDeclaration,
          CodeTokenKind.StructDefinition,
          CodeTokenKind.VBStructureStatement,
          CodeTokenKind.TypeDefinition,
          CodeTokenKind.UnionDeclaration,
          CodeTokenKind.UnionDefinition,
          CodeTokenKind.ClassDeclaration,
          CodeTokenKind.ClassDefinition,
          CodeTokenKind.ClassOrStructOrNamespaceDeclaration,
          CodeTokenKind.CSClassDefinition,
          CodeTokenKind.VBClassStatement,
          CodeTokenKind.VBModuleStatement,
          CodeTokenKind.JavaClassDefinition
        };
        foreach (CodeTokenKind codeTokenKind in ParsedContentWriterV4.ParseCodeSymbolsAsFields.GetCodeTokenKindsRelatedToInterface())
          kindsRelatedToType.Add(codeTokenKind);
        return kindsRelatedToType;
      }

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToBaseType() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.BaseTypeDeclaration,
        CodeTokenKind.CSBaseType,
        CodeTokenKind.VBHandlesItem,
        CodeTokenKind.VBInheritsStatement,
        CodeTokenKind.VBImplements,
        CodeTokenKind.VBClassImplements,
        CodeTokenKind.JavaBaseType
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToInterface() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.InterfaceDeclaration,
        CodeTokenKind.InterfaceDefinition,
        CodeTokenKind.CSInterfaceDefinition,
        CodeTokenKind.VBInterfaceStatement,
        CodeTokenKind.JavaInterfaceDefinition
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToMacro() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.MacroDefinition,
        CodeTokenKind.MacroReference,
        CodeTokenKind.MacroUndefinition
      };

      internal static HashSet<CodeTokenKind> GetCodeTokenKindsRelatedToField() => new HashSet<CodeTokenKind>()
      {
        CodeTokenKind.CSPropertyDeclarationName,
        CodeTokenKind.VBPropertyStatement,
        CodeTokenKind.ConstantFieldDeclaration,
        CodeTokenKind.FieldDeclaration,
        CodeTokenKind.CSFieldDeclaration,
        CodeTokenKind.CSFieldDeclarationAndAssignment,
        CodeTokenKind.VBFieldDeclaration,
        CodeTokenKind.VBFieldDeclarationAndAssignment,
        CodeTokenKind.JavaFieldDeclaration
      };

      public static Dictionary<string, List<CodeSymbol>> ParseCodeSymbolsInToFields(
        CodeParsedContent parsedContent)
      {
        Dictionary<string, List<CodeSymbol>> symbolsInToFields = new Dictionary<string, List<CodeSymbol>>();
        if (parsedContent.HasCodeSymbols)
        {
          foreach (CodeSymbol symbol in parsedContent.Symbols)
          {
            HashSet<string> stringSet;
            if (ParsedContentWriterV4.ParseCodeSymbolsAsFields.s_codeTokenKindToFieldsMap.TryGetValue(symbol.SymbolType, out stringSet) && stringSet != null)
            {
              foreach (string key in stringSet)
              {
                List<CodeSymbol> codeSymbolList;
                if (symbolsInToFields.TryGetValue(key, out codeSymbolList))
                  codeSymbolList.Add(symbol);
                else
                  symbolsInToFields.Add(key, new List<CodeSymbol>()
                  {
                    symbol
                  });
              }
            }
          }
        }
        return symbolsInToFields;
      }

      private static Dictionary<CodeTokenKind, HashSet<string>> GetCodeTokenKindToAssociatedFieldsMap()
      {
        Dictionary<CodeTokenKind, HashSet<string>> associatedFieldsMap = new Dictionary<CodeTokenKind, HashSet<string>>();
        foreach (string key1 in ParsedContentWriterV4.ParseCodeSymbolsAsFields.FieldToAssociatedCodeTokenKindsMap.Keys)
        {
          foreach (CodeTokenKind key2 in ParsedContentWriterV4.ParseCodeSymbolsAsFields.FieldToAssociatedCodeTokenKindsMap[key1])
          {
            HashSet<string> stringSet;
            associatedFieldsMap.TryGetValue(key2, out stringSet);
            if (stringSet == null)
              associatedFieldsMap.Add(key2, new HashSet<string>()
              {
                key1
              });
            else
              associatedFieldsMap[key2].Add(key1);
          }
        }
        return associatedFieldsMap;
      }
    }
  }
}
