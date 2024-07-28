// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public abstract class DirectoryRequest
  {
    public IEnumerable<string> Directories { get; set; }

    public IEnumerable<string> Preferences { get; set; }

    internal virtual void Validate()
    {
    }

    internal abstract DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories);

    protected string[] SanitizeDirectories() => this.Directories == null ? Array.Empty<string>() : new HashSet<string>(this.Directories).ToArray<string>();

    protected string[] SanitizeAndFilterDirectories(IVssRequestContext context)
    {
      string[] directories = this.SanitizeDirectories();
      return DirectoryOperationHelper.FilterDirectoriesForAadGuestUsers(context, directories);
    }

    protected string[] SanitizePreferences() => this.Preferences == null ? Array.Empty<string>() : new HashSet<string>(this.Preferences).ToArray<string>();
  }
}
