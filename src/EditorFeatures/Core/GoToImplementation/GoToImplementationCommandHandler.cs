﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Editor.CommandHandlers;
using Microsoft.CodeAnalysis.Editor.Commanding.Commands;
using Microsoft.CodeAnalysis.Editor.FindUsages;
using Microsoft.CodeAnalysis.Editor.Host;
using Microsoft.CodeAnalysis.Editor.Shared.Utilities;
using Microsoft.CodeAnalysis.FindUsages;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Internal.Log;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.SymbolMapping;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.CodeAnalysis.Editor.GoToImplementation
{
    [Export(typeof(ICommandHandler))]
    [ContentType(ContentTypeNames.RoslynContentType)]
    [Name(PredefinedCommandHandlerNames.GoToImplementation)]
    internal class GoToImplementationCommandHandler : AbstractGoToCommandHandler<IFindUsagesService, GoToImplementationCommandArgs>
    {
        [ImportingConstructor]
        [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
        public GoToImplementationCommandHandler(
            IThreadingContext threadingContext,
            IStreamingFindUsagesPresenter streamingPresenter) : base(threadingContext, streamingPresenter)
        {
        }

        public override string DisplayName => EditorFeaturesResources.Go_To_Implementation;

        protected override string ScopeDescription => EditorFeaturesResources.Locating_implementations;

        protected override FunctionId FunctionId => FunctionId.CommandHandler_GoToImplementation;

        protected override Task FindActionAsync(IFindUsagesService service, Document document, int caretPosition, IFindUsagesContext context, CancellationToken cancellationToken)
            => service.FindImplementationsAsync(document, caretPosition, context, cancellationToken);

        protected override IFindUsagesService? GetService(Document? document)
            => document?.GetLanguageService<IFindUsagesService>();

        protected override async Task<Solution> GetSolutionAsync(Document document, CancellationToken cancellationToken)
        {
            // The original document we may be starting with might be in a metadata-as-source workspace.
            // Ensure that we map back to the real primary workspace to present symbols in so we can
            // navigate back to source implementations there.
            var mappingService = document.Project.Solution.Workspace.Services.GetRequiredService<ISymbolMappingService>();
            var mappedProject = await mappingService.MapDocumentAsync(document, cancellationToken).ConfigureAwait(false);
            return mappedProject?.Solution ?? document.Project.Solution;
        }
    }
}
