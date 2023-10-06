using QFramework;

namespace Tabletop.Local
{
    /// <summary>
    /// 由裁判掌控游戏的进行
    /// </summary>
    public interface IReferee
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="grids"></param>
        /// <param name="map"></param>
        public void OnPieceDrop(LocalGridData grid, EasyGrid<LocalGridData> grids, LocalMapObj map);
    }
}
