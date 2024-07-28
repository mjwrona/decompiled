// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNodeExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 77D96756-A6EC-4CC5-958E-440F0412CE7F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Common.dll

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.TeamFoundation.CodeSense.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.CSharp
{
  public static class CSharpSyntaxNodeExtensions
  {
    private static ConditionalWeakTable<CSharpSyntaxNode, CodeElementIdentity> identityCache = new ConditionalWeakTable<CSharpSyntaxNode, CodeElementIdentity>();

    public static CodeElementIdentity GetCodeElementIdentity(this CSharpSyntaxNode node) => CSharpSyntaxNodeExtensions.identityCache.GetValue(node, CSharpSyntaxNodeExtensions.\u003C\u003EO.\u003C0\u003E__Create ?? (CSharpSyntaxNodeExtensions.\u003C\u003EO.\u003C0\u003E__Create = new ConditionalWeakTable<CSharpSyntaxNode, CodeElementIdentity>.CreateValueCallback(CSharpSyntaxNodeExtensions.Create)));

    private static CodeElementIdentity Create(CSharpSyntaxNode node) => ((SyntaxNode) node).Parent == null ? node.Accept<CodeElementIdentity>((CSharpSyntaxVisitor<CodeElementIdentity>) new CSharpSyntaxNodeExtensions.PropertyVisitor(new CodeElementIdentity("C#"))) : node.Accept<CodeElementIdentity>((CSharpSyntaxVisitor<CodeElementIdentity>) new CSharpSyntaxNodeExtensions.PropertyVisitor(((CSharpSyntaxNode) ((SyntaxNode) node).Parent).GetCodeElementIdentity()));

    private class PropertyVisitor : CSharpSyntaxVisitor<CodeElementIdentity>
    {
      private const string PropertyBagNameKey = "Name";
      private const string PropertyBagParametersKey = "Parameters";
      private readonly CodeElementIdentity parentPropertyBag;

      public PropertyVisitor(CodeElementIdentity parentPropertyBag)
      {
        ArgumentUtility.CheckForNull<CodeElementIdentity>(parentPropertyBag, nameof (parentPropertyBag));
        this.parentPropertyBag = parentPropertyBag;
      }

      public override CodeElementIdentity DefaultVisit(SyntaxNode node)
      {
        string str;
        if (!this.parentPropertyBag.TryGetValue("Name", out str))
          return new CodeElementIdentity("C#");
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
      {
        string str1 = CSharpSyntaxNodeExtensions.PropertyVisitor.RemoveTrivia<NameSyntax>(node.Name).ToString();
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitClassDeclaration(ClassDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString() + CSharpSyntaxNodeExtensions.PropertyVisitor.GetGenericParameters(node.TypeParameterList);
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitStructDeclaration(StructDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString() + CSharpSyntaxNodeExtensions.PropertyVisitor.GetGenericParameters(node.TypeParameterList);
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString() + CSharpSyntaxNodeExtensions.PropertyVisitor.GetGenericParameters(node.TypeParameterList);
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitEnumDeclaration(EnumDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString();
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitPropertyDeclaration(PropertyDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString();
        string str2 = node.ExplicitInterfaceSpecifier != null ? string.Format("({0}.{1})", (object) CSharpSyntaxNodeExtensions.PropertyVisitor.RemoveTrivia<NameSyntax>(node.ExplicitInterfaceSpecifier.Name).ToString(), (object) str1) : str1;
        string str3;
        string str4;
        if (!this.parentPropertyBag.TryGetValue("Name", out str3))
          str4 = str2;
        else
          str4 = string.Join(".", new string[2]
          {
            str3,
            str2
          });
        string str5 = str4;
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str5);
        codeElementIdentity.Add("Parameters", string.Empty);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitMethodDeclaration(MethodDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString() + CSharpSyntaxNodeExtensions.PropertyVisitor.GetGenericParameters(node.TypeParameterList);
        string str2 = node.ExplicitInterfaceSpecifier != null ? string.Format("({0}.{1})", (object) CSharpSyntaxNodeExtensions.PropertyVisitor.RemoveTrivia<NameSyntax>(node.ExplicitInterfaceSpecifier.Name).ToString(), (object) str1) : str1;
        string str3;
        string str4;
        if (!this.parentPropertyBag.TryGetValue("Name", out str3))
          str4 = str2;
        else
          str4 = string.Join(".", new string[2]
          {
            str3,
            str2
          });
        string str5 = str4;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string str6 = string.Join(",", node.ParameterList.Parameters.Select<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName ?? (CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName = new Func<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.GetParameterTypeName))));
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str5);
        codeElementIdentity.Add("Parameters", str6);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitOperatorDeclaration(OperatorDeclarationSyntax node)
      {
        string str1 = node.OperatorToken.ToString();
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string str5 = string.Join(",", node.ParameterList.Parameters.Select<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName ?? (CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName = new Func<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.GetParameterTypeName))));
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        codeElementIdentity.Add("Parameters", str5);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitIndexerDeclaration(IndexerDeclarationSyntax node)
      {
        string str1 = node.ExplicitInterfaceSpecifier != null ? string.Format("({0}.{1})", (object) CSharpSyntaxNodeExtensions.PropertyVisitor.RemoveTrivia<NameSyntax>(node.ExplicitInterfaceSpecifier.Name).ToString(), (object) "[]") : "[]";
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string str5 = string.Join(",", node.ParameterList.Parameters.Select<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName ?? (CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName = new Func<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.GetParameterTypeName))));
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        codeElementIdentity.Add("Parameters", str5);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitConversionOperatorDeclaration(
        ConversionOperatorDeclarationSyntax node)
      {
        string str1 = CSharpSyntaxNodeExtensions.PropertyVisitor.RemoveTrivia<TypeSyntax>(node.Type).ToString();
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string str5 = string.Join(",", node.ParameterList.Parameters.Select<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName ?? (CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName = new Func<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.GetParameterTypeName))));
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        codeElementIdentity.Add("Parameters", str5);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitConstructorDeclaration(
        ConstructorDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString();
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string str5 = string.Join(",", node.ParameterList.Parameters.Select<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName ?? (CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName = new Func<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.GetParameterTypeName))));
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        codeElementIdentity.Add("Parameters", str5);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitDestructorDeclaration(
        DestructorDeclarationSyntax node)
      {
        string str1 = "~" + node.Identifier.ToString();
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string str5 = string.Join(",", node.ParameterList.Parameters.Select<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName ?? (CSharpSyntaxNodeExtensions.PropertyVisitor.\u003C\u003EO.\u003C0\u003E__GetParameterTypeName = new Func<ParameterSyntax, string>(CSharpSyntaxNodeExtensions.PropertyVisitor.GetParameterTypeName))));
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        codeElementIdentity.Add("Parameters", str5);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitDelegateDeclaration(DelegateDeclarationSyntax node)
      {
        string str1 = node.Identifier.ToString() + CSharpSyntaxNodeExtensions.PropertyVisitor.GetGenericParameters(node.TypeParameterList);
        string str2;
        string str3;
        if (!this.parentPropertyBag.TryGetValue("Name", out str2))
          str3 = str1;
        else
          str3 = string.Join(".", new string[2]
          {
            str2,
            str1
          });
        string str4 = str3;
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("C#");
        codeElementIdentity.Add("Name", str4);
        return codeElementIdentity;
      }

      private static TSyntaxNode RemoveTrivia<TSyntaxNode>(TSyntaxNode node) where TSyntaxNode : SyntaxNode => node.ReplaceTrivia<TSyntaxNode>(node.DescendantTrivia((Func<SyntaxNode, bool>) null, false), (Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia>) ((originalTrivia, replacementTrivia) => new SyntaxTrivia()));

      private static string GetGenericParameters(TypeParameterListSyntax typeParameters) => typeParameters == null || !typeParameters.Parameters.Any() ? string.Empty : "<" + typeParameters.Parameters.Count.ToString() + ">";

      private static string GetParameterTypeName(ParameterSyntax param) => param.Identifier.IsKind(SyntaxKind.ArgListKeyword) ? param.Identifier.ToString() : CSharpSyntaxNodeExtensions.PropertyVisitor.RemoveTrivia<TypeSyntax>(param.Type).ToString();
    }
  }
}
