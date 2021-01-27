﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Composition;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Options;

// When building for source-build, there is no sqlite dependency
#if !DOTNET_BUILD_FROM_SOURCE
using Microsoft.CodeAnalysis.SQLite.v2;
#endif

namespace Microsoft.CodeAnalysis.Storage
{
    [ExportWorkspaceServiceFactory(typeof(IPersistentStorageService), ServiceLayer.Desktop), Shared]
    internal class PersistenceStorageServiceFactory : IWorkspaceServiceFactory
    {
#if !DOTNET_BUILD_FROM_SOURCE
        private readonly SQLiteConnectionPoolService _connectionPoolService;
#endif

        [ImportingConstructor]
        [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
        public PersistenceStorageServiceFactory(
#if !DOTNET_BUILD_FROM_SOURCE
            SQLiteConnectionPoolService connectionPoolService
#endif
            )
        {
#if !DOTNET_BUILD_FROM_SOURCE
            _connectionPoolService = connectionPoolService;
#endif
        }

        public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
        {
#if !DOTNET_BUILD_FROM_SOURCE
            var optionService = workspaceServices.GetRequiredService<IOptionService>();
            var database = optionService.GetOption(StorageOptions.Database);
            var mustSucceed = workspaceServices.Workspace.Options.GetOption(StorageOptions.DatabaseMustSucceed);

            var locationService = workspaceServices.GetService<IPersistentStorageLocationService>();
            if (locationService == null && mustSucceed)
                throw new InvalidOperationException($"Could not obtain {nameof(IPersistentStorageLocationService)}");

            if (locationService == null)
                return NoOpPersistentStorageService.Instance;

            switch (database)
            {
                case StorageDatabase.SQLite:
                    return new SQLitePersistentStorageService(_connectionPoolService, locationService);

                case StorageDatabase.CloudCache:
                    var provider = workspaceServices.GetService<ICloudCacheServiceProvider>();
                    if (provider == null && mustSucceed)
                        throw new InvalidOperationException($"Could not obtain {nameof(ICloudCacheServiceProvider)}");

                    return provider == null
                        ? NoOpPersistentStorageService.Instance
                        : new CloudCachePersistentStorageService(provider, locationService, mustSucceed);
            }
#endif

            return NoOpPersistentStorageService.Instance;
        }
    }
}
