using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.ComponentModelHost;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Threading;

namespace Allmhuran.Ssms
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GoFold
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("ee7e94c4-2d2b-4213-961f-12c1d4941d95");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoFold"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GoFold(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GoFold Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GoFold's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new GoFold(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            try
            {
                var textManager = (IVsTextManager)await ServiceProvider.GetServiceAsync(typeof(SVsTextManager));
                var componentModel = (IComponentModel)await ServiceProvider.GetServiceAsync(typeof(SComponentModel));
                var editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                if (textManager == null || componentModel == null) return;

                textManager.GetActiveView(1, null, out IVsTextView view);

                if (view == null) return;

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var dte = (EnvDTE.DTE) Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));

                IVsTextLines lines;
                view.GetBuffer(out lines);
                int lineCount;
                lines.GetLineCount(out lineCount);
                int blockStart = -1;
                for (int i = 0; i < lineCount; i++)
                {
                    lines.GetLengthOfLine(i, out int length);
                    if (length == 0) continue;
                    lines.GetLineText(i, 0, i, length, out string text);
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    if (length == 2 && text.Equals("GO", StringComparison.OrdinalIgnoreCase))
                    {
                        if (blockStart != -1)
                        {
                            if (i - blockStart > 2)
                            {
                                view.SetSelection(blockStart + 1, 0, i, 2);
                                dte.ExecuteCommand("Edit.HideSelection");
                            }
                            blockStart = -1;
                        }
                    }
                    else if (blockStart == -1) blockStart = i;
                }
            }
            catch (Exception x)
            {
                VsShellUtilities.ShowMessageBox
                (
                    this.package,
                    x.Message,
                    "GoFold non-fatal error",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
            }
        }
    }
}
