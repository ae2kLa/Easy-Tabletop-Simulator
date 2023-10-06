using QFramework;

namespace Tabletop.Local
{
    public interface IReferee
    {
        public bool CheckWin(GoChessColor color, LocalGridData grid, EasyGrid<LocalGridData> grids);
    }
}
