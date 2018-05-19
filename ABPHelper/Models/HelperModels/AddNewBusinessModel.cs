using ABPHelper.ViewModels;
using System.Collections.Generic;

namespace ABPHelper.Models.HelperModels
{
    public class AddNewBusinessModel
    {
        public string BusinessName { get; set; }

        public string ServiceFolder { get; set; }

        public string ServiceInterfaceName { get; set; }

        public string ServiceName { get; set; }

        public string ViewFolder { get; set; }

        public IEnumerable<AddNewBusinessViewModel.ViewFileViewModel> ViewFiles { get; set; }
    }
}