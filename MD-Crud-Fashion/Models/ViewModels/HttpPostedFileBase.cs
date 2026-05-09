
namespace MD_Crud_Fashion.Models.ViewModels
{
    public class HttpPostedFileBase
    {
        public string? FileName { get; internal set; }

        internal void SaveAs(object value)
        {
            throw new NotImplementedException();
        }
    }
}