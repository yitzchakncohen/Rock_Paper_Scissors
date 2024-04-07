namespace RockPaperScissors.Grids
{
    public interface IGridOccupantInterface 
    {
        public bool CanWalkOn(IGridOccupantInterface gridOccupantInterface);
        public bool IsBuilding {get; set;}
        public bool IsFriendly {get; set;}
    }
}
