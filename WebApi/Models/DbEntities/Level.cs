using Models.Core;

namespace Models.DbEntities
{
    public class Level:SoftDeletable
    {
        public int LevelId { get; set; }
        public string Name { get; set; } = null!;

    }
}
