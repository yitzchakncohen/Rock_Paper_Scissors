namespace RockPaperScissors.Grids
{
    public interface IGridOccupantInterface 
    {
        public bool CanWalkOnGridOccupant(IGridOccupantInterface gridOccupantInterface);
        public bool IsBuilding {get; set;}
        public bool IsFriendly {get; set;}
    }
}
