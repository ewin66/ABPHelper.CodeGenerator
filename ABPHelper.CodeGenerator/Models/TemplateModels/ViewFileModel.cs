﻿using System.Collections.Generic;
using ABPHelper.CodeGenerator.ViewModels;

namespace ABPHelper.CodeGenerator.Models.TemplateModels
{
    public class ViewFileModel
    {
        public string BusinessName { get; set; }

        public string ViewFolder { get; set; }

        public string Namespace { get; set; }

        public string FileName { get; set; }

        public bool IsPopup { get; set; }

        public IEnumerable<AddNewBusinessViewModel.ViewFileViewModel> ViewFiles { get; set; }
    }
}