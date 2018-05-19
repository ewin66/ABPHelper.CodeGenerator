using System;
using System.Windows.Controls;

namespace ABPHelper.CodeGenerator
{
    /// <summary>
    /// Interaction logic for AbpHelperWindowControl.
    /// </summary>
    public partial class AbpHelperWindowControl : UserControl
    {
        public static IServiceProvider ServiceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpHelperWindowControl"/> class.
        /// </summary>
        public AbpHelperWindowControl(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            ServiceProvider = serviceProvider;
        }
    }
}