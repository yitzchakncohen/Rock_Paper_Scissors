namespace RockPaperScissors.Grids
{
    public interface IGridOccupantInterface 
    {
        public bool CanWalkOn(IGridOccupantInterface gridOccupantInterface);
        public bool IsBuilding();
        public bool IsFriendly();
    }
}
