
using Models.Core;

namespace Models.DbEntities
{
    public class Language:SoftDeletable
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = null!;


    }
}
