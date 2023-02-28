using System.ComponentModel.DataAnnotations;
namespace PdfService;
public class DocumentModel
{
    public Guid DocumentNumber { get; set; }

    [StringLength(12)]
    // TODO: Should also check for only digits
    public string CustomerNumber { get; set; }
    public string DocumentText { get; set; }
}