// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.IDirectoryEntity
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Directories
{
  public interface IDirectoryEntity : IDirectoryEntityDescriptor
  {
    string EntityId { get; }

    new string EntityType { get; }

    new string EntityOrigin { get; }

    new string OriginDirectory { get; }

    new string OriginId { get; }

    new string LocalDirectory { get; }

    new string LocalId { get; }

    new string PrincipalName { get; }

    new string DisplayName { get; }

    string ScopeName { get; }

    string LocalDescriptor { get; }

    Microsoft.VisualStudio.Services.Common.SubjectDescriptor? SubjectDescriptor { get; }

    bool? Active { get; }
  }
}
