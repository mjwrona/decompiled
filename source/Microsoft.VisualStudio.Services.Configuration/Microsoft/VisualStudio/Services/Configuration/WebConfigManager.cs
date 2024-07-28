// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.WebConfigManager
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class WebConfigManager
  {
    private readonly XDocument m_xmlDoc;
    private readonly WebConfigManager.ConfigurationElement m_rootConfig;

    public WebConfigManager(string text)
    {
      this.m_xmlDoc = XDocument.Parse(text);
      this.m_rootConfig = new WebConfigManager.ConfigurationElement(this.m_xmlDoc.Root);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      using (Utf8StringWriter utf8StringWriter = new Utf8StringWriter(sb))
      {
        XDocument xdocument = new XDocument(this.m_xmlDoc);
        xdocument.Root.ReplaceWith((object) this.m_rootConfig.GetXml());
        xdocument.Save((TextWriter) utf8StringWriter);
      }
      return sb.ToString();
    }

    public WebConfigManager.ConfigurationElement.ConfigSectionsElement ConfigSections => this.m_rootConfig.ConfigSections;

    public WebConfigManager.AppSettingsElement AppSettings => this.m_rootConfig.AppSettings;

    public WebConfigManager.SystemWebElement SystemWeb => this.m_rootConfig.SystemWeb;

    public WebConfigManager.SystemWebServerElement SystemWebServer => this.m_rootConfig.SystemWebServer;

    public WebConfigManager.SystemWebExtensionsElement SystemWebExtensions => this.m_rootConfig.SystemWebExtensions;

    public WebConfigManager.LocationsElement Locations => this.m_rootConfig.Locations;

    public WebConfigManager.RuntimeElement Runtime => this.m_rootConfig.Runtime;

    public WebConfigManager.UriElement Uri => this.m_rootConfig.Uri;

    public void Include(string fileName) => this.m_xmlDoc.Root.Add((object) XElement.Load(fileName));

    public class ConfigurationElement : AppConfigElement
    {
      internal ConfigurationElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.ConfigurationElement.ConfigSectionsElement ConfigSections => new WebConfigManager.ConfigurationElement.ConfigSectionsElement(this.GetOrCreateElement("configSections"));

      public WebConfigManager.AppSettingsElement AppSettings => new WebConfigManager.AppSettingsElement(this.GetOrCreateElement("appSettings"));

      public WebConfigManager.SystemWebElement SystemWeb => new WebConfigManager.SystemWebElement(this.GetOrCreateElement("system.web"));

      public WebConfigManager.SystemWebServerElement SystemWebServer => new WebConfigManager.SystemWebServerElement(this.GetOrCreateElement("system.webServer"));

      public WebConfigManager.SystemWebExtensionsElement SystemWebExtensions => new WebConfigManager.SystemWebExtensionsElement(this.GetOrCreateElement("system.web.extensions"));

      public WebConfigManager.LocationsElement Locations => new WebConfigManager.LocationsElement(this.Root);

      public WebConfigManager.RuntimeElement Runtime => new WebConfigManager.RuntimeElement(this.GetOrCreateElement("runtime"));

      public WebConfigManager.UriElement Uri => new WebConfigManager.UriElement(this.GetOrCreateElement("uri"));

      public class ConfigSectionsElement : AppConfigElement
      {
        internal ConfigSectionsElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.ConfigurationElement.ConfigSectionsElement.SectionGroupElement Groups(
          string groupName,
          string typeName)
        {
          WebConfigManager.ConfigurationElement.ConfigSectionsElement.SectionGroupElement sectionGroupElement = new WebConfigManager.ConfigurationElement.ConfigSectionsElement.SectionGroupElement(this.GetOrCreateElement("sectionGroup", "name", groupName));
          sectionGroupElement["type"] = typeName;
          return sectionGroupElement;
        }

        public WebConfigManager.ConfigurationElement.ConfigSectionsElement Add(
          string sectionName,
          string typeName,
          string fileName)
        {
          XElement content1 = this.MakeElement("section");
          content1.Add((object) this.MakeAttribute("name", sectionName), (object) this.MakeAttribute("type", typeName));
          this.Root.Add((object) content1);
          XElement content2 = this.MakeElement(sectionName);
          content2.Add((object) this.MakeAttribute("configSource", fileName));
          this.Root.Parent.Add((object) content2);
          return this;
        }

        public class SectionGroupElement : AppConfigElement
        {
          internal SectionGroupElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.ConfigurationElement.ConfigSectionsElement.SectionGroupElement Add(
            string sectionName,
            string typeName,
            string fileName,
            bool? requirePermission = null)
          {
            XElement content1 = this.MakeElement("section");
            content1.Add((object) this.MakeAttribute("name", sectionName), (object) this.MakeAttribute("type", typeName));
            if (requirePermission.HasValue)
              content1.Add((object) this.MakeAttribute(nameof (requirePermission), requirePermission.ToString().ToLowerInvariant()));
            this.Root.Add((object) content1);
            Stack<string> stringStack = new Stack<string>();
            XElement xelement1;
            for (xelement1 = this.Root; xelement1.Name.LocalName == this.Root.Name.LocalName; xelement1 = xelement1.Parent)
              stringStack.Push(xelement1.Attribute(this.MakeName("name")).Value);
            XElement xelement2 = xelement1.Parent;
            while (stringStack.Count > 0)
            {
              string name = stringStack.Pop();
              XElement content2 = xelement2.Element(this.MakeName(name));
              if (content2 == null)
              {
                content2 = this.MakeElement(name);
                xelement2.Add((object) content2);
              }
              xelement2 = content2;
            }
            XElement content3 = this.MakeElement(sectionName);
            content3.Add((object) this.MakeAttribute("configSource", fileName));
            xelement2.Add((object) content3);
            return this;
          }
        }
      }
    }

    public class AppSettingsElement : AppConfigElement
    {
      internal AppSettingsElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.AppSettingsElement Add(string key, string value, IPosition position = null)
      {
        XElement content = this.MakeElement("add");
        content.Add((object) this.MakeAttribute(nameof (key), key));
        content.Add((object) this.MakeAttribute(nameof (value), value));
        if (position == null)
          this.Root.Add((object) content);
        else
          position.Insert(this.Root, content, new Func<XElement, string, bool>(this.FindPosition));
        return this;
      }

      private bool FindPosition(XElement setting, string anchorSettingName) => setting.Name.ToString() == "add" && setting.Attribute(this.MakeName("key"))?.Value == anchorSettingName;
    }

    public class SystemWebElement : AppConfigElement
    {
      internal SystemWebElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.SystemWebElement.AuthorizationElement Authorization => new WebConfigManager.SystemWebElement.AuthorizationElement(this.GetOrCreateElement("authorization"));

      public WebConfigManager.SystemWebElement.CachingElement Caching => new WebConfigManager.SystemWebElement.CachingElement(this.GetOrCreateElement("caching"));

      public WebConfigManager.SystemWebElement.CompilationElement Compilation => new WebConfigManager.SystemWebElement.CompilationElement(this.GetOrCreateElement("compilation"));

      public WebConfigManager.SystemWebElement.HttpRuntimeElement HttpRuntime => new WebConfigManager.SystemWebElement.HttpRuntimeElement(this.GetOrCreateElement("httpRuntime"));

      public WebConfigManager.SystemWebElement.PagesElement Pages => new WebConfigManager.SystemWebElement.PagesElement(this.GetOrCreateElement("pages"));

      public WebConfigManager.SystemWebElement.WebServicesElement WebServices => new WebConfigManager.SystemWebElement.WebServicesElement(this.GetOrCreateElement("webServices"));

      public WebConfigManager.SystemWebElement.MachineKeyElement MachineKey => new WebConfigManager.SystemWebElement.MachineKeyElement(this.GetOrCreateElement("machineKey"));

      public class AuthorizationElement : AppConfigElement
      {
        internal AuthorizationElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebElement.AuthorizationElement.AuthorizationRuleElement Deny() => new WebConfigManager.SystemWebElement.AuthorizationElement.AuthorizationRuleElement(this.GetOrCreateElement("deny"));

        public class AuthorizationRuleElement : AppConfigElement
        {
          internal AuthorizationRuleElement(XElement root)
            : base(root)
          {
          }
        }
      }

      public class CachingElement : AppConfigElement
      {
        internal CachingElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebElement.CachingElement.OutputCacheSettingsElement Assemblies => new WebConfigManager.SystemWebElement.CachingElement.OutputCacheSettingsElement(this.GetOrCreateElement("outputCacheSettings"));

        public class OutputCacheSettingsElement : AppConfigElement
        {
          internal OutputCacheSettingsElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebElement.CachingElement.OutputCacheSettingsElement.OutputCacheProfilesElement OutputCacheProfiles => new WebConfigManager.SystemWebElement.CachingElement.OutputCacheSettingsElement.OutputCacheProfilesElement(this.GetOrCreateElement("outputCacheProfiles"));

          public class OutputCacheProfilesElement : AppConfigElement
          {
            internal OutputCacheProfilesElement(XElement root)
              : base(root)
            {
            }

            public WebConfigManager.SystemWebElement.CachingElement.OutputCacheSettingsElement.OutputCacheProfilesElement.OutputCacheProfileElement Item(
              string name)
            {
              return new WebConfigManager.SystemWebElement.CachingElement.OutputCacheSettingsElement.OutputCacheProfilesElement.OutputCacheProfileElement(this.GetOrCreateElement("add", nameof (name), name));
            }

            public class OutputCacheProfileElement : AppConfigElement
            {
              internal OutputCacheProfileElement(XElement root)
                : base(root)
              {
              }
            }
          }
        }
      }

      public class CompilationElement : AppConfigElement
      {
        internal CompilationElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebElement.CompilationElement.AssembliesElement Assemblies => new WebConfigManager.SystemWebElement.CompilationElement.AssembliesElement(this.GetOrCreateElement("assemblies"));

        public WebConfigManager.SystemWebElement.CompilationElement.BuildProvidersElement BuildProviders => new WebConfigManager.SystemWebElement.CompilationElement.BuildProvidersElement(this.GetOrCreateElement("buildProviders"));

        public class AssembliesElement : AppConfigElement
        {
          internal AssembliesElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebElement.CompilationElement.AssembliesElement Add(
            string assembly)
          {
            XElement content = this.MakeElement("add");
            content.Add((object) this.MakeAttribute(nameof (assembly), assembly));
            this.Root.Add((object) content);
            return this;
          }
        }

        public class BuildProvidersElement : AppConfigElement
        {
          internal BuildProvidersElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebElement.CompilationElement.BuildProvidersElement Remove(
            string extension)
          {
            XElement content = this.MakeElement("remove");
            content.Add((object) this.MakeAttribute(nameof (extension), "." + extension));
            this.Root.Add((object) content);
            return this;
          }
        }
      }

      public class HttpRuntimeElement : AppConfigElement
      {
        internal HttpRuntimeElement(XElement root)
          : base(root)
        {
        }
      }

      public class PagesElement : AppConfigElement
      {
        internal PagesElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebElement.PagesElement.NamespacesElement Namespaces => new WebConfigManager.SystemWebElement.PagesElement.NamespacesElement(this.GetOrCreateElement("namespaces"));

        public class NamespacesElement : AppConfigElement
        {
          internal NamespacesElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebElement.PagesElement.NamespacesElement Add(
            string @namespace)
          {
            XElement content = this.MakeElement("add");
            content.Add((object) this.MakeAttribute(nameof (@namespace), @namespace));
            this.Root.Add((object) content);
            return this;
          }
        }
      }

      public class WebServicesElement : AppConfigElement
      {
        internal WebServicesElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebElement.WebServicesElement.SoapExtensionTypesElement SoapExtensionTypes => new WebConfigManager.SystemWebElement.WebServicesElement.SoapExtensionTypesElement(this.GetOrCreateElement("soapExtensionTypes"));

        public class SoapExtensionTypesElement : AppConfigElement
        {
          internal SoapExtensionTypesElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebElement.WebServicesElement.SoapExtensionTypesElement Add(
            string type,
            string priority,
            string group)
          {
            XElement content = this.MakeElement("add");
            content.Add((object) this.MakeAttribute(nameof (type), type));
            content.Add((object) this.MakeAttribute(nameof (priority), priority));
            content.Add((object) this.MakeAttribute(nameof (group), group));
            this.Root.Add((object) content);
            return this;
          }
        }
      }

      public class MachineKeyElement : AppConfigElement
      {
        internal MachineKeyElement(XElement root)
          : base(root)
        {
        }
      }
    }

    public class SystemWebServerElement : AppConfigElement
    {
      internal SystemWebServerElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.SystemWebServerElement.ModulesElement Modules => new WebConfigManager.SystemWebServerElement.ModulesElement(this.GetOrCreateElement("modules"));

      public WebConfigManager.SystemWebServerElement.HandlersElement Handlers => new WebConfigManager.SystemWebServerElement.HandlersElement(this.GetOrCreateElement("handlers"));

      public WebConfigManager.SystemWebServerElement.RewriteElement Rewrite => new WebConfigManager.SystemWebServerElement.RewriteElement(this.GetOrCreateElement("rewrite"));

      public WebConfigManager.SystemWebServerElement.SecurityElement Security => new WebConfigManager.SystemWebServerElement.SecurityElement(this.GetOrCreateElement("security"));

      public WebConfigManager.SystemWebServerElement.ServerRuntimeElement ServerRuntime => new WebConfigManager.SystemWebServerElement.ServerRuntimeElement(this.GetOrCreateElement("serverRuntime"));

      public class ModulesElement : AppConfigElement
      {
        internal ModulesElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebServerElement.ModulesElement Add(
          string name,
          string type,
          string precondition = null,
          IPosition position = null,
          ICondition condition = null)
        {
          XElement content = this.MakeElement("add");
          content.Add((object) this.MakeAttribute(nameof (name), name));
          content.Add((object) this.MakeAttribute(nameof (type), type));
          if (precondition != null)
            content.Add((object) this.MakeAttribute("preCondition", precondition));
          if (position == null)
            this.Root.Add((object) content);
          else
            position.Insert(this.Root, content, new Func<XElement, string, bool>(this.FindPosition));
          if (condition != null)
            content.AddAnnotation((object) condition);
          return this;
        }

        public WebConfigManager.SystemWebServerElement.ModulesElement Remove(string name)
        {
          XElement content = this.MakeElement("remove");
          content.Add((object) this.MakeAttribute(nameof (name), name));
          this.Root.Add((object) content);
          return this;
        }

        private bool FindPosition(XElement module, string anchorModuleName) => module.Name.ToString() == "add" && module.Attribute(this.MakeName("name"))?.Value == anchorModuleName;
      }

      public class HandlersElement : AppConfigElement
      {
        internal HandlersElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebServerElement.HandlersElement Add(
          string name,
          string verb,
          string path,
          string type,
          string precondition = null,
          IPosition position = null)
        {
          XElement content = this.MakeElement("add");
          content.Add((object) this.MakeAttribute(nameof (name), name));
          content.Add((object) this.MakeAttribute(nameof (verb), verb));
          content.Add((object) this.MakeAttribute(nameof (path), path));
          content.Add((object) this.MakeAttribute(nameof (type), type));
          if (precondition != null)
            content.Add((object) this.MakeAttribute("preCondition", precondition));
          if (position == null)
            this.Root.Add((object) content);
          else
            position.Insert(this.Root, content, new Func<XElement, string, bool>(this.FindAddPosition));
          return this;
        }

        public WebConfigManager.SystemWebServerElement.HandlersElement Remove(
          string name,
          IPosition position = null)
        {
          XElement content = this.MakeElement("remove");
          content.Add((object) this.MakeAttribute(nameof (name), name));
          if (position == null)
            this.Root.Add((object) content);
          else
            position.Insert(this.Root, content, new Func<XElement, string, bool>(this.FindRemovePosition));
          return this;
        }

        private bool FindAddPosition(XElement module, string anchorHandlerName) => module.Name.ToString() == "add" && module.Attribute(this.MakeName("name"))?.Value == anchorHandlerName;

        private bool FindRemovePosition(XElement module, string anchorHandlerName) => module.Name.ToString() == "remove" && module.Attribute(this.MakeName("name"))?.Value == anchorHandlerName;
      }

      public class RewriteElement : AppConfigElement
      {
        internal RewriteElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement Rules => new WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement(this.GetOrCreateElement("rules"));

        public class RulesElement : AppConfigElement
        {
          internal RulesElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement Item(
            string name)
          {
            return new WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement(this.GetOrCreateElement("rule", nameof (name), name));
          }

          public class RuleElement : AppConfigElement
          {
            internal RuleElement(XElement root)
              : base(root)
            {
            }

            public WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement.ActionElement Action => new WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement.ActionElement(this.GetOrCreateElement("action"));

            public WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement.ConditionsElement Conditions => new WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement.ConditionsElement(this.GetOrCreateElement("conditions"));

            public WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement.MatchElement Match => new WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement.MatchElement(this.GetOrCreateElement("match"));

            public class ActionElement : AppConfigElement
            {
              internal ActionElement(XElement root)
                : base(root)
              {
              }
            }

            public class ConditionsElement : AppConfigElement
            {
              internal ConditionsElement(XElement root)
                : base(root)
              {
              }

              public WebConfigManager.SystemWebServerElement.RewriteElement.RulesElement.RuleElement.ConditionsElement Add(
                string input,
                string pattern)
              {
                XElement content = this.MakeElement("add");
                content.Add((object) this.MakeAttribute(nameof (input), input));
                content.Add((object) this.MakeAttribute(nameof (pattern), pattern));
                this.Root.Add((object) content);
                return this;
              }
            }

            public class MatchElement : AppConfigElement
            {
              internal MatchElement(XElement root)
                : base(root)
              {
              }
            }
          }
        }
      }

      public class SecurityElement : AppConfigElement
      {
        internal SecurityElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement RequestFiltering => new WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement(this.GetOrCreateElement("requestFiltering"));

        public WebConfigManager.SystemWebServerElement.SecurityElement.AccessElement Access => new WebConfigManager.SystemWebServerElement.SecurityElement.AccessElement(this.GetOrCreateElement("access"));

        public class RequestFilteringElement : AppConfigElement
        {
          internal RequestFilteringElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement.HiddenSegmentsElement HiddenSegments => new WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement.HiddenSegmentsElement(this.GetOrCreateElement("hiddenSegments"));

          public WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement.FileExtensionsElement FileExtensions => new WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement.FileExtensionsElement(this.GetOrCreateElement("fileExtensions"));

          public WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement.RequestLimitsElement RequestLimits => new WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement.RequestLimitsElement(this.GetOrCreateElement("requestLimits"));

          public class HiddenSegmentsElement : AppConfigElement
          {
            internal HiddenSegmentsElement(XElement root)
              : base(root)
            {
            }

            public WebConfigManager.SystemWebServerElement.SecurityElement.RequestFilteringElement.HiddenSegmentsElement Add(
              string segment)
            {
              XElement content = this.MakeElement("add");
              content.Add((object) this.MakeAttribute(nameof (segment), segment));
              this.Root.Add((object) content);
              return this;
            }
          }

          public class FileExtensionsElement : AppConfigElement
          {
            internal FileExtensionsElement(XElement root)
              : base(root)
            {
            }
          }

          public class RequestLimitsElement : AppConfigElement
          {
            internal RequestLimitsElement(XElement root)
              : base(root)
            {
            }
          }
        }

        public class AccessElement : AppConfigElement
        {
          internal AccessElement(XElement root)
            : base(root)
          {
          }
        }
      }

      public class ServerRuntimeElement : AppConfigElement
      {
        internal ServerRuntimeElement(XElement root)
          : base(root)
        {
        }
      }
    }

    public class SystemWebExtensionsElement : AppConfigElement
    {
      internal SystemWebExtensionsElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.SystemWebExtensionsElement.ScriptingElement Scripting => new WebConfigManager.SystemWebExtensionsElement.ScriptingElement(this.GetOrCreateElement("scripting"));

      public class ScriptingElement : AppConfigElement
      {
        internal ScriptingElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.SystemWebExtensionsElement.ScriptingElement.WebServicesElement WebServices => new WebConfigManager.SystemWebExtensionsElement.ScriptingElement.WebServicesElement(this.GetOrCreateElement("webServices"));

        public class WebServicesElement : AppConfigElement
        {
          internal WebServicesElement(XElement root)
            : base(root)
          {
          }

          public WebConfigManager.SystemWebExtensionsElement.ScriptingElement.WebServicesElement.JsonSerializationElement JsonSerialization => new WebConfigManager.SystemWebExtensionsElement.ScriptingElement.WebServicesElement.JsonSerializationElement(this.GetOrCreateElement("jsonSerialization"));

          public class JsonSerializationElement : AppConfigElement
          {
            internal JsonSerializationElement(XElement root)
              : base(root)
            {
            }
          }
        }
      }
    }

    public class LocationsElement : AppConfigElement
    {
      internal LocationsElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.LocationElement Add(string path)
      {
        XElement xelement = this.MakeElement("location");
        this.Root.Add((object) xelement);
        WebConfigManager.LocationElement locationElement = new WebConfigManager.LocationElement(xelement);
        locationElement[nameof (path)] = path;
        return locationElement;
      }
    }

    public class LocationElement : AppConfigElement
    {
      internal LocationElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.SystemWebElement SystemWeb => new WebConfigManager.SystemWebElement(this.GetOrCreateElement("system.web"));

      public WebConfigManager.SystemWebServerElement SystemWebServer => new WebConfigManager.SystemWebServerElement(this.GetOrCreateElement("system.webServer"));
    }

    public class RuntimeElement : AppConfigElement
    {
      private static readonly XNamespace c_xmlns = XNamespace.Get("urn:schemas-microsoft-com:asm.v1");

      internal RuntimeElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.RuntimeElement.AppContextSwitchOverridesElement AppContextSwitchOverrides => new WebConfigManager.RuntimeElement.AppContextSwitchOverridesElement(this.GetOrCreateElement(nameof (AppContextSwitchOverrides)));

      public WebConfigManager.RuntimeElement.AssemblyBindingElement AssemblyBindings(
        string appliesTo = null)
      {
        return new WebConfigManager.RuntimeElement.AssemblyBindingElement(this.GetOrCreateElement(WebConfigManager.RuntimeElement.c_xmlns.GetName("assemblyBinding"), this.MakeName(nameof (appliesTo)), appliesTo));
      }

      public class AppContextSwitchOverridesElement : AppConfigElement
      {
        internal AppContextSwitchOverridesElement(XElement root)
          : base(root)
        {
        }
      }

      public class AssemblyBindingElement : AppConfigElement
      {
        internal AssemblyBindingElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.RuntimeElement.AssemblyBindingElement AddDependentAssembly(
          string name,
          string publicKeyToken,
          string oldVersion,
          string newVersion,
          string culture = null)
        {
          XElement content = this.MakeElement("dependentAssembly");
          XElement xelement1 = this.MakeElement("assemblyIdentity");
          xelement1.Add((object) this.MakeAttribute(nameof (name), name), (object) this.MakeAttribute(nameof (publicKeyToken), publicKeyToken));
          if (culture != null)
            xelement1.Add((object) this.MakeAttribute(nameof (culture), culture));
          XElement xelement2 = this.MakeElement("bindingRedirect");
          xelement2.Add((object) this.MakeAttribute(nameof (oldVersion), oldVersion), (object) this.MakeAttribute(nameof (newVersion), newVersion));
          content.Add((object) xelement1, (object) xelement2);
          this.Root.Add((object) content);
          return this;
        }

        public void RemoveDependentAssembly(string name)
        {
          XElement root = this.Root;
          if (root == null)
            return;
          IEnumerable<XElement> source1 = root.Elements();
          if (source1 == null)
            return;
          IEnumerable<XElement> source2 = source1.Where<XElement>((Func<XElement, bool>) (assemblyBinding => assemblyBinding?.Element(WebConfigManager.RuntimeElement.c_xmlns.GetName("assemblyIdentity"))?.Attribute((XName) nameof (name))?.Value == name));
          if (source2 == null)
            return;
          source2.Remove<XElement>();
        }

        protected override XElement MakeElement(string name) => new XElement(this.Root.Name.Namespace.GetName(name));
      }
    }

    public class UriElement : AppConfigElement
    {
      internal UriElement(XElement root)
        : base(root)
      {
      }

      public WebConfigManager.UriElement.SchemeSettingsElement SchemeSettings => new WebConfigManager.UriElement.SchemeSettingsElement(this.GetOrCreateElement("schemeSettings"));

      public class SchemeSettingsElement : AppConfigElement
      {
        internal SchemeSettingsElement(XElement root)
          : base(root)
        {
        }

        public WebConfigManager.UriElement.SchemeSettingsElement.SchemeSettingsItemElement Item(
          string name)
        {
          return new WebConfigManager.UriElement.SchemeSettingsElement.SchemeSettingsItemElement(this.GetOrCreateElement("add", nameof (name), name));
        }

        public class SchemeSettingsItemElement : AppConfigElement
        {
          internal SchemeSettingsItemElement(XElement root)
            : base(root)
          {
          }
        }
      }
    }
  }
}
