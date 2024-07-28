// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.GlobalScope
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  public sealed class GlobalScope : ActivationObject
  {
    private static Regex s_blanketPrefixes = new Regex("^(?:ms|MS|o|webkit|moz|Gecko|HTML)(?:[A-Z][a-z0-9]*)+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private HashSet<string> m_globalProperties;
    private HashSet<string> m_globalFunctions;
    private HashSet<string> m_assumedGlobals;
    private HashSet<UndefinedReference> m_undefined;

    public ICollection<UndefinedReference> UndefinedReferences => (ICollection<UndefinedReference>) this.m_undefined;

    internal GlobalScope(CodeSettings settings)
      : base((ActivationObject) null, settings)
    {
      this.ScopeType = ScopeType.Global;
      this.m_globalProperties = new HashSet<string>()
      {
        "DOMParser",
        "Image",
        "Infinity",
        "JSON",
        "Math",
        "NaN",
        "System",
        "Windows",
        "WinJS",
        "XMLHttpRequest",
        "applicationCache",
        "clientInformation",
        "clipboardData",
        "closed",
        "console",
        "defaultStatus",
        "devicePixelRatio",
        "document",
        "event",
        "external",
        "frameElement",
        "frames",
        "history",
        "indexedDB",
        "innerHeight",
        "innerWidth",
        "length",
        "localStorage",
        "location",
        "name",
        "navigator",
        "offscreenBuffering",
        "opener",
        "outerHeight",
        "outerWidth",
        "pageXOffset",
        "pageYOffset",
        "parent",
        "screen",
        "screenLeft",
        "screenTop",
        "screenX",
        "screenY",
        "self",
        "sessionStorage",
        "status",
        "top",
        "undefined",
        "window"
      };
      this.m_globalFunctions = new HashSet<string>()
      {
        "ActiveXObject",
        "Array",
        "ArrayBuffer",
        "ArrayBufferView",
        "Boolean",
        "DataView",
        "Date",
        "Debug",
        "Error",
        "EvalError",
        "EventSource",
        "File",
        "FileList",
        "FileReader",
        "Float32Array",
        "Float64Array",
        "Function",
        "Int16Array",
        "Int32Array",
        "Int8Array",
        "Iterator",
        "Map",
        "Node",
        "NodeFilter",
        "NodeIterator",
        "NodeList",
        "NodeSelector",
        "Number",
        "Object",
        "Proxy",
        "RangeError",
        "ReferenceError",
        "RegExp",
        "Set",
        "SharedWorker",
        "String",
        "SyntaxError",
        "TypeError",
        "Uint8Array",
        "Uint8ClampedArray",
        "Uint16Array",
        "Uint32Array",
        "URIError",
        "URL",
        "WeakMap",
        "WebSocket",
        "Worker",
        "addEventListener",
        "alert",
        "attachEvent",
        "blur",
        "cancelAnimationFrame",
        "captureEvents",
        "clearImmediate",
        "clearInterval",
        "clearTimeout",
        "close",
        "confirm",
        "createPopup",
        "decodeURI",
        "decodeURIComponent",
        "detachEvent",
        "dispatchEvent",
        "encodeURI",
        "encodeURIComponent",
        "escape",
        "eval",
        "execScript",
        "focus",
        "getComputedStyle",
        "getSelection",
        "importScripts",
        "isFinite",
        "isNaN",
        "matchMedia",
        "moveBy",
        "moveTo",
        "navigate",
        "open",
        "parseFloat",
        "parseInt",
        "postMessage",
        "prompt",
        "releaseEvents",
        "removeEventListener",
        "requestAnimationFrame",
        "resizeBy",
        "resizeTo",
        "scroll",
        "scrollBy",
        "scrollTo",
        "setActive",
        "setImmediate",
        "setInterval",
        "setTimeout",
        "showModalDialog",
        "showModelessDialog",
        "styleMedia",
        "unescape"
      };
    }

    public void AddUndefinedReference(UndefinedReference exception)
    {
      if (this.m_undefined == null)
        this.m_undefined = new HashSet<UndefinedReference>();
      this.m_undefined.Add(exception);
    }

    internal void SetAssumedGlobals(CodeSettings settings)
    {
      if (settings != null)
      {
        this.m_assumedGlobals = settings.KnownGlobalCollection == null ? new HashSet<string>() : new HashSet<string>(settings.KnownGlobalCollection);
        foreach (string debugLookup in settings.DebugLookupCollection)
          this.m_assumedGlobals.Add(debugLookup.SubstringUpToFirst('.'));
        foreach (ResourceStrings resourceString in (IEnumerable<ResourceStrings>) settings.ResourceStrings)
        {
          if (!resourceString.Name.IsNullOrWhiteSpace())
            this.m_assumedGlobals.Add(resourceString.Name.SubstringUpToFirst('.'));
        }
      }
      else
        this.m_assumedGlobals = new HashSet<string>();
    }

    internal override void AnalyzeScope()
    {
      this.ManualRenameFields();
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.ChildScopes)
      {
        if (!childScope.Existing)
          childScope.AnalyzeScope();
      }
    }

    internal override void AutoRenameFields()
    {
      foreach (ActivationObject childScope in (IEnumerable<ActivationObject>) this.ChildScopes)
      {
        if (!childScope.Existing)
          childScope.AutoRenameFields();
      }
    }

    public override JSVariableField this[string name]
    {
      get
      {
        JSVariableField variableField = ((base[name] ?? this.ResolveFromCollection(name, this.m_globalProperties, FieldType.Predefined, false)) ?? this.ResolveFromCollection(name, this.m_globalFunctions, FieldType.Predefined, true)) ?? this.ResolveFromCollection(name, this.m_assumedGlobals, FieldType.Global, false);
        if (variableField == null && GlobalScope.s_blanketPrefixes.IsMatch(name))
        {
          variableField = new JSVariableField(FieldType.Predefined, name, FieldAttributes.PrivateScope, (object) null);
          this.AddField(variableField);
        }
        return variableField;
      }
    }

    private JSVariableField ResolveFromCollection(
      string name,
      HashSet<string> collection,
      FieldType fieldType,
      bool isFunction)
    {
      if (!collection.Contains(name))
        return (JSVariableField) null;
      return this.AddField(new JSVariableField(fieldType, name, FieldAttributes.PrivateScope, (object) null)
      {
        IsFunction = isFunction
      });
    }

    public override void DeclareScope()
    {
      this.DefineLexicalDeclarations();
      this.DefineVarDeclarations();
    }

    public override JSVariableField CreateField(
      string name,
      object value,
      FieldAttributes attributes)
    {
      return new JSVariableField(FieldType.Global, name, attributes, value);
    }

    public override JSVariableField CreateField(JSVariableField outerField) => throw new NotImplementedException();
  }
}
