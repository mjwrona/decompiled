// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.ExtensibilityProviders.TunneledCommandExtensionProvider
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Ssh.Server.ExtensibilityProviders
{
  public class TunneledCommandExtensionProvider : ITunneledCommandExtensionProvider
  {
    internal const string s_Area = "Ssh";
    internal const string s_Layer = "TunneledCommandExtensionProvider";

    public IDictionary<string, List<Type>> Extensions { get; set; }

    private static List<Type> m_extensionCache { get; } = new List<Type>();

    internal virtual IEnumerable<Type> LoadExtensions<T>(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(13000220, "Ssh", nameof (TunneledCommandExtensionProvider), nameof (LoadExtensions));
      try
      {
        if (TunneledCommandExtensionProvider.m_extensionCache != null && TunneledCommandExtensionProvider.m_extensionCache.Any<Type>())
          return (IEnumerable<Type>) TunneledCommandExtensionProvider.m_extensionCache;
        using (IDisposableReadOnlyList<T> extensions = requestContext.GetService<IVssExtensionManagementService>().GetExtensions<T>(requestContext))
        {
          List<Type> list = extensions.Select<T, Type>((Func<T, Type>) (extensionType => extensionType.GetType())).ToList<Type>();
          TunneledCommandExtensionProvider.m_extensionCache?.AddRange((IEnumerable<Type>) list);
          return (IEnumerable<Type>) list;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13000228, "Ssh", nameof (TunneledCommandExtensionProvider), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(13000229, "Ssh", nameof (TunneledCommandExtensionProvider), nameof (LoadExtensions));
      }
    }

    public void Initialize(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(13000200, "Ssh", nameof (TunneledCommandExtensionProvider), nameof (Initialize));
      try
      {
        this.Extensions = (IDictionary<string, List<Type>>) new Dictionary<string, List<Type>>();
        foreach (Type loadExtension in this.LoadExtensions<ITunneledCommandExtension>(requestContext))
        {
          TunneledCommandAttribute customAttribute = (TunneledCommandAttribute) Attribute.GetCustomAttribute((MemberInfo) loadExtension, typeof (TunneledCommandAttribute));
          if (customAttribute == null)
            requestContext.Trace(13000201, TraceLevel.Warning, "Ssh", nameof (TunneledCommandExtensionProvider), "Type {0} did not contain {1} attribute, could not map command", (object) loadExtension, (object) typeof (TunneledCommandAttribute));
          else if (customAttribute.SupportedCommands != null)
          {
            foreach (string supportedCommand in customAttribute.SupportedCommands)
            {
              if (!this.Extensions.ContainsKey(supportedCommand))
              {
                requestContext.Trace(13000202, TraceLevel.Info, "Ssh", nameof (TunneledCommandExtensionProvider), "Mapping command {0} to type {1}", (object) supportedCommand, (object) loadExtension);
                this.Extensions.Add(supportedCommand, new List<Type>()
                {
                  loadExtension
                });
              }
              else
                this.Extensions[supportedCommand].Add(loadExtension);
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13000208, "Ssh", nameof (TunneledCommandExtensionProvider), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(13000209, "Ssh", nameof (TunneledCommandExtensionProvider), nameof (Initialize));
      }
    }

    public ITunneledCommandExtension GetCommandExtension(
      IVssRequestContext requestContext,
      string command)
    {
      requestContext.TraceEnter(13000210, "Ssh", nameof (TunneledCommandExtensionProvider), nameof (GetCommandExtension));
      try
      {
        List<Type> typeList;
        if (!this.Extensions.TryGetValue(command, out typeList) || typeList.Count == 0)
        {
          requestContext.Trace(13001242, TraceLevel.Info, "Ssh", nameof (TunneledCommandExtensionProvider), "Command name {0} not supported, closing connection.", (object) command);
          throw new CommandExtensionNotFoundException(command);
        }
        Type type = typeList[0];
        requestContext.Trace(13001243, TraceLevel.Info, "Ssh", nameof (TunneledCommandExtensionProvider), "Command name {0} supported by extension {1}.", (object) command, (object) type);
        return (ITunneledCommandExtension) Activator.CreateInstance(type);
      }
      finally
      {
        requestContext.TraceLeave(13000219, "Ssh", nameof (TunneledCommandExtensionProvider), nameof (GetCommandExtension));
      }
    }
  }
}
