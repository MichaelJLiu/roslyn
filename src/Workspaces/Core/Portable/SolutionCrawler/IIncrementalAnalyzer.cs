﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Options;

namespace Microsoft.CodeAnalysis.SolutionCrawler
{
    internal interface IIncrementalAnalyzer
    {
        Task AnalyzeDocumentAsync(Document document, SyntaxNode bodyOpt, InvocationReasons reasons, CancellationToken cancellationToken);
#if false
        Task AnalyzeSyntaxAsync(Document document, InvocationReasons reasons, CancellationToken cancellationToken);
        Task DocumentCloseAsync(Document document, CancellationToken cancellationToken);
        Task DocumentOpenAsync(Document document, CancellationToken cancellationToken);
        Task ActiveDocumentSwitchedAsync(TextDocument document, CancellationToken cancellationToken);
        Task NewSolutionSnapshotAsync(Solution solution, CancellationToken cancellationToken);
        Task AnalyzeProjectAsync(Project project, bool semanticsChanged, InvocationReasons reasons, CancellationToken cancellationToken);

        Task RemoveDocumentAsync(DocumentId documentId, CancellationToken cancellationToken);
        Task RemoveProjectAsync(ProjectId projectId, CancellationToken cancellationToken);

        Task NonSourceDocumentOpenAsync(TextDocument textDocument, CancellationToken cancellationToken);
        Task NonSourceDocumentCloseAsync(TextDocument textDocument, CancellationToken cancellationToken);

        Task AnalyzeNonSourceDocumentAsync(TextDocument textDocument, InvocationReasons reasons, CancellationToken cancellationToken);
        void LogAnalyzerCountSummary();
#endif

        void Shutdown();
    }
}
