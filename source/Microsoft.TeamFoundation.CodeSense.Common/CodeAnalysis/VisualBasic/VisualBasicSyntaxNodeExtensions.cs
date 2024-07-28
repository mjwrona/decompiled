// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNodeExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 77D96756-A6EC-4CC5-958E-440F0412CE7F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Common.dll

using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.TeamFoundation.CodeSense.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
  public static class VisualBasicSyntaxNodeExtensions
  {
    private static ConditionalWeakTable<VisualBasicSyntaxNode, CodeElementIdentity> identityCache = new ConditionalWeakTable<VisualBasicSyntaxNode, CodeElementIdentity>();

    public static CodeElementIdentity GetCodeElementIdentity(this VisualBasicSyntaxNode node) => VisualBasicSyntaxNodeExtensions.identityCache.GetValue(node, VisualBasicSyntaxNodeExtensions.\u003C\u003EO.\u003C0\u003E__Create ?? (VisualBasicSyntaxNodeExtensions.\u003C\u003EO.\u003C0\u003E__Create = new ConditionalWeakTable<VisualBasicSyntaxNode, CodeElementIdentity>.CreateValueCallback(VisualBasicSyntaxNodeExtensions.Create)));

    private static CodeElementIdentity Create(VisualBasicSyntaxNode node) => ((SyntaxNode) node).Parent == null ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor(new CodeElementIdentity("Visual Basic"))) : node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor(((VisualBasicSyntaxNode) ((SyntaxNode) node).Parent).GetCodeElementIdentity()));

    public class PropertyVisitor : VisualBasicSyntaxVisitor<CodeElementIdentity>
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
          return new CodeElementIdentity("Visual Basic");
        CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
        codeElementIdentity.Add("Name", str);
        return codeElementIdentity;
      }

      public override CodeElementIdentity VisitNamespaceBlock(NamespaceBlockSyntax node) => node.NamespaceStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitModuleBlock(ModuleBlockSyntax node) => node.BlockStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitClassBlock(ClassBlockSyntax node) => node.BlockStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitEnumBlock(EnumBlockSyntax node) => node.EnumStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitInterfaceBlock(InterfaceBlockSyntax node) => node.BlockStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitStructureBlock(StructureBlockSyntax node) => node.BlockStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitPropertyBlock(PropertyBlockSyntax node) => node.PropertyStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitMethodBlock(MethodBlockSyntax node) => node.BlockStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitConstructorBlock(ConstructorBlockSyntax node) => node.BlockStatement.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      public override CodeElementIdentity VisitNamespaceStatement(NamespaceStatementSyntax node) => !(((SyntaxNode) node).Parent is NamespaceBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitModuleStatement(ModuleStatementSyntax node) => !(((SyntaxNode) node).Parent is ModuleBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitClassStatement(ClassStatementSyntax node) => !(((SyntaxNode) node).Parent is ClassBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitEnumStatement(EnumStatementSyntax node) => !(((SyntaxNode) node).Parent is EnumBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitInterfaceStatement(InterfaceStatementSyntax node) => !(((SyntaxNode) node).Parent is InterfaceBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitStructureStatement(StructureStatementSyntax node) => !(((SyntaxNode) node).Parent is StructureBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitDeclareStatement(DeclareStatementSyntax node) => !(((SyntaxNode) node).Parent is MethodBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitMethodStatement(MethodStatementSyntax node) => !(((SyntaxNode) node).Parent is MethodBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitOperatorStatement(OperatorStatementSyntax node) => !(((SyntaxNode) node).Parent is MethodBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitSubNewStatement(SubNewStatementSyntax node) => !(((SyntaxNode) node).Parent is ConstructorBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitPropertyStatement(PropertyStatementSyntax node) => !(((SyntaxNode) node).Parent is PropertyBlockSyntax) ? node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag)) : (CodeElementIdentity) null;

      public override CodeElementIdentity VisitDelegateStatement(DelegateStatementSyntax node) => node.Accept<CodeElementIdentity>((VisualBasicSyntaxVisitor<CodeElementIdentity>) new VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor(this.parentPropertyBag));

      private class StatementSyntaxVisitor : VisualBasicSyntaxVisitor<CodeElementIdentity>
      {
        private readonly CodeElementIdentity parentPropertyBag;

        public StatementSyntaxVisitor(CodeElementIdentity parentPropertyBag) => this.parentPropertyBag = parentPropertyBag;

        public override CodeElementIdentity VisitNamespaceStatement(NamespaceStatementSyntax node)
        {
          string str1 = VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.RemoveTrivia<NameSyntax>(node.Name).ToString();
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
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitModuleStatement(ModuleStatementSyntax node)
        {
          string str1 = node.Identifier.ToString() + VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetGenericParameters(node.TypeParameterList);
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
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitClassStatement(ClassStatementSyntax node)
        {
          string str1 = node.Identifier.ToString() + VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetGenericParameters(node.TypeParameterList);
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
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitEnumStatement(EnumStatementSyntax node)
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
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitInterfaceStatement(InterfaceStatementSyntax node)
        {
          string str1 = node.Identifier.ToString() + VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetGenericParameters(node.TypeParameterList);
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
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitStructureStatement(StructureStatementSyntax node)
        {
          string str1 = node.Identifier.ToString() + VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetGenericParameters(node.TypeParameterList);
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
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitDeclareStatement(DeclareStatementSyntax node)
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
          string parameterListString = VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetParameterListString(node.ParameterList);
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          codeElementIdentity.Add("Parameters", parameterListString);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitMethodStatement(MethodStatementSyntax node)
        {
          string str1 = node.Identifier.ToString() + VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetGenericParameters(node.TypeParameterList);
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
          string parameterListString = VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetParameterListString(node.ParameterList);
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          codeElementIdentity.Add("Parameters", parameterListString);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitOperatorStatement(OperatorStatementSyntax node)
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
          string parameterListString = VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetParameterListString(node.ParameterList);
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          codeElementIdentity.Add("Parameters", parameterListString);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitSubNewStatement(SubNewStatementSyntax node)
        {
          string str1 = node.NewKeyword.ToString();
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
          string parameterListString = VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetParameterListString(node.ParameterList);
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          codeElementIdentity.Add("Parameters", parameterListString);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitPropertyStatement(PropertyStatementSyntax node)
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
          string parameterListString = VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetParameterListString(node.ParameterList);
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          codeElementIdentity.Add("Parameters", parameterListString);
          return codeElementIdentity;
        }

        public override CodeElementIdentity VisitDelegateStatement(DelegateStatementSyntax node)
        {
          string str1 = node.Identifier.ToString() + VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.GetGenericParameters(node.TypeParameterList);
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
          CodeElementIdentity codeElementIdentity = new CodeElementIdentity("Visual Basic");
          codeElementIdentity.Add("Name", str4);
          return codeElementIdentity;
        }

        private static TSyntaxNode RemoveTrivia<TSyntaxNode>(TSyntaxNode node) where TSyntaxNode : SyntaxNode => node.ReplaceTrivia<TSyntaxNode>(node.DescendantTrivia((Func<SyntaxNode, bool>) null, false), (Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia>) ((originalTrivia, replacementTrivia) => new SyntaxTrivia()));

        private static string GetGenericParameters(TypeParameterListSyntax typeParameters) => typeParameters == null || !typeParameters.Parameters.Any() ? string.Empty : "<" + typeParameters.Parameters.Count.ToString() + ">";

        private static string GetParameterListString(ParameterListSyntax parameterList) => parameterList == null || !parameterList.Parameters.Any() ? string.Empty : string.Join(",", parameterList.Parameters.Where<ParameterSyntax>((Func<ParameterSyntax, bool>) (parameter => parameter.AsClause != null && parameter.AsClause.Type != null)).Select<ParameterSyntax, string>((Func<ParameterSyntax, string>) (parameter => VisualBasicSyntaxNodeExtensions.PropertyVisitor.StatementSyntaxVisitor.RemoveTrivia<TypeSyntax>(parameter.AsClause.Type).ToString())));
      }
    }
  }
}
