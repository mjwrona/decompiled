// Decompiled with JetBrains decompiler
// Type: Tomlyn.Toml
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Tomlyn.Model;
using Tomlyn.Parsing;
using Tomlyn.Syntax;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn
{
  public static class Toml
  {
    public static readonly string Version = ((AssemblyFileVersionAttribute) typeof (Toml).Assembly.GetCustomAttributes(typeof (AssemblyFileVersionAttribute), false)[0]).Version;

    public static DocumentSyntax Parse(string text, string? sourcePath = null, TomlParserOptions options = TomlParserOptions.ParseAndValidate)
    {
      DocumentSyntax doc = new Parser<StringSourceView>((ITokenProvider<StringSourceView>) new Lexer<StringSourceView, StringCharacterIterator>(new StringSourceView(text, sourcePath ?? string.Empty))).Run();
      if (!doc.HasErrors && options == TomlParserOptions.ParseAndValidate)
        Toml.Validate(doc);
      return doc;
    }

    public static DocumentSyntax Parse(
      byte[] utf8Bytes,
      string? sourcePath = null,
      TomlParserOptions options = TomlParserOptions.ParseAndValidate)
    {
      DocumentSyntax doc = new Parser<StringUtf8SourceView>((ITokenProvider<StringUtf8SourceView>) new Lexer<StringUtf8SourceView, StringCharacterUtf8Iterator>(new StringUtf8SourceView(utf8Bytes, sourcePath ?? string.Empty))).Run();
      if (!doc.HasErrors && options == TomlParserOptions.ParseAndValidate)
        Toml.Validate(doc);
      return doc;
    }

    public static DocumentSyntax Validate(DocumentSyntax doc)
    {
      if (doc == null)
        throw new ArgumentNullException(nameof (doc));
      if (doc.HasErrors)
        return doc;
      new SyntaxValidator(doc.Diagnostics).Visit(doc);
      return doc;
    }

    public static string FromModel(object model, TomlModelOptions? options = null)
    {
      string modelAsToml;
      DiagnosticsBag diagnostics;
      if (Toml.TryFromModel(model, out modelAsToml, out diagnostics, options))
        return modelAsToml;
      throw new TomlException(diagnostics);
    }

    public static bool TryFromModel(
      object model,
      [NotNullWhen(true)] out string? modelAsToml,
      out DiagnosticsBag diagnostics,
      TomlModelOptions? options = null)
    {
      modelAsToml = (string) null;
      StringWriter writer = new StringWriter();
      if (!Toml.TryFromModel(model, (TextWriter) writer, out diagnostics, options))
        return false;
      modelAsToml = writer.ToString();
      return true;
    }

    public static bool TryFromModel(
      object model,
      TextWriter writer,
      out DiagnosticsBag diagnostics,
      TomlModelOptions? options = null)
    {
      DynamicModelWriteContext context = new DynamicModelWriteContext(options ?? new TomlModelOptions(), writer);
      new ModelToTomlTransform(model, context).Run();
      diagnostics = context.Diagnostics;
      return !context.Diagnostics.HasErrors;
    }

    public static TomlTable ToModel(string text, string? sourcePath = null, TomlModelOptions? options = null) => Toml.ToModel<TomlTable>(text, sourcePath, options);

    public static T ToModel<T>(string text, string? sourcePath = null, TomlModelOptions? options = null) where T : class, new()
    {
      DocumentSyntax syntax = Toml.Parse(text, sourcePath);
      return !syntax.HasErrors ? syntax.ToModel<T>(options) : throw new TomlException(syntax.Diagnostics);
    }

    public static bool TryToModel<T>(
      string text,
      [NotNullWhen(true)] out T? model,
      [NotNullWhen(false)] out DiagnosticsBag? diagnostics,
      string? sourcePath = null,
      TomlModelOptions? options = null)
      where T : class, new()
    {
      DocumentSyntax syntax = Toml.Parse(text, sourcePath);
      if (!syntax.HasErrors)
        return syntax.TryToModel<T>(out model, out diagnostics, options);
      diagnostics = syntax.Diagnostics;
      model = default (T);
      return false;
    }

    public static TomlTable ToModel(this DocumentSyntax syntax) => syntax != null ? TomlTable.From(syntax) : throw new ArgumentNullException(nameof (syntax));

    public static T ToModel<T>(this DocumentSyntax syntax, TomlModelOptions? options = null) where T : class, new()
    {
      T model;
      DiagnosticsBag diagnostics;
      if (!syntax.TryToModel<T>(out model, out diagnostics, options))
        throw new TomlException(diagnostics);
      return model;
    }

    public static bool TryToModel<T>(
      this DocumentSyntax syntax,
      [NotNullWhen(true)] out T? model,
      out DiagnosticsBag diagnostics,
      TomlModelOptions? options = null)
      where T : class, new()
    {
      model = new T();
      DynamicModelReadContext context = new DynamicModelReadContext(options ?? new TomlModelOptions());
      new SyntaxToModelTransform(context, (object) model).Visit(syntax);
      diagnostics = context.Diagnostics;
      return !context.Diagnostics.HasErrors;
    }
  }
}
