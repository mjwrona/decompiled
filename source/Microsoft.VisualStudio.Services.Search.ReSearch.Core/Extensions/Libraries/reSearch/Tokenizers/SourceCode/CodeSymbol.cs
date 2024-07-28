// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode.CodeSymbol
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode
{
  public class CodeSymbol : TextToken
  {
    public CodeTokenKind SymbolType { get; set; }

    public uint ScopeBegin { get; set; }

    public uint ScopeEnd { get; set; }

    public uint ScopeLevel { get; set; }

    public int ScopeBeginLine { get; set; }

    public int ScopeEndLine { get; set; }

    public bool IsExtendedSymbol { get; set; }

    public uint SymbolLengthExtended { get; set; }

    public uint FieldIndex { get; set; }

    public static bool HasScope(CodeTokenKind symbolType)
    {
      switch (symbolType)
      {
        case CodeTokenKind.ClassDefinition:
        case CodeTokenKind.ConstructorDefinition:
        case CodeTokenKind.DestructorDefinition:
        case CodeTokenKind.EnumeratorDefinition:
        case CodeTokenKind.FunctionDefinition:
        case CodeTokenKind.FunctionOrMethodDefinition:
        case CodeTokenKind.MethodDefinition:
        case CodeTokenKind.NamespaceDeclaration:
        case CodeTokenKind.OperatorDefinition:
        case CodeTokenKind.StructDefinition:
        case CodeTokenKind.TypeDefinition:
        case CodeTokenKind.UnionDefinition:
          return true;
        default:
          return false;
      }
    }

    public static Pivot GetPivotFromKind(CodeTokenKind tokenKind)
    {
      switch (tokenKind)
      {
        case CodeTokenKind.ArgumentDeclaration:
        case CodeTokenKind.CSParameterName:
        case CodeTokenKind.CSAttributeParameterName:
          return Pivot.Argument;
        case CodeTokenKind.BaseTypeDeclaration:
          return Pivot.BaseType;
        case CodeTokenKind.ClassDeclaration:
        case CodeTokenKind.ClassDefinition:
        case CodeTokenKind.CSClassDefinition:
          return Pivot.Class;
        case CodeTokenKind.ClassOrStructOrNamespaceDeclaration:
          return Pivot.ClassOrStructOrNamespace;
        case CodeTokenKind.Comment:
        case CodeTokenKind.CSComment:
          return Pivot.Comment;
        case CodeTokenKind.ConstantFieldDeclaration:
        case CodeTokenKind.FieldDeclaration:
        case CodeTokenKind.CSFieldDeclaration:
        case CodeTokenKind.CSFieldDeclarationAndAssignment:
          return Pivot.Field;
        case CodeTokenKind.ConstructorDeclaration:
        case CodeTokenKind.ConstructorDefinition:
        case CodeTokenKind.CSConstructorDeclaration:
        case CodeTokenKind.CSConstructorDefinition:
          return Pivot.Constructor;
        case CodeTokenKind.DestructorDeclaration:
        case CodeTokenKind.DestructorDefinition:
        case CodeTokenKind.CSDestructorDeclaration:
        case CodeTokenKind.CSDestructorDefinition:
          return Pivot.Destructor;
        case CodeTokenKind.EnumeratorDeclaration:
        case CodeTokenKind.EnumeratorDefinition:
        case CodeTokenKind.EnumeratorItemDeclaration:
        case CodeTokenKind.CSEnumMemberDeclaration:
        case CodeTokenKind.CSEnumDefinition:
          return Pivot.Enumerator;
        case CodeTokenKind.ExternVariableDeclaration:
        case CodeTokenKind.ExternFunctionDeclaration:
          return Pivot.Extern;
        case CodeTokenKind.FriendClassOrStructDeclaration:
        case CodeTokenKind.FriendFunctionDeclaration:
        case CodeTokenKind.FriendFunctionDefinition:
          return Pivot.Friend;
        case CodeTokenKind.FunctionDeclaration:
        case CodeTokenKind.FunctionDefinition:
          return Pivot.Function;
        case CodeTokenKind.FunctionOrMethodDefinition:
          return Pivot.FunctionOrMethod;
        case CodeTokenKind.GlobalConstantDefinition:
        case CodeTokenKind.GlobalConstantDeclaration:
        case CodeTokenKind.GlobalVariableDeclaration:
          return Pivot.Global;
        case CodeTokenKind.IdentifierReference:
        case CodeTokenKind.CSIdentifier:
        case CodeTokenKind.CSArgument:
        case CodeTokenKind.CSMemberAccess:
        case CodeTokenKind.CSAssignment:
          return Pivot.Reference;
        case CodeTokenKind.HeaderReference:
        case CodeTokenKind.SystemHeaderReference:
          return Pivot.Header;
        case CodeTokenKind.InterfaceDeclaration:
        case CodeTokenKind.InterfaceDefinition:
          return Pivot.Interface;
        case CodeTokenKind.MacroDefinition:
        case CodeTokenKind.MacroReference:
        case CodeTokenKind.MacroUndefinition:
          return Pivot.Macro;
        case CodeTokenKind.MethodDeclaration:
        case CodeTokenKind.MethodDefinition:
        case CodeTokenKind.CSMethodDefinition:
        case CodeTokenKind.CSInvocation:
        case CodeTokenKind.CSMethodDeclaration:
          return Pivot.Method;
        case CodeTokenKind.NamespaceDeclaration:
        case CodeTokenKind.NamespaceReference:
        case CodeTokenKind.CSNamespace:
          return Pivot.Namespace;
        case CodeTokenKind.OperatorDeclaration:
        case CodeTokenKind.OperatorDefinition:
        case CodeTokenKind.CSOperatorDeclaration:
        case CodeTokenKind.CSOperatorDefinition:
          return Pivot.Operator;
        case CodeTokenKind.ReturnTypeDeclaration:
        case CodeTokenKind.TypeReference:
        case CodeTokenKind.CSReturnTypeDeclaration:
        case CodeTokenKind.CSParameterType:
        case CodeTokenKind.CSPropertyDeclarationType:
        case CodeTokenKind.CSVariableDeclarationType:
        case CodeTokenKind.CSCast:
        case CodeTokenKind.CSCatchDeclarationType:
        case CodeTokenKind.CSConversionOperatorTypeDeclaration:
        case CodeTokenKind.CSFieldDeclarationType:
          return Pivot.Type;
        case CodeTokenKind.StructDeclaration:
        case CodeTokenKind.StructDefinition:
          return Pivot.Struct;
        case CodeTokenKind.TemplateArgumentDeclaration:
          return Pivot.TemplateArgument;
        case CodeTokenKind.TemplateSpecificationDeclaration:
          return Pivot.TemplateSpecification;
        case CodeTokenKind.TypeDefinition:
          return Pivot.Typedef;
        case CodeTokenKind.UnionDeclaration:
        case CodeTokenKind.UnionDefinition:
          return Pivot.Union;
        case CodeTokenKind.CSPropertyDeclarationName:
          return Pivot.Property;
        case CodeTokenKind.CSStringLiteral:
          return Pivot.StringLiteral;
        case CodeTokenKind.CSAttributeName:
          return Pivot.Attribute;
        default:
          return Pivot.Unknown;
      }
    }

    public static CodeTokenUse GetUseFromKind(CodeTokenKind tokenKind)
    {
      switch (tokenKind)
      {
        case CodeTokenKind.ArgumentDeclaration:
        case CodeTokenKind.BaseTypeDeclaration:
        case CodeTokenKind.ClassDeclaration:
        case CodeTokenKind.ClassOrStructOrNamespaceDeclaration:
        case CodeTokenKind.ConstantFieldDeclaration:
        case CodeTokenKind.ConstructorDeclaration:
        case CodeTokenKind.DestructorDeclaration:
        case CodeTokenKind.EnumeratorDeclaration:
        case CodeTokenKind.EnumeratorItemDeclaration:
        case CodeTokenKind.ExternVariableDeclaration:
        case CodeTokenKind.ExternFunctionDeclaration:
        case CodeTokenKind.FieldDeclaration:
        case CodeTokenKind.FriendClassOrStructDeclaration:
        case CodeTokenKind.FriendFunctionDeclaration:
        case CodeTokenKind.FunctionDeclaration:
        case CodeTokenKind.GlobalConstantDeclaration:
        case CodeTokenKind.GlobalVariableDeclaration:
        case CodeTokenKind.InterfaceDeclaration:
        case CodeTokenKind.MethodDeclaration:
        case CodeTokenKind.NamespaceDeclaration:
        case CodeTokenKind.OperatorDeclaration:
        case CodeTokenKind.ReturnTypeDeclaration:
        case CodeTokenKind.StructDeclaration:
        case CodeTokenKind.TemplateArgumentDeclaration:
        case CodeTokenKind.TemplateSpecificationDeclaration:
        case CodeTokenKind.UnionDeclaration:
        case CodeTokenKind.UnknownKindDeclaration:
        case CodeTokenKind.CSPropertyDeclarationName:
        case CodeTokenKind.CSVariableDeclaration:
        case CodeTokenKind.CSCatchDeclarationIdentifier:
        case CodeTokenKind.CSConstructorDeclaration:
        case CodeTokenKind.CSDelegateDeclaration:
        case CodeTokenKind.CSDestructorDeclaration:
        case CodeTokenKind.CSOperatorDeclaration:
          return CodeTokenUse.Declaration;
        case CodeTokenKind.ClassDefinition:
        case CodeTokenKind.ConstructorDefinition:
        case CodeTokenKind.DestructorDefinition:
        case CodeTokenKind.EnumeratorDefinition:
        case CodeTokenKind.FriendFunctionDefinition:
        case CodeTokenKind.FunctionDefinition:
        case CodeTokenKind.FunctionOrMethodDefinition:
        case CodeTokenKind.GlobalConstantDefinition:
        case CodeTokenKind.InterfaceDefinition:
        case CodeTokenKind.MacroDefinition:
        case CodeTokenKind.MethodDefinition:
        case CodeTokenKind.OperatorDefinition:
        case CodeTokenKind.StructDefinition:
        case CodeTokenKind.TypeDefinition:
        case CodeTokenKind.UnionDefinition:
        case CodeTokenKind.UnknownKindDefinition:
        case CodeTokenKind.CSMethodDefinition:
        case CodeTokenKind.CSParameterName:
        case CodeTokenKind.CSVariableDeclarationAndAssignment:
        case CodeTokenKind.CSClassDefinition:
        case CodeTokenKind.CSAlias:
        case CodeTokenKind.CSConversionOperatorTypeDefinition:
        case CodeTokenKind.CSOperatorDefinition:
        case CodeTokenKind.CSDestructorDefinition:
        case CodeTokenKind.CSConstructorDefinition:
        case CodeTokenKind.CSEnumDefinition:
          return CodeTokenUse.Definition;
        case CodeTokenKind.IdentifierReference:
        case CodeTokenKind.HeaderReference:
        case CodeTokenKind.SystemHeaderReference:
        case CodeTokenKind.MacroReference:
        case CodeTokenKind.NamespaceReference:
        case CodeTokenKind.TypeReference:
        case CodeTokenKind.CSReturnTypeDeclaration:
        case CodeTokenKind.CSParameterType:
        case CodeTokenKind.CSPropertyDeclarationType:
        case CodeTokenKind.CSVariableDeclarationType:
        case CodeTokenKind.CSIdentifier:
        case CodeTokenKind.CSArgument:
        case CodeTokenKind.CSMemberAccess:
        case CodeTokenKind.CSInvocation:
        case CodeTokenKind.CSAssignment:
        case CodeTokenKind.CSCast:
        case CodeTokenKind.CSCatchDeclarationType:
        case CodeTokenKind.CSFieldDeclarationType:
          return CodeTokenUse.Reference;
        case CodeTokenKind.MacroUndefinition:
          return CodeTokenUse.Reference;
        default:
          return CodeTokenUse.Unknown;
      }
    }
  }
}
