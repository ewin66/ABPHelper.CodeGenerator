using Microsoft.VisualStudio.Shell;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Runtime.InteropServices;
using ABPHelper.CodeGenerator.Templates;

namespace ABPHelper.CodeGenerator
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("1a406fba-71d5-462c-bada-82a009bc57a5")]
    public sealed class AbpHelperWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbpHelperWindow"/> class.
        /// </summary>
        public AbpHelperWindow() : base(null)
        {
            this.Caption = "AbpHelperWindow";
          //  InitRazorEngine();
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new AbpHelperWindowControl(this);
        }

        private void InitRazorEngine()
        {
            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new EmbeddedResourceTemplateManager(typeof(Template))
            };
            Engine.Razor = RazorEngineService.Create(config);
        }

    }
}
