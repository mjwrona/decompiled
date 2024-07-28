// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Json.JsonUtility
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.SignalR.Json
{
  public static class JsonUtility
  {
    private const int DefaultMaxDepth = 20;
    private static readonly string[] _jsKeywords = new string[45]
    {
      "break",
      "do",
      "instanceof",
      "typeof",
      "case",
      "else",
      "new",
      "var",
      "catch",
      "finally",
      "return",
      "void",
      "continue",
      "for",
      "switch",
      "while",
      "debugger",
      "function",
      "this",
      "with",
      "default",
      "if",
      "throw",
      "delete",
      "in",
      "try",
      "class",
      "enum",
      "extends",
      "super",
      "const",
      "export",
      "import",
      "implements",
      "let",
      "private",
      "public",
      "yield",
      "interface",
      "package",
      "protected",
      "static",
      "NaN",
      "undefined",
      "Infinity"
    };

    public static string CamelCase(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      return string.Join(".", ((IEnumerable<string>) name.Split('.')).Select<string, string>((Func<string, string>) (n => char.ToLower(n[0], CultureInfo.InvariantCulture).ToString() + n.Substring(1))));
    }

    public static string JsonMimeType => "application/json; charset=UTF-8";

    public static string JavaScriptMimeType => "application/javascript; charset=UTF-8";

    public static string CreateJsonpCallback(string callback, string payload)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!JsonUtility.IsValidJavaScriptCallback(callback))
        throw new InvalidOperationException();
      stringBuilder.AppendFormat("{0}(", (object) callback).Append(payload).Append(");");
      return stringBuilder.ToString();
    }

    public static JsonSerializerSettings CreateDefaultSerializerSettings() => new JsonSerializerSettings()
    {
      MaxDepth = new int?(20)
    };

    public static JsonSerializer CreateDefaultSerializer() => JsonSerializer.Create(JsonUtility.CreateDefaultSerializerSettings());

    internal static bool IsValidJavaScriptCallback(string callback)
    {
      if (string.IsNullOrWhiteSpace(callback))
        return false;
      string str = callback;
      char[] chArray = new char[1]{ '.' };
      foreach (string name in str.Split(chArray))
      {
        if (!JsonUtility.IsValidJavaScriptFunctionName(name))
          return false;
      }
      return true;
    }

    internal static bool IsValidJavaScriptFunctionName(string name)
    {
      if (string.IsNullOrWhiteSpace(name) || JsonUtility.IsJavaScriptReservedWord(name) || !JsonUtility.IsValidJavaScriptIdentifierStartChar(name[0]))
        return false;
      for (int index = 1; index < name.Length; ++index)
      {
        if (!JsonUtility.IsValidJavaScriptIdenfitierNonStartChar(name[index]))
          return false;
      }
      return true;
    }

    internal static bool TryRejectJSONPRequest(ConnectionConfiguration config, IOwinContext context)
    {
      if (config.EnableJSONP || string.IsNullOrEmpty(context.Request.Query.Get("callback")))
        return false;
      context.Response.StatusCode = 403;
      context.Response.ReasonPhrase = Microsoft.AspNet.SignalR.Resources.Forbidden_JSONPDisabled;
      return true;
    }

    private static bool IsValidJavaScriptIdentifierStartChar(char startChar) => char.IsLetter(startChar) || startChar == '$' || startChar == '_';

    private static bool IsValidJavaScriptIdenfitierNonStartChar(char identifierChar) => char.IsLetterOrDigit(identifierChar) || identifierChar == '$' || identifierChar == '_';

    private static bool IsJavaScriptReservedWord(string word) => ((IEnumerable<string>) JsonUtility._jsKeywords).Contains<string>(word);
  }
}
