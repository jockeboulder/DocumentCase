using System.ComponentModel.DataAnnotations;
namespace PdfService;
// Ideally this would be a shared model to use when calling APIs
public class DocumentModel
{
    public Guid DocumentNumber { get; set; }

    [StringLength(12)]
    // TODO: Should also check for only digits
    public string CustomerNumber { get; set; }
    public string DocumentText { get; set; }
}