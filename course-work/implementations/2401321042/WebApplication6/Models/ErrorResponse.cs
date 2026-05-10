namespace WebApplication6.Models
{
    public class ErrorResponse
    {
        public int Id { get; set; }
        public int Status { get; set; }        // 404, 500 и т.н.
        public string Title { get; set; }      // "Not Found"
        public string Detail { get; set; }     // конкретното съобщение
        public string Instance { get; set; }   // кой URL е причинил грешката

    }
}
