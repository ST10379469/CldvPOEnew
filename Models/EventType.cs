using Humanizer;
using static System.Net.Mime.MediaTypeNames;

namespace CldvPOEnew.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }
        public string Name { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}

//Referencing
//Asked chat do help me with filtering as this was one of the requiremets for the POE 
//smitpatel (2021). Querying Data - EF Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/ef/core/querying/basic [Accessed 23 Jun. 2025].

//In - text citation: (smitpatel, 2021)

//Ask chat to help me with error handling 
//tdykstra (2024). Handle errors in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling [Accessed 23 Jun. 2025].

//In - text citation: (tdykstra, 2024)