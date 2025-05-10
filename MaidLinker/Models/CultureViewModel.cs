using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaidLinker.Models
{
    public class CultureViewModel
    {
        public List<SelectListItem> AvailableCultures { get; set; }
        public string SelectedCulture { get; set; }
    }
}
